# Project Support App

This application is a project management web application, similar to MSProject. The main entities used for managing projects are: tasks, resources, dependencies and costs. For every project, this informations can be seen in a Gantt Diagram, that provides a quick overview of the project.

In order to access the application, users need to create an account. They can do this either by registering or by connecting with a Google or Facebook account.

In order to improve the management process and in order to increase the communication between the users in a project, real time communication was introduced to the app. The chats are of two types: Private and Room. Each project has a Room chat and every user can create a Private chat with any other user. In order to alert the users when they have been added or removed from a task or project, a real time notification system was also implemented.

When you check out the app, in the Login page I recommend choosing the option to Log In with a Demo Account in order to have access to all the features of the application.

Please visit the site and try it out: https://projectsupport-dinasergiu.azurewebsites.net

The documentation in Romanian for the project can be read here: https://pdfhost.io/v/uw8gjXSb9_Proiect__DINA_C_Sergiu.pdf

## Used Technologies

In order to create the app I used the programming language C# together with different frameworks from the .NET Core ecosystem, such as: ASP.NET Core, Entity Framework Core, ASP.NET Core Identity and SignalR. For implementing client-side functionalities I used HTML, CSS, Bootstrap, JavaScript and TypeScript, together with Razor Pages. The Database management system used during development was SQL Server and for publishing the App in Azure I switched to Azure SQL.

## Demo Accounts

A role based security system was implemented to create permission based roles. The roles of Admin, Manager and Developer restrict what the user can see and do.

If you want to check out the different roles, there is a Demo Account page, accesible from the Log In page, that allows you do log in with a demo account in order to see the easily see the diffrences between roles.

![ss5](https://user-images.githubusercontent.com/70022000/127128459-0d3c70d9-53a6-42f2-854a-d216706ef1a4.jpg)

## App Design

Here is the app in action:

### Log In Page
![ss1](https://user-images.githubusercontent.com/70022000/127128418-4ccf3b79-669e-401b-a264-227d03ab6eaf.jpg)

### Home Page
![ss6](https://user-images.githubusercontent.com/70022000/127128489-0ca0f647-0dfb-476c-aed1-86da0fe4f195.jpg)

### Admin Dashboard
![ss16](https://user-images.githubusercontent.com/70022000/127128524-a6a10fee-28b6-48d9-9601-3a769e9d7937.jpg)

### Admin Actions
![ss10](https://user-images.githubusercontent.com/70022000/127128548-a4903039-fe96-40f5-98f2-211e83a56a9b.jpg)

### User Details Page
![ss9](https://user-images.githubusercontent.com/70022000/127128561-3a63641e-e67c-43fc-8b89-2d8f68be3872.jpg)

### Project Actions
![ss12](https://user-images.githubusercontent.com/70022000/127128567-f9f52c27-666b-46db-8f0f-685638d150b1.jpg)

### The Gantt Diagram of a Project
![ss13](https://user-images.githubusercontent.com/70022000/127128571-70b9f269-f56c-481e-9288-56c537a01b24.jpg)

### Project Actions
![ss14](https://user-images.githubusercontent.com/70022000/127128575-8a2967cd-b8db-4a34-86e0-8062a4bde011.jpg)

### Project Chat
![ss15](https://user-images.githubusercontent.com/70022000/127128581-d1825bee-c160-42b2-a4ec-3451616ef561.jpg)
