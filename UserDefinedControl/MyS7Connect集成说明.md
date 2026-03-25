# MyS7Connect库集成说明

## 1. 当前状态
已经为您准备好了MyS7Connect库的集成框架，包含以下包装方法：
- `MyS7Connect_BoolWriteAsync(string adrName, bool value)`
- `MyS7Connect_ReadBoolAsync(string adrName)`

## 2. 需要修改的位置

### 2.1 添加引用
在Form1.cs文件开头，取消以下注释并根据您的MyS7Connect库调整命名空间：
```csharp
// using MyS7Connect; // 请根据您的MyS7Connect库的实际命名空间调整
```

### 2.2 初始化MyS7Connect实例（如果需要）
如果您的MyS7Connect库需要实例化，请在Form1类中添加：
```csharp
// private static MyS7Connect.S7Client s7Client; // 示例，请根据实际类名调整
```

### 2.3 修改写入方法
在`MyS7Connect_BoolWriteAsync`方法中，替换以下模拟调用：
```csharp
// 当前模拟调用（请替换）
await SimulateBoolWriteAsync(adrName, value);

// 替换为您的实际调用（选择其中一种方式）：

// 方式1：如果是静态方法
// await MyS7Connect.S7Client.BoolWriteAsync(adrName, value);

// 方式2：如果是实例方法
// await s7Client.BoolWriteAsync(adrName, value);

// 方式3：如果您的方法在其他类中
// await YourS7ConnectClass.BoolWriteAsync(adrName, value);
```

### 2.4 修改读取方法
在`MyS7Connect_ReadBoolAsync`方法中，替换以下模拟调用：
```csharp
// 当前模拟调用（请替换）
result = await SimulateReadBoolAsync(adrName);

// 替换为您的实际调用（选择其中一种方式）：

// 方式1：如果是静态方法
// result = await MyS7Connect.S7Client.ReadBoolAsync(adrName);

// 方式2：如果是实例方法
// result = await s7Client.ReadBoolAsync(adrName);

// 方式3：如果您的方法在其他类中
// result = await YourS7ConnectClass.ReadBoolAsync(adrName);
```

## 3. 初始化S7连接（如果需要）
如果您的MyS7Connect库需要初始化连接，请在Form1_Load或InitializeDemo方法中添加：
```csharp
// 示例初始化代码（请根据您的API调整）
// s7Client = new MyS7Connect.S7Client();
// await s7Client.ConnectAsync("192.168.1.100", 0, 1); // IP, Rack, Slot
```

## 4. 清理模拟代码
集成完成后，您可以删除以下临时模拟方法：
- `SimulateBoolWriteAsync`
- `SimulateReadBoolAsync`
- `addressStorage` 字典（如果不再需要）

## 5. 错误处理
包装方法中已经包含了错误处理，会：
- 捕获异常并记录到控制台
- 在界面上显示错误信息
- 对读取操作返回安全的默认值

## 6. 测试步骤
1. 确保MyS7Connect库的引用已添加到项目
2. 根据上述说明修改方法调用
3. 确保PLC连接正常
4. 运行程序测试控件功能

## 7. 当前地址配置
程序使用以下真实PLC地址：
- 控制地址：1214.PLC1.ADDR_BTN1, 1214.PLC1.ADDR_BTN2, 1214.PLC1.ADDR_BTN3
- SignalA地址：1214.PLC1.SIGNAL_A1, 1214.PLC1.SIGNAL_A2, 1214.PLC1.SIGNAL_A3
- SignalB地址：1214.PLC1.SIGNAL_B1, 1214.PLC1.SIGNAL_B2, 1214.PLC1.SIGNAL_B3

## 8. 性能特性
- 所有PLC通信都是异步的，不会阻塞UI线程
- 支持并发读取多个地址
- 包含网络超时和错误恢复机制
- 实时状态显示和日志记录