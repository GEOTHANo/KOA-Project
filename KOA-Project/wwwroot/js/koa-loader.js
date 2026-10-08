/**
 * KOA-Project Global JavaScript Loader & Helpers
 */

window.renderMemberQR = function (canvasOrContainerId, qrValue, retries = 20) {
    return new Promise((resolve, reject) => {
        let targetEl = document.getElementById(canvasOrContainerId);
        if (!targetEl) {
            if (retries > 0) {
                setTimeout(() => window.renderMemberQR(canvasOrContainerId, qrValue, retries - 1).then(resolve).catch(reject), 150);
            } else {
                console.error('renderMemberQR: target element not found: ' + canvasOrContainerId);
                reject('element not found: ' + canvasOrContainerId);
            }
            return;
        }

        let canvas = targetEl;
        if (targetEl.tagName !== 'CANVAS') {
            canvas = targetEl.querySelector('canvas');
            if (!canvas) {
                canvas = document.createElement('canvas');
                canvas.className = 'rounded-lg shadow-sm';
                targetEl.appendChild(canvas);
            }
        }

        const options = {
            width: 160,
            margin: 2,
            color: { dark: '#1a3a5c', light: '#ffffff' }
        };

        if (typeof QRCode !== 'undefined' && typeof QRCode.toCanvas === 'function') {
            QRCode.toCanvas(canvas, qrValue, options, (err) => {
                if (err) {
                    console.warn('QRCode.toCanvas failed, trying fallback image draw:', err);
                    window.drawFallbackQR(canvas, qrValue);
                }
                resolve();
            });
        } else {
            if (retries > 0) {
                setTimeout(() => window.renderMemberQR(canvasOrContainerId, qrValue, retries - 1).then(resolve).catch(reject), 150);
            } else {
                console.warn('QRCode library not ready, using API fallback draw');
                window.drawFallbackQR(canvas, qrValue);
                resolve();
            }
        }
    });
};

window.drawFallbackQR = function (canvas, qrValue) {
    if (!canvas || canvas.tagName !== 'CANVAS') return;
    const img = new Image();
    img.crossOrigin = 'Anonymous';
    img.onload = function () {
        canvas.width = 160;
        canvas.height = 160;
        const ctx = canvas.getContext('2d');
        if (ctx) {
            ctx.drawImage(img, 0, 0, 160, 160);
        }
    };
    img.src = `https://api.qrserver.com/v1/create-qr-code/?size=160x160&data=${encodeURIComponent(qrValue)}&color=1a3a5c&bgcolor=ffffff`;
};

window.downloadMemberQR = function (canvasOrContainerId, fileName) {
    let targetEl = document.getElementById(canvasOrContainerId);
    if (!targetEl) return;
    let canvas = targetEl.tagName === 'CANVAS' ? targetEl : targetEl.querySelector('canvas');
    if (!canvas) return;
    try {
        const link = document.createElement('a');
        link.download = 'KOA-QR-' + fileName + '.png';
        link.href = canvas.toDataURL('image/png');
        link.click();
    } catch (e) {
        console.error('Error downloading QR code:', e);
    }
};

// ==========================================
// QR SCANNER (html5-qrcode)
// ==========================================
let _html5QrScanner = null;

window.startQrScanner = function (elementId, dotNetRef) {
    if (_html5QrScanner) {
        _html5QrScanner.stop().catch(() => {});
        _html5QrScanner = null;
    }
    if (typeof Html5Qrcode === 'undefined') {
        console.error('Html5Qrcode library not loaded.');
        return;
    }
    _html5QrScanner = new Html5Qrcode(elementId);
    _html5QrScanner.start(
        { facingMode: 'environment' },
        { fps: 10, qrbox: { width: 250, height: 250 } },
        (decodedText) => {
            dotNetRef.invokeMethodAsync('OnQrScanned', decodedText);
        },
        () => {}
    ).catch(err => console.warn('QR scanner error:', err));
};

window.stopQrScanner = function () {
    if (_html5QrScanner) {
        _html5QrScanner.stop().then(() => {
            _html5QrScanner.clear();
            _html5QrScanner = null;
        }).catch(() => { _html5QrScanner = null; });
    }
};

window.hideKoaLoader = function() {
    const loader = document.getElementById('koa-page-loader');
    if (loader) {
        loader.classList.add('koa-loader-fade-out');
        setTimeout(() => loader.style.display = 'none', 400);
    }
};

document.addEventListener('DOMContentLoaded', () => {
    setTimeout(window.hideKoaLoader, 300);
});
