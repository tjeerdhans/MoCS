
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/19/2014 21:08:17
-- Generated from EDMX file: D:\Projects\Repos\github\MoCS\MoCS\MoCS.Data\Entity\MoCS.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [aspnet-MoCS.Web-20140218060439];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_TournamentTournamentAssignment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentAssignments] DROP CONSTRAINT [FK_TournamentTournamentAssignment];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentAssignmentAssignment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentAssignments] DROP CONSTRAINT [FK_TournamentAssignmentAssignment];
GO
IF OBJECT_ID(N'[dbo].[FK_AssignmentEnrollmentTournamentAssignment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AssignmentEnrollments] DROP CONSTRAINT [FK_AssignmentEnrollmentTournamentAssignment];
GO
IF OBJECT_ID(N'[dbo].[FK_AssignmentEnrollmentSubmit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Submits] DROP CONSTRAINT [FK_AssignmentEnrollmentSubmit];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamAssignmentEnrollment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AssignmentEnrollments] DROP CONSTRAINT [FK_TeamAssignmentEnrollment];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamSubmit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Submits] DROP CONSTRAINT [FK_TeamSubmit];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Tournaments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tournaments];
GO
IF OBJECT_ID(N'[dbo].[Assignments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Assignments];
GO
IF OBJECT_ID(N'[dbo].[TournamentAssignments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentAssignments];
GO
IF OBJECT_ID(N'[dbo].[Teams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Teams];
GO
IF OBJECT_ID(N'[dbo].[Submits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Submits];
GO
IF OBJECT_ID(N'[dbo].[AssignmentEnrollments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AssignmentEnrollments];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Tournaments'
CREATE TABLE [dbo].[Tournaments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [CreateDateTime] datetime  NOT NULL
);
GO

-- Creating table 'Assignments'
CREATE TABLE [dbo].[Assignments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [FriendlyName] nvarchar(max)  NOT NULL,
    [Tagline] nvarchar(max)  NOT NULL,
    [FilePath] nvarchar(max)  NOT NULL,
    [CreateDateTime] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'TournamentAssignments'
CREATE TABLE [dbo].[TournamentAssignments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AssignmentOrder] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreateDateTime] datetime  NOT NULL,
    [Tournament_Id] int  NOT NULL,
    [Assignment_Id] int  NOT NULL
);
GO

-- Creating table 'Teams'
CREATE TABLE [dbo].[Teams] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [CreateDateTime] datetime  NOT NULL,
    [Score] int  NOT NULL,
    [AdminUser] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'Submits'
CREATE TABLE [dbo].[Submits] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SubmitDateTime] datetime  NOT NULL,
    [Status] int  NOT NULL,
    [TimeSinceEnrollment] datetimeoffset  NOT NULL,
    [ProcessingDetails] nvarchar(max)  NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [FileContents] nvarchar(max)  NOT NULL,
    [FileData] varbinary(max)  NOT NULL,
    [LastModified] datetime  NOT NULL,
    [AssignmentEnrollment_Id] int  NOT NULL,
    [Team_Id] int  NOT NULL
);
GO

-- Creating table 'AssignmentEnrollments'
CREATE TABLE [dbo].[AssignmentEnrollments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [TournamentAssignment_Id] int  NOT NULL,
    [Team_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Tournaments'
ALTER TABLE [dbo].[Tournaments]
ADD CONSTRAINT [PK_Tournaments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Assignments'
ALTER TABLE [dbo].[Assignments]
ADD CONSTRAINT [PK_Assignments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TournamentAssignments'
ALTER TABLE [dbo].[TournamentAssignments]
ADD CONSTRAINT [PK_TournamentAssignments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Teams'
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [PK_Teams]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Submits'
ALTER TABLE [dbo].[Submits]
ADD CONSTRAINT [PK_Submits]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AssignmentEnrollments'
ALTER TABLE [dbo].[AssignmentEnrollments]
ADD CONSTRAINT [PK_AssignmentEnrollments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Tournament_Id] in table 'TournamentAssignments'
ALTER TABLE [dbo].[TournamentAssignments]
ADD CONSTRAINT [FK_TournamentTournamentAssignment]
    FOREIGN KEY ([Tournament_Id])
    REFERENCES [dbo].[Tournaments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentTournamentAssignment'
CREATE INDEX [IX_FK_TournamentTournamentAssignment]
ON [dbo].[TournamentAssignments]
    ([Tournament_Id]);
GO

-- Creating foreign key on [Assignment_Id] in table 'TournamentAssignments'
ALTER TABLE [dbo].[TournamentAssignments]
ADD CONSTRAINT [FK_TournamentAssignmentAssignment]
    FOREIGN KEY ([Assignment_Id])
    REFERENCES [dbo].[Assignments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentAssignmentAssignment'
CREATE INDEX [IX_FK_TournamentAssignmentAssignment]
ON [dbo].[TournamentAssignments]
    ([Assignment_Id]);
GO

-- Creating foreign key on [TournamentAssignment_Id] in table 'AssignmentEnrollments'
ALTER TABLE [dbo].[AssignmentEnrollments]
ADD CONSTRAINT [FK_AssignmentEnrollmentTournamentAssignment]
    FOREIGN KEY ([TournamentAssignment_Id])
    REFERENCES [dbo].[TournamentAssignments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AssignmentEnrollmentTournamentAssignment'
CREATE INDEX [IX_FK_AssignmentEnrollmentTournamentAssignment]
ON [dbo].[AssignmentEnrollments]
    ([TournamentAssignment_Id]);
GO

-- Creating foreign key on [AssignmentEnrollment_Id] in table 'Submits'
ALTER TABLE [dbo].[Submits]
ADD CONSTRAINT [FK_AssignmentEnrollmentSubmit]
    FOREIGN KEY ([AssignmentEnrollment_Id])
    REFERENCES [dbo].[AssignmentEnrollments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AssignmentEnrollmentSubmit'
CREATE INDEX [IX_FK_AssignmentEnrollmentSubmit]
ON [dbo].[Submits]
    ([AssignmentEnrollment_Id]);
GO

-- Creating foreign key on [Team_Id] in table 'AssignmentEnrollments'
ALTER TABLE [dbo].[AssignmentEnrollments]
ADD CONSTRAINT [FK_TeamAssignmentEnrollment]
    FOREIGN KEY ([Team_Id])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamAssignmentEnrollment'
CREATE INDEX [IX_FK_TeamAssignmentEnrollment]
ON [dbo].[AssignmentEnrollments]
    ([Team_Id]);
GO

-- Creating foreign key on [Team_Id] in table 'Submits'
ALTER TABLE [dbo].[Submits]
ADD CONSTRAINT [FK_TeamSubmit]
    FOREIGN KEY ([Team_Id])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamSubmit'
CREATE INDEX [IX_FK_TeamSubmit]
ON [dbo].[Submits]
    ([Team_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------