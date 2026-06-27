using System.Threading.Tasks;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services
{
    public interface ICoverageGapAlertService
    {
        Task<Result<int>> GetOpenAlertsCountAsync();
    }
}
