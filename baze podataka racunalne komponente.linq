<Query Kind="SQL">
  <Connection>
    <ID>69de4888-7bf3-4ef3-a51f-892a729e6d01</ID>
    <Persist>true</Persist>
    <Server>31.147.204.169\FERITMSSQL</Server>
    <SqlSecurity>true</SqlSecurity>
    <NoPluralization>true</NoPluralization>
    <UserName>student</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAShd6d9eRgUiMd2vk7cCBtAAAAAACAAAAAAAQZgAAAAEAACAAAAAao2ewLyOMYrbM8PlJ8L6UOltPZ7S72s3VmJ3FUN3sDgAAAAAOgAAAAAIAACAAAAAqYXAhOW1qltBYt7/JTlevKlRVhSMiCZvdTmARXcTSrxAAAAAZ2tI97wTnLehk88dbJ0MrQAAAAOsK58sssbrIrRsfiGk6P9waVbUG59agus7ShQYKONnrj+5GVatJZ0eziVS9AQagkjyLlXzqgSOjL4r1CmLSu6Q=</Password>
    <Database>student</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

CREATE TABLE Komponenta (
  ID INT IDENTITY(1,1) PRIMARY KEY,
  Proizvodac VARCHAR(255),
  Model VARCHAR(255),
  Cijena DECIMAL(10, 2),
  Kategorija VARCHAR(255)
);
CREATE TABLE Procesor (
  ID INT PRIMARY KEY,
  BrojJezgri TINYINT,
  RadniTakt DECIMAL(4, 2),
  CacheMemorija SMALLINT,
  TDP SMALLINT,
  ID_Komponente INT,
  Socket VARCHAR(50),
  CONSTRAINT fk_procesor_komponenta FOREIGN KEY (ID_Komponente) REFERENCES Komponenta(ID)
);
CREATE TABLE MaticnaPloca (
  ID INT PRIMARY KEY,
  Format VARCHAR(50),
  BrojMemorijskihModula TINYINT,
  Socket VARCHAR(50),
  PodrzanaMemorija VARCHAR(50),
  ID_Komponente INT,
  CONSTRAINT fk_maticna_ploca_komponenta FOREIGN KEY (ID_Komponente) REFERENCES Komponenta(ID)
);

CREATE TABLE GrafickaKartica (
  ID INT PRIMARY KEY,
  Memorija TINYINT,
  Takt DECIMAL(4, 2),
  TDP SMALLINT,
  ID_Komponente INT,
  CONSTRAINT fk_graficka_kartica_komponenta FOREIGN KEY (ID_Komponente) REFERENCES Komponenta(ID)
);
CREATE TABLE RadnaMemorija (
  ID INT PRIMARY KEY,
  Kapacitet SMALLINT,
  Tip VARCHAR(255),
  Brzina SMALLINT,
  Kolicina TINYINT,
  ID_Komponente INT,
  CONSTRAINT fk_radna_memorija_komponenta FOREIGN KEY (ID_Komponente) REFERENCES Komponenta(ID)
);
CREATE TABLE VanjskaMemorija (
  ID INT PRIMARY KEY,
  Kapacitet INT,
  Povezivost VARCHAR(50),
  ID_Komponente INT,
  TipMemorije VARCHAR(50),
  CONSTRAINT fk_vanjska_memorija_komponenta FOREIGN KEY (ID_Komponente) REFERENCES Komponenta(ID)
);
CREATE TABLE Napajanje (
  ID INT PRIMARY KEY,
  Snaga INT,
  Efikasnost VARCHAR(255),
  Modularno CHAR(2) DEFAULT 'NE',
  ID_Komponente INT,
  CONSTRAINT fk_napajanje_komponenta FOREIGN KEY (ID_Komponente) REFERENCES Komponenta(ID)
);
CREATE TABLE Kuciste (
  ID INT PRIMARY KEY,
  Vrsta VARCHAR(255),
  ID_Komponente INT,
  CONSTRAINT fk_kuciste_komponenta FOREIGN KEY (ID_Komponente) REFERENCES Komponenta(ID)
);
CREATE PROCEDURE DodajKomponentuProcesor
(
    @Proizvodac VARCHAR(255),
    @Model VARCHAR(255),
    @Cijena DECIMAL(10, 2),
    @Kategorija VARCHAR(255),
    @BrojJezgri TINYINT,
    @RadniTakt DECIMAL(4, 2),
    @CacheMemorija SMALLINT,
    @TDP SMALLINT,
    @Socket VARCHAR(50)
)
AS
BEGIN
    BEGIN TRANSACTION;
	
    INSERT INTO Komponenta (Proizvodac, Model, Cijena, Kategorija)
    VALUES (@Proizvodac, @Model, @Cijena, @Kategorija);
  
    DECLARE @KomponentaID INT = SCOPE_IDENTITY();
	
	DECLARE @ID_proc INT;
	
	SELECT @ID_proc = ISNULL(MAX(ID), 0) + 1 FROM Procesor;
	SET @ID_proc = @ID_proc + 1;
   
    INSERT INTO Procesor (ID, BrojJezgri, RadniTakt, CacheMemorija, TDP, ID_Komponente, Socket)
    VALUES (@ID_proc, @BrojJezgri, @RadniTakt, @CacheMemorija, @TDP, @KomponentaID, @Socket);

    COMMIT TRANSACTION;
