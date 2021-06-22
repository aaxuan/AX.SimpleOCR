using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AX.SimpleOCR
{
    public class ConfigManager
    {
        public const string ConfigFileName = "setting.json";

        public static Config Config { get; }

        static ConfigManager()
        {
            // 这里考虑到，对于本应用程序来说，缺少配置项是致命的。所以 配置项不合理时会直接提示，然后退出应用程序。

            // 初始化默认配置文件
            // 文件不存在 则新建默认配置我文件。
            if (File.Exists(ConfigFileName) == false)
            { File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(new Config())); }

            //读取配置
            var configJson = File.ReadAllText(ConfigFileName);
            Config = JsonConvert.DeserializeObject<Config>(configJson);
            if (string.IsNullOrWhiteSpace(Config.ApiKey) || string.IsNullOrWhiteSpace(Config.SecretKey))
            {
                MessageBox.Show("您尚未配置调用密钥", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName));
                Environment.Exit(0);
            }
            return;
        }

        private ConfigManager()
        { }
    }

    public class Config
    {
        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int Timeout { get; set; } = 60000;
        public bool OnScreenshotVisibleForm { get; set; } = true;
    }
}