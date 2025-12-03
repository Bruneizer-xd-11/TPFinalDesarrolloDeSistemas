USE task_manager;

-- Procedimientos almacenados útiles (simplificados)
-- IMPORTANTE: ajustes finales (validaciones, control de errores) pueden añadirse según necesidad.

DELIMITER $$

-- 1) Obtener todas las tareas (GET /api/tareas)
CREATE PROCEDURE sp_get_tareas()
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    LEFT JOIN columnas c ON t.columna_id=c.id
    LEFT JOIN tableros b ON t.tablero_id=b.id
    ORDER BY t.created_at DESC;
END$$


-- 2) Obtener tarea por id (GET /api/tareas/{id})
CREATE PROCEDURE sp_get_tarea_por_id(IN p_id BIGINT)
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    LEFT JOIN columnas c ON t.columna_id=c.id
    LEFT JOIN tableros b ON t.tablero_id=b.id
    WHERE t.id=p_id;
END$$


-- 3) Crear tarea (POST /api/tareas)
CREATE PROCEDURE sp_crear_tarea(
    IN p_tablero_id BIGINT,
    IN p_columna_id BIGINT,
    IN p_titulo VARCHAR(250),
    IN p_descripcion TEXT,
    IN p_prioridad VARCHAR(20),
    IN p_tiempo_estimado_min INT,
    IN p_fecha_inicio DATETIME,
    IN p_fecha_fin DATETIME,
    IN p_tipo VARCHAR(100),
    IN p_creado_por BIGINT
)
BEGIN
    INSERT INTO tareas(tablero_id,columna_id,titulo,descripcion,prioridad,tiempo_estimado_min,
                       fecha_inicio,fecha_fin,tipo,creado_por)
    VALUES(p_tablero_id,p_columna_id,p_titulo,p_descripcion,p_prioridad,p_tiempo_estimado_min,
           p_fecha_inicio,p_fecha_fin,p_tipo,p_creado_por);
    SELECT LAST_INSERT_ID() AS nueva_tarea_id;
END$$


-- 4) Actualizar tarea (PUT /api/tareas/{id})
CREATE PROCEDURE sp_actualizar_tarea(
    IN p_id BIGINT,
    IN p_tablero_id BIGINT,
    IN p_columna_id BIGINT,
    IN p_titulo VARCHAR(250),
    IN p_descripcion TEXT,
    IN p_prioridad VARCHAR(20),
    IN p_tiempo_estimado_min INT,
    IN p_fecha_inicio DATETIME,
    IN p_fecha_fin DATETIME,
    IN p_tipo VARCHAR(100),
    IN p_creado_por BIGINT
)
BEGIN
    UPDATE tareas SET
        tablero_id=p_tablero_id,
        columna_id=p_columna_id,
        titulo=p_titulo,
        descripcion=p_descripcion,
        prioridad=p_prioridad,
        tiempo_estimado_min=p_tiempo_estimado_min,
        fecha_inicio=p_fecha_inicio,
        fecha_fin=p_fecha_fin,
        tipo=p_tipo,
        creado_por=p_creado_por,
        updated_at=CURRENT_TIMESTAMP
    WHERE id=p_id;
    SELECT ROW_COUNT() AS filas_actualizadas;
END$$


-- 5) Eliminar tarea (DELETE /api/tareas/{id})
CREATE PROCEDURE sp_eliminar_tarea(IN p_id BIGINT)
BEGIN
    DELETE FROM tareas WHERE id=p_id;
    SELECT ROW_COUNT() AS filas_eliminadas;
END$$



-- 6) Obtener tareas por columna/estado (GET /api/tareas/estado/{estado})
-- Aquí 'estado' lo mapeamos a nombre de columna (ej: backlog, in progress, done)
CREATE PROCEDURE sp_get_tareas_por_estado(IN p_nombre_columna VARCHAR(100))
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    JOIN columnas c ON t.columna_id=c.id
    JOIN tableros b ON t.tablero_id=b.id
    WHERE LOWER(c.nombre)=LOWER(p_nombre_columna)
    ORDER BY t.created_at DESC;
END$$


-- 7) Obtener tareas por prioridad (GET /api/tareas/prioridad/{prioridad})
CREATE PROCEDURE sp_get_tareas_por_prioridad(IN p_prioridad VARCHAR(20))
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    LEFT JOIN columnas c ON t.columna_id=c.id
    LEFT JOIN tableros b ON t.tablero_id=b.id
    WHERE t.prioridad=p_prioridad
    ORDER BY FIELD(t.prioridad,'prendida_fuego','high','low'),t.created_at DESC;
END$$

CREATE PROCEDURE sp_registrar_usuario(
    IN p_nombre VARCHAR(150),
    IN p_email VARCHAR(255),
    IN p_username VARCHAR(100),
    IN p_password_hash VARCHAR(255)
)
BEGIN
    IF EXISTS(SELECT 1 FROM usuarios WHERE email=p_email OR username=p_username) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='El usuario o email ya existe';
    ELSE
        INSERT INTO usuarios(nombre,email,username,password_hash)
        VALUES(p_nombre,p_email,p_username,p_password_hash);
        SELECT LAST_INSERT_ID() AS nuevo_usuario_id;
    END IF;
END$$
-- 8) Obtener tareas de un usuario (GET /usuarios/{id}/tareas)
CREATE PROCEDURE sp_get_tareas_de_usuario(IN p_usuario_id BIGINT)
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    LEFT JOIN columnas c ON t.columna_id=c.id
    LEFT JOIN tableros b ON t.tablero_id=b.id
    WHERE t.creado_por = p_usuario_id
    ORDER BY t.created_at DESC;
END$$

-- 9) Obtener usuario por id
CREATE PROCEDURE sp_get_usuario_por_id(IN p_id BIGINT)
BEGIN
    SELECT u.* FROM usuarios u WHERE u.id = p_id LIMIT 1;
END$$

-- 10) Obtener usuario por username
CREATE PROCEDURE sp_get_usuario_por_username(IN p_username VARCHAR(100))
BEGIN
    SELECT u.* FROM usuarios u WHERE u.username = p_username LIMIT 1;
END$$

-- 11) Mover tarea a otra columna (actualiza columna_id)
CREATE PROCEDURE sp_mover_tarea_columna(IN p_tarea_id BIGINT, IN p_nueva_columna_id BIGINT)
BEGIN
    UPDATE tareas SET columna_id = p_nueva_columna_id, updated_at = CURRENT_TIMESTAMP WHERE id = p_tarea_id;
    SELECT ROW_COUNT() AS filas_actualizadas;
END$$

DELIMITER ;
