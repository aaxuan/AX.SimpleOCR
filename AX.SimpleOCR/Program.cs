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
            //��ʵ�����м��
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out bool isRunOne);
            if (isRunOne == false)
            {
                MessageBox.Show("Ŀǰ���г���ʵ�������У������ظ����г���");
                Environment.Exit(0);
            }

            // ���� Winform ȫ���쳣����
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
            sb.AppendLine($"�쳣ʱ�䣺{DateTime.Now.ToString("G")}");
            if (exception != null)
            {
                sb.AppendLine($"�쳣���ͣ�{exception.GetType().FullName}");
                sb.AppendLine($"========== ========== ==========");
                sb.AppendLine($"�쳣��Ϣ��{exception.Message}");
                sb.AppendLine($"========== ========== ==========");
                sb.AppendLine($"�쳣��ջ��{exception.StackTrace}");
                sb.AppendLine($"========== ========== ==========");
                sb.AppendLine($"�쳣��Ϣ�Ѹ��Ƶ�ճ���塣");
            }
            Clipboard.SetDataObject(exception.Message);
            MessageBox.Show(sb.ToString(), "ȫ���쳣��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}