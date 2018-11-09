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