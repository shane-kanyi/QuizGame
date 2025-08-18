document.addEventListener('DOMContentLoaded', () => {
    // --- State Management ---
    let currentView = 'home';
    let currentQuiz = null; // { id, name }
    let currentQuestion = null; // The question being played

    // --- View Elements ---
    const views = {
        home: document.getElementById('home-view'),
        create: document.getElementById('create-quiz-view'),
        addQuestion: document.getElementById('add-question-view'),
        playList: document.getElementById('play-quiz-list-view'),
        game: document.getElementById('game-view'),
    };

    // --- Button/Input Elements ---
    const showCreateBtn = document.getElementById('show-create-view-btn');
    const showPlayBtn = document.getElementById('show-play-view-btn');
    const createQuizBtn = document.getElementById('create-quiz-btn');
    // ... (other buttons) ...
    const finishQuizBtn = document.getElementById('finish-quiz-btn');
    const quizListContainer = document.getElementById('quiz-list-container');
    const backButtons = document.querySelectorAll('.back-btn'); 

    // --- Navigation Logic ---
    const navigateTo = (viewName) => {
        Object.values(views).forEach(view => view.classList.add('hidden'));
        views[viewName].classList.remove('hidden');
        currentView = viewName;
    };

    const goHome = () => navigateTo('home');

    // --- Event Listeners for Navigation ---
    showCreateBtn.addEventListener('click', () => navigateTo('create'));
    showPlayBtn.addEventListener('click', async () => {
        await renderQuizList();
        navigateTo('playList');
    });
    finishQuizBtn.addEventListener('click', goHome);
    backButtons.forEach(btn => btn.addEventListener('click', goHome)); 

    // --- Core Logic: Creating a Quiz ---
    createQuizBtn.addEventListener('click', async () => {
        const name = quizNameInput.value.trim();
        if (!name) { alert('Please enter a quiz name.'); return; }

        const response = await fetch('/api/quizzes', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name })
        });
        currentQuiz = await response.json();
        
        document.getElementById('add-question-title').innerText = `Adding to: ${currentQuiz.name}`;
        renderQuestionForm();
        navigateTo('addQuestion');
    });

    // --- Core Logic: Adding Questions ---
    questionTypeSelect.addEventListener('change', renderQuestionForm);
    addQuestionBtn.addEventListener('click', async () => {
        const payload = buildQuestionPayload();
        if (!payload) return; // Error is handled inside the builder

        await fetch(`/api/quizzes/${currentQuiz.id}/questions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        
        alert('Question added!');
        renderQuestionForm(); // Clear the form for the next question
    });

    function renderQuestionForm() {
        const type = questionTypeSelect.value;
        let html = '<input type="text" id="q-text" placeholder="Question Text..." class="question-input">';
        switch (type) {
            case 'MultipleChoice':
                html += `
                    <input type="text" placeholder="Option 1" class="option-input">
                    <input type="text" placeholder="Option 2" class="option-input">
                    <input type="text" placeholder="Option 3" class="option-input">
                    <input type="text" placeholder="Option 4" class="option-input">
                    <input type="number" id="q-correct-index" placeholder="Correct Option Number (1-4)" min="1" max="4">
                `;
                break;
            case 'OpenEnded':
                html += '<input type="text" id="q-answers" placeholder="Acceptable Answers (comma-separated)">';
                break;
            case 'TrueFalse':
                html += `
                    <select id="q-correct-bool">
                        <option value="true">True</option>
                        <option value="false">False</option>
                    </select>
                `;
                break;
        }
        questionFormArea.innerHTML = html;
    }

    function buildQuestionPayload() {
        const payload = {
            type: questionTypeSelect.value,
            text: document.getElementById('q-text').value.trim()
        };
        if (!payload.text) { alert('Question text cannot be empty.'); return null; }

        switch (payload.type) {
            case 'MultipleChoice':
                payload.options = Array.from(document.querySelectorAll('.option-input')).map(i => i.value.trim());
                payload.correctOptionIndex = parseInt(document.getElementById('q-correct-index').value, 10) - 1;
                if (payload.options.some(o => !o) || isNaN(payload.correctOptionIndex)) {
                    alert('All multiple choice fields are required.'); return null;
                }
                break;
            case 'OpenEnded':
                payload.acceptableAnswers = document.getElementById('q-answers').value.split(',').map(a => a.trim());
                if (payload.acceptableAnswers.length === 0 || !payload.acceptableAnswers[0]) {
                    alert('Please provide at least one acceptable answer.'); return null;
                }
                break;
            case 'TrueFalse':
                payload.correctBoolAnswer = document.getElementById('q-correct-bool').value === 'true';
                break;
        }
        return payload;
    }

    // --- Core Logic: Playing a Quiz ---
    async function renderQuizList() {
        const response = await fetch('/api/quizzes');
        const quizzes = await response.json();
        if (quizzes.length === 0) {
            quizListContainer.innerHTML = '<p>No quizzes have been created yet. Go create one!</p>';
            return;
        }
        quizListContainer.innerHTML = quizzes.map(q => 
            `<button class="quiz-select-btn" data-id="${q.id}" data-name="${q.name}">${q.name}</button>`
        ).join('');
        
        document.querySelectorAll('.quiz-select-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                currentQuiz = { id: btn.dataset.id, name: btn.dataset.name };
                startGame();
            });
        });
    }

    const startGame = async () => {
        navigateTo('game');
        await fetch(`/api/quizzes/${currentQuiz.id}/start`, { method: 'POST' });
        await fetchPlayQuestion();
    };

    const fetchPlayQuestion = async () => {
        const response = await fetch(`/api/quizzes/${currentQuiz.id}/question`);
        if (response.status === 204) { showResults(); return; }
        currentQuestion = await response.json();
        renderPlayQuestion(currentQuestion);
    };
    
    // Renders the actual game question (re-uses some old logic)
    const renderPlayQuestion = (question) => {
        let html = `<h2 id="question-text">${question.text}</h2>`;
        switch (question.type) {
            case 'MultipleChoice':
                html += '<div class="options-container">' + question.options.map((opt, i) => `<button class="option-button" data-answer="${i + 1}">${opt}</button>`).join('') + '</div>';
                break;
            case 'TrueFalse':
                html += `<div class="options-container"><button class="option-button" data-answer="true">True</button><button class="option-button" data-answer="false">False</button></div>`;
                break;
            case 'OpenEnded':
                html += `<input type="text" id="open-ended-input" placeholder="Type your answer..."><button id="submit-button">Submit</button>`;
                break;
        }
        views.game.innerHTML = html;
        
        document.querySelectorAll('.option-button').forEach(b => b.addEventListener('click', () => handlePlaySubmit(b.dataset.answer)));
        const submitBtn = document.getElementById('submit-button');
        if (submitBtn) submitBtn.addEventListener('click', () => handlePlaySubmit(document.getElementById('open-ended-input').value));
    };

    const handlePlaySubmit = async (answer) => {
        const response = await fetch(`/api/quizzes/${currentQuiz.id}/answer`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ questionIndex: currentQuestion.questionIndex, answer })
        });
        const result = await response.json();
        
        let feedbackHtml = `<div id="result-text" class="${result.isCorrect ? 'correct' : 'incorrect'}">
                                ${result.isCorrect ? 'Correct!' : `Incorrect! The answer is: ${result.correctAnswer}`}
                            </div>`;
        views.game.innerHTML += feedbackHtml;
        setTimeout(fetchPlayQuestion, 2000);
    };

    const showResults = async () => {
        const response = await fetch(`/api/quizzes/${currentQuiz.id}/results`);
        const results = await response.json();
        views.game.innerHTML = `<h2>Quiz Complete!</h2>
                              <p>Your final score for "${currentQuiz.name}" is: ${results.score} / ${results.totalQuestions}</p>
                              <button id="play-again-button">Back to Home</button>`;
        document.getElementById('play-again-button').addEventListener('click', goHome);
    };
});