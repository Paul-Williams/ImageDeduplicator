USE [Data.DataContext]
GO

/****** Object:  StoredProcedure [dbo].[FuzzyMatchBytes]    Script Date: 01/10/2019 20:37:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Return rows with fuzzy-match on bytes column
-- =============================================
CREATE PROCEDURE [dbo].[FuzzyMatchBytes] 
	@bytes binary(256)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select * from [Images]
	where dbo.FuzzyMatch(@bytes ,Bytes)=1


END
GO


