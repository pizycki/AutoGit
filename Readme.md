# AutoGit
[![NuGet](https://img.shields.io/nuget/v/dotnet-AutoGit.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-AutoGit/)
[![NuGet](https://img.shields.io/nuget/dt/dotnet-AutoGit.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-AutoGit/)


## Goal
Create tool that will commit changes in git repository for every set period of time.

## Install

Make sure you have [.NET Core SDK](https://www.microsoft.com/net/download) installed.

Then go to repository directory you'd like to start work on and type
```
dotnet tool install --tool-path tools dotnet-AutoGit
```

This will add AutoGit to your project as a tool.

> Directory with dotnet CLI tools should be configured to be ignored by Git.

You can also add AutoGit to as a global tool available in any directory on your computer.

```
dotnet tool install --global dotnet-AutoGit
```
That way it'll be easier to keep up with the latest version of AutoGit. For update type
```
dotnet tool update --global dotnet-AutoGit
```

> You can also update AutoGit installed in your project only

## How to use

Once you have AutoGit installed you can start auto-commit loop

```
dotnet-autogit
```

The default configuration will stage and commit all of your changes every minute.

#### Configure
To see configuration options type
```
dotnet-autogit --help
```

Here is an example
```
dotnet-autogit --directory E:\dev\repo --interval 5 --push
```

This will start auto-commit loop in `E:\dev\repo` Git repository with interval of 5 minutes and with pushing to remote after commiting all changes.

