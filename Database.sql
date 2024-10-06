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
-- InstructorConfirmation
CREATE TABLE InstructorConfirmation(
	 ConfirmationID INT PRIMARY KEY,
	 UserID nvarchar(450),
	 Certificatelink nvarchar(400),
	 SendDDate DATETIME DEFAULT GETDATE(),
	 FOREIGN KEY (UserID) REFERENCES AspNetUsers(Id) 
)
ALTER TABLE InstructorConfirmation
ADD SendDDate DATETIME DEFAULT GETDATE();
GO

CREATE TABLE Category(
	CategoryID INT PRIMARY KEY IDENTITY(1,1),
	FullName NVARCHAR(50),
    [Description] NVARCHAR(MAX)
)

-- Course table
CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
	Title NVARCHAR(255),
	CourseCode NVARCHAR(20),
	[Description] NVARCHAR(MAX),
	CoverImagePath NVARCHAR(255),
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
	FOREIGN KEY (categoryID) REFERENCES Category(categoryID),  
    FOREIGN KEY (instructorID) REFERENCES Instructors(InstructorID)   -- Foreign key to Instructor (userID from User)
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
	FOREIGN KEY (courseID) REFERENCES Courses(courseID)
);
ALTER TABLE Payment
ADD CONSTRAINT UC_Payment UNIQUE (StudentID, CourseID);
go

-- Enrollment table
CREATE TABLE Enrollment(
    EnrollmentID INT PRIMARY KEY IDENTITY(1,1),
    PaymentID INT,  -- Foreign key to Courses
    EnrollmentDate DATETIME DEFAULT GETDATE(),  
	[Status] NVARCHAR(50) CHECK (status IN ('Paid', 'Free')),
    FOREIGN KEY (paymentID) REFERENCES Payment (paymentID)  
);

CREATE TABLE Cart (
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID NVARCHAR(450),
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id) 
);
GO
CREATE TABLE CartItem (
    CartItemID INT IDENTITY(1,1) PRIMARY KEY,
    Quantity INT,
    TotalCost MONEY,
    CartID INT,
    CourseID INT,
    FOREIGN KEY (CartID) REFERENCES Cart(CartID),  
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)  
);

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

CREATE TABLE LectureCompletion (
    CompletionID INT PRIMARY KEY IDENTITY(1,1),
    UserID NVARCHAR(450),
    LectureID INT, 
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletionDate DATETIME, 
    FOREIGN KEY (UserID) REFERENCES AspNetUsers(id),
    FOREIGN KEY (LectureID) REFERENCES Lecture(LectureID) 
);

