using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 控件拖拽功能;
using static UserC.CustomGateValveBtn;

namespace UserC
{
    public partial class CustomInput : UserControl
    {
        private bool _readEnable = false;
        private string _controlName;
        private double _outValue;
      
        private double _step = 1.0;
        private int _decimiPlace = 1;
        private string _writeAdr;

        // 定义一个事件，用于通知_readEnable属性的变化
        public event EventHandler ReadEnableChanged;
        public CustomInput()
        {
            InitializeComponent();
            txtWrite.ValueChanged += TxtWrite_ValueChanged;
        }
        private void TxtWrite_ValueChanged(object sender, double value)
        {
            if (!_readEnable)
                return;
            switch (_dataType)
            {
                case DataTypeOptions.flaotType:
                    OpcUa.FloatWrite(WriteAdr, Convert.ToSingle(Math.Round(value, 2)));
                    break;
                case DataTypeOptions.intType:
                    OpcUa.IntWrite(WriteAdr, (Int16)value);
                    break;

            }
           
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
        public string ControlName
        {
            get => _controlName;
            set
            {
                _controlName = value;
              
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
        [Description("小数点")]
        public int DecimiPlace
        {
            get => _decimiPlace;
            set
            {
                _decimiPlace = value;
                switch(_dataType)
                {
                    case DataTypeOptions.flaotType:
                        txtWrite.DecimalPlaces = _decimiPlace;
                        break;
                    case DataTypeOptions.intType:
                        txtWrite.DecimalPlaces = 0;
                        break;
                }
              
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
        [Description("写入地址")]
        public string WriteAdr
        {
            get => _writeAdr;
            set
            {
                _writeAdr = value;
            }
        }

        public enum DataTypeOptions
        {
            [Description("int")]
            intType,
            [Description("float")]
            flaotType
        }
        private DataTypeOptions _dataType = DataTypeOptions.flaotType;

        // 属性：写地址
        [Category("Custom")]
        [Description("控制模式选择")]
        public DataTypeOptions DataType
        {
            get => _dataType;
            set
            {
                _dataType = value;
              
            }
        }

        public void FisrtUpdateFromExternal()
        {
            if (_readEnable)
            {
                switch (_dataType)
                {
                    case DataTypeOptions.flaotType:
                        txtWrite.Value = OpcUa.ReadFloatOP(_writeAdr);
                        break;
                    case DataTypeOptions.intType:
                        txtWrite.Value = (double)OpcUa.ReadIntOP(_writeAdr);
                        break;
                }
               
            }
            else
            {
                txtWrite.Value = 0d;
            }

        }
        public void UpdateFromExternal(double fbValue)
        {
           
          
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
