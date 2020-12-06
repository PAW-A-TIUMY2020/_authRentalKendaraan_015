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
    public class KendaraansController : Controller
    {
        private readonly Rental_KendaraanContext _context;

        public KendaraansController(Rental_KendaraanContext context)
        {
            _context = context;
        }

        // GET: Kendaraans
        public async Task<IActionResult> Index(string ktsd, string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            //list menyimpan ketersediaan
            var ktsdList = new List<string>();

            //query mengambil data
            var ktsQuery = from d in _context.Kendaraan orderby d.Ketersediaan select d.Ketersediaan;

            ktsdList.AddRange(ktsQuery.Distinct());

            //Menampilkan di view
            ViewBag.ktsd = new SelectList(ktsdList);

            //memanggil db context
            var menu = from m in _context.Kendaraan.Include(k => k.IdJenisKendaraanNavigation) select m;

            //memilih dropdownlist Ketersediaan
            if (!string.IsNullOrEmpty(ktsd))
            {
                menu = menu.Where(x => x.Ketersediaan == ktsd);
            }

            //Search data
            if (!string.IsNullOrEmpty(searchString))
            {
                menu = menu.Where(s => s.NoPolisi.Contains(searchString) || s.NamaKendaraan.Contains(searchString)
                || s.NoStnk.Contains(searchString));
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
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["NopolSortParam"] = sortOrder == "Nopol" ? "nopol_desc" : "Nopol";
            ViewData["StnkSortParam"] = sortOrder == "Stnk" ? "stnk_desc" : "Stnk";
            ViewData["KtsdSortParam"] = sortOrder == "Ktsd" ? "ktsd_desc" : "Ktsd";
            ViewData["JnsSortParam"] = sortOrder == "Jns" ? "jns_desc" : "Jns";

            switch (sortOrder)
            {
                case "name_desc":
                    menu = menu.OrderByDescending(s => s.NamaKendaraan);
                    break;

                case "Nopol":
                    menu = menu.OrderBy(s => s.NoPolisi);
                    break;

                case "nopol_desc":
                    menu = menu.OrderByDescending(s => s.NoPolisi);
                    break;

                case "Stnk":
                    menu = menu.OrderBy(s => s.NoStnk);
                    break;

                case "stnk_desc":
                    menu = menu.OrderByDescending(s => s.NoStnk);
                    break;

                case "Ktsd":
                    menu = menu.OrderBy(s => s.Ketersediaan);
                    break;

                case "ktsd_desc":
                    menu = menu.OrderByDescending(s => s.Ketersediaan);
                    break;

                case "Jns":
                    menu = menu.OrderBy(s => s.IdJenisKendaraanNavigation.NamaJenisKendaraan);
                    break;

                case "jns_desc":
                    menu = menu.OrderByDescending(s => s.IdJenisKendaraanNavigation.NamaJenisKendaraan);
                    break;

                default:
                    menu = menu.OrderBy(s => s.NamaKendaraan);
                    break;
            }


            return View(await PaginatedList<Kendaraan>.CreateAsync(menu.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        // GET: Kendaraans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kendaraan = await _context.Kendaraan
                .Include(k => k.IdJenisKendaraanNavigation)
                .FirstOrDefaultAsync(m => m.IdKendaraan == id);
            if (kendaraan == null)
            {
                return NotFound();
            }

            return View(kendaraan);
        }

        // GET: Kendaraans/Create
        public IActionResult Create()
        {
            ViewData["IdJenisKendaraan"] = new SelectList(_context.JenisKendaraan, "IdJenisKendaraan", "NamaJenisKendaraan");
            return View();
        }

        // POST: Kendaraans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKendaraan,NamaKendaraan,NoPolisi,NoStnk,IdJenisKendaraan,Ketersediaan")] Kendaraan kendaraan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kendaraan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdJenisKendaraan"] = new SelectList(_context.JenisKendaraan, "IdJenisKendaraan", "IdJenisKendaraan", kendaraan.IdJenisKendaraan);
            return View(kendaraan);
        }

        // GET: Kendaraans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kendaraan = await _context.Kendaraan.FindAsync(id);
            if (kendaraan == null)
            {
                return NotFound();
            }
            ViewData["IdJenisKendaraan"] = new SelectList(_context.JenisKendaraan, "IdJenisKendaraan", "IdJenisKendaraan", kendaraan.IdJenisKendaraan);
            return View(kendaraan);
        }

        // POST: Kendaraans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKendaraan,NamaKendaraan,NoPolisi,NoStnk,IdJenisKendaraan,Ketersediaan")] Kendaraan kendaraan)
        {
            if (id != kendaraan.IdKendaraan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kendaraan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KendaraanExists(kendaraan.IdKendaraan))
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
            ViewData["IdJenisKendaraan"] = new SelectList(_context.JenisKendaraan, "IdJenisKendaraan", "IdJenisKendaraan", kendaraan.IdJenisKendaraan);
            return View(kendaraan);
        }

        // GET: Kendaraans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kendaraan = await _context.Kendaraan
                .Include(k => k.IdJenisKendaraanNavigation)
                .FirstOrDefaultAsync(m => m.IdKendaraan == id);
            if (kendaraan == null)
            {
                return NotFound();
            }

            return View(kendaraan);
        }

        // POST: Kendaraans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kendaraan = await _context.Kendaraan.FindAsync(id);
            _context.Kendaraan.Remove(kendaraan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KendaraanExists(int id)
        {
            return _context.Kendaraan.Any(e => e.IdKendaraan == id);
        }
    }
}
