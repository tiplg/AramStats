# AramStats
Tools to find ARAM game stats. Used for [HowlingAbyss.xyz](http://howlingabyss.xyz).

GameFinder uses the GAME-V1.3 API to find aram gameids. It also uses the same api to find the summonerids of aram player for future scraping of gameids.

StatScraper uses the MATCH-V2.2 to record the stats of aram games. These stats are entered in a database per champion. This database can be used on the website to display statistics per champions such as win rate, pick rate and average cs.

Both will be moved to the MATCH-V3 API when RiotSharp gets updated.

Data is saved in a Mysql database.

## Libraries used

This repo uses [RiotSharp](https://github.com/BenFradet/RiotSharp).

## Disclaimer

HowlingAbyss.xyz / AramStats isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc. League of Legends © Riot Games, Inc.
