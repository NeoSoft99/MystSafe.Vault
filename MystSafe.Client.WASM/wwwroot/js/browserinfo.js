window.getBrowserInfo = function () {
    return {
        deviceMemory: navigator.deviceMemory || 0,
        userAgent: navigator.userAgent || 'unknown',
        hardwareConcurrency: navigator.hardwareConcurrency || 0,
        platform: navigator.platform || 'unknwon',
        language: navigator.language || 'unknown'
    };
};

