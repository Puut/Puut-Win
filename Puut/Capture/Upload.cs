using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Puut.Capture
{
    public class Upload
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        public async void DoUpload(Image image)
        {
            String host = Puut.Properties.Settings.Default.ServerURL;
            String url = Path.Combine(host, "upload").Replace(Path.DirectorySeparatorChar, '/');

            String response = await this.DoMultipartPost(url, image);

            Console.WriteLine(response);
        }

        private async Task<String> DoMultipartPost(String url, Image image)
        {
            return await this.DoMultipartPost(url, image, null, null);
        }
        private async Task<String> DoMultipartPost(String url, Image image, String username, String password)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imageData = (byte[])converter.ConvertTo(image, typeof(byte[]));

            return await this.DoMultipartPost(url, imageData, username, password);
        }
        private async Task<String> DoMultipartPost(String url, byte[] image, String username, String password)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            MultipartFormDataContent content = new MultipartFormDataContent();
            ByteArrayContent file = new ByteArrayContent(image);
            file.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                Name = "image",
                FileName = "image",
                FileNameStar = "image",
                Size = image.LongLength
            };
            file.Headers.ContentLength = image.LongLength;
            file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            content.Add(file);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentEncoding.Add("utf-8");

            HttpResponseMessage response = await client.PostAsync(url, content);
            String s = await response.Content.ReadAsStringAsync();

            return s;
        }
    }
}
