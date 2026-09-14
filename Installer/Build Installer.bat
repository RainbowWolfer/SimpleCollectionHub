@echo off

echo Compiling...

iscc "%~dp0Installer_SimpleCollectionHub.iss"

if %errorlevel% equ 0 (
    echo Success
) else (
    echo Fail
)
pause