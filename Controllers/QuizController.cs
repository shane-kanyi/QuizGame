using Microsoft.AspNetCore.Mvc;
using QuizGame;

namespace QuizWebApp.Controllers;

// These are simple classes to define the data structure for our API
public class QuestionDto
{
    public int QuestionIndex { get; set; }
    public string Text { get; set; } = "";
    public string Type { get; set; } = "";
    public List<string>? Options { get; set; }
}

public class AnswerPayload
{
    public int QuestionIndex { get; set; }
    public string Answer { get; set; } = "";
}

public class AnswerResultDto
{
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = "";
}

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly Quiz _quiz;

    public QuizController(Quiz quiz)
    {
        _quiz = quiz;
    }

    [HttpPost("start")]
    public IActionResult StartQuiz()
    {
        _quiz.StartNewGame();
        return Ok(new { message = "New game started." });
    }

    [HttpGet("question")]
    public ActionResult<QuestionDto> GetNextQuestion()
    {
        var (question, index) = _quiz.GetNextQuestion();
        if (question == null) return NoContent();

        var dto = new QuestionDto { QuestionIndex = index, Text = question.Text };
        
        if (question is MultipleChoiceQuestion mcq)
        {
            dto.Type = "MultipleChoice";
            dto.Options = mcq.Options;
        }
        else if (question is OpenEndedQuestion) dto.Type = "OpenEnded";
        else if (question is TrueFalseQuestion) dto.Type = "TrueFalse";

        return Ok(dto);
    }
    
    [HttpPost("answer")]
    public ActionResult<AnswerResultDto> SubmitAnswer([FromBody] AnswerPayload payload)
    {
        var question = _quiz.GetQuestionByIndex(payload.QuestionIndex);
        if (question == null) return NotFound();

        bool isCorrect = question.IsCorrect(payload.Answer);
        if (isCorrect) _quiz.IncrementScore();

        return Ok(new AnswerResultDto { IsCorrect = isCorrect, CorrectAnswer = question.GetCorrectAnswer() });
    }

    [HttpGet("results")]
    public IActionResult GetResults()
    {
        return Ok(new { Score = _quiz.Score, TotalQuestions = _quiz.TotalQuestions });
    }
}