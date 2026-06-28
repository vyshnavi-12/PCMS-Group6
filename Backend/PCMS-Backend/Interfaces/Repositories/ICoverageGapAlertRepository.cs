using System.Threading.Tasks;

namespace PCMS_Backend.Interfaces.Repositories
{
    public interface ICoverageGapAlertRepository
    {
        Task<int> GetOpenAlertsCountAsync();
    }
}
