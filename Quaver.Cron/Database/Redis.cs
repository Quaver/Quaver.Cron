using System;
using System.Drawing;
using Quaver.Cron.Config;
using ServiceStack.Redis;
using StackExchange.Redis;

namespace Quaver.Cron.Database
{
    public static class Redis
    {
        /// <summary>
        ///     Redis client.
        /// </summary>
        public static ConnectionMultiplexer Multiplexer { get; private set;  }

        /// <summary>
        ///     Initializes the redis connection.
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(Configuration config)
        {
            var connection = !string.IsNullOrEmpty(config.RedisPassword) ? $"{config.RedisPassword}@{config.RedisServer}" : $"{config.RedisServer}";
            Multiplexer = ConnectionMultiplexer.Connect(connection);
            
            Console.WriteLine($"Successfully created RedisManagerPool", Color.LimeGreen);
        }
    }
}