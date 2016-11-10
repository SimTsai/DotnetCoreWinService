# DotnetCoreWinService
Windows service container of dotnet core

A container for run dotnet core in windows service

##How to use.
- 1. run command as administrator create service for this container.
```
sc create <serviceName> binPath= <path for this container>
```
- 2. config container, edit config.json
```
{
  "dotnetPath": "<path to your dotnet.exe or blank use envPath>",
  "configs": [
    {
      "assemblyPath": "<path to your dotnet core entrypoint>.dll",
      "workDirectory": "blank or path to your work directory",
      "parameter": "parameters"
    }
  ]
}
```
- 3. run command as administrator to run the container.
```
sc start <serviceName>
```
