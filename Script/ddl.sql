-- schema.sql
-- Sistema de gestión de tareas (MySQL)
-- Crear bases, tablas, índices, constraints y procedimientos almacenados
-- Fecha: 2025-11-05

-- Usar/crear la base de datos
CREATE DATABASE IF NOT EXISTS task_manager CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE task_manager;

-- Tabla: usuarios (personas que se asignan a tareas)
CREATE TABLE IF NOT EXISTS usuarios (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    email VARCHAR(255) UNIQUE,
    username VARCHAR(100) UNIQUE AFTER email,
    password_hash VARCHAR(255) NOT NULL AFTER username,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Tabla: tableros
CREATE TABLE IF NOT EXISTS tableros (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    descripcion TEXT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Tabla: columnas (cada tablero puede definir sus columnas; mínimo: backlog, in progress, done)
CREATE TABLE IF NOT EXISTS columnas (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    tablero_id BIGINT NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    posicion INT NOT NULL DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (tablero_id) REFERENCES tableros(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Tabla: tareas
CREATE TABLE IF NOT EXISTS tareas (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    tablero_id BIGINT NOT NULL,
    columna_id BIGINT NOT NULL,
    titulo VARCHAR(250) NOT NULL,
    descripcion TEXT NULL,
    -- urgencia/ prioridad: low, high, prendida_fuego (para ayer)
    prioridad ENUM('low','high','prendida_fuego') NOT NULL DEFAULT 'low',
    -- tiempo estimado en minutos (puede ser NULL)
    tiempo_estimado_min INT NULL,
    -- para Gantt: fechas de inicio y fin
    fecha_inicio DATETIME NULL,
    fecha_fin DATETIME NULL,
    -- campo adicional: tipo (puede usarse para buscar por tipo)
    tipo VARCHAR(100) NULL,
    creado_por BIGINT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (tablero_id) REFERENCES tableros(id) ON DELETE CASCADE,
    FOREIGN KEY (columna_id) REFERENCES columnas(id) ON DELETE CASCADE,
    FOREIGN KEY (creado_por) REFERENCES usuarios(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- -- Tabla intermedia: asignaciones de tarea a usuario (una tarea puede tener varias personas)
-- CREATE TABLE IF NOT EXISTS tarea_asignaciones (
--     tarea_id BIGINT NOT NULL,
--     usuario_id BIGINT NOT NULL,
--     asignado_desde TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
--     PRIMARY KEY (tarea_id, usuario_id),
--     FOREIGN KEY (tarea_id) REFERENCES tareas(id) ON DELETE CASCADE,
--     FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE
-- ) ENGINE=InnoDB;

-- Índices para búsquedas rápidas
CREATE INDEX idx_tareas_tablero ON tareas(tablero_id);
CREATE INDEX idx_tareas_columna ON tareas(columna_id);
CREATE INDEX idx_tareas_tipo ON tareas(tipo);
CREATE INDEX idx_tareas_prioridad ON tareas(prioridad);
CREATE INDEX idx_tareas_titulo ON tareas(titulo(100)); -- prefijo para acelerar búsquedas por nombre

-- Datos iniciales de ejemplo: columnas por defecto (backlog, in progress, done)
-- Nota: se insertan como ejemplo en un tablero demo que creamos.
INSERT INTO tableros (nombre, descripcion) VALUES ('Demo Board', 'Tablero demo con columnas por defecto');
SET @board_id = LAST_INSERT_ID();

INSERT INTO columnas (tablero_id, nombre, posicion) VALUES
(@board_id, 'backlog', 1),
(@board_id, 'in progress', 2),
(@board_id, 'done', 3);

-- Procedimientos almacenados útiles (simplificados)
-- IMPORTANTE: ajustes finales (validaciones, control de errores) pueden añadirse según necesidad.


--Stored PROCEDURE

DELIMITER $$

-- 1) Obtener todas las tareas (GET /api/tareas)
CREATE PROCEDURE sp_get_tareas()
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    LEFT JOIN columnas c ON t.columna_id = c.id
    LEFT JOIN tableros b ON t.tablero_id = b.id
    ORDER BY t.created_at DESC;
END$$

-- 2) Obtener tarea por id (GET /api/tareas/{id})
CREATE PROCEDURE sp_get_tarea_por_id(IN p_id BIGINT)
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    LEFT JOIN columnas c ON t.columna_id = c.id
    LEFT JOIN tableros b ON t.tablero_id = b.id
    WHERE t.id = p_id;
END$$

-- 3) Crear tarea (POST /api/tareas)
CREATE PROCEDURE sp_crear_tarea(
    IN p_tablero_id BIGINT,
    IN p_columna_id BIGINT,
    IN p_titulo VARCHAR(250),
    IN p_descripcion TEXT,
    IN p_prioridad ENUM('low','high','prendida_fuego'),
    IN p_tiempo_estimado_min INT,
    IN p_fecha_inicio DATETIME,
    IN p_fecha_fin DATETIME,
    IN p_tipo VARCHAR(100),
    IN p_creado_por BIGINT
)
BEGIN
    INSERT INTO tareas (
        tablero_id, columna_id, titulo, descripcion, prioridad, tiempo_estimado_min,
        fecha_inicio, fecha_fin, tipo, creado_por
    ) VALUES (
        p_tablero_id, p_columna_id, p_titulo, p_descripcion, p_prioridad, p_tiempo_estimado_min,
        p_fecha_inicio, p_fecha_fin, p_tipo, p_creado_por
    );
    SELECT LAST_INSERT_ID() AS nueva_tarea_id;
END$$

-- 4) Actualizar tarea (PUT /api/tareas/{id})
CREATE PROCEDURE sp_actualizar_tarea(
    IN p_id BIGINT,
    IN p_tablero_id BIGINT,
    IN p_columna_id BIGINT,
    IN p_titulo VARCHAR(250),
    IN p_descripcion TEXT,
    IN p_prioridad ENUM('low','high','prendida_fuego'),
    IN p_tiempo_estimado_min INT,
    IN p_fecha_inicio DATETIME,
    IN p_fecha_fin DATETIME,
    IN p_tipo VARCHAR(100),
    IN p_creado_por BIGINT
)
BEGIN
    UPDATE tareas
    SET tablero_id = p_tablero_id,
        columna_id = p_columna_id,
        titulo = p_titulo,
        descripcion = p_descripcion,
        prioridad = p_prioridad,
        tiempo_estimado_min = p_tiempo_estimado_min,
        fecha_inicio = p_fecha_inicio,
        fecha_fin = p_fecha_fin,
        tipo = p_tipo,
        creado_por = p_creado_por,
        updated_at = CURRENT_TIMESTAMP
    WHERE id = p_id;
    SELECT ROW_COUNT() AS filas_actualizadas;
END$$

-- 5) Eliminar tarea (DELETE /api/tareas/{id})
CREATE PROCEDURE sp_eliminar_tarea(IN p_id BIGINT)
BEGIN
    DELETE FROM tareas WHERE id = p_id;
    SELECT ROW_COUNT() AS filas_eliminadas;
END$$

-- 6) Obtener tareas por columna/estado (GET /api/tareas/estado/{estado})
-- Aquí 'estado' lo mapeamos a nombre de columna (ej: backlog, in progress, done)
CREATE PROCEDURE sp_get_tareas_por_estado(IN p_nombre_columna VARCHAR(100))
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    JOIN columnas c ON t.columna_id = c.id
    JOIN tableros b ON t.tablero_id = b.id
    WHERE LOWER(c.nombre) = LOWER(p_nombre_columna)
    ORDER BY t.created_at DESC;
