# AramStats
Tools to find ARAM game stats. Used for [HowlingAbyss.xyz](http://howlingabyss.xyz).

GameFinder uses the Game-V1.3 API to check the recent matches of known ARAM players. The recent matches get checked against the database. If there are any new aram matches these get added to the database. The players of the aram matches are also added to the database.

StatScraper uses the MATCH-V2.2 to record the stats of aram games. These stats are entered in a database per champion. This database can be used by the website to display statistics per champions such as win rate, pick rate and average cs.

Both programs will be moving to MATCH-V3 API when RiotSharp gets updated.

## Libraries used

This repo uses [RiotSharp](https://github.com/BenFradet/RiotSharp).

## Disclaimer

HowlingAbyss.xyz / AramStats isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc. League of Legends © Riot Games, Inc.
