using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using Amib.Threading;
using Quaver.Cron.Config;
using Quaver.Cron.Database;
using Console = Colorful.Console;

namespace Quaver.Cron
{
    internal static class Program
    {
        /// <summary>
        ///     The path of the current executable.
        /// </summary>
        internal static string ExecutablePath => System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", "");

        /// <summary>
        ///     The current working directory of the executable.
        /// </summary>
        internal static string WorkingDirectory => Path.GetDirectoryName(ExecutablePath).Replace(@"file:\", "");

        /// <summary>
        ///     Thread pool used to run different tasks
        /// </summary>
        internal static SmartThreadPool Pool { get; } = new SmartThreadPool(new STPStartInfo
        {
            AreThreadsBackground = true,
            IdleTimeout = 600000,
            MaxWorkerThreads = 32,
            MinWorkerThreads = 8,
            ThreadPriority = ThreadPriority.AboveNormal
        });

        /// <summary>
        ///     The config file used to determine the type of things to perform on the cron.
        /// </summary>
        private static Configuration Config { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        internal static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            Config = new Configuration();
            
            Redis.Initialize(Config);
            SQL.Initialize(Config);
            Console.WriteLine(Config.ToString());

            if (Config.PopulateLeaderboards)
                Pool.QueueWorkItem(Leaderboard.Populate);


            while (Pool.CurrentWorkItemsCount != 0)
            {
            }
            
            stopwatch.Stop();            
            Console.WriteLine($"Cron has completed in {stopwatch.ElapsedMilliseconds / 1000} sec", Color.LimeGreen);
        }
    }
}
