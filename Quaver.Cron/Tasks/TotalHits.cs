using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MySql.Data.MySqlClient;
using Quaver.API.Enums;
using Quaver.Cron.Database;
using Quaver.Cron.Objects;

namespace Quaver.Cron.Tasks
{
    public static class TotalHits
    {
        /// <summary>
        /// </summary>
        public static void Recalculate()
        {
            Console.WriteLine($"Starting task to recalculate user total hits...");

            try
            {

                using (var conn = new MySqlConnection(SQL.ConnString))
                {
                    conn.Open();

                    var cmd = new MySqlCommand()
                    {
                        Connection = conn,
                        CommandText = $"SELECT id FROM users"
                    };

                    var users = new List<int>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            users.Add(id);
                        }
                    }

                    foreach (var user in users)
                    {
                        UpdateTotalHitsForMode(conn, GameMode.Keys4, user);
                        UpdateTotalHitsForMode(conn, GameMode.Keys7, user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            Console.WriteLine($"Done recalculating user total hits!", Color.LimeGreen);
        }

        private static void UpdateTotalHitsForMode(MySqlConnection conn, GameMode mode, int user)
        {
            var cmd2 = new MySqlCommand()
            {
                Connection = conn,
                CommandText = $"SELECT SUM(count_marv) AS total_marv, SUM(count_perf) AS total_perf, " +
                              $"SUM(count_great) AS total_great, SUM(count_good) AS total_good, " +
                              $"SUM(count_okay) AS total_okay " +
                              "FROM scores WHERE user_id = @id AND mode = @mode"
            };

            cmd2.Prepare();
            cmd2.Parameters.AddWithValue("id", user);
            cmd2.Parameters.AddWithValue("mode", (int) mode);

            var judgements = new Dictionary<Judgement, int>()
            {
                {Judgement.Marv, 0},
                {Judgement.Perf, 0},
                {Judgement.Great, 0},
                {Judgement.Good, 0},
                {Judgement.Okay, 0}
            };

            using (var reader = cmd2.ExecuteReader())
            {
                if (!reader.HasRows)
                    return;

                while (reader.Read())
                {
                    var totalMarv = reader.GetValue(0);

                    if (totalMarv == null || totalMarv == DBNull.Value)
                        break;

                    judgements[Judgement.Marv] = int.Parse(totalMarv.ToString());
                    judgements[Judgement.Perf] = reader.GetInt32(1);
                    judgements[Judgement.Great] = reader.GetInt32(2);
                    judgements[Judgement.Good] = reader.GetInt32(3);
                    judgements[Judgement.Okay] = reader.GetInt32(4);
                }
            }

            var cmd3 = new MySqlCommand()
            {
                Connection = conn,
                CommandText = $"UPDATE user_stats_{mode.ToString().ToLower()} SET count_marv = @count_marv, " +
                              $"count_perf = @count_perf, count_great = @count_great, count_good = @count_good, " +
                              $"count_okay = @count_okay WHERE user_id = @id"
            };

            cmd3.Prepare();

            cmd3.Parameters.AddWithValue("count_marv", judgements[Judgement.Marv]);
            cmd3.Parameters.AddWithValue("count_perf", judgements[Judgement.Perf]);
            cmd3.Parameters.AddWithValue("count_great", judgements[Judgement.Great]);
            cmd3.Parameters.AddWithValue("count_good", judgements[Judgement.Good]);
            cmd3.Parameters.AddWithValue("count_okay", judgements[Judgement.Okay]);
            cmd3.Parameters.AddWithValue("id", user);

            cmd2.ExecuteNonQuery();
            Console.WriteLine($"Updated total hits for user: {user} - {mode} - {judgements.Values.Sum()}");
        }
    }
}