using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace WouldYouRather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dictionary<string, string> questionsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"questions.json"));

        private static List<string> askedQuestions = new List<string>();

        public static bool soundEnabled = true;

        public static SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        static MainWindow mainWindow;

        private static string[] questionClasses =
        {
            "silly",
            "gross",
            "money",
            "relationship",
            "positiveHard",
            "negativeAbility",
            "positiveAbility",
            "negativePhysical",
            "positivePhysical"
        };

        public static void Initialize()
        {
            mainWindow.Title = "WYR Game";
            ReadGameInfo();
        }

        public static void ReadGameInfo()
        {
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.Volume = 80;
            synthesizer.SpeakAsync("Hello, player! \"Would you rather\" is a conversation or party game that poses a dilemma in the form of a question beginning with \"would you rather\". The dilemma can be between two supposedly good options, such as, \"Would you rather have the power of flight or the power of invisibility ? \", or it can be between two supposedly bad options, as in, \"Would you rather sleep with your best friend\'s lover or your lover\'s best friend ? \" The players, sometimes including the questioner, then must choose their answers. Answering \"neither\" or \"both\" is against the rules. This leads the players to debate their rationales. I wish you luck!");
        }

        public static void GenerateNextQuestion()
        {
            string randomClass = questionClasses[NextInt(0, questionClasses.Length)];
            int randomIntOne = NextInt(0, questionsDict.Where(x => x.Value == randomClass).ToDictionary(x => x.Key, x => x.Value).Count);
            int randomIntTwo = NextInt(0, questionsDict.Where(x => x.Value == randomClass).ToDictionary(x => x.Key, x => x.Value).Count);
            string questionSectionOne = questionsDict.Where(x => x.Value == randomClass).ToDictionary(x => x.Key, x => x.Value).Keys.ElementAt(randomIntOne);
            string questionSectionTwo = questionsDict.Where(x => x.Value == randomClass).ToDictionary(x => x.Key, x => x.Value).Keys.ElementAt(randomIntTwo);
            if (!askedQuestions.Contains($"{questionSectionOne}   OR   {questionSectionTwo}") && !askedQuestions.Contains($"{questionSectionTwo}   OR   {questionSectionOne}") && questionSectionOne != questionSectionTwo)
            {
                WriteCurrentClass(randomClass);
                AskQuestion(questionSectionOne, questionSectionTwo);
            }
            else GenerateNextQuestion();
        }

        private static int NextInt(int min, int max)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];

            rng.GetBytes(buffer);
            int result = BitConverter.ToInt32(buffer, 0);

            return new Random(result).Next(min, max);
        }

        private static void AddToBlackList(string givenQuestion)
        {
            askedQuestions.Add(givenQuestion);
        }

        public static void AskQuestion(string questionSectionOne, string questionSectionTwo)
        {
            string question = $"{questionSectionOne}   OR   {questionSectionTwo}";
            string questionRevert = $"{questionSectionTwo}   OR   {questionSectionOne}";
            AddToBlackList(question);
            AddToBlackList(questionRevert);
            mainWindow.LeftSectionText.Text = " " + questionSectionOne + " ";
            mainWindow.RightSectionText.Text = " " + questionSectionTwo + " ";
        }

        public static void WriteCurrentClass(string currentClass)
        {
            switch (currentClass)
            {
                case "silly":
                    mainWindow.CurrentClassText.Text = "Silly";
                    break;
                case "gross":
                    mainWindow.CurrentClassText.Text = "Gross";
                    break;
                case "money":
                    mainWindow.CurrentClassText.Text = "Money";
                    break;
                case "relationship":
                    mainWindow.CurrentClassText.Text = "Relationships";
                    break;
                case "positiveHard":
                    mainWindow.CurrentClassText.Text = "Hard";
                    break;
                case "negativeAbility":
                    mainWindow.CurrentClassText.Text = "Negative Ability";
                    break;
                case "positiveAbility":
                    mainWindow.CurrentClassText.Text = "Positive Ability";
                    break;
                case "negativePhysical":
                    mainWindow.CurrentClassText.Text = "Negative Physical";
                    break;
                case "positivePhysical":
                    mainWindow.CurrentClassText.Text = "Positive Physical";
                    break;
                default:
                    mainWindow.CurrentClassText.Text = "NULL";
                    break;
            }
            mainWindow.CurrentClassText.Text += " question";
        }

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            Initialize();
            GenerateNextQuestion();
        }

        private void LeftSection_Click(object sender, RoutedEventArgs e)
        {
            if(soundEnabled) Console.Beep();
            GenerateNextQuestion();
        }

        private void RightSection_Click(object sender, RoutedEventArgs e)
        {
            if(soundEnabled) Console.Beep();
            GenerateNextQuestion();
        }

        private void LeftSectionText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (soundEnabled) Console.Beep();
            GenerateNextQuestion();
        }

        private void RightSectionText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (soundEnabled) Console.Beep();
            GenerateNextQuestion();
        }

        private void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            soundEnabled = !soundEnabled;
            if (mainWindow.SoundText.Text == "Sound")
            {
                mainWindow.SoundText.Text = "No Sound";
                synthesizer.SpeakAsyncCancelAll();
                synthesizer.Volume = 0;
            }
            else
            {
                mainWindow.SoundText.Text = "Sound";
                synthesizer.Volume = 80;
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            if(soundEnabled) ReadGameInfo();
        }
    }
}
