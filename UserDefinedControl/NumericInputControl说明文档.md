# NumericInputControl 美观数值输入控件

## 🎨 全新美观设计

经过重新设计，`NumericInputControl` 现在具有更加现代化和美观的外观：

### ✨ 视觉改进
- **圆角设计**：所有区域都采用圆角矩形，更加现代化
- **优雅配色**：
  - 反馈区域：淡绿背景 + 森林绿文字 + 浅绿边框
  - 设置区域：淡蓝白背景 + 深蓝文字 + 浅蓝边框  
  - 按钮渐变：淡蓝白（正常）→ 天蓝（悬停）→ 矢车菊蓝（按下）
- **精致尺寸**：220×50 的黄金比例，更加协调美观
- **完美间距**：精心调整的边距和间隙，视觉平衡
- **优化字体**：微软雅黑 10pt，清晰易读

### 🎯 布局优化
- **左侧反馈区域**：实时数值 + 单位显示，圆角绿色主题
- **右侧设置区域**：输入框 + 上下按钮，圆角蓝色主题
- **精美按钮**：小巧的圆角按钮，深灰色箭头图标
- **响应式设计**：鼠标悬停和点击的平滑视觉反馈

## 📋 属性配置

### 数值属性
```csharp
[Category("数值属性")]
public double FeedbackValue { get; }      // 反馈值（只读）
public double SetValue { get; set; }      // 设置值
public double StepValue { get; set; }     // 步长值（默认1.0）
public double MinValue { get; set; }      // 最小值（默认0.0）
public double MaxValue { get; set; }      // 最大值（默认100.0）
```

### 显示属性
```csharp
[Category("显示属性")]
public string Unit { get; set; }          // 单位（如"°C", "bar", "rpm"）
```

### 颜色设置
```csharp
[Category("颜色设置")]
public Color FeedbackBackColor { get; set; }     // 反馈框背景色
public Color FeedbackTextColor { get; set; }     // 反馈值文字色
public Color SetValueBackColor { get; set; }     // 设置框背景色
public Color SetValueTextColor { get; set; }     // 设置值文字色
```

### 控制设置
```csharp
[Category("控制设置")]
public string FeedbackAddress { get; set; }      // 反馈值地址
public string SetValueAddress { get; set; }      // 设置值地址
public WriteFloatAddressAsyncDelegate WriteFloatAddressMethod { get; set; }
public ReadFloatAddressAsyncDelegate ReadFloatAddressMethod { get; set; }
```

## 🛠️ 项目集成

### 1. 已完成的集成
- ✅ 控件源文件：`NumericInputControl.cs`
- ✅ 资源文件：`NumericInputControl.resx`
- ✅ 项目文件引用已添加
- ✅ Form1.Designer.cs 中已添加3个演示实例
- ✅ Form1.cs 中已添加浮点数通信包装方法

### 2. 演示实例配置

#### 温度控制 (numericInput1)
```csharp
this.numericInput1.FeedbackAddress = "1214.PLC1.TEMP_FEEDBACK";
this.numericInput1.SetValueAddress = "1214.PLC1.TEMP_SETPOINT";
this.numericInput1.Unit = "°C";
this.numericInput1.MinValue = 0D;
this.numericInput1.MaxValue = 100D;
this.numericInput1.StepValue = 1D;
this.numericInput1.SetValue = 25D;
```

#### 压力控制 (numericInput2)
```csharp
this.numericInput2.FeedbackAddress = "1214.PLC1.PRESSURE_FEEDBACK";
this.numericInput2.SetValueAddress = "1214.PLC1.PRESSURE_SETPOINT";
this.numericInput2.Unit = "bar";
this.numericInput2.MinValue = 0D;
this.numericInput2.MaxValue = 10D;
this.numericInput2.StepValue = 0.1D;
this.numericInput2.SetValue = 5.5D;
```

