SET fwpath="%windir%\Microsoft.NET\Framework\v4.0.30319"

SET mypath=%~dp0
echo %mypath:~0,-1%

%fwpath%\regasm.exe /tlb /codebase "%mypath%\EchoFileSplitterActiveX.dll"


pause

%fwpath%\regasm.exe /tlb /codebase "%mypath%\Newtonsoft.Json.dll"
pause