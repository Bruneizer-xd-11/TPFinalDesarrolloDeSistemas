document.addEventListener('DOMContentLoaded', () => {
  let dragged = null;

  document.querySelectorAll('.task-item').forEach(item => {
    item.addEventListener('dragstart', (e) => {
      dragged = item;
      e.dataTransfer.effectAllowed = 'move';
    });
  });

  document.querySelectorAll('.task-list').forEach(list => {
    list.addEventListener('dragover', (e) => {
      e.preventDefault();
      e.dataTransfer.dropEffect = 'move';
    });

    list.addEventListener('drop', async (e) => {
      e.preventDefault();
      if (!dragged) return;
      // agregar al DOM
      list.appendChild(dragged);
      const tareaId = dragged.getAttribute('data-tareaid');
      const nuevaColumnaId = list.getAttribute('data-columnaid');

      // Llamada al endpoint para mover tarea
      try {
        const resp = await fetch(`/api/tareas/${tareaId}/mover`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ nuevaColumnaId: parseInt(nuevaColumnaId) })
        });
        if (!resp.ok) {
          alert('Error al mover la tarea en el servidor');
          // revertir UI si querés
        }
      } catch (err) {
        console.error(err);
        alert('Error de red al mover la tarea');
      }
    });
  });
});
