document.querySelectorAll('.matching-question').forEach(function (questionEl) {
    const qid = questionEl.getAttribute('data-question-id');
    const container = document.getElementById(`sortable_${qid}`);
    const hiddenInput = document.getElementById(`matchingInput_${qid}`);
    if (!container || !hiddenInput) return;

    let dragged = null;

    function saveOrder() {
        const items = Array.from(container.querySelectorAll('.term-card'));
        const order = items.map(el => parseInt(el.dataset.index));
        hiddenInput.value = JSON.stringify(order);
    }

    function handleDragStart(e) {
        dragged = e.currentTarget;
        setTimeout(() => dragged.classList.add('is-dragging'), 0);
        e.dataTransfer.effectAllowed = "move";
        e.dataTransfer.setData("text/plain", "");
    }

    function handleDragEnd() {
        if (dragged) {
            dragged.classList.remove('is-dragging');
            dragged = null;
        }
        saveOrder();
        if (typeof window.saveTaskAnswers === "function") window.saveTaskAnswers();
    }

    function handleDragOver(e) {
        e.preventDefault();
    }

    function handleDragEnter(e) {
        e.preventDefault();
        if (e.currentTarget !== dragged) {
            e.currentTarget.classList.add('drag-over');
        }
    }

    function handleDragLeave(e) {
        e.currentTarget.classList.remove('drag-over');
    }

    function handleDrop(e) {
        e.preventDefault();
        e.currentTarget.classList.remove('drag-over');
        if (!dragged || dragged === e.currentTarget) return;

        const afterElement = getDragAfterElement(container, e.clientY);
        if (afterElement == null) {
            container.appendChild(dragged);
        } else {
            container.insertBefore(dragged, afterElement);
        }

        dragged.classList.add('swapping');
        setTimeout(() => dragged.classList.remove('swapping'), 400);

        saveOrder();
        if (typeof window.saveTaskAnswers === "function") window.saveTaskAnswers();
    }

    function getDragAfterElement(container, y) {
        const draggableElements = [...container.querySelectorAll('.term-card:not(.is-dragging)')];
        return draggableElements.reduce((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = y - box.top - box.height / 2;
            if (offset < 0 && offset > closest.offset) {
                return { offset: offset, element: child };
            }
            return closest;
        }, { offset: Number.NEGATIVE_INFINITY }).element;
    }

    container.addEventListener('dragover', handleDragOver);
    container.addEventListener('drop', handleDrop);

    container.querySelectorAll('.term-card').forEach(card => {
        card.setAttribute('draggable', 'true');
        card.addEventListener('dragstart', handleDragStart);
        card.addEventListener('dragend', handleDragEnd);
        card.addEventListener('dragenter', handleDragEnter);
        card.addEventListener('dragleave', handleDragLeave);
        card.addEventListener('drop', handleDrop);
    });

    saveOrder();
        if (typeof window.saveTaskAnswers === "function") window.saveTaskAnswers();
});
