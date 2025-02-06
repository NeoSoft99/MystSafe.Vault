function autoAdjustTextareaHeightBySelector(selector) {
    var textareas = document.querySelectorAll(selector);
    textareas.forEach(textarea => {
        textarea.style.height = 'auto';
        textarea.style.height = textarea.scrollHeight + 'px';
    });
}
