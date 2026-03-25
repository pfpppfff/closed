using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static closed.Mbus;


namespace closed
{
    public partial class mudbus : Form
    {
        public mudbus()
        {
            InitializeComponent();
        }

       Mbus.Comm comm1 = new Mbus.Comm();
        private SerialPort serialPort2;
        private void Form3_Load(object sender, EventArgs e)
        {
            comm1 = new Comm()
            {
                Com = "COM1",
                BaudRate =  9600 ,
                DataBits =  8 ,
                Parity =Parity.None,
                StopBits = StopBits.One,
                SlaveId = 1,
                FunctionCode = 3,
                StartAddress = 100,
                NumDataRegisters = 3,
                RecNumByte = 11,
            };
            Mbus.Init(serialPort2, comm1);
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comm1.BaudRate = 333;
            comm1.Com = "COM2";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

          
            Task.Run(new Action(() =>
            {
                var readvalueArry = Mbus.RtuRead(comm1);
                UInt16[] readvalue = readvalueArry.Item1;
                bool portIsOpen = readvalueArry.Item2;                     
            }));
            Thread.Sleep(200);
            int fff = 1;
            bool fsfg = false;
        }
    }
}
