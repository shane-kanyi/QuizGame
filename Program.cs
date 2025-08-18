using QuizGame; // This tells our server about the Quiz logic

var builder = WebApplication.CreateBuilder(args);

// --- Services Configuration ---

// Add services to the container. We'll use this for our API.
builder.Services.AddControllers();

// **CRITICAL:** We register our Quiz class as a Singleton.
// This means there will be only ONE instance of the Quiz for the entire
// application. All users will be playing the same game instance.
builder.Services.AddSingleton<QuizService>();


var app = builder.Build();

// --- Middleware Pipeline ---

// This tells the app to serve static files (like index.html, css, js) from the wwwroot folder.
app.UseDefaultFiles(); // Looks for index.html by default
app.UseStaticFiles();

// This maps the API endpoints we will create in our QuizController.
app.MapControllers();

app.Run();