END;
CREATE PROCEDURE DodajKomponentuMaticnaPloca
(
	@Proizvodac VARCHAR(255),
    @Model VARCHAR(255),
    @Cijena DECIMAL(10, 2),
    @Kategorija VARCHAR(255),
  	@Format VARCHAR(50),
  	@BrojMemorijskihModula TINYINT,
  	@Socket VARCHAR(50),
  	@PodrzanaMemorija VARCHAR(50)
)
AS
BEGIN
    BEGIN TRANSACTION;

    -- Umetanje podataka u tablicu Komponenta (ID će se automatski generirati)
    INSERT INTO Komponenta (Proizvodac, Model, Cijena, Kategorija)
    VALUES (@Proizvodac, @Model, @Cijena, @Kategorija);

    -- Dohvaćanje generirane ID vrijednosti
    DECLARE @KomponentaID INT = SCOPE_IDENTITY();
	
	DECLARE @ID_mp INT;
	
	SELECT @ID_mp = ISNULL(MAX(ID), 0) + 1 FROM MaticnaPloca;
	SET @ID_mp = @ID_mp + 1;
	
    -- Umetanje podataka u tablicu Procesor
    INSERT INTO MaticnaPloca (ID, Format, BrojMemorijskihModula, Socket, PodrzanaMemorija, ID_komponente)
    VALUES (@ID_mp, @Format, @BrojMemorijskihModula, @Socket, @PodrzanaMemorija, @KomponentaID);

    COMMIT TRANSACTION;
END;
CREATE PROCEDURE DodajKomponentuGrafickaKartica
(
    @Proizvodac VARCHAR(255),
    @Model VARCHAR(255),
    @Cijena DECIMAL(10, 2),
    @Kategorija VARCHAR(255),
    @Memorija TINYINT,
    @Takt DECIMAL(4, 2),
    @TDP SMALLINT
)
AS
BEGIN
    BEGIN TRANSACTION;

    -- Umetanje podataka u tablicu Komponenta (ID će se automatski generirati)
    INSERT INTO Komponenta (Proizvodac, Model,  Cijena, Kategorija)
    VALUES (@Proizvodac, @Model, @Cijena, @Kategorija);

    -- Dohvaćanje generirane ID vrijednosti za Komponentu
    DECLARE @KomponentaID INT = SCOPE_IDENTITY();

    -- Generiranje ID vrijednosti za GrafickuKartica
    DECLARE @GrafickaKarticaID INT;

    -- Pronalazak najveće vrijednosti ID-ja u tablici GrafickaKartica
    SELECT @GrafickaKarticaID = ISNULL(MAX(ID), 0) + 1 FROM GrafickaKartica;

    -- Umetanje podataka u tablicu GrafickaKartica
    INSERT INTO GrafickaKartica (ID, Memorija, Takt, TDP, ID_Komponente)
    VALUES (@GrafickaKarticaID, @Memorija, @Takt, @TDP, @KomponentaID);

    COMMIT TRANSACTION;
