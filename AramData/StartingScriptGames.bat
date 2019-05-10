@ECHO OFF
set /p num="Number of Clients: "

FOR /L %%A IN (1,1,%num%) DO (
  ECHO "Client: "%%A
  start C:\Users\Tijmen\source\repos\tiplg\AramStats\AramData\GameFinderV4\bin\Debug\GameFinderV4.exe %%A
  timeout 10
)
