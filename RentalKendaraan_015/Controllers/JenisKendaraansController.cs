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
    public class JenisKendaraansController : Controller
    {
        private readonly Rental_KendaraanContext _context;

        public JenisKendaraansController(Rental_KendaraanContext context)
        {
            _context = context;
        }

        // GET: JenisKendaraans
        public async Task<IActionResult> Index(string jnsk, string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            //list menyimpan ketersediaan
            var jnskList = new List<string>();

            //query mengambil data
            var jnskQuery = from d in _context.JenisKendaraan orderby d.NamaJenisKendaraan select d.NamaJenisKendaraan;

            jnskList.AddRange(jnskQuery.Distinct());

            //Menampilkan di view
            ViewBag.jnsk = new SelectList(jnskList);

            //memanggil db context
            var menu = from m in _context.JenisKendaraan select m;

            //memilih dropdownlist Nama JenisKendaraan
            if (!string.IsNullOrEmpty(jnsk))
            {
                menu = menu.Where(x => x.NamaJenisKendaraan == jnsk);
            }

            //Search data
            if (!string.IsNullOrEmpty(searchString))
            {
                menu = menu.Where(s => s.NamaJenisKendaraan.Contains(searchString));
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
            ViewData["JenisSortParam"] = string.IsNullOrEmpty(sortOrder) ? "jenis_desc" : "";


            switch (sortOrder)
            {
                case "jenis_desc":
                    menu = menu.OrderByDescending(s => s.NamaJenisKendaraan);
                    break;

                default:
                    menu = menu.OrderBy(s => s.NamaJenisKendaraan);
                    break;
            }


            return View(await PaginatedList<JenisKendaraan>.CreateAsync(menu.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: JenisKendaraans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jenisKendaraan = await _context.JenisKendaraan
                .FirstOrDefaultAsync(m => m.IdJenisKendaraan == id);
            if (jenisKendaraan == null)
            {
                return NotFound();
            }

            return View(jenisKendaraan);
        }

        // GET: JenisKendaraans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JenisKendaraans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdJenisKendaraan,NamaJenisKendaraan")] JenisKendaraan jenisKendaraan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jenisKendaraan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jenisKendaraan);
        }

        // GET: JenisKendaraans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jenisKendaraan = await _context.JenisKendaraan.FindAsync(id);
            if (jenisKendaraan == null)
            {
                return NotFound();
            }
            return View(jenisKendaraan);
        }

        // POST: JenisKendaraans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdJenisKendaraan,NamaJenisKendaraan")] JenisKendaraan jenisKendaraan)
        {
            if (id != jenisKendaraan.IdJenisKendaraan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jenisKendaraan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JenisKendaraanExists(jenisKendaraan.IdJenisKendaraan))
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
            return View(jenisKendaraan);
        }

        // GET: JenisKendaraans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jenisKendaraan = await _context.JenisKendaraan
                .FirstOrDefaultAsync(m => m.IdJenisKendaraan == id);
            if (jenisKendaraan == null)
            {
                return NotFound();
            }

            return View(jenisKendaraan);
        }

        // POST: JenisKendaraans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jenisKendaraan = await _context.JenisKendaraan.FindAsync(id);
            _context.JenisKendaraan.Remove(jenisKendaraan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JenisKendaraanExists(int id)
        {
            return _context.JenisKendaraan.Any(e => e.IdJenisKendaraan == id);
        }
    }
}
