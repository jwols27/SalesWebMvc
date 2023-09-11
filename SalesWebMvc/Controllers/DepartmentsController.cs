using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers {
    public class DepartmentsController : Controller {
        private readonly DepartmentService _departmentService;

        public DepartmentsController(DepartmentService departmentService) {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index() {
            var list = await _departmentService.FindAllAsync();
            return View(list);
        }


        public IActionResult Create() {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department) {

            if (!ModelState.IsValid) {
                return View(department);
            }

            await _departmentService.InsertAsync(department);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id) {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "ID not provided" });

            var obj = await _departmentService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = "ID not found" });

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            try {
                await _departmentService.RemoveByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e) {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }


        public async Task<IActionResult> Edit(int? id) {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "ID not provided" });

            var obj = await _departmentService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = "ID not found" });

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department) {
            if (!ModelState.IsValid) {
                return View(department);
            }

            if (id != department.Id)
                return RedirectToAction(nameof(Error), new { message = "ID mismatch" });

            try {
                await _departmentService.UpdateAsync(department);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e) {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        public IActionResult Error(string message) {
            var viewModel = new ErrorViewModel {
                Message = message,
                RequestId = Activity.Current?.Id
            };
            return View(viewModel);
        }
    }
}
