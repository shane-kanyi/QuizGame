using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace QuizGame
{
    // This service will be our new Singleton. It manages all quiz instances.
    public class QuizService
    {
        // We use a ConcurrentDictionary to safely handle web requests from multiple users.
        private readonly ConcurrentDictionary<string, Quiz> _quizzes = new ConcurrentDictionary<string, Quiz>();

        public string CreateQuiz(string name)
        {
            // Generate a unique, URL-friendly ID for the quiz.
            var id = $"{name.ToLower().Replace(" ", "-")}-{Guid.NewGuid().ToString().Substring(0, 4)}";
            var newQuiz = new Quiz(name);
            _quizzes.TryAdd(id, newQuiz);
            return id;
        }

        public Quiz? GetQuiz(string id)
        {
            _quizzes.TryGetValue(id, out var quiz);
            return quiz;
        }

        public IEnumerable<object> GetAllQuizzes()
        {
            // Return a simple list of quiz names and IDs for the selection screen.
            return _quizzes.Select(pair => new { id = pair.Key, name = pair.Value.Name });
        }
    }
}