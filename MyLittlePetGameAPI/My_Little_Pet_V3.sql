Create database My_Little_Pet_V3;
GO
USE My_Little_Pet_V3;
GO
Drop database My_Little_Pet_V3;
CREATE TABLE [User] (
    ID INT PRIMARY KEY  IDENTITY(1,1),
	Role NVARCHAR(50) NOT NULL,
	UserName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    Password NVARCHAR(100) NOT NULL,
	UserStatus NVARCHAR(20) DEFAULT 'ACTIVE' CHECK (UserStatus IN ('ACTIVE', 'INACTIVE', 'BANNED', 'ONLINE')),  

	Level INT DEFAULT 1,
	Coin INT,
	Diamond INT DEFAULT 0,
    Gem INT DEFAULT 0,	
    JoinDate DATETIME DEFAULT GETDATE(),
);



CREATE TABLE Shop (
    ShopID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Type VARCHAR(10),
    Description NVARCHAR(255)
);
GO
CREATE TABLE ShopProduct (
    ShopProductID INT PRIMARY KEY IDENTITY(1,1),
    ShopID INT NOT NULL,
	AdminID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Type VARCHAR(20) NOT NULL,
    Description NVARCHAR(255),
    ImageUrl NVARCHAR(255),
    Price INT NOT NULL,
    CurrencyType VARCHAR(20) NOT NULL,
    Quality INT DEFAULT 100,
	Status INT DEFAULT 1,
    FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),
	FOREIGN KEY (AdminID) REFERENCES [User](ID)
);


CREATE TABLE PlayerInventory (
    PRIMARY KEY (PlayerID, ShopProductID),
    PlayerID INT NOT NULL,
    ShopProductID INT NOT NULL,
    Quantity INT DEFAULT 1,
    AcquiredAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PlayerID) REFERENCES [User](ID),
    FOREIGN KEY (ShopProductID) REFERENCES ShopProduct(ShopProductID),
);


--Done with Shop and Player 
CREATE TABLE Pet (
    PetID INT PRIMARY KEY IDENTITY(1,1),
	AdminID INT,
    PetType VARCHAR(50) NOT NULL,  
	PetDefaultName VARCHAR(50) NOT NULL,
	PetStatus INT DEFAULt 1,
    Description TEXT,
	FOREIGN KEY (AdminID) REFERENCES [User](ID)
);


CREATE TABLE PlayerPet (
    PlayerPetID INT PRIMARY KEY IDENTITY(1,1),
    PlayerID INT NOT NULL,
    PetID INT NOT NULL,
    PetCustomName VARCHAR(50),             
    AdoptedAt DATETIME DEFAULT GETDATE(),
	UNIQUE(PlayerID, PetCustomName),
    Level INT DEFAULT 1,
	Status NVARCHAR(50),
    LastStatusUpdate DATETIME DEFAULT GETDATE(),
	 
    FOREIGN KEY (PlayerID) REFERENCES [User](ID),
    FOREIGN KEY (PetID) REFERENCES Pet(PetID)
);
--Còn playerPet vs ACtivity
CREATE TABLE CareActivity (
    ActivityID INT PRIMARY KEY IDENTITY(1,1),
    ActivityType VARCHAR(50) NOT NULL,   
    Description TEXT
);

CREATE TABLE CareHistory (
    CareHistoryID INT PRIMARY KEY IDENTITY(1,1),
    PlayerPetID INT NOT NULL,
	PlayerID INT NOT NULL,
    ActivityID INT NOT NULL,           
    PerformedAt DATETIME DEFAULT GETDATE(),
   
    FOREIGN KEY (PlayerPetID) REFERENCES PlayerPet(PlayerPetID),
    FOREIGN KEY (ActivityID) REFERENCES CareActivity(ActivityID),
	FOREIGN KEY (PlayerID) REFERENCES [User](ID)
 
);


--Player vs Achievement
CREATE TABLE Achievement (
    AchievementID INT PRIMARY KEY IDENTITY,
    AchievementName VARCHAR(100) NOT NULL,
    Description TEXT
);
CREATE TABLE PlayerAchievement (
    PlayerID INT,
    AchievementID INT,
    EarnedAt DATETIME DEFAULT GETDATE(),
	IsCollected BIT DEFAULT 0,
    PRIMARY KEY (PlayerID, AchievementID),
    FOREIGN KEY (PlayerID) REFERENCES [User](ID),
    FOREIGN KEY (AchievementID) REFERENCES Achievement(AchievementID)
);



--Player vs Minigame
CREATE TABLE Minigame (
    MinigameID INT PRIMARY KEY IDENTITY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT
);

CREATE TABLE GameRecord (
    PlayerID INT,
    MinigameID INT,
    PlayedAt DATETIME DEFAULT GETDATE(),
    Score INT,
    PRIMARY KEY (PlayerID, MinigameID),
    FOREIGN KEY (PlayerID) REFERENCES  [User](ID),
    FOREIGN KEY (MinigameID) REFERENCES Minigame(MinigameID)
);

INSERT INTO [User] (Role, UserName, Email, Password, UserStatus, Level, Coin, Diamond, Gem)
VALUES 
('Player', N'CatLover01', 'catlover01@example.com', 'pass1234', 'ACTIVE', 5, 1000, 5, 3),
('Player', N'DogMaster99', 'dogmaster99@example.com', 'dogpass99', 'ONLINE', 3, 800, 2, 1),
('Admin', N'AdminPetCare', 'admin@petgame.com', 'adminpass', 'ACTIVE', 10, 5000, 50, 20),
('Player', N'BunnyQueen', 'bunnyq@example.com', 'bunny123', 'INACTIVE', 2, 300, 0, 0),
('Player', N'HamsterHero', 'hamhero@example.com', 'hamham', 'ACTIVE', 4, 700, 3, 2),
('Moderator', N'ModPuppy', 'modpuppy@example.com', 'mod123', 'ACTIVE', 6, 2000, 10, 5),
('Player', N'KittyCraze', 'kittycraze@example.com', 'meowmeow', 'BANNED', 1, 100, 0, 0),
('Player', N'FishyFella', 'fishyfella@example.com', 'fishfish', 'ACTIVE', 3, 600, 1, 1),
('Player', N'BirdWatcher', 'birdw@example.com', 'tweet123', 'ONLINE', 5, 1200, 6, 4),
('Player', N'ReptileRider', 'reptrider@example.com', 'reptilepass', 'ACTIVE', 7, 1500, 8, 7);

