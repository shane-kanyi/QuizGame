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

    // --- The MultipleChoiceQuestion, OpenEndedQuestion, and TrueFalseQuestion classes remain unchanged ---
    // (They are included here for completeness of the file)
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
    public class OpenEndedQuestion : Question
    {
        public List<string> AcceptableAnswers { get; private set; }
        public OpenEndedQuestion(string text, List<string> acceptableAnswers) : base(text)
        {
            AcceptableAnswers = acceptableAnswers.Select(a => a.ToLower()).ToList();
        }
        public override bool IsCorrect(string answer) => AcceptableAnswers.Contains(answer.Trim().ToLower());
        public override string GetCorrectAnswer() => AcceptableAnswers.First();
    }
    public class TrueFalseQuestion : Question
    {
        private bool _correctAnswer;
        public TrueFalseQuestion(string text, bool correctAnswer) : base(text) => _correctAnswer = correctAnswer;
        public override bool IsCorrect(string answer)
        {
            string formattedAnswer = answer.Trim().ToLower();
            if (formattedAnswer == "true" || formattedAnswer == "t") return _correctAnswer == true;
            if (formattedAnswer == "false" || formattedAnswer == "f") return _correctAnswer == false;
            return false;
        }
        public override string GetCorrectAnswer() => _correctAnswer.ToString();
    }


    // Main Game Logic Class - Heavily Refactored
    public class Quiz
    {
        public string Name { get; private set; }
        public List<Question> Questions { get; private set; } = new List<Question>();
        
        // --- Properties for managing a single game session ---
        private int _currentQuestionIndex = -1;
        public int Score { get; private set; }
        public int TotalQuestions => Questions.Count;

        // Constructor for a new, empty quiz
        public Quiz(string name)
        {
            Name = name;
        }
        
        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }

        // --- Methods for managing a game session ---
        public void StartNewGame()
        {
            Score = 0;
            _currentQuestionIndex = -1;
        }

        public (Question? question, int index) GetNextQuestion()
        {
            _currentQuestionIndex++;
            if (_currentQuestionIndex < Questions.Count)
            {
                return (Questions[_currentQuestionIndex], _currentQuestionIndex);
            }
            return (null, -1);
        }

        public Question? GetQuestionByIndex(int index)
        {
            if (index >= 0 && index < Questions.Count) return Questions[index];
            return null;
        }

        public void IncrementScore()
        {
            Score++;
        }
    }
}