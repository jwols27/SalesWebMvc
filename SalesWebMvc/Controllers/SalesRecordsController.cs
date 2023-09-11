using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SalesWebMvc.Controllers {
    public class SalesRecordsController : Controller {
        private readonly SalesRecordService _salesRecordService;
        private readonly SellerService _sellerService;

        public SalesRecordsController(SalesRecordService salesRecordService, SellerService sellerService) {
            _salesRecordService = salesRecordService;
            _sellerService = sellerService;
        }

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Create() {
            var sellers = await _sellerService.FindAllAsync();
            var viewModel = new SalesRecordFormViewModel { Sellers = sellers };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesRecord salesRecord) {
            if (!ModelState.IsValid) {
                var sellers = await _sellerService.FindAllAsync();
                var viewModel = new SalesRecordFormViewModel { SalesRecord = salesRecord, Sellers = sellers };
                return View(viewModel);
            }

            await _salesRecordService.InsertAsync(salesRecord);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id) {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "ID not provided" });

            var obj = await _salesRecordService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = "ID not found" });

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            try {
                await _salesRecordService.RemoveByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e) {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        public async Task<IActionResult> Details(int? id) {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "ID not provided" });

            var obj = await _salesRecordService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = "ID not found" });
            return View(obj);
        }

        public async Task<IActionResult> Edit(int? id) {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "ID not provided" });

            var obj = await _salesRecordService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = "ID not found" });

            List<Seller> sellers = await _sellerService.FindAllAsync();
            SalesRecordFormViewModel viewModel = new SalesRecordFormViewModel { SalesRecord = obj, Sellers = sellers };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SalesRecord salesRecord) {
            if (!ModelState.IsValid) {
                var sellers = await _sellerService.FindAllAsync();
                var viewModel = new SalesRecordFormViewModel { SalesRecord = salesRecord, Sellers = sellers };
                return View(viewModel);
            }

            if (id != salesRecord.Id)
                return RedirectToAction(nameof(Error), new { message = "ID mismatch" });

            try {
                await _salesRecordService.UpdateAsync(salesRecord);
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


        public async Task<IActionResult> SimpleSearch(DateTime? minDate, DateTime? maxDate) {
            if (!minDate.HasValue)
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            if (!maxDate.HasValue)
                maxDate = DateTime.Now;

            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");

            var result = await _salesRecordService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }

        public async Task<IActionResult> GroupingSearch(DateTime? minDate, DateTime? maxDate) {
            if (!minDate.HasValue)
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            if (!maxDate.HasValue)
                maxDate = DateTime.Now;

            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");

            var result = await _salesRecordService.FindByDateGroupingAsync(minDate, maxDate);
            return View(result);
        }
    }
}
