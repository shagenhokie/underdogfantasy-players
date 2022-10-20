# underdogfantasy-players
---
## Setup
To generate the LiteDB data file from the source json files (at repo root) that were downloaded from http://api.cbssports.com:
- In UnderdogFantasy.Players.DataImportTool\appsettings.json, update the BaseballPlayersJsonFilePath, BasketballPlayersJsonFilePath, and FootballPlayersJsonFilePath settings to point to the source json files at repo root; easiest just to use full path
- Still in UnderdogFantasy.Players.DataImportTool\appsettings.json, update LiteDatabaseFilePath to point to your local repo root dir; note that the tool will generate the specified file (e.g. c:\src\underdogfantasy-players\PlayerData.db) on first run
- Build the UnderdogFantasy.Players.DataImportTool project and run it; you should see the LiteDB data file appear shortly thereafter at the location specified by LiteDatabaseFilePath
---
## Run and Test
To run the service and test it:
- In UnderdogFantasy.Players.WebApi\appsettings.json, update the LiteDatabase.FilePath setting to be the full path to the LiteDB data file you generated above
- Build the UnderdogFantasy.Players.WebApi project and run it
---
Examples:
- Player with id=2139744
https://localhost:44348/players/2139744
- All players
https://localhost:44348/players
- All football players
https://localhost:44348/players?sport=football
- Players having last name that starts with 'H'
https://localhost:44348/players?last_name_initial=H
- Players that are 45 years old
https://localhost:44348/players?age=45
- Players 33 years old or older
https://localhost:44348/players?min_age=33
- Players 21 years old or younger
https://localhost:44348/players?max_age=21
- Players 20 to 25 years old
https://localhost:44348/players?min_age=20&max_age=25
- Quarterbacks
https://localhost:44348/players?position=QB
- Baseball starting pitchers that are 35 or older
https://localhost:44348/players?sport=baseball&position=SP&min_age=35
- Basketball centers that are 18 to 22 years old
https://localhost:44348/players?sport=basketball&position=C&min_age=18&max_age=22

The possibilities are endless. Enjoy!
