using Bockstation.EggRace.Common.Interfaces;
using Bockstation.EggRace.Data.Common.Models;
using Bockstation.EggRace.UI.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bockstation.EggRace.UI.Web.Services
{
    public class ResultsService : BackgroundService
    {
        #region Constants
        public const string MqttServerKey = "MqttServer";
        public const string SimulateKey = "Simulate";
        #endregion Constants

        #region Private fields
        private readonly IConfiguration _configuration;
        private readonly IDataRepository<Team, Result> _dataRepository;
        private readonly IHubContext<ResultsHub, IResultsHub<Result>> _resultsHub;
        private readonly Core.Mqtt.Connector _mqttConnector;
        private Result _currentResult;
        #endregion Private fields

        #region Constructors
        public ResultsService(IConfiguration configuration, IDataRepository<Team, Result> dataRepository, IHubContext<ResultsHub, IResultsHub<Result>> resultsHub)
        {
            _configuration = configuration;
            _dataRepository = dataRepository;
            _resultsHub = resultsHub;

            var mqttServer = _configuration.GetSection(MqttServerKey).Value;
            _mqttConnector = new Core.Mqtt.Connector(mqttServer);
            _mqttConnector.MessageReceived += MqttConnector_MessageReceived;
        }
        #endregion Constructors

        #region Methods
        #region Public
        public void Start(Result result)
        {
            _currentResult = result;
            _dataRepository.AddResult(_currentResult);

            if (bool.TryParse(_configuration.GetSection(SimulateKey).Value, out var simulate) && simulate)
            {
                Simulate();
            }
        }

        public IEnumerable<Result> GetResults()
        {
            var results = _dataRepository.GetResults();
            return results;
        }
        #endregion Public

        #region Private
        private void Simulate()
        {
            if (_currentResult != null)
            {
                _currentResult.StartTime = DateTime.Now.TimeOfDay;

                Thread.Sleep(new Random(DateTime.Now.Millisecond).Next(2000));
                _currentResult.SplitTime1 = DateTime.Now.TimeOfDay - _currentResult.StartTime;
                _dataRepository.UpdateResult(_currentResult);
                _resultsHub.Clients.All.ReceiveResult(_currentResult);

                Thread.Sleep(new Random(DateTime.Now.Millisecond).Next(2000));
                _currentResult.SplitTime2 = DateTime.Now.TimeOfDay - _currentResult.StartTime;
                _dataRepository.UpdateResult(_currentResult);
                _resultsHub.Clients.All.ReceiveResult(_currentResult);

                Thread.Sleep(new Random(DateTime.Now.Millisecond).Next(2000));
                _currentResult.TotalTime = DateTime.Now.TimeOfDay - _currentResult.StartTime;
                _dataRepository.UpdateResult(_currentResult);
                _resultsHub.Clients.All.ReceiveResult(_currentResult);

                _currentResult = null;
            }
        }

        private TimeSpan ParseTimeSpan(long timestamp)
        {
            TimeSpan result = TimeSpan.Zero;

            if (int.TryParse(timestamp.ToString().Substring(0, 2), out var hours) &&
                int.TryParse(timestamp.ToString().Substring(2, 2), out var minutes) &&
                int.TryParse(timestamp.ToString().Substring(4, 2), out var seconds) &&
                int.TryParse(timestamp.ToString().Substring(6, 3), out var millis))
            {
                result = new TimeSpan(0, hours, minutes, seconds, millis);
            }

            return result;
        }

        private DateTime ParseDateTime(long timestamp)
        {
            DateTime result = DateTime.Today;

            if (int.TryParse(timestamp.ToString().Substring(0, 2), out var hours) &&
                int.TryParse(timestamp.ToString().Substring(2, 2), out var minutes) &&
                int.TryParse(timestamp.ToString().Substring(4, 2), out var seconds) &&
                int.TryParse(timestamp.ToString().Substring(6, 3), out var millis))
            {
                result = result.Add(new TimeSpan(0, hours, minutes, seconds, millis));
            }

            return result;
        }
        #endregion Private

        #region Overridden
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _mqttConnector.ConnectAsync(stoppingToken);
        }
        #endregion Overridden
        #endregion Methods

        #region Event handlers
        private void MqttConnector_MessageReceived(object sender, Tuple<long, long, long, long> e)
        {
            Console.WriteLine($"Message received: {e}");

            if (_currentResult != null)
            {
                if (e.Item1 > 0)
                {
                    _currentResult.StartTime = ParseTimeSpan(e.Item1);

                    if (e.Item2 == 0)
                    {
                        Console.WriteLine($"Started at: {_currentResult.StartTime}");
                    }
                    else
                    {
                        var splitTime1 = ParseTimeSpan(e.Item2);
                        _currentResult.SplitTime1 = splitTime1 - _currentResult.StartTime;

                        if (e.Item3 == 0)
                        {
                            Console.WriteLine(
                                $"First intermediate time at: {splitTime1} (after {_currentResult.SplitTime1})");
                        }
                        else
                        {
                            var splitTime2 = ParseTimeSpan(e.Item3);
                            _currentResult.SplitTime2 = splitTime2 - _currentResult.StartTime;

                            if (e.Item4 == 0)
                            {
                                Console.WriteLine(
                                    $"Second intermediate time at: {splitTime2} (after {_currentResult.SplitTime2})");
                            }
                            else
                            {
                                var totalTime = ParseTimeSpan(e.Item4);
                                _currentResult.TotalTime = totalTime - _currentResult.StartTime;

                                Console.WriteLine($"Finished at: {totalTime} (after {_currentResult.TotalTime})");
                            }
                        }

                        _dataRepository.UpdateResult(_currentResult);
                        _resultsHub.Clients.All.ReceiveResult(_currentResult);

                        if (_currentResult.TotalTime.HasValue)
                        {
                            _currentResult = null;
                        }
                    }
                }
            }
        }
        #endregion Event handlers
    }
}
