using System.Threading.Tasks;

namespace Bockstation.EggRace.Common.Interfaces
{
    public interface IResultsHub<in T>
        where T : class
    {
        Task ReceiveResult(T result);
    }
}
