cd %~dp0OpenNos.Master.Server\bin\Debug
start OpenNos.Master.Server.exe
timeout 2
:: edit to have wanted amount of world servers,
:: dont forget to wait about 5 seconds before starting next world server
:: cd %~dp0OpenNos.World\bin\Debug
:: start OpenNos.World.exe
:: timeout 20
cd %~dp0OpenNos.Login\bin\Debug
start OpenNos.Login.exe
exit