// Drag and Drop for Todo Kanban
// Enables dragging todos between Open/Completed columns
// Triggers existing toggle endpoint on drop

let dragProxy = null;
let draggedItem = null;
let sourceZone = null;

document.addEventListener('dragstart', e => {
    const item = e.target.closest('[draggable="true"]');
    if (!item) return;

    draggedItem = item;
    sourceZone = item.closest('[data-drop-zone]');
    if (!sourceZone) return; // Not in a drop zone, ignore

    // Create visual proxy that follows mouse
    dragProxy = item.cloneNode(true);
    dragProxy.className = 'drag-proxy';
    dragProxy.style.width = item.offsetWidth + 'px';
    document.body.appendChild(dragProxy);

    // Hide native ghost (set to 1x1 transparent element)
    const ghost = document.createElement('div');
    e.dataTransfer.setDragImage(ghost, 0, 0);
    e.dataTransfer.effectAllowed = 'move';

    item.classList.add('dragging');
});

document.addEventListener('dragover', e => {
    if (!draggedItem) return;
    e.preventDefault();

    // Proxy follows mouse - top-left at cursor
    if (dragProxy) {
        dragProxy.style.left = e.clientX + 'px';
        dragProxy.style.top = e.clientY + 'px';
    }

    // Highlight valid drop zone (the OTHER zone, not source)
    document.querySelectorAll('.drag-over').forEach(el => el.classList.remove('drag-over'));
    const zone = e.target.closest('[data-drop-zone]');
    if (zone && zone !== sourceZone) {
        zone.classList.add('drag-over');
    }
});

document.addEventListener('drop', e => {
    const zone = e.target.closest('[data-drop-zone]');
    if (zone && zone !== sourceZone && draggedItem) {
        // Extract ID from data-drag-id and trigger toggle
        const dragId = draggedItem.dataset.dragId;
        if (dragId) {
            htmx.ajax('POST', '/interaction/todos/toggle', {values: {id: dragId}, swap: 'none'});
        }
    }
    cleanup();
});

document.addEventListener('dragend', cleanup);

function cleanup() {
    if (dragProxy) {
        dragProxy.remove();
        dragProxy = null;
    }
    if (draggedItem) {
        draggedItem.classList.remove('dragging');
        draggedItem = null;
    }
    document.querySelectorAll('.drag-over').forEach(el => el.classList.remove('drag-over'));
    sourceZone = null;
}
