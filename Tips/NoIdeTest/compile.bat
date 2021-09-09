@echo off
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
/r:C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll ^
%1
%~n1.exe

echo.
echo "compile.bat" done.
pause