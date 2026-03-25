# NumericInputControl 数值输入控件使用说明

## 控件概述
NumericInputControl 是一个类似 NumericUpDown 的自定义 WinForms 控件，专为工业控制界面设计，支持实时数据显示和PLC通信。

## 主要特性

### 1. 界面布局
- **左侧反馈显示框**：显示实时浮点数反馈值 + 单位
- **右侧设置输入框**：可直接输入设置值
- **上下调节按钮**：带三角形图标的加减按钮，上下排列

### 2. 核心功能
- ✅ 实时反馈值显示（从PLC读取）
- ✅ 设置值输入和调节
- ✅ 步值控制（运行时可调节）
- ✅ 上下限位设置
- ✅ 单位显示自定义
- ✅ 异步PLC通信
- ✅ 美观的界面设计

### 3. 地址控制
- **反馈值地址**：用于读取实时反馈数据
- **设置值地址**：用于读写设置数据
- **异步通信**：支持 FloatWriteAsync 和 ReadFloatAsync 方法

## 属性设置

### 数值属性
- `FeedbackValue`：反馈值（只读）
- `SetValue`：设置值（可读写，自动限制在Min/Max范围内）
- `StepValue`：步长值（加减按钮的增减量）
- `MinValue`：最小值限制
- `MaxValue`：最大值限制

### 显示属性
- `Unit`：单位字符串（如 "°C", "bar", "rpm"）

### 地址设置
- `FeedbackAddress`：反馈值的PLC地址
- `SetValueAddress`：设置值的PLC地址
- `WriteFloatAddressMethod`：写入方法委托
- `ReadFloatAddressMethod`：读取方法委托

## 使用示例

### 在Form1.Designer.cs中的配置
```csharp
// 温度控制
this.numericInput1.FeedbackAddress = "1214.PLC1.TEMP_FEEDBACK";
this.numericInput1.SetValueAddress = "1214.PLC1.TEMP_SETPOINT";
this.numericInput1.Unit = "°C";
this.numericInput1.MinValue = 0D;
this.numericInput1.MaxValue = 100D;
this.numericInput1.StepValue = 1D;
this.numericInput1.SetValue = 25D;

// 压力控制
this.numericInput2.FeedbackAddress = "1214.PLC1.PRESSURE_FEEDBACK";
this.numericInput2.SetValueAddress = "1214.PLC1.PRESSURE_SETPOINT";
this.numericInput2.Unit = "bar";
this.numericInput2.MinValue = 0D;
this.numericInput2.MaxValue = 10D;
this.numericInput2.StepValue = 0.1D;
this.numericInput2.SetValue = 5.5D;

// 速度控制
this.numericInput3.FeedbackAddress = "1214.PLC1.SPEED_FEEDBACK";
this.numericInput3.SetValueAddress = "1214.PLC1.SPEED_SETPOINT";
this.numericInput3.Unit = "rpm";
this.numericInput3.MinValue = 0D;
this.numericInput3.MaxValue = 3000D;
this.numericInput3.StepValue = 10D;
this.numericInput3.SetValue = 1500D;
```

### 在Form1.cs中设置委托方法
```csharp
private void SetupNumericInputControl(NumericInputControl control)
{
    // 绑定浮点数读写方法
    control.WriteFloatAddressMethod = MyS7Connect_FloatWriteAsync;
    control.ReadFloatAddressMethod = MyS7Connect_ReadFloatAsync;
}
```

## 通信方法

### PLC通信包装器
项目中已包含以下包装方法：

```csharp
// 浮点数写入
private async Task MyS7Connect_FloatWriteAsync(string adrName, double value)
{
    await OpcUa.FloatWriteAsync(adrName, (float)value);
}

// 浮点数读取  
private async Task<double> MyS7Connect_ReadFloatAsync(string adrName)
{
    float result = await OpcUa.ReadFloatAsync(adrName);
    return (double)result;
}
```

## 事件支持

### 可用事件
- `SetValueChanged`：设置值改变时触发
- `FeedbackValueChanged`：反馈值改变时触发

### 事件使用示例
```csharp
numericInput1.SetValueChanged += (sender, newValue) => {
    Console.WriteLine($"设置值改变为: {newValue}");
};

numericInput1.FeedbackValueChanged += (sender, newValue) => {
    Console.WriteLine($"反馈值更新为: {newValue}");
};
```

## 操作方式

### 1. 直接输入
- 在右侧设置框中直接输入数值
- 支持小数点和负号
- 按回车或失去焦点时生效
- 自动限制在设定的范围内

### 2. 按钮调节
- 点击上箭头按钮：增加一个步值
- 点击下箭头按钮：减少一个步值
- 按钮有鼠标悬停和按下的视觉反馈

### 3. 实时更新
- 反馈值每秒自动从PLC读取更新
- 设置值变更后自动写入PLC
- UI更新在主线程中安全执行

## 性能特性

### 异步通信
- 所有PLC通信都是异步的，不阻塞UI
- 错误处理和异常捕获
- 网络超时保护

### 界面优化
- 双缓冲绘制，避免闪烁
- 局部重绘，提高性能
- 抗锯齿文本和图形

### 线程安全
- 使用 BeginInvoke 确保UI更新在主线程
- 异步操作的安全错误处理

## 故障排除

### 常见问题
1. **控件不显示数据**：检查地址设置和委托方法绑定
2. **无法写入数据**：确认PLC连接状态和地址权限
3. **显示格式问题**：检查单位设置和数值范围

### 调试信息
- 所有PLC通信都有Console日志输出
- 错误信息会显示在主窗体的状态标签中
- 支持Debug模式下的详细日志

## 扩展功能

### 自定义外观
控件支持通过属性设置自定义外观：
- 背景色设置
- 字体设置
- 边框样式
- 颜色主题

### 数据验证
- 输入值自动验证
- 范围限制检查
- 数据类型转换保护

## 项目集成状态

✅ 已完成的功能：
- 控件基础实现
- 项目文件集成
- 资源文件创建
- 主窗体演示实例
- PLC通信包装器
- 错误处理机制

📝 下一步可以考虑：
- 添加更多视觉样式选项
- 支持不同数据类型（int, long等）
- 添加数据记录和历史功能
- 集成报警和限值监控