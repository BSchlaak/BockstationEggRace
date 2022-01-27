using CsvHelper.Configuration.Attributes;
using System;

namespace Bockstation.EggRace.Data.Common.Models
{
    public class Result
    {
        [Name("Teamname")]
        public string TeamName { get; set; }
        [Name("Spieler")]
        public string PlayerName { get; set; }
        [Ignore]
        public TimeSpan StartTime { get; set; }
        [Name("Zwischenzeit1")]
        public TimeSpan? SplitTime1 { get; set; }
        [Name("Zwischenzeit2")]
        public TimeSpan? SplitTime2 { get; set; }
        [Name("Gesamtzeit")]
        public TimeSpan? TotalTime { get; set; }
    }
}
