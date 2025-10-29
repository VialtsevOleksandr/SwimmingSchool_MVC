# SwimmingSchool_MVC

## 🚀 Overview
SwimmingSchool_MVC is a comprehensive web application designed to manage swimming school activities. It includes features for managing days, events, groups, schedules, and more. This project is ideal for swimming school administrators, trainers, and parents who need a robust system to keep track of schedules, events, and student information.

## ✨ Features
- **Day Management**: Create, edit, and delete days of the week.
- **Event Management**: Create, edit, and delete events with detailed information.
- **Group Management**: Manage groups and their schedules.
- **Pupil Management**: Track student information and their participation in events.
- **Trainer Management**: Manage trainer information and their groups.
- **Scheduling**: Create and manage group schedules.
- **API Endpoints**: RESTful API for managing data.

## 🛠️ Tech Stack
- **Programming Language**: C#
- **Frameworks**: ASP.NET Core MVC
- **Database**: SQL Server
- **Frontend**: Bootstrap, jQuery
- **Other Tools**: Entity Framework Core, Visual Studio

## 📦 Installation

### Prerequisites
- .NET SDK 6.0 or later
- SQL Server (or any other supported database)
- Visual Studio or any other C# IDE

### Quick Start
1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/SwimmingSchool_MVC.git
    ```
2. Open the solution in Visual Studio.
3. Restore NuGet packages:
    ```bash
    dotnet restore
    ```
4. Run the application:
    ```bash
    dotnet run
    ```

### Alternative Installation Methods
- **Docker**: Use Docker to containerize the application.
- **Package Managers**: Use NuGet to manage dependencies.


## 📁 Project Structure
```
SwimmingSchool_MVC/
│
├── Controllers/
│   ├── DaysController.cs
│   ├── EventsController.cs
│   ├── GroupsController.cs
│   ├── GroupSchedulesController.cs
│   ├── GroupTypesController.cs
│   ├── PupilsController.cs
│   ├── PupilsEventsController.cs
│   ├── TrainersController.cs
│
├── Models/
│   ├── Day.cs
│   ├── Event.cs
│   ├── Group.cs
│   ├── GroupSchedule.cs
│   ├── GroupType.cs
│   ├── Pupil.cs
│   ├── PupilsEvent.cs
│   ├── Trainer.cs
│
├── Views/
│   ├── Days/
│   ├── Events/
│   ├── Groups/
│   ├── GroupSchedules/
│   ├── GroupTypes/
│   ├── Home/
│   ├── Pupils/
│   ├── PupilsEvents/
│   ├── Trainers/
│
├── wwwroot/
│   ├── lib/
│   ├── css/
│   ├── js/
│
├── APIControllers/
│   ├── DaysAPIController.cs
│   ├── EventsAPIController.cs
│   ├── GroupsAPIController.cs
│   ├── GroupSchedulesAPIController.cs
│   ├── GroupTypesAPIController.cs
│   ├── PupilsAPIController.cs
│   ├── PupilsEventsAPIController.cs
│   ├── TrainersAPIController.cs
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── launchSettings.json
└── README.md
```

## 🔧 Configuration
- **Environment Variables**: Set environment variables in the `appsettings.json` file.
- **Configuration Files**: Modify the `appsettings.json` and `appsettings.Development.json` files for different environments.

## 👥 Authors 
- **Maintainers**: Vialtsev Oleksandr
