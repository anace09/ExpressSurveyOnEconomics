(function () {
    const form         = document.querySelector("form");
    const modal        = document.getElementById("submitModal");
    const timerDisplay = document.getElementById("timer");
    const cheatOverlay = document.getElementById("cheatOverlay");
    const contentVeil  = document.getElementById("contentVeil");
    const countdownEl  = document.getElementById("cheatCountdown");
    const exitedField  = document.getElementById("exitedFullscreen");

    document.body.appendChild(modal);
    document.body.appendChild(cheatOverlay);
    document.body.appendChild(contentVeil);

    if (!form || !timerDisplay) return;

    // Время окончания передаётся через data-атрибут на теге <form>
    const endTime = new Date(form.dataset.endTime);

    let forceSubmit      = false;
    let fullscreenActive = false;
    let cheatPending     = false;
    let cheatTimer       = null;
    const CHEAT_SEC      = 10;

    function doSubmit() {
        if (forceSubmit) return;
        forceSubmit = true;
        form.submit();
    }

    function isFullscreenNow() {
        return !!(document.fullscreenElement      ||
                  document.webkitFullscreenElement ||
                  document.mozFullScreenElement    ||
                  document.msFullscreenElement);
    }

    function enterFullscreen() {
        const el = document.documentElement;
        const fn = el.requestFullscreen      ||
                   el.webkitRequestFullscreen ||
                   el.mozRequestFullScreen    ||
                   el.msRequestFullscreen;
        if (fn) return fn.call(el).catch(() => {});
        return Promise.resolve();
    }

    function isComplete() {
        const cat = document.getElementById('categorizationAnswers');
        if (cat) {
            const total    = document.querySelectorAll('.term-item').length;
            const answered = Object.keys(JSON.parse(cat.value || '{}')).length;
            if (answered < total) return false;
        }
        const radioNames = [...new Set(
            [...document.querySelectorAll('input[type="radio"]')].map(r => r.name)
        )];
        for (const name of radioNames) {
            if (!document.querySelector(`input[name="${name}"]:checked`)) return false;
        }
        const allInputs = [...document.querySelectorAll('.cell-input, .condition-select, input[name^="optimal"]')];
        if (allInputs.some(el => !el.value.trim())) return false;
        return true;
    }

    // ── Submit modal ──────────────────────────────────────────────────────────
    form.addEventListener('submit', function (e) {
        if (forceSubmit) return;
        if (!isComplete()) {
            e.preventDefault();
            modal.classList.add('show');
        }
    });

    document.getElementById('modalConfirm').addEventListener('click', function () {
        modal.classList.remove('show');
        doSubmit();
    });

    document.getElementById('modalCancel').addEventListener('click', function () {
        modal.classList.remove('show');
    });

    modal.addEventListener('click', function (e) {
        if (e.target === modal) modal.classList.remove('show');
    });

    // ── Cheat countdown ───────────────────────────────────────────────────────
    function startCheat() {
        if (cheatPending || forceSubmit) return;
        cheatPending = true;
        if (exitedField) exitedField.value = 'true';
        cheatOverlay.classList.add('show');

        let rem = CHEAT_SEC;
        countdownEl.textContent = rem;

        cheatTimer = setInterval(() => {
            rem--;
            countdownEl.textContent = rem;
            if (rem <= 0) {
                clearInterval(cheatTimer);
                doSubmit();
            }
        }, 1000);
    }

    function cancelCheat() {
        if (!cheatPending || forceSubmit) return;
        clearInterval(cheatTimer);
        cheatTimer   = null;
        cheatPending = false;
        if (exitedField) exitedField.value = 'false';
        cheatOverlay.classList.remove('show');
    }

    cheatOverlay.addEventListener('click', function () {
        if (cheatPending && !forceSubmit) enterFullscreen();
    });

    // ── Fullscreen ────────────────────────────────────────────────────────────
    contentVeil.style.display = 'flex';

    contentVeil.addEventListener('click', function () {
        enterFullscreen().catch(() => {
            contentVeil.style.display = 'none';
        });
    });

    function onFSChange() {
        const isFS = isFullscreenNow();
        if (isFS) {
            fullscreenActive = true;
            contentVeil.style.display = 'none';
            cancelCheat();
        } else if (fullscreenActive && !forceSubmit) {
            startCheat();
        }
    }

    document.addEventListener('fullscreenchange',       onFSChange);
    document.addEventListener('webkitfullscreenchange', onFSChange);
    document.addEventListener('mozfullscreenchange',    onFSChange);
    document.addEventListener('MSFullscreenChange',     onFSChange);

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && fullscreenActive && !forceSubmit) {
            setTimeout(() => {
                if (!isFullscreenNow()) enterFullscreen();
            }, 150);
        }
    });

    document.addEventListener('visibilitychange', function () {
        if (document.hidden && fullscreenActive && !forceSubmit && !cheatPending) {
            startCheat();
        }
        if (!document.hidden && cheatPending && !forceSubmit) {
            if (isFullscreenNow()) cancelCheat();
        }
    });

    window.addEventListener('blur', function () {
        if (fullscreenActive && !forceSubmit && !cheatPending) {
            startCheat();
        }
    });

    window.addEventListener('focus', function () {
        if (cheatPending && !forceSubmit) {
            setTimeout(() => {
                if (isFullscreenNow()) cancelCheat();
            }, 300);
        }
    });

    // ── Таймер ───────────────────────────────────────────────────────────────
    function updateTimer() {
        const rem = Math.floor((endTime - Date.now()) / 1000);
        if (rem <= 0) {
            timerDisplay.textContent = "Время вышло!";
            timerDisplay.className = "timer-value text-danger";
            doSubmit();
            return;
        }
        const m = Math.floor(rem / 60);
        const s = rem % 60;
        timerDisplay.textContent = m.toString().padStart(2,'0') + ':' + s.toString().padStart(2,'0');
        timerDisplay.classList.toggle("text-warning", rem <= 10);
        timerDisplay.classList.toggle("text-danger",  false);
        setTimeout(updateTimer, 400);
    }

    updateTimer();
})();


