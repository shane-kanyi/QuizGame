using Microsoft.AspNetCore.Mvc;
using QuizGame;

namespace QuizWebApp.Controllers;

// --- THIS SECTION IS THE FIX: The DTOs now have their properties ---
public class CreateQuizPayload { public string Name { get; set; } = ""; }
public class AddQuestionPayload {
    public string Type { get; set; } = "";
    public string Text { get; set; } = "";
    public List<string>? Options { get; set; }
    public int? CorrectOptionIndex { get; set; }
    public List<string>? AcceptableAnswers { get; set; }
    public bool? CorrectBoolAnswer { get; set; }
}
public class QuestionDto { 
    public int QuestionIndex { get; set; }
    public string Text { get; set; } = "";
    public string Type { get; set; } = "";
    public List<string>? Options { get; set; }
}
public class AnswerPayload { 
    public int QuestionIndex { get; set; }
    public string Answer { get; set; } = "";
}
public class AnswerResultDto { 
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = "";
}

[ApiController]
[Route("api/quizzes")]
public class QuizController : ControllerBase
{
    private readonly QuizService _quizService;

    public QuizController(QuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpPost]
    public IActionResult CreateQuiz([FromBody] CreateQuizPayload payload)
    {
        var quizId = _quizService.CreateQuiz(payload.Name);
        return Ok(new { id = quizId, name = payload.Name });
    }

    [HttpGet]
    public IActionResult GetAllQuizzes()
    {
        return Ok(_quizService.GetAllQuizzes());
    }

    [HttpPost("{id}/questions")]
    public IActionResult AddQuestion(string id, [FromBody] AddQuestionPayload payload)
    {
        var quiz = _quizService.GetQuiz(id);
        if (quiz == null) return NotFound();

        Question? newQuestion = payload.Type switch
        {
            "MultipleChoice" when payload.Options != null && payload.CorrectOptionIndex.HasValue => 
                new MultipleChoiceQuestion(payload.Text, payload.Options, payload.CorrectOptionIndex.Value),
            "OpenEnded" when payload.AcceptableAnswers != null => 
                new OpenEndedQuestion(payload.Text, payload.AcceptableAnswers),
            "TrueFalse" when payload.CorrectBoolAnswer.HasValue => 
                new TrueFalseQuestion(payload.Text, payload.CorrectBoolAnswer.Value),
            _ => null
        };
        
        if (newQuestion == null) return BadRequest("Invalid question payload.");
        
        quiz.AddQuestion(newQuestion);
        return Ok(new { message = "Question added." });
    }

    [HttpPost("{id}/start")]
    public IActionResult StartQuiz(string id)
    {
        var quiz = _quizService.GetQuiz(id);
        if (quiz == null) return NotFound();
        quiz.StartNewGame();
        return Ok(new { message = "New game started." });
    }

    [HttpGet("{id}/question")]
    public ActionResult<QuestionDto> GetNextQuestion(string id)
    {
        var quiz = _quizService.GetQuiz(id);
        if (quiz == null) return NotFound();
        
        var (question, index) = quiz.GetNextQuestion();
        if (question == null) return NoContent();

        var dto = new QuestionDto { QuestionIndex = index, Text = question.Text };
        if (question is MultipleChoiceQuestion mcq) { dto.Type = "MultipleChoice"; dto.Options = mcq.Options; }
        else if (question is OpenEndedQuestion) { dto.Type = "OpenEnded"; }
        else if (question is TrueFalseQuestion) { dto.Type = "TrueFalse"; }

        return Ok(dto);
    }
    
    [HttpPost("{id}/answer")]
    public ActionResult<AnswerResultDto> SubmitAnswer(string id, [FromBody] AnswerPayload payload)
    {
        var quiz = _quizService.GetQuiz(id);
        if (quiz == null) return NotFound();
        
        var question = quiz.GetQuestionByIndex(payload.QuestionIndex);
        if (question == null) return NotFound();

        bool isCorrect = question.IsCorrect(payload.Answer);
        if (isCorrect) quiz.IncrementScore();

        return Ok(new AnswerResultDto { IsCorrect = isCorrect, CorrectAnswer = question.GetCorrectAnswer() });
    }

    [HttpGet("{id}/results")]
    public IActionResult GetResults(string id)
    {
        var quiz = _quizService.GetQuiz(id);
        if (quiz == null) return NotFound();
        return Ok(new { Score = quiz.Score, TotalQuestions = quiz.TotalQuestions });
    }
}