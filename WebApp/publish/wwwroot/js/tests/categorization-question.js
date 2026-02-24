// Глобальные функции — вызываются из HTML атрибутов ondrop/ondragover/etc
function allowDrop(ev) {
    ev.preventDefault();
}

function handleDragEnter(ev) {
    ev.preventDefault();
    const body = ev.target.closest('.category-body');
    if (body) {
        body.classList.add('highlight');
        if (!body.querySelector('.placeholder')) {
            const ph = document.createElement('div');
            ph.className = 'placeholder';
            body.appendChild(ph);
        }
    }
}

function handleDragLeave(ev) {
    const body = ev.target.closest('.category-body');
    if (body) {
        body.classList.remove('highlight');
        const ph = body.querySelector('.placeholder');
        if (ph) ph.remove();
    }
}

function drop(ev) {
    ev.preventDefault();
    const data = ev.dataTransfer.getData("text/plain");
    const draggedElement = document.querySelector(`[data-index="${data}"]`);
    if (!draggedElement) return;

    const body = ev.target.closest('.category-body');
    if (body) {
        const ph = body.querySelector('.placeholder');
        if (ph) ph.remove();

        body.appendChild(draggedElement);
        draggedElement.classList.remove('dragging');
        draggedElement.classList.add('animate-drop');

        setTimeout(() => draggedElement.classList.remove('animate-drop'), 500);

        collectAnswers();
    if (typeof window.saveTaskAnswers === "function") window.saveTaskAnswers();
        equalizeCategories();
    }
    handleDragLeave(ev);
}

function collectAnswers() {
    const answers = {};
    document.querySelectorAll('.drop-zone').forEach(zone => {
        const category = zone.getAttribute('data-category');
        zone.querySelectorAll('.term-item').forEach(term => {
            answers[term.getAttribute('data-index')] = category;
        });
    });
    const field = document.getElementById('categorizationAnswers');
    if (field) field.value = JSON.stringify(answers);
}

function equalizeCategories() {
    const bodies = document.querySelectorAll('.category-body');
    bodies.forEach(b => b.style.minHeight = '');
    const max = Math.max(...[...bodies].map(b => b.offsetHeight));
    bodies.forEach(b => b.style.minHeight = max + 'px');
}

// Инициализация после загрузки DOM
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.draggable').forEach(item => {
        item.addEventListener('dragstart', function (ev) {
            ev.dataTransfer.setData("text/plain", ev.target.getAttribute("data-index"));
            ev.target.classList.add('dragging');
        });
        item.addEventListener('dragend', function (ev) {
            ev.target.classList.remove('dragging');
        });
    });

    const form = document.querySelector('form');
    if (form) form.addEventListener('submit', collectAnswers);

    equalizeCategories();
});
