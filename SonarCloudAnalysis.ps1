#We have to rerun the coverage results and export to HTML to get the coverage percentage
#Run the coverage results in Visual Studio and export the snapshot TestResultCoverage.html 
dotnet sonarscanner begin -k:"VirtualPhotonics_VTS" /o:"virtualphotonics" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="{PLACE_LOGIN_TOKEN_HERE}" /d:sonar.c.file.suffixes=- /d:sonar.cpp.file.suffixes=- /d:sonar.objc.file.suffixes=- /d:sonar.cs.nunit.reportsPaths=TestResult.trx /d:sonar.cs.dotcover.reportsPaths=TestResultCoverage.html
dotnet build $PWD\src\VtsLibrary.sln /t:Rebuild /p:Configuration=Release
dotnet test $PWD\src\VtsLibrary.sln -c:Release -l:"trx;LogFileName=..\..\..\TestResult.trx"
dotnet sonarscanner end /d:sonar.login="{PLACE_LOGIN_TOKEN_HERE}"
Read-Host -Prompt "Press Enter to exit"