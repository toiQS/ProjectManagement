# Project_Management

# Code of table:
- 1001 project
- 1002 role in project 
- 1003 role application user in project
- 1004 position in project
- 1005 position work of member
- 1006 plan in project
- 1007 plan
- 1008 task in plan
- 1009 task
- 1010 member in task
- 1011 application user
- 

```
Project_Management
├─ .git
│  ├─ description
│  ├─ hooks
│  │  ├─ applypatch-msg.sample
│  │  ├─ commit-msg.sample
│  │  ├─ fsmonitor-watchman.sample
│  │  ├─ post-update.sample
│  │  ├─ pre-applypatch.sample
│  │  ├─ pre-commit.sample
│  │  ├─ pre-merge-commit.sample
│  │  ├─ pre-push.sample
│  │  ├─ pre-rebase.sample
│  │  ├─ pre-receive.sample
│  │  ├─ prepare-commit-msg.sample
│  │  ├─ push-to-checkout.sample
│  │  ├─ sendemail-validate.sample
│  │  └─ update.sample
│  ├─ info
│  │  └─ exclude
│  ├─ objects
│  │  ├─ pack
│  │  │  ├─ pack-3af2c8c72248bcc62ba336388f8435cc13a08f0e.pack
│  │  │  ├─ pack-3af2c8c72248bcc62ba336388f8435cc13a08f0e.rev
│  │  │  └─ pack-3af2c8c72248bcc62ba336388f8435cc13a08f0e.idx
│  │  ├─ info
│  │  ├─ 8f
│  │  │  └─ 5f0237ebfa11ac4aad02f69dbf219e23d408ae
│  │  ├─ 17
│  │  │  └─ 8609c4923c35ef5bcae44e0e4fb19853801cf8
│  │  ├─ 73
│  │  │  ├─ bf49632a389226b9c06e13c0321043151946a2
│  │  │  └─ 8f0331f9d7e273a6f40757872e9d495d5cec13
│  │  ├─ 16
│  │  │  └─ 33576247e81619cfdf809a12928f8c3874fa4e
│  │  ├─ f2
│  │  │  └─ 6f93c0954e1d33e24b6f76bc3d6ca948c8de15
│  │  ├─ ee
│  │  │  └─ d65ac4c385983b62ab54314444e57f6763b955
│  │  ├─ 26
│  │  │  └─ 469082d48573a8c34b5d52620980c431c8378e
│  │  ├─ 2e
│  │  │  └─ 5b1c3873b038f2cf403a128edffde7ca7b8808
│  │  ├─ e6
│  │  │  └─ 9de29bb2d1d6434b8b29ae775ad8c2e48c5391
│  │  ├─ ca
│  │  │  └─ e592efbc55fe14263e3c83621eba756554abd5
│  │  ├─ b7
│  │  │  └─ 01daefa71c1c22b75c3050ae4ccc68c65574f3
│  │  ├─ 47
│  │  │  └─ 6778b0c10f236a1453b1859a814dc2b5069fde
│  │  ├─ b2
│  │  │  └─ f5f0ccb1a4f3da57f7d9bc39275a763e6d87bf
│  │  ├─ b8
│  │  │  └─ 19f9f6eb33481788cac22f235892dace6feff9
│  │  ├─ 31
│  │  │  └─ ed6100a98fe1fdde87d54804f44bd747404177
│  │  ├─ 84
│  │  │  └─ 232646150316e8e4eaec7595ddd3afce920470
│  │  ├─ 24
│  │  │  └─ 199955f2cee6c2e56dbd23365adf205234864f
│  │  ├─ da
│  │  │  └─ 749e8636afc6354052cda77020a34c2ce40731
│  │  ├─ 40
│  │  │  └─ 1541af2b8c005369fd7821ab20a2a8964b6b3c
│  │  ├─ 25
│  │  │  └─ 9225f24909734bc42b0700c8163162cc269c99
│  │  ├─ 29
│  │  │  └─ 9defdb02e3949925574c93f98bca08784e0bf7
│  │  ├─ bb
│  │  │  └─ e0689bd567b0535b386dcc93860d85d83bbb14
│  │  └─ d8
│  │     └─ 1e47d46f624ca082a0af918743093e96adf63f
│  ├─ refs
│  │  ├─ heads
│  │  │  └─ master
│  │  ├─ tags
│  │  └─ remotes
│  │     └─ origin
│  │        ├─ master
│  │        └─ HEAD
│  ├─ packed-refs
│  ├─ HEAD
│  ├─ FETCH_HEAD
│  ├─ config
│  ├─ COMMIT_EDITMSG
│  └─ index
├─ .gitattributes
├─ .gitignore
├─ API Gateway
├─ Application
│  ├─ GrpcService1
│  │  ├─ GrpcService1.csproj
│  │  ├─ Program.cs
│  │  ├─ Properties
│  │  │  └─ launchSettings.json
│  │  ├─ Protos
│  │  │  └─ greet.proto
│  │  ├─ Services
│  │  │  └─ GreeterService.cs
│  │  ├─ appsettings.Development.json
│  │  └─ appsettings.json
│  └─ PM.Application
│     ├─ PM.ApplicationServices.csproj
│     ├─ Properties
│     │  └─ launchSettings.json
│     ├─ Protos
│     │  └─ greet.proto
│     ├─ Services
│     │  └─ GreeterService.cs
│     ├─ appsettings.Development.json
│     ├─ appsettings.json
│     ├─ Program.cs
│     └─ UserFlow
├─ Domain
│  ├─ PM.Domain
│  │  ├─ Class1.cs
│  │  ├─ DTOs
│  │  │  ├─ ApplicationUser.cs
│  │  │  ├─ Class1.cs
│  │  │  ├─ MemberInTaskDTO.cs
│  │  │  ├─ PlanDTO.cs
│  │  │  ├─ PlanInProjectDTO.cs
│  │  │  ├─ PositionWorkOfMemberDTO.cs
│  │  │  ├─ PostitionInProjectDTO.cs
│  │  │  ├─ ProjectDTO.cs
│  │  │  ├─ RoleApplicationUserInProjectDTO.cs
│  │  │  ├─ RoleInProjectDTO.cs
│  │  │  ├─ TaskDTO.cs
│  │  │  └─ TaskInPlanDTO.cs
│  │  ├─ FileName.cs
│  │  └─ PM.Domain.csproj
│  └─ PM.DomainServices
│     ├─ PM.DomainServices.csproj
│     ├─ Shared
│     │  └─ ServicesResuslt.cs
│     ├─ ILogic
│     │  └─ IUserLogic.cs
│     ├─ Logic
│     │  └─ UserLogin.cs
│     └─ Models
│        ├─ InputModels
│        │  ├─ Users
│        │  │  ├─ LoginModels.cs
│        │  │  └─ RegisterModels.cs
│        │  ├─ Projects
│        │  ├─ Plans
│        │  └─ Tasks
│        └─ OutputModels
│           └─ Users
│              └─ ApplicationUserModels.cs
├─ Infrastructure
│  ├─ PM.Infrastructure
│  │  ├─ Class1.cs
│  │  ├─ Configurations
│  │  │  └─ Configuration.cs
│  │  ├─ Jwts
│  │  │  └─ JwtHepler.cs
│  │  ├─ Loggers
│  │  │  └─ LoggerHelper.cs
│  │  └─ PM.Infrastructure.csproj
│  └─ PM.Persistence
│     ├─ Configurations
│     │  └─ PersistenceServiceRegistration.cs
│     ├─ Context
│     │  └─ ApplicationDbContext.cs
│     ├─ FileName.cs
│     ├─ IServices
│     │  ├─ IApplicationUserServices.cs
│     │  ├─ IMemberInTaskServices.cs
│     │  ├─ IPlanInProjectServices.cs
│     │  ├─ IPlanServices.cs
│     │  ├─ IPositionInProjectServices.cs
│     │  ├─ IPositionWorkOfMemberServices.cs
│     │  ├─ IProjectServices.cs
│     │  ├─ IRepository.cs
│     │  ├─ IRoleApplicationUserInProjectServices.cs
│     │  ├─ IRoleInProjectServices.cs
│     │  ├─ ITaskInPlanServices.cs
│     │  └─ ITaskServices.cs
│     ├─ Migrations
│     │  ├─ 20241026043743_PM-Adding-Models.Designer.cs
│     │  ├─ 20241026043743_PM-Adding-Models.cs
│     │  ├─ 20241118171259_demo.Designer.cs
│     │  ├─ 20241118171259_demo.cs
│     │  └─ ApplicationDbContextModelSnapshot.cs
│     ├─ PM.Persistence.csproj
│     └─ Services
│        ├─ ApplicationUserServices.cs
│        ├─ MemberInTaskServices.cs
│        ├─ PlanInProjectServices.cs
│        ├─ PlanServices.cs
│        ├─ PositionInProjectServices.cs
│        ├─ PositionWorkOfMemberServices.cs
│        ├─ ProjectServices.cs
│        ├─ Repository.cs
│        ├─ RoleApplicationUserInProjectServices.cs
│        ├─ RoleInProjectServices.cs
│        ├─ TaskInPlanServices.cs
│        └─ TaskServices.cs
├─ Presentation
│  └─ PM.WPF
│     ├─ App.xaml
│     ├─ App.xaml.cs
│     ├─ AssemblyInfo.cs
│     ├─ Assets
│     │  ├─ background-2.jpg
│     │  ├─ background-3.jpg
│     │  ├─ background.jpg
│     │  ├─ logo-1.png
│     │  ├─ logo-2.png
│     │  ├─ logo.png
│     │  ├─ wpfui-icon-1024.png
│     │  └─ wpfui-icon-256.png
│     ├─ Helpers
│     │  └─ RelativePathConverter .cs
│     ├─ Models
│     │  ├─ Header.cs
│     │  ├─ Person.cs
│     │  ├─ member
│     │  │  ├─ MemberItem.cs
│     │  │  ├─ NewMember.cs
│     │  │  └─ UpdateMember.cs
│     │  ├─ plan
│     │  │  ├─ InfoDetailPlan.cs
│     │  │  ├─ NewPlan.cs
│     │  │  ├─ PlanItem.cs
│     │  │  └─ UpdatePlan.cs
│     │  ├─ projects
│     │  │  ├─ InfoProject.cs
│     │  │  ├─ NewProject.cs
│     │  │  ├─ ProjectDashBoard.cs
│     │  │  ├─ ProjectItem.cs
│     │  │  └─ UpdateProject.cs
│     │  └─ task
│     │     ├─ InfoDetailTask.cs
│     │     ├─ NewTask.cs
│     │     ├─ TaskItem.cs
│     │     └─ UpdateTask.cs
│     ├─ PM.WPF.csproj
│     ├─ ViewModels
│     │  ├─ HomePageViewModel.cs
│     │  ├─ MemberProjectPageModelView.cs
│     │  ├─ PlanViewModel.cs
│     │  ├─ ProjectDashBoardViewModel.cs
│     │  ├─ TaskWindowViewModel.cs
│     │  └─ ViewModel.cs
│     ├─ Views
│     │  ├─ Pages
│     │  │  ├─ HeaderFrame.xaml
│     │  │  ├─ HeaderFrame.xaml.cs
│     │  │  ├─ Pages of MainWindow
│     │  │  │  ├─ HomePage.xaml
│     │  │  │  ├─ HomePage.xaml.cs
│     │  │  │  ├─ LogOutPage.xaml
│     │  │  │  ├─ LogOutPage.xaml.cs
│     │  │  │  ├─ ProfilePage.xaml
│     │  │  │  ├─ ProfilePage.xaml.cs
│     │  │  │  ├─ RecentlyPage.xaml
│     │  │  │  ├─ RecentlyPage.xaml.cs
│     │  │  │  ├─ RepositoryPage.xaml
│     │  │  │  ├─ RepositoryPage.xaml.cs
│     │  │  │  ├─ RepositoryPages
│     │  │  │  │  ├─ AddProject.xaml
│     │  │  │  │  ├─ AddProject.xaml.cs
│     │  │  │  │  ├─ ProjectDetail.xaml
│     │  │  │  │  ├─ ProjectDetail.xaml.cs
│     │  │  │  │  ├─ UpdateProject.xaml
│     │  │  │  │  └─ UpdateProject.xaml.cs
│     │  │  │  ├─ SettingPage.xaml
│     │  │  │  ├─ SettingPage.xaml.cs
│     │  │  │  ├─ ThankPage.xaml
│     │  │  │  └─ ThankPage.xaml.cs
│     │  │  └─ Pages of ProjectWindow
│     │  │     ├─ DashboardPage.xaml
│     │  │     ├─ DashboardPage.xaml.cs
│     │  │     ├─ MemberPages
│     │  │     │  ├─ AddMember.xaml
│     │  │     │  ├─ AddMember.xaml.cs
│     │  │     │  ├─ UpdateMember.xaml
│     │  │     │  └─ UpdateMember.xaml.cs
│     │  │     ├─ MemberProjectPage.xaml
│     │  │     ├─ MemberProjectPage.xaml.cs
│     │  │     ├─ PlanManagementPage.xaml
│     │  │     ├─ PlanManagementPage.xaml.cs
│     │  │     ├─ PlanPages
│     │  │     │  ├─ InfoDetailPlan.xaml
│     │  │     │  ├─ InfoDetailPlan.xaml.cs
│     │  │     │  ├─ NewPlan.xaml
│     │  │     │  ├─ NewPlan.xaml.cs
│     │  │     │  ├─ UpdatePlan.xaml
│     │  │     │  └─ UpdatePlan.xaml.cs
│     │  │     ├─ SettingPage.xaml
│     │  │     ├─ SettingPage.xaml.cs
│     │  │     ├─ TaskPages
│     │  │     │  ├─ InfoDetailTask.xaml
│     │  │     │  ├─ InfoDetailTask.xaml.cs
│     │  │     │  ├─ NewTask.xaml
│     │  │     │  ├─ NewTask.xaml.cs
│     │  │     │  ├─ UpdateTask.xaml
│     │  │     │  └─ UpdateTask.xaml.cs
│     │  │     ├─ ViewPage.xaml
│     │  │     └─ ViewPage.xaml.cs
│     │  └─ Windows
│     │     ├─ LoginWindow.xaml
│     │     ├─ LoginWindow.xaml.cs
│     │     ├─ MainWindow.xaml
│     │     ├─ MainWindow.xaml.cs
│     │     ├─ ProjectWindow.xaml
│     │     ├─ ProjectWindow.xaml.cs
│     │     ├─ RegisterWindow.xaml
│     │     ├─ RegisterWindow.xaml.cs
│     │     ├─ TaskWindow.xaml
│     │     ├─ TaskWindow.xaml.cs
│     │     ├─ UserWindow.xaml
│     │     └─ UserWindow.xaml.cs
│     ├─ app.manifest
│     └─ wpfui-icon.ico
├─ Project_Management.sln
├─ README.md
├─ .github
│  └─ workflows
└─ .config
   └─ dotnet-tools.json

```