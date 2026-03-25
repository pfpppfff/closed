using System;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace 串口通讯
{
    /// <summary>
    /// 通讯服务类，包含串口Modbus自由口通讯、RTU通讯和Modbus TCP通讯功能
    /// </summary>
    public class CommunicationService : IDisposable
    {
        #region 字段和属性

        // 串口相关
        private SerialPort _serialPort;
        private bool _isSerialConnected;
        
        // 用于存储接收到的数据
        private Queue<byte> _receivedDataQueue = new Queue<byte>();
        private object _dataLock = new object();
        
        // 数据接收统计
        private long _totalReceivedBytes = 0;
        private int _lastReceivedLength = 0;
        private object _statsLock = new object();

        // TCP相关
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private bool _isTcpConnected;
        private Queue<byte> _tcpReceivedDataQueue = new Queue<byte>();
        private object _tcpDataLock = new object();
        private System.Threading.CancellationTokenSource _tcpCancellationSource;

        // 公共属性
        public bool IsSerialConnected => _isSerialConnected;
        public bool IsTcpConnected => _isTcpConnected;
        
        /// <summary>
        /// 获取总接收数据字节数
        /// </summary>
        public long TotalReceivedBytes
        {
            get
            {
                lock (_statsLock)
                {
                    return _totalReceivedBytes;
                }
            }
        }
        
        /// <summary>
        /// 获取最后一次接收的数据长度
        /// </summary>
        public int LastReceivedLength
        {
            get
            {
                lock (_statsLock)
                {
                    return _lastReceivedLength;
                }
            }
        }
        
        /// <summary>
        /// 重置接收数据统计
        /// </summary>
        public void ResetReceivedBytesCounter()
        {
            lock (_statsLock)
            {
                _totalReceivedBytes = 0;
                _lastReceivedLength = 0;
            }
        }

        #endregion

        #region 构造函数和析构函数

        public CommunicationService()
        {
            // 初始化串口
            _serialPort = new SerialPort();
            _serialPort.DataReceived += SerialPort_DataReceived;

            // 初始化TCP客户端
            _tcpClient = new TcpClient();
        }

        public void Dispose()
        {
            // 释放串口资源
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _serialPort.Dispose();
            }

            // 释放TCP资源
            DisconnectTcp();

            // 释放网络流
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
        }

        #endregion

        #region 串口Modbus自由口通讯

        /// <summary>
        /// 打开串口连接
        /// </summary>
        /// <param name="portName">串口名称，如"COM1"</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="parity">奇偶校验</param>
        /// <param name="stopBits">停止位</param>
        /// <returns>是否成功打开</returns>
        public bool OpenSerialPort(string portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                _serialPort.PortName = portName;
                _serialPort.BaudRate = baudRate;
                _serialPort.DataBits = dataBits;
                _serialPort.Parity = parity;
                _serialPort.StopBits = stopBits;

                _serialPort.Open();
                _isSerialConnected = _serialPort.IsOpen;
                return _isSerialConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"打开串口失败: {ex.Message}");
                _isSerialConnected = false;
                return false;
            }
        }

        /// <summary>
        /// 关闭串口连接
        /// </summary>
        public void CloseSerialPort()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _isSerialConnected = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"关闭串口失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送数据到串口
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns>是否发送成功</returns>
        public bool SendSerialData(byte[] data)
        {
            try
            {
                if (!_isSerialConnected || !_serialPort.IsOpen)
                    return false;

                _serialPort.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"串口发送数据失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 异步发送数据到串口
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns>是否发送成功</returns>
        public async Task<bool> SendSerialDataAsync(byte[] data)
        {
            return await Task.Run(() => SendSerialData(data));
        }

        /// <summary>
        /// 串口数据接收事件
        /// </summary>
        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                int dataLength = sp.BytesToRead;
                byte[] data = new byte[dataLength];
                sp.Read(data, 0, dataLength);

                // 将接收到的数据存储到队列中
                lock (_dataLock)
                {
                    foreach (byte b in data)
                    {
                        _receivedDataQueue.Enqueue(b);
                    }
                }
                
                // 更新接收数据统计
                lock (_statsLock)
                {
                    _totalReceivedBytes += dataLength;
                    _lastReceivedLength = dataLength;
                }

                // 触发数据接收事件
                OnSerialDataReceived?.Invoke(this, new SerialDataReceivedEventArgs(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"串口接收数据异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 读取接收到的数据
        /// </summary>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>接收到的数据</returns>
        public async Task<byte[]> ReadReceivedDataAsync(int timeoutMs = 1000)
        {
            // 等待数据到达
            var tcs = new TaskCompletionSource<bool>();
            var timer = new Timer(_ => tcs.SetResult(true), null, timeoutMs, Timeout.Infinite);
            
            try
            {
                // 等待直到有数据或超时
                while (true)
                {
                    lock (_dataLock)
                    {
                        if (_receivedDataQueue.Count > 0)
                        {
                            byte[] data = _receivedDataQueue.ToArray();
                            _receivedDataQueue.Clear();
                            return data;
                        }
                    }
                    
                    // 短暂延迟以避免过度占用CPU
                    await Task.Delay(10);
                    
                    // 检查是否超时
                    if (tcs.Task.IsCompleted)
                        break;
                }
                
                // 超时返回空数组
                return new byte[0];
            }
            finally
            {
                timer.Dispose();
            }
        }

        /// <summary>
        /// 串口数据接收事件委托
        /// </summary>
        public event EventHandler<SerialDataReceivedEventArgs> OnSerialDataReceived;

        #endregion

        #region Modbus RTU通讯

        /// <summary>
        /// 计算CRC校验码
        /// </summary>
        /// <param name="data">要计算CRC的数据</param>
        /// <param name="length">数据长度</param>
        /// <returns>CRC校验码</returns>
        private ushort CalculateCRC(byte[] data, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                crc ^= data[i];

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        /// <summary>
        /// 发送Modbus RTU读取保持寄存器命令
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numberOfPoints">读取数量</param>
        /// <returns>返回的字节数组</returns>
        public async Task<byte[]> ReadHoldingRegistersRtu(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                // 清空接收队列中的旧数据
                lock (_dataLock)
                {
                    _receivedDataQueue.Clear();
                }

                // 构建请求帧
                byte[] request = new byte[8];
                request[0] = slaveAddress;           // 从站地址
                request[1] = 0x03;                   // 功能码：读取保持寄存器
                request[2] = (byte)(startAddress >> 8);  // 起始地址高字节
                request[3] = (byte)(startAddress & 0xFF); // 起始地址低字节
                request[4] = (byte)(numberOfPoints >> 8); // 寄存器数量高字节
                request[5] = (byte)(numberOfPoints & 0xFF); // 寄存器数量低字节

                // 计算CRC
                ushort crc = CalculateCRC(request, 6);
                request[6] = (byte)(crc & 0xFF);     // CRC低字节
                request[7] = (byte)(crc >> 8);       // CRC高字节

                // 发送请求
                if (!SendSerialData(request))
                    return null;

                // 等待并读取响应
                byte[] response = await ReadReceivedDataAsync(1000);
                return response.Length > 0 ? response : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RTU读取保持寄存器失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发送Modbus RTU读取保持寄存器命令并解析为short数组
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numberOfPoints">读取数量</param>
        /// <returns>返回的short数组</returns>
        public async Task<short[]> ReadHoldingRegistersAsShortRtu(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                byte[] response = await ReadHoldingRegistersRtu(slaveAddress, startAddress, numberOfPoints);
                if (response == null || response.Length < 5) // 最少5字节：地址(1) + 功能码(1) + 字节数(1) + 至少1个寄存器(2) + CRC(2)
                    return null;

                // 检查CRC
                if (response.Length >= 4)
                {
                    ushort receivedCrc = (ushort)(response[response.Length - 1] << 8 | response[response.Length - 2]);
                    ushort calculatedCrc = CalculateCRC(response, response.Length - 2);
                    if (receivedCrc != calculatedCrc)
                    {
                        Console.WriteLine("RTU读取保持寄存器CRC校验失败");
                        return null;
                    }
                }

                // 解析数据
                int byteCount = response[2];
                int registerCount = byteCount / 2;
                short[] result = new short[registerCount];

                for (int i = 0; i < registerCount; i++)
                {
                    // Modbus使用大端序
                    result[i] = (short)(response[3 + i * 2] << 8 | response[4 + i * 2]);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RTU读取保持寄存器解析为short失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发送Modbus RTU读取保持寄存器命令并解析为float数组
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numberOfPoints">读取数量（必须是2的倍数，因为每个float需要2个寄存器）</param>
        /// <returns>返回的float数组</returns>
        public async Task<float[]> ReadHoldingRegistersAsFloatRtu(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                // 确保读取的寄存器数量是2的倍数
                if (numberOfPoints % 2 != 0)
                {
                    Console.WriteLine("RTU读取float数据时，寄存器数量必须是2的倍数");
                    return null;
                }

                byte[] response = await ReadHoldingRegistersRtu(slaveAddress, startAddress, numberOfPoints);
                if (response == null || response.Length < 5)
                    return null;

                // 检查CRC
                if (response.Length >= 4)
                {
                    ushort receivedCrc = (ushort)(response[response.Length - 1] << 8 | response[response.Length - 2]);
                    ushort calculatedCrc = CalculateCRC(response, response.Length - 2);
                    if (receivedCrc != calculatedCrc)
                    {
                        Console.WriteLine("RTU读取保持寄存器CRC校验失败");
                        return null;
                    }
                }

                // 解析数据
                int byteCount = response[2];
                int floatCount = byteCount / 4; // 每个float需要4个字节（2个寄存器）
                float[] result = new float[floatCount];

                for (int i = 0; i < floatCount; i++)
                {
                    // 构造4字节数据（大端序）
                    byte[] floatBytes = new byte[4];
                    floatBytes[0] = response[3 + i * 4];
                    floatBytes[1] = response[4 + i * 4];
                    floatBytes[2] = response[5 + i * 4];
                    floatBytes[3] = response[6 + i * 4];

                    // 如果系统是小端序，需要反转字节序
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(floatBytes);

                    result[i] = BitConverter.ToSingle(floatBytes, 0);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RTU读取保持寄存器解析为float失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发送Modbus RTU读取输入寄存器命令
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numberOfPoints">读取数量</param>
        /// <returns>返回的字节数组</returns>
        public async Task<byte[]> ReadInputRegistersRtu(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                // 清空接收队列中的旧数据
                lock (_dataLock)
                {
                    _receivedDataQueue.Clear();
                }

                // 构建请求帧
                byte[] request = new byte[8];
                request[0] = slaveAddress;           // 从站地址
                request[1] = 0x04;                   // 功能码：读取输入寄存器
                request[2] = (byte)(startAddress >> 8);  // 起始地址高字节
                request[3] = (byte)(startAddress & 0xFF); // 起始地址低字节
                request[4] = (byte)(numberOfPoints >> 8); // 寄存器数量高字节
                request[5] = (byte)(numberOfPoints & 0xFF); // 寄存器数量低字节

                // 计算CRC
                ushort crc = CalculateCRC(request, 6);
                request[6] = (byte)(crc & 0xFF);     // CRC低字节
                request[7] = (byte)(crc >> 8);       // CRC高字节

                // 发送请求
                if (!SendSerialData(request))
                    return null;

                // 等待并读取响应
                byte[] response = await ReadReceivedDataAsync(1000);
                return response.Length > 0 ? response : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RTU读取输入寄存器失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发送Modbus RTU读取线圈命令
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numberOfPoints">读取数量</param>
        /// <returns>返回的字节数组</returns>
        public async Task<byte[]> ReadCoilsRtu(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                // 清空接收队列中的旧数据
                lock (_dataLock)
                {
                    _receivedDataQueue.Clear();
                }

                // 构建请求帧
                byte[] request = new byte[8];
                request[0] = slaveAddress;           // 从站地址
                request[1] = 0x01;                   // 功能码：读取线圈
                request[2] = (byte)(startAddress >> 8);  // 起始地址高字节
                request[3] = (byte)(startAddress & 0xFF); // 起始地址低字节
                request[4] = (byte)(numberOfPoints >> 8); // 线圈数量高字节
                request[5] = (byte)(numberOfPoints & 0xFF); // 线圈数量低字节

                // 计算CRC
                ushort crc = CalculateCRC(request, 6);
                request[6] = (byte)(crc & 0xFF);     // CRC低字节
                request[7] = (byte)(crc >> 8);       // CRC高字节

                // 发送请求
                if (!SendSerialData(request))
                    return null;

                // 等待并读取响应
                byte[] response = await ReadReceivedDataAsync(1000);
                return response.Length > 0 ? response : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RTU读取线圈失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发送Modbus RTU写单个线圈命令
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="coilAddress">线圈地址</param>
        /// <param name="value">线圈值（true为ON，false为OFF）</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteSingleCoilRtu(byte slaveAddress, ushort coilAddress, bool value)
        {
            try
            {
                // 清空接收队列中的旧数据
                lock (_dataLock)
                {
                    _receivedDataQueue.Clear();
                }

                // 构建请求帧
                byte[] request = new byte[8];
                request[0] = slaveAddress;               // 从站地址
                request[1] = 0x05;                       // 功能码：写单个线圈
                request[2] = (byte)(coilAddress >> 8);   // 线圈地址高字节
                request[3] = (byte)(coilAddress & 0xFF); // 线圈地址低字节
                request[4] = (byte)(value ? 0xFF : 0x00); // 值高字节（FF00为ON，0000为OFF）
                request[5] = 0x00;                       // 值低字节

                // 计算CRC
                ushort crc = CalculateCRC(request, 6);
                request[6] = (byte)(crc & 0xFF);         // CRC低字节
                request[7] = (byte)(crc >> 8);           // CRC高字节

                // 发送请求
                if (!SendSerialData(request))
                    return false;

                // 等待并读取响应
                byte[] response = await ReadReceivedDataAsync(1000);
                return response != null && response.Length >= 8;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RTU写单个线圈失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 发送Modbus RTU写单个保持寄存器命令
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">写入值</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteSingleRegisterRtu(byte slaveAddress, ushort registerAddress, ushort value)
        {
            try
            {
                // 清空接收队列中的旧数据
                lock (_dataLock)
                {
                    _receivedDataQueue.Clear();
                }

                // 构建请求帧
                byte[] request = new byte[8];
                request[0] = slaveAddress;               // 从站地址
                request[1] = 0x06;                       // 功能码：写单个保持寄存器
                request[2] = (byte)(registerAddress >> 8);   // 寄存器地址高字节
                request[3] = (byte)(registerAddress & 0xFF); // 寄存器地址低字节
                request[4] = (byte)(value >> 8);         // 写入值高字节
                request[5] = (byte)(value & 0xFF);       // 写入值低字节

                // 计算CRC
                ushort crc = CalculateCRC(request, 6);
                request[6] = (byte)(crc & 0xFF);         // CRC低字节
                request[7] = (byte)(crc >> 8);           // CRC高字节

                // 发送请求
                if (!SendSerialData(request))
                    return false;

                // 等待并读取响应
                byte[] response = await ReadReceivedDataAsync(1000);
                return response.Length > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RTU写单个寄存器失败: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Modbus TCP通讯

        /// <summary>
        /// 连接到Modbus TCP服务器
        /// </summary>
        /// <param name="ipAddress">服务器IP地址</param>
        /// <param name="port">端口号，默认502</param>
        /// <returns>是否连接成功</returns>
        public async Task<bool> ConnectToTcpServer(string ipAddress, int port = 502)
        {
            try
            {
                if (_isTcpConnected)
                {
                    DisconnectTcp();
                }

                await _tcpClient.ConnectAsync(ipAddress, port);
                _networkStream = _tcpClient.GetStream();
                _isTcpConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP连接失败: {ex.Message}");
                _isTcpConnected = false;
                return false;
            }
        }

        /// <summary>
        /// 断开TCP连接
        /// </summary>
        public void DisconnectTcp()
        {
            try
            {
                _networkStream?.Close();
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    _tcpClient.Close();
                }
                _isTcpConnected = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"断开TCP连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送Modbus TCP读取保持寄存器命令
        /// </summary>
        /// <param name="slaveId">从站ID</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numberOfPoints">读取数量</param>
        /// <returns>返回的字节数组</returns>
        public async Task<byte[]> ReadHoldingRegistersTcp(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (!_isTcpConnected || !_tcpClient.Connected)
                {
                    Console.WriteLine("TCP未连接");
                    return null;
                }

                // 构建MBAP头和PDU
                byte[] request = new byte[12];
                byte[] transactionId = BitConverter.GetBytes((ushort)1); // 事务处理标识符
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(transactionId);

                request[0] = transactionId[0];     // 事务处理标识符高字节
                request[1] = transactionId[1];     // 事务处理标识符低字节
                request[2] = 0x00;                 // 协议标识符高字节
                request[3] = 0x00;                 // 协议标识符低字节
                request[4] = 0x00;                 // 长度高字节
                request[5] = 0x06;                 // 长度低字节（后面数据的字节数）
                request[6] = slaveId;              // 单元标识符
                request[7] = 0x03;                 // 功能码：读取保持寄存器
                request[8] = (byte)(startAddress >> 8);   // 起始地址高字节
                request[9] = (byte)(startAddress & 0xFF); // 起始地址低字节
                request[10] = (byte)(numberOfPoints >> 8); // 寄存器数量高字节
                request[11] = (byte)(numberOfPoints & 0xFF); // 寄存器数量低字节

                // 发送请求
                await _networkStream.WriteAsync(request, 0, request.Length);

                // 接收响应
                byte[] response = new byte[1024];
                int bytesRead = await _networkStream.ReadAsync(response, 0, response.Length);

                // 返回有效数据
                byte[] result = new byte[bytesRead];
                Array.Copy(response, result, bytesRead);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP读取保持寄存器失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发送Modbus TCP写单个保持寄存器命令
        /// </summary>
        /// <param name="slaveId">从站ID</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">写入值</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteSingleRegisterTcp(byte slaveId, ushort registerAddress, ushort value)
        {
            try
            {
                if (!_isTcpConnected || !_tcpClient.Connected)
                {
                    Console.WriteLine("TCP未连接");
                    return false;
                }

                // 构建MBAP头和PDU
                byte[] request = new byte[12];
                byte[] transactionId = BitConverter.GetBytes((ushort)1); // 事务处理标识符
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(transactionId);

                request[0] = transactionId[0];     // 事务处理标识符高字节
                request[1] = transactionId[1];     // 事务处理标识符低字节
                request[2] = 0x00;                 // 协议标识符高字节
                request[3] = 0x00;                 // 协议标识符低字节
                request[4] = 0x00;                 // 长度高字节
                request[5] = 0x06;                 // 长度低字节（后面数据的字节数）
                request[6] = slaveId;              // 单元标识符
                request[7] = 0x06;                 // 功能码：写单个保持寄存器
                request[8] = (byte)(registerAddress >> 8);   // 寄存器地址高字节
                request[9] = (byte)(registerAddress & 0xFF); // 寄存器地址低字节
                request[10] = (byte)(value >> 8);    // 写入值高字节
                request[11] = (byte)(value & 0xFF);  // 写入值低字节

                // 发送请求
                await _networkStream.WriteAsync(request, 0, request.Length);

                // 接收响应
                byte[] response = new byte[1024];
                int bytesRead = await _networkStream.ReadAsync(response, 0, response.Length);

                // 检查响应是否正确（简单检查）
                if (bytesRead >= 12 && response[7] == 0x06 && response[8] == request[8] && response[9] == request[9])
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP写单个寄存器失败: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region TCP自由口通讯

        /// <summary>
        /// 连接到TCP自由口服务器
        /// </summary>
        /// <param name="ipAddress">服务器IP地址</param>
        /// <param name="port">端口号</param>
        /// <returns>是否连接成功</returns>
        public async Task<bool> ConnectToTcpFreePort(string ipAddress, int port)
        {
            try
            {
                if (_isTcpConnected)
                {
                    DisconnectTcpFreePort();
                }

                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ipAddress, port);
                _networkStream = _tcpClient.GetStream();
                _isTcpConnected = true;
                
                // 启动接收线程
                _tcpCancellationSource = new System.Threading.CancellationTokenSource();
                _ = Task.Run(() => TcpReceiveLoop(_tcpCancellationSource.Token));
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP连接失败: {ex.Message}");
                _isTcpConnected = false;
                return false;
            }
        }

        /// <summary>
        /// 断开TCP自由口连接
        /// </summary>
        public void DisconnectTcpFreePort()
        {
            try
            {
                _isTcpConnected = false;
                
                // 取消接收线程
                _tcpCancellationSource?.Cancel();
                
                _networkStream?.Close();
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    _tcpClient.Close();
                }
                
                _tcpCancellationSource?.Dispose();
                _tcpCancellationSource = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"断开TCP连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// TCP接收循环
        /// </summary>
        private async Task TcpReceiveLoop(System.Threading.CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];
            
            try
            {
                while (!cancellationToken.IsCancellationRequested && _isTcpConnected && _tcpClient != null && _tcpClient.Connected)
                {
                    if (_networkStream != null && _networkStream.DataAvailable)
                    {
                        int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        
                        if (bytesRead > 0)
                        {
                            byte[] data = new byte[bytesRead];
                            Array.Copy(buffer, data, bytesRead);
                            
                            // 存储到队列
                            lock (_tcpDataLock)
                            {
                                foreach (byte b in data)
                                {
                                    _tcpReceivedDataQueue.Enqueue(b);
                                }
                            }
                            
                            // 更新统计
                            lock (_statsLock)
                            {
                                _totalReceivedBytes += bytesRead;
                                _lastReceivedLength = bytesRead;
                            }
                            
                            // 触发事件
                            OnTcpDataReceived?.Invoke(this, new TcpDataReceivedEventArgs(data));
                        }
                    }
                    
                    await Task.Delay(10, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP接收数据异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送TCP数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns>是否发送成功</returns>
        public async Task<bool> SendTcpDataAsync(byte[] data)
        {
            try
            {
                if (!_isTcpConnected || _networkStream == null || !_tcpClient.Connected)
                    return false;

                await _networkStream.WriteAsync(data, 0, data.Length);
                await _networkStream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP发送数据失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 读取TCP接收到的数据
        /// </summary>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>接收到的数据</returns>
        public async Task<byte[]> ReadTcpReceivedDataAsync(int timeoutMs = 1000)
        {
            var tcs = new TaskCompletionSource<bool>();
            var timer = new Timer(_ => tcs.SetResult(true), null, timeoutMs, Timeout.Infinite);
            
            try
            {
                while (true)
                {
                    lock (_tcpDataLock)
                    {
                        if (_tcpReceivedDataQueue.Count > 0)
                        {
                            byte[] data = _tcpReceivedDataQueue.ToArray();
                            _tcpReceivedDataQueue.Clear();
                            return data;
                        }
                    }
                    
                    await Task.Delay(10);
                    
                    if (tcs.Task.IsCompleted)
                        break;
                }
                
                return new byte[0];
            }
            finally
            {
                timer.Dispose();
            }
        }

        /// <summary>
        /// 发送TCP字符串数据
        /// </summary>
        /// <param name="text">要发送的字符串</param>
        /// <param name="encoding">编码方式，默认UTF8</param>
        /// <returns>是否发送成功</returns>
        public async Task<bool> SendTcpStringAsync(string text, Encoding encoding = null)
        {
            try
            {
                if (!_isTcpConnected || _networkStream == null || !_tcpClient.Connected)
                    return false;

                encoding = encoding ?? Encoding.UTF8;
                byte[] data = encoding.GetBytes(text);
                
                await _networkStream.WriteAsync(data, 0, data.Length);
                await _networkStream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP发送字符串失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 发送TCP字符串数据并添加结束符
        /// </summary>
        /// <param name="text">要发送的字符串</param>
        /// <param name="terminator">结束符，如"\r\n"</param>
        /// <param name="encoding">编码方式，默认UTF8</param>
        /// <returns>是否发送成功</returns>
        public async Task<bool> SendTcpStringWithTerminatorAsync(string text, string terminator = "\r\n", Encoding encoding = null)
        {
            return await SendTcpStringAsync(text + terminator, encoding);
        }

        /// <summary>
        /// 读取TCP接收到的字符串数据
        /// </summary>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <param name="encoding">编码方式，默认UTF8</param>
        /// <returns>接收到的字符串</returns>
        public async Task<string> ReadTcpStringAsync(int timeoutMs = 1000, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] data = await ReadTcpReceivedDataAsync(timeoutMs);
            
            if (data != null && data.Length > 0)
            {
                return encoding.GetString(data);
            }
            
            return string.Empty;
        }

        /// <summary>
        /// 读取TCP接收到的字符串直到指定结束符
        /// </summary>
        /// <param name="terminator">结束符，如"\r\n"</param>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <param name="encoding">编码方式，默认UTF8</param>
        /// <returns>接收到的字符串（不包含结束符）</returns>
        public async Task<string> ReadTcpStringUntilAsync(string terminator = "\r\n", int timeoutMs = 1000, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            StringBuilder sb = new StringBuilder();
            DateTime startTime = DateTime.Now;
            
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                lock (_tcpDataLock)
                {
                    if (_tcpReceivedDataQueue.Count > 0)
                    {
                        byte[] data = _tcpReceivedDataQueue.ToArray();
                        string received = encoding.GetString(data);
                        
                        int terminatorIndex = received.IndexOf(terminator);
                        if (terminatorIndex >= 0)
                        {
                            // 找到结束符，提取消息
                            string message = received.Substring(0, terminatorIndex);
                            
                            // 计算需要移除的字节数
                            int bytesToRemove = encoding.GetByteCount(received.Substring(0, terminatorIndex + terminator.Length));
                            for (int i = 0; i < bytesToRemove && _tcpReceivedDataQueue.Count > 0; i++)
                            {
                                _tcpReceivedDataQueue.Dequeue();
                            }
                            
                            return message;
                        }
                    }
                }
                
                await Task.Delay(10);
            }
            
            // 超时，返回当前缓冲区的内容
            lock (_tcpDataLock)
            {
                if (_tcpReceivedDataQueue.Count > 0)
                {
                    byte[] data = _tcpReceivedDataQueue.ToArray();
                    _tcpReceivedDataQueue.Clear();
                    return encoding.GetString(data);
                }
            }
            
            return string.Empty;
        }

        /// <summary>
        /// 发送字符串并等待响应
        /// </summary>
        /// <param name="text">要发送的字符串</param>
        /// <param name="timeoutMs">等待响应超时时间（毫秒）</param>
        /// <param name="encoding">编码方式，默认UTF8</param>
        /// <returns>响应字符串</returns>
        public async Task<string> SendAndReceiveTcpStringAsync(string text, int timeoutMs = 1000, Encoding encoding = null)
        {
            // 清空接收缓冲区
            lock (_tcpDataLock)
            {
                _tcpReceivedDataQueue.Clear();
            }
            
            // 发送数据
            bool sent = await SendTcpStringAsync(text, encoding);
            if (!sent)
                return string.Empty;
            
            // 等待并读取响应
            return await ReadTcpStringAsync(timeoutMs, encoding);
        }

        /// <summary>
        /// 发送带结束符的字符串并等待带结束符的响应
        /// </summary>
        /// <param name="text">要发送的字符串</param>
        /// <param name="terminator">结束符</param>
        /// <param name="timeoutMs">等待响应超时时间（毫秒）</param>
        /// <param name="encoding">编码方式，默认UTF8</param>
        /// <returns>响应字符串（不包含结束符）</returns>
        public async Task<string> SendAndReceiveTcpStringWithTerminatorAsync(string text, string terminator = "\r\n", int timeoutMs = 1000, Encoding encoding = null)
        {
            // 清空接收缓冲区
            lock (_tcpDataLock)
            {
                _tcpReceivedDataQueue.Clear();
            }
            
            // 发送带结束符的数据
            bool sent = await SendTcpStringWithTerminatorAsync(text, terminator, encoding);
            if (!sent)
                return string.Empty;
            
            // 等待并读取带结束符的响应
            return await ReadTcpStringUntilAsync(terminator, timeoutMs, encoding);
        }

        /// <summary>
        /// TCP数据接收事件委托
        /// </summary>
        public event EventHandler<TcpDataReceivedEventArgs> OnTcpDataReceived;

        /// <summary>
        /// 发送TCP数据并直接等待响应（不依赖后台接收循环）
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="timeoutMs">等待响应超时时间（毫秒）</param>
        /// <returns>响应数据</returns>
        public async Task<byte[]> SendAndReceiveTcpDataAsync(byte[] data, int timeoutMs = 1000)
        {
            try
            {
                if (!_isTcpConnected || _networkStream == null || !_tcpClient.Connected)
                    return null;

                // 发送数据
                await _networkStream.WriteAsync(data, 0, data.Length);
                await _networkStream.FlushAsync();

                // 直接从网络流读取响应（阻塞式）
                byte[] buffer = new byte[1024];
                
                // 设置读取超时
                _tcpClient.ReceiveTimeout = timeoutMs;
                
                // 等待数据到达
                int totalBytesRead = 0;
                DateTime startTime = DateTime.Now;
                
                // 先等待一小段时间让数据到达
                await Task.Delay(50);
                
                // 读取所有可用数据
                while (_networkStream.DataAvailable || totalBytesRead == 0)
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMs)
                        break;
                    
                    if (_networkStream.DataAvailable)
                    {
                        int bytesRead = await _networkStream.ReadAsync(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                        if (bytesRead > 0)
                        {
                            totalBytesRead += bytesRead;
                            // 短暂等待看是否还有更多数据
                            await Task.Delay(20);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        await Task.Delay(10);
                    }
                }

                if (totalBytesRead > 0)
                {
                    // 更新统计
                    lock (_statsLock)
                    {
                        _totalReceivedBytes += totalBytesRead;
                        _lastReceivedLength = totalBytesRead;
                    }

                    byte[] result = new byte[totalBytesRead];
                    Array.Copy(buffer, result, totalBytesRead);
                    return result;
                }

                return new byte[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP发送接收数据失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 使用独立连接发送数据并等待响应（完全独立，不受后台循环影响）
        /// </summary>
        /// <param name="ipAddress">服务器IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="data">要发送的数据</param>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>响应数据</returns>
        public async Task<byte[]> SendAndReceiveWithNewConnectionAsync(string ipAddress, int port, byte[] data, int timeoutMs = 2000)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            
            try
            {
                // 创建新的TCP连接
                client = new TcpClient();
                client.ReceiveTimeout = timeoutMs;
                client.SendTimeout = timeoutMs;
                
                await client.ConnectAsync(ipAddress, port);
                stream = client.GetStream();
                
                // 发送数据
                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();
                
                // 等待响应
                byte[] buffer = new byte[4096];
                int totalBytesRead = 0;
                DateTime startTime = DateTime.Now;
                
                // 使用阻塞式读取
                while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                        if (bytesRead > 0)
                        {
                            totalBytesRead += bytesRead;
                            // 等待一下看是否有更多数据
                            await Task.Delay(50);
                            if (!stream.DataAvailable)
                                break;
                        }
                    }
                    else
                    {
                        await Task.Delay(10);
                    }
                }
                
                if (totalBytesRead > 0)
                {
                    // 更新统计
                    lock (_statsLock)
                    {
                        _totalReceivedBytes += totalBytesRead;
                        _lastReceivedLength = totalBytesRead;
                    }
                    
                    byte[] result = new byte[totalBytesRead];
                    Array.Copy(buffer, result, totalBytesRead);
                    return result;
                }
                
                return new byte[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"独立TCP连接通讯失败: {ex.Message}");
                return null;
            }
            finally
            {
                stream?.Close();
                client?.Close();
            }
        }

        #endregion

        #region TCP读写保持寄存器（Modbus TCP协议）

        // Modbus TCP事务ID计数器
        private ushort _transactionId = 0;
        
        /// <summary>
        /// 获取下一个事务ID
        /// </summary>
        private ushort GetNextTransactionId()
        {
            return ++_transactionId;
        }

        /// <summary>
        /// 连接到Modbus TCP服务器（读写保持寄存器）
        /// </summary>
        /// <param name="ipAddress">服务器IP地址</param>
        /// <param name="port">端口号，默认502</param>
        /// <returns>是否连接成功</returns>
        public async Task<bool> ConnectToModbusTcp(string ipAddress, int port = 502)
        {
            try
            {
                if (_isTcpConnected)
                {
                    DisconnectModbusTcp();
                }

                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ipAddress, port);
                _networkStream = _tcpClient.GetStream();
                _isTcpConnected = true;
                _transactionId = 0;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus TCP连接失败: {ex.Message}");
                _isTcpConnected = false;
                return false;
            }
        }

        /// <summary>
        /// 断开Modbus TCP连接
        /// </summary>
        public void DisconnectModbusTcp()
        {
            try
            {
                _isTcpConnected = false;
                _networkStream?.Close();
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    _tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"断开Modbus TCP连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 构建Modbus TCP请求帧（MBAP头 + PDU）
        /// </summary>
        private byte[] BuildModbusTcpRequest(byte slaveId, byte functionCode, byte[] pdu)
        {
            ushort transId = GetNextTransactionId();
            ushort length = (ushort)(1 + pdu.Length); // 单元标识符 + PDU长度
            
            byte[] request = new byte[7 + pdu.Length];
            request[0] = (byte)(transId >> 8);      // 事务ID高字节
            request[1] = (byte)(transId & 0xFF);    // 事务ID低字节
            request[2] = 0x00;                       // 协议标识符高字节
            request[3] = 0x00;                       // 协议标识符低字节
            request[4] = (byte)(length >> 8);        // 长度高字节
            request[5] = (byte)(length & 0xFF);      // 长度低字节
            request[6] = slaveId;                    // 单元标识符
            
            Array.Copy(pdu, 0, request, 7, pdu.Length);
            return request;
        }

        /// <summary>
        /// 发送Modbus TCP请求并接收响应
        /// </summary>
        private async Task<byte[]> SendModbusTcpRequestAsync(byte[] request, int timeoutMs = 3000)
        {
            try
            {
                if (!_isTcpConnected || _networkStream == null || !_tcpClient.Connected)
                {
                    Console.WriteLine("Modbus TCP未连接");
                    return null;
                }

                // 发送请求
                await _networkStream.WriteAsync(request, 0, request.Length);
                await _networkStream.FlushAsync();

                // 接收响应
                byte[] response = new byte[1024];
                
                // 设置读取超时
                _networkStream.ReadTimeout = timeoutMs;
                int bytesRead = await _networkStream.ReadAsync(response, 0, response.Length);

                if (bytesRead > 0)
                {
                    // 更新统计
                    lock (_statsLock)
                    {
                        _totalReceivedBytes += bytesRead;
                        _lastReceivedLength = bytesRead;
                    }
                    
                    byte[] result = new byte[bytesRead];
                    Array.Copy(response, result, bytesRead);
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus TCP通讯失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 功能码03：读取保持寄存器（Read Holding Registers）
        /// </summary>
        /// <param name="slaveId">从站ID</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="quantity">读取数量</param>
        /// <returns>返回寄存器值数组</returns>
        public async Task<ushort[]> ModbusTcp_ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort quantity)
        {
            try
            {
                // 构建PDU: 功能码(1) + 起始地址(2) + 数量(2)
                byte[] pdu = new byte[5];
                pdu[0] = 0x03;                              // 功能码
                pdu[1] = (byte)(startAddress >> 8);         // 起始地址高字节
                pdu[2] = (byte)(startAddress & 0xFF);       // 起始地址低字节
                pdu[3] = (byte)(quantity >> 8);             // 数量高字节
                pdu[4] = (byte)(quantity & 0xFF);           // 数量低字节

                byte[] request = BuildModbusTcpRequest(slaveId, 0x03, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 9)
                    return null;

                // 检查功能码是否正确
                if (response[7] != 0x03)
                {
                    Console.WriteLine($"Modbus TCP响应错误，功能码: {response[7]:X2}");
                    return null;
                }

                // 解析数据
                int byteCount = response[8];
                int regCount = byteCount / 2;
                ushort[] result = new ushort[regCount];

                for (int i = 0; i < regCount; i++)
                {
                    result[i] = (ushort)(response[9 + i * 2] << 8 | response[10 + i * 2]);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取保持寄存器失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 功能码03：读取保持寄存器并转换为short数组
        /// </summary>
        public async Task<short[]> ModbusTcp_ReadHoldingRegistersAsShort(byte slaveId, ushort startAddress, ushort quantity)
        {
            ushort[] data = await ModbusTcp_ReadHoldingRegisters(slaveId, startAddress, quantity);
            if (data == null) return null;
            return data.Select(x => (short)x).ToArray();
        }

        /// <summary>
        /// 功能码03：读取保持寄存器并转换为float数组（每2个寄存器组成1个float）
        /// </summary>
        public async Task<float[]> ModbusTcp_ReadHoldingRegistersAsFloat(byte slaveId, ushort startAddress, ushort quantity)
        {
            if (quantity % 2 != 0)
            {
                Console.WriteLine("读取float数据时，寄存器数量必须是2的倍数");
                return null;
            }

            ushort[] data = await ModbusTcp_ReadHoldingRegisters(slaveId, startAddress, quantity);
            if (data == null) return null;

            int floatCount = data.Length / 2;
            float[] result = new float[floatCount];

            for (int i = 0; i < floatCount; i++)
            {
                byte[] floatBytes = new byte[4];
                floatBytes[0] = (byte)(data[i * 2] >> 8);
                floatBytes[1] = (byte)(data[i * 2] & 0xFF);
                floatBytes[2] = (byte)(data[i * 2 + 1] >> 8);
                floatBytes[3] = (byte)(data[i * 2 + 1] & 0xFF);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(floatBytes);

                result[i] = BitConverter.ToSingle(floatBytes, 0);
            }

            return result;
        }

        /// <summary>
        /// 功能码04：读取输入寄存器（Read Input Registers）
        /// </summary>
        public async Task<ushort[]> ModbusTcp_ReadInputRegisters(byte slaveId, ushort startAddress, ushort quantity)
        {
            try
            {
                byte[] pdu = new byte[5];
                pdu[0] = 0x04;                              // 功能码
                pdu[1] = (byte)(startAddress >> 8);
                pdu[2] = (byte)(startAddress & 0xFF);
                pdu[3] = (byte)(quantity >> 8);
                pdu[4] = (byte)(quantity & 0xFF);

                byte[] request = BuildModbusTcpRequest(slaveId, 0x04, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 9)
                    return null;

                if (response[7] != 0x04)
                {
                    Console.WriteLine($"Modbus TCP响应错误，功能码: {response[7]:X2}");
                    return null;
                }

                int byteCount = response[8];
                int regCount = byteCount / 2;
                ushort[] result = new ushort[regCount];

                for (int i = 0; i < regCount; i++)
                {
                    result[i] = (ushort)(response[9 + i * 2] << 8 | response[10 + i * 2]);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取输入寄存器失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 功能码01：读取线圈状态（Read Coils）
        /// </summary>
        public async Task<bool[]> ModbusTcp_ReadCoils(byte slaveId, ushort startAddress, ushort quantity)
        {
            try
            {
                byte[] pdu = new byte[5];
                pdu[0] = 0x01;                              // 功能码
                pdu[1] = (byte)(startAddress >> 8);
                pdu[2] = (byte)(startAddress & 0xFF);
                pdu[3] = (byte)(quantity >> 8);
                pdu[4] = (byte)(quantity & 0xFF);

                byte[] request = BuildModbusTcpRequest(slaveId, 0x01, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 9)
                    return null;

                if (response[7] != 0x01)
                {
                    Console.WriteLine($"Modbus TCP响应错误，功能码: {response[7]:X2}");
                    return null;
                }

                int byteCount = response[8];
                bool[] result = new bool[quantity];

                for (int i = 0; i < quantity; i++)
                {
                    int byteIndex = i / 8;
                    int bitIndex = i % 8;
                    result[i] = (response[9 + byteIndex] & (1 << bitIndex)) != 0;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取线圈失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 功能码02：读取离散输入（Read Discrete Inputs）
        /// </summary>
        public async Task<bool[]> ModbusTcp_ReadDiscreteInputs(byte slaveId, ushort startAddress, ushort quantity)
        {
            try
            {
                byte[] pdu = new byte[5];
                pdu[0] = 0x02;                              // 功能码
                pdu[1] = (byte)(startAddress >> 8);
                pdu[2] = (byte)(startAddress & 0xFF);
                pdu[3] = (byte)(quantity >> 8);
                pdu[4] = (byte)(quantity & 0xFF);

                byte[] request = BuildModbusTcpRequest(slaveId, 0x02, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 9)
                    return null;

                if (response[7] != 0x02)
                {
                    Console.WriteLine($"Modbus TCP响应错误，功能码: {response[7]:X2}");
                    return null;
                }

                int byteCount = response[8];
                bool[] result = new bool[quantity];

                for (int i = 0; i < quantity; i++)
                {
                    int byteIndex = i / 8;
                    int bitIndex = i % 8;
                    result[i] = (response[9 + byteIndex] & (1 << bitIndex)) != 0;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取离散输入失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 功能码05：写单个线圈（Write Single Coil）
        /// </summary>
        public async Task<bool> ModbusTcp_WriteSingleCoil(byte slaveId, ushort coilAddress, bool value)
        {
            try
            {
                byte[] pdu = new byte[5];
                pdu[0] = 0x05;                              // 功能码
                pdu[1] = (byte)(coilAddress >> 8);
                pdu[2] = (byte)(coilAddress & 0xFF);
                pdu[3] = (byte)(value ? 0xFF : 0x00);       // ON=0xFF00, OFF=0x0000
                pdu[4] = 0x00;

                byte[] request = BuildModbusTcpRequest(slaveId, 0x05, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 12)
                    return false;

                // 验证响应
                return response[7] == 0x05 && 
                       response[8] == pdu[1] && response[9] == pdu[2];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写单个线圈失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 功能码06：写单个保持寄存器（Write Single Register）
        /// </summary>
        public async Task<bool> ModbusTcp_WriteSingleRegister(byte slaveId, ushort registerAddress, ushort value)
        {
            try
            {
                byte[] pdu = new byte[5];
                pdu[0] = 0x06;                              // 功能码
                pdu[1] = (byte)(registerAddress >> 8);
                pdu[2] = (byte)(registerAddress & 0xFF);
                pdu[3] = (byte)(value >> 8);
                pdu[4] = (byte)(value & 0xFF);

                byte[] request = BuildModbusTcpRequest(slaveId, 0x06, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 12)
                    return false;

                // 验证响应
                return response[7] == 0x06 && 
                       response[8] == pdu[1] && response[9] == pdu[2] &&
                       response[10] == pdu[3] && response[11] == pdu[4];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写单个寄存器失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 功能码0F：写多个线圈（Write Multiple Coils）
        /// </summary>
        public async Task<bool> ModbusTcp_WriteMultipleCoils(byte slaveId, ushort startAddress, bool[] values)
        {
            try
            {
                int quantity = values.Length;
                int byteCount = (quantity + 7) / 8;
                
                byte[] pdu = new byte[6 + byteCount];
                pdu[0] = 0x0F;                              // 功能码
                pdu[1] = (byte)(startAddress >> 8);
                pdu[2] = (byte)(startAddress & 0xFF);
                pdu[3] = (byte)(quantity >> 8);
                pdu[4] = (byte)(quantity & 0xFF);
                pdu[5] = (byte)byteCount;

                // 将bool数组转换为字节
                for (int i = 0; i < quantity; i++)
                {
                    if (values[i])
                    {
                        int byteIndex = i / 8;
                        int bitIndex = i % 8;
                        pdu[6 + byteIndex] |= (byte)(1 << bitIndex);
                    }
                }

                byte[] request = BuildModbusTcpRequest(slaveId, 0x0F, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 12)
                    return false;

                return response[7] == 0x0F;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写多个线圈失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 功能码10：写多个保持寄存器（Write Multiple Registers）
        /// </summary>
        public async Task<bool> ModbusTcp_WriteMultipleRegisters(byte slaveId, ushort startAddress, ushort[] values)
        {
            try
            {
                int quantity = values.Length;
                int byteCount = quantity * 2;

                byte[] pdu = new byte[6 + byteCount];
                pdu[0] = 0x10;                              // 功能码
                pdu[1] = (byte)(startAddress >> 8);
                pdu[2] = (byte)(startAddress & 0xFF);
                pdu[3] = (byte)(quantity >> 8);
                pdu[4] = (byte)(quantity & 0xFF);
                pdu[5] = (byte)byteCount;

                // 写入寄存器值
                for (int i = 0; i < quantity; i++)
                {
                    pdu[6 + i * 2] = (byte)(values[i] >> 8);
                    pdu[7 + i * 2] = (byte)(values[i] & 0xFF);
                }

                byte[] request = BuildModbusTcpRequest(slaveId, 0x10, pdu);
                byte[] response = await SendModbusTcpRequestAsync(request);

                if (response == null || response.Length < 12)
                    return false;

                return response[7] == 0x10 &&
                       response[10] == pdu[3] && response[11] == pdu[4];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写多个寄存器失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 功能码10：写多个保持寄存器（short数组）
        /// </summary>
        public async Task<bool> ModbusTcp_WriteMultipleRegistersShort(byte slaveId, ushort startAddress, short[] values)
        {
            ushort[] ushortValues = values.Select(x => (ushort)x).ToArray();
            return await ModbusTcp_WriteMultipleRegisters(slaveId, startAddress, ushortValues);
        }

        /// <summary>
        /// 功能码10：写多个保持寄存器（float数组，每个float占2个寄存器）
        /// </summary>
        public async Task<bool> ModbusTcp_WriteMultipleRegistersFloat(byte slaveId, ushort startAddress, float[] values)
        {
            ushort[] ushortValues = new ushort[values.Length * 2];

            for (int i = 0; i < values.Length; i++)
            {
                byte[] floatBytes = BitConverter.GetBytes(values[i]);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(floatBytes);

                ushortValues[i * 2] = (ushort)(floatBytes[0] << 8 | floatBytes[1]);
                ushortValues[i * 2 + 1] = (ushort)(floatBytes[2] << 8 | floatBytes[3]);
            }

            return await ModbusTcp_WriteMultipleRegisters(slaveId, startAddress, ushortValues);
        }

        #endregion
    }

    /// <summary>
    /// 串口数据接收事件参数
    /// </summary>
    public class SerialDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }

        public SerialDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
    
    /// <summary>
    /// TCP数据接收事件参数
    /// </summary>
    public class TcpDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }

        public TcpDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}