// ── Сохранение ответов в sessionStorage ──────────────────────────────────────
const TASK_STORAGE_KEY = 'task_answers_' + (document.querySelector('[name="taskId"]')?.value || '');

window.saveTaskAnswers = function () {
    const data = {};

    document.querySelectorAll('input[type="radio"]:checked').forEach(el => {
        data[el.name] = el.value;
    });

    document.querySelectorAll('.cell-input, .condition-select').forEach(el => {
        if (el.name) data[el.name] = el.value;
    });

    const cat = document.getElementById('categorizationAnswers');
    if (cat && cat.value) data['__categorizationAnswers'] = cat.value;

    document.querySelectorAll('[id^="matchingInput_"]').forEach(el => {
        if (el.value) data[el.id] = el.value;
    });

    sessionStorage.setItem(TASK_STORAGE_KEY, JSON.stringify(data));
};

function restoreTaskAnswers() {
    const raw = sessionStorage.getItem(TASK_STORAGE_KEY);
    if (!raw) return;
    let data;
    try { data = JSON.parse(raw); } catch { return; }

    Object.entries(data).forEach(([name, value]) => {
        const radio = document.querySelector(`input[type="radio"][name="${name}"][value="${value}"]`);
        if (radio) radio.checked = true;
    });

    document.querySelectorAll('.cell-input, .condition-select').forEach(el => {
        if (el.name && data[el.name] !== undefined) el.value = data[el.name];
    });

    if (data['__categorizationAnswers']) {
        const cat = document.getElementById('categorizationAnswers');
        if (cat) cat.value = data['__categorizationAnswers'];
    }

    document.querySelectorAll('[id^="matchingInput_"]').forEach(el => {
        if (data[el.id] !== undefined) el.value = data[el.id];
    });
}

document.addEventListener('DOMContentLoaded', restoreTaskAnswers);
document.addEventListener('change', window.saveTaskAnswers);
document.addEventListener('input', window.saveTaskAnswers);

const _saveForm = document.querySelector('form');
if (_saveForm) {
    _saveForm.addEventListener('submit', function () {
        sessionStorage.removeItem(TASK_STORAGE_KEY);
    });
}
