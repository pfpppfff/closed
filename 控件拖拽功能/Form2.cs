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
using UcAsp.Opc;
using UserC;
using static 控件拖拽功能.Form1;

namespace 控件拖拽功能
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            customDoubleUpDown1.ReadEnable = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadButtonsFromFile();
            customInput2.ReadEnable = true;
            ConnectOPC();
        }
        private List<ButtonInfo> _savedButtons = new List<ButtonInfo>(); // 保存按钮信息的列表
        private string _saveFilePath = "buttons.json"; // 保存文件路径
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
        string[] strs ={
            //"1200.PLC1._System._NoError",
            //"1200.PLC1.DisData.Flow.Flow_1",
            //"1200.PLC1.DisData.Flow.Flow_2",
            //"1200.PLC1.DisData.Press.Inpress_1",
            //"1200.PLC1.DisData.Press.Outpress_1",
            //"1200.PLC1.DisData.PowerM.item1.Voltage",
            //"1200.PLC1.DisData.PowerM.item1.Current",
            //"1200.PLC1.DisData.PowerM.item1.Power",
            //"1200.PLC1.DisData.PowerM.item1.Powerf",
            //"1200.PLC1.DisData.PowerM.item1.Fry",
            "1200.PLC1.Val.Ctrl.item1.PDegree",
            "1200.PLC1.Val.Ctrl.item1.PDegree_FB",
            "1200.PLC1.Val.Ctrl.item2.PDegree",
            "1200.PLC1.Val.Ctrl.item2.PDegree_FB",
            "1200.PLC1.PowerCtrl.ABB.item1.PC_Start",
            "1200.PLC1.PowerCtrl.Fry.item1.PC_FryPowerCtrl",
            "1200.PLC1.PowerCtrl.Fry.item2.PC_FryPowerCtrl",
            "1200.PLC1.PowerCtrl.ABB.item1.PC_Start",
            
            "1200.PLC1.DisData.otherM.item1.EleSpeed"};
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

            //int is_MotorSel = (Int16)res[0].Value;
            //int is_PhaseSel = (Int16)res[1].Value;
            //bool bs_HeaterStart = (bool)res[2].Value;
            //bool bs_TempeStart = (bool)res[3].Value;

            float rs_PDegree1 = (float)res[0].Value;
            float rs_PDegreeFB1 = (float)res[1].Value;
            float rs_PDegree2 = (float)res[2].Value;
            float rs_PDegreeFB2 = (float)res[3].Value;
            bool bs_bool = (bool)res[4].Value;
            int bs_int = (Int16)res[5].Value;
            //bool bs_tempeValOpen_1 = bs_Q[0];
            //bool bs_tempeValClose_1 = bs_Q[1];
            //bool bs_tempeValOpen_2 = bs_Q[8];
            //bool bs_tempeValClose_2 = bs_Q[9];
            //bool bs_pipeValOpen_1 = bs_Q[10];
            //bool bs_pipeValClose_1 = bs_Q[11];
            //bool bs_pipeValOpen_2 = bs_Q[12];
            //bool bs_pipeValClose_2 = bs_Q[13];
            //bool bs_WaterValOpen = bs_Q[14];
            //bool bs_WaterValClose = bs_Q[15];
            //float rs_Tempe = (float)res[20].Value;
            //float rs_TempeSet = (float)res[21].Value;

            boolSwitch.Invoke(new Action(() => { boolSwitch.UpdateBoolStateFromExternal(bs_bool); }));
            intSwitch.Invoke(new Action(() => { intSwitch.UpdateIntStateFromExternal(bs_int); }));
            //customUIGataValve1.Invoke(new Action(() => { customUIGataValve1.UpdateStateFromExternal(bs_I[0], bs_I[1], bs_I[2], bs_I[3]); }));
            customDoubleUpDown1.Invoke(new Action(() => { customDoubleUpDown1.UpdateFromExternal(rs_PDegreeFB1); }));
            //degreeValue1.Invoke(new Action(() => { degreeValue1.UpdateFromExternal(rs_TempeSet); }));
        }
    }
}
