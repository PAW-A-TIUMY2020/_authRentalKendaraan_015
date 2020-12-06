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
    public class KondisiKendaraansController : Controller
    {
        private readonly Rental_KendaraanContext _context;

        public KondisiKendaraansController(Rental_KendaraanContext context)
        {
            _context = context;
        }

        // GET: KondisiKendaraans
        public async Task<IActionResult> Index(string knds, string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            //list menyimpan ketersediaan
            var kndsList = new List<string>();

            //query mengambil data
            var kndsQuery = from d in _context.KondisiKendaraan orderby d.NamaKondisi select d.NamaKondisi;

            kndsList.AddRange(kndsQuery.Distinct());

            //Menampilkan di view
            ViewBag.knds = new SelectList(kndsList);

            //memanggil db context
            var menu = from m in _context.KondisiKendaraan select m;

            //memilih dropdownlist Nama KondisiKendaraan
            if (!string.IsNullOrEmpty(knds))
            {
                menu = menu.Where(x => x.NamaKondisi == knds);
            }

            //Search data
            if (!string.IsNullOrEmpty(searchString))
            {
                menu = menu.Where(s => s.NamaKondisi.Contains(searchString));
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
            ViewData["KondisiSortParam"] = string.IsNullOrEmpty(sortOrder) ? "kondisi_desc" : "";


            switch (sortOrder)
            {
                case "kondisi_desc":
                    menu = menu.OrderByDescending(s => s.NamaKondisi);
                    break;

                default:
                    menu = menu.OrderBy(s => s.NamaKondisi);
                    break;
            }


            return View(await PaginatedList<KondisiKendaraan>.CreateAsync(menu.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: KondisiKendaraans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kondisiKendaraan = await _context.KondisiKendaraan
                .FirstOrDefaultAsync(m => m.IdKondisi == id);
            if (kondisiKendaraan == null)
            {
                return NotFound();
            }

            return View(kondisiKendaraan);
        }

        // GET: KondisiKendaraans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KondisiKendaraans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKondisi,NamaKondisi")] KondisiKendaraan kondisiKendaraan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kondisiKendaraan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kondisiKendaraan);
        }

        // GET: KondisiKendaraans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kondisiKendaraan = await _context.KondisiKendaraan.FindAsync(id);
            if (kondisiKendaraan == null)
            {
                return NotFound();
            }
            return View(kondisiKendaraan);
        }

        // POST: KondisiKendaraans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKondisi,NamaKondisi")] KondisiKendaraan kondisiKendaraan)
        {
            if (id != kondisiKendaraan.IdKondisi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kondisiKendaraan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KondisiKendaraanExists(kondisiKendaraan.IdKondisi))
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
            return View(kondisiKendaraan);
        }

        // GET: KondisiKendaraans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kondisiKendaraan = await _context.KondisiKendaraan
                .FirstOrDefaultAsync(m => m.IdKondisi == id);
            if (kondisiKendaraan == null)
            {
                return NotFound();
            }

            return View(kondisiKendaraan);
        }

        // POST: KondisiKendaraans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kondisiKendaraan = await _context.KondisiKendaraan.FindAsync(id);
            _context.KondisiKendaraan.Remove(kondisiKendaraan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KondisiKendaraanExists(int id)
        {
            return _context.KondisiKendaraan.Any(e => e.IdKondisi == id);
        }
    }
}