END;
CREATE PROCEDURE DodajKomponentuRadnaMemorija
(
    @Proizvodac VARCHAR(255),
    @Model VARCHAR(255),
    @Cijena DECIMAL(10, 2),
    @Kategorija VARCHAR(255),
    @Kapacitet SMALLINT,
    @Tip VARCHAR(255),
    @Brzina SMALLINT,
    @Kolicina TINYINT
)
AS
BEGIN
    BEGIN TRANSACTION;

    -- Umetanje podataka u tablicu Komponenta (ID će se automatski generirati)
    INSERT INTO Komponenta (Proizvodac, Model,  Cijena, Kategorija)
    VALUES (@Proizvodac, @Model,@Cijena, @Kategorija);

    -- Dohvaćanje generirane ID vrijednosti
    DECLARE @KomponentaID INT = SCOPE_IDENTITY();

    -- Dohvaćanje ID vrijednosti za RadnaMemorija
    DECLARE @ID_rm INT;
    SELECT @ID_rm = ISNULL(MAX(ID), 0) + 1 FROM RadnaMemorija;

    -- Umetanje podataka u tablicu RadnaMemorija
    INSERT INTO RadnaMemorija (ID, Kapacitet, Tip, Brzina, Kolicina, ID_Komponente)
    VALUES (@ID_rm, @Kapacitet, @Tip, @Brzina, @Kolicina, @KomponentaID);

    COMMIT TRANSACTION;
END;
CREATE PROCEDURE DodajKomponentuVanjskaMemorija
(
    @Proizvodac VARCHAR(100),
    @Kapacitet INT,
    @Povezivost VARCHAR(50),
    @TipMemorije VARCHAR(50),
    @Model VARCHAR(255),
    @Cijena DECIMAL(10, 2),
    @Kategorija VARCHAR(255)
)
AS
BEGIN
    BEGIN TRANSACTION;

    -- Umetanje podataka u tablicu Komponenta (ID će se automatski generirati)
    INSERT INTO Komponenta (Proizvodac, Model,  Cijena, Kategorija)
    VALUES (@Proizvodac, @Model,@Cijena, @Kategorija);

    -- Dohvaćanje generirane ID vrijednosti
    DECLARE @KomponentaID INT = SCOPE_IDENTITY();

    -- Dohvaćanje ID vrijednosti za VanjskaMemorija
    DECLARE @ID_vm INT;
    SELECT @ID_vm = ISNULL(MAX(ID), 0) + 1 FROM VanjskaMemorija;

    -- Umetanje podataka u tablicu VanjskaMemorija
    INSERT INTO VanjskaMemorija (ID, Kapacitet, Povezivost, ID_Komponente, TipMemorije)
    VALUES (@ID_vm, @Kapacitet, @Povezivost, @KomponentaID, @TipMemorije);

    COMMIT TRANSACTION;
END;
CREATE PROCEDURE DodajKomponentuNapajanje
(
    @Proizvodac VARCHAR(255),
    @Model VARCHAR(255),
    @Cijena DECIMAL(10, 2),
    @Kategorija VARCHAR(255),
    @Snaga INT,
    @Efikasnost VARCHAR(255),
    @Modularno CHAR(2)
)
AS
BEGIN
    BEGIN TRANSACTION;

    -- Umetanje podataka u tablicu Komponenta (ID će se automatski generirati)
    INSERT INTO Komponenta (Proizvodac, Model,  Cijena, Kategorija)
    VALUES (@Proizvodac, @Model,@Cijena, @Kategorija);

    -- Dohvaćanje generirane ID vrijednosti
    DECLARE @KomponentaID INT = SCOPE_IDENTITY();

    -- Dohvaćanje ID vrijednosti za Napajanje
    DECLARE @ID_np INT;
    SELECT @ID_np = ISNULL(MAX(ID), 0) + 1 FROM Napajanje;

    -- Umetanje podataka u tablicu Napajanje
    INSERT INTO Napajanje (ID, Snaga, Efikasnost, Modularno, ID_Komponente)
    VALUES (@ID_np, @Snaga, @Efikasnost, @Modularno, @KomponentaID);

    COMMIT TRANSACTION;
