using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Puut.Capture
{
    public class Upload
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        public async void DoUpload(Image image, String username, String password)
        {
            String host = Puut.Properties.Settings.Default.ServerURL;
            String url = Path.Combine(host, "upload").Replace(Path.DirectorySeparatorChar, '/');
            Console.WriteLine("Käsewurst");

            HttpResponseMessage response = await this.DoMultipartPost(url, image, username, password);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private async Task<HttpResponseMessage> DoMultipartPost(String url, Image image)
        {
            return await this.DoMultipartPost(url, image, null, null);
        }
        private async Task<HttpResponseMessage> DoMultipartPost(String url, Image image, String username, String password)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imageData = (byte[])converter.ConvertTo(image, typeof(byte[]));

            return await this.DoMultipartPost(url, imageData, username, password);
        }
        private async Task<HttpResponseMessage> DoMultipartPost(String url, byte[] image, String username, String password)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password)));

            var requestContent = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(image);
            requestContent.Add(imageContent, "image", "image.png");


            return await client.PostAsync(url, requestContent);
        }
    }
}
