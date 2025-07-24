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



--Go
INSERT INTO [User] (Role, UserName, Email, Password, Level, Coin, Diamond, Gem)
VALUES 
('Player', N'CatLover01', 'catlover01@example.com', 'pass1234', 5, 1000, 5, 3),
('Player', N'DogMaster99', 'dogmaster99@example.com', 'dogpass99', 3, 800, 2, 1),
('Admin', N'AdminPetCare', 'admin@petgame.com', 'adminpass', 10, 5000, 50, 20),
('Player', N'BunnyQueen', 'bunnyq@example.com', 'bunny123', 2, 300, 0, 0),
('Player', N'HamsterHero', 'hamhero@example.com', 'hamham', 4, 700, 3, 2),
('Admin', N'ModPuppy', 'modpuppy@example.com', 'mod123', 6, 2000, 10, 5),
('Player', N'KittyCraze', 'kittycraze@example.com', 'meowmeow', 1, 100, 0, 0),
('Player', N'FishyFella', 'fishyfella@example.com', 'fishfish', 3, 600, 1, 1),
('Player', N'BirdWatcher', 'birdw@example.com', 'tweet123', 5, 1200, 6, 4),
('Player', N'ReptileRider', 'reptrider@example.com', 'reptilepass',7, 1500, 8, 7);


INSERT INTO Shop (Name, Type, Description)
VALUES 
('Pets Shop', 'Pet', 'Pet adoption and animal companions'),
('Item Shop', 'Item', 'Supplies, food and toys for pets');

INSERT INTO Pet (AdminID, PetType, PetDefaultName, Description)
VALUES 
(1, 'Cat', 'Brown Cat', 'A calm and cuddly brown cat who loves to nap in the sun.'),
(1, 'Cat', 'White Cat', 'A playful white cat with bright blue eyes and a curious nature.'),
(1, 'Cat', 'Orange Cat', 'A mischievous orange cat full of energy and charm.'),
(1, 'Chicken', 'Chicken', 'A cheerful chicken that lays eggs and enjoys pecking around the yard.'),
(1, 'Peacock', 'Peacock', 'A majestic peacock with shimmering feathers and an elegant strut.'),
(1, 'Porcupine', 'Porcupine', 'A shy porcupine with spiky quills and a big heart.');


INSERT INTO ShopProduct (ShopID, AdminID,PetID, Name, Type, Description, ImageUrl, Price, CurrencyType)
VALUES 
-- Pets Shop (ShopID = 1)
(1, 1,1, 'Brown Cat', 'Pet', 'A calm and cuddly brown cat who loves to nap in the sun.', 'https://drive.google.com/file/d/1RaHsGrKbr0gMZqQzIL1DS0JfZtAGre1P/view', 100, 'Coin'),
(1, 1,2, 'White Cat', 'Pet', 'A playful white cat with bright blue eyes and a curious nature.', 'https://drive.google.com/file/d/12b1oAHdgh4bfoPBNHVd-EaJZMmQl4b0J/view', 120, 'Coin'),
(1, 1,3, 'Orange Cat', 'Pet', 'A mischievous orange cat full of energy and charm.', 'https://drive.google.com/file/d/1505vN47KRjXfx3gM5NbHVSnYVN2gpWdG/view', 150, 'Coin'),
(1, 1,4, 'Chicken', 'Pet', 'A cheerful chicken that lays eggs and enjoys pecking around the yard.', 'https://drive.google.com/file/d/1fsJXvABMVtfGSPJz7E-_yhqv0H7Fo8oS/view', 300, 'Coin'),
(1, 1,5, 'Peacock', 'Pet', 'A majestic peacock with shimmering feathers and an elegant strut.', 'https://drive.google.com/file/d/1GE8iqscHwOYsgTuO36Lx09PiEc6ct4WH/view', 15, 'Gem'),
(1, 1,6, 'Porcupine', 'Pet', 'A shy porcupine with spiky quills and a big heart.', 'https://drive.google.com/file/d/1SfqfPU2KDpSEmVLvXqupFVs7q5PLcD0I/view', 35, 'Diamond');




