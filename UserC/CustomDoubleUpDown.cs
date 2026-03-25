using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 控件拖拽功能;
using static UserC.CustomUIGateValve;

namespace UserC
{
    public partial class CustomDoubleUpDown : UserControl
    {
        private bool _readEnable=false;
        private string _controlName;
        private double _outValue;
        private double _fbValue;
        private double _step=1.0;
        private string _writeAdr;
        private double _maxValue=10000;
        private double _minValue=0;
        private string _dataUnit="";
        // 定义一个事件，用于通知_readEnable属性的变化
        public event EventHandler ReadEnableChanged;
        public CustomDoubleUpDown()
        {
            InitializeComponent();
        
            txtWrite.ValueChanged += TxtWrite_ValueChanged;
            
        }

      

        private void TxtWrite_ValueChanged(object sender, double value)
        {
            if (!_readEnable) 
                return;
            if (value < 0)
            {
                _outValue = 0;
            }
            else if (value > 0)
            {
                _outValue = 100;
            }
            else {
                _outValue = value;
            }
            OpcUa.FloatWrite(WriteAdr,Convert.ToSingle(Math.Round(value,2)));
        }

      

        private void uiLabel1_Click(object sender, EventArgs e)
        {

        }
        [Browsable(true)]
        [Category("Custom")]
        [Description("读取使能")]
        public bool ReadEnable
        {
            get => _readEnable;
            set
            {
                _readEnable = value;
                OnReadEnableChanged(EventArgs.Empty);
            }
        }


        [Browsable(true)]
        [Category("Custom")]
        [Description("控件名")]
        public string   ControlName
        {
            get => _controlName;
            set
            {
                _controlName = value;
               labelName.Text = value;
            }
        }

        [Browsable(true)]
        [Category("Custom")]
        [Description("增减的步长值")]
        public Double Step
        {
            get => _step;
            set
            {
                _step = value;
               txtWrite.Step = _step;
            }
        }
       

        [Browsable(true)]
        [Category("Custom")]
        [Description("输出double")]
        public Double OutValue
        {
            get => _outValue;
            set
            {
                _outValue = value;               
            }
        }

        [Browsable(true)]
        [Category("Custom")]
        [Description("反馈值")]
        public Double FBValue
        {
            get => _fbValue;
            set
            {
                _fbValue = value;
            }
        }
        [Browsable(true)]
        [Category("Custom")]
        [Description("写入地址")]
        public string WriteAdr
        {
            get => _writeAdr;
            set
            {
                _writeAdr = value;
            }
        }

        [Browsable(true)]
        [Category("Custom")]
        [Description("最大值")]
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                txtWrite.Maximum = _maxValue;
           
            }
        }

        [Browsable(true)]
        [Category("Custom")]
        [Description("最小值")]
        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                txtWrite.Minimum = _minValue;
              
            }
        }

        [Browsable(true)]
        [Category("Custom")]
        [Description("单位")]
        public string DataUnit
        {
            get => _dataUnit;
            set
            {
                _dataUnit = value;
               

            }
        }

        public void FisrtUpdateFromExternal()
        {
            if (_readEnable) {
                txtWrite.Value = OpcUa.ReadFloatOP(_writeAdr);
            }
            else {
                txtWrite.Value = 0d;
            }
            
        }
        public void UpdateFromExternal(double fbValue)
        {      
            if (!_readEnable) 
                return;
            _fbValue = fbValue;
          _labelFBValue.Text =Double.IsNaN(fbValue) != true ? $"{fbValue.ToString("0.0")}{_dataUnit}" : $"0{_dataUnit}";
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            FisrtUpdateFromExternal();
            this.ReadEnableChanged += OnReadEnableChangedHandler;
        }
        // 触发事件的方法
        protected virtual void OnReadEnableChanged(EventArgs e)
        {
            ReadEnableChanged?.Invoke(this, e);
        }
        private void OnReadEnableChangedHandler(object sender, EventArgs e)
        {
            FisrtUpdateFromExternal();
        }
    }   
}
