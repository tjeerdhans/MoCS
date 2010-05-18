
USE MoCS

DELETE dbo.Submit
DELETE dbo.AssignmentEnrollment
DELETE dbo.Team
DELETE TournamentAssignment
DELETE Tournament
DELETE Assignment

SET IDENTITY_INSERT dbo.Team ON

IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 1)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(1,'Team01','Team01','',0,0, GETDATE())
		
	END
	
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 2)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(2,'Team02','Team02','',0,0, GETDATE())
		
	END
	
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 3)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(3,'Team03','Team03','',0,0, GETDATE())
		
	END
	
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 4)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(4,'Team04','Team04','',0,0, GETDATE())
		
	END
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 5)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(5,'Team05','Team05','',0,0, GETDATE())
		
	END
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 6)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(6,'Team06','Team06','',0,0, GETDATE())
		
	END
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 7)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(7,'Team07','Team07','',0,0, GETDATE())
		
	END
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 8)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(8,'Team08','Team08','',0,0, GETDATE())
		
	END
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 9)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(9,'Team09','Team09','',0,0, GETDATE())
		
	END
	IF NOT EXISTS(SELECT * FROM dbo.Team
	WHERE [Id] = 10)
	BEGIN
		INSERT INTO [dbo].Team				   
           ([Id]
           ,[Name]
           ,[Password]
           ,[Members]
           ,[IsAdmin]
           ,[Score]
           ,[CreateDate])
     VALUES(10,'Team10','Team10','',0,0, GETDATE())
		
	END
	
SET IDENTITY_INSERT dbo.Team OFF

SET IDENTITY_INSERT dbo.Tournament ON

IF NOT EXISTS(SELECT * FROM dbo.Tournament
	WHERE [Id] = 1)
	BEGIN
		INSERT INTO [dbo].[Tournament]
           ([Id]
           ,[Name]
           ,[CreateDate])
     VALUES( 1, 'MoCS Tournament (Try Out)', GETDATE())
		
	END

IF NOT EXISTS(SELECT * FROM dbo.Tournament
	WHERE [Id] = 2)
	BEGIN
		INSERT INTO [dbo].[Tournament]
           ([Id]
           ,[Name]
           ,[CreateDate])
     VALUES( 2, 'MoCS Tournament', GETDATE())
		
	END
	
	SET IDENTITY_INSERT dbo.Tournament OFF

SET IDENTITY_INSERT dbo.Assignment ON

IF NOT EXISTS(SELECT * FROM dbo.Assignment
	WHERE [Id] = 1)
	BEGIN
		INSERT INTO [dbo].Assignment
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[Path]
      ,[CreateDate])
     VALUES( 1, 'HelloWorld','Hello World','How hard can it be?','C:\Projects\MoCS\assignments\HelloWorld', GETDATE())
		
	END	
	
	
	IF NOT EXISTS(SELECT * FROM dbo.Assignment
	WHERE [Id] = 2)
	BEGIN
		INSERT INTO [dbo].Assignment
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[Path]
      ,[CreateDate])
     VALUES( 2, 'LoveCalculator','Love Calculator','Tough love','C:\Projects\MoCS\assignments\LoveCalculator', GETDATE())
		
	END	
	
		IF NOT EXISTS(SELECT * FROM dbo.Assignment
	WHERE [Id] = 3)
	BEGIN
		INSERT INTO [dbo].Assignment
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[Path]
      ,[CreateDate])
     VALUES( 3, 'FizzBuzz','FizzBuzz!','Fizz or Buzz or Both','C:\Projects\MoCS\assignments\FizzBuzz', GETDATE())
		
	END	
		
		IF NOT EXISTS(SELECT * FROM dbo.Assignment
	WHERE [Id] = 4)
	BEGIN
		INSERT INTO [dbo].Assignment
           ([Id]
      ,[Name]
      ,[FriendlyName]
      ,[Tagline]
      ,[Path]
      ,[CreateDate])
     VALUES( 4, 'Postman','The Postman Never Walks Twice','Sorting Up and Down','C:\Projects\MoCS\assignments\Postman', GETDATE())
		
	END	
	
	
SET IDENTITY_INSERT dbo.Assignment OFF
	
	SET IDENTITY_INSERT dbo.TournamentAssignment ON

IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignment
	WHERE [Id] = 1)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignment]
           ([Id]
           ,[AssignmentOrder]
           ,[Points1]
           ,[Points2]
           ,[Points3]
           ,[IsActive]
           ,[CreateDate]
           ,[TournamentId]
           ,[AssignmentId])
           VALUES(1, 0, 0,0,0,1,GETDATE(),1,1)
           
     END
           
           IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignment
	WHERE [Id] = 2)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignment]
           ([Id]
           ,[AssignmentOrder]
           ,[Points1]
           ,[Points2]
           ,[Points3]
           ,[IsActive]
           ,[CreateDate]
           ,[TournamentId]
           ,[AssignmentId])
           VALUES(2, 0, 0,0,0,1,GETDATE(),2,2)
           
     END
     
                IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignment
	WHERE [Id] = 3)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignment]
           ([Id]
           ,[AssignmentOrder]
           ,[Points1]
           ,[Points2]
           ,[Points3]
           ,[IsActive]
           ,[CreateDate]
           ,[TournamentId]
           ,[AssignmentId])
           VALUES(3, 1, 0,0,0,1,GETDATE(),2,3)
           
     END
     
                     IF NOT EXISTS(SELECT * FROM dbo.TournamentAssignment
	WHERE [Id] = 4)
	BEGIN
	INSERT INTO [dbo].[TournamentAssignment]
           ([Id]
           ,[AssignmentOrder]
           ,[Points1]
           ,[Points2]
           ,[Points3]
           ,[IsActive]
           ,[CreateDate]
           ,[TournamentId]
           ,[AssignmentId])
           VALUES(4, 1, 0,0,0,1,GETDATE(),2,4)
           
     END
SET IDENTITY_INSERT dbo.TournamentAssignment OFF