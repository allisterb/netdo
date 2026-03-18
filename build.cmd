@echo off
echo Building netdo...
dotnet restore src\NetDo.sln
dotnet build src\NetDo.Cli\NetDo.Cli.csproj /p:Configuration=Debug