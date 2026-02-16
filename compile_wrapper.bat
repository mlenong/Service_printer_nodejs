@echo off
set CSC_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    echo CSC not found at %CSC_PATH%
    echo Searching for csc.exe...
    for /f "delims=" %%i in ('where /r C:\Windows\Microsoft.NET csc.exe') do set CSC_PATH="%%i" & goto Found
)

:Found
echo Using CSC at %CSC_PATH%
"%CSC_PATH%" /target:winexe /out:ServicePrintTray.exe /win32icon:printer.ico src\ServicePrintTray.cs
if %errorlevel% neq 0 (
    echo Compilation failed!
    pause
    exit /b %errorlevel%
)
echo Compilation successful. ServicePrintTray.exe created.
