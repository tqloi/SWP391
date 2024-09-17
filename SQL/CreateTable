CREATE DATABASE Online_Learning_Platform;
GO
USE Online_Learning_Platform;
GO

CREATE TABLE Users (
    userID INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(255) NOT NULL,
    password NVARCHAR(255) NOT NULL,  -- Hash of the password
    email NVARCHAR(255) NOT NULL,
    phone NVARCHAR(15),
    profileImage NVARCHAR(255),
    status NVARCHAR(50) CHECK (status IN ('active', 'inactive', 'suspended')),
    lastLogin DATE,
    userType NVARCHAR(50) CHECK (userType IN ('student', 'instructor', 'admin'))
);

-- Instructor table (specific instructor-related fields)
CREATE TABLE Instructor (
    userID INT PRIMARY KEY,
    bio TEXT,
    FOREIGN KEY (userID) REFERENCES Users(userID) 
);

-- Course table
CREATE TABLE Course (
    courseID INT PRIMARY KEY IDENTITY(1,1),
    numberOfStudents INT,
    userID INT,  -- Foreign key to Instructor (userID from User)
    material NVARCHAR(255),  -- Link to course material
    FOREIGN KEY (userID) REFERENCES Instructor(userID)  
);
-- Student table (specific student-related fields)
CREATE TABLE Student (
    userID INT PRIMARY KEY,
    completedCourseID INT,
    completeLectureID INT,
    FOREIGN KEY (userID) REFERENCES Users(userID) 
);

-- Enrollment table
CREATE TABLE Enrollment (
    enrollmentID INT PRIMARY KEY IDENTITY(1,1),
    courseID INT,  -- Foreign key to Courses
    studentID INT,  -- Foreign key to Student (userID from Users)
    enrollmentDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Course(courseID),
    FOREIGN KEY (studentID) REFERENCES Student (userID)  
);
-- Lecture table
CREATE TABLE Lecture (
    lectureID INT PRIMARY KEY IDENTITY(1,1),
    courseID INT,  -- Foreign key to Courses
    title NVARCHAR(255),
    material NVARCHAR(255),  -- Link to lecture-specific material
    date DATETIME,
    FOREIGN KEY (courseID) REFERENCES Course(courseID)
);

-- Test table
CREATE TABLE Test (
    testID INT PRIMARY KEY IDENTITY(1,1), 
    courseID INT,  -- Foreign key to Courses
    startTime DATETIME,
    endTime DATETIME,
    status NVARCHAR(255),
    FOREIGN KEY (courseID) REFERENCES Course(courseID)
);

-- Score table
CREATE TABLE Score (
    studentID INT NOT NULL,
    testID INT NOT NULL,
    score FLOAT NOT NULL,
    FOREIGN KEY (studentID) REFERENCES Student(userID),
    FOREIGN KEY (testID) REFERENCES Test(testID)
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
    FOREIGN KEY (testID) REFERENCES Test(testID)
);

-- Assignment table
CREATE TABLE Assignment (
    assignmentID INT PRIMARY KEY IDENTITY(1,1),
    courseID INT,  -- Foreign key to Courses
    title NVARCHAR(255),
    description TEXT,
    dueDate DATETIME,
    FOREIGN KEY (courseID) REFERENCES Course(courseID)
);

-- Submission table
CREATE TABLE Submission (
    submissionID INT PRIMARY KEY IDENTITY(1,1),
    assignmentID INT,  -- Foreign key to Assignment
    studentID INT,  -- Foreign key to Student (userID from Users)
    submissionLink NVARCHAR(255),  -- Link to the file
    submissionDate DATETIME,
    FOREIGN KEY (assignmentID) REFERENCES Assignment(assignmentID),
    FOREIGN KEY (studentID) REFERENCES Student(userID)  -- References Users table
);

-- Message table
CREATE TABLE Message (
    messageID INT PRIMARY KEY IDENTITY(1,1),
    senderID INT,  -- Foreign key to Users
    receiverID INT,  -- Foreign key to Users
    content TEXT,
    timestamp DATETIME,
    FOREIGN KEY (senderID) REFERENCES Users(userID),  -- Can be a Student or Instructor
    FOREIGN KEY (receiverID) REFERENCES Users(userID)  -- Can be a Student or Instructor
);

-- Comment table
CREATE TABLE Comment (
    commentID INT PRIMARY KEY IDENTITY(1,1),
    courseID INT,  -- Foreign key to Courses
    lectureID INT NULL,  -- Foreign key to Lecture (nullable for course-level comments)
    userID INT,  -- Foreign key to Users (can be Student or Instructor)
    content TEXT,
    timestamp DATETIME,
    parentCmtId INT NULL,  -- Foreign key to Comment (nullable for root comments)
    FOREIGN KEY (courseID) REFERENCES Course(courseID),
    FOREIGN KEY (lectureID) REFERENCES Lecture(lectureID),
    FOREIGN KEY (userID) REFERENCES Users(userID),  -- Could be Student or Instructor
    FOREIGN KEY (parentCmtId) REFERENCES Comment(commentID)
);

GO
CREATE TRIGGER trg_DeleteChildComments
ON Comment
INSTEAD OF DELETE
AS
BEGIN
    -- Recursive CTE to find all child comments
    WITH comments_to_delete AS (
        -- Anchor member: Select the comments that are directly being deleted
        SELECT commentID
        FROM DELETED
        UNION ALL
        -- Recursive member: Select child comments of the deleted comments
        SELECT c.commentID
        FROM Comment c
        INNER JOIN comments_to_delete p ON c.parentCmtId = p.commentID
    )
    -- Perform the delete operation on the identified comments
    DELETE FROM Comment
    WHERE commentID IN (SELECT commentID FROM comments_to_delete);
END;