END;
CREATE PROCEDURE DodajKomponentuKuciste
(
    @Proizvodac VARCHAR(255),
    @Model VARCHAR(255),
    @Cijena DECIMAL(10, 2),
    @Kategorija VARCHAR(255),
    @Vrsta VARCHAR(255)
)
AS
BEGIN
    BEGIN TRANSACTION;

    -- Umetanje podataka u tablicu Komponenta (ID će se automatski generirati)
    INSERT INTO Komponenta (Proizvodac, Model,  Cijena, Kategorija)
    VALUES (@Proizvodac, @Model, @Cijena, @Kategorija);

    -- Dohvaćanje generirane ID vrijednosti
    DECLARE @KomponentaID INT = SCOPE_IDENTITY();

    -- Dohvaćanje ID vrijednosti za Kuciste
    DECLARE @ID_kc INT;
    SELECT @ID_kc = ISNULL(MAX(ID), 0) + 1 FROM Kuciste;

    -- Umetanje podataka u tablicu Kuciste
    INSERT INTO Kuciste (ID, Vrsta, ID_Komponente)
    VALUES (@ID_kc, @Vrsta, @KomponentaID);

    COMMIT TRANSACTION;
END;
CREATE PROCEDURE PrikaziKompatibilneProcesore(@Socket VARCHAR(50))
AS
BEGIN
  SET @Socket = UPPER(@Socket); -- Pretvara socket u velika slova
  SELECT P.ID, K.Proizvodac, K.Model, P.BrojJezgri, P.RadniTakt, P.CacheMemorija, P.TDP
  FROM Procesor P
  INNER JOIN Komponenta K ON P.ID_Komponente = K.ID
  WHERE UPPER(P.Socket) = @Socket; -- Uspoređuje socket u velikim slovima
END;
CREATE PROCEDURE ObrisiProcesorPoModelu(@Model VARCHAR(255))
AS
BEGIN
  BEGIN TRANSACTION;

  DECLARE @KomponentaID INT;
  DECLARE @ProcesorID INT;

  -- Pronađi ID komponente na temelju modela
  SELECT @KomponentaID = ID
  FROM Komponenta
  WHERE Model = @Model;

  -- Obriši procesor ako postoji
  IF (@KomponentaID IS NOT NULL)
  BEGIN
    -- Pronađi ID procesora na temelju ID komponente
    SELECT @ProcesorID = ID
    FROM Procesor
    WHERE ID_Komponente = @KomponentaID;

    -- Obriši procesor ako postoji
    IF (@ProcesorID IS NOT NULL)
    BEGIN
      DELETE FROM Procesor WHERE ID = @ProcesorID;
    END

    -- Obriši komponentu
    DELETE FROM Komponenta WHERE ID = @KomponentaID;

    COMMIT TRANSACTION;
    PRINT 'Procesor je uspješno obrisan.';
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Procesor s navedenim modelom ne postoji.';
  END
