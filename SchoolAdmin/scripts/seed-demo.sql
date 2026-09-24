/*
  School Admin - demo data
  -------------------------------------------------------------
  Adds sample classes, subjects, staff, students and two extra
  logins so the app can be tested and demonstrated.

  Safe to run more than once: rows that already exist are skipped.

  Start the app once first so it creates the tables, then run:
    sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d SchoolAdmin -i scripts\seed-demo.sql

  Demo logins added (NEVER run this on a real school database):
    accountant / Accountant@123   (Accountant)
    examstaff  / Exam@123         (Exam Staff)
    teacher    / Teacher@123      (Teacher - Meena Sundaram, EMP002)
    student    / Student@123      (Student - Surya Venkatesan, Class 5, ADM2026005)
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

------------------------------------------------------------
-- Classes: LKG, UKG, Class 1 - 12 (section A)
------------------------------------------------------------
DECLARE @classes TABLE (Name nvarchar(30), DisplayOrder int);
INSERT INTO @classes VALUES
 ('LKG', 1), ('UKG', 2), ('Class 1', 3), ('Class 2', 4), ('Class 3', 5), ('Class 4', 6),
 ('Class 5', 7), ('Class 6', 8), ('Class 7', 9), ('Class 8', 10), ('Class 9', 11),
 ('Class 10', 12), ('Class 11', 13), ('Class 12', 14);

INSERT INTO Classes (Name, Section, DisplayOrder)
SELECT c.Name, 'A', c.DisplayOrder
FROM @classes c
WHERE NOT EXISTS (SELECT 1 FROM Classes x WHERE x.Name = c.Name AND x.Section = 'A');

------------------------------------------------------------
-- Subjects
------------------------------------------------------------
DECLARE @subjects TABLE (Name nvarchar(50), Code nvarchar(10), DisplayOrder int);
INSERT INTO @subjects VALUES
 ('Tamil', 'TAM', 1), ('English', 'ENG', 2), ('Mathematics', 'MAT', 3),
 ('Science', 'SCI', 4), ('Social Science', 'SOC', 5), ('Computer Science', 'CS', 6);

INSERT INTO Subjects (Name, Code, DisplayOrder)
SELECT s.Name, s.Code, s.DisplayOrder
FROM @subjects s
WHERE NOT EXISTS (SELECT 1 FROM Subjects x WHERE x.Name = s.Name);

------------------------------------------------------------
-- Staff: 10 teachers + 2 office staff
------------------------------------------------------------
DECLARE @staff TABLE (Code nvarchar(20), FullName nvarchar(100), Designation nvarchar(50),
                      Phone nvarchar(15), Email nvarchar(100), JoiningDate date);
INSERT INTO @staff VALUES
 ('EMP001', 'Lakshmi Narayanan', 'Principal',            '9840011001', 'principal@school.local',  '2012-06-01'),
 ('EMP002', 'Meena Sundaram',    'Tamil Teacher',        '9840011002', 'meena.s@school.local',    '2015-06-03'),
 ('EMP003', 'Rajesh Kumar',      'English Teacher',      '9840011003', 'rajesh.k@school.local',   '2016-06-01'),
 ('EMP004', 'Priya Venkatesh',   'Mathematics Teacher',  '9840011004', 'priya.v@school.local',    '2017-06-05'),
 ('EMP005', 'Suresh Babu',       'Science Teacher',      '9840011005', 'suresh.b@school.local',   '2014-06-02'),
 ('EMP006', 'Kavitha Ramesh',    'Social Science Teacher','9840011006','kavitha.r@school.local',  '2018-06-04'),
 ('EMP007', 'Arun Prakash',      'Computer Teacher',     '9840011007', 'arun.p@school.local',     '2019-06-03'),
 ('EMP008', 'Deepa Krishnan',    'Primary Teacher',      '9840011008', 'deepa.k@school.local',    '2020-06-01'),
 ('EMP009', 'Senthil Murugan',   'Physical Education',   '9840011009', 'senthil.m@school.local',  '2016-06-06'),
 ('EMP010', 'Revathi Balaji',    'Primary Teacher',      '9840011010', 'revathi.b@school.local',  '2021-06-02'),
 ('EMP011', 'Ganesh Raman',      'Accountant',           '9840011011', 'accounts@school.local',   '2013-04-01'),
 ('EMP012', 'Saranya Mohan',     'Office Clerk',         '9840011012', 'office@school.local',     '2022-04-04');

INSERT INTO Staff (EmployeeCode, FullName, Designation, Phone, Email, JoiningDate, IsActive)
SELECT s.Code, s.FullName, s.Designation, s.Phone, s.Email, s.JoiningDate, 1
FROM @staff s
WHERE NOT EXISTS (SELECT 1 FROM Staff x WHERE x.EmployeeCode = s.Code);

------------------------------------------------------------
-- Students: 60 students, 6 per class across Class 1 - 10
------------------------------------------------------------
DECLARE @boys TABLE (i int IDENTITY(0,1), Name nvarchar(50));
INSERT INTO @boys (Name) VALUES
 ('Arjun'), ('Karthik'), ('Vignesh'), ('Harish'), ('Surya'), ('Pranav'), ('Rahul'), ('Dinesh'),
 ('Gokul'), ('Naveen'), ('Aakash'), ('Bharath'), ('Sanjay'), ('Varun'), ('Hari'), ('Ashwin'),
 ('Manoj'), ('Vishal'), ('Kishore'), ('Adithya'), ('Rohit'), ('Siddharth'), ('Tarun'), ('Yuvan'),
 ('Nithin'), ('Mukesh'), ('Ajay'), ('Santhosh'), ('Lokesh'), ('Praveen');

DECLARE @girls TABLE (i int IDENTITY(0,1), Name nvarchar(50));
INSERT INTO @girls (Name) VALUES
 ('Divya'), ('Keerthana'), ('Swathi'), ('Harini'), ('Nandhini'), ('Aishwarya'), ('Priyanka'), ('Sneha'),
 ('Janani'), ('Pooja'), ('Anjali'), ('Kavya'), ('Monisha'), ('Varsha'), ('Deepika'), ('Shalini'),
 ('Ramya'), ('Sangeetha'), ('Bhavya'), ('Lavanya'), ('Madhu'), ('Nivetha'), ('Oviya'), ('Pavithra'),
 ('Revathi'), ('Sowmya'), ('Thenmozhi'), ('Vaishnavi'), ('Yamini'), ('Abinaya');

DECLARE @fathers TABLE (i int IDENTITY(0,1), Name nvarchar(50));
INSERT INTO @fathers (Name) VALUES
 ('Ramesh'), ('Kumar'), ('Selvam'), ('Murugan'), ('Srinivasan'), ('Venkatesan'), ('Rajendran'),
 ('Balasubramanian'), ('Chandrasekar'), ('Ganesan'), ('Palani'), ('Saravanan'), ('Sivakumar'),
 ('Mohan'), ('Anand'), ('Elango'), ('Jayaraman'), ('Natarajan'), ('Prabhu'), ('Thirumalai');

DECLARE @areas TABLE (i int IDENTITY(0,1), Name nvarchar(50));
INSERT INTO @areas (Name) VALUES
 ('Anna Nagar'), ('T. Nagar'), ('Velachery'), ('Tambaram'), ('Adyar'), ('Mylapore'),
 ('Porur'), ('Chromepet'), ('Guindy'), ('Perambur');

DECLARE @k int = 1;
WHILE @k <= 60
BEGIN
    DECLARE @classNo int   = ((@k - 1) % 10) + 1;                -- Class 1 .. 10
    DECLARE @round int     = (@k - 1) / 10;                      -- 0 .. 5 (one student per class per round)
    DECLARE @isBoy bit     = CASE WHEN @round % 2 = 0 THEN 1 ELSE 0 END;   -- 3 boys + 3 girls per class
    DECLARE @nameIdx int   = (@round / 2) * 10 + (@k - 1) % 10;  -- 0 .. 29, unique per gender
    DECLARE @father nvarchar(50) = (SELECT Name FROM @fathers WHERE i = (@k * 13 + @round) % 20);
    DECLARE @area nvarchar(50)   = (SELECT Name FROM @areas WHERE i = (@k * 7 + @round) % 10);
    DECLARE @first nvarchar(50)  = CASE WHEN @isBoy = 1
                                        THEN (SELECT Name FROM @boys WHERE i = @nameIdx)
                                        ELSE (SELECT Name FROM @girls WHERE i = @nameIdx) END;
    DECLARE @admNo nvarchar(20)  = CONCAT('ADM2026', RIGHT(CONCAT('000', @k), 3));
    DECLARE @classId int = (SELECT Id FROM Classes WHERE Name = CONCAT('Class ', @classNo) AND Section = 'A');
    -- Age = class + 5, with birthdays spread across the year
    DECLARE @dob date = DATEADD(DAY, (@k * 37) % 365, DATEFROMPARTS(2026 - (@classNo + 6), 1, 1));

    IF NOT EXISTS (SELECT 1 FROM Students WHERE AdmissionNo = @admNo)
        INSERT INTO Students (AdmissionNo, FirstName, LastName, DateOfBirth, Gender, SchoolClassId,
                              GuardianName, GuardianPhone, Address, AdmissionDate, IsActive)
        VALUES (@admNo, @first, @father, @dob,
                CASE WHEN @isBoy = 1 THEN 'Male' ELSE 'Female' END,
                @classId,
                CONCAT(@father, ' ', LEFT(@first, 1)),
                CONCAT('98', RIGHT(CONCAT('00000000', 41000000 + @k * 1373), 8)),
                CONCAT(10 + (@k * 13) % 190, ', ', (SELECT n FROM (VALUES ('Main Road'), ('Gandhi Street'), ('Nehru Street'), ('Kamarajar Salai'), ('2nd Cross Street')) v(n) ORDER BY n OFFSET ((@k + @round) % 5) ROWS FETCH NEXT 1 ROWS ONLY), ', ', @area, ', Chennai'),
                DATEFROMPARTS(2026 - (@k % 5), 6, 1 + (@k % 10)),
                1);

    SET @k += 1;
END;

-- Roll numbers: alphabetical within each class, only for students without one
;WITH numbered AS (
    SELECT Id, RollNo,
           ROW_NUMBER() OVER (PARTITION BY SchoolClassId ORDER BY FirstName, LastName) AS rn
    FROM Students
)
UPDATE numbered SET RollNo = rn WHERE RollNo IS NULL;

------------------------------------------------------------
-- Demo logins (BCrypt hashes of the passwords shown above)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'accountant')
    INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES ('accountant', '$2a$11$5MPpKxK/efZF1gGecB8j5Oz3YCkXi7.o/HDXVyhZZjlyl0LPCzKJ.',
            'Ganesh Raman', 'Accountant', 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'examstaff')
    INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES ('examstaff', '$2a$11$pT8uvXhguXWmWBpV3hmANO4mpYIVFAWgSnFN0gzici3mTREasJGba',
            'Priya Venkatesh', 'ExamStaff', 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'teacher')
    INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive, CreatedAt, StaffId)
    SELECT 'teacher', '$2a$11$EwSC1i96vNabBsK4wYyRl.CLh3H/gOa3gj0LPuD0vPh0ENYqdv/j6',
           s.FullName, 'Teacher', 1, GETDATE(), s.Id
    FROM Staff s WHERE s.EmployeeCode = 'EMP002';

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'student')
    INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive, CreatedAt, StudentId)
    SELECT 'student', '$2a$11$JQ52iIsU0sW7LMX/fPOhfOhN83IebxH4aog5Alp6Hjdkmsshc1jxi',
           CONCAT(s.FirstName, ' ', s.LastName), 'Student', 1, GETDATE(), s.Id
    FROM Students s WHERE s.AdmissionNo = 'ADM2026005';

COMMIT TRANSACTION;

------------------------------------------------------------
-- Summary
------------------------------------------------------------
SELECT (SELECT COUNT(*) FROM Classes)  AS Classes,
       (SELECT COUNT(*) FROM Subjects) AS Subjects,
       (SELECT COUNT(*) FROM Staff)    AS Staff,
       (SELECT COUNT(*) FROM Students) AS Students,
       (SELECT COUNT(*) FROM Users)    AS Users;
