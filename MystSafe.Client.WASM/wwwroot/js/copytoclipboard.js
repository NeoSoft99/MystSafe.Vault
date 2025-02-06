function copyTextToClipboard(text) {
    if (navigator.clipboard && window.isSecureContext) {
        // Navigator clipboard is available
        return navigator.clipboard.writeText(text).then(function () {
            return true;
        }, function (err) {
            console.error("Could not copy text: ", err);
            return false;
        });
    } else {
        // Fallback: Create text area
        let textArea = document.createElement("textarea");
        textArea.value = text;
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        try {
            let successful = document.execCommand('copy');
            document.body.removeChild(textArea);
            return successful;
        } catch (err) {
            console.error("Fallback: Oops, unable to copy", err);
            document.body.removeChild(textArea);
            return false;
        }
    }
}
