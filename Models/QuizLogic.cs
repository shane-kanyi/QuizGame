using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizGame
{
    // Abstraction: Base class for all question types
    public abstract class Question
    {
        public string Text { get; set; }

        protected Question(string text)
        {
            Text = text;
        }

        public abstract bool IsCorrect(string answer);
        public abstract string GetCorrectAnswer();
    }

    // Inheritance: Derived class for multiple-choice questions
    public class MultipleChoiceQuestion : Question
    {
        public List<string> Options { get; private set; }
        private int _correctOptionIndex;

        public MultipleChoiceQuestion(string text, List<string> options, int correctOptionIndex) : base(text)
        {
            Options = options;
            _correctOptionIndex = correctOptionIndex;
        }

        public override bool IsCorrect(string answer)
        {
            if (int.TryParse(answer, out int choice))
            {
                return choice - 1 == _correctOptionIndex;
            }
            return false;
        }

        public override string GetCorrectAnswer()
        {
            return Options[_correctOptionIndex];
        }
    }

    // Inheritance: Derived class for open-ended questions
    public class OpenEndedQuestion : Question
    {
        public List<string> AcceptableAnswers { get; private set; }

        public OpenEndedQuestion(string text, List<string> acceptableAnswers) : base(text)
        {
            AcceptableAnswers = acceptableAnswers.Select(a => a.ToLower()).ToList();
        }



        public override bool IsCorrect(string answer)
        {
            return AcceptableAnswers.Contains(answer.Trim().ToLower());
        }

        public override string GetCorrectAnswer()
        {
            return AcceptableAnswers.First();
        }
    }

    // Inheritance: Derived class for true/false questions
    public class TrueFalseQuestion : Question
    {
        private bool _correctAnswer;

        public TrueFalseQuestion(string text, bool correctAnswer) : base(text)
        {
            _correctAnswer = correctAnswer;
        }

        public override bool IsCorrect(string answer)
        {
            string formattedAnswer = answer.Trim().ToLower();
            if (formattedAnswer == "true" || formattedAnswer == "t")
            {
                return _correctAnswer == true;
            }
            if (formattedAnswer == "false" || formattedAnswer == "f")
            {
                return _correctAnswer == false;
            }
            return false;
        }

        public override string GetCorrectAnswer()
        {
            return _correctAnswer.ToString();
        }
    }

    // Main Game Logic Class - Now Adapted for a Web API
    public class Quiz
    {
        private List<Question> _questions = new List<Question>();
        private int _currentQuestionIndex = -1;
        public int Score { get; private set; }
        public int TotalQuestions => _questions.Count;

        // The constructor now loads the questions directly
        public Quiz()
        {
            _questions.Add(new MultipleChoiceQuestion("What is the capital of France?", new List<string> { "London", "Berlin", "Paris", "Madrid" }, 2));
            _questions.Add(new OpenEndedQuestion("Which country is known as the Land of the Rising Sun?", new List<string> { "Japan", "Nippon" }));
            _questions.Add(new TrueFalseQuestion("The Great Wall of China is visible from space with the naked eye.", false));
            _questions.Add(new MultipleChoiceQuestion("Which is the largest planet in our solar system?", new List<string> { "Earth", "Jupiter", "Mars", "Saturn" }, 1));
        }

        public void StartNewGame()
        {
            Score = 0;
            _currentQuestionIndex = -1;
            // Optional: Shuffle questions for a new experience each time
            // var random = new Random();
            // _questions = _questions.OrderBy(q => random.Next()).ToList();
        }

        public (Question? question, int index) GetNextQuestion()
        {
            _currentQuestionIndex++;
            if (_currentQuestionIndex < _questions.Count)
            {
                return (_questions[_currentQuestionIndex], _currentQuestionIndex);
            }
            return (null, -1); // No more questions
        }

        public Question? GetQuestionByIndex(int index)
        {
            if (index >= 0 && index < _questions.Count)
            {
                return _questions[index];
            }
            return null;
        }

        public void IncrementScore()
        {
            Score++;
        }
    }
}