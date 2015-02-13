using Newtonsoft.Json;
using RestSharp;

namespace TinderLiker.Helpers
{
    public static class TinderHelper
    {
        private const string ApiUrl = "https://api.gotinder.com";
        private static readonly RestClient Client = new RestClient(ApiUrl);

        static TinderHelper()
        {
            Client.UserAgent = "Tinder/4.0.9 (iPhone; iOS 8.1.1; Scale/2.00)";
        }

        internal static void SetTinderToken()
        {
            Client.AddDefaultHeader("X-Auth-Token", Program.User.TinderAccessToken);
        }

        public static bool Authenticate(string fbId, string fbToken)
        {
            var request = new RestRequest("auth", Method.POST);
            request.AddParameter("facebook_id", fbId);
            request.AddParameter("facebook_token", fbToken);

            var response = Client.Execute<dynamic>(request);
            var json = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Program.User.TinderAccessToken = json["token"];
            
            SetTinderToken();

            return !string.IsNullOrEmpty(Program.User.TinderAccessToken);
        }

        public static dynamic GetResults()
        {
            var request = new RestRequest("user/recs", Method.POST);
            var response = Client.Execute<dynamic>(request);
            var json = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return json != null ? json["results"] : null;
        }

        public static bool LikeResult(string id)
        {
            var request = new RestRequest("like/" + id, Method.GET);
            var response = Client.Execute<dynamic>(request);
            var json = JsonConvert.DeserializeObject<dynamic>(response.Content);

            if (json == null) return false;

            var isMatch = json["match"];

            return isMatch.ToString() != "False";
        }
    }
}
