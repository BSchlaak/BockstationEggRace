using Bockstation.EggRace.Common.Interfaces;
using Bockstation.EggRace.Data.Common.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bockstation.EggRace.Data.Common
{
    public class DataRepository : IDataRepository<Team, Result>
    {
        #region Constants
        public const string FilePathsKey = "FilePaths";
        public const string TeamsKey = "Teams";
        public const string ResultsKey = "Results";
        #endregion Constants

        #region Private fields
        private readonly IConfiguration _configuration;
        private readonly CsvConfiguration _csvConfiguration;
        #endregion Private fields

        #region Constructors
        public DataRepository(IConfiguration configuration)
        {
            _configuration = configuration;

            _csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };
        }
        #endregion Constructors

        #region Methods
        #region Public
        public string DownloadResultsFile()
        {
            var result = string.Empty;

            try
            {
                string filePath = GetResultsFilePath();
                result = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        public IEnumerable<Team> GetTeams()
        {
            var result = Enumerable.Empty<Team>();

            try
            {
                string filePath = GetTeamsFilePath();
                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader, _csvConfiguration))
                    {
                        result = csv.GetRecords<Team>().Where(t => t.Eggs > 0).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        public IEnumerable<Result> GetResults()
        {
            var result = Enumerable.Empty<Result>();

            try
            {
                string filePath = GetResultsFilePath();
                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader, _csvConfiguration))
                    {
                        result = csv.GetRecords<Result>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        public bool AddResult(Result result)
        {
            var results = GetResults().ToList();

            var resultToAdd = results
                .SingleOrDefault(r => r.TeamName.Equals(result.TeamName) && r.PlayerName.Equals(result.PlayerName));
            if (resultToAdd == null)
            {
                results.Add(result);

                return WriteResults(results);
            }

            return false;
        }

        public bool DeleteResult(Result result)
        {
            var results = GetResults().ToList();

            var resultToDelete = results
                .SingleOrDefault(r => r.TeamName.Equals(result.TeamName) && r.PlayerName.Equals(result.PlayerName));
            if (resultToDelete != null)
            {
                if (results.Remove(resultToDelete))
                {
                    return WriteResults(results);
                }
            }

            return false;
        }

        public bool UpdateResult(Result result)
        {
            var results = GetResults();

            var resultToUpdate = results
                .SingleOrDefault(r => r.TeamName.Equals(result.TeamName) && r.PlayerName.Equals(result.PlayerName));
            if (resultToUpdate != null)
            {
                resultToUpdate.SplitTime1 = result.SplitTime1;
                resultToUpdate.SplitTime2 = result.SplitTime2;
                resultToUpdate.TotalTime = result.TotalTime;

                return WriteResults(results);
            }

            return false;
        }

        public bool UploadTeamsFile(Stream content)
        {
            bool result = false;

            try
            {
                string filePath = GetTeamsFilePath();
                using (var file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    content.CopyTo(file);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }
        #endregion Public

        #region Private
        private string GetResultsFilePath()
        {
            return GetFilePath(ResultsKey);
        }

        private string GetTeamsFilePath()
        {
            return GetFilePath(TeamsKey);
        }

        private string GetFilePath(string filePathKey)
        {
            var result = string.Empty;

            try
            {
                result = _configuration.GetSection($"{FilePathsKey}:{filePathKey}").Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        private bool WriteResults(IEnumerable<Result> results)
        {
            bool result = false;

            try
            {
                string filePath = _configuration.GetSection($"{FilePathsKey}:{ResultsKey}").Value;
                using (var writer = new StreamWriter(filePath))
                {
                    using (var csv = new CsvWriter(writer, _csvConfiguration))
                    {
                        csv.WriteRecords(results);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }
        #endregion Private
        #endregion Methods
    }
}