END;
CREATE PROCEDURE ObrisiMaticnuPlocuPoModelu(@Model VARCHAR(255))
AS
BEGIN
  BEGIN TRANSACTION;

  DECLARE @KomponentaID INT;
  DECLARE @MaticnaPlocaID INT;

  -- Pronađi ID komponente na temelju modela
  SELECT @KomponentaID = ID
  FROM Komponenta
  WHERE Model = @Model;

  -- Obriši matičnu ploču ako postoji
  IF (@KomponentaID IS NOT NULL)
  BEGIN
    -- Pronađi ID matične ploče na temelju ID komponente
    SELECT @MaticnaPlocaID = ID
    FROM MaticnaPloca
    WHERE ID_Komponente = @KomponentaID;

    -- Obriši matičnu ploču ako postoji
    IF (@MaticnaPlocaID IS NOT NULL)
    BEGIN
      DELETE FROM MaticnaPloca WHERE ID = @MaticnaPlocaID;
    END

    -- Obriši komponentu
    DELETE FROM Komponenta WHERE ID = @KomponentaID;

    COMMIT TRANSACTION;
    PRINT 'Matična ploča je uspješno obrisana.';
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Matična ploča s navedenim modelom ne postoji.';
  END
END;
CREATE PROCEDURE ObrisiGrafickuKarticuPoModelu(@Model VARCHAR(255))
AS
BEGIN
  BEGIN TRANSACTION;

  DECLARE @KomponentaID INT;
  DECLARE @GrafickaKarticaID INT;

  -- Pronađi ID komponente na temelju modela
  SELECT @KomponentaID = ID
  FROM Komponenta
  WHERE Model = @Model;

  -- Obriši grafičku karticu ako postoji
  IF (@KomponentaID IS NOT NULL)
  BEGIN
    -- Pronađi ID grafičke kartice na temelju ID komponente
    SELECT @GrafickaKarticaID = ID
    FROM GrafickaKartica
    WHERE ID_Komponente = @KomponentaID;

    -- Obriši grafičku karticu ako postoji
    IF (@GrafickaKarticaID IS NOT NULL)
    BEGIN
      DELETE FROM GrafickaKartica WHERE ID = @GrafickaKarticaID;
    END

    -- Obriši komponentu
    DELETE FROM Komponenta WHERE ID = @KomponentaID;

    COMMIT TRANSACTION;
    PRINT 'Grafička kartica je uspješno obrisana.';
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Grafička kartica s navedenim modelom ne postoji.';
  END
END;
CREATE PROCEDURE ObrisiRadnuMemorijuPoModelu(@Model VARCHAR(255))
AS
BEGIN
  BEGIN TRANSACTION;

  DECLARE @KomponentaID INT;
  DECLARE @RadnaMemorijaID INT;

  -- Pronađi ID komponente na temelju modela
  SELECT @KomponentaID = ID
  FROM Komponenta
  WHERE Model = @Model;

  -- Obriši radnu memoriju ako postoji
  IF (@KomponentaID IS NOT NULL)
  BEGIN
    -- Pronađi ID radne memorije na temelju ID komponente
    SELECT @RadnaMemorijaID = ID
    FROM RadnaMemorija
    WHERE ID_Komponente = @KomponentaID;

    -- Obriši radnu memoriju ako postoji
    IF (@RadnaMemorijaID IS NOT NULL)
    BEGIN
      DELETE FROM RadnaMemorija WHERE ID = @RadnaMemorijaID;
    END

    -- Obriši komponentu
    DELETE FROM Komponenta WHERE ID = @KomponentaID;

    COMMIT TRANSACTION;
    PRINT 'Radna memorija je uspješno obrisana.';
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Radna memorija s navedenim modelom ne postoji.';
  END
END;

CREATE PROCEDURE ObrisiNapajanjePoModelu(@Model VARCHAR(255))
AS
BEGIN
  BEGIN TRANSACTION;

  DECLARE @KomponentaID INT;
  DECLARE @NapajanjeID INT;

  -- Pronađi ID komponente na temelju modela
  SELECT @KomponentaID = ID
  FROM Komponenta
  WHERE Model = @Model;

  -- Obriši napajanje ako postoji
  IF (@KomponentaID IS NOT NULL)
  BEGIN
    -- Pronađi ID napajanja na temelju ID komponente
    SELECT @NapajanjeID = ID
    FROM Napajanje
    WHERE ID_Komponente = @KomponentaID;

    -- Obriši napajanje ako postoji
    IF (@NapajanjeID IS NOT NULL)
    BEGIN
      DELETE FROM Napajanje WHERE ID = @NapajanjeID;
    END

    -- Obriši komponentu
    DELETE FROM Komponenta WHERE ID = @KomponentaID;

    COMMIT TRANSACTION;
    PRINT 'Napajanje je uspješno obrisano.';
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Napajanje s navedenim modelom ne postoji.';
  END