-- Item Shop (ShopID = 2)
(2, 1,NULL, 'Cookies', 'Food', 'Sweet and crunchy cookies for your pet to enjoy', 'https://drive.google.com/file/d/1BI3P_--YCN0OQvrpeEWVK1l0dbIPFLOt/view', 40, 'Coin'),
(2, 1,NULL, 'Chocolate', 'Food', 'A special chocolate treat (non-toxic for pets)', 'https://drive.google.com/file/d/1eeJ-Tx6ztnARuhZF3rTvThSEb1uR9ScH/view', 40, 'Coin'),
(2, 1,NULL, 'Orange', 'Food', 'Fresh and juicy orange slices full of vitamins', 'https://drive.google.com/file/d/1Mz8_kpl7E1_MAlQ_IF1vdeXpZwEKLK0Y/view', 40, 'Coin'),
(2, 1,NULL, 'Cherry', 'Food', 'Sweet cherries that boost pet energy', 'https://drive.google.com/file/d/1vRZIx7kORpayLeHnxZSpJ1LwIGcsgrx4/view', 40, 'Coin'),
(2, 1,NULL, 'Pear', 'Food', 'Ripe and juicy pears for healthy digestion', 'https://drive.google.com/file/d/1Rm5aSvUASG_KzZMIWWLaSmVOx8f0Ts-8/view', 40, 'Coin'),
(2, 1,NULL, 'Banana', 'Food', 'Soft and sweet bananas loved by all pets', 'https://drive.google.com/file/d/1gJtCKHc1IBc_JRGLd29yaYbaCUETSM8A/view', 40, 'Coin');

Select * from ShopProduct;

UPDATE ShopProduct
SET Type = 'Pet'
WHERE ShopID = 1 AND PetID IS NOT NULL;






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
(5, 6, 2),
(6, 6, 3),
(7, 7, 1),
(8, 7, 2);
INSERT INTO Achievement (AchievementName, Description)
VALUES
('Welcome Aboard!', 'First login to receive a reward'),
('Pet Collector I', 'Own 2 pets to receive a reward'),
('Pet Collector II', 'Own 4 pets to receive a reward'),
('Pet Collector III', 'Own 6 pets to receive a reward'),
('Wealthy I', 'Own 1,000 coins to receive a reward'),
('Wealthy II', 'Own 5,000 coins to receive a reward'),
('Wealthy III', 'Own 10,000 coins to receive a reward');

INSERT INTO Minigame (Name, Description)
VALUES 
('Dark', 'Escape room');


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
	Status NVARCHAR(50),
    LastStatusUpdate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PlayerID) REFERENCES [User](ID),
    FOREIGN KEY (PetID) REFERENCES Pet(PetID)
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



INSERT INTO [User] (Role, UserName, Email, Password, Level, Coin, Diamond, Gem)
VALUES 
('Player', N'CatLover01', 'catlover01@example.com', 'pass1234', 5, 1000, 5, 3),
('Player', N'DogMaster99', 'dogmaster99@example.com', 'dogpass99', 3, 800, 2, 1),
('Admin', N'AdminPetCare', 'admin@petgame.com', 'adminpass', 10, 5000, 50, 20),
('Player', N'BunnyQueen', 'bunnyq@example.com', 'bunny123', 2, 300, 0, 0),
('Player', N'HamsterHero', 'hamhero@example.com', 'hamham', 4, 700, 3, 2),
('Admin', N'ModPuppy', 'modpuppy@example.com', 'mod123', 6, 2000, 10, 5),
('Player', N'KittyCraze', 'kittycraze@example.com', 'meowmeow', 1, 100, 0, 0),
('Player', N'FishyFella', 'fishyfella@example.com', 'fishfish', 3, 600, 1, 1),
('Player', N'BirdWatcher', 'birdw@example.com', 'tweet123', 5, 1200, 6, 4),
('Player', N'ReptileRider', 'reptrider@example.com', 'reptilepass',7, 1500, 8, 7);


INSERT INTO Shop (Name, Type, Description)
VALUES 
('Pets Shop', 'Pet', 'Pet adoption and animal companions'),
('Item Shop', 'Item', 'Supplies, food and toys for pets');

INSERT INTO Pet (AdminID, PetType, PetDefaultName, Description)
VALUES 
(1, 'Cat', 'Brown Cat', 'A calm and cuddly brown cat who loves to nap in the sun.'),
(1, 'Cat', 'White Cat', 'A playful white cat with bright blue eyes and a curious nature.'),
(1, 'Cat', 'Orange Cat', 'A mischievous orange cat full of energy and charm.'),
(1, 'Chicken', 'Chicken', 'A cheerful chicken that lays eggs and enjoys pecking around the yard.'),
(1, 'Peacock', 'Peacock', 'A majestic peacock with shimmering feathers and an elegant strut.'),
(1, 'Porcupine', 'Porcupine', 'A shy porcupine with spiky quills and a big heart.');


