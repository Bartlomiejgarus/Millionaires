using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Millionaires
{
    public partial class GamePage : ContentPage
    {
        private List<Question> questions;
        private Question currentQuestion;
        private int questionIndex;
        private bool askTheAudienceUsed;
        public Stage stage;

        public GamePage()
        {
            InitializeComponent();
            InitGame();
        }

        private void InitGame()
        {
            InitButtons();
            stage = Stage.Zero;
            questionIndex = 0;
            questions = Question.LoadQuestions();
            DisplayQuestion();
        }

        private void InitButtons() //When the game begins, all lifelines are active
        {
            AskTheAudienceButton.IsEnabled = FiftyFiftyButton.IsEnabled = PhoneToFriendButton.IsEnabled = true;
        }

        private void DisplayQuestion()
        {
            currentQuestion = questions[questionIndex];
            QuestionLabel.Text = $"{currentQuestion.QuestionText}";
            ButtonA.Text = $"{currentQuestion.OptionA}";
            ButtonB.Text = $"{currentQuestion.OptionB}";
            ButtonC.Text = $"{currentQuestion.OptionC}";
            ButtonD.Text = $"{currentQuestion.OptionD}";

            // Enable all the answer buttons
            ButtonA.IsEnabled = ButtonB.IsEnabled = ButtonC.IsEnabled = ButtonD.IsEnabled = true;
        }

        private async void AnswerButton_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text.StartsWith(currentQuestion.CorrectAnswer))
            {
                stage++;
                await DisplayAlert("Poprawna odpowiedź", "Gratulacje! Wybrałeś " + stage.Prize(), "Następne pytanie");

                questionIndex++;
                DisplayQuestion();
            }
            else
            {
                await DisplayAlert("Błędna odpowiedź", "Twoja wygrana "+ stage.LastGuaranteed().Prize(), "Spróbuj ponownie");
                InitGame();
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
            var remainingOptions = new List<Button> { ButtonA, ButtonB, ButtonC, ButtonD };
            remainingOptions.RemoveAll(b => b.Text.StartsWith(currentQuestion.CorrectAnswer));

            Random random = new Random();
            for (int i = 0; i < 2; i++)
            {
                int indexToRemove = random.Next(remainingOptions.Count);
                remainingOptions[indexToRemove].IsEnabled = false;
                remainingOptions.RemoveAt(indexToRemove);
            }

            FiftyFiftyButton.IsEnabled = false; // Disable the 50:50 button after it's used
        }


        private void AskTheAudienceButton_Clicked(object sender, EventArgs e)
        {
            if (!askTheAudienceUsed)
            {
                ShowPercentages();
                askTheAudienceUsed = true;
                AskTheAudienceButton.IsEnabled = false;
            }
        }


        private void ShowPercentages()
        {
            var answerButtons = new List<Button> { ButtonA, ButtonB, ButtonC, ButtonD };
            Random random = new Random();

            foreach (Button button in answerButtons)
            {
                int percentage = random.Next(1, 100);
                button.Text = $"{button.Text} ({percentage}%)";
            }
        }

    }
}