END;
CREATE PROCEDURE ObrisiKucistePoModelu(@Model VARCHAR(255))
AS
BEGIN
  BEGIN TRANSACTION;

  DECLARE @KomponentaID INT;
  DECLARE @KucisteID INT;

  -- Pronađi ID komponente na temelju modela
  SELECT @KomponentaID = ID
  FROM Komponenta
  WHERE Model = @Model;

  -- Obriši kućište ako postoji
  IF (@KomponentaID IS NOT NULL)
  BEGIN
    -- Pronađi ID kućišta na temelju ID komponente
    SELECT @KucisteID = ID
    FROM Kuciste
    WHERE ID_Komponente = @KomponentaID;

    -- Obriši kućište ako postoji
    IF (@KucisteID IS NOT NULL)
    BEGIN
      DELETE FROM Kuciste WHERE ID = @KucisteID;
    END

    -- Obriši komponentu
    DELETE FROM Komponenta WHERE ID = @KomponentaID;

    COMMIT TRANSACTION;
    PRINT 'Kućište je uspješno obrisano.';
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Kućište s navedenim modelom ne postoji.';
  END
END;
CREATE PROCEDURE ObrisiVanjskuMemorijuPoModelu(@Model VARCHAR(255))
AS
BEGIN
  BEGIN TRANSACTION;

  DECLARE @KomponentaID INT;
  DECLARE @VanjskaMemorijaID INT;

  -- Pronađi ID komponente na temelju modela
  SELECT @KomponentaID = ID
  FROM Komponenta
  WHERE Model = @Model;

  -- Obriši vanjsku memoriju ako postoji
  IF (@KomponentaID IS NOT NULL)
  BEGIN
    -- Pronađi ID vanjske memorije na temelju ID komponente
    SELECT @VanjskaMemorijaID = ID
    FROM VanjskaMemorija
    WHERE ID_Komponente = @KomponentaID;

    -- Obriši vanjsku memoriju ako postoji
    IF (@VanjskaMemorijaID IS NOT NULL)
    BEGIN
      DELETE FROM VanjskaMemorija WHERE ID = @VanjskaMemorijaID;
    END

    -- Obriši komponentu
    DELETE FROM Komponenta WHERE ID = @KomponentaID;

    COMMIT TRANSACTION;
    PRINT 'Vanjska memorija je uspješno obrisana.';
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Vanjska memorija s navedenim modelom ne postoji.';
  END
END;
CREATE FUNCTION DohvatiCijenuKomponenti
(
    @ModeliKomponenti VARCHAR(MAX)
)
RETURNS DECIMAL(10, 2)
AS
BEGIN
    DECLARE @Cijena DECIMAL(10, 2) = 0;

    SELECT @Cijena = SUM(Cijena)
    FROM Komponenta
    WHERE Model IN (SELECT value FROM STRING_SPLIT(@ModeliKomponenti, ','));

    RETURN @Cijena;
END;

DECLARE @Modeli VARCHAR(MAX) = 'Ryzen 7 5700X,B450,H510,RTX 2080,Vengeance LPX,Expansion,RM750X';
DECLARE @UkupnaCijena DECIMAL(10, 2);

SET @UkupnaCijena = dbo.DohvatiCijenuKomponenti(@Modeli);

SELECT @UkupnaCijena AS 'Ukupna Cijena Komponenti';

CREATE FUNCTION DohvatiKomponentePremaCijeni
(
    @MaksimalnaCijena DECIMAL(10, 2)
)
RETURNS TABLE
AS
RETURN
(
    SELECT Komponenta.ID, Komponenta.Proizvodac, Komponenta.Model, Komponenta.Cijena, Komponenta.Kategorija
    FROM Komponenta
    WHERE Komponenta.Cijena <= @MaksimalnaCijena
);

