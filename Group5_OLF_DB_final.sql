CREATE DATABASE Online_Learning_Platform;
GO
USE Online_Learning_Platform;
GO

--  ASP.NET Identity --
CREATE TABLE [AspNetUsers] (
    [Id] NVARCHAR(450) PRIMARY KEY,
    [UserName] NVARCHAR(256) NULL,
    [NormalizedUserName] NVARCHAR(256) NULL UNIQUE,
    [Email] NVARCHAR(256) NULL,
    [NormalizedEmail] NVARCHAR(256) NULL,
    [EmailConfirmed] BIT NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [SecurityStamp] NVARCHAR(MAX) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(15) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled] BIT NOT NULL,
    [LockoutEnd] DATETIMEOFFSET NULL,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,
    [Address] NVARCHAR(256) NULL,
    [Dob] DATE NULL,
    [Gender] NVARCHAR(50) NULL,
    [FirstName] NVARCHAR(50) NULL,
    [LastName] NVARCHAR(50) NULL,
    [WalletUser] DECIMAL(18, 2) NULL,
	[ProfileImagePath] NVARCHAR(MAX) NULL
);

CREATE TABLE [AspNetRoles] (
    [Id] NVARCHAR(450) PRIMARY KEY,
    [Name] NVARCHAR(256) NULL,
    [NormalizedName] NVARCHAR(256) NULL UNIQUE,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL
);
INSERT INTO [AspNetRoles] (Id, [Name], [NormalizedName], [ConcurrencyStamp])
VALUES 
    ('1', 'Admin', 'ADMIN', NEWID()),
    ('2', 'Student', 'STUDENT', NEWID()),
	('3', 'Instructor', 'INSTRUCTOR', NEWID());

CREATE TABLE [AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    PRIMARY KEY ([UserId], [RoleId]),
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [ProviderKey] NVARCHAR(450) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    PRIMARY KEY ([LoginProvider], [ProviderKey]),
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(450) NOT NULL,
    [Value] NVARCHAR(MAX) NULL,
    PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleId] NVARCHAR(450) NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);
--  ASP.NET Identity - end --

-- instructor table
CREATE TABLE Instructors (
    InstructorID nvarchar(450) PRIMARY KEY,
	[Description] NVARCHAR(255),
    FOREIGN KEY (instructorID) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Notification
CREATE TABLE [Notification] (
    NotificationID INT PRIMARY KEY IDENTITY(1,1), 
    UserID nvarchar(450) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL,  
    CreatedAt DATETIME DEFAULT GETDATE() 
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- InstructorConfirmation
CREATE TABLE InstructorConfirmation(
	 ConfirmationID INT PRIMARY KEY IDENTITY(1,1),
	 UserID nvarchar(450),
	 Certificatelink nvarchar(MAX),
	 [FileName] NVARCHAR(255),
	 SendDate DATETIME DEFAULT GETDATE(),
	 [Description] NVARCHAR(255),
	 FOREIGN KEY (UserID) REFERENCES AspNetUsers(Id) ON DELETE CASCADE 
)

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
	IsBaned BIT NOT NULL DEFAULT 0,
	CreateDate DATE,
	LastUpdate DATE,
	EndDate DATE,
	NumberOfRate INT DEFAULT 0,
	Rating FLOAT NULL,
	FOREIGN KEY (categoryID) REFERENCES Category(categoryID),  
    FOREIGN KEY (instructorID) REFERENCES Instructors(InstructorID) ON DELETE SET NULL  -- Foreign key to Instructor (userID from User)
);
alter table Courses
ADD IsBaned BIT NOT NULL DEFAULT 0;

-- them bang material, 1 khoa hoc co nhieu material
CREATE TABLE CourseMaterials (
    MaterialID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT NOT NULL,
	[FileName] NVARCHAR(255),
    MaterialsLink NVARCHAR(MAX) NOT NULL,
	FileExtension NVARCHAR(20),
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
	CONSTRAINT UC_Payment UNIQUE (StudentID, CourseID),
	FOREIGN KEY (studentID) REFERENCES AspNetUsers(id),
	FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE 
);

-- BookMark
CREATE TABLE Bookmark (
    BookmarkID INT IDENTITY(1,1) PRIMARY KEY,
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
    CertificateStatus NVARCHAR(50) CHECK (certificateStatus IN ('In Progress', 'Completed')), -- Trạng thái chứng chỉ
    EnrollmentDate DATETIME NOT NULL,
    CompletionDate DATETIME NULL,
	CONSTRAINT UC_StudentCourses UNIQUE (StudentID, CourseID),
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id), 
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);

