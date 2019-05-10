@ECHO OFF
set /p snum="Number of Summoner Clients: "

FOR /L %%A IN (1,1,%snum%) DO (
  ECHO "Client: "%%A
  start C:\Users\Tijmen\source\repos\tiplg\AramStats\AramData\SummonerFinderV4\bin\Debug\SummonerFinderV4.exe %%A
  timeout 10
)
