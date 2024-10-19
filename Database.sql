CREATE DATABASE Online_Learning_Platform;
GO
USE Online_Learning_Platform;
GO

-- instructor table
CREATE TABLE Instructors (
    InstructorID nvarchar(450) PRIMARY KEY,
	[Description] TEXT,
    FOREIGN KEY (instructorID) REFERENCES AspNetUsers(Id) 
);

-- Notification
CREATE TABLE [Notification] (
    NotificationID INT PRIMARY KEY IDENTITY(1,1), 
    UserID nvarchar(450) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL,  
    CreatedAt DATETIME DEFAULT GETDATE() 
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(Id) 
);

-- InstructorConfirmation
CREATE TABLE InstructorConfirmation(
	 ConfirmationID INT PRIMARY KEY IDENTITY(1,1),
	 UserID nvarchar(450),
	 Certificatelink nvarchar(400),
	 [FileName] NVARCHAR(255),
	 SendDate DATETIME DEFAULT GETDATE(),
	 [Description] TEXT,
	 FOREIGN KEY (UserID) REFERENCES AspNetUsers(Id) ON DELETE CASCADE 
)
ALTER TABLE InstructorConfirmation
ADD [Description] TEXT;
GO
ALTER TABLE InstructorConfirmation
ADD SendDate DATETIME DEFAULT GETDATE();
GO

-- Category
CREATE TABLE Category(
	CategoryID INT PRIMARY KEY IDENTITY(1,1),
	FullName NVARCHAR(50),
    [Description] NVARCHAR(MAX)
)
INSERT INTO Category (FullName, [Description]) 
VALUES
    ('Programming', 'Courses related to programming and software development.'),
    ('Data Science', 'Courses focused on data analysis and machine learning.'),
    ('Web Development', 'Courses for building websites and web applications.'),
    ('Design', 'Courses for graphic design and multimedia.'); 

-- Course table
CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
	Title NVARCHAR(255),
	CourseCode NVARCHAR(20),
	[Description] NVARCHAR(MAX),
	CoverImagePath NVARCHAR(MAX),
	InstructorID NVARCHAR(450), -- instructor
    NumberOfStudents INT DEFAULT 0, 
	Price DECIMAL(10,2),
    CategoryID INT,
    [Level] NVARCHAR(50) CHECK (level IN ('Beginner', 'Intermediate', 'Advanced')),
    [Status] BIT NOT NULL DEFAULT 1, -- true: active, false: inactive
	CreateDate DATE,
	LastUpdate DATE,
	EndDate DATE,
	NumberOfRate INT DEFAULT 0,
	Rating FLOAT NULL,
	FOREIGN KEY (categoryID) REFERENCES Category(categoryID),  
    FOREIGN KEY (instructorID) REFERENCES Instructors(InstructorID) ON DELETE SET NULL  -- Foreign key to Instructor (userID from User)
);
alter table Courses
ADD Rating FLOAT NULL;

-- them bang material, 1 khoa hoc co nhieu material
CREATE TABLE CourseMaterials (
    MaterialID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT NOT NULL,
	[FileName] NVARCHAR(255),
    MaterialsLink NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE 
);

-- Payment table
CREATE TABLE Payment(
	PaymentID INT PRIMARY KEY IDENTITY(1,1),
	CourseID INT,
	StudentID NVARCHAR(450), -- student
	Amount DECIMAL(10,2),
	PaymentDate DATETIME DEFAULT GETDATE(), 
	[Status] NVARCHAR(30) CHECK (status IN ('Pending', 'Completed', 'Failed', 'Cancelled')),
	FOREIGN KEY (studentID) REFERENCES AspNetUsers(id),
	FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE 
);
ALTER TABLE Payment
ADD CONSTRAINT UC_Payment UNIQUE (StudentID, CourseID);
go

-- BookMark
CREATE TABLE Bookmark (
    BookmarkID INT IDENTITY(1,1) PRIMARY KEY,
    TotalCost MONEY,
    StudentID NVARCHAR(450),  
    CourseID INT,
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id), 
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE    
);

