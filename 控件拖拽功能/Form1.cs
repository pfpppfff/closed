using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UcAsp.Opc;
using UserC;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using Formatting = Newtonsoft.Json.Formatting;


namespace 控件拖拽功能
{
    public partial class Form1 : Form
    {
        private Button button134;

        public Form1()
        {
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadButtonsFromFile();

            customUIButtonLight2.ButtonClicked += CustomControl_ButtonClicked;
            customUIButtonLight1.ButtonClicked += CustomControl_ButtonClicked;

            customUIGataValve1.MouseDownUpEvent += CustomControl_MouseClicked;
            customUIGataValve2.MouseDownUpEvent += CustomControl_MouseClicked;
            ConnectOPC();
        }
        private void ConnectOPC()
        {
           
                if (OpcUa.objUa.Connect == OpcStatus.Connected)
                {
                    MessageBox.Show("连接成功！");
                OpenDataThread();
                }
                else
                {
                    
                    MessageBox.Show("连接失败！");

                }  
        }
       
        CancellationTokenSource cts = new CancellationTokenSource();
        string[] strs =
     { "1200.PLC1.Ctl.BtnMotor", "1200.PLC1.Ctl.BtnPh", "1200.PLC1.Ctl.BtnHeaterStart","1200.PLC1.Ctl.BtnTempeStart", "1200.PLC1.Ctl.BtnWaterValOpen"
         ,"1200.PLC1.Ctl.BtnWaterValClose","1200.PLC1.Ctl.VolSet", "1200.PLC1.Ctl.FrySet", "1200.PLC1.Ctl.VolRead", "1200.PLC1.Ctl.FryRead", "1200.PLC1.FB.I_1"
         ,"1200.PLC1.FB.Q_1","1200.PLC1.Ctl.BtnValTempeOpen_1","1200.PLC1.Ctl.BtnValTempeClose_1","1200.PLC1.Ctl.BtnValTempeOpen_2","1200.PLC1.Ctl.BtnValTempeClose_2"
         ,"1200.PLC1.Ctl.BtnPipeValOpen_1","1200.PLC1.Ctl.BtnPipeValClose_1","1200.PLC1.Ctl.BtnPipeValOpen_2","1200.PLC1.Ctl.BtnPipeValClose_2"
        ,"1200.PLC1.FB.Tempe","1200.PLC1.FB.TempeSet"};
        private void OpenDataThread()
        {
            Task.Run(new Action(() =>
            {
                while (cts.IsCancellationRequested == false)
                {
                    try
                    {
                        Read();
                        Thread.Sleep(100);
                    }
                    catch { }
                }

            }), cts.Token);
        }

        private void Read()
        {
            List<OpcItemValue> res = OpcUa.objUa.Read(strs);

            int is_MotorSel = (Int16)res[0].Value;
            int is_PhaseSel = (Int16)res[1].Value;
            bool bs_HeaterStart = (bool)res[2].Value;
            bool bs_TempeStart = (bool)res[3].Value;

            float rs_VolSet = (float)res[6].Value;
            float rs_frySet = (float)res[7].Value;
            float rs_VolRead = (float)res[8].Value;
            float rs_fryRead = (float)res[9].Value;
            bool[] bs_I = (bool[])res[10].Value;
            bool[] bs_Q = (bool[])res[11].Value;
            bool bs_tempeValOpen_1 = bs_Q[0];
            bool bs_tempeValClose_1 = bs_Q[1];
            bool bs_tempeValOpen_2 = bs_Q[8];
            bool bs_tempeValClose_2 = bs_Q[9];
            bool bs_pipeValOpen_1 = bs_Q[10];
            bool bs_pipeValClose_1 = bs_Q[11];
            bool bs_pipeValOpen_2 = bs_Q[12];
            bool bs_pipeValClose_2 = bs_Q[13];
            bool bs_WaterValOpen = bs_Q[14];
            bool bs_WaterValClose = bs_Q[15];
            float rs_Tempe = (float)res[20].Value;
            float rs_TempeSet = (float)res[21].Value;

            customUIButtonLight1.Invoke(new Action(() => { customUIButtonLight1.UpdateStateFromExternal(bs_HeaterStart); }));
            customUIGataValve1.Invoke(new Action(() => { customUIGataValve1.UpdateStateFromExternal(bs_I[0], bs_I[1], bs_I[2], bs_I[3]); }));
            //degreeValue.Invoke(new Action(() => { degreeValue.UpdateFromExternal(rs_Tempe); }));
            //degreeValue1.Invoke(new Action(() => { degreeValue1.UpdateFromExternal(rs_TempeSet); }));
        }
        private void CustomControl_ButtonClicked(object sender, CustomUIButtonLight.ButtonClickedEventArgs e)
        {
            //MessageBox.Show($"按钮被点击了！这是方法 B 的动作！读地址是: {e.ReadAddress}");
            if (e.ReadAddress == customUIButtonLight1.BtnName)
            {
                OpcUa.BoolSwith("1200.PLC1.Ctl.BtnHeaterStart");
            }

        }
        private void CustomControl_MouseClicked(object sender, CustomUIGateValve.MouseActionEventArgs e)
        {
           
            if(e.ReadAddress== customUIGataValve1.BtnOpenName)
            {
                if(e.ValveDir=="Down")
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValOpen_1", true);
                }
              
                else
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValOpen_1", false);
                }
            }
            else if(e.ReadAddress == customUIGataValve1.BtnCloseName)
            {
                if (e.ValveDir == "Down")
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValClose_1", true);
                }

