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

        public TwitterAPI(string t_ConsumerKey, string t_ConsumerSecret, string t_AccessToken, string t_AccessTokenSecret)
        {
            this.t_ConsumerKey = t_ConsumerKey;
            this.t_AccessToken = t_AccessToken;
            this.t_AccessTokenSecret = t_AccessTokenSecret;
            signatureKey = string.Format("{0}&{1}", t_ConsumerSecret, t_AccessTokenSecret);        
        }

        public TwitterAPI(string t_ConsumerKey, string t_ConsumerSecret) : this(t_ConsumerKey, t_ConsumerSecret, "", "") { }

        public void getPINCode()
        {
            Diagnostics.Process.Start(API_BASE_URL + "oauth/authorize?oauth_token="
                + t_AccessToken + "&oauth_token_secret=" + t_AccessTokenSecret);
        }

        public Task<string> Tweet(string text)
        {
            var param = new Dictionary<string, string>
            {
                { "status",text },
                { "trim_user","1" }
            };
            return SendRequest("1.1/statuses/update.json", HttpMethod.Post, param);
        }

        private async Task<string> SendRequest(string endpoint, HttpMethod method, IEnumerable<KeyValuePair<String, String>> param = null)
        {
            if ((method.Equals(HttpMethod.Post) || method.Equals(HttpMethod.Put)) && param == null) throw new NullReferenceException();

            using (var http = new HttpClient())
            {
                var url = API_BASE_URL + endpoint;
                if (method.Equals(HttpMethod.Get) && param != null)
                {
                    url += "?" + string.Join("&", ToEscapedDictionary(param).Select(kvp => PairFormat("{0}={1}", kvp)));
                }
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = method
                };
                if (method.Equals(HttpMethod.Post))
                {
                    request.Content = new FormUrlEncodedContent(param);
                }

                var header = GenerateOAuthHeaderBase();
                header.Add("oauth_signature", GenerateSignature(url, method,
                    method.Equals(HttpMethod.Get) ? header : header.Concat(param)));
                request.Headers.Add("Authorization", GenerateOAuthHeader(header));

                var httpResp = await http.SendAsync(request);
                var respBody = await httpResp.Content.ReadAsStringAsync();

                return respBody;
            }
        }

        private Dictionary<String, String> GenerateOAuthHeaderBase()
        {
            var unixTimeStamp = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            var header = new Dictionary<String, String>
            {
                { "oauth_consumer_key", t_ConsumerKey },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", unixTimeStamp },
                { "oauth_nonce", GenerateOAuthNonce() },
                { "oauth_version", "1.0" },
            };
            if (!string.IsNullOrEmpty(t_AccessToken)) header.Add("oauth_token", t_AccessToken);
            return header;
        }

        private string GenerateOAuthHeader(Dictionary<String, String> header)
        {
            
            return "OAuth " + string.Join(", ", OrderByKey(ToEscapedDictionary(header)).Select(kvp => PairFormat("{0}=\"{1}\"", kvp)));
        }

        private string GenerateSignature(string url, HttpMethod method, IEnumerable<KeyValuePair<String, String>> param)
        {
            var hash = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            var builtParam = string.Join("&", OrderByKey(ToEscapedDictionary(param)).Select(kvp => PairFormat("{0}={1}", kvp)));
            var data = string.Format("{0}&{1}&{2}", method.ToString(), Uri.EscapeDataString(url), Uri.EscapeDataString(builtParam));
            var hasedData = hash.ComputeHash(Encoding.ASCII.GetBytes(data));

            return Convert.ToBase64String(hasedData);
        }

        private Dictionary<String, String> ToEscapedDictionary(IEnumerable<KeyValuePair<String, String>> src)
        {
            
            return src.ToDictionary(kvp => Uri.EscapeDataString(kvp.Key), kvp => Uri.EscapeDataString(kvp.Value));
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
    }
}
