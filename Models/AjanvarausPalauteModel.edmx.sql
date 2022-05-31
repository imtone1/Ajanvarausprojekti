
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/31/2022 14:25:59
-- Generated from EDMX file: C:\Users\helih\git\Ajanvarausprojekti\Models\AjanvarausPalauteModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [aikapalaute];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_kestot]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ajat] DROP CONSTRAINT [FK_kestot];
GO
IF OBJECT_ID(N'[dbo].[FK_oikeudet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Kayttajatunnukset] DROP CONSTRAINT [FK_oikeudet];
GO
IF OBJECT_ID(N'[dbo].[FK_opentunnus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Kayttajatunnukset] DROP CONSTRAINT [FK_opentunnus];
GO
IF OBJECT_ID(N'[dbo].[FK_opettajanaika]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ajat] DROP CONSTRAINT [FK_opettajanaika];
GO
IF OBJECT_ID(N'[dbo].[FK_palautetyyppi]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Palautteet] DROP CONSTRAINT [FK_palautetyyppi];
GO
IF OBJECT_ID(N'[dbo].[FK_palautteet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Palautteet] DROP CONSTRAINT [FK_palautteet];
GO
IF OBJECT_ID(N'[dbo].[FK_varausaika]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Varaukset] DROP CONSTRAINT [FK_varausaika];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Ajat]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Ajat];
GO
IF OBJECT_ID(N'[dbo].[Kayttajatunnukset]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Kayttajatunnukset];
GO
IF OBJECT_ID(N'[dbo].[Kestot]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Kestot];
GO
IF OBJECT_ID(N'[dbo].[Opettajat]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Opettajat];
GO
IF OBJECT_ID(N'[dbo].[Palautetyypit]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Palautetyypit];
GO
IF OBJECT_ID(N'[dbo].[Palautteet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Palautteet];
GO
IF OBJECT_ID(N'[dbo].[Varaukset]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Varaukset];
GO
IF OBJECT_ID(N'[dbo].[Yllapitooikeudet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Yllapitooikeudet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Ajat'
CREATE TABLE [dbo].[Ajat] (
    [aika_id] int IDENTITY(1,1) NOT NULL,
    [alku_aika] datetime  NOT NULL,
    [kesto_id] int  NOT NULL,
    [opettaja_id] int  NOT NULL,
    [aihe] varchar(max)  NULL,
    [paikka] varchar(2000)  NULL
);
GO

-- Creating table 'Kayttajatunnukset'
CREATE TABLE [dbo].[Kayttajatunnukset] (
    [kayttajatunnus_id] int IDENTITY(1,1) NOT NULL,
    [kayttajatunnus] varchar(50)  NOT NULL,
    [salasana] varchar(200)  NOT NULL,
    [opettaja_id] int  NOT NULL,
    [oikeudet_id] int  NOT NULL
);
GO

-- Creating table 'Kestot'
CREATE TABLE [dbo].[Kestot] (
    [kesto_id] int  NOT NULL,
    [kesto] int  NOT NULL
);
GO

-- Creating table 'Opettajat'
CREATE TABLE [dbo].[Opettajat] (
    [opettaja_id] int IDENTITY(1,1) NOT NULL,
    [sahkoposti] varchar(150)  NOT NULL,
    [etunimi] varchar(50)  NOT NULL,
    [sukunimi] varchar(100)  NOT NULL,
    [nimike] varchar(100)  NOT NULL,
    [kuva] varchar(250)  NULL,
    [opeimage] varbinary(max)  NULL
);
GO

-- Creating table 'Palautetyypit'
CREATE TABLE [dbo].[Palautetyypit] (
    [palautetyyppi_id] int IDENTITY(1,1) NOT NULL,
    [palautetyyppi] varchar(50)  NOT NULL
);
GO

-- Creating table 'Palautteet'
CREATE TABLE [dbo].[Palautteet] (
    [palaute_id] int IDENTITY(1,1) NOT NULL,
    [palaute] varchar(max)  NOT NULL,
    [palaute_pvm] datetime  NOT NULL,
    [palautetyyppi_id] int  NULL,
    [opettaja_id] int  NOT NULL
);
GO

-- Creating table 'Varaukset'
CREATE TABLE [dbo].[Varaukset] (
    [varaus_id] int IDENTITY(1,1) NOT NULL,
    [varaaja_nimi] varchar(50)  NULL,
    [varattu_pvm] datetime  NOT NULL,
    [aika_id] int  NOT NULL,
    [varaaja_sahkoposti] varchar(250)  NULL,
    [id_hash] varchar(350)  NULL,
    [aihe] varchar(max)  NULL
);
GO

-- Creating table 'Yllapitooikeudet'
CREATE TABLE [dbo].[Yllapitooikeudet] (
    [oikeudet_id] int  NOT NULL,
    [oikeudet] varchar(25)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [aika_id] in table 'Ajat'
ALTER TABLE [dbo].[Ajat]
ADD CONSTRAINT [PK_Ajat]
    PRIMARY KEY CLUSTERED ([aika_id] ASC);
GO

-- Creating primary key on [kayttajatunnus_id] in table 'Kayttajatunnukset'
ALTER TABLE [dbo].[Kayttajatunnukset]
ADD CONSTRAINT [PK_Kayttajatunnukset]
    PRIMARY KEY CLUSTERED ([kayttajatunnus_id] ASC);
GO

-- Creating primary key on [kesto_id] in table 'Kestot'
ALTER TABLE [dbo].[Kestot]
ADD CONSTRAINT [PK_Kestot]
    PRIMARY KEY CLUSTERED ([kesto_id] ASC);
GO

-- Creating primary key on [opettaja_id] in table 'Opettajat'
ALTER TABLE [dbo].[Opettajat]
ADD CONSTRAINT [PK_Opettajat]
    PRIMARY KEY CLUSTERED ([opettaja_id] ASC);
GO

-- Creating primary key on [palautetyyppi_id] in table 'Palautetyypit'
ALTER TABLE [dbo].[Palautetyypit]
ADD CONSTRAINT [PK_Palautetyypit]
    PRIMARY KEY CLUSTERED ([palautetyyppi_id] ASC);
GO

-- Creating primary key on [palaute_id] in table 'Palautteet'
ALTER TABLE [dbo].[Palautteet]
ADD CONSTRAINT [PK_Palautteet]
    PRIMARY KEY CLUSTERED ([palaute_id] ASC);
GO

-- Creating primary key on [varaus_id] in table 'Varaukset'
ALTER TABLE [dbo].[Varaukset]
ADD CONSTRAINT [PK_Varaukset]
    PRIMARY KEY CLUSTERED ([varaus_id] ASC);
GO

-- Creating primary key on [oikeudet_id] in table 'Yllapitooikeudet'
ALTER TABLE [dbo].[Yllapitooikeudet]
ADD CONSTRAINT [PK_Yllapitooikeudet]
    PRIMARY KEY CLUSTERED ([oikeudet_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [kesto_id] in table 'Ajat'
ALTER TABLE [dbo].[Ajat]
ADD CONSTRAINT [FK_kestot]
    FOREIGN KEY ([kesto_id])
    REFERENCES [dbo].[Kestot]
        ([kesto_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_kestot'
CREATE INDEX [IX_FK_kestot]
ON [dbo].[Ajat]
    ([kesto_id]);
GO

-- Creating foreign key on [opettaja_id] in table 'Ajat'
ALTER TABLE [dbo].[Ajat]
ADD CONSTRAINT [FK_opettajanaika]
    FOREIGN KEY ([opettaja_id])
    REFERENCES [dbo].[Opettajat]
        ([opettaja_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_opettajanaika'
CREATE INDEX [IX_FK_opettajanaika]
ON [dbo].[Ajat]
    ([opettaja_id]);
GO

-- Creating foreign key on [aika_id] in table 'Varaukset'
ALTER TABLE [dbo].[Varaukset]
ADD CONSTRAINT [FK_varausaika]
    FOREIGN KEY ([aika_id])
    REFERENCES [dbo].[Ajat]
        ([aika_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_varausaika'
CREATE INDEX [IX_FK_varausaika]
ON [dbo].[Varaukset]
    ([aika_id]);
GO

-- Creating foreign key on [oikeudet_id] in table 'Kayttajatunnukset'
ALTER TABLE [dbo].[Kayttajatunnukset]
ADD CONSTRAINT [FK_oikeudet]
    FOREIGN KEY ([oikeudet_id])
    REFERENCES [dbo].[Yllapitooikeudet]
        ([oikeudet_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_oikeudet'
CREATE INDEX [IX_FK_oikeudet]
ON [dbo].[Kayttajatunnukset]
    ([oikeudet_id]);
GO

-- Creating foreign key on [opettaja_id] in table 'Kayttajatunnukset'
ALTER TABLE [dbo].[Kayttajatunnukset]
ADD CONSTRAINT [FK_opentunnus]
    FOREIGN KEY ([opettaja_id])
    REFERENCES [dbo].[Opettajat]
        ([opettaja_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_opentunnus'
CREATE INDEX [IX_FK_opentunnus]
ON [dbo].[Kayttajatunnukset]
    ([opettaja_id]);
GO

-- Creating foreign key on [opettaja_id] in table 'Palautteet'
ALTER TABLE [dbo].[Palautteet]
ADD CONSTRAINT [FK_palautteet]
    FOREIGN KEY ([opettaja_id])
    REFERENCES [dbo].[Opettajat]
        ([opettaja_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_palautteet'
CREATE INDEX [IX_FK_palautteet]
ON [dbo].[Palautteet]
    ([opettaja_id]);
GO

-- Creating foreign key on [palautetyyppi_id] in table 'Palautteet'
ALTER TABLE [dbo].[Palautteet]
ADD CONSTRAINT [FK_palautetyyppi]
    FOREIGN KEY ([palautetyyppi_id])
    REFERENCES [dbo].[Palautetyypit]
        ([palautetyyppi_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_palautetyyppi'
CREATE INDEX [IX_FK_palautetyyppi]
ON [dbo].[Palautteet]
    ([palautetyyppi_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------