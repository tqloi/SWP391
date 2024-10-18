// Timer variables
var timeLeft = 2412; // Set the time in seconds (40 minutes and 12 seconds)

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
        // Optionally auto-submit the form
        document.querySelector('form').submit();
    }
}

// Start the timer when the page loads
window.onload = function () {
    updateTimer();
};

// Question navigation variables
var currentQuestionIndex = 0;
var totalQuestions = 0; // This will be initialized by Razor

// Function to show a specific question by index
function showQuestion(index) {
    // Hide the current question
    document.getElementById("question-" + currentQuestionIndex).style.display = "none";
    document.querySelector(".question-nav-buttons .btn:nth-child(" + (currentQuestionIndex + 1) + ")").classList.remove("active");

    // Show the new question
    currentQuestionIndex = index;
    document.getElementById("question-" + currentQuestionIndex).style.display = "block";
    document.querySelector(".question-nav-buttons .btn:nth-child(" + (currentQuestionIndex + 1) + ")").classList.add("active");
}

// Function to show the next question
function nextQuestion() {
    if (currentQuestionIndex < totalQuestions - 1) {
        showQuestion(currentQuestionIndex + 1);
    }
}
