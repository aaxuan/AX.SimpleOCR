using System;
using System.Text;
using System.Windows.Forms;

namespace AX.SimpleOCR
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //单实例运行检测
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out bool isRunOne);
            if (isRunOne == false)
            {
                MessageBox.Show("目前已有程序实例在运行，请勿重复运行程序");
                Environment.Exit(0);
            }

            // 设置 Winform 全局异常拦截
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler((sender, e) => { ShowExceptionMsg(e.Exception); });
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((sender, e) => { ShowExceptionMsg(e.ExceptionObject as System.Exception); });

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void ShowExceptionMsg(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"异常时间：{DateTime.Now.ToString("G")}");
            if (exception != null)
            {
                sb.AppendLine($"异常类型：{exception.GetType().FullName}");
                sb.AppendLine($"========== ========== ==========");
                sb.AppendLine($"异常信息：{exception.Message}");
                sb.AppendLine($"========== ========== ==========");
                sb.AppendLine($"异常堆栈：{exception.StackTrace}");
                sb.AppendLine($"========== ========== ==========");
                sb.AppendLine($"异常信息已复制到粘贴板。");
            }
            Clipboard.SetDataObject(exception.Message);
            MessageBox.Show(sb.ToString(), "全局异常信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}