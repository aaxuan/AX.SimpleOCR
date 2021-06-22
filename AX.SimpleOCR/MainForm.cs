using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AX.SimpleOCR
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //窗体加载
        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = "AX.SimpleOCR - 简单截图 OCR";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            //界面
            //notifyIcon.Visible = true; //显示托盘图标
            //ShowInTaskbar = false;//图标不显示在任务栏
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //注册全局热键
            //var result = RegisterHotKey(Handle, 99999, 2, Keys.Q);
            //if (result != 0)
            //{
            //    var errorCode = Marshal.GetLastWin32Error();
            //    SettoolStripStatusLabelText($"注册热键失败  注册返回:{result}  错误码:{errorCode}");
            //}
            //else
            //{
            //    SettoolStripStatusLabelText($"注册热键成功 {result}");
            //}

            //菜单项点击
            toolStripMenuItemScreenshot.Click += ToolStripMenuItemScreenshot_Click;
        }

        //窗体关闭
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //注销全局热键
            UnregisterHotKey(Handle, 99999);
            SettoolStripStatusLabelText("注销热键成功");
        }

        //工具栏-截图
        private void ToolStripMenuItemScreenshot_Click(object sender, EventArgs e)
        {
            Ocr();
        }

        //工具栏-退出
        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        //任务栏图标双击
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                Show();
                Focus();
            }
        }

        //窗体尺寸变化
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        #region 内部方法

        private void SettoolStripStatusLabelText(string message)
        {
            message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Debug.WriteLine(message);
            toolStripStatusLabel.Text = message;
        }

        public void Ocr()
        {
            // 是否隐藏窗体 隐藏后延时1s 防止截屏软件包含窗口残影
            if (ConfigManager.Config.OnScreenshotVisibleForm)
            { this.Visible = false; Thread.Sleep(1000); }

            // 清除粘贴板
            Clipboard.Clear();
            textBox.Clear();

            // 调用截图软件 并阻塞等待
            var snapShotProcess = Process.Start("SnapShot.exe");
            snapShotProcess.WaitForExit();

            // 从粘贴板读取截图 无图片则结束
            if (Clipboard.ContainsImage() == false) { this.Visible = true; SettoolStripStatusLabelText("未读取到图片，请重试"); return; }
            pictureBox.Image = Clipboard.GetImage();

            // 调用 OCR
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var result = new OCRProvider.TencentCloudOCRProvider().OCR(pictureBox.Image);
            watch.Stop();

            //显示结果
            textBox.Text = result;
            this.Visible = true;

            // 自动复制到粘贴板
            Clipboard.SetText(result);
            SettoolStripStatusLabelText($"识别成功 调用耗时:{watch.ElapsedMilliseconds}ms 已复制到粘贴板");
        }

        #endregion 内部方法

        #region 全局热键

        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄
            int id,                     //定义热键ID（不能与其它ID重复）
            int key,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk                     //定义热键的内容
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                if (m.WParam.ToInt32() == 99999)
                {
                    SettoolStripStatusLabelText("触发全局快捷键");
                    Ocr();
                }
            }

            base.WndProc(ref m);
        }

        #endregion 全局热键
    }
}