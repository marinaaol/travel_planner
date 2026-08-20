-- Estrutura inicial da base de dados do projeto VOYAGE.

CREATE DATABASE IF NOT EXISTS voyage_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE voyage_db;

CREATE TABLE IF NOT EXISTS usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR (100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    senha_hash VARCHAR(255) NOT NULL,
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS roteiros (
    roteiro_id INT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(250) NOT NULL,
    destino VARCHAR(250) NOT NULL,
    data_inicio DATE NOT NULL,
    data_fim DATE NOT NULL,
    usuario_id INT NOT NULL,

    CONSTRAINT fk_roteiros_usuarios
        FOREIGN KEY (usuario_id)
        REFERENCES usuarios(id)
        ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS atividades (
    atividade_id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    tipo VARCHAR(150) NOT NULL,
    valor DECIMAL(10,2),
    data_atividade DATE NOT NULL,
    hora TIME,
    roteiro_id INT NOT NULL,

    CONSTRAINT fk_atividades_roteiros
    FOREIGN KEY (roteiro_id)
    REFERENCES roteiros(roteiro_id)
    ON DELETE CASCADE
) ENGINE=InnoDB;


