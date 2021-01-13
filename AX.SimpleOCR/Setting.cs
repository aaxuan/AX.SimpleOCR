using Newtonsoft.Json;
using System.IO;

namespace AX.SimpleOCR
{
    public class Setting
    {
        [JsonIgnore]
        public static readonly string SettingFileName = "setting.json";

        // 初始化默认配置文件
        public static void InitSettingFile()
        {
            if (File.Exists(SettingFileName) == false)
            { File.WriteAllText(SettingFileName, JsonConvert.SerializeObject(new Setting())); }
        }

        //尝试初始化配置
        public static Setting Init(out string message)
        {
            //创建配置文件
            InitSettingFile();
            //读取配置
            var settingJsonStr = File.ReadAllText(SettingFileName);
            Setting setting = JsonConvert.DeserializeObject<Setting>(settingJsonStr);
            if (string.IsNullOrWhiteSpace(setting.ApiKey) || string.IsNullOrWhiteSpace(setting.SecretKey))
            { message = "请配置 ApiKey / SecretKey"; return null; }
            message = "读取配置成功";
            return setting;
        }

        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int Timeout { get; set; } = 60000;
        public bool OnScreenshotVisibleForm { get; set; } = true;
    }
}