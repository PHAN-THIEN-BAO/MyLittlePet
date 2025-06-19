Create database My_Little_Pet_V3;
GO
	
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
('Admin', 'Admin01', 'admin01@example.com', 'pass123', 'ACTIVE', 10, 1000, 50, 10),
('Admin', 'Admin02', 'admin02@example.com', 'pass123', 'ACTIVE', 10, 800, 40, 5),
('Player', 'Player01', 'player01@example.com', 'pass123', 'ACTIVE', 5, 300, 10, 3),
('Player', 'Player02', 'player02@example.com', 'pass123', 'ACTIVE', 3, 200, 5, 1),
('Player', 'Player03', 'player03@example.com', 'pass123', 'BANNED', 2, 100, 2, 0),
('Player', 'Player04', 'player04@example.com', 'pass123', 'ACTIVE', 4, 250, 6, 2),
('Player', 'Player05', 'player05@example.com', 'pass123', 'ACTIVE', 1, 50, 1, 0),
('Player', 'Player06', 'player06@example.com', 'pass123', 'ACTIVE', 6, 400, 12, 4),
('Player', 'Player07', 'player07@example.com', 'pass123', 'ACTIVE', 7, 500, 15, 6),
('Player', 'Player08', 'player08@example.com', 'pass123', 'BANNED', 1, 20, 0, 0);
INSERT INTO Shop (Name, Type, Description)
VALUES 
('Main Shop', 'Pet', 'Pet adoption and supplies'),
('Toy Store', 'Item', 'Fun toys for pets'),
('Food Mart', 'Item', 'Healthy pet food'),
('Clothing Shop', 'Item', 'Outfits for pets'),
('Medical Shop', 'Item', 'Medicine and care items'),
('Accessory Hut', 'Item', 'Decor and accessories'),
('Training Shop', 'Item', 'Skill boosting items'),
('Magic Shop', 'Item', 'Mystical items for pets'),
('Event Shop', 'Event', 'Limited-time items'),
('Premium Shop', 'Premium', 'Premium currency only');
INSERT INTO ShopProduct (ShopID, AdminID, Name, Type, Description, ImageUrl, Price, CurrencyType)
VALUES 
(1, 1, 'Basic Cat', 'Pet', 'A common cat', 'img/cat.png', 100, 'Coin'),
(1, 2, 'Loyal Dog', 'Pet', 'Friendly dog', 'img/dog.png', 120, 'Coin'),
(2, 1, 'Rubber Ball', 'Toy', 'Pet toy ball', 'img/ball.png', 30, 'Coin'),
(2, 2, 'Chew Toy', 'Toy', 'For dogs', 'img/chew.png', 40, 'Coin'),
(3, 1, 'Cat Food', 'Food', 'Delicious food for cats', 'img/catfood.png', 20, 'Coin'),
(3, 2, 'Dog Biscuit', 'Food', 'Crunchy biscuit', 'img/biscuit.png', 25, 'Coin'),
(4, 1, 'Pet Hat', 'Clothing', 'Cute hat', 'img/hat.png', 60, 'Gem'),
(5, 1, 'Pet Medicine', 'Medical', 'Health recovery', 'img/med.png', 70, 'Coin'),
(6, 2, 'Pet Collar', 'Accessory', 'Stylish collar', 'img/collar.png', 50, 'Coin'),
(10, 1, 'Golden Treat', 'Premium', 'Special treat', 'img/treat.png', 100, 'Diamond');
INSERT INTO PlayerInventory (PlayerID, ShopProductID, Quantity)
VALUES 
(3, 1, 1),
(3, 3, 2),
(4, 2, 1),
(4, 4, 1),
(5, 5, 3),
(6, 6, 2),
(6, 7, 1),
(7, 8, 1),
(8, 9, 1),
(9, 10, 1);
INSERT INTO Pet (AdminID, PetType, PetDefaultName, Description)
VALUES 
(1, 'Cat', 'Mimi', 'Playful and smart'),
(2, 'Dog', 'Rex', 'Loyal and brave'),
(1, 'Rabbit', 'Bunny', 'Cute and fast'),
(2, 'Hamster', 'Hamy', 'Tiny and adorable'),
(1, 'Parrot', 'Polly', 'Talkative and colorful'),
(2, 'Turtle', 'Slowmo', 'Slow but wise'),
(1, 'Fish', 'Bubbles', 'Glowy fins'),
(2, 'Fox', 'Flame', 'Mischievous'),
(1, 'Panda', 'Pandy', 'Loves bamboo'),
(2, 'Dragon', 'Draco', 'Rare and powerful');
INSERT INTO PlayerPet (PlayerID, PetID, PetCustomName, Status)
VALUES 
(3, 1, 'Whiskers', '100%50%20'),
(3, 2, 'Barker', '50%50%100'),
(4, 3, 'Fluffy', '100%100%100'),
(5, 4, 'Speedy', '20%50%100'),
(6, 5, 'Talky', '100%50%20'),
(6, 6, 'Shell', '50%50%20'),
(7, 7, 'Splash', '20%20%50'),
(7, 8, 'Firetail', '100%20%50'),
(8, 9, 'Bamboo', '50%50%20'),
(9, 10, 'Skyflame', '100%20%50');
INSERT INTO CareActivity (ActivityType, Description)
VALUES 
('Feed', 'Give food to pet'),
('Play', 'Play with your pet'),
('Sleep', 'Put pet to rest'),
('Bath', 'Clean your pet'),
('Vet Visit', 'Medical care'),
('Walk', 'Take a walk'),
('Training', 'Train pet'),
('Talk', 'Communicate'),
('Reward', 'Give treat'),
('Groom', 'Brush and groom');
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
('First Pet', 'Adopt your first pet'),
('Loyal Owner', 'Keep pet for 7 days'),
('Playful Day', 'Play 5 times'),
('Clean Pet', 'Give 3 baths'),
('Veteran', 'Reach level 5'),
('Wealthy', 'Earn 1000 coins'),
('Tidy', 'Groom pet 3 times'),
('Trainer', 'Train pet 3 times'),
('Socializer', 'Talk to pet 5 times'),
('Champion', 'Reach top score in minigame');
INSERT INTO PlayerAchievement (PlayerID, AchievementID)
VALUES 
(3, 1),
(3, 3),
(4, 1),
(4, 4),
(5, 1),
(6, 2),
(6, 5),
(7, 6),
(8, 7),
(9, 10);
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