                else
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValClose_1", false);
                }
            }
            if (e.ReadAddress == customUIGataValve2.BtnOpenName)
            {
                if (e.ValveDir == "Down")
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValOpen_2", true);
                }

                else
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValOpen_2", false);
                }
            }
            else if (e.ReadAddress == customUIGataValve2.BtnCloseName)
            {
                if (e.ValveDir == "Down")
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValClose_2", true);
                }

                else
                {
                    OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValClose_2", false);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Button bt = null;
            //for (int x = 0; x < int.Parse(textBox1.Text); x++)
            //{
            //    for (int y = 0; y < int.Parse(textBox2.Text); y++)
            //    {
            //        bt = new Button();
            //        bt.Location = new Point(50 * x, 30 * y); // 控件的位置，需要跟着数量的增加，位置也应该移动
            //        bt.Size = new Size(50, 30);
            //        bt.Name = "bt" + x.ToString() + y.ToString(); // 控件名称
            //        bt.Text = bt.Name; // 控件显示的名称
            //        this.Controls.Add(bt); // 创建在窗体上面显示
            //        bt.Click += new System.EventHandler(this.ck); // 关联事件
            //    }
            //}
        }

      

        private Point _dragStart ; // 用于存储拖拽开始的位置
        private Button _newButton = null; // 用于存储正在创建的按钮
        private Control _control ;
        private  void OnMouseDown(object sender, MouseEventArgs e)
        {
           
            //base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {

                _control = sender as Control;
                _dragStart = _control.PointToScreen(new Point(e.X, e.Y)); ; // 记录拖拽开始的位置
                _dragStart= this.PointToClient(_dragStart);
            
                _newButton = new Button(); // 创建一个新的按钮
              
                _newButton.Size = new Size(50, 30); // 设置按钮大小
                _newButton.Location = _dragStart; // 初始位置为鼠标按下位置
                _newButton.Name = "btDragged"; // 按钮名称
                _newButton.Text = "Drag Me"; // 按钮显示文本
                _newButton.TextAlign = ContentAlignment.BottomCenter;

                 // 为按钮添加右键菜单
                 ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem deleteItem = new ToolStripMenuItem("删除");
                deleteItem.Click += DeleteButton_Click; // 绑定删除事件
                contextMenu.Items.Add(deleteItem);
                _newButton.ContextMenuStrip = contextMenu;

                this.Controls.Add(_newButton); // 将按钮添加到窗体
            }

        }


       private  void OnMouseMove(object sender, MouseEventArgs e)
        {
            //base.OnMouseMove(e);
            if (!_dragStart.IsEmpty && e.Button == MouseButtons.Left)
            {
                // 如果正在拖拽，则更新按钮位置
                Control control = sender as Control;
                Point pointOnForm = control.PointToScreen(e.Location);
                pointOnForm = this.PointToClient(pointOnForm);
                _dragStart= pointOnForm;
             
                _newButton.Location = _dragStart;
             
            }
        }

        private  void OnMouseUp(object sender,MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left && _newButton != null)
            {
                // 当鼠标松开时，完成拖拽
                //_dragStart = null;
                // 弹出输入窗口
                using (InputForm inputForm = new InputForm())
                {
                    if (inputForm.ShowDialog() == DialogResult.OK)
                    {
                        // 获取用户输入的按钮名称和点击内容
                        string buttonName = inputForm.ButtonName;
                        string buttonClickContent = inputForm.ButtonClickContent;
                        int buttonWidth = inputForm.ButtonWidth;
                        int buttonHeight = inputForm.ButtonHeight;
                        // 设置按钮名称
                        _newButton.Name = buttonName;
                        _newButton.Text = buttonName;

                        // 注册按钮点击事件，并设置点击时的行为
                        _newButton.Click -= ck; // 移除旧的点击事件
                        _newButton.Click += (s, args) => MessageBox.Show(buttonClickContent);

                        // 将按钮信息保存到列表
                        _savedButtons.Add(new ButtonInfo
                        {
                            Name = buttonName,
                            X = _newButton.Location.X,
                            Y = _newButton.Location.Y,
                            Width = buttonWidth,
                            Height = buttonHeight,
                            ClickContent = buttonClickContent // 新增字段保存点击内容
                        });

                        // 保存按钮信息到文件
                        SaveButtonsToFile();
                    }
                    else
                    {
                        // 如果用户取消输入，则移除按钮
                        this.Controls.Remove(_newButton);
                        _newButton.Dispose();
                        _newButton = null;
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // 获取触发事件的菜单项
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null && menuItem.Owner is ContextMenuStrip contextMenu) // 确保 Owner 是 ContextMenuStrip
            {
                // 获取关联的按钮控件
                Control parentControl = contextMenu.SourceControl; // 使用 ContextMenuStrip 的 SourceControl 属性
                if (parentControl is Button button)
                {
                    // 从窗体中移除按钮
                    this.Controls.Remove(button);
                    button.Dispose();

                    // 如果需要保存按钮状态，还需要从保存的列表中移除该按钮的信息
                    _savedButtons.RemoveAll(b => b.Name == button.Name);
                    SaveButtonsToFile();
                }
            }
        }

        private void ck(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            string s = bt.Name; // 获取按下控件对应的名称
            listBox1.Items.Add(s); // 把按下控件的名称显示出来
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {

            OnMouseDown(sender,e);
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(sender,e);
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(sender, e);
        }

        public class ButtonInfo
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public string ClickContent { get; set; } // 新增字段保存点击内容
        }

        private List<ButtonInfo> _savedButtons = new List<ButtonInfo>(); // 保存按钮信息的列表
        private string _saveFilePath = "buttons.json"; // 保存文件路径

        //protected override void OnMouseUp(MouseEventArgs e)
        //{
          
        //}

        private void SaveButtonsToFile()
        {
            string json = JsonConvert.SerializeObject(_savedButtons, Formatting.Indented);
            File.WriteAllText(_saveFilePath, json);
        }
        private void LoadButtonsFromFile()
        {
            if (File.Exists(_saveFilePath))
            {
                string json = File.ReadAllText(_saveFilePath);
                _savedButtons = JsonConvert.DeserializeObject<List<ButtonInfo>>(json);

                foreach (var buttonInfo in _savedButtons)
                {
                    Button btn = new Button
                    {
                        Name = buttonInfo.Name,
                        Location = new Point(buttonInfo.X, buttonInfo.Y),
                        Size = new Size(buttonInfo.Width, buttonInfo.Height),
                        Text = buttonInfo.Name
                    };

                    this.Controls.Add(btn); // 将按钮添加到窗体

                    // 设置按钮点击事件
                    btn.Click += (s, args) => boolWITER(buttonInfo.ClickContent);
                }
            }
        }

        private void boolWITER(string sts)
        {
            MessageBox.Show(sts);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")] //导入user32.dll函数库
        public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);//获取鼠标坐标

        private void button4_Click(object sender, EventArgs e)
        {
          
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
           
           
        }

        private void button3_Click(object sender, EventArgs e)
        {

          customUIButtonLight2.UpdateStateFromExternal(true);
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValOpen_1", true);
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            OpcUa.BoolWrite("1200.PLC1.Ctl.BtnPipeValOpen_1", false);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void userControl21_Load(object sender, EventArgs e)
        {

        }
    }
}
