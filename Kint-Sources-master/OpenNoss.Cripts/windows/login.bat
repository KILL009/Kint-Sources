:: Author: Kint <Kint@gamefield.com>
:: Title: NosHeat Install Server Script for Windows
:: Description: This script will launch NosHeat emulator for Windows Operating Systems.
@echo OFF

:: Define constants
set DIRECTORY=dist\bin\login
set EXECUTABLE_NAME=OpenNos.Login.exe

%DIRECTORY%\%EXECUTABLE_NAME%

:EOF