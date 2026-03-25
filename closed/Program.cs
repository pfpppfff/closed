using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace closed
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new 重绘曲线1());
            //Application.Run(new 自制曲线图());
            //Application.Run(new 重绘曲线2());
             Application.Run(new 自动代码());
            //Application.Run(new 自动代码2());
            // Application.Run(new 自制曲线图());
        }
    }
}
