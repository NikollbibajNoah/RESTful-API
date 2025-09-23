-- ***** Database Schema ***** --

-- Create non existing database
CREATE DATABASE IF NOT EXISTS RESTful
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;

USE RESTful;

-- ***** Tables ***** --

-- Users Table --
CREATE TABLE Users(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE, -- No equals emails allowed --
    Position VARCHAR(100) NOT NULL
)