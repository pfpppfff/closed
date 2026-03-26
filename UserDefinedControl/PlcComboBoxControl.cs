using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserDefinedControl
{
    /// <summary>
    /// 自定义 PLC 下拉选择控件 (支持显示框与下拉列表颜色分离)
    /// </summary>
    public partial class PlcComboBoxControl : UserControl
    {
        #region 私有字段
        private short _currentValue = 0;
        private int _selectedIndex = -1;
        private string _valueAddress = "";
        private bool _enablePlcComm = true;

        private string[] _itemNames = new string[] { "手动", "自动", "停止" };
        private short[] _itemValues = new short[] { 0, 1, 2 };

        private bool _isMouseOver = false;
        private bool _isDroppedDown = false;
        private Timer _updateTimer;
        private ComboBox _innerComboBox;
        private int _buttonRadius = 5;

        // 颜色设置字段
        private Color _boxBackColor = Color.White;                          // 显示框背景颜色
        private Color _textColor = Color.FromArgb(64, 64, 64);                // 字体颜色
        private Color _borderColor = Color.FromArgb(169, 169, 169);           // 正常边框颜色
        private Color _hoverBorderColor = Color.FromArgb(100, 149, 237);      // 悬停/展开时边框颜色

        // --- 新增：下拉列表专属颜色 ---
        private Color _dropdownBackColor = Color.White;                       // 下拉列表背景色
        private Color _dropdownHoverBackColor = Color.FromArgb(100, 149, 237);  // 下拉列表选中/悬停背景色
        private Color _dropdownHoverTextColor = Color.White;                  // 下拉列表选中/悬停字体色

        // 委托
        private WriteShortAddressAsyncDelegate _writeShortAddressMethod;
        private ReadShortAddressAsyncDelegate _readShortAddressMethod;
        #endregion

        #region 委托定义
        public delegate Task WriteShortAddressAsyncDelegate(string address, short value);
        public delegate Task<short> ReadShortAddressAsyncDelegate(string address);
        #endregion

        #region 事件定义
        public event EventHandler<short> ValueChanged;
        #endregion

        #region 公共属性 (数据与控制)
        [Category("数据绑定"), Description("下拉框显示的文本序列数组")]
        public string[] ItemNames
        {
            get => _itemNames;
            set { _itemNames = value; UpdateSelectedIndexFromValue(); Invalidate(); }
        }

        [Category("数据绑定"), Description("与文本对应的 short (Int16) 值序列数组")]
        public short[] ItemValues
        {
            get => _itemValues;
            set { _itemValues = value; UpdateSelectedIndexFromValue(); Invalidate(); }
        }

        [Category("数值属性"), Description("当前选中的底层 short 值")]
        public short CurrentValue
        {
            get => _currentValue;
            set
            {
                if (_currentValue != value)
                {
                    _currentValue = value;
                    UpdateSelectedIndexFromValue();
                    Invalidate();
                    ValueChanged?.Invoke(this, _currentValue);
                }
            }
        }

        [Category("控制设置"), Description("short值的读写地址")]
        public string ValueAddress
        {
            get => _valueAddress;
            set => _valueAddress = value ?? "";
        }

        [Category("控制设置"), Description("是否启用后台定时读取和点击写入 PLC 交互")]
        public bool EnablePlcComm
        {
            get => _enablePlcComm;
            set => _enablePlcComm = value;
        }

        [Browsable(false)]
        public WriteShortAddressAsyncDelegate WriteShortAddressMethod
        {
            get => _writeShortAddressMethod;
            set => _writeShortAddressMethod = value;
        }

        [Browsable(false)]
        public ReadShortAddressAsyncDelegate ReadShortAddressMethod
        {
            get => _readShortAddressMethod;
            set => _readShortAddressMethod = value;
        }
        #endregion

        #region 公共属性 (外观与颜色)
        [Category("外观设置"), Description("下拉框圆角大小")]
        public int ButtonRadius
        {
            get => _buttonRadius;
            set { _buttonRadius = value; Invalidate(); }
        }

        [Category("颜色设置"), Description("主显示框的背景颜色")]
        public Color BoxBackColor
        {
            get => _boxBackColor;
            set { _boxBackColor = value; Invalidate(); }
        }

        [Category("颜色设置"), Description("主显示框的字体颜色")]
        public Color TextColor
        {
            get => _textColor;
            set { _textColor = value; Invalidate(); }
        }

        [Category("颜色设置"), Description("正常状态下的边框颜色")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("颜色设置"), Description("鼠标悬停或展开时的边框颜色")]
        public Color HoverBorderColor
        {
            get => _hoverBorderColor;
            set { _hoverBorderColor = value; Invalidate(); }
        }

        [Category("颜色设置"), Description("下拉列表展开时的整体背景颜色")]
        public Color DropdownBackColor
        {
            get => _dropdownBackColor;
            set { _dropdownBackColor = value; }
        }

        [Category("颜色设置"), Description("下拉列表中鼠标悬停或选中项的背景颜色")]
        public Color DropdownHoverBackColor
        {
            get => _dropdownHoverBackColor;
            set { _dropdownHoverBackColor = value; }
        }

        [Category("颜色设置"), Description("下拉列表中鼠标悬停或选中项的字体颜色")]
        public Color DropdownHoverTextColor
        {
            get => _dropdownHoverTextColor;
            set { _dropdownHoverTextColor = value; }
        }
        #endregion

        #region 构造函数
        public PlcComboBoxControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.Size = new Size(120, 35);
            this.BackColor = Color.Transparent;
            this.Font = new Font("微软雅黑", 10F, FontStyle.Regular);

            InitializeInnerComboBox();

            _updateTimer = new Timer();
            _updateTimer.Interval = 500;
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "PlcComboBoxControl";
            this.ResumeLayout(false);
        }
        #endregion

        #region 数据与同步
        private void UpdateSelectedIndexFromValue()
        {
            _selectedIndex = -1;
            if (_itemValues != null)
            {
                for (int i = 0; i < _itemValues.Length; i++)
                {
                    if (_itemValues[i] == _currentValue)
                    {
                        _selectedIndex = i;
                        break;
                    }
                }
            }
        }
        #endregion

        #region 重写重绘与鼠标事件
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            float penWidth = _isMouseOver || _isDroppedDown ? 2f : 1.5f;
            int margin = (int)Math.Ceiling(penWidth / 2.0f);

            Rectangle dropdownRect = new Rectangle(margin, margin, Width - margin * 2 - 1, Height - margin * 2 - 1);

            // 1. 绘制显示框区域背景
            using (SolidBrush brush = new SolidBrush(_boxBackColor))
            {
                GraphicsPath path = GetRoundedRectanglePath(dropdownRect, _buttonRadius);
                g.FillPath(brush, path);
            }

            // 2. 绘制边框
            Color currentBorderColor = _isMouseOver || _isDroppedDown ? _hoverBorderColor : _borderColor;
            using (Pen borderPen = new Pen(currentBorderColor, penWidth))
            {
                GraphicsPath path = GetRoundedRectanglePath(dropdownRect, _buttonRadius);
                g.DrawPath(borderPen, path);
            }

            // 3. 绘制当前选中的文本
            string displayText = "";
            if (_selectedIndex >= 0 && _itemNames != null && _selectedIndex < _itemNames.Length)
            {
                displayText = _itemNames[_selectedIndex];
            }

            Rectangle textRect = new Rectangle(dropdownRect.X + 8, dropdownRect.Y, dropdownRect.Width - 30, dropdownRect.Height);
            using (SolidBrush textBrush = new SolidBrush(_textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(displayText, Font, textBrush, textRect, sf);
            }

            // 4. 绘制右侧下拉箭头
            int arrowWidth = 10;
            int arrowHeight = 6;
            int arrowX = dropdownRect.Right - 15 - arrowWidth / 2;
            int arrowY = dropdownRect.Y + (dropdownRect.Height - arrowHeight) / 2;

            Point[] arrowPoints = new Point[]
            {
                new Point(arrowX, arrowY),
                new Point(arrowX + arrowWidth, arrowY),
                new Point(arrowX + arrowWidth / 2, arrowY + arrowHeight)
            };

            using (SolidBrush arrowBrush = new SolidBrush(currentBorderColor))
            {
                g.FillPolygon(arrowBrush, arrowPoints);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ActivateDropdownMode();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isMouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseOver = false;
            Invalidate();
        }
        #endregion

        #region 内部隐藏下拉框处理 (自绘实现分离)
        private void InitializeInnerComboBox()
        {
            _innerComboBox = new ComboBox();
            _innerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _innerComboBox.Font = this.Font;
            _innerComboBox.Visible = false;

            // 启用自定义绘制下拉项 (核心：接管下拉列表绘制，实现颜色分离)
            _innerComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            _innerComboBox.DrawItem += InnerComboBox_DrawItem;

            _innerComboBox.SelectionChangeCommitted += InnerComboBox_SelectionChangeCommitted;
            _innerComboBox.DropDownClosed += InnerComboBox_DropDownClosed;
            _innerComboBox.LostFocus += InnerComboBox_LostFocus;

            this.Controls.Add(_innerComboBox);
        }

        /// <summary>
        /// 自绘下拉列表项事件
        /// </summary>
        private void InnerComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // 判断当前是在绘制主显示框，还是在绘制展开后的下拉列表项
            bool isEditArea = (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit;
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor;
            Color textColor;

            // 分离逻辑：显示框使用 BoxBackColor，下拉列表使用 DropdownBackColor
            if (isEditArea)
            {
                backColor = _boxBackColor;
                textColor = _textColor;
            }
            else
            {
                backColor = isSelected ? _dropdownHoverBackColor : _dropdownBackColor;
                textColor = isSelected ? _dropdownHoverTextColor : _textColor;
            }

            // 1. 填充背景
            using (SolidBrush bgBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            // 2. 绘制文本
            string text = _innerComboBox.Items[e.Index].ToString();
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                Rectangle textRect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height);
                e.Graphics.DrawString(text, e.Font, textBrush, textRect, sf);
            }
        }

        private void ActivateDropdownMode()
        {
            _isDroppedDown = true;

            // 将内部背景设置为下拉列表背景，防止空余区域颜色异常
            _innerComboBox.BackColor = _dropdownBackColor;
            _innerComboBox.Font = this.Font;

            _innerComboBox.Items.Clear();
            if (_itemNames != null)
            {
                _innerComboBox.Items.AddRange(_itemNames);
            }

            _innerComboBox.Bounds = new Rectangle(0, (Height - _innerComboBox.Height) / 2, Width, _innerComboBox.Height);
            _innerComboBox.SelectedIndex = _selectedIndex >= 0 && _selectedIndex < _innerComboBox.Items.Count ? _selectedIndex : -1;

            _innerComboBox.Visible = true;
            _innerComboBox.BringToFront();
            _innerComboBox.Focus();
            _innerComboBox.DroppedDown = true;
        }

        private void InnerComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_innerComboBox.SelectedIndex >= 0 && _innerComboBox.SelectedIndex < _itemValues.Length)
            {
                short newValue = _itemValues[_innerComboBox.SelectedIndex];
                if (newValue != _currentValue)
                {
                    CurrentValue = newValue;

                    if (_enablePlcComm)
                    {
                        _ = WriteValueToAddressAsync(newValue);
                    }
                }
            }
        }

        private void InnerComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DeactivateDropdownMode();
        }

        private void InnerComboBox_LostFocus(object sender, EventArgs e)
        {
            if (!_innerComboBox.DroppedDown)
            {
                DeactivateDropdownMode();
            }
        }

        private void DeactivateDropdownMode()
        {
            _innerComboBox.Visible = false;
            _isDroppedDown = false;
            this.Invalidate();
        }
        #endregion

        #region PLC 通讯更新
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_enablePlcComm && !_isDroppedDown)
            {
                _ = UpdateValueFromAddressAsync();
            }
        }

        private async Task UpdateValueFromAddressAsync()
        {
            if (_readShortAddressMethod != null && !string.IsNullOrEmpty(_valueAddress))
            {
                try
                {
                    short addressValue = await _readShortAddressMethod(_valueAddress);
                    if (_currentValue != addressValue)
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new Action(() => CurrentValue = addressValue));
                        }
                        else
                        {
                            CurrentValue = addressValue;
                        }
                    }
                }
                catch (Exception)
                {
                    // 忽略异常或记录
                }
            }
        }

        private async Task WriteValueToAddressAsync(short value)
        {
            if (_writeShortAddressMethod != null && !string.IsNullOrEmpty(_valueAddress))
            {
                try
                {
                    await _writeShortAddressMethod(_valueAddress, value);
                }
                catch (Exception)
                {
                    // 忽略异常或记录
                }
            }
        }
        #endregion

        #region 图形辅助方法
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer.Tick -= UpdateTimer_Tick;
                _updateTimer?.Dispose();
                _updateTimer = null;

                if (_innerComboBox != null)
                {
                    // 释放自绘绑定的事件
                    _innerComboBox.DrawItem -= InnerComboBox_DrawItem;
                    _innerComboBox.SelectionChangeCommitted -= InnerComboBox_SelectionChangeCommitted;
                    _innerComboBox.DropDownClosed -= InnerComboBox_DropDownClosed;
                    _innerComboBox.LostFocus -= InnerComboBox_LostFocus;
                    _innerComboBox.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}