using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using Quaver.Cron.Database;

namespace Quaver.Cron
{
    public static class Leaderboard
    {
        /// <summary>
        ///     Populates the global and country leaderboards in redis.
        /// </summary>
        public static void Populate()
        {
            Console.WriteLine($"Populating leaderboards in Redis...", Color.LimeGreen);

            
            try
            {
                foreach (var ep in Redis.Multiplexer.GetEndPoints())
                {
                    var database = Redis.Multiplexer.GetDatabase(); 
                    var server = Redis.Multiplexer.GetServer(ep);

                    // Get the keys for country & global leaderboards.
                    var countryKeys = server.Keys(database.Database, "quaver:country_leaderboard:*").ToList();
                    var keys = server.Keys(database.Database, "quaver:leaderboard:*").ToList();   
                    
                    // Delete existing keys
                    keys.ForEach(x => database.KeyDelete(x));
                    countryKeys.ForEach(x => database.KeyDelete(x));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }
    }
}