using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kartian_s_Booru_Snatcher
{
    public class ImageData()
    {
        public string FileUrl { get; set; }
        public string PreviewUrl { get; set; }
        public string SampleUrl { get; set; }
        public int PostId { get; set; }
        public string PostTags { get; set; }
        public string Rating { get; set; }
        public string SourceUrl { get; set; }
    }
    public class Snatching
    {
        CookieContainer cookieContainer;
        HttpClientHandler clientHandler;
        HttpClient client;

        
        public void Declaration(string MainPageLink)
        {
            cookieContainer = new CookieContainer();
            clientHandler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                AllowAutoRedirect = true
            };
            client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:151.0) Gecko/20100101 Firefox/151.0");
            client.DefaultRequestHeaders.Add("Referer", MainPageLink);
        }

        public async Task<List<ImageData>> RetrieveImages(string[] tags, string link, int page, int limit)
        {
            string tagsIncluded = string.Join("+", tags);
            string jsonLink = $"https://safebooru.org/index.php?page=dapi&s=post&q=index&json=1&pid={page}&limit={limit}&tags={tagsIncluded}";
            HttpResponseMessage dx = await client.GetAsync(jsonLink);
            string dxJson = await dx.Content.ReadAsStringAsync();
            List<ImageData> images = new List<ImageData>();
            JsonDocument imagesJson = JsonDocument.Parse(dxJson);
            foreach (var el in imagesJson.RootElement.EnumerateArray())
            {
                images.Add(new ImageData
                {
                    FileUrl = el.GetProperty("file_url").GetString(),
                    PreviewUrl = el.GetProperty("preview_url").GetString(),
                    SampleUrl = el.GetProperty("sample_url").GetString(),
                    PostId = el.GetProperty("id").GetInt32(),
                    PostTags = el.GetProperty("tags").GetString(),
                    Rating = el.GetProperty("rating").GetString(),
                    SourceUrl = el.GetProperty("source").GetString()
                });
            };
            return images;
        }
    }
}
