using SalesWebMvc.Data;
using System.Collections.Generic;
using System.Linq;
using SalesWebMvc.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services {
    public class DepartmentService {
        private readonly SalesWebMvcContext _context;

        public DepartmentService(SalesWebMvcContext context) {
            _context = context;
        }

        public async Task<List<Department>> FindAllAsync() {
            return await _context.Department.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task InsertAsync(Department obj) {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<Department> FindByIdAsync(int id) {
            return await _context.Department
                .FirstOrDefaultAsync(sel => sel.Id == id);
        }

        public async Task RemoveByIdAsync(int id) {
            try {
                var obj = await _context.Department.FindAsync(id);
                _context.Department.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                throw new IntegrityException("Can't delete department because it has sellers");
            }
        }

        public async Task UpdateAsync(Department obj) {
            bool hasAny = await _context.Department.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny) {
                throw new NotFoundException("Department ID not found");
            }
            try {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