-- certificate table
CREATE TABLE [Certificate](
	CertificateID INT PRIMARY KEY IDENTITY(1,1),
	StudentID NVARCHAR(450), 
	CourseID INT,  
    CompletionDate DATETIME NOT NULL,
	CertificateLink NVARCHAR(MAX),
	FOREIGN KEY (studentID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);

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
	CourseID INT,
    CompletionDate DATETIME, 
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (LectureID) REFERENCES Lecture(LectureID) ON DELETE CASCADE
);

-- Course image and video;
CREATE TABLE LectureFiles (
    FileID INT PRIMARY KEY IDENTITY(1,1),
    LectureID INT,  
    [FileName] NVARCHAR(255),
	FileType NVARCHAR(255) CHECK (FileType IN ('Document', 'Video')),
    FilePath NVARCHAR(MAX),  
	FileExtension NVARCHAR(20),
    UploadDate DATETIME DEFAULT GETDATE(),  
    FOREIGN KEY (lectureID) REFERENCES Lecture(lectureID) ON DELETE CASCADE  
);

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
	TestTime TIME(0),
	PassingScore FLOAT,
	NumberOfMaxAttempt INT,
	AlowRedo NVARCHAR(50) CHECK(AlowRedo In ('Yes', 'No')),
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);

-- Score table
CREATE TABLE Score (
	ScoreID INT PRIMARY KEY IDENTITY(1,1),
    studentID NVARCHAR(450) NOT NULL,
    testID INT NOT NULL,
    score FLOAT NOT NULL,
	DoTestAt Datetime,
	NumberOfAttempt INT,
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

-- Assignment table
CREATE TABLE Assignment (
    AssignmentID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT,  -- Foreign key to Courses
    Title NVARCHAR(255),
	AssignmentLink NVARCHAR(MAX),
	StartDate DATETIME,
    DueDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);

CREATE TABLE ScoreAssignment (
	ScoreAssignmentID INT PRIMARY KEY IDENTITY(1,1),
    studentID NVARCHAR(450) NOT NULL,
    AssignmentID INT NOT NULL,
	score FLOAT NOT NULL,
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (AssignmentID) REFERENCES Assignment(AssignmentID) ON DELETE CASCADE  
);

-- Submission table
CREATE TABLE Submission (
    SubmissionID INT PRIMARY KEY IDENTITY(1,1),
    AssignmentID INT,  -- Foreign key to Assignment
    StudentID NVARCHAR(450),  -- Foreign key to Student (userID from Users)
    SubmissionLink NVARCHAR(MAX),  -- Link to the file
	[FileName] varchar(250),
    SubmissionDate DATETIME,
    FOREIGN KEY (assignmentID) REFERENCES Assignment(assignmentID) ON DELETE CASCADE,
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id)  -- References Users table
);

-- Message table
CREATE TABLE [Message] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SenderId NVARCHAR(450),  -- Foreign key to Users
	ReceiverId NVARCHAR(450),
    Content TEXT,
    [Timestamp] DATETIME DEFAULT GETDATE(),
	FOREIGN KEY (ReceiverId) REFERENCES  AspNetUsers(Id),
    FOREIGN KEY (senderID) REFERENCES AspNetUsers(Id),  -- Can be a Student or Instructor
);

