﻿using SalesWebMvc.Data;
using System.Collections.Generic;
using System.Linq;
using SalesWebMvc.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
    }
}
