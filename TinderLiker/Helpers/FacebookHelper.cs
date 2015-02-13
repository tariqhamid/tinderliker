using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestSharp;
using SimpleBrowser;

namespace TinderLiker.Helpers
{
    public static class FacebookHelper
    {
        private const string FacebookUrl = "https://www.facebook.com/dialog/oauth?client_id=464891386855067&redirect_uri=https://www.facebook.com/connect/login_success.html&scope=basic_info,email,public_profile,user_about_me,user_activities,user_birthday,user_education_history,user_friends,user_interests,user_likes,user_location,user_photos,user_relationship_details&response_type=token";
        private const string GraphUrl = "https://graph.facebook.com";

        public static bool Authenticate(string email, string password)
        {
            try
            {
                var browser = new Browser
                {
                    UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10"
                };

                browser.Navigate(FacebookUrl);

                browser.Find("email").Value = email;
                browser.Find("pass").Value = password;

                browser.Find("Input", FindBy.Name, "login").Click();

                var url = browser.Url.ToString();
                if (url.Contains("access_token"))
                {
                    var match = Regex.Match(url, @"#access_token=(.*)&expires_in", RegexOptions.IgnoreCase);
                    Program.User.FacebookAccessToken = match.Groups[1].Value;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            if (string.IsNullOrEmpty(Program.User.FacebookAccessToken)) return false;
            var client = new RestClient(GraphUrl);

            var request = new RestRequest("me", Method.GET);
            request.AddQueryParameter("access_token", Program.User.FacebookAccessToken);

            var response = client.Execute<dynamic>(request);
            var json = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Program.User.FacebookId = json["id"];


            return !string.IsNullOrEmpty(Program.User.FacebookId);
        }
    }
}
