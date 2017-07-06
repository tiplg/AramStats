# AramStats
Tools to find ARAM game stats. Used for HowlingAbyss.xyz.

GameFinder uses the CHAMPION-V1.2 API to find aram gameids. It also uses the same api to find the summonerids of aram player for future scraping of gameids.

StatScraper uses the MATCH-V2.2 to record the stats of aram games. These stats are entered in a database per champion. This database can be used on the website to display statistics per champions such as win rate, pick rate and average cs.

Right now both programs use .xml to save the data this will be changed to mySQL shortly. summonerids are not linked to the gameids so no per player stats can be displayed.
