using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;

namespace SalesWebMvc.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async  Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result 
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<IGrouping<Department, SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            // Executa a query no banco sem o GroupBy
            var query = _context.SalesRecord
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .Where(x => (!minDate.HasValue || x.Date >= minDate.Value) &&
                            (!maxDate.HasValue || x.Date <= maxDate.Value))
                .OrderByDescending(x => x.Date);

            // Carrega os dados na memória
            var salesRecords = await query.ToListAsync();

            // Executa o GroupBy na memória
            var groupedResult = salesRecords
                .GroupBy(x => x.Seller.Department)
                .ToList();

            return groupedResult;
        }
    }
}
