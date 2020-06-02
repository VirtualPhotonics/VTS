#set EnableNuGetPackageRestore=true

$nunit="$PWD\tools\nunit\Desktop\nunit3-console"

& "$nunit" "/result:TestResult-Vts-Debug.xml" "$PWD\src\Vts.Desktop.Test\bin\Debug\Vts.Desktop.Test.dll" 
& "$nunit" "/result:TestResult-Vts-Release.xml" "$PWD\src\Vts.Desktop.Test\bin\Release\Vts.Desktop.Test.dll"
