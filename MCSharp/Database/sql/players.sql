CREATE TABLE Players (
	PlayerID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	GroupID INTEGER NOT NULL,
	ExpLevelID INTEGER NOT NULL,
	Name VARCHAR(255) NOT NULL,
	IPAddress INTEGER NOT NULL,
	TextColor INTEGER NOT NULL,
	Muted BOOLEAN NOT NULL,
	Jailed BOOLEAN NOT NULL,
	SpawnMap VARCHAR(255) NOT NULL,
	SpawnX INTEGER NOT NULL,
	SpawnY INTEGER NOT NULL,
	SpawnZ INTEGER NOT NULL,
	SpawnPitch INTEGER NOT NULL,
	SpawnYaw INTEGER NOT NULL
);

INSERT INTO Players 
(GroupID, ExpLevelID, Name, IPAddress, TextColor, Muted, Jailed, SpawnMap, SpawnX, SpawnY, SpawnZ, SpawnPitch, SpawnYaw)
VALUES (0, 0, "Voziv", 265255255256, 3, 0, 1, "main", -20, -2456, 200, 50, 240);

INSERT INTO Players 
(GroupID, ExpLevelID, Name, IPAddress, TextColor, Muted, Jailed, SpawnMap, SpawnX, SpawnY, SpawnZ, SpawnPitch, SpawnYaw)
VALUES (0, 0, "Feliv", 265255255258, 6, 1, 1, "guestmap", -156, 24, 20, 34, 64);

INSERT INTO Players 
(GroupID, ExpLevelID, Name, IPAddress, TextColor, Muted, Jailed, SpawnMap, SpawnX, SpawnY, SpawnZ, SpawnPitch, SpawnYaw)
VALUES (0, 0, "Yanix", 265255255260, 3, 1, 0, "vozivConstruction", 1, -65, 467, 36, 971);

INSERT INTO Players 
(GroupID, ExpLevelID, Name, IPAddress, TextColor, Muted, Jailed, SpawnMap, SpawnX, SpawnY, SpawnZ, SpawnPitch, SpawnYaw)
VALUES (0, 0, "Voziv", 265255255256, 12, 0, 1, "main", -20, -2456, 200, 50, 240);

INSERT INTO Players 
(GroupID, ExpLevelID, Name, IPAddress, TextColor, Muted, Jailed, SpawnMap, SpawnX, SpawnY, SpawnZ, SpawnPitch, SpawnYaw)
VALUES (0, 0, "Voziv", 265255255256, 3, 1, 0, "main", -20, -2456, 200, 50, 240);