using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Millionaires
{
    public partial class MainPage : ContentPage
    {
        private List<TriviaQuestion> questions;
        private TriviaQuestion currentQuestion;
        private int questionIndex;
        private bool fiftyFiftyUsed;

        public MainPage()
        {
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion();
        }

        private void LoadQuestions()
        {
            questions = new List<TriviaQuestion>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Millionaires.trivia.txt");

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    TriviaQuestion question = new TriviaQuestion
                    {
                        Question = reader.ReadLine(),
                        OptionA = reader.ReadLine(),
                        OptionB = reader.ReadLine(),
                        OptionC = reader.ReadLine(),
                        OptionD = reader.ReadLine(),
                        CorrectAnswer = reader.ReadLine()
                    };
                    questions.Add(question);
                }
            }
            questions = questions.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private void DisplayQuestion()
        {
            currentQuestion = questions[questionIndex];
            QuestionLabel.Text = $"{currentQuestion.Question}";
            ButtonA.Text = $"{currentQuestion.OptionA}";
            ButtonB.Text = $"{currentQuestion.OptionB}";
            ButtonC.Text = $"{currentQuestion.OptionC}";
            ButtonD.Text = $"{currentQuestion.OptionD}";

            // Enable all the answer buttons
            ButtonA.IsEnabled = true;
            ButtonB.IsEnabled = true;
            ButtonC.IsEnabled = true;
            ButtonD.IsEnabled = true;

            fiftyFiftyUsed = false;
        }



        private async void AnswerButton_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text.StartsWith(currentQuestion.CorrectAnswer))
            {
                await DisplayAlert("Poprawna odpowiedź", "Gratulacje! Wybrałeś właściwą odpowiedź.", "Następne pytanie");
                questionIndex++;
                DisplayQuestion();
            }
            else
            {
                await DisplayAlert("Błędna odpowiedź", "Niestety, to nie jest prawidłowa odpowiedź.", "Spróbuj ponownie");
            }
        }

        private async void RezygnacjaButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Rezygnacja", "Czy na pewno chcesz zakończyć grę?", "Tak", "Nie");
            if (answer)
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        private void FiftyFiftyButton_Clicked(object sender, EventArgs e)
        {
            if (!fiftyFiftyUsed)
            {
                var remainingOptions = new List<Button> { ButtonA, ButtonB, ButtonC, ButtonD };
                remainingOptions.RemoveAll(b => b.Text.StartsWith(currentQuestion.CorrectAnswer));

                Random random = new Random();
                for (int i = 0; i < 2; i++)
                {
                    int indexToRemove = random.Next(remainingOptions.Count);
                    remainingOptions[indexToRemove].IsEnabled = false;
                    remainingOptions.RemoveAt(indexToRemove);
                }

                fiftyFiftyUsed = true;
            }
        }
    }

    public class TriviaQuestion
    {
        public string Question { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
