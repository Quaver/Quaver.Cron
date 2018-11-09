/*
 * Copyright (c) 2018 Swan <me@swan.moe>
 * Licensed under the Apache 2.0 License
 * https://github.com/Swan/Quaver.Cron/blob/master/LICENSE
 */
using System;
using System.Drawing;
using Console = Colorful.Console;

namespace Quaver.Cron.Config
{
    public class Configuration
    {
        public string SQLHost { get; }
        public string SQLUsername { get; }
        public string SQLPassword { get; }
        public string SQLDatabase { get; }
        public string RedisServer { get; }
        public string RedisPassword { get; }
        public int Workers { get; }

        /// <summary>
        ///     Determines if we want to populate the leaderboards in Redis with user ratings.
        /// </summary>
        public bool PopulateLeaderboards { get; }

        /// <summary>
        ///     Determines if during the job we want to fix multiple personal best scores
        ///     on a map by a user.
        /// </summary>
        public bool FixMultiplePersonalBestScores { get; }

        /// <summary>
        ///     Determines if during the cron, we'll remove scores that are no longer on ranked maps.
        /// </summary>
        public bool SyncScoresWithRankedStatus { get; }

        /// <summary>
        ///     Determines if during the cron, we'll calculate the overall accuracy of users.
        /// </summary>
        public bool RecalculateOverallAccuracy { get; }

        /// <summary>
        ///     Detrmines if during the cron, we'll calculate the overall performance rating for users.
        /// </summary>
        public bool RecalculateOverallPerformanceRating { get; }

        /// <summary>
        ///     The path of the config file.
        /// </summary>
        private static string ConfigPath { get; } = $"./.env";

        /// <summary>
        /// </summary>
        public Configuration()
        {
            try
            {
                DotNetEnv.Env.Load(ConfigPath);

                SQLHost = DotNetEnv.Env.GetString("SQLHost");
                SQLUsername = DotNetEnv.Env.GetString("SQLUsername");
                SQLPassword = DotNetEnv.Env.GetString("SQLPassword");
                SQLDatabase = DotNetEnv.Env.GetString("SQLDatabase");
                RedisServer = DotNetEnv.Env.GetString("RedisServer");
                RedisPassword = DotNetEnv.Env.GetString("RedisPassword");
                Workers = DotNetEnv.Env.GetInt("Workers");
                PopulateLeaderboards = DotNetEnv.Env.GetBool("PopulateLeaderboards");
                FixMultiplePersonalBestScores = DotNetEnv.Env.GetBool("FixMultiplePersonalBestScores");
                SyncScoresWithRankedStatus = DotNetEnv.Env.GetBool("SyncScoresWithRankedStatus");
                RecalculateOverallAccuracy = DotNetEnv.Env.GetBool("RecalculateOverallAccuracy");
                RecalculateOverallPerformanceRating = DotNetEnv.Env.GetBool("RecalculateOverallPerformanceRating");
            }
            catch (Exception e)
            {
                Console.WriteLine("Config file could not be loaded.", Color.Red);
                Console.WriteLine(e, Color.Red);
                Environment.Exit(-1);
            }

            Console.WriteLine($"Config file successfully loaded!", Color.LimeGreen);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"PopulateLeaderboards = {PopulateLeaderboards}\n" +
                                             $"FixMultiplePersonalBestScores = {FixMultiplePersonalBestScores}\n" +
                                             $"SyncScoresWithRankedStatus = {SyncScoresWithRankedStatus}\n" +
                                             $"RecalculateOverallAccuracy = {RecalculateOverallAccuracy}\n" +
                                             $"RecalculateOverallPerformanceRating = {RecalculateOverallPerformanceRating}";
    }
}