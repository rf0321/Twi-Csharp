using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.IO;
using System.Web;

//using System.Text;
//using System.Text.RegularExpressions;
using System.Xml;
namespace System.Twitter
{
    class TwitterAPI
    {
        private readonly string t_ConsumerKey; //APIたたくのにいるトークンキーたち
        private readonly string t_AccessToken;
        private readonly string t_AccessTokenSecret;
        private readonly string signatureKey;
        const string API_BASE_URL = "https://api.twitter.com/";

        public  TwitterAPI(string t_ConsumerKey, string t_ConsumerSecret, string t_AccessToken, string t_AccessTokenSecret)
        {
            Net.ServicePointManager.Expect100Continue = false;
            this.t_ConsumerKey = t_ConsumerKey;
            this.t_AccessToken = t_AccessToken;
            this.t_AccessTokenSecret = t_AccessTokenSecret;
            signatureKey = string.Format("{0}&{1}", Uri.EscapeDataString(t_ConsumerSecret), Uri.EscapeDataString(t_AccessTokenSecret));        
        }

        public Task<string> Tweet(string text)
        {
            var param = new Dictionary<string, string>
            {
                { "stauts",text },
                { "trim_user","1" }
            };
            return SendPostRequest("1.1/statuses/update.json", param);
        }

        private async Task<string> SendPostRequest(string endpoint, IEnumerable<KeyValuePair<String, String>> param)
        {
            var url = API_BASE_URL + endpoint;
            using (var http = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(param)
                };
                request.Headers.Add("Authorization", GenerateOAuthHeader(url, "POST", param));

                var httpResp = await http.SendAsync(request);
                var respBody = await httpResp.Content.ReadAsStringAsync();

                return respBody;
            }
        }

        private async Task<string> SendGetRequest(string endpoint)
        {
            var url = API_BASE_URL + endpoint;
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", GenerateOAuthHeader(url, "GET", null));

                var httpResp = await http.GetAsync(url);
                var respBody = await httpResp.Content.ReadAsStringAsync();

                return respBody;
            }
        }

        private string GenerateOAuthHeader(string url, string method, IEnumerable<KeyValuePair<String, String>> param)
        {
            var unixTimeStamp = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            var header = new Dictionary<String, String>
            {
                { "oauth_consumer_key", t_ConsumerKey },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", unixTimeStamp },
                { "oauth_nonce", GenerateOAuthNonce() },
                { "oauth_token", t_AccessToken },
                { "oauth_version", "1.0" },
            };
            header = ToEscapedDictionary(header);
            header.Add("oauth_signature", GenerateSignature(url, method, header.Concat(ToEscapedDictionary(param))));

            return "OAuth " + string.Join(", ", OrderByKey(header).Select(kvp => PairFormat("{0}=\"{1}\"", kvp)));
        }

        private string GenerateSignature(string url, string method, IEnumerable<KeyValuePair<String, String>> param)
        {
            var hash = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            var builtParam = string.Join("&", OrderByKey(param).Select(kvp => PairFormat("{0}={1}", kvp)));
            var data = string.Format("{0}&{1}&{2}", method, Uri.EscapeDataString(url), builtParam);
            var hasedData = hash.ComputeHash(Encoding.ASCII.GetBytes(data));

            return Convert.ToBase64String(hasedData);
        }

        private Dictionary<String, String> ToEscapedDictionary(IEnumerable<KeyValuePair<String, String>> src)
        {
            
            return src.ToDictionary(kvp => Uri.EscapeUriString(kvp.Key), kvp => Uri.EscapeUriString(kvp.Value));
        }

        private IEnumerable<KeyValuePair<String, String>> OrderByKey(IEnumerable<KeyValuePair<String, String>> src)
        {
            return src.OrderBy(kvp => kvp.Key);
        }

        private String GenerateOAuthNonce()
        {
            var random = new Random();
            return random.Next(123400, 9999999).ToString(); // 適当に拾ってきた
        }

        private String PairFormat(string format, KeyValuePair<String, String> src)
        {
            return string.Format(format, src.Key, src.Value);
        }

        /*public void getPINCode()
        {
            Diagnostics.Process.Start(API_BASE_URL + "oauth/authorize?oauth_token="
                + t_AccessToken + "&oauth_token_secret=" + t_AccessTokenSecret);
        }*/

        static void Main(string[] args)
        {
            var api = new TwitterAPI("", "", "", "");

            var task = api.Tweet("");
            task.Wait();
            Console.WriteLine(task.Result);
        }
    }
}
