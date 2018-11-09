/*
 * Copyright (c) 2018 Swan <me@swan.moe>
 * Licensed under the Apache 2.0 License
 * https://github.com/Swan/Quaver.Cron/blob/master/LICENSE
 */
using Quaver.Cron.Config;

namespace Quaver.Cron.Database
{
    public static class SQL
    {
        /// <summary>
        /// </summary>
        public static string ConnString { get; set; } 

        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(Configuration config)
        {
            ConnString = $"Server={config.SQLHost};" +
                             $"User ID={config.SQLUsername};" +
                             $"Password={config.SQLPassword};" +
                             $"Database={config.SQLDatabase}";            
        }
    }
}