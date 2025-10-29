-- Crear base de datos y usarla
CREATE DATABASE IF NOT EXISTS TPFinal;
USE TPFinal;

-- 1️⃣ Usuarios
CREATE TABLE usuarios (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  nombre_usuario VARCHAR(100) NOT NULL UNIQUE,
  nombre_completo VARCHAR(200),
  correo VARCHAR(200) UNIQUE,
  contrasena VARCHAR(200) NOT NULL,
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 2️⃣ Tableros
CREATE TABLE tableros (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(150) NOT NULL,
  descripcion TEXT,
  creado_por INT UNSIGNED NOT NULL,
  creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (creado_por) REFERENCES usuarios(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 3️⃣ Usuarios_Tableros (permite compartir tableros con otros usuarios)
-- Relación muchos a muchos entre usuarios y tableros
CREATE TABLE usuarios_tableros (
  id_usuario INT UNSIGNED NOT NULL,
  id_tablero INT UNSIGNED NOT NULL,
  rol ENUM('propietario','colaborador') DEFAULT 'colaborador',
  PRIMARY KEY (id_usuario, id_tablero),
  FOREIGN KEY (id_usuario) REFERENCES usuarios(id) ON DELETE CASCADE,
  FOREIGN KEY (id_tablero) REFERENCES tableros(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 4️⃣ Tareas
CREATE TABLE tareas (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  id_tablero INT UNSIGNED NOT NULL,
  titulo VARCHAR(200) NOT NULL,
  descripcion TEXT,
  prioridad ENUM('baja','media','alta') DEFAULT 'baja',
  estado ENUM('pendiente','en_progreso','completada') DEFAULT 'pendiente',
  fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  fecha_vencimiento DATE,
  FOREIGN KEY (id_tablero) REFERENCES tableros(id) ON DELETE CASCADE
) ENGINE=InnoDB;
