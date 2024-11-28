# **LearnOn - Online Learning Platform**

LearnOn is a modern online learning platform designed, work as a platform for students and instructors to share their knowledges. It supports robust course management, test creation, and secure user authentication using .NET technologies.

---

## **Features**

- 🔒 **User Authentication**: Secure login with Google integration.
- 📚 **Course Management**: Add, update, and manage courses effortlessly.
- 📝 **Test Creation**: Create and manage tests with multiple-choice questions.
- 📊 **Progress Tracking**: View and manage user scores and progress.
- 🎓 **Instructor Tools**: Filter courses and tests relevant to instructors.

---

## **Technologies Used**

- **Languages**: C#, JavaScript, HTML, CSS
- **Frameworks**: ASP.NET Core MVC, Entity Framework Core
- **Database**: SQL Server, FireBase
- **Tools**: Visual Studio, Git, NuGet, ApiVideo, Stringee, VNPay
- **Authentication**: Google OAuth
- **Version Control**: GitHub

 ---
## **Getting Started**

### **Prerequisites**

Before starting, ensure you have the following installed:

- **Visual Studio** (2022 or later)
- **.NET 8 SDK**
- **SQL Server Management Studio 19+**
- **Git**

---

### **Setup Instructions**

Follow these steps to set up the project on your local machine:

1. **Clone the Repository**
   - Create a folder on your system.
   - Navigate to the folder, open git bash and run:
     ```bash
     git clone https://github.com/tqloi/SWP391.git
     ```

2. **Open the Project**
   - Open the `OnlineLearning.sln` file using Visual Studio.

3. **Configure the Database Connection**
   - Open the `appsettings.json` file.
   - Update the connection string under `"ConnectionStrings"` to match your SQL Server configuration.

4. **Update the Database**
   - Open the **NuGet Package Manager Console** in Visual Studio.
   - Run the following command on __nuget console__ to apply database migrations:
     ```bash
     Update-Database
     ```

5. **Run the Application**
   - Press **F5** or click the "Run" button in Visual Studio.
  
---

## **Documentation**

Find detailed documentation for the project below:

- 📄 [Final Release Document](https://docs.google.com/document/d/1XcMouZa1it_Hk98y8HaNa16dJa8bKs6ap4kB5a7dKrA/edit?tab=t.0)
- 📊 [Requirements & Design Specification (RDS)](https://docs.google.com/document/d/1QzPS8zu3B0v14IwpV5-x7v7HA8v9d1W4uCc8XMnWyOg/edit?tab=t.0)
- 📐 [Software Design Specification (SDS)](https://docs.google.com/document/d/1ZRv2UmEPHFXeP16bJ6JGqFPA271WxsXC7vASijOYA44/edit?tab=t.0)
- 🗂️ [Project Tracking](https://docs.google.com/spreadsheets/d/1ZW6KByj_nBRRZMZvkMfH4kUFxZQ4eY571sV5qHblnJA/edit?gid=172658304#gid=172658304)

> Others: [Black Box Testing result](https://docs.google.com/spreadsheets/d/1_-gtIZ_di4ohP15g5OR3zMDd1Rg7rZLE/edit?gid=75712555#gid=75712555).

---

## **Contributing**

We welcome contributions to make LearnOn better! 🚀  
- For bug fixes or feature suggestions, please open an issue.
- To contribute code, fork the repository, make your changes, and submit a pull request.

---

## **License**

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for more details.

---

This project is for Student like us to learn and study, hope you for the best.

🎉 **Happy Learning with LearnOn!** 🎉

