using Bockstation.EggRace.Common.Interfaces;
using Bockstation.EggRace.Data.Common.Models;
using Bockstation.EggRace.UI.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bockstation.EggRace.UI.Web.Services
{
    public class ResultsService : BackgroundService
    {
        #region Constants
        public const string MqttServerKey = "MqttServer";
        public const string MqttMeasurementsTopicKey = "MqttMeasurementsTopic";
        public const string MqttResultsTopicKey = "MqttResultsTopic";
        public const string SimulateKey = "Simulate";
        #endregion Constants

        #region Private fields
        private static Core.Mqtt.Connector _mqttConnector;

        private readonly IConfiguration _configuration;
        private readonly IDataRepository<Team, Result> _dataRepository;
        private readonly IHubContext<ResultsHub, IResultsHub<Result>> _resultsHub;
        private Result _currentResult;
        #endregion Private fields

        #region Constructors
        public ResultsService(IConfiguration configuration, IDataRepository<Team, Result> dataRepository, IHubContext<ResultsHub, IResultsHub<Result>> resultsHub)
        {
            _configuration = configuration;
            _dataRepository = dataRepository;
            _resultsHub = resultsHub;

            var mqttServer = _configuration.GetSection(MqttServerKey).Value;
            var mqttMeasurementsTopic = _configuration.GetSection(MqttMeasurementsTopicKey).Value;
            var mqttResultsTopic = _configuration.GetSection(MqttResultsTopicKey).Value;

            _mqttConnector ??= new Core.Mqtt.Connector(mqttServer, mqttMeasurementsTopic, mqttResultsTopic);
            _mqttConnector.MessageReceived += MqttConnector_MessageReceived;
        }
        #endregion Constructors

        #region Methods
        #region Public
        public void Start(Result result)
        {
            if (result.SplitTime1.HasValue || result.SplitTime2.HasValue || result.TotalTime.HasValue)
            {
                _dataRepository.DeleteResult(result);
            }

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
        private async void MqttConnector_MessageReceived(object sender, Tuple<long, long> e)
        {
            Console.WriteLine($"Message received: {e}");

            if (_currentResult != null)
            {
                switch (e.Item1)
                {
                    case 0:
                        if (_currentResult.StartTime == TimeSpan.Zero)
                        {
                            _currentResult.StartTime = ParseTimeSpan(e.Item2);
                        }
                        break;

                    case 1:
                        if (_currentResult.StartTime != TimeSpan.Zero &&
                            !_currentResult.SplitTime1.HasValue)
                        {
                            var splitTime1 = ParseTimeSpan(e.Item2);
                            _currentResult.SplitTime1 = splitTime1 - _currentResult.StartTime;
                        }
                        break;

                    case 2:
                        if (_currentResult.StartTime != TimeSpan.Zero &&
                            _currentResult.SplitTime1.HasValue &&
                            !_currentResult.SplitTime2.HasValue)
                        {
                            var splitTime2 = ParseTimeSpan(e.Item2);
                            _currentResult.SplitTime2 = splitTime2 - _currentResult.StartTime;
                        }
                        break;

                    case 3:
                        if (_currentResult.StartTime != TimeSpan.Zero &&
                            _currentResult.SplitTime1.HasValue &&
                            _currentResult.SplitTime2.HasValue &&
                            !_currentResult.TotalTime.HasValue)
                        {
                            var totalTime = ParseTimeSpan(e.Item2);
                            _currentResult.TotalTime = totalTime - _currentResult.StartTime;
                        }
                        break;
                }

                _dataRepository.UpdateResult(_currentResult);
                await _resultsHub.Clients.All.ReceiveResult(_currentResult);
                var payload = JsonSerializer.Serialize(_currentResult);
                await _mqttConnector.PublishResultsAsync(payload);

                if (_currentResult.TotalTime.HasValue)
                {
                    _currentResult = null;
                }
            }
        }
        #endregion Event handlers
    }
}
