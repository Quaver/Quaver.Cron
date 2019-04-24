/*
 * Copyright (c) 2018 Swan <me@swan.moe>
 * Licensed under the Apache 2.0 License
 * https://github.com/Swan/Quaver.Cron/blob/master/LICENSE
 */
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Quaver.API.Enums;
using Quaver.Cron.Database;
using Console = Colorful.Console;

namespace Quaver.Cron.Tasks
{
    public static class Leaderboard
    {
        /// <summary>
        ///     Populates the global and country leaderboards in redis.
        /// </summary>
        public static void Populate()
        {
            Console.WriteLine($"Starting task to populate leaderboards in Redis...");

            try
            {
                DeleteExistingKeys();
                CacheUsers();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, Color.Red);
                return;
            }

            Console.WriteLine("Done populating leaderboards in Redis!", Color.LimeGreen);
        }

        /// <summary>
        ///     Deletes existing leaderboard & country leaderboard keys in the database.
        /// </summary>
        private static void DeleteExistingKeys()
        {
            foreach (var ep in Redis.Multiplexer.GetEndPoints())
            {
                var database = Redis.Multiplexer.GetDatabase();
                var server = Redis.Multiplexer.GetServer(ep);

                // Get the keys for country & global leaderboards.
                var countryKeys = server.Keys(database.Database, "quaver:country_leaderboard:*").ToList();
                var keys = server.Keys(database.Database, "quaver:leaderboard:*").ToList();
                var multiplayerKeys = server.Keys(database.Database, "quaver:multiplayer_win_leaderboard:*").ToList();

                // Delete existing keys
                keys.ForEach(x => database.KeyDelete(x));
                countryKeys.ForEach(x => database.KeyDelete(x));
                multiplayerKeys.ForEach(x => database.KeyDelete(x));
            }
        }

        /// <summary>
        ///     Caches all non-banned users in the global and their respective country leaderboard.
        /// </summary>
        private static void CacheUsers()
        {
            var redis = Redis.Multiplexer.GetDatabase();

            foreach (GameMode mode in Enum.GetValues(typeof(GameMode)))
            {
                using (var conn = new MySqlConnection(SQL.ConnString))
                {
                    conn.Open();

                    var cmd = new MySqlCommand()
                    {
                        Connection = conn,
                        CommandText = $"SELECT " +
                                      $"u.id, u.country, s.overall_performance_rating, s.multiplayer_wins " +
                                      $"FROM " +
                                      $"users u " +
                                      $"INNER JOIN " +
                                        $"user_stats_{mode.ToString().ToLower()} s ON u.id = s.user_id " +
                                      $"WHERE " +
                                        $"allowed = 1"
                    };

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var country = reader.GetString(1);
                            var rating = reader.GetDouble(2);
                            var multiplayerWins = reader.GetInt32(3);

                            // Add to global leaderboard
                            redis.SortedSetAdd($"quaver:leaderboard:{(byte) mode}", id, rating);
                            redis.SortedSetAdd($"quaver:multiplayer_win_leaderboard:{(byte) mode}", id, multiplayerWins);

                            // Skip users with an unknown country.
                            if (country.ToUpper() == "XX")
                                continue;

                            // Add to country leaderboards.
                            redis.SortedSetAdd($"quaver:country_leaderboard:{country.ToLower()}:{(byte) mode}", id, rating);
                        }
                    }
                }
            }
        }
    }
}