Create database My_Little_Pet_V3;
--GO

--CACH CHAY DATABASE 
--1 NEU CO DB CU THI CHAY  "Drop database My_Little_Pet_V3;"
--2 SAU DO NEW MOI DB
/*3 CHAY TU "CREATE TABLE [User]"    --->   "    FROM ShopProduct SP
    INNER JOIN inserted i ON SP.ShopProductID = i.ShopProductID
    WHERE i.ImageUrl LIKE '%drive.google.com/file/d/%';
END"  */
--4 CHAY PHAN CON LAI
select * from ShopProduct
--GO
select * from [User]
Drop database My_Little_Pet_V3;
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
	Position FLOAT,
	EXP INT
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
	PetID INT NULL,
    Name NVARCHAR(100) NOT NULL,
    Type VARCHAR(20) NOT NULL,
    Description NVARCHAR(255),
    ImageUrl NVARCHAR(255),
    Price INT NOT NULL,
    CurrencyType VARCHAR(20) NOT NULL,
    Quantity INT,
	Status INT DEFAULT 1,
    FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),
	FOREIGN KEY (AdminID) REFERENCES [User](ID),
	FOREIGN key (PetID) REFERENCES Pet(PetID)
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
	EXP INT, 
    FOREIGN KEY (PlayerID) REFERENCES [User](ID),
    FOREIGN KEY (PetID) REFERENCES Pet(PetID)
);
--C n playerPet vs ACtivity
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
select * from ShopProduct


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


GO
CREATE TRIGGER trg_UpdateImageUrl
ON ShopProduct
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SP
    SET ImageUrl = 
        'https://drive.google.com/uc?id=' + 
        SUBSTRING(
            i.ImageUrl, 
            CHARINDEX('/d/', i.ImageUrl) + 3, 
            CHARINDEX('/', i.ImageUrl, CHARINDEX('/d/', i.ImageUrl) + 3) - (CHARINDEX('/d/', i.ImageUrl) + 3)
        )
    FROM ShopProduct SP
    INNER JOIN inserted i ON SP.ShopProductID = i.ShopProductID
    WHERE i.ImageUrl LIKE '%drive.google.com/file/d/%';
END



select * from ShopProduct
INSERT INTO [User] (Role, UserName, Email, Password, UserStatus, Level, Coin, Diamond, Gem)
VALUES 
('Player', N'CatLover01', 'catlover01@example.com', 'pass1234', 'ACTIVE', 5, 1000, 5, 3),
('Player', N'DogMaster99', 'dogmaster99@example.com', 'dogpass99', 'ACTIVE', 3, 800, 2, 1),
('Admin', N'AdminPetCare', 'admin@petgame.com', 'adminpass', 'ACTIVE', 10, 5000, 50, 20),
('Player', N'BunnyQueen', 'bunnyq@example.com', 'bunny123', 'BANNED', 2, 300, 0, 0),
('Player', N'HamsterHero', 'hamhero@example.com', 'hamham', 'ACTIVE', 4, 700, 3, 2),
('Admin', N'ModPuppy', 'modpuppy@example.com', 'mod123', 'ACTIVE', 6, 2000, 10, 5),
('Player', N'KittyCraze', 'kittycraze@example.com', 'meowmeow', 'BANNED', 1, 100, 0, 0),
('Player', N'FishyFella', 'fishyfella@example.com', 'fishfish', 'ACTIVE', 3, 600, 1, 1),
('Player', N'BirdWatcher', 'birdw@example.com', 'tweet123', 'ACTIVE', 5, 1200, 6, 4),
('Player', N'ReptileRider', 'reptrider@example.com', 'reptilepass', 'ACTIVE', 7, 1500, 8, 7);
INSERT INTO Shop (Name, Type, Description)
VALUES 
('Pets Shop', 'Pet', 'Pet adoption and animal companions'),
('Item Shop', 'Item', 'Supplies, food and toys for pets');

INSERT INTO Pet (AdminID, PetType, PetDefaultName, Description)
VALUES 
(1, 'Cat', 'Mimi', 'Playful and smart'),
(1, 'Chicken', 'Chicky', 'This Pet have sth you never know'),
(1, 'Fish', 'Bubbles', 'Glowy fins');

INSERT INTO ShopProduct (ShopID, AdminID,PetID, Name, Type, Description, ImageUrl, Price, CurrencyType)
VALUES 
-- Pets Shop (ShopID = 1)
(1, 1,1, 'Mimi', 'Pet', 'Playful and smart', 'https://drive.google.com/file/d/1nkOmQE4OQxJNE_-toGhVN7b0zrQf3L2H/view', 100, 'Coin'),
(1, 2,2, 'Chicky', 'Pet', 'This Pet have sth you never know', 'https://drive.google.com/file/d/1dnKvmkFxuECn9T10cQRB1QKFV0-uy97W/view', 120, 'Coin'),
(1, 1,3, 'Bubbles', 'Pet', 'Glowy fins', 'https://drive.google.com/file/d/1fsJXvABMVtfGSPJz7E-_yhqv0H7Fo8oS/view', 150, 'Coin')

Select * from [User]

UPDATE ShopProduct
SET Type = 'Pet'
WHERE ShopID = 1 AND PetID IS NOT NULL;


