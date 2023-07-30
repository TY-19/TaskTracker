# Task Tracker 
### Table of contents
* [Introduction](#Introduction)
* [Launch](#Launch)
* [Functionality](#Functionality)

### Introduction
Task Tracker is a simple and user-friendly solution designed to streamline task management for teams. It offers a versatile platform that simplifies the process of organizing and tracking tasks within an organization. Role-based access control and intuitive interface empowers administrators, managers, and employees to collaborate efficiently and achieve their project goals.

### Launch
#### Preparation before first launch
1. Install .NET dependencies by running the following command in the project folder:
`dotnet restore`
2. Install Angular dependencies by running:
`npm install`
3. Configure the **appsettings.json** file located in the **/src/WebAPI folder.**
3.1. Set the connection string to your instance of MS SQL Server by editing **line 3** of the **appsettings.json** file.
3.2. (*optional*) Change the default admin credentials (lines 8-10 of the appsettings.json file).
3.3. Change the JWT secret by modifying line 14 of the appsettings.json file.
3.4. (*optional*) Change launch settings in the /src/WebAPI/Properties/launchSettings.json file if you need to use another port for the backend server (default ports are 40080 for HTTP and 40443 for HTTPS).

#### Start application
Launch the backend server by running the following command from the **/src/WebAPI** folder:
`dotnet run`
Launch the frontend dev server by running the following command from the **/src/WebUI** folder:
`ng serve`

### Application functionality
#### Role-based Access Control
Task Tracker implements a role-based access control system to ensure that users have appropriate permissions based on their roles within the organization. The following roles are supported:
**Administrator**: Administrators have full control over the system. They can manage users, boards, and perform various administrative tasks.
**Manager**: Managers have the authority to manage boards tasks or employees. 
**Employee**: Employees have limited access, allowing them to view boards they are added to and tasks on these boards, change the stage of tasks they are working on, and mark tasks as completed.

#### Board Management
Administrators and managers can create boards. Administrators also have the privilege to edit and delete boards. Each board can be customized with various stages, which represent the different phases a task must go through. Managers can further populate these boards with tasks, adding essential information such as topic, description, deadline, and the responsible employee.

#### Task Subparts
Tasks within Task Tracker can be broken down into subparts, each with a value that represents its magnitude relative to the overall task. This feature enables granular tracking and evaluation of task progress, providing valuable insights for effective project management.

#### User Registration and Profile Management
Task Tracker requires users to register to access the platform's features. Unregistered users can easily sign up and create an account. Once registered, users can view and edit their profiles, change their passwords, and manage their preferences.

#### User Role Management
Administrators hold the authority to view, edit, or delete users and assign them roles. This capability allows for seamless employee management and ensures that users are appropriately assigned roles that align with their responsibilities within the organization.