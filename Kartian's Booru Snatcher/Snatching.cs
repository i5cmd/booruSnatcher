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
    public class ImageData
    {
        public string FileUrl { get; set; }
        public string PreviewUrl { get; set; }
        public string SampleUrl { get; set; }
        public int PostId { get; set; }
        public string PostTags { get; set; }
        public string Rating { get; set; }
        public string SourceUrl { get; set; }
    }

    public class SnatchingResult
    {
        public int StatusCode { get; set; }
        public List<ImageData> Images { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorDescription { get; set; }
    }
    public class Snatching
    {
        CookieContainer cookieContainer;
        HttpClientHandler clientHandler;
        HttpClient client;

        public string link { get; set; }


        public void Declaration()
        {
            cookieContainer = new CookieContainer();
            clientHandler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                AllowAutoRedirect = false
            };
            client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:151.0) Gecko/20100101 Firefox/151.0");
            if (!link.Contains("http://") && !link.Contains("https://"))
            {
                link = "https://" + link + "/";
            }
            client.DefaultRequestHeaders.Add("Referer", link);
        }

        public async Task<SnatchingResult> RetrieveImages(string[] tags, int page, int limit, BooruConfiguration currentConfig, MainWindow window)
        {
            string tagsIncluded = string.Join("+", tags);
            string jsonLink = "";
            if (currentConfig.Engine == BooruEngine.Gelbooru)
            {
                jsonLink = $"{link}index.php?page=dapi&s=post&q=index&json=1&pid={page}&limit={limit}&tags={tagsIncluded}&api_key={currentConfig.Api}&user_id={currentConfig.UserId}";
            }
            else if (currentConfig.Engine == BooruEngine.Moebooru)
            {
                page++;
                jsonLink = $"{link}post.json?page={page}&limit={limit}&tags={tagsIncluded}&password_hash={currentConfig.Api}&login={currentConfig.UserId}";
            }
            else if (currentConfig.Engine == BooruEngine.Danbooru)
            {
                page++;
                jsonLink = $"{link}posts.json?page={page}&limit={limit}&tags={tagsIncluded}&api_key={currentConfig.Api}&login={currentConfig.UserId}";
            }
            HttpResponseMessage dx = await client.GetAsync(jsonLink);
            if (dx.StatusCode != HttpStatusCode.OK)
            {
                window.ShowDialog($"Failed to snatch images from {link}. It might be the issue with the current configuration or internal problem with the website.", "Snatching error");
                return new SnatchingResult
                {
                    Images = new List<ImageData>(),
                    StatusCode = (int)dx.StatusCode,
                    ErrorTitle = "Snatching error",
                    ErrorDescription = $"Failed to snatch images from {link}. It might be the issue with the current configuration or internal problem with the website. HTML Status Code: {dx.StatusCode.ToString()}"
                };

            }
            string dxJson = await dx.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(dxJson))
            {
                return new SnatchingResult
                {
                    Images = new List<ImageData>(),
                    StatusCode = (int)dx.StatusCode,
                    ErrorTitle = "Nobody here but us chickens!",
                    ErrorDescription = $"No posts were found for these tags. Ensure that all tags are correct."
                };
            }
            System.Diagnostics.Debug.WriteLine(dxJson);
            List<ImageData> images = new List<ImageData>();
            using JsonDocument imagesJson = JsonDocument.Parse(dxJson);
            JsonElement targetJson;
            if (imagesJson.RootElement.GetArrayLength() < 5)
            {
                return new SnatchingResult
                {
                    Images = new List<ImageData>(),
                    StatusCode = (int)dx.StatusCode,
                    ErrorTitle = "Nobody here but us chickens!",
                    ErrorDescription = $"No posts were found for these tags. Ensure that all tags are correct."
                };
            }
            if (imagesJson.RootElement.ValueKind == JsonValueKind.Object && currentConfig.Engine == BooruEngine.Gelbooru)
            {
                targetJson = imagesJson.RootElement.GetProperty("post");
            }
            else
            {
                targetJson = imagesJson.RootElement;
            }

            foreach (var el in targetJson.EnumerateArray())
            {
                string sample = "";
                if (el.GetProperty("sample_url").GetString().Trim() == "")
                {
                    sample = el.GetProperty("file_url").GetString().Replace(".com/", ".com//");
                }
                else
                {
                    sample = el.GetProperty("sample_url").GetString().Replace(".com/", ".com//");
                }
                images.Add(new ImageData
                {
                    FileUrl = el.GetProperty("file_url").GetString().Replace(".com/", ".com//"),
                    PreviewUrl = el.GetProperty("preview_url").GetString().Replace(".com/", ".com//"),
                    SampleUrl = sample,
                    PostId = el.GetProperty("id").GetInt32(),
                    PostTags = el.GetProperty("tags").GetString(),
                    Rating = el.GetProperty("rating").GetString(),
                    SourceUrl = el.GetProperty("source").GetString()
                });
            }
            ;
            return new SnatchingResult
            {
                Images = images,
                StatusCode = (int)dx.StatusCode,
                ErrorTitle = null,
                ErrorDescription = null
            };
        }
    }
}