#### 速度控制 (numericInput3)
```csharp
this.numericInput3.FeedbackAddress = "1214.PLC1.SPEED_FEEDBACK";
this.numericInput3.SetValueAddress = "1214.PLC1.SPEED_SETPOINT";
this.numericInput3.Unit = "rpm";
this.numericInput3.MinValue = 0D;
this.numericInput3.MaxValue = 3000D;
this.numericInput3.StepValue = 10D;
this.numericInput3.SetValue = 1500D;
```

## 🔌 PLC通信集成

### 包装方法
在 `Form1.cs` 中已添加以下包装方法：

```csharp
/// <summary>
/// 浮点数写入包装器
/// </summary>
private async Task MyS7Connect_FloatWriteAsync(string adrName, double value)
{
    await OpcUa.FloatWriteAsync(adrName, (float)value);
    Console.WriteLine($"MyS7Connect FloatWrite: {adrName} = {value}");
}

/// <summary>
/// 浮点数读取包装器
/// </summary>
private async Task<double> MyS7Connect_ReadFloatAsync(string adrName)
{
    float result = await OpcUa.ReadFloatAsync(adrName);
    Console.WriteLine($"MyS7Connect ReadFloat: {adrName} = {result}");
    return (double)result;
}
```

### 委托绑定
```csharp
private void SetupNumericInputControl(NumericInputControl control)
{
    // 绑定浮点数读写方法
    control.WriteFloatAddressMethod = MyS7Connect_FloatWriteAsync;
    control.ReadFloatAddressMethod = MyS7Connect_ReadFloatAsync;
}
```

## 🖱️ 操作方式

### 1. 直接输入
- 在右侧设置框中直接输入数值
- 支持小数点和负号输入
- 按回车或失去焦点时生效
- 自动限制在Min-Max范围内

### 2. 按钮调节
- 点击 **▲** 按钮：增加一个步值
- 点击 **▼** 按钮：减少一个步值
- 按钮支持鼠标悬停和按下的视觉反馈

### 3. 实时更新
- 反馈值每秒从PLC自动读取更新
- 设置值变更后立即异步写入PLC
- 所有UI更新都在主线程中安全执行

## ⚡ 技术特性

### 异步通信
- 所有PLC通信都是异步的，不阻塞UI线程
- 完整的错误处理和异常捕获
- 通信日志输出到控制台

### 界面优化
- 双缓冲绘制，避免界面闪烁
- 抗锯齿文本和图形渲染
- 局部重绘优化性能

### 线程安全
- 使用 `BeginInvoke` 确保UI更新在主线程
- 异步操作的安全错误处理机制

## 🎨 使用建议

### 1. 工业场景适配
- **温度控制**：单位°C，步值1，范围0-100
- **压力控制**：单位bar，步值0.1，范围0-10
- **速度控制**：单位rpm，步值10，范围0-3000
- **流量控制**：单位L/min，步值0.5，范围0-50

### 2. 自定义外观
```csharp
// 温馨橙色主题
control.FeedbackBackColor = Color.FromArgb(255, 248, 220);
control.FeedbackTextColor = Color.FromArgb(255, 140, 0);

// 工业蓝色主题  
control.FeedbackBackColor = Color.FromArgb(230, 240, 255);
control.FeedbackTextColor = Color.FromArgb(0, 100, 200);
```

### 3. 事件处理
```csharp
numericInput1.SetValueChanged += (sender, newValue) => {
    Console.WriteLine($"设置值改变: {newValue}");
};

numericInput1.FeedbackValueChanged += (sender, newValue) => {
    Console.WriteLine($"反馈值更新: {newValue}");
};
```

## 🚀 立即开始

1. **编译运行**：项目现在可以直接编译运行查看效果
2. **拖拽使用**：从工具箱拖拽控件到其他窗体
3. **设置属性**：在属性面板中配置数值范围和地址
4. **绑定通信**：调用 `SetupNumericInputControl()` 绑定PLC通信

## 🎯 完美适配

这个控件完全遵循您现有项目的：
- ✅ 代码风格一致
- ✅ 颜色主题统一  
- ✅ 通信模式相同
- ✅ 属性分类规范
- ✅ 错误处理机制

可以与您的 `IndicatorButtonControl` 完美搭配使用，构建完整的工业控制界面！