-- Comment table
CREATE TABLE Comment (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    LectureID INT NULL,  -- Foreign key to Lecture (nullable for course-level comments)
    UserID NVARCHAR(450),  -- Foreign key to Users (can be Student or Instructor)
    Content NVARCHAR(MAX),
    [Timestamp] DATETIME,
    ParentCmtId INT NULL,
	 -- Foreign key to Comment (nullable for root comments)
    FOREIGN KEY (lectureID) REFERENCES Lecture(lectureID) ON DELETE CASCADE,
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
    UserID NVARCHAR(450),
    Rating FLOAT CHECK (rating >= 0 AND rating <= 5),
    Comment NVARCHAR(255),
    ReviewDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID),
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(id) ON DELETE CASCADE
);
GO

CREATE TABLE Report (
    ReportID INT PRIMARY KEY IDENTITY(1,1), 
    UserID NVARCHAR(450),  
	[Subject] NVARCHAR(50), 
    Comment NVARCHAR(MAX), 
    FeedbackDate DATETIME DEFAULT GETDATE(),  
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(id) 
);
go

CREATE TABLE VideoCallInfo (
    VideoCallId INT PRIMARY KEY IDENTITY(1,1),
    SendID NVARCHAR(450) NOT NULL,
    ReceiveID NVARCHAR(450) NOT NULL,
	CreateAt Datetime,
    FOREIGN KEY (SendID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (ReceiveID) REFERENCES AspNetUsers(id)
);
go

CREATE TABLE LivestreamRecord 
(
	LivestreamRecordID INT PRIMARY KEY IDENTITY(1,1),
	UserID NVARCHAR(450),
	CreateDate DATETIME,
	UpdateDate DATETIME,
	CourseID INT,
	Title NVARCHAR(255),
	ScheduleStartTime DATETIME DEFAULT '2001-01-01 00:00:00',
	ScheduleLiveDuration TIME(0),
	FOREIGN KEY (UserID) REFERENCES AspNetUsers(id) ON DELETE CASCADE,
	FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
go

INSERT INTO Instructors (InstructorID, [Description])
VALUES 
	('instructor-user-id', 'Experienced Java and C# instructor specializing in advanced programming techniques.');

INSERT INTO Courses
    (Title, CourseCode, [Description], CoverImagePath, InstructorID, NumberOfStudents, Price, CategoryID, [Level], [Status], IsBaned, CreateDate, LastUpdate, EndDate, NumberOfRate, Rating) 
VALUES
    -- Programming Category
    ('Python for Beginners', 'CS101', 'Learn Python programming from scratch.', '/Images/cover/python.jpg', 'instructor-user-id', 0, 0.00, 1, 'Beginner', 1, 0, '2024-09-01', '2024-09-20', '2024-12-20', 1, 5),
    ('Java Advanced Techniques', 'CS102', 'Explore advanced Java programming concepts.', '/Images/cover/java.jpg', 'instructor-user-id', 0, 120000.00, 1, 'Advanced', 1, 0, '2024-09-05', '2024-09-21', '2024-12-20', 1, 5),
    ('C# for Beginners', 'CS103', 'Introduction to C# programming.', '/Images/cover/csharp.jpg', 'instructor-user-id', 0, 90000.00, 1, 'Beginner', 1, 0, '2024-09-10', '2024-09-22', '2024-12-20', 0, 0),
    -- Data Science Category
    ('Machine Learning A-Z', 'DS102', 'Master machine learning algorithms and techniques.', '/Images/cover/machine_learning.jpg', 'instructor-user-id', 0, 200000.00, 2, 'Advanced', 1, 0, '2024-09-05', '2024-09-21', '2024-12-20', 1, 5),
    ('Data Science for Everyone', 'DS103', 'Introduction to data science concepts.', '/Images/cover/data_science.jpg', 'instructor-user-id', 0, 100000.00, 2, 'Beginner', 1, 0, '2024-09-10', '2024-09-22', '2024-12-20', 1, 5),
    -- Web Development Category
    ('React for Beginners', 'WD103', 'Build user interfaces with React.', '/Images/cover/react.jpg', 'instructor-user-id', 0, 150000.00, 3, 'Intermediate', 1, 0, '2024-09-10', '2024-09-22', '2024-12-20', 1, 5),
    ('Node.js for Beginners', 'WD104', 'Learn how to build web applications using Node.js.', '/Images/cover/nodejs_web.jpg', 'instructor-user-id', 0, 120000.00, 3, 'Intermediate', 1, 0, '2024-09-15', '2024-09-23', '2024-12-20', 1, 5),
    ('Advanced CSS Techniques', 'WD105', 'Explore advanced techniques in CSS for modern web design.', '/Images/cover/advanced_css.jpg', 'instructor-user-id', 0, 130000.00, 3, 'Advanced', 1, 0, '2024-09-20', '2024-09-24', '2024-12-20', 1, 5),
	-- Design Category
    ('UI/UX Design Fundamentals', 'D102', 'Get started with UI/UX design principles.', '/Images/cover/uiux_design.jpg', 'instructor-user-id', 0, 100000.00, 4, 'Intermediate', 1, 0, '2024-09-05', '2024-09-21', '2024-12-20', 1, 5),
    ('Adobe Photoshop for Beginners', 'D103', 'Learn the basics of Adobe Photoshop.', '/Images/cover/photoshop.jpg', 'instructor-user-id', 0, 120000.00, 4, 'Beginner', 1, 0, '2024-09-10', '2024-09-22', '2024-12-20', 1, 5);

-- sua course id
-- lecture
-- Chèn bài giảng cho khóa 'Python for Beginners'
INSERT INTO Lecture (CourseID, Title, [Description], UpLoadDate)
VALUES
    (23, 'Introduction to Python', 'Learn the basics of Python programming.', '2024-09-01'),
    (23, 'Variables and Data Types', 'Understand variables and data types in Python.', '2024-09-02'),
    (23, 'Control Structures in Python', 'Learn about loops and conditionals in Python.', '2024-09-03'),
    (23, 'Functions and Modules', 'Explore functions and modules in Python.', '2024-09-04'),
    (23, 'Object-Oriented Programming in Python', 'Introduction to OOP in Python.', '2024-09-05'),

-- Chèn bài giảng cho khóa 'Java Advanced Techniques'
    (24, 'Advanced Java Concepts', 'Learn about advanced Java programming concepts.', '2024-09-06'),
    (24, 'Java Streams API', 'Work with the Streams API in Java.', '2024-09-07'),
    (24, 'Java Multithreading', 'Explore multithreading techniques in Java.', '2024-09-08'),
    (24, 'Java Design Patterns', 'Understand design patterns in Java.', '2024-09-09'),
    (24, 'Java Networking', 'Learn how to create network applications in Java.', '2024-09-10'),

-- Chèn bài giảng cho khóa 'C# for Beginners'
    (25, 'Introduction to C#', 'Learn the fundamentals of C# programming.', '2024-09-11'),
    (25, 'C# Data Types and Variables', 'Understand data types and variables in C#.', '2024-09-12'),

-- Chèn bài giảng cho khóa 'Machine Learning A-Z'
    (26, 'Introduction to Machine Learning', 'Learn the basics of machine learning.', '2024-09-06'),
    (26, 'Supervised Learning Algorithms', 'Explore supervised learning techniques.', '2024-09-07'),

-- Chèn bài giảng cho khóa 'Data Science for Everyone'
    (27, 'Introduction to Data Science', 'Learn the fundamentals of data science.', '2024-09-11'),
    (27, 'Data Preprocessing', 'Understand the process of data cleaning and preparation.', '2024-09-12'),

-- Chèn bài giảng cho khóa 'React for Beginners'
    (28, 'Introduction to React', 'Learn the basics of React.', '2024-09-13'),
    (28, 'State and Props in React', 'Explore state and props in React.', '2024-09-14'),

-- Chèn bài giảng cho khóa 'Node.js for Beginners'
    (29, 'Introduction to Node.js', 'Learn the basics of Node.js.', '2024-09-16'),

-- Chèn bài giảng cho khóa 'Advanced CSS Techniques'
    (30, 'Advanced CSS Selectors', 'Learn advanced CSS selector techniques.', '2024-09-21'),

-- Chèn bài giảng cho khóa 'UI/UX Design Fundamentals'
    (31, 'Introduction to UI/UX Design', 'Learn the principles of UI/UX design.', '2024-09-06'),

-- Chèn bài giảng cho khóa 'Adobe Photoshop for Beginners'
    (32, 'Introduction to Adobe Photoshop', 'Learn the basics of Adobe Photoshop.', '2024-09-12');
go

-- file
-- sua lecture id
INSERT INTO LectureFiles (LectureID, FileName, FileType, FilePath, FileExtension, UploadDate)
VALUES
-- Thêm file cho bài giảng 'Introduction to Python'
    (1, 'Introduction_to_Python_Notes', 'Document', '/Files/Python/Intro_Python_Notes.pdf', '.pdf' , '2024-09-16'),
    (1, 'Introduction_to_Python_Video', 'Video', '/Files/Python/Intro_Python_Video.mp4', '.mp4', '2024-09-16'),
-- Thêm file cho bài giảng 'Variables and Data Types'
    (2, 'Variables_and_Data_Types_Notes', 'Document', '/Files/Python/Variables_and_Data_Types.pdf', '.pdf', '2024-09-16'),
    (2, 'Variables_and_Data_Types_Video', 'Video', '/Files/Python/Variables_and_Data_Types.mp4', '.mp4', '2024-09-16'),
-- Thêm file cho bài giảng 'Control Structures in Python'
    (3, 'Control_Structures_Notes', 'Document', '/Files/Python/Control_Structures_Notes.pdf', '.pdf', '2024-09-16'),
	(3, 'Control_Structures_Notes_2', 'Document', '/Files/Python/Control_Structures_Notes.pdf', '.pdf', '2024-09-16'),
-- Thêm file cho bài giảng 'Functions and Modules'
    (4, 'Functions_and_Modules_Video', 'Video', '/Files/Python/Functions_and_Modules.mp4', '.mp4', '2024-09-16'),
-- Thêm file cho bài giảng 'Object-Oriented Programming in Python'
    (5, 'OOP_in_Python_Notes', 'Document', '/Files/Python/OOP_in_Python_Notes.pdf', '.pdf', '2024-09-16'),

-- Thêm file cho bài giảng 'Advanced Java Concepts'
    (6, 'Advanced_Java_Concepts_Notes', 'Document', '/Files/Java/Advanced_Java_Concepts.pdf', '.pdf', '2024-09-16'),
    (6, 'Advanced_Java_Concepts_Video', 'Video', '/Files/Java/Advanced_Java_Concepts.mp4', '.mp4', '2024-09-16'),
-- Thêm file cho bài giảng 'Java Streams API'
    (7, 'Java_Streams_API_Notes', 'Document', '/Files/Java/Java_Streams_API_Notes.pdf', '.pdf', '2024-09-16'),
    (7, 'Java_Streams_API_Video', 'Video', '/Files/Java/Java_Streams_API.mp4', '.mp4', '2024-09-16'),
-- Thêm file cho bài giảng 'Java Multithreading'
    (8, 'Java_Multithreading_Notes', 'Document', '/Files/Java/Java_Multithreading_Notes.pdf', '.pdf', '2024-09-16'),
    (8, 'Java_Multithreading_Notes_2', 'Document', '/Files/Java/Java_Multithreading_Notes.pdf', '.pdf', '2024-09-16'),
-- Thêm file cho bài giảng 'Java Design Patterns'
    (9, 'Java_Design_Patterns_Video', 'Video', '/Files/Java/Java_Design_Patterns.mp4', '.mp4', '2024-09-16'),
-- Thêm file cho bài giảng 'Java Networking'
    (10, 'Java_Networking_Notes', 'Document', '/Files/Java/Java_Networking_Notes.pdf', '.pdf', '2024-09-16');
