using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserDefinedControl.OPCUA;

namespace UserDefinedControl
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        private IOpcUaService _opcService;
        private void Form3_Load(object sender, EventArgs e)
        {
            _opcService = OpcUaServiceManager.Current;
            var  plc1Status = _opcService.GetPlcStatus("PLC1");
        }
    }
}
