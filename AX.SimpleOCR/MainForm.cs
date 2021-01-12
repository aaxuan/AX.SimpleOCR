using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = "AX.SimpleOCR - 简单截图 OCR";

            //界面
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //菜单项点击
            toolStripMenuItemScreenshot.Click += ToolStripMenuItemScreenshot_Click;
            toolStripMenuItemSetting.Click += ToolStripMenuItemSetting_Click;

            //初始化配置
            LoadSetting();
        }

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

        private void ToolStripMenuItemSetting_Click(object sender, EventArgs e)
        {
           var process = Process.Start("notepad.exe", Setting.SettingFileName);
            process.WaitForExit();
            LoadSetting();
        }

        private void ToolStripMenuItemScreenshot_Click(object sender, EventArgs e)
        {
            Ocr();
        }

        private void SettoolStripStatusLabelText(string message)
        {
            toolStripStatusLabel.Text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
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
    }
}