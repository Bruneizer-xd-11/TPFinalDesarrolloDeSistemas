document.querySelectorAll(".tarea").forEach(t => {
    t.addEventListener("dragstart", e => {
        e.dataTransfer.setData("tareaId", t.dataset.tareaId);
    });
});

document.querySelectorAll(".columna").forEach(c => {
    c.addEventListener("dragover", e => e.preventDefault());

    c.addEventListener("drop", async e => {
        e.preventDefault();

        const tareaId = e.dataTransfer.getData("tareaId");
        const columnaId = c.dataset.columnaId;

        await fetch(`/api/tareas/${tareaId}/mover?nuevaColumnaId=${columnaId}`, {
            method: "POST"
        });

        location.reload();
    });
});