-- StudentCourses
CREATE TABLE StudentCourses (
    StudentCourseID INT PRIMARY KEY IDENTITY(1,1), 
    StudentID NVARCHAR(450),  
    CourseID INT,        
    Progress DECIMAL(5,2) CHECK (progress >= 0 AND progress <= 100),  -- Tiến độ học tập (% hoàn thành)
    CertificateStatus NVARCHAR(50) CHECK (certificateStatus IN ('Not Started', 'In Progress', 'Completed', 'Certified')), -- Trạng thái chứng chỉ
    EnrollmentDate DATETIME NOT NULL,
    CompletionDate DATETIME NULL,
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id), 
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);
ALTER TABLE StudentCourses
ADD CONSTRAINT UC_StudentCourses UNIQUE (StudentID, CourseID);
go

-- certificate table
CREATE TABLE [Certificate](
	CertificateID INT PRIMARY KEY IDENTITY(1,1),
	StudentID NVARCHAR(450), 
	CourseID INT,  
	EnrollmentDate DATETIME NOT NULL,
    CompletionDate DATETIME NOT NULL,
	FOREIGN KEY (studentID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (courseID) REFERENCES Courses(courseID)
)

-- Lecture table
CREATE TABLE Lecture (
    LectureID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT,  -- Foreign key to Courses
    Title NVARCHAR(255),
	[Description] NVARCHAR(255),
    UpLoadDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);

--completion
CREATE TABLE LectureCompletion (
    CompletionID INT PRIMARY KEY IDENTITY(1,1),
    UserID NVARCHAR(450),
    LectureID INT, 
    CompletionDate DATETIME, 
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (LectureID) REFERENCES Lecture(LectureID) ON DELETE CASCADE
);
DROP TABLE LectureCompletion

-- Course image and video;
CREATE TABLE LectureFiles (
    FileID INT PRIMARY KEY IDENTITY(1,1),
    LectureID INT,  
    [FileName] NVARCHAR(255),
	FileType NVARCHAR(255) CHECK (FileType IN ('Document', 'Video')),
    FilePath NVARCHAR(MAX),  
    UploadDate DATETIME DEFAULT GETDATE(),  
    FOREIGN KEY (lectureID) REFERENCES Lecture(lectureID) ON DELETE CASCADE  
);
alter table LectureFiles
add FileType NVARCHAR(255) CHECK (FileType IN ('Document', 'Video'));
go

--RequestTranfer
CREATE TABLE RequestTranfer (
    TranferID INT PRIMARY KEY IDENTITY(1,1),
    UserID NVARCHAR(450), -- Kích thước tùy thuộc vào kích thước của UserId trong AppUserModel
    BankName NVARCHAR(255) NOT NULL,
    AccountNumber NVARCHAR(255) NOT NULL,
	CreateAt datetime,
    FullName NVARCHAR(255) NOT NULL,
    MoneyNumber FLOAT NOT NULL,
    Status NVARCHAR(255), -- Tùy thuộc vào yêu cầu, có thể thay đổi kiểu dữ liệu
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(Id) -- Thay AspNetUsers với tên bảng của người dùng nếu khác
);
alter table RequestTranfer add CreateAt datetime 

-- Test table
CREATE TABLE Test (
    TestID INT PRIMARY KEY IDENTITY(1,1), 
    CourseID INT,  -- Foreign key to Courses
	Title NVARCHAR(255),
	[Description] NVARCHAR(255),
    StartTime DATETIME,
    EndTime DATETIME,
	NumberOfQuestion INT NOT NULL,
    [Status] NVARCHAR(255),
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);
<<<<<<< HEAD

Alter table Test
Add Description NVARCHAR(255)
Alter table Test
Add Title NVARCHAR(255)
=======
Alter table Test
Add Description NVARCHAR(255);
Alter table Test
Add Title NVARCHAR(255);
>>>>>>> 769c5be67e4be216db31656986825dd96a4f4857

-- Score table
CREATE TABLE Score (
    studentID NVARCHAR(450) NOT NULL,
    testID INT NOT NULL,
    score FLOAT NOT NULL,
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (testID) REFERENCES Test(testID) ON DELETE CASCADE  
);
-- Question table
CREATE TABLE Question (
    questionID INT PRIMARY KEY IDENTITY(1,1),
    testID INT NOT NULL,
    question NVARCHAR(255) NOT NULL,
	ImagePath NVARCHAR(MAX),
	[Description] NVARCHAR(255),
	Title NVARCHAR(255),
    answerA NVARCHAR(255) NOT NULL,
    answerB NVARCHAR(255),
    answerC NVARCHAR(255),
    answerD NVARCHAR(255),
    correctAnswer NVARCHAR(255),
    FOREIGN KEY (testID) REFERENCES Test(testID) ON DELETE CASCADE  
);
ALTER TABLE Question
ADD ImagePath NVARCHAR(MAX)
Alter table Question
Add Description NVARCHAR(255);
Alter table Question
Add Title NVARCHAR(255);
Alter table Question
Add ImagePath NVARCHAR(MAX);
go

-- Assignment table
CREATE TABLE Assignment (
    AssignmentID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT,  -- Foreign key to Courses
    Title NVARCHAR(255),
	AssignmentLink NVARCHAR(MAX),
    [Description] TEXT,
    DueDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);
alter table Assignment add AssignmentLink  NVARCHAR(MAX);
go  

CREATE TABLE ScoreAssignment (
	ScoreAssignmentID INT PRIMARY KEY IDENTITY(1,1),
    studentID NVARCHAR(450) NOT NULL,
    AssignmentID INT NOT NULL,
	score FLOAT NOT NULL,
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (AssignmentID) REFERENCES Assignment(AssignmentID) ON DELETE CASCADE  
);
insert into Assignment(CourseID, Title, Description, DueDate)
values (9,'assignment 1','abcde',GETDATE())

-- Submission table
CREATE TABLE Submission (
    SubmissionID INT PRIMARY KEY IDENTITY(1,1),
    AssignmentID INT,  -- Foreign key to Assignment
    StudentID NVARCHAR(450),  -- Foreign key to Student (userID from Users)
    SubmissionLink NVARCHAR(MAX),  -- Link to the file
    SubmissionDate DATETIME,
    FOREIGN KEY (assignmentID) REFERENCES Assignment(assignmentID),
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id)  -- References Users table
);
alter table Submission add FileName nvarchar(250)
-- Chatbox
CREATE TABLE ChatBox (
    ChatBoxID INT PRIMARY KEY IDENTITY(1,1),
    SenderID NVARCHAR(450),  -- Foreign key to Users
    ReceiverID NVARCHAR(450),  -- Foreign key to Users
    Title TEXT,
    FOREIGN KEY (senderID) REFERENCES AspNetUsers(Id),  -- Can be a Student or Instructor
    FOREIGN KEY (receiverID) REFERENCES AspNetUsers(Id),  -- Can be a Student or Instructor
);

