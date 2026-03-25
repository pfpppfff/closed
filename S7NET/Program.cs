using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S7NET
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 设置全局异常处理器
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "Application Thread Exception");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, "AppDomain Unhandled Exception");
        }

        private static void HandleException(Exception ex, string source)
        {
            try
            {
                var message = $"发生未处理的异常:\n\n来源: {source}\n异常类型: {ex?.GetType().Name}\n异常信息: {ex?.Message}";

                // 检查是否是连接相关异常
                if (ex != null && IsConnectionRelatedError(ex))
                {
                    message = "PLC连接异常，程序将继续运行。\n\n如果问题持续，请检查PLC连接状态。";
                }

                MessageBox.Show(message, "系统异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                // 如果异常处理本身出错，显示简单消息
                MessageBox.Show("发生系统异常，程序将继续运行。", "系统异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static bool IsConnectionRelatedError(Exception ex)
        {
            if (ex == null) return false;

            var message = ex.Message?.ToLower() ?? "";
            var typeName = ex.GetType().Name.ToLower();

            return message.Contains("连接") ||
                   message.Contains("远程主机") ||
                   message.Contains("plc") ||
                   typeName.Contains("plcexception") ||
                   typeName.Contains("socketexception") ||
                   ex is System.Net.Sockets.SocketException ||
                   ex is System.IO.IOException;
        }
    }
}
