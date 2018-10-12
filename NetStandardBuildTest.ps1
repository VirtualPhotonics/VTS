dotnet build .\src\Vts\Vts.csproj -c Debug
dotnet build .\src\Vts\Vts.csproj -c Release

dotnet build .\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet build .\src\Vts.Test\Vts.Test.csproj -c Release

dotnet test .\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet test .\src\Vts.Test\Vts.Test.csproj -c Release

Read-Host -Prompt "Press Enter to exit"
