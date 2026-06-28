using System.Threading.Tasks;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services
{
    public class CoverageGapAlertService : ICoverageGapAlertService
    {
        private readonly ICoverageGapAlertRepository _repository;

        public CoverageGapAlertService(ICoverageGapAlertRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<int>> GetOpenAlertsCountAsync()
        {
            var count = await _repository.GetOpenAlertsCountAsync();
            return Result<int>.Ok(count);
        }

       
    }
}