-- Course image and video;
CREATE TABLE LectureFiles (
    FileID INT PRIMARY KEY IDENTITY(1,1),
    LectureID INT,  
    [FileName] NVARCHAR(255),  
    FilePath NVARCHAR(500),  
    UploadDate DATETIME DEFAULT GETDATE(),  
    FOREIGN KEY (lectureID) REFERENCES Lecture(lectureID) ON DELETE CASCADE  
);
-- Test table
CREATE TABLE Test (
    TestID INT PRIMARY KEY IDENTITY(1,1), 
    CourseID INT,  -- Foreign key to Courses
    StartTime DATETIME,
    EndTime DATETIME,
	NumberOfQuestion INT NOT NULL,
    [Status] NVARCHAR(255),
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);
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
    [Description] TEXT,
    DueDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE  
);
-- Submission table
CREATE TABLE Submission (
    SubmissionID INT PRIMARY KEY IDENTITY(1,1),
    AssignmentID INT,  -- Foreign key to Assignment
    StudentID NVARCHAR(450),  -- Foreign key to Student (userID from Users)
    SubmissionLink NVARCHAR(255),  -- Link to the file
    SubmissionDate DATETIME,
    FOREIGN KEY (assignmentID) REFERENCES Assignment(assignmentID),
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id)  -- References Users table
);
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
CREATE TABLE MessageFile (
    FileID INT PRIMARY KEY IDENTITY(1,1),
    MessageID INT,  
    [FileName] NVARCHAR(255),  
    FilePath NVARCHAR(500),  
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
CREATE TABLE CommentFile (
    FileID INT PRIMARY KEY IDENTITY(1,1),
    CommentID INT,  
    [FileName] NVARCHAR(255),  
    FilePath NVARCHAR(500),  
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
    FOREIGN KEY (studentID) REFERENCES AspNetUsers(id)
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

ALTER TABLE Courses
ADD EndDate DATE;

INSERT INTO Category (FullName, [Description]) 
VALUES
    ('Programming', 'Courses related to programming and software development.'),
    ('Data Science', 'Courses focused on data analysis and machine learning.'),
    ('Web Development', 'Courses for building websites and web applications.'),
    ('Design', 'Courses for graphic design and multimedia.'); 

INSERT INTO Courses 
    (Title, CourseCode, [Description], CoverImagePath, InstructorID, NumberOfStudents, Price, CategoryID, [Level], [Status], CreateDate, LastUpdate, EndDate, NumberOfRate) 
VALUES
    -- Programming Category
    ('Python for Beginners', 'CS101', 'Learn Python programming from scratch.', 'cover/python.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 100.00, 1, 'Beginner', 1, '2024-09-01', '2024-09-20', '2024-12-20', 0),
    ('Java Advanced Techniques', 'CS102', 'Explore advanced Java programming concepts.', 'cover/java.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 120.00, 1, 'Advanced', 1, '2024-09-05', '2024-09-21', '2024-12-20', 0),
    ('C# for Beginners', 'CS103', 'Introduction to C# programming.', 'cover/csharp.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 90.00, 1, 'Beginner', 1, '2024-09-10', '2024-09-22', '2024-12-20', 0),
    ('Full-Stack Development with Node.js', 'CS104', 'Learn full-stack development using Node.js.', 'cover/nodejs.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 150.00, 1, 'Intermediate', 1, '2024-09-15', '2024-09-23', '2024-12-20', 0),
    ('Introduction to Algorithms', 'CS105', 'Learn about algorithms and data structures.', 'cover/algorithms.jpg', '5c7c5bc0-06c9-4840-8dd5-7dc0860469ed', 0, 110.00, 1, 'Advanced', 1, '2024-09-20', '2024-09-24', '2024-12-20', 0),

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
ALTER TABLE Courses
DROP COLUMN SyllabusLink;

-- them bang material, 1 khoa hoc co nhieu material
CREATE TABLE CourseMaterials (
    MaterialID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT NOT NULL,
    MaterialsLink NVARCHAR(255) NOT NULL,
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE 
);
go

SELECT * FROM Courses;
Insert into Instructors(InstructorID, Description)
values
('5c7c5bc0-06c9-4840-8dd5-7dc0860469ed','Heloo student');

ALTER TABLE Courses
ALTER COLUMN Description VARCHAR(MAX);
go

UPDATE Courses
SET [Description] = CASE 
    WHEN CourseCode = 'CS101' THEN 'This course will provide a comprehensive introduction to Python programming, covering essential concepts such as data types, control structures, and functions. Students will engage in hands-on coding exercises to solidify their understanding of Python syntax and libraries. By the end, they will be able to write simple programs and understand basic programming logic.'
    WHEN CourseCode = 'CS102' THEN 'This advanced Java course delves into sophisticated programming techniques and concepts such as concurrency, networking, and design patterns. Students will learn to develop scalable applications and enhance their problem-solving skills through real-world projects. The course will also cover Java’s rich ecosystem of frameworks and libraries for efficient development.'
    WHEN CourseCode = 'CS103' THEN 'In this beginner-friendly C# course, students will learn the basics of C# programming, including object-oriented programming principles, data types, and exception handling. Through practical examples and projects, learners will develop their programming skills and understand how to create applications using the .NET framework. By the end of the course, students will feel confident in their ability to write functional C# programs.'
    WHEN CourseCode = 'CS104' THEN 'This full-stack development course with Node.js will teach you how to create robust web applications from scratch. Students will explore both server-side and client-side programming, learning to build APIs and work with databases. The course emphasizes practical application through hands-on projects, enabling students to deploy real-world applications by the end.'
    WHEN CourseCode = 'CS105' THEN 'Explore the world of algorithms and data structures in this comprehensive course focusing on designing and analyzing algorithms. Students will learn about various algorithmic techniques, including sorting, searching, and graph algorithms, and how to apply them to solve complex problems. Practical coding assignments will enhance students ability to implement efficient algorithms in their projects.'
    WHEN CourseCode = 'DS101' THEN 'This course provides an introduction to data analysis using R programming, covering data manipulation, visualization, and statistical modeling. Students will learn to work with real-world datasets, applying R tools to derive meaningful insights. The hands-on approach ensures that learners can confidently perform data analysis tasks by the end of the course.'
    WHEN CourseCode = 'DS102' THEN 'Master machine learning algorithms and techniques in this extensive course, focusing on the implementation and evaluation of various models. Students will learn about supervised and unsupervised learning, feature engineering, and model selection through practical projects. By the end, participants will have a strong foundation to tackle machine learning challenges in their own projects.'
    WHEN CourseCode = 'DS103' THEN 'Designed for beginners, this course introduces key concepts in data science, including data collection, analysis, and visualization techniques. Students will explore various tools and libraries to manipulate data and create insightful visualizations. The course emphasizes hands-on projects to help learners apply their knowledge to real-world scenarios.'
    WHEN CourseCode = 'DS104' THEN 'Dive into deep learning with TensorFlow in this advanced course, learning to build and train neural networks for various applications. Students will cover essential topics such as convolutional networks, recurrent networks, and model optimization. The course includes practical exercises to reinforce understanding and prepare learners for real-world deep learning projects.'
    WHEN CourseCode = 'DS105' THEN 'In this course, students will explore essential statistical methods used in data science, including descriptive and inferential statistics. Topics will include probability distributions, hypothesis testing, and regression analysis, with an emphasis on practical applications. By the end of the course, students will be equipped to analyze data and draw meaningful conclusions.'
    WHEN CourseCode = 'WD101' THEN 'This beginner-friendly course covers the fundamentals of web development using HTML and CSS to create responsive web pages. Students will learn about page structure, styling techniques, and best practices for modern web design. Practical projects will provide opportunities to build real websites from scratch.'
    WHEN CourseCode = 'WD102' THEN 'Get started with JavaScript in this essential course, learning to add interactivity and manipulate the DOM in web pages. Students will cover foundational concepts such as events, functions, and API integration, allowing them to create dynamic user experiences. The course includes hands-on coding exercises to reinforce learning.'
    WHEN CourseCode = 'WD103' THEN 'This course focuses on building user interfaces using React, covering component-based architecture, state management, and routing. Students will learn to create single-page applications with a responsive design, utilizing React’s powerful features. Practical projects will help solidify their skills in developing modern web applications.'
    WHEN CourseCode = 'WD104' THEN 'Learn how to build scalable web applications with Node.js in this hands-on course, focusing on server-side programming and database integration. Students will explore RESTful APIs, middleware, and database connectivity while developing their own web applications. The course encourages practical application through projects that simulate real-world scenarios.'
    WHEN CourseCode = 'WD105' THEN 'Explore advanced CSS techniques for modern web design, learning Flexbox, Grid Layout, and responsive design principles. Students will create visually appealing layouts and optimize their websites for various devices. The course includes practical projects to enhance understanding and application of advanced CSS skills.'
    WHEN CourseCode = 'D101' THEN 'This course introduces the principles of graphic design, including color theory, typography, and composition through practical projects. Students will learn to create visually compelling designs and develop a portfolio of work. Emphasis is placed on understanding design concepts and applying them to real-world scenarios.'
    WHEN CourseCode = 'D102' THEN 'Get started with UI/UX design in this fundamental course, focusing on user research, wireframing, and creating user-centered designs. Students will learn the importance of usability and accessibility in design. The course involves practical assignments to develop skills in creating intuitive and engaging user interfaces.'
    WHEN CourseCode = 'D103' THEN 'In this beginner-level course, students will learn the basics of Adobe Photoshop and explore photo editing techniques. From retouching photos to creating stunning graphics, learners will gain hands-on experience with essential tools and features. The course prepares students to tackle various graphic design projects using Photoshop.'
    WHEN CourseCode = 'D104' THEN 'Master illustration techniques for graphic design in this advanced course, learning various styles and digital tools for visual narratives. Students will explore character design, storytelling, and composition while developing their own illustration projects. The course emphasizes creativity and technical skills in the field of illustration.'
    WHEN CourseCode = 'D105' THEN 'This course provides an introduction to motion graphics using Adobe After Effects, covering animation and visual effects fundamentals. Students will learn to create eye-catching animations and apply visual effects to enhance their projects. The course includes hands-on projects that allow learners to experiment with motion graphics techniques.'
    ELSE [Description] -- Keep existing description if CourseCode does not match
END;
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
(2, 'Giới thiệu về lập trình', 'Bài giảng giới thiệu về lập trình.', GETDATE()),
(2, 'Cấu trúc dữ liệu', 'Tìm hiểu về cấu trúc dữ liệu cơ bản.', GETDATE()),
(2, 'Giới thiệu về thuật toán', 'Các thuật toán cơ bản trong lập trình.', GETDATE()),
(2, 'Lập trình hướng đối tượng', 'Khái niệm lập trình hướng đối tượng.', GETDATE()),
(2, 'Xử lý chuỗi', 'Cách xử lý chuỗi trong lập trình.', GETDATE()),
(2, 'Giới thiệu về cơ sở dữ liệu', 'Các khái niệm cơ bản về cơ sở dữ liệu.', GETDATE()),
(2, 'Thao tác với SQL', 'Hướng dẫn các lệnh SQL cơ bản.', GETDATE()),
(2, 'Xây dựng ứng dụng web', 'Các bước xây dựng ứng dụng web.', GETDATE()),
(2, 'Kiểm thử phần mềm', 'Các phương pháp kiểm thử phần mềm.', GETDATE()),
(2, 'Triển khai ứng dụng', 'Hướng dẫn triển khai ứng dụng lên server.', GETDATE());

INSERT INTO LectureCompletion (UserID, LectureID, IsCompleted, CompletionDate) VALUES
('5e9bf460-cbe6-4f0b-b1dd-1c7d817bfffc', 4, 1, GETDATE()),-- Bài giảng 1 đã hoàn thành
('5e9bf460-cbe6-4f0b-b1dd-1c7d817bfffc', 2, 1, GETDATE()),  -- Bài giảng 2 đã hoàn thành
('5e9bf460-cbe6-4f0b-b1dd-1c7d817bfffc', 3, 1, GETDATE());  -- Bài giảng 3 đã hoàn thành
go

CREATE TRIGGER trg_UpdateStudentCourseProgress
ON LectureCompletion
AFTER INSERT
AS
BEGIN
    DECLARE @UserID NVARCHAR(450);
    DECLARE @CourseID INT;

    -- Lấy UserID và CourseID từ bản ghi vừa thêm vào
    SELECT @UserID = UserID, @CourseID = Lecture.CourseID
    FROM Lecture
    INNER JOIN inserted ON Lecture.LectureID = inserted.LectureID;

    -- Tính số lượng bài giảng đã hoàn thành cho User trong khóa học
    DECLARE @CompletedCount INT;
    SELECT @CompletedCount = COUNT(*)
    FROM LectureCompletion
    WHERE UserID = @UserID AND IsCompleted = 1 
      AND LectureID IN (
          SELECT LectureID
          FROM Lecture
          WHERE CourseID = @CourseID
      );

    -- Tính tổng số bài giảng trong khóa học
    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*)
    FROM Lecture
    WHERE CourseID = @CourseID;

    -- Tính tỷ lệ hoàn thành (progress)
    DECLARE @Progress FLOAT;
    IF @TotalCount > 0
    BEGIN
        SET @Progress = CAST(@CompletedCount AS FLOAT) / CAST(@TotalCount AS FLOAT);
    END
    ELSE
    BEGIN
        SET @Progress = NULL; -- Nếu không có bài giảng nào, progress sẽ NULL
    END

    -- Cập nhật bảng StudentCourses với tiến độ
    UPDATE StudentCourses
    SET Progress = @Progress
    WHERE StudentID = @UserID AND CourseID = @CourseID;
END;

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