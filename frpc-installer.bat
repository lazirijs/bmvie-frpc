@echo off
:: Define the URL of the PowerShell script
set "scriptUrl=https://raw.githubusercontent.com/BmTechT/frpc/4735cae6a0122de57c2f2a4ad8d2f379f5a5d641/frpc-v2.ps1"
:: Define the local path where the script will be saved
set "scriptPath=%temp%\frpc-v2.ps1"

:: Check if the script is running with administrator privileges
openfiles >nul 2>&1
if %errorlevel% neq 0 (
    :: If not running as admin, relaunch the batch file with elevated permissions
    echo Requesting administrative privileges...
    powershell -Command "Start-Process cmd -ArgumentList '/c %~s0' -Verb runAs"
    exit /b
)

:: Download the PowerShell script
echo Downloading the PowerShell script...
powershell.exe -Command "Invoke-WebRequest -Uri '%scriptUrl%' -OutFile '%scriptPath%'"

:: Check if the download was successful
if exist "%scriptPath%" (
    :: Run the PowerShell script
    powershell.exe -ExecutionPolicy Bypass -File "%scriptPath%"
) else (
    echo Failed to download the script from '%scriptUrl%'.
)

REM Prevent the batch file from closing automatically
pause