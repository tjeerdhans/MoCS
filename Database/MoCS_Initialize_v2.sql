USE MoCS

DELETE Submits
DELETE AssignmentEnrollments
DELETE TournamentAssignments
DELETE Tournaments
DELETE Assignments

SET IDENTITY_INSERT dbo.Tournaments ON

IF NOT EXISTS(SELECT * FROM dbo.Tournaments
	WHERE [Id] = 1)
	BEGIN
		INSERT INTO [dbo].[Tournaments]
           ([Id]
           ,[Name]
           ,[CreateDateTime])
     VALUES( 1, 'MoCS Tournament (Try Out)', GETDATE())
		
	END

IF NOT EXISTS(SELECT * FROM dbo.Tournaments
	WHERE [Id] = 2)
	BEGIN
		INSERT INTO [dbo].[Tournaments]
           ([Id]
           ,[Name]
           ,[CreateDateTime])
     VALUES( 2, 'MoCS Tournament', GETDATE())
		
	END
	
	SET IDENTITY_INSERT dbo.Tournaments OFF

SET IDENTITY_INSERT dbo.Assignments ON

IF NOT EXISTS(SELECT * FROM dbo.Assignments
	WHERE [Id] = 1)
	BEGIN
		INSERT INTO [dbo].Assignments
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[FilePath]
      ,[CreateDateTime])
     VALUES( 1, 'HelloWorld','Hello World','How hard can it be?','C:\Projects\MoCS\assignments\HelloWorld', GETDATE())
		
	END	
	
	
	IF NOT EXISTS(SELECT * FROM dbo.Assignments
	WHERE [Id] = 2)
	BEGIN
		INSERT INTO [dbo].Assignments
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[FilePath]
      ,[CreateDateTime])
     VALUES( 2, 'LoveCalculator','Love Calculator','Tough love','C:\Projects\MoCS\assignments\LoveCalculator', GETDATE())
		
	END	
	
		IF NOT EXISTS(SELECT * FROM dbo.Assignments
	WHERE [Id] = 3)
	BEGIN
		INSERT INTO [dbo].Assignments
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[FilePath]
      ,[CreateDateTime])
     VALUES( 3, 'FizzBuzz','FizzBuzz!','Fizz or Buzz or Both','C:\Projects\MoCS\assignments\FizzBuzz', GETDATE())
		
	END	
		
		IF NOT EXISTS(SELECT * FROM dbo.Assignments
	WHERE [Id] = 4)
	BEGIN
		INSERT INTO [dbo].Assignments
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[FilePath]
      ,[CreateDateTime])
     VALUES( 4, 'Postman','The Postman Never Walks Twice','Sorting Up and Down','C:\Projects\MoCS\assignments\Postman', GETDATE())
		
	END	
	
	
SET IDENTITY_INSERT dbo.Assignments OFF
	
	SET IDENTITY_INSERT dbo.TournamentAssignments ON

IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignments
	WHERE [Id] = 1)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignments]
           ([Id]
           ,[AssignmentOrder]
           ,[IsActive]
           ,[CreateDateTime]
           ,[Tournament_Id]
           ,[Assignment_Id])
           VALUES(1, 0,1,GETDATE(),1,1)
           
     END
           
           IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignments
	WHERE [Id] = 2)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignments]
           ([Id]
           ,[AssignmentOrder]
           ,[IsActive]
           ,[CreateDateTime]
           ,[Tournament_Id]
           ,[Assignment_Id])
           VALUES(2, 0, 1,GETDATE(),2,2)
           
     END
     
                IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignments
	WHERE [Id] = 3)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignments]
           ([Id]
           ,[AssignmentOrder]
           ,[IsActive]
           ,[CreateDateTime]
           ,[Tournament_Id]
           ,[Assignment_Id])
           VALUES(3, 1,1,GETDATE(),2,3)
           
     END
     
                     IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignments
	WHERE [Id] = 4)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignments]
           ([Id]
           ,[AssignmentOrder]
           ,[IsActive]
           ,[CreateDateTime]
           ,[Tournament_Id]
           ,[Assignment_Id])
           VALUES(4, 1, 1,GETDATE(),2,4)
           
     END
SET IDENTITY_INSERT dbo.TournamentAssignments OFF