-- Found this kicking around in my documents folder.
-- May be the basis for one of the database stored procedures, not sure.
-- Included here, just in case.

select Id,[Path]
from [Images]
group by [Bytes]
having COUNT(*) >1

WITH cte AS
(SELECT *, ROW_NUMBER() OVER (PARTITION BY Bytes ORDER BY (SELECT 0)) AS DuplicateRowNumber
    FROM Images
)
select *  FROM cte WHERE DuplicateRowNumber > 1

SELECT * FROM Images WHERE id IN
    (SELECT * FROM Images GROUP BY Bytes HAVING COUNT(*) > 1)
   

	Select min(v.id) as RealID, v2.id as DuplicateId
From images v join images v2
    on v.Bytes= v2.Bytes
	and v.id <> v2.id
Group by v2.id


select count(*) Bytes from Images
group by Bytes
Having count (*) >1