INSERT INTO ShopProduct (ShopID, AdminID,PetID, Name, Type, Description, ImageUrl, Price, CurrencyType)
VALUES 
-- Pets Shop (ShopID = 1)
(1, 1,1, 'Brown Cat', 'Pet', 'A calm and cuddly brown cat who loves to nap in the sun.', 'https://drive.google.com/file/d/1RaHsGrKbr0gMZqQzIL1DS0JfZtAGre1P/view', 100, 'Coin'),
(1, 1,2, 'White Cat', 'Pet', 'A playful white cat with bright blue eyes and a curious nature.', 'https://drive.google.com/file/d/12b1oAHdgh4bfoPBNHVd-EaJZMmQl4b0J/view', 120, 'Coin'),
(1, 1,3, 'Orange Cat', 'Pet', 'A mischievous orange cat full of energy and charm.', 'https://drive.google.com/file/d/1505vN47KRjXfx3gM5NbHVSnYVN2gpWdG/view', 150, 'Coin'),
(1, 1,4, 'Chicken', 'Pet', 'A cheerful chicken that lays eggs and enjoys pecking around the yard.', 'https://drive.google.com/file/d/1fsJXvABMVtfGSPJz7E-_yhqv0H7Fo8oS/view', 150, 'Coin'),
(1, 1,5, 'Peacock', 'Pet', 'A majestic peacock with shimmering feathers and an elegant strut.', 'https://drive.google.com/file/d/1GE8iqscHwOYsgTuO36Lx09PiEc6ct4WH/view', 150, 'Coin'),
(1, 1,6, 'Porcupine', 'Pet', 'A shy porcupine with spiky quills and a big heart.', 'https://drive.google.com/file/d/1SfqfPU2KDpSEmVLvXqupFVs7q5PLcD0I/view', 150, 'Coin'),




-- Item Shop (ShopID = 2)
(2, 1,NULL, 'Cookies', 'Food', 'Sweet and crunchy cookies for your pet to enjoy', 'https://drive.google.com/file/d/1BI3P_--YCN0OQvrpeEWVK1l0dbIPFLOt/view', 40, 'Coin'),
(2, 1,NULL, 'Chocolate', 'Food', 'A special chocolate treat (non-toxic for pets)', 'https://drive.google.com/file/d/1eeJ-Tx6ztnARuhZF3rTvThSEb1uR9ScH/view', 40, 'Coin'),
(2, 1,NULL, 'Orange', 'Food', 'Fresh and juicy orange slices full of vitamins', 'https://drive.google.com/file/d/1Mz8_kpl7E1_MAlQ_IF1vdeXpZwEKLK0Y/view', 40, 'Coin'),
(2, 1,NULL, 'Cherry', 'Food', 'Sweet cherries that boost pet energy', 'https://drive.google.com/file/d/1vRZIx7kORpayLeHnxZSpJ1LwIGcsgrx4/view', 40, 'Coin'),
(2, 1,NULL, 'Pear', 'Food', 'Ripe and juicy pears for healthy digestion', 'https://drive.google.com/file/d/1Rm5aSvUASG_KzZMIWWLaSmVOx8f0Ts-8/view', 40, 'Coin'),
(2, 1,NULL, 'Banana', 'Food', 'Soft and sweet bananas loved by all pets', 'https://drive.google.com/file/d/1gJtCKHc1IBc_JRGLd29yaYbaCUETSM8A/view', 40, 'Coin');

Select * from ShopProduct;

UPDATE ShopProduct
SET Type = 'Pet'
WHERE ShopID = 1 AND PetID IS NOT NULL;






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
(5, 6, 2),
(6, 6, 3),
(7, 7, 1),
(8, 7, 2);
INSERT INTO Achievement (AchievementName, Description)
VALUES
('Welcome Aboard!', 'First login to receive a reward'),
('Pet Collector I', 'Own 2 pets to receive a reward'),
('Pet Collector II', 'Own 4 pets to receive a reward'),
('Pet Collector III', 'Own 6 pets to receive a reward'),
('Wealthy I', 'Own 1,000 coins to receive a reward'),
('Wealthy II', 'Own 5,000 coins to receive a reward'),
('Wealthy III', 'Own 10,000 coins to receive a reward');

INSERT INTO Minigame (Name, Description)
VALUES 
('Dark', 'Escape room');








