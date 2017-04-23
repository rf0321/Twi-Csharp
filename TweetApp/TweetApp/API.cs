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
       private readonly string t_ConsumerSecret;
       private readonly string t_AccessToken;
       private readonly string t_AccessTokenSecret;
        const string TwitterApiBaseUrl = "https://api.twitter.com/1.1/";
      

        readonly HMACSHA1 SignatureHashCode;
        readonly DateTime UnixTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public  TwitterAPI(string t_ConsumerKey, string t_ConsumerSecret, string t_AccessToken, string t_AccessTokenSecret)
        {
            this.t_ConsumerKey = t_ConsumerKey;
            this.t_ConsumerSecret = t_ConsumerSecret;
            this.t_AccessToken = t_AccessToken;
            this.t_AccessTokenSecret = t_AccessTokenSecret;

            SignatureHashCode = new HMACSHA1(new ASCIIEncoding().GetBytes(string.Format("{0}&{1}", t_ConsumerSecret,t_AccessTokenSecret)));         
        }
        public Task<string>Tweet(string text)
        {
            var DATA = new Dictionary<string, string>{};
			DATA.Add("status", text);
			DATA.Add("trim_user", "1");

            return SEND_REQUEST("statuses/update.json",DATA);
        }
        Task<string>SEND_REQUEST(string URL, Dictionary<string, string> DATA)
        {
            var Full_URL = TwitterApiBaseUrl + URL;
            var timestamp = (int)((DateTime.UtcNow-UnixTimeStamp).TotalSeconds);
             DATA.Add("oauth_consumer_key", t_ConsumerKey);
             DATA.Add("oauth_signature_method", "HMAC-SHA1");
             DATA.Add("oauth_timestamp", timestamp.ToString());
             DATA.Add("oauth_nonce", "a");
             DATA.Add("oauth_token", t_AccessToken);
             DATA.Add("oauth_version", "1.0");

            DATA.Add("oauth_signature", GenerateSignature(Full_URL, DATA));
            string oAuthHeader = GenerateOAuthHeader(DATA);
            var formData = new FormUrlEncodedContent(DATA.Where(kvp => !kvp.Key.StartsWith("oauth_")));

            return SEND_REQUEST(Full_URL, oAuthHeader, formData);
        }
        string GenerateSignature(string URL, Dictionary<string, string> DATA)
        {
            var SigString = string.Join("&", DATA.Union(DATA).Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key),
                Uri.EscapeDataString(kvp.Value))).OrderBy(s => s));

            var FULL_SIGDATA=string.Format("{0}&{1}&{2}","POST", Uri.EscapeDataString(URL),
            Uri.EscapeDataString(SigString.ToString()));
            return Convert.ToBase64String(SignatureHashCode.ComputeHash(new ASCIIEncoding().GetBytes(FULL_SIGDATA.ToString())));
        }
        string GenerateOAuthHeader(Dictionary<string, string> DATA)
        {
            return "OAuth " + string.Join(", ",DATA
            .Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
            .OrderBy(s => s));
        }
        async Task<string> SEND_REQUEST(string FUll_URL, string oAuthHeader, FormUrlEncodedContent formData)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", oAuthHeader);

                var httpResp = await http.PostAsync(FUll_URL, formData);
                var respBody = await httpResp.Content.ReadAsStringAsync();

                return respBody;
            }
        }
    }
}
