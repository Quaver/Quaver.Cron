# Quaver.Cron [![Discord](https://discordapp.com/api/guilds/354206121386573824/widget.png?style=shield)](https://discord.gg/nJa8VFr)

>🕒 A cron for Quaver to perform automated tasks related to its database.

Developed for internal purposes. No support or documentation for this software is provided.

## Usage

You'll need [.NET Core 2.0](https://www.microsoft.com/net/download/dotnet-core/2.0) to run the cron.

```bash
git clone --recurse-submodules https://github.com/Quaver/Quaver.Cron
dotnet build --configuration release
cd bin/Release/netcoreapp2.0
nano .env
(Setup Config)
dotnet Quaver.Cron.dll
```

## Configuration

The cron is completely configurable and runs only the tasks that are given. Assuming you have a `.env` already created, here is a sample config file.

```ini
SQLHost = 
SQLUsername = 
SQLPassword = 
SQLDatabase = 

RedisServer = 
RedisPassword =

Workers = 8

PopulateLeaderboards = True
FixMultiplePersonalBestScores = True
SyncScoresWithRankedStatus = True
RecalculateOverallAccuracy = True
RecalculateOverallPerformanceRating = True
```

# LICENSE

All of the code in this repository is licensed under the Apache 2.0 License. 

View the [LICENSE](https://github.com/Swan/Quaver.Cron/blob/master/LICENSE) file for more information.