-- Item Shop (ShopID = 2)
(2, 1,NULL, 'Cat Food', 'Food', 'Nutritious food for healthy cats', 'https://drive.google.com/file/d/1siQWAMVbrnAqCnpnhbN5luvDEDJSgsmV/view', 30, 'Coin'),
(2, 2,NULL, 'Chicken Food', 'Food', 'Premium grains for chickens', 'https://drive.google.com/file/d/16asRZC5bJStd8OlYVmjuOE1q6IhWlcWp/view', 35, 'Coin'),
(2, 1,NULL, 'Dog Food', 'Food', 'High-protein dog meal', 'https://drive.google.com/file/d/1UFHJK5hW3A5l5UTgZDT5dy8YBfv_0Qvh/view', 40, 'Coin'),
(2, 1,NULL, 'Cookies', 'Food', 'Sweet and crunchy cookies for your pet to enjoy', 'https://drive.google.com/file/d/1BI3P_--YCN0OQvrpeEWVK1l0dbIPFLOt/view', 40, 'Coin'),
(2, 1,NULL, 'Chocolate', 'Food', 'A special chocolate treat (non-toxic for pets)', 'https://drive.google.com/file/d/1eeJ-Tx6ztnARuhZF3rTvThSEb1uR9ScH/view', 40, 'Coin'),
(2, 1,NULL, 'Orange', 'Food', 'Fresh and juicy orange slices full of vitamins', 'https://drive.google.com/file/d/1Mz8_kpl7E1_MAlQ_IF1vdeXpZwEKLK0Y/view', 40, 'Coin'),
(2, 1,NULL, 'Cherry', 'Food', 'Sweet cherries that boost pet energy', 'https://drive.google.com/file/d/1vRZIx7kORpayLeHnxZSpJ1LwIGcsgrx4/view', 40, 'Coin'),
(2, 1,NULL, 'Pear', 'Food', 'Ripe and juicy pears for healthy digestion', 'https://drive.google.com/file/d/1Rm5aSvUASG_KzZMIWWLaSmVOx8f0Ts-8/view', 40, 'Coin'),
(2, 1,NULL, 'Banana', 'Food', 'Soft and sweet bananas loved by all pets', 'https://drive.google.com/file/d/1gJtCKHc1IBc_JRGLd29yaYbaCUETSM8A/view', 40, 'Coin');
INSERT INTO PlayerInventory (PlayerID, ShopProductID, Quantity)
VALUES 
(1, 1, 1),
(1, 3, 2),
(1, 2, 1),
(1, 4, 1),
(1, 5, 3),
(2, 6, 2),
(2, 7, 1),
(2, 8, 1),
(2, 9, 1),
(2, 10, 1);

Select * from [User]

INSERT INTO PlayerPet (PlayerID, PetID, PetCustomName, Status)
VALUES 
(2, 1, 'Whiskers', '100%50%20'),
(2, 2, 'Barker', '50%50%100'),
(1, 1, 'Fluffy', '100%100%100'),
(2, 1, 'Speedy', '20%50%100'),
(4, 2, 'Talky', '100%50%20'),
(1, 3, 'Shell', '50%50%20'),
(2, 1, 'Splash', '20%20%50'),
(5, 2, 'Firetail', '100%20%50'),
(7, 3, 'Bamboo', '50%50%20'),
(10, 2, 'Skyflame', '100%20%50');
INSERT INTO CareActivity (ActivityType, Description)
VALUES 
('Feed', 'Give food to pet'),
('Sleep', 'Put pet to rest'),
('Play', 'Play with your pet');
INSERT INTO CareHistory (PlayerPetID, PlayerID, ActivityID)
VALUES 
(1, 3, 1),
(1, 3, 2),
(2, 3, 3),
(3, 4, 1),
(4, 5, 4),
(5, 6, 2),
(6, 6, 3),
(7, 7, 1),
(8, 7, 2),
(9, 8, 5);
INSERT INTO Achievement (AchievementName, Description)
VALUES
('Welcome Aboard!', 'First login to receive a reward'),
('Pet Collector I', 'Own 2 pets to receive a reward'),
('Pet Collector II', 'Own 4 pets to receive a reward'),
('Pet Collector III', 'Own 6 pets to receive a reward'),
('Wealthy I', 'Own 1,000 coins to receive a reward'),
('Wealthy II', 'Own 5,000 coins to receive a reward'),
('Wealthy III', 'Own 10,000 coins to receive a reward');
INSERT INTO PlayerAchievement (PlayerID, AchievementID)
VALUES 
(1, 1),
(2, 3),
(1, 1),
(4, 4),
(5, 1),
(6, 2),
(6, 5),
(7, 6);
INSERT INTO Minigame (Name, Description)
VALUES 
('Fetch Frenzy', 'Throw and catch game'),
('Food Hunt', 'Find hidden treats'),
('Bath Time', 'Clean pet challenge'),
('Training Rush', 'Quick reaction training'),
('Maze Runner', 'Navigate the maze'),
('Memory Match', 'Match items'),
('Color Catch', 'Catch correct colors'),
('Fly High', 'Flying pet game'),
('Sleepy Time', 'Put pet to sleep fast'),
('Happy Bar', 'Maximize pet happiness');
INSERT INTO GameRecord (PlayerID, MinigameID, Score)
VALUES 
(3, 1, 85),
(3, 2, 90),
(4, 3, 78),
(4, 4, 88),
(5, 5, 60),
(6, 6, 95),
(6, 7, 72),
(7, 8, 100),
(8, 9, 65),
(9, 10, 98);




