﻿Write-Host "Build Vts library Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts\Vts.csproj -c Debug
dotnet build $PWD\src\Vts\Vts.csproj -c Release

Write-Host "Build Vts.Test Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Release
Write-Host "Run Vts.Test Debug and Release" -ForegroundColor Green
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -l "console;verbosity=quiet" -c Debug
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c Release

Write-Host "Publish the VTS dll to platform specific folders" -ForegroundColor Green
Remove-Item $PWD\publish -Recurse -ErrorAction Ignore
dotnet publish $PWD\src\Vts\Vts.csproj -f net6.0 -c Release -o $PWD\publish\win-x64 --self-contained false
dotnet publish $PWD\src\Vts\Vts.csproj -f net6.0 -c Release -o $PWD\publish\linux-x64 --self-contained false
dotnet publish $PWD\src\Vts\Vts.csproj -f net6.0 -c Release -o $PWD\publish\osx-x64 --self-contained false
