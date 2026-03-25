using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace closed
{
    //public int Id { get; set; }
    class Mbus
    {
        #region 创建串口
        
        private static SerialPort serialPort = new SerialPort();

        public struct Comm
        {
            //private bool isOpen;
            //private string com;
            //private int baudRate;
            //private int dataBits;
            //private Parity parity;
            //private StopBits stopBits;
            //private Byte slaveId;
            //private byte functionCode;
            //private ushort startAddress;
            //private ushort numDataRegisters;//读取数据寄存器数
            //private ushort recNumByte;//接受的字节长度}

            public bool IsOpen
            {
                get;
                set;
            }
            public string Com
            {
                get;
                set;
            }
            public int BaudRate
            {
                get;
                set;
            }
            public int DataBits
            {
                get;
                set;
            }
            public Parity Parity
            {
                get;
                set;
            }
            public StopBits StopBits
            {
                get;
                set;
            }
            public Byte SlaveId
            {
                get;
                set;
            }
            public Byte FunctionCode
            {
                get;
                set;
            }
            public ushort StartAddress
            {
                get;
                set;
            }
            public ushort NumDataRegisters
            {
                get;
                set;
            }
            public ushort RecNumByte
            {
                get;
                set;
            }
            public SerialPort serialPort { get; set; }
        }
        #endregion

        #region 串口初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        static public void Init(SerialPort port, Comm comm = new Comm())
        {
            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Contains(comm.Com))
            {
                serialPort.PortName = comm.Com;//声明串口
                serialPort.BaudRate = comm.BaudRate;//波特率
                serialPort.DataBits = comm.DataBits;//数据位
                serialPort.Parity = comm.Parity;
                serialPort.StopBits = comm.StopBits;
            }
            else
            { return; }

        }
        #endregion

        #region Rtu读取
        /// <summary>
        /// rtu通讯
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numRegisters"></param>
        static public (ushort[], bool) RtuRead(Comm commt = new Comm())
        {
            ushort[] result = new ushort[commt.RecNumByte];
            for (int i = 0; i < commt.RecNumByte - 1; i++)
            {
                result[i] = 0;
            }
            try
            {
                if (serialPort.IsOpen)
                {
                    commt.IsOpen = true;
                    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);
                    ushort[] registers = master.ReadHoldingRegisters(commt.SlaveId, commt.StartAddress, commt.RecNumByte);
                    result = registers;

                    //this.textBox1.Invoke(new Action(() => { textBox1.Text = registers[0].ToString(); }));
                }
                else
                {

                    Init(serialPort, commt);
                    serialPort.Open();
                    if (serialPort.IsOpen)
                    {
                        commt.IsOpen = true;
                        IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);
                        ushort[] registers = master.ReadHoldingRegisters(commt.SlaveId, commt.StartAddress, commt.RecNumByte);
                        result = registers;
                    }
                    else
                    {
                        commt.IsOpen = false;
                        for (int i = 0; i < commt.RecNumByte - 1; i++)
                        {
                            result[i] = 0;
                        }
                    }

                    //this.textBox1.Invoke(new Action(() => { textBox1.Text = "0"; }));                  
                }
            }
            catch
            {
                commt.IsOpen = false;
                for (int i = 0; i < commt.RecNumByte - 1; i++)
                {
                    result[i] = 0;
                }

                //this.textBox1.Invoke(new Action(() => { textBox1.Text = "0"; }));
            }
            return (result, commt.IsOpen);
        }
        #endregion

        #region Rtu写入
        /// <summary>
        /// rtu通讯
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numRegisters"></param>
        static public void RtuWrite(ushort startAddress, ushort writeData, Comm commt = new Comm())
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    commt.IsOpen = true;
                    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);
                    master.WriteSingleRegister(commt.SlaveId, startAddress, writeData);
                }
                else
                {
                    Init(serialPort, commt);
                    serialPort.Open();
                    if (serialPort.IsOpen)
                    {
                        commt.IsOpen = true;
                        IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);
                        master.WriteSingleRegister(commt.SlaveId, startAddress, writeData);
                    }
                    else
                    {
                        commt.IsOpen = false;

                    }
                }
            }
            catch
            {
                commt.IsOpen = false;
            }
        }
        #endregion

        #region  自由口通讯
        /// <summary>
        /// 自由口
        /// </summary>
        /// <param name="commt"></param>
        /// <returns></returns>
        static public (byte[], int, bool) FreeRead(byte[] writeData, Comm commt = new Comm())
        {
            int countRead = 0;
            byte[] response = new byte[commt.RecNumByte];
            for (int i = 0; i < commt.RecNumByte - 1; i++)
            {
                response[i] = 0;
            }
            try
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        commt.IsOpen = true;
                        serialPort.Write(writeData, 0, writeData.Length);
                        Thread.Sleep(100);
                        countRead = (serialPort.BytesToRead);

                        if (countRead > 0)
                        {
                            for (int i = 0; i < response.Length; i++)
                            {
                                response[i] = (byte)(serialPort.ReadByte());
                            }
                        }
                        else
                        {
                            for (int i = 0; i < commt.RecNumByte - 1; i++)
                            {
                                response[i] = 0;
                            }
                        }
                    }
                    catch
                    {
                        for (int i = 0; i < commt.RecNumByte - 1; i++)
                        {
                            response[i] = 0;
                        }
                    }
                }
                else
                {
                    Init(serialPort, commt);
                    serialPort.Open();
                    if (serialPort.IsOpen)
                    {
                        commt.IsOpen = true;
                        serialPort.Write(writeData, 0, writeData.Length);
                        Thread.Sleep(100);
                        countRead = (serialPort.BytesToRead);
                        if (countRead > 0)
                        {
                            for (int i = 0; i < response.Length; i++)
                            {
                                response[i] = (byte)(serialPort.ReadByte());
                            }
                        }
                        else
                        {
                            for (int i = 0; i < commt.RecNumByte - 1; i++)
                            {
                                response[i] = 0;
                            }
                        }
                    }
                    else
                    {
                        commt.IsOpen = false;
                        for (int i = 0; i < commt.RecNumByte - 1; i++)
                        {
                            response[i] = 0;
                        }
                    }

                    //this.textBox1.Invoke(new Action(() => { textBox1.Text = "0"; }));
                }
            }
            catch
            {
                commt.IsOpen = false;
                for (int i = 0; i < commt.RecNumByte - 1; i++)
                {
                    response[i] = 0;
                }
            }

            return (response, countRead, commt.IsOpen);

        }

        static public void portClose()
        {
            serialPort.Close();
        }
        #endregion

        #region ModbusTcp
        //private IPAddress address;
        //private int tcpPort;
        //private static  TcpClient client = new TcpClient(address.ToString(), 502);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="iPAddress"></param>
        /// <param name="tcpPort"></param>
        /// <param name="idAdr"></param>
        /// <param name="startAddress"></param>
        /// <param name="numRegs"></param>
        /// <returns></returns>
        static public ushort[] TcpClient(int mode, TcpClient client, byte idAdr, ushort startAddress, ushort numRegs)
        {
            ushort[] regsBuffer = new ushort[numRegs];
            for (int i = 0; i < numRegs - 1; i++)
            {
                regsBuffer[i] = 0;
            }
            try
            {
                client.SendTimeout = 1;
                ModbusIpMaster modbusTcpSlave = ModbusIpMaster.CreateIp(client);
                switch (mode)
                {
                    case 0:
                        regsBuffer = modbusTcpSlave.ReadHoldingRegisters(idAdr, startAddress, numRegs);
                        break;
                    case 1:
                        modbusTcpSlave.WriteSingleRegister(idAdr, startAddress, numRegs);
                        break;
                }
            }
            catch
            { }
            return regsBuffer;
        }
        #endregion

        #region 16位CRC校验

        static public byte[] CRC16ArryByte(Comm comm = new Comm())
        {
            //从站地址
            byte slaveAddress = comm.SlaveId;
            //功能码
            byte functionCode = comm.FunctionCode;
            //保持寄存器开始地址，16位寄存器需要把int转为ushort。因通信格式是高位在前，所以要Reverse()
            int start = comm.StartAddress;
            byte[] Starts = BitConverter.GetBytes((ushort)start).Reverse().ToArray();
            //访问保存寄存器数量
            int number = comm.NumDataRegisters;
            byte[] Numbers = BitConverter.GetBytes((ushort)number).Reverse().ToArray();//
                                                                                       //Modbus功能码03通讯指令共8个字节
            byte[] command = new byte[8];
            command[0] = slaveAddress;
            command[1] = functionCode;
            Array.Copy(Starts, 0, command, 2, Starts.Length);//Starts.Length实际就是2
            Array.Copy(Numbers, 0, command, 4, Numbers.Length);//Numbers.Length实际也是2
                                                               //计算CRC值                                                 
            byte[] bdata = new byte[6];
            Array.Copy(command, 0, bdata, 0, bdata.Length);
            //CRC校验码，crc16[0]为高位，crc16[1]为低位，所以不用再Reverse()
            //得到2个字节校验码crc16
            byte[] crc16 = CRC16(bdata);
            //将CRC校验码写入指令command
            //得到8个字节完整发送指command
            Array.Copy(crc16, 0, command, 6, crc16.Length);
            /*
            至此，便构建完成查询保存寄存器（功能码0x03）的通讯指令command
            */
            //输出16进制string类型结果cmd
            StringBuilder resultCmd = new StringBuilder();
            foreach (var one in command)
            {
                resultCmd.Append(string.Format("{0:X2}", one));
                resultCmd.Append(" ");
            }
            string cmd = resultCmd.ToString().Trim();
            return command;
        }
        /// <summary>
        /// CRC校验，参数data为byte数组
        /// </summary>
        /// <param name="data">校验数据，字节数组</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        static public byte[] CRC16(byte[] data)
        {
            //crc计算赋初始值
            int crc = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                crc = crc ^ data[i];
                for (int j = 0; j < 8; j++)
                {
                    int temp;
                    temp = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (temp == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            //CRC寄存器的高低位进行互换
            byte[] crc16 = new byte[2];
            //CRC寄存器的高8位变成低8位，
            crc16[1] = (byte)((crc >> 8) & 0xff);
            //CRC寄存器的低8位变成高8位
            crc16[0] = (byte)(crc & 0xff);
            return crc16;
        }

        /// <summary>
        /// CRC校验，参数为空格或逗号间隔的字符串
        /// </summary>
        /// <param name="data">校验数据，逗号或空格间隔的16进制字符串(带有0x或0X也可以),逗号与空格不能混用</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>

        static public byte[] CRC16(string data)
        {
            //分隔符是空格还是逗号进行分类，并去除输入字符串中的多余空格
            IEnumerable<string> datac = data.Contains(",") ? data.Replace(" ", "").Replace("0x", "").Replace("0X", "").Trim().Split(',') : data.Replace("0x", "").Replace("0X", "").Split(' ').ToList().Where(u => u != "");
            List<byte> bytedata = new List<byte>();
            foreach (string str in datac)
            {
                bytedata.Add(byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            byte[] crcbuf = bytedata.ToArray();
            //crc计算赋初始值
            int crc = 0xffff;
            for (int i = 0; i < crcbuf.Length; i++)
            {
                crc = crc ^ crcbuf[i];
                for (int j = 0; j < 8; j++)
                {
                    int temp;
                    temp = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (temp == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            //CRC寄存器的高低位进行互换
            byte[] crc16 = new byte[2];
            //CRC寄存器的高8位变成低8位
            crc16[1] = (byte)((crc >> 8) & 0xff);
            //CRC寄存器的低8位变成高8位
            crc16[0] = (byte)(crc & 0xff);

            return crc16;
        }

        #endregion
    }


    class DataCvt
    {

        #region 流量转速仪转化
        /// <summary>
        /// 
        /// </summary>
        /// <param name="decimalBit"></param>
        /// <param name="oneBit"></param>
        /// <param name="hundredBit"></param>
        /// <param name="TenThousandsBit"></param>
        /// <param name="outValue"></param>
        static public double FlowSpeedCvt(byte decimalBit, byte oneBit, byte hundredBit, byte TenThousandsBit)
        {
            int thousands = hundredBit & 0xF0;
            int hundred = hundredBit & 0x0F;
            int ten = oneBit & 0xF0;
            int one = oneBit & 0x0F;

            int decimal1 = decimalBit & 0xF0;
            int decimal2 = decimalBit & 0x0F;
            int oneHundredThousands = TenThousandsBit & 0xF0;
            int tenThousands = TenThousandsBit & 0x0F;

            double outValue = thousands * 1000.0 / 16.0 + hundred * 100.0 + ten * 10.0 / 16.0 + one * 1.0 + decimal1 * 0.1 / 16.0
                + decimal2 * 0.01 + oneHundredThousands * 100000.0 / 16.0 + tenThousands / 10000.0;
            return outValue;
        }

        static public double[] GetFlowSpeed(byte[] bytearry)
        {

            double[] data = new double[5];
            double flowSpeedPowerFry = FlowSpeedCvt(bytearry[2], bytearry[1], 0, 0);
            double flowSpeed = FlowSpeedCvt(bytearry[5], bytearry[4], bytearry[3], 0);
            double rotorFqy = FlowSpeedCvt(bytearry[9], bytearry[8], bytearry[7], bytearry[6]);
            double slipRat = FlowSpeedCvt(bytearry[12], bytearry[10], 0, 0);
            double speedFlow = FlowSpeedCvt(bytearry[15], bytearry[14], bytearry[13], bytearry[12]);
            return data = new double[] { flowSpeed, flowSpeedPowerFry, rotorFqy, slipRat, speedFlow };
        }
        #endregion

        #region 2字节转整数
        /// <summary>
        /// 2byte转化int
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        static public int Byte2ToReal(byte byte1, byte byte2)
        {
            byte[] l_bytes = new byte[2];
            l_bytes[1] = byte1;
            l_bytes[0] = byte2;
            return BitConverter.ToInt16(l_bytes, 0);
        }
        static public int Byte4ToReal(byte byte1, byte byte2, byte byte3, byte byte4)
        {
            byte[] l_bytes = new byte[4];
            l_bytes[3] = byte1;
            l_bytes[2] = byte2;
            l_bytes[1] = byte3;
            l_bytes[0] = byte4;
            return BitConverter.ToInt16(l_bytes, 0);
        }
        #endregion


        #region 判断输入字符串为合法浮点数或者整数

        static public bool IsLegal(string dataType, string str)
        {
            bool result = false;
            if (str == "")
            {
                result = false;
            }
            else
            {
                try
                {
                    switch (dataType)
                    {
                        case "float":
                            float.Parse(str);
                            result = true;
                            break;
                        case "int":
                            int.Parse(str);
                            result = true;
                            break;
                        case "ip":
                            Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                            result = (str != "" && validipregex.IsMatch(str.Trim())) ? true : false;
                            break;

                    }
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }
        #endregion

        static public float StrToReal(byte[] b)
        {
            //string[] str = new string[B1.Length];
            //foreach(byte tem in B1)
            //{
            //     str[tem] = ConvertBCDToInt(B1[tem]).ToString();
            //}
            int bitSign = ConvertBCDToInt(b[0]);
            int bitDecPlace = ConvertBCDToInt(b[6]);
            string s1 = ConvertBCDToInt(b[1]).ToString();
            string s2 = ConvertBCDToInt(b[2]).ToString();
            string s3 = ConvertBCDToInt(b[3]).ToString();
            string s4 = ConvertBCDToInt(b[4]).ToString();
            string s5 = ConvertBCDToInt(b[5]).ToString();
            string s6 = $"{s1}{s2}{s3}{s4}{s5}";
            if (IsLegal("float", s6))
            {
                return Convert.ToSingle(Convert.ToSingle(s6) * Math.Pow(-1.0, bitSign) * Math.Pow(0.1, 5 - bitDecPlace));
            }
            else
            {
                return 0;
            }
        }


        #region 
        public static int ConvertBCDToInt(byte b)
        {
            //高四位  
            byte b1 = (byte)((b >> 4) & 0xF);
            //低四位  
            byte b2 = (byte)(b & 0xF);

            return (b1 * 10 + b2);
        }
        #endregion
    }
}
