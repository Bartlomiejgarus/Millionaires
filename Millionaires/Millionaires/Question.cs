using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Millionaires
{
    public class Question
    {
        public string QuestionText { get; private set; }
        public string OptionA { get; private set; }
        public string OptionB { get; private set; }
        public string OptionC { get; private set; }
        public string OptionD { get; private set; }
        public string CorrectAnswer { get; private set; }

        public Question(string questionText, string optionA, string optionB, string optionC, string optionD, string correctAnswer)
        {
            QuestionText = questionText;
            OptionA = optionA;
            OptionB = optionB;
            OptionC = optionC;
            OptionD = optionD;
            CorrectAnswer = correctAnswer;
        }

        public static List<Question> LoadQuestions()
        {
            var questions = new List<Question>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(GamePage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Millionaires.data.txt");

            using (var reader = new StreamReader(stream))
            {
                while (questions.Count < 12)
                {
                    if (reader.EndOfStream)
                        throw new Exception("No data in data.txt");

                    Question question = new Question
                    (
                        questionText: reader.ReadLine(),
                        optionA: reader.ReadLine(),
                        optionB: reader.ReadLine(),
                        optionC: reader.ReadLine(),
                        optionD: reader.ReadLine(),
                        correctAnswer: reader.ReadLine()
                    );
                    questions.Add(question);
                }
            }
            return questions.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}