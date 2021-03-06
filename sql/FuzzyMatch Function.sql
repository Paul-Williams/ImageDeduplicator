USE [Data.DataContext]
GO
/****** Object:  UserDefinedFunction [dbo].[FuzzyMatch]    Script Date: 01/10/2019 18:13:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[FuzzyMatch] 
(
	@P1 binary(256), @P2 binary(256)
)
RETURNS bit
AS
BEGIN

	if @P1 = @P2 return 1

	declare @threshold int = 20
	declare @maxDiffs int =  45
	DECLARE @ResultVar bit
	declare @diffs int = 0
	declare @Counter int = 1
	declare @ColumnLength int = 256
	--set @Counter = 1
	--set @ColumnLength = LEN(@P1)

	while (@Counter <= @ColumnLength) begin		
		if  ABS(CONVERT(INT,SUBSTRING(@P1, @Counter, 1)) - CONVERT(INT,SUBSTRING(@P2, @Counter, 1))) > @threshold set @diffs = @diffs + 1
		if @diffs > @maxDiffs return 0
		set @Counter = @Counter + 2
	end /* while */
	
	return 1
END
