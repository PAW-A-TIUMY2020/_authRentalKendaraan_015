using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalKendaraan_015.Models;

namespace RentalKendaraan_015.Controllers
{
    public class JaminansController : Controller
    {
        private readonly Rental_KendaraanContext _context;

        public JaminansController(Rental_KendaraanContext context)
        {
            _context = context;
        }

        // GET: Jaminans
        public async Task<IActionResult> Index(string jmn, string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            //list menyimpan ketersediaan
            var jmnList = new List<string>();

            //query mengambil data
            var jmnQuery = from d in _context.Jaminan orderby d.NamaJaminan select d.NamaJaminan;

            jmnList.AddRange(jmnQuery.Distinct());

            //Menampilkan di view
            ViewBag.jmn = new SelectList(jmnList);

            //memanggil db context
            var menu = from m in _context.Jaminan select m;

            //memilih dropdownlist Nama Jaminan
            if (!string.IsNullOrEmpty(jmn))
            {
                menu = menu.Where(x => x.NamaJaminan == jmn);
            }

            //Search data
            if (!string.IsNullOrEmpty(searchString))
            {
                menu = menu.Where(s => s.NamaJaminan.Contains(searchString));
            }


            //Membuat pagedlist
            ViewData["currentSort"] = sortOrder;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["currentFilter"] = searchString;

            //definisi jum;ah data pada halaman
            int pageSize = 5;

            //Sorting data
            ViewData["JaminanSortParam"] = string.IsNullOrEmpty(sortOrder) ? "jaminan_desc" : "";


            switch (sortOrder)
            {
                case "jaminan_desc":
                    menu = menu.OrderByDescending(s => s.NamaJaminan);
                    break;

                default:
                    menu = menu.OrderBy(s => s.NamaJaminan);
                    break;
            }


            return View(await PaginatedList<Jaminan>.CreateAsync(menu.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Jaminans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jaminan = await _context.Jaminan
                .FirstOrDefaultAsync(m => m.IdJaminan == id);
            if (jaminan == null)
            {
                return NotFound();
            }

            return View(jaminan);
        }

        // GET: Jaminans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jaminans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdJaminan,NamaJaminan")] Jaminan jaminan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jaminan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jaminan);
        }

        // GET: Jaminans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jaminan = await _context.Jaminan.FindAsync(id);
            if (jaminan == null)
            {
                return NotFound();
            }
            return View(jaminan);
        }

        // POST: Jaminans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdJaminan,NamaJaminan")] Jaminan jaminan)
        {
            if (id != jaminan.IdJaminan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jaminan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JaminanExists(jaminan.IdJaminan))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jaminan);
        }

        // GET: Jaminans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jaminan = await _context.Jaminan
                .FirstOrDefaultAsync(m => m.IdJaminan == id);
            if (jaminan == null)
            {
                return NotFound();
            }

            return View(jaminan);
        }

        // POST: Jaminans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jaminan = await _context.Jaminan.FindAsync(id);
            _context.Jaminan.Remove(jaminan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JaminanExists(int id)
        {
            return _context.Jaminan.Any(e => e.IdJaminan == id);
        }
    }
}