SELECT * FROM DohvatiKomponentePremaCijeni(200.00);

CREATE VIEW PrikazKomponenti AS
SELECT K.ID, K.Proizvodac, K.Model, K.Cijena, K.Kategorija,
       P.BrojJezgri, P.RadniTakt, P.CacheMemorija, P.TDP, P.Socket,
       MP.Format, MP.BrojMemorijskihModula, MP.Socket AS MaticnaPlocaSocket, MP.PodrzanaMemorija,
       GK.Memorija, GK.Takt, GK.TDP AS GrafickaKarticaTDP,
       RM.Kapacitet AS RadnaMemorijaKapacitet, RM.Tip AS RadnaMemorijaTip, RM.Brzina AS RadnaMemorijaBrzina, RM.Kolicina AS RadnaMemorijaKolicina,
       VM.Kapacitet AS VanjskaMemorijaKapacitet, VM.Povezivost AS VanjskaMemorijaPovezivost, VM.TipMemorije,
       N.Snaga AS NapajanjeSnaga, N.Efikasnost AS NapajanjeEfikasnost, N.Modularno,
       KC.Vrsta AS KucisteVrsta
FROM Komponenta K
LEFT JOIN Procesor P ON K.ID = P.ID_Komponente
LEFT JOIN MaticnaPloca MP ON K.ID = MP.ID_Komponente
LEFT JOIN GrafickaKartica GK ON K.ID = GK.ID_Komponente
LEFT JOIN RadnaMemorija RM ON K.ID = RM.ID_Komponente
LEFT JOIN VanjskaMemorija VM ON K.ID = VM.ID_Komponente
LEFT JOIN Napajanje N ON K.ID = N.ID_Komponente
LEFT JOIN Kuciste KC ON K.ID = KC.ID_Komponente;

SELECT ID, Proizvodac, Model, Cijena, Kategorija FROM PrikazKomponenti;

SELECT * FROM PrikazKomponenti
SELECT * FROM VanjskaMemorija, Komponenta WHERE Komponenta.ID = VanjskaMemorija.ID_komponente

EXEC DodajKomponentuProcesor 'Intel','i9-13900k', 349.99, 'Procesor', 24, 5.8, 32, 253, 'FCLGA1700';
EXEC DodajKomponentuProcesor 'AMD','Ryzen 7 5700X', 200.00, 'Procesor', 8, 4.7, 32, 65, 'AM4';
EXEC DodajKomponentuProcesor 'Intel', 'i7-9700K', 299.99, 'Procesor', 8, 3.6, 12, 95, 'LGA1151';

EXEC DodajKomponentuMaticnaPloca 'Gigabyte', 'Z590', 249.99, 'Maticna ploca', 'ATX', 4, 'LGA1200', 'DDR4';
EXEC DodajKomponentuMaticnaPloca 'MSI', 'X570', 299.99, 'Maticna ploca', 'ATX', 4, 'AM4', 'DDR4';
EXEC DodajKomponentuMaticnaPloca 'Asrock', 'B450', 149.99, 'Maticna ploca', 'ATX', 4, 'AM4', 'DDR4';

EXEC DodajKomponentuGrafickaKartica 'Nvidia', 'RTX 2080', 799.99, 'Graficka kartica', 8, 1.8, 250;
EXEC DodajKomponentuGrafickaKartica 'AMD', 'RX 6700 XT', 549.99, 'Graficka kartica', 12, 2.3, 300;
EXEC DodajKomponentuGrafickaKartica 'Nvidia', 'GTX 1660 Super', 299.99, 'Graficka kartica', 6, 1.7, 150;

