/*
 * Copyright (c) 2018 Swan <me@swan.moe>
 * Licensed under the Apache 2.0 License
 * https://github.com/Swan/Quaver.Cron/blob/master/LICENSE
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MySql.Data.MySqlClient;
using Quaver.API.Enums;
using Quaver.Cron.Database;
using Quaver.Cron.Objects;
using Console = Colorful.Console;

namespace Quaver.Cron.Tasks
{
    public static class Scores
    {
        /// <summary>
        ///     Goes through user scores and fixes if they have multiple best scores on the same
        ///     map
        /// </summary>
        public static void FixMultiplePersonalBests()
        {
            Console.WriteLine($"Starting task to fix multiple personal best scores...");

            try
            {
                var scores = new List<Score>();
                
                using (var conn = new MySqlConnection(SQL.ConnString))
                {
                    conn.Open();

                    var cmd = new MySqlCommand()
                    {
                        Connection = conn,
                        CommandText = $"SELECT " +
                                            $"id, user_id, map_md5, mode, performance_rating " +
                                      $"FROM " +
                                        $"scores " +
                                      $"WHERE personal_best = 1 AND is_donator_score = 0"
                    };

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;

                        while (reader.Read())
                        {
                            scores.Add(new Score()
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                MapMd5 = reader.GetString(2),
                                Mode = (GameMode) reader.GetSByte(3),
                                PerformanceRating = reader.GetDouble(4)
                            });
                        }
                    } 
                    
                    var fixedScores = new List<int>();
                
                    // Go through each score and fix duplicate completed ones.
                    foreach (var score in scores)
                    {
                        if (fixedScores.Contains(score.Id))
                            continue;
                    
                        var mapScores = scores.FindAll(x => x.MapMd5 == score.MapMd5 && x.UserId == score.UserId)
                            .OrderByDescending(x => x.PerformanceRating).ToList();

                        if (mapScores.Count > 1)
                        {
                            Console.WriteLine($"Found multiple personal best scores for user: {score.UserId} | {score.MapMd5}", Color.Yellow);   
                            
                            var topScore = mapScores.First();

                            foreach (var s in mapScores)
                            {
                                var updateCmd = new MySqlCommand()
                                {
                                    Connection = conn,
                                    CommandText =  s == topScore ? 
                                        "UPDATE scores SET personal_best = 1 WHERE id = @id" 
                                        : "UPDATE scores SET personal_best = 0 WHERE id = @id"
                                }; 
                                
                                updateCmd.Prepare();
                                updateCmd.Parameters.AddWithValue("id", s.Id);
                                updateCmd.ExecuteNonQuery();
                                
                                fixedScores.Add(s.Id);  
                            }  
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e, Color.LimeGreen);
                return;
            }
            
            Console.WriteLine($"Done fixing multiple personal best scores!", Color.LimeGreen);
        }

        /// <summary>
        ///     When the ranked status of a map is no longer ranked, this will go through and delete all of the scores
        ///     for that map.
        /// </summary>
        public static void SyncScoresWithRankedStatus()
        {
            Console.WriteLine("Starting task to sync scores with their maps ranked status...");

            try
            {
                using (var conn = new MySqlConnection(SQL.ConnString))
                {
                    conn.Open();

                    var cmd = new MySqlCommand()
                    {
                        Connection = conn,
                        CommandText = "DELETE " +
                                            "s.* FROM scores s " +
                                      "LEFT JOIN " +
                                        "maps m ON s.map_md5 = m.md5 " +
                                      $"WHERE m.ranked_status <= {(byte) RankedStatus.Unranked} AND s.is_donator_score = 0"
                    };
                    
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e, Color.LimeGreen);
                return;
            }
            
            Console.WriteLine($"Done syncing scores with their ranked status!", Color.LimeGreen);
        }        
    }
}
