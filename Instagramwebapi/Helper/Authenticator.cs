using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace Instagramwebapi.Helper
{
    class Authenticator : BaseRequest
    {
        private const string BaseUrl = "https://www.instagram.com/";
        private const string LoginUrl = "https://www.instagram.com/accounts/login/ajax/";
        private static Random random = new Random();
        private List<string> listOfUserAgents = new List<string> {
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; FSL 7.0.6.01001)",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; FSL 7.0.7.01001)",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; FSL 7.0.5.01003)",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0",
                "Mozilla/5.0 (X11; U; Linux x86_64; de; rv:1.9.2.8) Gecko/20100723 Ubuntu/10.04 (lucid) Firefox/3.6.8",
                "Mozilla/5.0 (Windows NT 5.1; rv:13.0) Gecko/20100101 Firefox/13.0.1",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0",
                "Mozilla/5.0 (X11; U; Linux x86_64; de; rv:1.9.2.8) Gecko/20100723 Ubuntu/10.04 (lucid) Firefox/3.6.8",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.0.3705)",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)",
                "Opera/9.80 (Windows NT 5.1; U; en) Presto/2.10.289 Version/12.01",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)",
                "Mozilla/5.0 (Windows NT 5.1; rv:5.0.1) Gecko/20100101 Firefox/5.0.1",
                "Mozilla/5.0 (Windows NT 6.1; rv:5.0) Gecko/20100101 Firefox/5.02",
                "Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.112 Safari/535.1",
                "Mozilla/4.0 (compatible; MSIE 6.0; MSIE 5.5; Windows NT 5.0) Opera 7.02 Bork-edition [en]",
            };

        public async Task<HttpResponseMessage> Authenticate()
        {
            string CSRFToken = GetCSRFToken().Result;
            Console.WriteLine($"csrf_token: {CSRFToken}");

            int randomNumber = random.Next(listOfUserAgents.Count);

            var body = new Dictionary<string, string>()
            {
                { "username", Environment.GetEnvironmentVariable("instagram_username") },
                { "password", Environment.GetEnvironmentVariable("instagram_password") }
            };

            var content = new FormUrlEncodedContent(body);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(LoginUrl),
                Headers = {
                        { HttpRequestHeader.Accept.ToString(), "*/*" },
                        { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                        { HttpRequestHeader.Connection.ToString(), "keep-alive" },
                        { HttpRequestHeader.Host.ToString(), "www.instagram.com" },
                        { HttpRequestHeader.UserAgent.ToString(), listOfUserAgents.ElementAt(randomNumber) },
                        { "Origin", "https://www.instagram.com" },
                        { "x-requested-with", "application/json" },
                        { "X-Instagram-AJAX", "1" },
                        { "X-CSRFToken", CSRFToken },
                    },
                Content = content
            };

            var responseMessage = await client.SendAsync(httpRequestMessage);
            System.Diagnostics.Debug.WriteLine($"Response of the req: {responseMessage}");
            return responseMessage;
        }

        private async Task<string> GetCSRFToken()
        {
            var response = await client.GetAsync(BaseUrl);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            // Looks for "csrf_token" followed by its value
            var pattern = @"(?<=""csrf_token"":"")\w+";
            Regex regex = new Regex(pattern);

            var match = regex.Match(content);
            Console.WriteLine($"IS MATCHED: {match.Success} \n pattern: {pattern}");
            if (match.Success)
            {
                // csrf_token has been found
                return match.Value;
            }
            throw new Exception("Could not retrieve CSRF token");
        }

    }
}
