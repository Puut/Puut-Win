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
using RestSharp;

namespace Puut.Capture
{
    public class Upload
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        public async void DoUpload(Image image, String username, String password)
        {
            String host = Puut.Properties.Settings.Default.ServerURL;

            String response = await this.DoMultipartPost(host, "upload", image, username, password);
            Console.WriteLine(response);
        }

        private async Task<String> DoMultipartPost(String url, String path, Image image)
        {
            return await this.DoMultipartPost(url, path, image, null, null);
        }
        private async Task<String> DoMultipartPost(String url, String path, Image image, String username, String password)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imageData = (byte[])converter.ConvertTo(image, typeof(byte[]));

            return await this.DoMultipartPost(url, path, imageData, username, password);
        }
        private async Task<String> DoMultipartPost(String url, String path, byte[] image, String username, String password)
        {
            RestClient client = new RestClient(url);

            if(username != null && password != null)
                client.Authenticator = new HttpBasicAuthenticator(username, password);

            RestRequest request = new RestRequest(path, Method.POST);
            request.AddFile("image", image, "image.png", "image/png");
           
            request.AddHeader("Accept", "application/json");

            return await Task.Run<String>(() =>
            {
                IRestResponse response = client.Execute(request);
                return response.Content;
            }); ;
        }
    }
}
