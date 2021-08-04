dotnet sonarscanner begin -k:"lmalenfant_VTS" /o:"lmalenfant" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="{PLACE_LOGIN_TOKEN_HERE}" /d:sonar.c.file.suffixes=- /d:sonar.cpp.file.suffixes=- /d:sonar.objc.file.suffixes=- /d:sonar.cs.nunit.reportsPaths=TestResult.trx /d:sonar.cs.dotcover.reportsPaths=TestResultCoverage.html
dotnet build $PWD\src\Vts\Vts.csproj /t:Rebuild /p:Configuration=Release
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c:Release -l:"trx;LogFileName=..\..\..\TestResult.trx"
dotnet sonarscanner end /d:sonar.login="{PLACE_LOGIN_TOKEN_HERE}"
Read-Host -Prompt "Press Enter to exit"

