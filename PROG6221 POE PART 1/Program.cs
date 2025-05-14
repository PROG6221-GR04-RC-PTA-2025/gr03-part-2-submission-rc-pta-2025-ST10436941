using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using System.Threading;

namespace PROG6221_POE_PART_1
{
    class Chatbot
    {
        static Dictionary<string, string> keywordResponses = new Dictionary<string, string>
        {
            { "password", "Make sure to use strong, unique passwords for each account. Avoid using personal details." },
            { "scam", "Be cautious of deals that seem too good to be true, and never share personal information." },
            { "privacy", "Regularly review your privacy settings and avoid oversharing online." }
        };

        static Dictionary<string, List<string>> randomResponses = new Dictionary<string, List<string>>
        {
            { "phishing", new List<string> {
                "Don't click on suspicious links in emails.",
                "Verify sender addresses before responding.",
                "Look for spelling errors—it's a red flag!"
            }}
        };

        static Dictionary<string, string> userData = new Dictionary<string, string>();
        static string currentTopic = "";
        static Random rand = new Random();

        static void Main()
        {
            Console.Title = "Starting Cybersecurity Awareness Bot...";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Clear();

            PlayVoiceGreeting();
            DisplayAsciiImage();

            TypeEffect("\nBot: Hello! What's your name? ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string userName = Console.ReadLine();
            userData["name"] = userName;
            Console.ForegroundColor = ConsoleColor.Cyan;

            TypeEffect($"\nBot: Nice to meet you, {userName}! How can I assist you today?\n");
            PrintDivider();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("You: ");
                string userInput = Console.ReadLine().ToLowerInvariant();
                Console.ForegroundColor = ConsoleColor.Cyan;
                bool handled = false;

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    TypeEffect("Bot: Please type something so I can help you.");
                    continue;
                }

                if (userInput.Contains("exit") || userInput.Contains("bye"))
                {
                    TypeEffect($"Bot: Goodbye, {userData["name"]}! Stay safe online!");
                    break;
                }

                handled = HandleSentiment(userInput);
                if (!handled) handled = HandleKeywordResponses(userInput);
                if (!handled) handled = HandleRandomResponses(userInput);
                if (!handled && userInput.Contains("more") && currentTopic == "phishing")
                {
                    ProvideRandomResponse("phishing");
                    handled = true;
                }
                if (!handled && userInput.Contains("interested in"))
                {
                    string interest = userInput.Substring(userInput.IndexOf("interested in") + 13).Trim();
                    userData["interest"] = interest;
                    TypeEffect($"Bot: Great! I'll remember that you're interested in {interest}. It's a crucial part of staying safe online.");
                    handled = true;
                }
                if (!handled && userData.ContainsKey("interest") && userInput.Contains("remind me"))
                {
                    TypeEffect($"Bot: As someone interested in {userData["interest"]}, you might want to review the security settings on your accounts.");
                    handled = true;
                }
                if (!handled)
                {
                    TypeEffect("Bot: I'm not sure I understand. Can you try rephrasing?");
                }

                PrintDivider();
            }
        }

        static bool HandleKeywordResponses(string input)
        {
            foreach (var keyword in keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    TypeEffect($"Bot: {keywordResponses[keyword]}");
                    currentTopic = keyword;
                    return true;
                }
            }
            return false;
        }

        static bool HandleRandomResponses(string input)
        {
            foreach (var topic in randomResponses.Keys)
            {
                if (input.Contains(topic))
                {
                    ProvideRandomResponse(topic);
                    currentTopic = topic;
                    return true;
                }
            }
            return false;
        }

        static void ProvideRandomResponse(string topic)
        {
            var responses = randomResponses[topic];
            TypeEffect($"Bot: {responses[rand.Next(responses.Count)]}");
        }

        static bool HandleSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("anxious"))
            {
                TypeEffect("Bot: It's okay to feel that way. Cybersecurity can be tricky, but I'm here to help.");
                return true;
            }
            else if (input.Contains("curious"))
            {
                TypeEffect("Bot: That's great! Curiosity is the first step to being cyber smart.");
                return true;
            }
            else if (input.Contains("frustrated"))
            {
                TypeEffect("Bot: Don't worry, we'll get through this together. Just ask me anything.");
                return true;
            }
            return false;
        }

        static void PlayVoiceGreeting()
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "voice.wav");

            try
            {
                using (SoundPlayer player = new SoundPlayer(filePath))
                {
                    player.PlaySync();
                }
                Console.WriteLine("Voice greeting played successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing the voice greeting: " + ex.Message);
            }
        }

        static void DisplayAsciiImage()
        {
            string[] botArt = {
                "        [------]        ",
                "        | .  . |        ",
                "        |  --  |        ",
                "        [------]        ",
                "       /  ----  \\",
                "      | |      | |      ",
                "      | |      | |      ",
                "      []        []      ",
                "     /_\\        /_\\    "
            };

            foreach (string line in botArt)
            {
                Console.WriteLine(line);
            }
        }

        static void TypeEffect(string message, int delay = 30)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        static void PrintDivider()
        {
            Console.WriteLine("--------------------------------------------------");
        }
    }
}