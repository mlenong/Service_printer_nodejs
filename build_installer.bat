@echo off
echo ==========================================
echo Building Service Print Installer
echo ==========================================

echo [1/4] Building Node.js Backend...
call build_backend.bat
if %errorlevel% neq 0 exit /b %errorlevel%

echo [2/4] Compiling Tray Application...
call compile_wrapper.bat
if %errorlevel% neq 0 exit /b %errorlevel%

echo [3/4] Verifying Assets...
if not exist "SumatraPDF.exe" (
    echo Error: SumatraPDF.exe not found!
    exit /b 1
)
if not exist "printer.ico" (
    echo Error: printer.ico not found!
    exit /b 1
)

echo [4/4] creating Installer with Inno Setup...
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" setup.iss
if %errorlevel% neq 0 (
    echo Installer creation failed!
    exit /b %errorlevel%
)

echo ==========================================
echo SUCCESS! Installer created: ServicePrintInstaller.exe
echo ==========================================
