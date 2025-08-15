using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QuizGame
{
    // Abstraction: Base class for all question types
    public abstract class Question
    {
        public string Text { get; set; }

        // Encapsulation: Using properties
        protected Question(string text)
        {
            Text = text;
        }

        public abstract bool IsCorrect(string answer);
        public abstract string GetCorrectAnswer();
        public abstract void Display();
    }

    // Inheritance: Derived class for multiple-choice questions
    public class MultipleChoiceQuestion : Question
    {
        public List<string> Options { get; private set; }
        private int _correctOptionIndex;

        public MultipleChoiceQuestion(string text, List<string> options, int correctOptionIndex) : base(text)
        {
            if (options == null || options.Count != 4)
                throw new ArgumentException("Multiple choice questions must have exactly 4 options.");
            if (correctOptionIndex < 0 || correctOptionIndex >= 4)
                throw new ArgumentException("Correct option index is out of bounds.");

            Options = options;
            _correctOptionIndex = correctOptionIndex;
        }

        public override void Display()
        {
            Console.WriteLine(Text);
            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Options[i]}");
            }
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
            if (acceptableAnswers == null || acceptableAnswers.Count == 0)
                throw new ArgumentException("Open-ended questions must have at least one acceptable answer.");

            AcceptableAnswers = acceptableAnswers.Select(a => a.ToLower()).ToList();
        }

        public override void Display()
        {
            Console.WriteLine(Text);
        }

        public override bool IsCorrect(string answer)
        {
            return AcceptableAnswers.Contains(answer.Trim().ToLower());
        }

        public override string GetCorrectAnswer()
        {
            return AcceptableAnswers.First(); // Return the primary correct answer
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

        public override void Display()
        {
            Console.WriteLine($"{Text} (True/False)");
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

    // Main Game Logic Class
    public class Quiz
    {
        // Using a linear data structure that maintains a variable number of elements
        private List<Question> _questions = new List<Question>();

        public void AddQuestion()
        {
            Console.WriteLine("\nSelect question type to add:");
            Console.WriteLine("1. Multiple Choice");
            Console.WriteLine("2. Open-Ended");
            Console.WriteLine("3. True/False");
            Console.Write("Enter your choice: ");
            
            // Use '!' to assert that Console.ReadLine() is non-null
            string choice = Console.ReadLine()!; 

            Console.Write("Enter the question text: ");
            string text = Console.ReadLine()!;

            switch (choice)
            {
                case "1":
                    List<string> options = new List<string>();
                    for (int i = 0; i < 4; i++)
                    {
                        Console.Write($"Enter option {i + 1}: ");
                        options.Add(Console.ReadLine()!);
                    }
                    Console.Write("Enter the correct option number (1-4): ");
                    // Handle potential parse failure more robustly
                    if (int.TryParse(Console.ReadLine(), out int correctIndex))
                    {
                        _questions.Add(new MultipleChoiceQuestion(text, options, correctIndex - 1));
                    }
                    else
                    {
                        Console.WriteLine("Invalid index provided.");
                    }
                    break;
                case "2":
                    Console.Write("Enter all acceptable answers, separated by a comma (e.g., UK,United Kingdom): ");
                    string answersStr = Console.ReadLine()!;
                    List<string> acceptableAnswers = answersStr.Split(',').Select(a => a.Trim()).ToList();
                    _questions.Add(new OpenEndedQuestion(text, acceptableAnswers));
                    break;
                case "3":
                    Console.Write("Is the correct answer True or False? ");
                    bool correctAnswer = Console.ReadLine()!.Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
                    _questions.Add(new TrueFalseQuestion(text, correctAnswer));
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            Console.WriteLine("Question added successfully!");
        }
        
        private Question? SelectQuestion(string action)
        {
            if (_questions.Count == 0)
            {
                Console.WriteLine("No questions available to " + action + ".");
                return null;
            }
            
            Console.WriteLine($"\nSelect a question to {action}:");
            for (int i = 0; i < _questions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_questions[i].Text}");
            }
            Console.Write("Enter question number: ");
            
            // Handle null and non-integer input
            if(int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _questions.Count)
            {
                return _questions[index - 1];
            }
            
            Console.WriteLine("Invalid selection.");
            return null;
        }


        public void EditQuestion()
        {
            Question? questionToEdit = SelectQuestion("edit");
            if (questionToEdit != null)
            {
                int index = _questions.IndexOf(questionToEdit);
                _questions.RemoveAt(index);
                Console.WriteLine("The old question has been removed. Please add the updated version.");
                AddQuestion(); // Re-add as new for simplicity
                 Console.WriteLine("Question edited successfully!");
            }
        }

        public void DeleteQuestion()
        {
            Question? questionToDelete = SelectQuestion("delete");
             if (questionToDelete != null)
            {
                _questions.Remove(questionToDelete);
                Console.WriteLine("Question deleted successfully!");
            }
        }

        public void Play()
        {
            if (_questions.Count == 0)
            {
                Console.WriteLine("\nNo questions available to play. Please add some questions first.");
                return;
            }

            int score = 0;
            // Ensure Tuple handles potential null answers
            var userAnswers = new List<Tuple<Question, string?>>();
            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine("\n--- Starting Quiz ---");
            stopwatch.Start();

            foreach (var question in _questions)
            {
                question.Display();
                Console.Write("Your answer: ");
                // Ensure we read the answer (which might be null if the stream ends)
                string? answer = Console.ReadLine(); 
                userAnswers.Add(new Tuple<Question, string?>(question, answer));
                
                // Only evaluate if the answer is not null
                if (answer != null && question.IsCorrect(answer))
                {
                    score++;
                }
            }

            stopwatch.Stop();
            Console.WriteLine("\n--- Quiz Finished ---");
            Console.WriteLine($"Your final score: {score} out of {_questions.Count}");
            Console.WriteLine($"Time taken: {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds.");
            
            Console.Write("\nDo you want to see the correct answers? (yes/no): ");
            // Use '!' on Console.ReadLine() again
            if (Console.ReadLine()!.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\n--- Correct Answers ---");
                foreach(var qa in userAnswers)
                {
                    Console.WriteLine($"Q: {qa.Item1.Text}");
                    Console.WriteLine($"Your answer: {qa.Item2 ?? "[No answer provided]"} | Correct answer: {qa.Item1.GetCorrectAnswer()}");
                    Console.WriteLine("--------------------");
                }
            }
        }
    }

    // Main Program Entry Point
    class Program
    {
        static void Main(string[] args)
        {
            Quiz quiz = new Quiz();
            bool exit = false;

            // --- REMOVED: The problematic reflection code for sample questions ---

            while (!exit)
            {
                Console.WriteLine("\n--- Geography Quiz Game ---");
                Console.WriteLine("1. Create a game (Add/Edit/Delete Questions)");
                Console.WriteLine("2. Play Game");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice: ");
                // Use '!' to assert that Console.ReadLine() is non-null
                string choice = Console.ReadLine()!; 

                switch (choice)
                {
                    case "1":
                        ManageQuestions(quiz);
                        break;
                    case "2":
                        quiz.Play();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        static void ManageQuestions(Quiz quiz)
        {
            bool back = false;
            while(!back)
            {
                Console.WriteLine("\n--- Manage Questions ---");
                Console.WriteLine("1. Add a new question");
                Console.WriteLine("2. Edit an existing question");
                Console.WriteLine("3. Delete an existing question");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("Enter your choice: ");
                // Use '!' to assert that Console.ReadLine() is non-null
                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        quiz.AddQuestion();
                        break;
                    case "2":
                        quiz.EditQuestion();
                        break;
                    case "3":
                        quiz.DeleteQuestion();
                        break;
                    case "4":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}