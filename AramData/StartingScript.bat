@ECHO OFF

  ECHO "Client: Summoners"
  start C:\Users\Tijmen\source\repos\tiplg\AramStats\AramData\SummonerFinderV4\bin\Debug\SummonerFinderV4.exe 1
  timeout 10

FOR /L %%A IN (1,1,5) DO (
  ECHO "Client: "%%A
  start C:\Users\Tijmen\source\repos\tiplg\AramStats\AramData\GameFinderV4\bin\Debug\GameFinderV4.exe %%A
  timeout 10
)
