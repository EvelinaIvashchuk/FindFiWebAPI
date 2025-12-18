CREATE DATABASE IF NOT EXISTS rent_core;
USE rent_core;

CREATE TABLE IF NOT EXISTS Product (
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(200) NOT NULL,
  Price DECIMAL(18,2) NOT NULL
);

-- Початкові дані (опціонально)
INSERT INTO Product (Name, Price) VALUES ('Квартира в центрі', 15000.00);
INSERT INTO Product (Name, Price) VALUES ('Будинок біля озера', 25000.00);
