--Part 1

INT Id
LONGTEXT Name
INT EmployerId

--Part 2

SELECT Name FROM Employers WHERE Location LIKE "St. Louis City";

--Part 3

SELECT DISTINCT Name, Description
FROM Skills LEFT JOIN JobSkills ON Skills.Id = JobSkills.SkillId
WHERE SkillId IS NOT NULL ORDER BY Name ASC;
