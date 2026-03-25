# 串口通讯服务类说明

本项目提供了一个完整的通讯服务类，支持以下三种通讯方式：

1. 串口Modbus自由口通讯（发送接收）
2. Modbus RTU通讯（读取写入）
3. Modbus TCP通讯（读写）

## 文件结构

- `CommunicationService.cs`: 核心通讯服务类
- `Form1.cs`: 使用示例的Windows窗体
- `串口通讯.csproj`: 项目配置文件

## 主要功能

### 1. 串口Modbus自由口通讯
- 支持打开/关闭串口连接
- 支持发送数据到串口
- 支持异步发送数据
- 支持接收串口数据事件

### 2. Modbus RTU通讯
- 支持读取保持寄存器
- 支持写入单个保持寄存器
- 内置CRC校验计算

### 3. Modbus TCP通讯
- 支持连接到Modbus TCP服务器
- 支持断开TCP连接
- 支持读取保持寄存器
- 支持写入单个保持寄存器

## 使用方法

### 创建通讯服务实例
```csharp
private CommunicationService _commService = new CommunicationService();
```

### 串口通讯示例
```csharp
// 打开串口
bool result = _commService.OpenSerialPort("COM1");

// 发送数据
byte[] dataToSend = Encoding.UTF8.GetBytes("Hello, Serial Port!");
bool sendResult = await _commService.SendSerialDataAsync(dataToSend);

// 接收数据（通过事件）
_commService.OnSerialDataReceived += OnSerialDataReceived;

private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
{
    string receivedData = Encoding.UTF8.GetString(e.Data);
    // 处理接收到的数据
}
```

### Modbus RTU通讯示例
```csharp
// 读取保持寄存器
byte[] data = await _commService.ReadHoldingRegistersRtu(slaveAddress, startAddress, numberOfPoints);

// 写入单个寄存器
bool result = await _commService.WriteSingleRegisterRtu(slaveAddress, registerAddress, value);
```

### Modbus TCP通讯示例
```csharp
// 连接到TCP服务器
bool connected = await _commService.ConnectToTcpServer("192.168.1.100", 502);

// 读取保持寄存器
byte[] data = await _commService.ReadHoldingRegistersTcp(slaveId, startAddress, numberOfPoints);

// 写入单个寄存器
bool result = await _commService.WriteSingleRegisterTcp(slaveId, registerAddress, value);
```

## 注意事项

1. 使用完通讯服务后，请调用`Dispose()`方法释放资源
2. 确保目标设备的串口参数与设置一致
3. 对于TCP通讯，请确保网络连接正常且目标设备开启Modbus TCP服务
4. 实际应用中建议增加更多的错误处理和超时机制

## 依赖项

- .NET Framework 4.7
- System.IO.Ports（用于串口通讯）