EXEC DodajKomponentuRadnaMemorija 'G.Skill', 'Trident Z RGB', 129.99, 'Radna memorija', 32, 'DDR4', 3600, 4;
EXEC DodajKomponentuRadnaMemorija 'Corsair', 'Vengeance LPX', 79.99, 'Radna memorija', 16, 'DDR4', 3200, 2;
EXEC DodajKomponentuRadnaMemorija 'Kingston', 'HyperX Fury', 59.99, 'Radna memorija', 8, 'DDR4', 2666, 1;

EXEC DodajKomponentuVanjskaMemorija 'Western Digital', 500, 'USB 3.0', 'SSD', 'My Passport', 79.99, 'Vanjska memorija';
EXEC DodajKomponentuVanjskaMemorija 'Seagate', 2000, 'SATA 3', 'HDD', 'Expansion', 89.99, 'Vanjska memorija';
EXEC DodajKomponentuVanjskaMemorija 'Samsung', 4000, 'Thunderbolt 3', 'SSD', 'X5', 349.99, 'Vanjska memorija';

EXEC DodajKomponentuKuciste 'Fractal Design', 'Meshify C', 89.99, 'Kuciste', 'ATX';
EXEC DodajKomponentuKuciste 'NZXT', 'H510', 99.99, 'Kuciste', 'ATX'; za ovo
EXEC DodajKomponentuKuciste 'Cooler Master', 'MasterCase H500', 119.99, 'Kuciste', 'ATX';

EXEC DodajKomponentuNapajanje 'EVGA', 'SuperNOVA 850 G3', 169.99, 'Napajanje', 850, '80 Plus Gold', 'DA';
EXEC DodajKomponentuNapajanje 'Seasonic', 'Focus GX-650', 119.99, 'Napajanje', 650, '80 Plus Gold', 'DA';
EXEC DodajKomponentuNapajanje 'Corsair', 'RM750x', 149.99, 'Napajanje', 750, '80 Plus Gold', 'DA';

EXEC PrikaziKompatibilneProcesore 'am4';
EXEC PrikaziKompatibilneProcesoreZaModelMaticne 'B450';
EXEC ObrisiProcesorPoModelu 'i9-13900k';
EXEC ObrisiMaticnuPlocuPoModelu 'B450';

EXEC PrikaziKompatibilneProcesore 'am4';
EXEC PrikaziKompatibilneProcesoreZaModelMaticne 'B450';
EXEC ObrisiProcesorPoModelu 'i7-9700k';

DELETE FROM Komponenta;
DELETE FROM GrafickaKartica;
DELETE FROM VanjskaMemorija;
DELETE FROM RadnaMemorija;
DELETE FROM Kuciste;
DELETE FROM Napajanje;
DELETE FROM Procesor;
DELETE FROM MaticnaPloca;

DROP TABLE Komponenta;
DROP TABLE Kuciste;
DROP TABLE RadnaMemorija;
DROP TABLE VanjskaMemorija;
DROP TABLE MaticnaPloca;
DROP TABLE Procesor;
DROP TABLE GrafickaKartica;
DROP TABLE Napajanje;

DROP PROCEDURE DodajKomponentuProcesor;
DROP PROCEDURE DodajKomponentuMaticnaPloca;
DROP PROCEDURE DodajKomponentuGrafickaKartica;
DROP PROCEDURE DodajKomponentuRadnaMemorija;
DROP PROCEDURE DodajKomponentuVanjskaMemorija;
DROP PROCEDURE DodajKomponentuKuciste;
DROP PROCEDURE DodajKomponentuNapajanje;
DROP PROCEDURE PrikaziKompatibilneProcesore;
DROP PROCEDURE PrikaziKompatibilneProcesoreZaModelMaticne;
DROP VIEW PrikazKomponenti

SELECT * FROM Komponenta, Procesor WHERE Komponenta.ID = Procesor.ID_Komponente;
SELECT * FROM Komponenta, MaticnaPloca WHERE Komponenta.ID = MaticnaPloca.ID_Komponente;
SELECT * FROM Komponenta, GrafickaKartica WHERE Komponenta.ID = GrafickaKartica.ID_Komponente;
SELECT * FROM Komponenta;