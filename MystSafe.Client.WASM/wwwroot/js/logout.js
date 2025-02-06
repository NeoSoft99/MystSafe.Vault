window.activityDetector = {
    timer: null,
    timeout: null, // Store the timeout value
    dotNetReference: null, // Store the DotNet reference

    startTimer: function (dotNetReference, timeout) {
        this.dotNetReference = dotNetReference; // Store the DotNet reference
        this.timeout = timeout; // Store the initial timeout value

        function resetTimer(newTimeout = window.activityDetector.timeout) {
            clearTimeout(window.activityDetector.timer);
            window.activityDetector.timeout = newTimeout; // Update the timeout value
            window.activityDetector.timer = setTimeout(() => window.activityDetector.dotNetReference.invokeMethodAsync('OnTimer'), newTimeout);
        }

        window.activityDetector.resetTimer = resetTimer; // Expose resetTimer

        var events = ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart'];
        events.forEach(function (name) {
            document.addEventListener(name, function () {
                resetTimer(window.activityDetector.timeout); // Use the current timeout value
            }, true);
        });

        window.addEventListener('blur', function () {
            resetTimer(window.activityDetector.timeout); // Use the current timeout value
        });

        resetTimer(this.timeout); // Start with the initial timeout
    },

    resetInactivityTimer: function (newTimeout) {
        if (window.activityDetector.resetTimer) {
            window.activityDetector.resetTimer(newTimeout);
        }
    }
};



//window.activityDetector = {
//    timer: null,

//    startTimer: function (dotNetReference, timeout) {
//        function resetTimer() {
//            clearTimeout(window.activityDetector.timer);
//            window.activityDetector.timer = setTimeout(() => dotNetReference.invokeMethodAsync('OnTimer'), timeout);
//        }

//        window.activityDetector.resetTimer = resetTimer; // Expose resetTimer

//        var events = ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart'];
//        events.forEach(function (name) {
//            document.addEventListener(name, resetTimer, true);
//        });

//        window.addEventListener('blur', resetTimer);

//        resetTimer();
//    },

//    resetInactivityTimer: function () {
//        if (window.activityDetector.resetTimer) {
//            window.activityDetector.resetTimer();
//        }
//    }
//};
