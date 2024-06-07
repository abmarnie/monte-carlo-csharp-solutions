## Dependencies
[Latest .NET SDK](https://dotnet.microsoft.com/en-us/download)

## Downloading
`git clone https://github.com/abmarnie/monte-carlo-csharp-solutions.git`

## Running
`dotnet build`

`dotnet run`

## Distributing
`dotnet publish -c Release -r win-x64 --self-contained` (for example, but for more details, see [this](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)).

Zip up the `bin\Release\net8.0\win-x64\publish` folder, and send that to someone. They can double click on `GameDevMathProblems.exe` to run the application.
