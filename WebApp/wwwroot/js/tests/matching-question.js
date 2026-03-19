document.querySelectorAll('.matching-question').forEach(function (questionEl) {
    const qid = questionEl.getAttribute('data-question-id');
    const container = document.getElementById(`sortable_${qid}`);
    const hiddenInput = document.getElementById(`matchingInput_${qid}`);
    if (!container || !hiddenInput) return;

    let dragged = null;
    let ghost = null;
    let offsetX = 0, offsetY = 0;

    function equalizeHeights() {
        const definitions = Array.from(document.querySelectorAll(`#definitions_${qid} .definition-card`));
        const terms = Array.from(container.querySelectorAll('.term-card'));
        const count = Math.min(definitions.length, terms.length);

        definitions.forEach(el => el.style.height = '');
        terms.forEach(el => el.style.height = '');

        for (let i = 0; i < count; i++) {
            const h = Math.max(definitions[i].offsetHeight, terms[i].offsetHeight);
            definitions[i].style.height = h + 'px';
            terms[i].style.height = h + 'px';
        }
    }

    function saveOrder() {
        const items = Array.from(container.querySelectorAll('.term-card'));
        const order = items.map(el => parseInt(el.dataset.index));
        hiddenInput.value = JSON.stringify(order);
    }

    function createGhost(card, x, y) {
        const g = document.createElement('div');
        g.className = 'ghost-card';
        g.textContent = card.querySelector('.term-text')?.textContent ?? card.textContent.trim();
        const rect = card.getBoundingClientRect();
        g.style.width = rect.width + 'px';
        g.style.minHeight = rect.height + 'px';
        offsetX = x - rect.left;
        offsetY = y - rect.top;
        g.style.left = (x - offsetX) + 'px';
        g.style.top = (y - offsetY) + 'px';
        document.body.appendChild(g);
        return g;
    }

    function moveGhost(x, y) {
        if (!ghost) return;
        ghost.style.left = (x - offsetX) + 'px';
        ghost.style.top = (y - offsetY) + 'px';
    }

    function removeGhost() {
        if (ghost) { ghost.remove(); ghost = null; }
    }

    function clearIndicators() {
        container.querySelectorAll('.term-card').forEach(c => {
            c.classList.remove('drag-over-top', 'drag-over-bottom');
        });
    }

    function getTarget(y) {
        const cards = [...container.querySelectorAll('.term-card:not(.is-dragging)')];
        for (const card of cards) {
            const rect = card.getBoundingClientRect();
            if (y < rect.top + rect.height / 2) return { card, pos: 'top' };
        }
        return cards.length ? { card: cards[cards.length - 1], pos: 'bottom' } : null;
    }

    function onPointerDown(e) {
        if (e.button !== undefined && e.button !== 0) return;
        dragged = e.currentTarget;
        const x = e.clientX ?? e.touches[0].clientX;
        const y = e.clientY ?? e.touches[0].clientY;
        ghost = createGhost(dragged, x, y);
        dragged.classList.add('is-dragging');
        e.preventDefault();
    }

    function onPointerMove(e) {
        if (!dragged) return;
        const x = e.clientX ?? e.touches[0].clientX;
        const y = e.clientY ?? e.touches[0].clientY;
        moveGhost(x, y);
        clearIndicators();
        const target = getTarget(y);
        if (target) target.card.classList.add(target.pos === 'top' ? 'drag-over-top' : 'drag-over-bottom');
    }

    function onPointerUp(e) {
        if (!dragged) return;
        const x = e.clientX ?? e.changedTouches?.[0].clientX;
        const y = e.clientY ?? e.changedTouches?.[0].clientY;

        clearIndicators();
        const target = getTarget(y);

        if (target && target.card !== dragged) {
            if (target.pos === 'top') {
                container.insertBefore(dragged, target.card);
            } else {
                container.insertBefore(dragged, target.card.nextSibling);
            }
            dragged.classList.add('animate-swap');
            setTimeout(() => dragged && dragged.classList.remove('animate-swap'), 350);
        }

        dragged.classList.remove('is-dragging');
        dragged = null;
        removeGhost();
        equalizeHeights();
        saveOrder();
        if (typeof window.saveTaskAnswers === "function") window.saveTaskAnswers();
    }

    container.querySelectorAll('.term-card').forEach(card => {
        card.addEventListener('mousedown', onPointerDown);
        card.addEventListener('touchstart', onPointerDown, { passive: false });
    });

    document.addEventListener('mousemove', onPointerMove);
    document.addEventListener('touchmove', onPointerMove, { passive: false });
    document.addEventListener('mouseup', onPointerUp);
    document.addEventListener('touchend', onPointerUp);

    equalizeHeights();
    window.addEventListener('resize', equalizeHeights);
    saveOrder();
    if (typeof window.saveTaskAnswers === "function") window.saveTaskAnswers();
});