using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories;

namespace PCMS_Backend.Repositories
{
    public class CoverageGapAlertRepository : ICoverageGapAlertRepository
    {
        private readonly PcmsDbContext _context;

        public CoverageGapAlertRepository(PcmsDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetOpenAlertsCountAsync()
        {
            // Ensure case matches your DB values exactly
            return await _context.CoverageGapAlerts
                .CountAsync(a => a.AlertStatus.ToUpper() == "OPEN");
        }

       
    }
}
