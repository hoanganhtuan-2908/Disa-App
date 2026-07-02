Create Database TaskDb;
use	TaskDb	;
Go
CREATE TABLE MissionTemplates
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    Name NVARCHAR(200) NOT NULL,

    Description NVARCHAR(MAX),

    Type INT NOT NULL,

    RewardXP INT NOT NULL DEFAULT 0,

    RewardCoins INT NOT NULL DEFAULT 0,

    RequiresPhoto BIT NOT NULL DEFAULT 0,

    RequiresVideo BIT NOT NULL DEFAULT 0,

    RequiresLocation BIT NOT NULL DEFAULT 0,

    MinVideoSeconds INT NULL,

    IsActive BIT NOT NULL DEFAULT 1
);
CREATE TABLE UserMissions
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    UserId UNIQUEIDENTIFIER NOT NULL,

    TripId UNIQUEIDENTIFIER NOT NULL,

    PlaceId UNIQUEIDENTIFIER NOT NULL,

    TemplateId UNIQUEIDENTIFIER NOT NULL,

    Title NVARCHAR(255) NOT NULL,

    Status INT NOT NULL DEFAULT 0,

    RewardXP INT NOT NULL DEFAULT 0,

    RewardCoins INT NOT NULL DEFAULT 0,

    StartAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    ExpiredAt DATETIME2,

    CompletedAt DATETIME2,

    CONSTRAINT FK_UserMission_Template
        FOREIGN KEY(TemplateId)
        REFERENCES MissionTemplates(Id)
);
CREATE TABLE MissionSubmissions
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    UserMissionId UNIQUEIDENTIFIER NOT NULL,

    SubmittedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    VerificationStatus INT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Submission_UserMission
        FOREIGN KEY(UserMissionId)
        REFERENCES UserMissions(Id)
);
CREATE TABLE MissionEvidences
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    SubmissionId UNIQUEIDENTIFIER NOT NULL,

    Type INT NOT NULL,

    MediaUrl NVARCHAR(500) NOT NULL,

    Latitude FLOAT,

    Longitude FLOAT,

    TakenAt DATETIME2,

    CONSTRAINT FK_Evidence_Submission
        FOREIGN KEY(SubmissionId)
        REFERENCES MissionSubmissions(Id)
);
CREATE TABLE UserRewards
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    UserId UNIQUEIDENTIFIER NOT NULL,

    UserMissionId UNIQUEIDENTIFIER NOT NULL,

    XP INT NOT NULL,

    Coins INT NOT NULL,

    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),

    CONSTRAINT FK_UserReward_UserMission
        FOREIGN KEY(UserMissionId)
        REFERENCES UserMissions(Id)
);
CREATE TABLE UserProgress
(
    UserId UNIQUEIDENTIFIER PRIMARY KEY,

    TotalXP INT DEFAULT 0,

    Coins INT DEFAULT 0,

    CurrentLevel INT DEFAULT 1,

    CurrentStreak INT DEFAULT 0
);
CREATE TABLE Badges
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    Name NVARCHAR(200),

    Description NVARCHAR(MAX),

    IconUrl NVARCHAR(500)
);
CREATE TABLE UserBadges
(
    UserId UNIQUEIDENTIFIER NOT NULL,

    BadgeId UNIQUEIDENTIFIER NOT NULL,

    EarnedAt DATETIME2 DEFAULT GETUTCDATE(),

    PRIMARY KEY(UserId, BadgeId),

    CONSTRAINT FK_UserBadge_Badge
        FOREIGN KEY(BadgeId)
        REFERENCES Badges(Id)
);
Select * from UserMissions