-- Message table
CREATE TABLE [Message] (
    MessageID INT PRIMARY KEY IDENTITY(1,1),
    SenderID NVARCHAR(450),  -- Foreign key to Users
	ChatBoxID INT,
    Content TEXT,
	IsRead BIT DEFAULT 0,
    [Timestamp] DATETIME DEFAULT GETDATE(),
	FOREIGN KEY (chatBoxID) REFERENCES ChatBox(ChatBoxID) ON DELETE CASCADE,
    FOREIGN KEY (senderID) REFERENCES AspNetUsers(Id),  -- Can be a Student or Instructor
);

--MessageFile
CREATE TABLE MessageFile (
    FileID INT PRIMARY KEY IDENTITY(1,1),
    MessageID INT,  
    [FileName] NVARCHAR(255),  
    FilePath NVARCHAR(MAX),  
    UploadDate DATETIME DEFAULT GETDATE(),  
    FOREIGN KEY (messageID) REFERENCES [Message](messageID) ON DELETE CASCADE  
);

-- Comment table
CREATE TABLE Comment (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    LectureID INT NULL,  -- Foreign key to Lecture (nullable for course-level comments)
    UserID NVARCHAR(450),  -- Foreign key to Users (can be Student or Instructor)
    Content TEXT,
    [Timestamp] DATETIME,
    ParentCmtId INT NULL,  -- Foreign key to Comment (nullable for root comments)
    FOREIGN KEY (lectureID) REFERENCES Lecture(lectureID),
    FOREIGN KEY (userID) REFERENCES AspNetUsers(Id),  -- Could be Student or Instructor
    FOREIGN KEY (parentCmtId) REFERENCES Comment(commentID)
);

