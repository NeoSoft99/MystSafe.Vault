// Make sure you have jsQR loaded via a <script> reference or a local import before this.
// e.g., <script src="https://cdn.jsdelivr.net/npm/jsqr@1.4.0/dist/jsQR.js"></script>

window.jsQrInterop = {
    // Store references & objects
    dotNetRef: null,
    video: null,
    canvas: null,
    ctx: null,
    stream: null,
    scanning: false,

    startJsQrScanner: async function (dotNetRef, videoElementId, canvasElementId) {
        // Save ref so we can call DotNet back
        this.dotNetRef = dotNetRef;
        this.video = document.getElementById(videoElementId);
        this.canvas = document.getElementById(canvasElementId);
        this.ctx = this.canvas.getContext("2d");
        this.scanning = true;

        try {
            // Request camera access (rear camera on mobile, if available)
            this.stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } });
            this.video.srcObject = this.stream;
            this.video.setAttribute("playsinline", true); // iOS Safari
            this.video.play();

            // Start scanning loop
            requestAnimationFrame(this.tick.bind(this));
        } catch (err) {
            console.error("Camera init error:", err);
        }
    },

    stopJsQrScanner: function () {
        this.scanning = false;
        if (this.stream) {
            this.stream.getTracks().forEach(track => track.stop());
        }
        this.stream = null;
    },

    tick: function () {
        if (!this.scanning) return;

        if (this.video.readyState === this.video.HAVE_ENOUGH_DATA) {
            // Match canvas size to video for consistent capture
            this.canvas.height = this.video.videoHeight;
            this.canvas.width = this.video.videoWidth;
            this.ctx.drawImage(this.video, 0, 0, this.canvas.width, this.canvas.height);

            // Grab frame data and pass to jsQR
            const imageData = this.ctx.getImageData(0, 0, this.canvas.width, this.canvas.height);
            const code = jsQR(imageData.data, imageData.width, imageData.height);

            if (code) {
                // We found a QR code! Send it to Blazor
                if (this.dotNetRef) {
                    this.dotNetRef.invokeMethodAsync("OnJsQrCodeDecoded", code.data);
                }
                // Optionally stop scanning if you only want one detection
                // this.stopJsQrScanner();
            }
        }
        requestAnimationFrame(this.tick.bind(this));
    }
};