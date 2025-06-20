CREATE DATABASE QuanLiBanHangTapHoa;

USE QuanLiBanHangTapHoa;

CREATE TABLE Users (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL
);

-- Thêm dữ liệu mẫu
INSERT INTO Users (Email, Password) VALUES ('admin@gmail.com', '123456');