declare @directory nvarchar(2000) = '';


select i.[Id], i.Path, fm.Id, fm.path 
from [Images] i
inner join [Images] fm

on i.Id < fm.id and dbo.FuzzyMatch(i.Bytes,fm.Bytes)=1

where 
 i.Path like @directory
	and
 fm.Path like @directory



 -- Trial / example code below:

--WITH matches ([iId],[iPath],[fmId], [fmPath])
--AS (

--select i.[Id], i.Path, fm.Id, fm.path 
--from [Images] i
--inner join [Images] fm

--on  dbo.FuzzyMatch(i.Bytes,fm.Bytes)=1
----i.Id < fm.id and
--where 
----i.Id < fm.id
---- and
-- i.Path like @directory
--	and
-- fm.Path like @directory
-- )

 --select * from matches

-- SELECT * FROM matches
--  EXCEPT
--SELECT * FROM matches WHERE iid < fmId;

 --select * from matches where fmId not in (select iId from matches) 

--select t1.* , t2.*
--from matches t1
--    left join matches t2
--       on t1.iId = t2.fmId
--       and t1.fmid = t2.iid
--where  t2.fmId



 -- This takes 1:47
-- WITH subset ([Id],[Bytes], [Path])
-- AS (SELECT [id], [bytes], [Path] FROM Images WHERE [Path] like @directory  )

--select i.[Id], i.Path, fm.Id, fm.path 
--from subset i
--inner join subset fm

--on dbo.FuzzyMatch(i.Bytes,fm.Bytes)=1

--where i.Id <> fm.id