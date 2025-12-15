-- Sistema de gestión de tareas (MySQL)
-- Crear bases, tablas, índices, constraints y datos iniciales
-- Fecha: 2025-11-05

-- =============================
-- CREAR BASE DE DATOS
-- =============================
DROP DATABASE IF EXISTS task_manager;
CREATE DATABASE task_manager CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE task_manager;

-- =============================
-- TABLA USUARIOS
-- =============================
CREATE TABLE usuarios (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    email VARCHAR(255) UNIQUE,
    username VARCHAR(100) UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- =============================
-- TABLA TABLEROS
-- =============================
CREATE TABLE tableros (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    descripcion TEXT,
    propietario_id BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (propietario_id) REFERENCES usuarios(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- =============================
-- TABLA COLUMNAS
-- =============================
CREATE TABLE columnas (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    tablero_id BIGINT NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    posicion INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (tablero_id) REFERENCES tableros(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- =============================
-- TABLA TAREAS
-- =============================
CREATE TABLE tareas (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    tablero_id BIGINT NOT NULL,
    columna_id BIGINT NOT NULL,
    titulo VARCHAR(250) NOT NULL,
    descripcion TEXT,
    prioridad ENUM('low','high','prendida_fuego') DEFAULT 'low',
    tiempo_estimado_min INT,
    fecha_inicio DATETIME,
    fecha_fin DATETIME,
    tipo VARCHAR(100),
    creado_por BIGINT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (tablero_id) REFERENCES tableros(id) ON DELETE CASCADE,
    FOREIGN KEY (columna_id) REFERENCES columnas(id) ON DELETE CASCADE,
    FOREIGN KEY (creado_por) REFERENCES usuarios(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- =============================
-- TABLA COMPARTICIÓN DE TABLEROS
-- =============================
CREATE TABLE tablero_compartido (
    tablero_id BIGINT NOT NULL,
    usuario_id BIGINT NOT NULL,
    PRIMARY KEY (tablero_id, usuario_id),
    FOREIGN KEY (tablero_id) REFERENCES tableros(id) ON DELETE CASCADE,
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE
);

-- =============================
-- ÍNDICES
-- =============================
CREATE INDEX idx_tareas_tablero ON tareas(tablero_id);
CREATE INDEX idx_tareas_columna ON tareas(columna_id);
CREATE INDEX idx_tareas_tipo ON tareas(tipo);
CREATE INDEX idx_tareas_prioridad ON tareas(prioridad);
CREATE INDEX idx_tareas_titulo ON tareas(titulo(100));

-- ============================================================
-- SEED: Datos iniciales
-- ============================================================
INSERT INTO usuarios (nombre, email, username, password_hash)
VALUES ('Carlos', 'carlos@example.com', 'carlos123', SHA2('123456',256));
SET @user_id = LAST_INSERT_ID();

INSERT INTO tableros (nombre, descripcion, propietario_id)
VALUES ('Demo Board', 'Tablero demo con columnas por defecto', @user_id);
SET @board_id = LAST_INSERT_ID();

INSERT INTO columnas (tablero_id, nombre, posicion) VALUES
(@board_id, 'backlog', 1),
(@board_id, 'in progress', 2),
(@board_id, 'done', 3);

SET @columna_backlog = (SELECT id FROM columnas WHERE tablero_id = @board_id AND nombre='backlog' LIMIT 1);

INSERT INTO tareas (tablero_id, columna_id, titulo, descripcion, prioridad, tiempo_estimado_min, fecha_inicio, fecha_fin, tipo, creado_por)
VALUES (@board_id, @columna_backlog, 'Tarea ejemplo', 'Esta es una tarea de ejemplo para demo', 'high', 90,
        '2025-11-06 09:00:00', '2025-11-07 18:00:00', 'feature', @user_id);
