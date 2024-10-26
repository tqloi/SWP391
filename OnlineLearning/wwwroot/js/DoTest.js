var timeLeft = 2412; // Set the time in seconds (40 minutes and 12 seconds), currently not use yet?
var currentQuestionIndex = 0;
var numberOfChoosenQuestions = 0; // Initialize number of selected questions

// Function to update the progress bar based on selected answers
function updateProgressBar() {
    // Calculate the progress percentage (based on how many answers are selected)
    var selectedOptions = document.querySelectorAll('input[type="radio"]:checked');
    var progressPercentage = (selectedOptions.length / totalQuestions) * 100;

    // Update the progress bar width and text
    var progressBar = document.querySelector('.progress-bar');
    progressBar.style.width = progressPercentage + "%";
    progressBar.setAttribute('aria-valuenow', progressPercentage);
}

// Function to update the number of selected questions
function updateChosenQuestions() {
    // Select all radio buttons that are checked
    var selectedOptions = document.querySelectorAll('input[type="radio"]:checked');
    numberOfChoosenQuestions = selectedOptions.length;

    // Update the progress bar after each question is answered
    updateProgressBar();
}
//make sure at lease one question is checked
document.querySelector("form").addEventListener("submit", function (event) {
    const answers = document.querySelectorAll("input[type='radio']:checked");
    if (answers.length === 0) {
        event.preventDefault();
        alert("Please select at least one answer before submitting.");
    }
});

// Add onchange event to all radio buttons
document.addEventListener('DOMContentLoaded', function () {
    var radios = document.querySelectorAll('input[type="radio"]');
    radios.forEach(function (radio) {
        radio.addEventListener('change', updateChosenQuestions);
    });

    // Initialize the progress bar when the page loads
    updateProgressBar();
});

// Function to hide all questions
function hideAllQuestions() {
    var allQuestions = document.querySelectorAll('[id^="question-"]');
    allQuestions.forEach(function (question) {
        question.style.display = "none"; // Hide each question
    });
}

// Function to show a specific question by index
function showQuestion(index) {
    // Hide all questions first
    hideAllQuestions();

    // Show the selected question
    var currentQuestion = document.getElementById("question-" + index);
    if (currentQuestion) {
        currentQuestion.style.display = "block";
    }

    // Update active button in navigation
    document.querySelectorAll(".question-nav-buttons .btn").forEach((btn, i) => {
        btn.classList.toggle("active", i === index);
    });

    // Update currentQuestionIndex to the selected question index
    currentQuestionIndex = index;

    // Update the progress bar
    updateProgressBar();
}


// Function to show the next question
function nextQuestion() {
    if (currentQuestionIndex < totalQuestions - 1) {
        currentQuestionIndex++;
        showQuestion(currentQuestionIndex); // Show the next question
    } else {
        alert("You are on the last question.");
    }
}
// Timer logic (unchanged from your previous code)
function updateTimer() {
    var minutes = Math.floor(timeLeft / 60);
    var seconds = timeLeft % 60;

    // Update the timer display
    document.querySelector('.timer').textContent = `Time left: ${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

    timeLeft--;

    if (timeLeft >= 0) {
        setTimeout(updateTimer, 1000); // Call updateTimer every 1 second
    } else {
        alert('Time is up!');
        document.querySelector('form').submit();
    }
}

// Initialize by showing the first question and hiding the rest
window.onload = function () {
    hideAllQuestions();
    showQuestion(currentQuestionIndex);
    updateTimer();
    noBack();
};

window.history.forward();
//Disable Browser Back Button
function noBack() {
    window.history.forward();
}
window.onload = noBack;
window.onpageshow = function (evt) {
    if (evt.persisted) noBack();
};
window.onunload = function () { null };
