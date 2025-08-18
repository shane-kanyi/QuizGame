document.addEventListener("DOMContentLoaded", () => {
    const gameArea = document.getElementById("game-area");
    const startScreen = document.getElementById("start-screen");
    const startButton = document.getElementById("start-button");
    let currentQuestion = null;

    // Start the quiz
    const startQuiz = async () => {
        startScreen.style.display = "none";
        gameArea.style.display = "block";
        await fetch("/api/quiz/start", { method: "POST" });
        await loadNextQuestion();
    };

    // Load the next question
    const loadNextQuestion = async () => {
        const response = await fetch("/api/quiz/question");
        if (response.status === 204) {
            showResults();
        } else {
            currentQuestion = await response.json();
            renderQuestion(currentQuestion);
        }
    };

    // Render the question
    const renderQuestion = (question) => {
        let html = `<h2 id="question-text">${question.text}</h2>`;
        switch (question.type) {
            case "MultipleChoice":
                html += '<div class="options-container">';
                question.options.forEach((option, idx) => {
                    html += `<button class="option-button" data-answer="${idx + 1}">${option}</button>`;
                });
                html += "</div>";
                break;
            case "TrueFalse":
                html += `<div class="options-container">
                    <button class="option-button" data-answer="true">True</button>
                    <button class="option-button" data-answer="false">False</button>
                </div>`;
                break;
            case "OpenEnded":
                html += `<input type="text" id="open-ended-input" placeholder="Type your answer...">
                    <button id="submit-button">Submit</button>`;
                break;
        }
        gameArea.innerHTML = html;
        setupAnswerListeners();
    };

    // Setup listeners for answer buttons
    const setupAnswerListeners = () => {
        document.querySelectorAll(".option-button").forEach(btn => {
            btn.addEventListener("click", () => submitAnswer(btn.dataset.answer));
        });
        const submitBtn = document.getElementById("submit-button");
        if (submitBtn) {
            submitBtn.addEventListener("click", () => {
                const input = document.getElementById("open-ended-input");
                submitAnswer(input.value);
            });
        }
    };

    // Submit the answer
    const submitAnswer = async (answer) => {
        const payload = {
            questionIndex: currentQuestion.questionIndex,
            answer: answer
        };
        const response = await fetch("/api/quiz/answer", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        const result = await response.json();
        showResult(result);
    };

    // Show result feedback
    const showResult = (result) => {
        let html = `<div id="result-text" class="${result.isCorrect ? "correct" : "incorrect"}">
            ${result.isCorrect ? "Correct!" : `Incorrect! The answer is: ${result.correctAnswer}`}
        </div>`;
        gameArea.innerHTML += html;
        setTimeout(loadNextQuestion, 2000);
    };

    // Show final results
    const showResults = async () => {
        const response = await fetch("/api/quiz/results");
        const results = await response.json();
        gameArea.innerHTML = `<h2>Quiz Complete!</h2>
            <p>Your final score is: ${results.score} / ${results.totalQuestions}</p>
            <button id="play-again-button">Play Again</button>`;
        document.getElementById("play-again-button").addEventListener("click", startQuiz);
    };

    startButton.addEventListener("click", startQuiz);
});