:: Author: Kint <Kint@gamefield.com>
:: Title: NosHeat Install Server Script for Windows
:: Description: This script will launch NosHeat emulator for Windows Operating Systems.
@echo OFF

:: Define constants
set DIRECTORY=C:Kint-Sources-master\OpenNos.Master.Server\bin\Release
set EXECUTABLE_NAME=OpenNos.Master.Server.exe

%DIRECTORY%\%EXECUTABLE_NAME%

:EOF