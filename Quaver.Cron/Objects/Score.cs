/*
 * Copyright (c) 2018 Swan <me@swan.moe>
 * Licensed under the Apache 2.0 License
 * https://github.com/Swan/Quaver.Cron/blob/master/LICENSE
 */

using Quaver.API.Enums;

namespace Quaver.Cron.Objects
{
    public class Score
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MapMd5 { get; set; }
        public long Timestamp { get; set; }
        public GameMode Mode { get; set; }
        public bool PersonalBest { get; set; }
        public double PerformanceRating { get; set; }
        public ModIdentifier Mods { get; set; }
        public bool Failed { get; set; }
        public int TotalScore { get; set; }
        public double Accuracy { get; set; }
    }
}