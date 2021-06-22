using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Text;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ocr.V20181119;
using TencentCloud.Ocr.V20181119.Models;

namespace AX.SimpleOCR.OCRProvider
{
    public class TencentCloudOCRProvider : BaseOCRProvider
    {
        private static Credential Credential
        {
            get
            {
                return new Credential
                {
                    SecretId = ConfigManager.Config.ApiKey,
                    SecretKey = ConfigManager.Config.SecretKey
                };
            }
        }

        private static ClientProfile ClientProfile = new ClientProfile();

        private static HttpProfile HttpProfile = new HttpProfile() { Endpoint = "ocr.tencentcloudapi.com" };

        public override string OCR(Image image)
        {
            ClientProfile.HttpProfile = HttpProfile;

            OcrClient client = new OcrClient(Credential, "ap-beijing", ClientProfile);
            AdvertiseOCRRequest req = new AdvertiseOCRRequest();

            var imageByte = ImageToByte(image);
            req.ImageBase64 = Convert.ToBase64String(imageByte);

            AdvertiseOCRResponse resp = client.AdvertiseOCRSync(req);
            var resultJobj = JObject.Parse(AbstractModel.ToJsonString(resp));

            var resultWodsJArry = JArray.Parse(resultJobj["TextDetections"].ToString());
            var sb = new StringBuilder("");
            foreach (var item in resultWodsJArry)
            { sb.AppendLine(item["DetectedText"].ToString()); }

            return sb.ToString();
        }
    }
}