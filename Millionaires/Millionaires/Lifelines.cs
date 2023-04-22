using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Millionaires
{
    interface ILifeline
    {
        void UseLifeline(List<Button> lActiveButtons, string correctAnswer);
    }

    class PhoneToFriend : ILifeline
    {
        private Page parentPage;
        private string questionText;

        public PhoneToFriend(Page parentPage, string questionText)
        {
            this.parentPage = parentPage;
            this.questionText = questionText;
        }

        public void UseLifeline(List<Button> lActiveButtons, string correctAnswer)
        {
            Random random = new Random();
            int percentage = random.Next(1, 101); // Losowanie procentu

            // Zapytanie użytkownika, czy chce zadzwonić do przyjaciela
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool callFriend = await parentPage.DisplayAlert("Telefon do przyjaciela", "Czy chciałbyś zadzwonić do prawdziwego przyjaciela?", "Prawdziwy (dzwonię)", "Komputer");

                if (callFriend)
                    CallFriend(lActiveButtons.Select(b=>b.Text).ToList());
                else
                {
                    if (percentage <= 35) // W 70% przypadków pokaż poprawną odpowiedź
                    {
                        await parentPage.DisplayAlert("Telefon do przyjaciela", "Jestem pewna że to: \"" + correctAnswer + "\"", "OK");
                    }
                    if (percentage <= 70) // W 70% przypadków pokaż poprawną odpowiedź
                    {
                        await parentPage.DisplayAlert("Telefon do przyjaciela", "Myślę że to: \"" + correctAnswer + "\"", "OK");
                    }
                    else if (percentage <= 80) // W 10% przypadków powiedz, że nie wiesz
                    {
                        await parentPage.DisplayAlert("Telefon do przyjaciela", "Nie mam pojęcia", "OK");
                    }
                    else // W 20% przypadków wybierz losową odpowiedź z listy lActiveButtons
                    {
                        string[] options = new string[4]; // Tablica opcji odpowiedzi
                        options[0] = correctAnswer; // Pierwsza opcja to zawsze poprawna odpowiedź

                        foreach (var button in lActiveButtons)
                        {
                            int index = lActiveButtons.IndexOf(button);
                            if (index > 0)
                            {
                                options[index] = button.Text;
                            }
                        }

                        int randomIndex = random.Next(0, 4); // Losowanie indeksu z zakresu od 0 do 3
                        string randomOption = options[randomIndex]; // Wybranie losowej opcji

                        await parentPage.DisplayAlert("Telefon do przyjaciela", "Myślę że to: \"" + randomOption + "\"", "OK");
                    }
                }
            });
        }

        public async void CallFriend(List<string> options)
        {
            // Tworzenie nowej formatki z odświeżającym się czasem
            ContentPage timePage = new ContentPage();
            StackLayout layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Label titleLabel = new Label
            {
                Text = "Telefon do przyjaciela",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };
            Label timeLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            Button confirmButton = new Button
            {
                Text = "Zakończ",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
                HorizontalOptions = LayoutOptions.Center
            };
            Label questionLabel = new Label
            {
                Text = questionText, // Wyświetlenie tekstu pytania
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            confirmButton.Clicked += async (sender, e) =>
            {
                await parentPage.Navigation.PopModalAsync(); // Zamknięcie formatki z czasem
                layout.Children.Clear();
            };
            layout.Children.Add(titleLabel);
            layout.Children.Add(timeLabel);
            layout.Children.Add(confirmButton);
            layout.Children.Add(questionLabel);
            foreach (var option in options)
            {
                Button optionButton = new Button
                {
                    Text = option,
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
                    IsEnabled = false,
                    HorizontalOptions = LayoutOptions.Center
                };
                layout.Children.Add(optionButton);
            }

            timePage.Content = layout;
            await parentPage.Navigation.PushModalAsync(timePage);

            DateTime startTime = DateTime.Now; // Deklaracja zmiennej startTime na początku metody

            while ((DateTime.Now - startTime).TotalSeconds < 35)
            {
                int secondsRemaining = 35 - (int)(DateTime.Now - startTime).TotalSeconds;
                timeLabel.Text = "Pozostało " + secondsRemaining + " sekund.";

                // Wstrzymanie działania metody na 1 sekundę
                await Task.Delay(1000);
            }

            if (timePage != null && timePage.Parent != null)
            {

                // Po upływie 35 sekund, zamknięcie formatki z czasem
                await parentPage.Navigation.PopModalAsync();

                // Wyświetlenie komunikatu z wynikiem na naszej formatce
                await parentPage.DisplayAlert("Telefon do przyjaciela", "Upłynęło 35 sekund!", "OK");
            }
        }
    }

    class AskTheAudience : ILifeline
    {
        public void UseLifeline(List<Button> lActiveButtons, string correctAnswer)
        {
            Random random = new Random();

            int correctAnswerPercentage;

            if (lActiveButtons.Count == 2)
            {
                correctAnswerPercentage = random.Next(35, 81);
            }
            else if (lActiveButtons.Count == 4)
            {
                correctAnswerPercentage = random.Next(30, 61);
            }
            else
                throw new Exception("AskTheAudience on " + lActiveButtons.Count() + " buttons");


            var correctbutton = lActiveButtons.First(b => b.Text.StartsWith(correctAnswer));
            correctbutton.Text = $"{correctbutton.Text} ({correctAnswerPercentage}%)";
            lActiveButtons.Remove(correctbutton);

            int remainingPercentage = 100 - correctAnswerPercentage;


            foreach (var button in lActiveButtons)
            {
                if (button.Text != correctAnswer)
                {
                    int percentage = random.Next(1, remainingPercentage);
                    button.Text = $"{button.Text} ({percentage}%)";
                    remainingPercentage -= percentage;
                }
                else
                {
                    button.Text = $"{button.Text} ({remainingPercentage}%)";
                }
            }
        }
    }

    class FiftyFifty : ILifeline
    {
        public void UseLifeline(List<Button> lActiveButtons, string correctAnswer)
        {
            lActiveButtons.RemoveAll(b => b.Text.StartsWith(correctAnswer));
            var posibleToExcept = lActiveButtons.Where(b => !b.Text.StartsWith(correctAnswer)).ToList();

            Random random = new Random();
            for (int i = 0; i < 2; i++)
            {
                int indexToRemove = random.Next(posibleToExcept.Count - i);

                if (!posibleToExcept[indexToRemove].IsEnabled)
                    indexToRemove++;

                posibleToExcept[indexToRemove].IsEnabled = false;
            }
        }
    }
}
