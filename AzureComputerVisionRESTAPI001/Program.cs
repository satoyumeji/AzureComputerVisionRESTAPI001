using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureComputerVisionRESTAPI001
{
    static class Program
    {
        static string subscriptionKey = "API_KEY"; // APIキー
        static string endpoint = "END_POINT"; // エンドポイント
        static string uriBase = endpoint + "vision/v3.1/analyze";

        static string imageName = @"FILE_NAME"; // ファイル名
        // パス、プロジェクトのbinフォルダまで
        static string imageFilePath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()))
            + "/" + imageName;

        public static void Main()
        {
            // API接続
            MakeAnalysisRequest(imageFilePath).Wait();
        }

        static async Task MakeAnalysisRequest(string imageFilePath)
        {
            try
            {
                // Http通信接続用クラス
                HttpClient client = new HttpClient();

                // APIキーの設定
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // 分析の追加
                // 公式Doc　https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.azure.cognitiveservices.vision.computervision.models.visualfeaturetypes?view=azure-dotnet
                string requestParameters =
                    "visualFeatures=Categories,Description,Color";

                // リクエストURI
                string uri = uriBase + "?" + requestParameters;

                // レスポンス受け取り用クラス
                HttpResponseMessage response;

                // 画像をバイナリデータに変換
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // リクエスト発行
                using (ByteArrayContent content = new ByteArrayContent(byteData)) // imageバイナリデータを通信用のデータに変換
                {
                    // レスポンスデータのファイルタイプ
                    // 下記の指定だとJSONファイル形式
                    // Doc https://developer.mozilla.org/ja/docs/Web/HTTP/Headers/Content-Type
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // リクエスト発行
                    response = await client.PostAsync(uri, content);
                }

                // レスポンスのJSONファイルを文字列として受け取る
                string contentString = await response.Content.ReadAsStringAsync();

                // 文字列をJSONファイル形式で出力
                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        // 画像バイナリ変換用メソッド
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // バイナリ変換
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
