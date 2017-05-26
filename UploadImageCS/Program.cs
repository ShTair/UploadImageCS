using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace UploadImageCS
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var stream = File.OpenRead(@"アップロードするファイルパス"))
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");

                var content = new MultipartFormDataContent();
                //content.Add(new StreamContent(stream), "uploadfile", fileName);
                ////パターンA↑　本来はこれでよい。
                content.Add(CreateStreamContent(stream, fileName + ".jpg")); //←拡張子必須
                content.Add(new StringContent(fileName), "title");

                var handler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential("ユーザ名", "パスワード"),
                };

                using (var hc = new HttpClient(handler))
                {
                    var reqContentString = content.ReadAsStringAsync().Result;

                    var res = hc.PostAsync("http://cs.nagao.nuie.nagoya-u.ac.jp/upload/images", content).Result;
                    var resContentString = res.Content.ReadAsStringAsync().Result;
                }
            }
        }

        private static StreamContent CreateStreamContent(Stream stream, string fileName)
        {
            var content = new StreamContent(stream);
            //content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            //{
            //    Name = "uploadfile",
            //    FileName = fileName,
            //};
            ////パターンA'↑　パターンAでうまくいかないときはこうする。ほぼパターンAと同じ意味。
            ////しかしこれでもうまくいかないので
            content.Headers.Add("Content-Disposition", "form-data; name=\"uploadfile\"; filename=\"" + fileName + "\"");
            ////パターンB↑　ヘッダに直接書く。
            ////パターンA'だと、form-data; name=uploadfile; filename=fileNameというヘッダになる。
            ////なぜかダブルクオーテーション必須で、form-data; name="uploadfile"; filename="fileName"にしないといけない。
            content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return content;
        }
    }
}
