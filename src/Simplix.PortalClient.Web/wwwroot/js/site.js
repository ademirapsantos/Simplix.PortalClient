(function () {
  const board = document.querySelector("[data-ticket-board]");
  if (!board) {
    return;
  }

  const moveForm = document.getElementById("ticket-move-form");
  const moveId = document.getElementById("ticket-move-id");
  const moveStatus = document.getElementById("ticket-move-status");
  const canManageWorkflow = board.dataset.canManageWorkflow === "true";
  const edgeThreshold = 120;
  const scrollStep = 18;
  let draggedCard = null;
  let suppressClick = false;
  let autoScrollFrame = null;
  let horizontalScrollDirection = 0;

  const stopAutoScroll = () => {
    horizontalScrollDirection = 0;
    if (autoScrollFrame !== null) {
      window.cancelAnimationFrame(autoScrollFrame);
      autoScrollFrame = null;
    }
  };

  const runAutoScroll = () => {
    if (horizontalScrollDirection === 0) {
      autoScrollFrame = null;
      return;
    }

    board.scrollLeft += scrollStep * horizontalScrollDirection;
    autoScrollFrame = window.requestAnimationFrame(runAutoScroll);
  };

  const updateAutoScroll = (clientX) => {
    const bounds = board.getBoundingClientRect();
    const nearLeftEdge = clientX <= bounds.left + edgeThreshold;
    const nearRightEdge = clientX >= bounds.right - edgeThreshold;

    if (nearLeftEdge && board.scrollLeft > 0) {
      horizontalScrollDirection = -1;
    } else if (nearRightEdge && board.scrollLeft + board.clientWidth < board.scrollWidth) {
      horizontalScrollDirection = 1;
    } else {
      stopAutoScroll();
      return;
    }

    if (autoScrollFrame === null) {
      autoScrollFrame = window.requestAnimationFrame(runAutoScroll);
    }
  };

  const canDropCardInColumn = (card, targetStatus) => {
    if (canManageWorkflow) {
      return true;
    }

    const currentStatus = card.dataset.ticketStatus;
    const canClientComplete = card.dataset.canClientComplete === "true";

    if (!canClientComplete) {
      return false;
    }

    return targetStatus === "Concluido" &&
      (currentStatus === "Criado" || currentStatus === "Homologacao");
  };

  document.querySelectorAll("[data-ticket-card]").forEach((card) => {
    card.addEventListener("dragstart", (event) => {
      draggedCard = card;
      suppressClick = true;
      card.classList.add("kanban-card-dragging");
      event.dataTransfer.effectAllowed = "move";
      event.dataTransfer.setData("text/plain", card.dataset.ticketId);
    });

    card.addEventListener("dragend", () => {
      card.classList.remove("kanban-card-dragging");
      stopAutoScroll();
      draggedCard = null;
      window.setTimeout(() => {
        suppressClick = false;
      }, 0);
    });

    card.addEventListener("click", () => {
      if (suppressClick) {
        return;
      }

      const detailUrl = card.dataset.detailUrl;
      if (detailUrl) {
        window.location.href = detailUrl;
      }
    });
  });

  document.querySelectorAll("[data-ticket-column]").forEach((column) => {
    column.addEventListener("dragover", (event) => {
      if (!draggedCard) {
        return;
      }

      const targetStatus = column.dataset.status;
      if (!canDropCardInColumn(draggedCard, targetStatus)) {
        return;
      }

      event.preventDefault();
      event.dataTransfer.dropEffect = "move";
      updateAutoScroll(event.clientX);
      column.classList.add("kanban-column-drop");
    });

    column.addEventListener("dragleave", () => {
      column.classList.remove("kanban-column-drop");
    });

    column.addEventListener("drop", (event) => {
      if (!draggedCard) {
        return;
      }

      const targetStatus = column.dataset.status;
      column.classList.remove("kanban-column-drop");

      if (!canDropCardInColumn(draggedCard, targetStatus)) {
        return;
      }

      event.preventDefault();
      stopAutoScroll();
      moveId.value = draggedCard.dataset.ticketId;
      moveStatus.value = targetStatus;
      moveForm.submit();
    });
  });

  board.addEventListener("dragover", (event) => {
    if (!draggedCard) {
      return;
    }

    updateAutoScroll(event.clientX);
  });

  board.addEventListener("drop", () => {
    stopAutoScroll();
  });
})();
