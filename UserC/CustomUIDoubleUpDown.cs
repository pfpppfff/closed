using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserC
{
    public partial class CustomUIDoubleUpDown : UserControl
    {
        // 按钮和灯的控件
        private UIButton _button;
        private UIDoubleUpDown _doubleUpDown;
        private UILabel _lableValveName;
        private UILabel _lableFBdegree;

        private Font _font = new Font("宋体", 9.0f);
        public CustomUIDoubleUpDown()
        {
            InitializeComponent();
            InitializeComponent1();
        }

        private void CustomUIDoubleUpDown_Load(object sender, EventArgs e)
        {

        }

        private void InitializeComponent1()
        {
            // 初始化按钮
            _button = new UIButton
            {
                Text = "确认",
                Font = _font,
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            ////  _buttonOpen.Click += Button_Click;
            //_buttonOpen.MouseDown += ButtonOpen_MouseDown;  // 绑定 MouseDown 事件
            //_buttonOpen.MouseUp += _ButtonOpen_MouseUp;

            _doubleUpDown = new UIDoubleUpDown
            {
               
                Font = _font,
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };

            _lableValveName = new UILabel
            {
                Text = "femane1;",
                Font = _font,
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            _lableFBdegree = new UILabel
            {
                Text = "100%",
                Font = _font,
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };

            //_buttonClose.MouseDown += _ButtonClose_MouseDown;
            //_buttonClose.MouseUp += _ButtonClose_MouseUp;


            // 使用 TableLayoutPanel 管理布局
            var layout = new TableLayoutPanel
            {
                ColumnCount = 1, // 设置两列
                RowCount = 2,    // 设置2行
                Dock = DockStyle.Fill,
                Margin = new Padding(3),
                Padding = new Padding(3)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            var layout1 = new TableLayoutPanel
            {
                ColumnCount = 3, 
                RowCount = 1,   
                Dock = DockStyle.Fill,
                Margin = new Padding(3),
                Padding = new Padding(3)
            };

            //// 设置列样式
            layout1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // 按钮占 50%
            layout1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // 灯占 50%
            layout1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // 灯占 50%

           

            // 添加控件到布局
            layout.Controls.Add(layout1, 0, 1);
            layout.Controls.Add(_lableValveName, 0, 0);

            layout1.Controls.Add(_lableFBdegree, 0, 0); // 按钮放在第一列
            layout1.Controls.Add(_doubleUpDown, 1, 0);  // 灯放在第二列
            layout1.Controls.Add(_button, 2, 0);
            // 确保AutoSize设置为true
            layout.AutoSize = true;
            layout1.AutoSize = true;

            // 使用AutoSizeMode确保内容可见
            layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            layout1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _button.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _doubleUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _lableValveName.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _lableFBdegree.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            // 将布局添加到控件
            this.Controls.Add(layout);

            // 设置控件大小
            this.Size = new Size(250, 80); // 默认大小
            this.AutoScaleMode = AutoScaleMode.Font;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // 如果需要额外的缩放逻辑，可以在这里实现
        }
    }
}
