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
        public Stage stage;

        private PhoneToFriend phoneToFriend;
        private AskTheAudience askTheAudience;
        private FiftyFifty fiftyFifty;

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

        /// <summary></summary>
        /// <remarks></remarks>
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
                if ((int)stage == 12)
                {
                    await DisplayAlert("Poprawna odpowiedź", "Gratulacje! Wybrałeś " + stage.Prize(), "Zagraj jeszcze raz");
                    InitGame();
                }
                else
                {
                    await DisplayAlert("Poprawna odpowiedź", "Gratulacje! Wybrałeś " + stage.Prize(), "Następne pytanie");

                    questionIndex++;
                    DisplayQuestion();
                }
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
                await DisplayAlert("Gra zakończona", "Twoja wygrana " + stage.LastGuaranteed().Prize(), "Spróbuj ponownie");
                InitGame();
            }
        }

        private void PhoneToFriendButton_Clicked(object sender, EventArgs e) => UseLifeline(phoneToFriend = new PhoneToFriend(this, currentQuestion.QuestionText), (Button)sender);
        private void AskTheAudienceButton_Clicked(object sender, EventArgs e) => UseLifeline(askTheAudience = new AskTheAudience(), (Button)sender);
        private void FiftyFiftyButton_Clicked(object sender, EventArgs e) => UseLifeline(fiftyFifty = new FiftyFifty(), (Button)sender);

        private void UseLifeline(ILifeline lifeline, Button lifelineButton)
        {
            var buttons = new List<Button> { ButtonA, ButtonB, ButtonC, ButtonD }.Where(button => button.IsEnabled).ToList();

            lifeline.UseLifeline(buttons, currentQuestion.CorrectAnswer);
            lifelineButton.IsEnabled = false; //Disable lifeline
        }


    }
}