
CREATE DATABASE IAMDb;
GO

USE IAMDb;
GO

-- =========================
-- USERS
-- =========================
CREATE TABLE Users
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),

    Username NVARCHAR(100) NOT NULL UNIQUE,

    Email NVARCHAR(255) NOT NULL UNIQUE,

    PasswordHash NVARCHAR(MAX) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    UpdatedAt DATETIME2 NULL
);
GO

-- =========================
-- ROLES
-- =========================
CREATE TABLE Roles
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),

    Name NVARCHAR(50) NOT NULL UNIQUE,

    Description NVARCHAR(255) NULL
);
GO

-- =========================
-- USER ROLES
-- =========================
CREATE TABLE UserRoles
(
    UserId UNIQUEIDENTIFIER NOT NULL,

    RoleId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_UserRoles
        PRIMARY KEY (UserId, RoleId),

    CONSTRAINT FK_UserRoles_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_UserRoles_Roles
        FOREIGN KEY (RoleId)
        REFERENCES Roles(Id)
        ON DELETE CASCADE
);
GO

-- =========================
-- REFRESH TOKENS
-- =========================
CREATE TABLE RefreshTokens
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),

    UserId UNIQUEIDENTIFIER NOT NULL,

    Token NVARCHAR(MAX) NOT NULL,

    ExpiresAt DATETIME2 NOT NULL,

    RevokedAt DATETIME2 NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_RefreshTokens_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE
);
GO

-- =========================
-- INDEXES
-- =========================
CREATE INDEX IX_Users_Email
ON Users(Email);

CREATE INDEX IX_Users_Username
ON Users(Username);

CREATE INDEX IX_RefreshTokens_UserId
ON RefreshTokens(UserId);
GO

-- =========================
-- SEED ROLES
-- =========================
INSERT INTO Roles (Name, Description)
VALUES
('Admin', 'System Administrator'),
('Manager', 'Business Manager'),
('Customer', 'Customer User');
GO

-- =========================
-- XEM DỮ LIỆU
-- =========================
SELECT * FROM Roles;
GO