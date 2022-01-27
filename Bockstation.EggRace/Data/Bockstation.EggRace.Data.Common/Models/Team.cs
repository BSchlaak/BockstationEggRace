using CsvHelper.Configuration.Attributes;
using System;

namespace Bockstation.EggRace.Data.Common.Models
{
    public class Team
    {
        [Name("Start")]
        public TimeSpan Start { get; set; }
        [Name("Ende")]
        public TimeSpan End { get; set; }
        [Name("Teamname")]
        public string TeamName { get; set; }
        [Name("Buchungsperson")]
        public string PersonName { get; set; }
        [Name("Eier")]
        public int Eggs { get; set; }
    }
}
