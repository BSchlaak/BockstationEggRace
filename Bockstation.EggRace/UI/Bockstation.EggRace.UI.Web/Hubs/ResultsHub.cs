using Bockstation.EggRace.Common.Interfaces;
using Bockstation.EggRace.Data.Common.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Bockstation.EggRace.UI.Web.Hubs
{
    public class ResultsHub : Hub<IResultsHub<Result>>
    {
        public Task SendResult(Result result)
        {
            return Clients.All.ReceiveResult(result);
        }
    }
}
