document.querySelectorAll('.ai-button').forEach(button => {
    button.addEventListener('click', async function () {
        const questionNumber = this.getAttribute('data-question');
        const inputField = document.getElementById(`userInput-${questionNumber}`);
        const input = inputField.value;

        const { GoogleGenerativeAI } = await import('https://esm.run/@google/generative-ai');
        const genAI = new GoogleGenerativeAI("AIzaSyAV5j80BEDSj6l1bAL2mqXorlXp91VLdP8");

        const model = genAI.getGenerativeModel({ model: "gemini-1.5-flash" });
        const prompt = `"${input}"`;

        const result = await model.generateContent(prompt);
        const response = await result.response;
        const text = await response.text();

        // Convert Markdown to HTML
        const htmlContent = marked.parse(text);

        // Display formatted response in the corresponding question
        const displayElement = document.getElementById(`displayText-${questionNumber}`);
        displayElement.innerHTML = htmlContent;

        // Apply highlight.js to code blocks
        document.querySelectorAll('pre code').forEach((block) => {
            hljs.highlightElement(block);
        });

        //console.log(text);
    });
});
