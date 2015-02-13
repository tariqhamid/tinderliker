using System;
using System.Threading;
using TinderLiker.Helpers;

namespace TinderLiker
{
    class Program
    {
        private const int SecondsBetweenEachSwipe = 3;
        private const int SecondsToWaitWhenNoMoreRecomendations = 60;

        private static int _attempts = 0;
        private static bool _authenticated = false;
        public static User User = new User();

        private static string _email;
        private static string _password;
        private const string HeaderMessage = " ==== Welcome to Tinder auto Liker ====";
        private const string FacebookStatus = " Facebook status: ";
        private const string TinderStatus = " Tinder status: ";
        private const string Spacer = "\n-------------------------------------------\n";

        static void Main(string[] args)
        {
            while (!_authenticated)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(HeaderMessage);
                Console.WriteLine(Spacer);
                Console.WriteLine(FacebookStatus + ((_attempts > 0) ? "Wrong email or password (attempt: " + _attempts + ")" : "Waiting for credentials"));
                Console.WriteLine(Spacer);
                Console.WriteLine("");
                Console.Write(" Email: ");
                _email = Console.ReadLine();
                Console.Write(" Password: ");
                _password = ConsoleHelper.ReadPassword();


                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(HeaderMessage);
                Console.WriteLine(Spacer);
                Console.WriteLine(FacebookStatus + "Authenticating");
                Console.WriteLine(Spacer);

                _authenticated = FacebookHelper.Authenticate(_email, _password);

                _attempts++;
            }

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(HeaderMessage);
            Console.WriteLine(Spacer);
            Console.WriteLine(FacebookStatus + "Authenticated\n");
            Console.WriteLine(TinderStatus + "Connecting to Tinder API");
            Console.WriteLine(Spacer);

            TinderHelper.Authenticate(User.FacebookId, User.FacebookAccessToken);

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(HeaderMessage);
            Console.WriteLine(Spacer);
            Console.WriteLine(FacebookStatus + "Authenticated\n");
            Console.WriteLine(TinderStatus + "Swiping riiight a lot!");
            Console.WriteLine(Spacer);

            Console.WriteLine("Press ctrl+c to abort.");

            while (true)
            {
                var response = TinderHelper.GetResults();
                while (response != null)
                {
                    foreach (var tinderPeep in response)
                    {
                        var id = tinderPeep["_id"].ToString();

                        var name = tinderPeep["name"].ToString() + " (" + id + ")";
                        Console.Write(name);
                        Thread.Sleep(SecondsBetweenEachSwipe * 1000); // Seconds between each swipe
                        bool didMatchAlready = TinderHelper.LikeResult(id);
                        Console.Write((didMatchAlready ? " - A Match!" : ""));
                        Console.WriteLine("");
                    }

                    response = TinderHelper.GetResults();
                }

                Console.WriteLine("No recommendations");
                Thread.Sleep(SecondsToWaitWhenNoMoreRecomendations * 1000); // Let's give it a while before we check if there's new results.
            }
        }
    }

    public class User
    {
        public string FacebookId { get; set; }
        public string FacebookAccessToken { get; set; }
        public string TinderAccessToken { get; set; }
    }
}
