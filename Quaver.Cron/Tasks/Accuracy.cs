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
    public static class Accuracy
    {
        /// <summary>
        ///     Calculates the overall accuracy for all users.
        /// </summary>
        public static void RecalculateOverall()
        {
            Console.WriteLine($"Starting task to recalculate users' overall accuracy...");

            try
            {
                // Stores the scores for each user <user_id, scores>
                var scores = new Dictionary<int, List<Score>>();

                using (var conn = new MySqlConnection(SQL.ConnString))
                {
                    conn.Open();

                    var cmd = new MySqlCommand()
                    {
                        Connection = conn,
                        CommandText = "SELECT " +
                                            $"s.user_id, s.mode, s.accuracy " +
                                      $"FROM scores s " +
                                      $"INNER JOIN users u ON u.id = s.user_id " +
                                      $"WHERE personal_best = 1 AND s.is_donator_score = 0 " +
                                      $"ORDER BY s.performance_rating DESC"
                    };

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;

                        while (reader.Read())
                        {
                            var score = new Score
                            {
                                UserId = reader.GetInt32(0),
                                Mode = (GameMode) reader.GetSByte(1),
                                Accuracy = reader.GetDouble(2)
                            };

                            if (!scores.ContainsKey(score.UserId))
                                scores[score.UserId] = new List<Score>();

                            scores[score.UserId].Add(score);
                        }
                    }

                    foreach (var user in scores)
                    {
                        foreach (GameMode mode in Enum.GetValues(typeof(GameMode)))
                        {
                            var overallAccuracy = CalculateOverallAccuracy(user.Value.FindAll(x => x.Mode == mode).ToList());

                            var updateCmd = new MySqlCommand()
                            {
                                Connection = conn,
                                CommandText =  $"UPDATE user_stats_{mode.ToString().ToLower()} " +
                                               $"SET overall_accuracy = @acc WHERE user_id = @id"
                            };

                            updateCmd.Prepare();
                            updateCmd.Parameters.AddWithValue("id", user.Key);
                            updateCmd.Parameters.AddWithValue("acc", overallAccuracy);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e, Color.Red);
                return;
            }

            Console.WriteLine($"Done calculating overall accuracy for users!", Color.LimeGreen);
        }

        /// <summary>
        ///     Calculates overall accuracy from a list of scores
        ///
        ///     (Should be given as ordered)
        /// </summary>
        /// <param name="scores"></param>
        /// <returns></returns>
        private static double CalculateOverallAccuracy(List<Score> scores)
        {
            var total = 0d;
            var divideTotal = 0d;

            for (var i = 0; i < scores.Count; i++)
            {
                var add = Math.Pow(0.95f, i) * 100;
                total += scores[i].Accuracy * add;
                divideTotal += add;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (divideTotal == 0)
                return 0;

            return total / divideTotal;
        }
    }
}
