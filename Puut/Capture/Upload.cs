using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;

namespace Puut.Capture
{
    public class Upload
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        public async Task<String> DoUpload(Image image, String username, String password)
        {
            String host = Puut.Properties.Settings.Default.ServerURL;

            IRestResponse response = await this.DoMultipartPost(host, "upload", image, username, password);
            Console.WriteLine(response.Content);
            String id = this.ParseUploadResponse(response);

            return id;
        }

        private String ParseUploadResponse(IRestResponse response)
        {
            String content = response.Content;
            if ( response.StatusCode == System.Net.HttpStatusCode.OK )
            {
                if ( content.Contains("id") )
                {
                    Regex regex = new Regex("\"[a-zA-Z0-9]{4,}\"");
                    if ( regex.IsMatch(content) )
                    {
                        String id = regex.Match(content).Captures[0].Value;
                        id = id.Substring(1, id.Length - 2); // remove leading and trailing \"
                        return id;
                    }
                    return null;
                }
            }

            return null;
        }

        private async Task<IRestResponse> DoMultipartPost(String url, String path, Image image)
        {
            return await this.DoMultipartPost(url, path, image, null, null);
        }
        private async Task<IRestResponse> DoMultipartPost(String url, String path, Image image, String username, String password)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imageData = (byte[])converter.ConvertTo(image, typeof(byte[]));

            return await this.DoMultipartPost(url, path, imageData, username, password);
        }
        private async Task<IRestResponse> DoMultipartPost(String url, String path, byte[] image, String username, String password)
        {
            RestClient client = new RestClient(url);

            if(username != null && password != null)
                client.Authenticator = new HttpBasicAuthenticator(username, password);

            RestRequest request = new RestRequest(path, Method.POST);
            request.AddFile("image", image, "image.png", "image/png");
           
            request.AddHeader("Accept", "application/json");

            return await Task.Run<IRestResponse>(() =>
            {
                IRestResponse response = client.Execute(request);
                return response;
            }); ;
        }
    }
}
