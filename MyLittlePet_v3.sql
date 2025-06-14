Create database My_Little_Pet_V3;
GO
USE My_Little_Pet_V3;
GO


CREATE TABLE [User] (
    ID INT PRIMARY KEY  IDENTITY(1,1),
	Role NVARCHAR(50) NOT NULL,
	UserName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    Password NVARCHAR(100) NOT NULL,
	
	Level INT DEFAULT 1,
	Coin INT,
	Diamond INT DEFAULT 0,
    Gem INT DEFAULT 0,	
    JoinDate DATETIME DEFAULT GETDATE(),
);


GO
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
    Description TEXT,
	FOREIGN KEY (AdminID) REFERENCES [User](ID)
);


CREATE TABLE PlayerPet (
    PlayerPetID INT PRIMARY KEY IDENTITY(1,1),
    PlayerID INT NOT NULL,
    PetID INT NOT NULL,
    PetName VARCHAR(50),             
    AdoptedAt DATETIME DEFAULT GETDATE(),
	UNIQUE(PlayerID, PetName),
    Level INT DEFAULT 0,
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



