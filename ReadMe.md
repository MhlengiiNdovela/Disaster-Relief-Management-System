# Disaster Relief Management System

## Overview

The **Disaster Relief Management System** is a comprehensive C# web application designed to support and streamline disaster-relief operations for the **Gift of the Givers Foundation**.

The platform will provide a centralised digital hub for managing disaster-response activities, coordinating volunteers, allocating resources, and monitoring relief efforts. By bringing these processes together into a single system, the application aims to improve operational efficiency, reduce response times, and ensure that critical resources reach affected communities where they are needed most.

## Live site

The web application is hosted on Render:

**https://gift-of-the-givers-8f3i.onrender.com**

The home page is public. Sign in with the demo accounts below. The free instance sleeps after a period without visitors, so the first request can take about a minute.

## Project Objectives

The system aims to:

- Centralise disaster-relief information and operations.
- Manage and coordinate volunteers effectively.
- Track available relief resources and supplies.
- Allocate resources to disaster-affected areas based on demand.
- Manage disaster incidents and relief requests.
- Provide relevant information to administrators and relief coordinators.
- Improve communication and coordination between stakeholders.
- Provide dashboards and reports for monitoring relief operations.
- Maintain secure and reliable access to sensitive operational information.
- Support better decision-making during disaster-response activities.

## Core System Areas

The application is expected to include functionality for:

### Disaster & Incident Management
- Create and manage disaster incidents.
- Record affected locations and communities.
- Track the severity and status of incidents.
- Record relief requirements and requests.
- Monitor the progress of response operations.

### Volunteer Management
- Register and manage volunteers.
- Maintain volunteer profiles and skills.
- Track volunteer availability.
- Assign volunteers to relief activities.
- Monitor volunteer assignments and participation.

### Resource Management
- Register relief resources and supplies.
- Track resource quantities and availability.
- Categorise resources such as food, water, clothing, medical supplies and equipment.
- Monitor resource usage.
- Track resource movement and allocation.

### Resource Allocation
- Match available resources with disaster needs.
- Allocate resources to affected areas.
- Track allocation status.
- Monitor shortages and outstanding requirements.

### Administration
- Manage system users and roles.
- Provide role-based access control.
- Manage system data and configurations.
- Monitor system activity.

### Reporting & Dashboard
- Display an overview of active disasters.
- Show available and allocated resources.
- Display volunteer information.
- Monitor outstanding relief requests.
- Generate operational reports and statistics.

## Run without Azure

The web app uses SQLite when no SQL Server connection string is configured, and it generates the donation tax certificate inside the site when Azure Functions is not available. Demo accounts are created on first launch:

- Admin: `admin@giftofthegivers.org` / `Admin#12345`
- Employee: `employee@giftofthegivers.org` / `Employee#12345`
- Donor: `donor@giftofthegivers.org` / `Donor#12345`

```powershell
dotnet run --project Web
```

## Free deployment (GitHub + Render)

The source repository is [MhlengiiNdovela/Disaster-Relief-Management-System](https://github.com/MhlengiiNdovela/Disaster-Relief-Management-System). Azure DevOps and the Azure for Students subscription are not required for this host.

GitHub Actions (`.github/workflows/ci.yml`) builds the solution and runs the tests on every push. `render.yaml` builds the root `Dockerfile` and publishes the Web project as the free Render service `gift-of-the-givers`. A push to `master` updates that service.

An existing Azure SQL connection string is still honoured when `ConnectionStrings:GiftOfTheGivers` contains `Server=`. When it does not, the site uses SQLite and creates the database on startup.

## Technology

The project will be developed using modern software engineering practices and a C#/.NET technology stack.

Potential technologies include:

- **C#**
- **ASP.NET Core**
- **Entity Framework Core**
- **SQL Server**
- **HTML5 / CSS3**
- **JavaScript**
- **RESTful APIs**
- **Git & GitHub**
- **Azure DevOps**

The final technology stack may be refined during the planning and architecture stages.

## Project Goal

The ultimate goal of the project is to create a reliable, secure and user-friendly platform that can help humanitarian organisations coordinate disaster-relief operations more effectively.

This project represents an opportunity to apply software engineering principles to a real-world humanitarian problem and develop technology that can contribute to faster, more organised and more effective disaster response.