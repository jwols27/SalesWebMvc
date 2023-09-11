using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Services {
    public class SalesRecordService {

        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context) {
            _context = context;
        }

        public async Task InsertAsync(SalesRecord obj) {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<SalesRecord> FindByIdAsync(int id) {
            return await _context.SalesRecord.Include(obj => obj.Seller)
                .FirstOrDefaultAsync(sel => sel.Id == id);
        }

        public async Task RemoveByIdAsync(int id) {
            try {
                var obj = await _context.SalesRecord.FindAsync(id);
                _context.SalesRecord.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e) {
                throw new IntegrityException(e.Message);
            }
        }

        public async Task UpdateAsync(SalesRecord obj) {
            bool hasAny = await _context.SalesRecord.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny) {
                throw new NotFoundException("Sales record ID not found");
            }
            try {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) {
                throw new DbConcurrencyException(e.Message);
            }
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? min, DateTime? max) {
            var result = from obj in _context.SalesRecord select obj;
            if (min.HasValue)
                result = result.Where(x => x.Date >= min.Value);
            if (max.HasValue)
                result = result.Where(x => x.Date <= max.Value);

            result = result.Include(x => x.Seller).Include(x => x.Seller.Department);

            return await result.OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task<List<IGrouping<Department, SalesRecord>>> FindByDateGroupingAsync(DateTime? min, DateTime? max) {
            var result = from obj in _context.SalesRecord select obj;
            if (min.HasValue)
                result = result.Where(x => x.Date >= min.Value);
            if (max.HasValue)
                result = result.Where(x => x.Date <= max.Value);

            result = result.Include(x => x.Seller).Include(x => x.Seller.Department);

            return await result.OrderByDescending(x => x.Date)
                .GroupBy(x => x.Seller.Department)
                .ToListAsync();
        }
    }
}
