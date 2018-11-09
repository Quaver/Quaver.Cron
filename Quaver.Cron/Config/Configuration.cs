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

        /// <summary>
        ///     Determines if we want to populate the leaderboards in Redis with user ratings.
        /// </summary>
        public bool PopulateLeaderboards { get; }

        /// <summary>
        ///     The path of the config file.
        /// </summary>
        private static string ConfigPath { get; } = $"{Program.WorkingDirectory}/.env";
        
        /// <summary>
        /// </summary>
        public Configuration()
        {
            Console.WriteLine($"Loading config at path: {ConfigPath}", Color.LimeGreen);

            try
            {
                DotNetEnv.Env.Load(ConfigPath);

                SQLHost = DotNetEnv.Env.GetString("SQLHost");
                SQLUsername = DotNetEnv.Env.GetString("SQLUsername");
                SQLPassword = DotNetEnv.Env.GetString("SQLPassword");
                SQLDatabase = DotNetEnv.Env.GetString("SQLDatabase");
                RedisServer = DotNetEnv.Env.GetString("RedisServer");
                RedisPassword = DotNetEnv.Env.GetString("RedisPassword");             
                PopulateLeaderboards = DotNetEnv.Env.GetBool("PopulateLeaderboards");
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
        public override string ToString()
        {
            return $"PopulateLeaderboards = {PopulateLeaderboards}";
        }
    }
}