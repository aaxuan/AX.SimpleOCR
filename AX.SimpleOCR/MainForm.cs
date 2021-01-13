﻿using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AX.SimpleOCR
{
    public partial class MainForm : Form
    {
        private Baidu.Aip.Ocr.Ocr BaiDuAIClient { get; set; }
        private Setting CurrentSetting { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        //窗体加载
        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = "AX.SimpleOCR - 简单截图 OCR";
            //界面
            notifyIcon.Visible = true; //显示托盘图标
            ShowInTaskbar = false;//图标不显示在任务栏
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //注册全局热键
            var result = RegisterHotKey(Handle, 99999, 2, Keys.Q);
            if (result != 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                SettoolStripStatusLabelText($"注册热键失败 注册返回:{result} 错误码:{errorCode}");
            }
            else
            {
                SettoolStripStatusLabelText($"注册热键成功 {result}");
            }

            //菜单项点击
            toolStripMenuItemScreenshot.Click += ToolStripMenuItemScreenshot_Click;
            toolStripMenuItemSetting.Click += ToolStripMenuItemSetting_Click;

            //初始化配置
            LoadSetting();
        }

        //工具栏-设置
        private void ToolStripMenuItemSetting_Click(object sender, EventArgs e)
        {
            var process = Process.Start("notepad.exe", Setting.SettingFileName);
            process.WaitForExit();
            LoadSetting();
        }

        //工具栏-截图
        private void ToolStripMenuItemScreenshot_Click(object sender, EventArgs e)
        {
            Ocr();
        }

        //任务栏图标双击
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        #region 内部方法

        private void LoadSetting()
        {
            CurrentSetting = Setting.Init(out string message);
            if (CurrentSetting == null)
            {
                MessageBox.Show(message, "读取配置文件异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SettoolStripStatusLabelText(message);
                return;
            }
            SettoolStripStatusLabelText(message);

            //初始
            BaiDuAIClient = new Baidu.Aip.Ocr.Ocr(CurrentSetting.ApiKey, CurrentSetting.SecretKey);
            BaiDuAIClient.Timeout = CurrentSetting.Timeout;
        }

        private void SettoolStripStatusLabelText(string message)
        {
            message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Debug.WriteLine(message);
            toolStripStatusLabel.Text = message;
        }

        public void Ocr()
        {
            // 是否隐藏窗体 隐藏后延时1s 防止截屏软件包含窗口残影
            if (CurrentSetting.OnScreenshotVisibleForm)
            { this.Visible = false; Thread.Sleep(1000); }

            // 清除粘贴板
            Clipboard.Clear();
            textBox.Clear();

            // 调用截图软件 并阻塞等待
            var snapShotProcess = Process.Start("SnapShot.exe");
            snapShotProcess.WaitForExit();

            // 从粘贴板读取截图 无图片则结束
            if (Clipboard.ContainsImage() == false) { this.Visible = true; return; }
            pictureBox.Image = Clipboard.GetImage();

            // 调用 OCR
            System.Diagnostics.Stopwatch watch = new Stopwatch();
            watch.Start();

            pictureBox.Image.Save("ocr_temporaryfile.png", System.Drawing.Imaging.ImageFormat.Png);
            var resultJObj = BaiDuAIClient.General(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ocr_temporaryfile.png")));
            var resultWodsJArry = JArray.Parse(resultJObj["words_result"].ToString());
            var sb = new StringBuilder("");
            foreach (var item in resultWodsJArry)
            { sb.AppendLine(item["words"].ToString()); }

            watch.Stop();

            // 显示结果
            textBox.Text = sb.ToString();
            this.Visible = true;

            // 自动复制到粘贴板
            Clipboard.SetText(sb.ToString());

            SettoolStripStatusLabelText($"识别成功 调用耗时:{watch.ElapsedMilliseconds.ToString()}ms");
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

        #endregion 全局热键

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //注销全局热键
            UnregisterHotKey(Handle, 99999);
            SettoolStripStatusLabelText("注销热键成功");
        }

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
    }
}