-- comment file
CREATE TABLE CommentFile (
    FileID INT PRIMARY KEY IDENTITY(1,1),
    CommentID INT,  
    [FileName] NVARCHAR(255),  
    FilePath NVARCHAR(MAX),  
    UploadDate DATETIME DEFAULT GETDATE(),  
    FOREIGN KEY (commentID) REFERENCES Comment(commentID) ON DELETE CASCADE  
);

-- Course review table
CREATE TABLE Review (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT,
    StudentID NVARCHAR(450),
    Rating FLOAT CHECK (rating >= 0 AND rating <= 5),
    Comment NVARCHAR(255),
    ReviewDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID),
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id) ON DELETE CASCADE
);
GO

CREATE TRIGGER trg_DeleteChildComments
ON Comment
INSTEAD OF DELETE
AS
BEGIN
    WITH comments_to_delete AS (
        SELECT commentID
        FROM DELETED
        UNION ALL
        SELECT c.commentID
        FROM Comment c
        INNER JOIN comments_to_delete p ON c.parentCmtId = p.commentID
    )
    DELETE FROM Comment
    WHERE commentID IN (SELECT commentID FROM comments_to_delete);
END;

INSERT INTO Category (FullName, [Description]) 
VALUES
    ('Programming', 'Courses related to programming and software development.'),
    ('Data Science', 'Courses focused on data analysis and machine learning.'),
    ('Web Development', 'Courses for building websites and web applications.'),
    ('Design', 'Courses for graphic design and multimedia.'); 
INSERT INTO Instructors(InstructorID, [Description]) 
VALUES
    ('d4a32111-743b-4125-b949-3fe37bfb7eae', 'Courses related to programming and software development.')
INSERT INTO Courses 
    (Title, CourseCode, [Description], CoverImagePath, InstructorID, NumberOfStudents, Price, CategoryID, [Level], [Status], CreateDate, LastUpdate, EndDate, NumberOfRate) 