END$$

-- 7) Obtener tareas por prioridad (GET /api/tareas/prioridad/{prioridad})
CREATE PROCEDURE sp_get_tareas_por_prioridad(IN p_prioridad ENUM('low','high','prendida_fuego'))
BEGIN
    SELECT t.*, c.nombre AS columna_nombre, b.nombre AS tablero_nombre
    FROM tareas t
    LEFT JOIN columnas c ON t.columna_id = c.id
    LEFT JOIN tableros b ON t.tablero_id = b.id
    WHERE t.prioridad = p_prioridad
    ORDER BY FIELD(t.prioridad, 'prendida_fuego','high','low'), t.created_at DESC;
END$$

DELIMITER ;

-- Ejemplos de uso (queries que puede llamar el backend):
-- CALL sp_get_tareas();
-- CALL sp_get_tarea_por_id(1);
-- CALL sp_crear_tarea(1, 1, 'Mi tarea', 'Descripción', 'high', 120, '2025-11-01 09:00:00','2025-11-03 17:00:00', 'feature', NULL);
-- CALL sp_actualizar_tarea(1,1,2,'Titulo nuevo','Desc nueva','low',null,null,null,'bug',NULL);
-- CALL sp_eliminar_tarea(5);
-- CALL sp_get_tareas_por_estado('backlog');
-- CALL sp_get_tareas_por_prioridad('prendida_fuego');

-- Datos de ejemplo extra: crear un usuario y una tarea ejemplo
INSERT INTO usuarios (nombre, email) VALUES ('Carlos', 'carlos@example.com');
SET @usuario_example_id = LAST_INSERT_ID();

-- Crear otra tarea de ejemplo en el tablero demo y columna backlog
SET @columna_backlog = (SELECT id FROM columnas WHERE tablero_id = @board_id AND nombre = 'backlog' LIMIT 1);
INSERT INTO tareas (tablero_id, columna_id, titulo, descripcion, prioridad, tiempo_estimado_min, fecha_inicio, fecha_fin, tipo, creado_por)
VALUES (@board_id, @columna_backlog, 'Tarea ejemplo', 'Esta es una tarea de ejemplo para demo', 'high', 90, '2025-11-06 09:00:00', '2025-11-07 18:00:00', 'feature', @usuario_example_id);

-- Fin del script
DELIMITER $$

CREATE PROCEDURE sp_registrar_usuario(
    IN p_nombre VARCHAR(150),
    IN p_email VARCHAR(255),
    IN p_username VARCHAR(100),
    IN p_password_hash VARCHAR(255)
)
BEGIN
    IF EXISTS (SELECT 1 FROM usuarios WHERE email = p_email OR username = p_username) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El usuario o email ya existe';
    ELSE
        INSERT INTO usuarios (nombre, email, username, password_hash)
        VALUES (p_nombre, p_email, p_username, p_password_hash);
        SELECT LAST_INSERT_ID() AS nuevo_usuario_id;
    END IF;
END$$
DELIMITER ;