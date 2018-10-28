# AutoGit
[![NuGet](https://img.shields.io/nuget/v/dotnet-AutoGit.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-AutoGit/)
[![NuGet](https://img.shields.io/nuget/dt/dotnet-AutoGit.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-AutoGit/)


## Goal
Create tool that will commit changes in git repository for every set period of time.

## Libs
* Hangfire with Memory Storage
* Some UI framework (later)
* M$ Command line extensions
* https://github.com/libgit2/libgit2sharp For Git

## Links
https://discuss.hangfire.io/t/console-dashboard/192

## Ideas
* as dotnet tool
* args
  * --gui
  * --src
  * --cron
  * --push
  * global/local modes


## Install

Make sure you have [.NET Core SDK](https://www.microsoft.com/net/download) installed.

Then go to repository directory you'd like to start work on and type
```
dotnet install --tool-path tools dotnet-AutoGit
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

> You can also update tool installed only in your project

## How to use

#### List all available commands
```
dotnet-autogit --help
```
Though `-h` or `-?` will also work

This will print all available commands (under the label `commands`) you can run.

On each command you can invoke `--help` for more details what this command does.

This will print `start` command details
```
dotnet-autogit start --help
```

#### Start auto committing

`start` command allows to [cron](https://en.wikipedia.org/wiki/Cron) set up an job which will stage and commit all of your changes in the repository.

Each commit will have message contaning time when it was made.
```
PS> dotnet-autogit start --src E:\dev\your-fav-repo ` 
                         --username "Gordon Freeman" `
                         --email "gordon.freeman@hl2.nova"
```

TODO Add demo gif