VALUES
    -- Programming Category
    ('Python for Beginners', 'CS101', 'Learn Python programming from scratch.', 'cover/python.jpg', '039f2d18-90fe-412c-89a4-038d55908b65', 0, 100.00, 1, 'Beginner', 1, '2024-09-01', '2024-09-20', '2024-12-20', 0),
    ('Java Advanced Techniques', 'CS102', 'Explore advanced Java programming concepts.', 'cover/java.jpg', '039f2d18-90fe-412c-89a4-038d55908b65', 0, 120.00, 1, 'Advanced', 1, '2024-09-05', '2024-09-21', '2024-12-20', 0),
    ('C# for Beginners', 'CS103', 'Introduction to C# programming.', 'cover/csharp.jpg', '039f2d18-90fe-412c-89a4-038d55908b65', 0, 90.00, 1, 'Beginner', 1, '2024-09-10', '2024-09-22', '2024-12-20', 0),
    ('Full-Stack Development with Node.js', 'CS104', 'Learn full-stack development using Node.js.', 'cover/nodejs.jpg', '039f2d18-90fe-412c-89a4-038d55908b65', 0, 150.00, 1, 'Intermediate', 1, '2024-09-15', '2024-09-23', '2024-12-20', 0),
    ('Introduction to Algorithms', 'CS105', 'Learn about algorithms and data structures.', 'cover/algorithms.jpg', '039f2d18-90fe-412c-89a4-038d55908b65', 0, 110.00, 1, 'Advanced', 1, '2024-09-20', '2024-09-24', '2024-12-20', 0),

    -- Data Science Category
    ('Data Analysis with R', 'DS101', 'Learn data analysis using R programming.', 'cover/data_analysis.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 130.00, 2, 'Beginner', 1, '2024-09-01', '2024-09-20', '2024-12-20', 0),
    ('Machine Learning A-Z', 'DS102', 'Master machine learning algorithms and techniques.', 'cover/machine_learning.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 200.00, 2, 'Advanced', 1, '2024-09-05', '2024-09-21', '2024-12-20', 0),
    ('Data Science for Everyone', 'DS103', 'Introduction to data science concepts.', 'cover/data_science.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 100.00, 2, 'Beginner', 1, '2024-09-10', '2024-09-22', '2024-12-20', 0),
    ('Deep Learning with TensorFlow', 'DS104', 'Explore deep learning with TensorFlow.', 'cover/deep_learning.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 150.00, 2, 'Intermediate', 1, '2024-09-15', '2024-09-23', '2024-12-20', 0),
    ('Statistics for Data Science', 'DS105', 'Learn statistics for data science applications.', 'cover/statistics.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 120.00, 2, 'Intermediate', 1, '2024-09-20', '2024-09-24', '2024-12-20', 0),

    -- Web Development Category
    ('HTML & CSS for Beginners', 'WD101', 'Learn the basics of web development with HTML & CSS.', 'cover/html_css.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 80.00, 3, 'Beginner', 1, '2024-09-01', '2024-09-20', '2024-12-20', 0),
    ('JavaScript Essentials', 'WD102', 'Get started with JavaScript for web development.', 'cover/javascript.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 100.00, 3, 'Beginner', 1, '2024-09-05', '2024-09-21', '2024-12-20', 0),
    ('React for Beginners', 'WD103', 'Build user interfaces with React.', 'cover/react.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 150.00, 3, 'Intermediate', 1, '2024-09-10', '2024-09-22', NULL, 0),
    ('Node.js for Beginners', 'WD104', 'Learn how to build web applications using Node.js.', 'cover/nodejs_web.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 120.00, 3, 'Intermediate', 1, '2024-09-15', '2024-09-23', '2024-12-20', 0),
    ('Advanced CSS Techniques', 'WD105', 'Explore advanced techniques in CSS for modern web design.', 'cover/advanced_css.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 130.00, 3, 'Advanced', 1, '2024-09-20', '2024-09-24', '2024-12-20', 0),

    -- Design Category
    ('Graphic Design Basics', 'D101', 'Learn the principles of graphic design.', 'cover/graphic_design.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 90.00, 4, 'Beginner', 1, '2024-09-01', '2024-09-20', '2024-12-20', 0),
    ('UI/UX Design Fundamentals', 'D102', 'Get started with UI/UX design principles.', 'cover/uiux_design.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 100.00, 4, 'Intermediate', 1, '2024-09-05', '2024-09-21', '2024-12-20', 0),
    ('Adobe Photoshop for Beginners', 'D103', 'Learn the basics of Adobe Photoshop.', 'cover/photoshop.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 120.00, 4, 'Beginner', 1, '2024-09-10', '2024-09-22', '2024-12-20', 0),
    ('Illustration for Designers', 'D104', 'Master illustration techniques for graphic design.', 'cover/illustration.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 130.00, 4, 'Advanced', 1, '2024-09-15', '2024-09-23', '2024-12-20', 0),
    ('Motion Graphics Basics', 'D105', 'Introduction to motion graphics using After Effects.', 'cover/motion_graphics.jpg', '9fc33dde-075d-4ede-87a7-c89e0564887a', 0, 150.00, 4, 'Advanced', 1, '2024-09-20', '2024-09-24', '2024-12-20', 0);
	
	
	-- sua id instructor
SELECT * FROM Courses;
Insert into Instructors(InstructorID, Description)
values
('039f2d18-90fe-412c-89a4-038d55908b65','Heloo student');
go

CREATE TRIGGER trg_UpdateNumberOfRates
ON Review
AFTER INSERT
AS
BEGIN
    UPDATE Courses
    SET NumberOfRate = (
        SELECT COUNT(*)
        FROM Review
        WHERE Review.CourseID = Courses.CourseID
    )
    WHERE CourseID IN (
        SELECT DISTINCT CourseID
        FROM inserted
    );
END;
GO

CREATE TRIGGER trg_UpdateNumberOfRates_Delete
ON Review
AFTER DELETE
AS
BEGIN
    UPDATE Courses
    SET NumberOfRate = (
        SELECT COUNT(*)
        FROM Review
        WHERE Review.CourseID = Courses.CourseID
    )
    WHERE CourseID IN (
        SELECT DISTINCT CourseID
        FROM deleted
    );
END;
GO

INSERT INTO StudentCourses (StudentID, CourseID, Progress, CertificateStatus, EnrollmentDate)
VALUES ('5e9bf460-cbe6-4f0b-b1dd-1c7d817bfffc', 2, 0, 'Not Started', GETDATE());

INSERT INTO StudentCourses (StudentID, CourseID, Progress, CertificateStatus, EnrollmentDate)
VALUES ('a3f1464d-21b0-4354-b4f9-ce357e0ae4c5', 2, 0, 'Not Started', GETDATE());

INSERT INTO Review (CourseID, StudentID, Rating, Comment, ReviewDate)
VALUES ('2', '5e9bf460-cbe6-4f0b-b1dd-1c7d817bfffc', 5, 'comment_value', GETDATE());

INSERT INTO Review (CourseID, StudentID, Rating, Comment, ReviewDate)
VALUES ('2', 'a3f1464d-21b0-4354-b4f9-ce357e0ae4c5', 4, 'nice', GETDATE());

ALTER TABLE Courses
ADD Rating FLOAT NULL;
go

CREATE TRIGGER UpdateCourseRating
ON Review
AFTER INSERT
AS
BEGIN
    -- Cập nhật Rating cho từng CourseID trong bảng Review
    UPDATE Courses
    SET Rating = (
        SELECT AVG(Rating)
        FROM Review
        WHERE CourseID = Courses.CourseID
    )
    WHERE CourseID IN (SELECT CourseID FROM inserted);
END;

INSERT INTO Lecture (CourseID, Title, [Description], UpLoadDate) VALUES
(1, 'Introduction to Programming', 'An introductory lecture on programming concepts.', GETDATE()),
(1, 'Data Structures', 'An overview of basic data structures.', GETDATE()),
(1, 'Introduction to Algorithms', 'Basic algorithms in programming.', GETDATE()),
(1, 'Object-Oriented Programming', 'Concepts of object-oriented programming.', GETDATE()),
(1, 'String Manipulation', 'How to manipulate strings in programming.', GETDATE()),
(1, 'Introduction to Databases', 'Basic concepts of databases.', GETDATE()),
(1, 'SQL Basics', 'Guide to basic SQL commands.', GETDATE()),
(1, 'Building Web Applications', 'Steps to build web applications.', GETDATE()),
(1, 'Software Testing', 'Methods of software testing.', GETDATE()),
(1, 'Application Deployment', 'Guide to deploying applications on a server.', GETDATE());
go

ALTER TABLE Test
ADD NumberOfQuestion INT NOT NULL;
GO

CREATE TRIGGER trg_UpdateNumberOfQuestion
ON Question
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE t
    SET t.NumberOfQuestion = q.CountOfQuestions
    FROM Test t
    INNER JOIN (
        SELECT testID, COUNT(*) AS CountOfQuestions
        FROM Question
        GROUP BY testID
    ) q ON t.TestID = q.testID;
END;

INSERT INTO Payment (CourseID, StudentID, Amount, [Status])
VALUES (3, '5e9bf460-cbe6-4f0b-b1dd-1c7d817bfffc', 150.00, 'Pending');

SELECT 
    CONSTRAINT_NAME, 
    TABLE_NAME 
FROM 
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE 
    CONSTRAINT_TYPE = 'FOREIGN KEY';

	ALTER TABLE [AspNetUsers] ADD [WalletUser] float NULL DEFAULT 0.0;

go

ALTER TABLE AspNetUsers
ALTER COLUMN ProfileImagePath NVARCHAR(MAX);

ALTER TABLE InstructorConfirmation 
ALTER COLUMN Certificatelink NVARCHAR(MAX);

ALTER TABLE Courses 
ALTER COLUMN CoverImagePath NVARCHAR(MAX);

ALTER TABLE CourseMaterials 
ALTER COLUMN MaterialsLink NVARCHAR(MAX);

ALTER TABLE Assignment 
ALTER COLUMN AssignmentLink NVARCHAR(MAX);

ALTER TABLE CommentFile 
ALTER COLUMN FilePath NVARCHAR(MAX);

ALTER TABLE LectureFiles 
ALTER COLUMN FilePath NVARCHAR(MAX);

ALTER TABLE MessageFile 
ALTER COLUMN FilePath NVARCHAR(MAX);

Alter table Question
Add ImagePath NVARCHAR(MAX);

Alter table CourseMaterials
Add [FileName] NVARCHAR(255);

Alter table InstructorConfirmation
Add [FileName] NVARCHAR(255);