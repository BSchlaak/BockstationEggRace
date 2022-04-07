using System.Collections.Generic;
using System.IO;

namespace Bockstation.EggRace.Common.Interfaces
{
    public interface IDataRepository<out TTeam, TResult>
        where TTeam : class
        where TResult : class
    {
        string DownloadResultsFile();

        IEnumerable<TTeam> GetTeams();

        IEnumerable<TResult> GetResults();

        bool AddResult(TResult result);

        bool DeleteResult(TResult result);

        bool UpdateResult(TResult result);

        bool UploadTeamsFile(Stream content);
    }
}
