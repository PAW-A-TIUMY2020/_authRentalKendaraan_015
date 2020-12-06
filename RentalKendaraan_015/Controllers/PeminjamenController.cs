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
    public class PeminjamenController : Controller
    {
        private readonly Rental_KendaraanContext _context;

        public PeminjamenController(Rental_KendaraanContext context)
        {
            _context = context;
        }

        // GET: Peminjamen
        public async Task<IActionResult> Index(string pnjm, string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            //list menyimpan ketersediaan
            var pnjmList = new List<string>();

            //query mengambil data
            var pnjmQuery = from d in _context.Peminjaman orderby d.IdPeminjaman select d.IdPeminjaman.ToString();

            pnjmList.AddRange(pnjmQuery.Distinct());

            //Menampilkan di view
            ViewBag.pnjm = new SelectList(pnjmList);

            //memanggil db context
            var menu = from m in _context.Peminjaman.Include(k => k.IdCustomerNavigation).Include(k=> k.IdJaminanNavigation).Include(k => k.IdKendaraanNavigation) select m;

            //memilih dropdownlist IdPeminjaman.ToString()
            if (!string.IsNullOrEmpty(pnjm))
            {
                menu = menu.Where(x => x.IdPeminjaman.ToString() == pnjm);
            }

            //Search data
            if (!string.IsNullOrEmpty(searchString))
            {
                menu = menu.Where(s => s.Biaya.ToString().Contains(searchString) || s.IdCustomerNavigation.NamaCustomer.Contains(searchString)
                || s.IdJaminanNavigation.NamaJaminan.Contains(searchString) || s.IdKendaraanNavigation.NamaKendaraan.Contains(searchString));
            }

            //Membuat pagedlist
            ViewData["currentSort"] = sortOrder;

            if(searchString != null)
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
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder)?"name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "Date"? "date_desc" : "Date";
            ViewData["BiayaSortParam"] = sortOrder == "Biaya" ? "biaya_desc" : "Biaya";
            ViewData["JaminanSortParam"] = sortOrder== "Jaminan"? "jaminan_desc" : "Jaminan";
            ViewData["KendaraanSortParam"] = sortOrder == "Kendaraan" ? "kendaraan_desc" : "Kendaraan";

            switch (sortOrder)
            {
                case "name_desc":
                    menu = menu.OrderByDescending(s => s.IdCustomerNavigation.NamaCustomer);
                    break;

                case "Date":
                    menu = menu.OrderBy(s => s.TglPeminjaman);
                    break;

                case "date_desc":
                    menu = menu.OrderByDescending(s => s.TglPeminjaman);
                    break;

                case "Biaya":
                    menu = menu.OrderBy(s => s.Biaya);
                    break;

                case "biaya_desc":
                    menu = menu.OrderByDescending(s => s.Biaya);
                    break;

                case "Jaminan":
                    menu = menu.OrderBy(s => s.IdJaminanNavigation.NamaJaminan);
                    break;

                case "jaminan_desc":
                    menu = menu.OrderByDescending(s => s.IdJaminanNavigation.NamaJaminan);
                    break;

                case "Kendaraan":
                    menu = menu.OrderBy(s => s.IdKendaraanNavigation.NamaKendaraan);
                    break;

                case "kendaraan_desc":
                    menu = menu.OrderByDescending(s => s.IdKendaraanNavigation.NamaKendaraan);
                    break;

                default:
                    menu = menu.OrderBy(s => s.IdCustomerNavigation.NamaCustomer);
                    break;
            }


            return View(await PaginatedList<Peminjaman>.CreateAsync(menu.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await menu.ToListAsync());
        }

        // GET: Peminjamen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peminjaman = await _context.Peminjaman
                .Include(p => p.IdCustomerNavigation)
                .Include(p => p.IdJaminanNavigation)
                .Include(p => p.IdKendaraanNavigation)
                .FirstOrDefaultAsync(m => m.IdPeminjaman == id);
            if (peminjaman == null)
            {
                return NotFound();
            }

            return View(peminjaman);
        }

        // GET: Peminjamen/Create
        public IActionResult Create()
        {
            ViewData["IdCustomer"] = new SelectList(_context.Customer, "IdCustomer", "NamaCustomer");
            ViewData["IdJaminan"] = new SelectList(_context.Jaminan, "IdJaminan", "NamaJaminan");
            ViewData["IdKendaraan"] = new SelectList(_context.Kendaraan, "IdKendaraan", "NamaKendaraan");

            //ViewData["NamaKendaraan"] = new SelectList(_context.Kendaraan, "NamaKendaraan", "NamaKendaraan");
            //ViewData["NamaJaminan"] = new SelectList(_context.Jaminan, "NamaJaminan", "NamaJaminan");
            //ViewData["NamaCustomer"] = new SelectList(_context.Customer, "NamaCustomer", "NamaCustomer");
            return View();
        }

        // POST: Peminjamen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPeminjaman,TglPeminjaman,IdKendaraan,IdCustomer,IdJaminan,Biaya")] Peminjaman peminjaman)
        {
            if (ModelState.IsValid)
            {
                _context.Add(peminjaman);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCustomer"] = new SelectList(_context.Customer, "IdCustomer", "IdCustomer", peminjaman.IdCustomer);
            ViewData["IdJaminan"] = new SelectList(_context.Jaminan, "IdJaminan", "IdJaminan", peminjaman.IdJaminan);
            ViewData["IdKendaraan"] = new SelectList(_context.Kendaraan, "IdKendaraan", "IdKendaraan", peminjaman.IdKendaraan);

            return View(peminjaman);
        }

        // GET: Peminjamen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peminjaman = await _context.Peminjaman.FindAsync(id);
            if (peminjaman == null)
            {
                return NotFound();
            }
            ViewData["IdCustomer"] = new SelectList(_context.Customer, "IdCustomer", "NamaCustomer", peminjaman.IdCustomer);
            ViewData["IdJaminan"] = new SelectList(_context.Jaminan, "IdJaminan", "NamaJaminan", peminjaman.IdJaminan);
            ViewData["IdKendaraan"] = new SelectList(_context.Kendaraan, "IdKendaraan", "NamaKendaraan", peminjaman.IdKendaraan);
            return View(peminjaman);
        }

        // POST: Peminjamen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPeminjaman,TglPeminjaman,IdKendaraan,IdCustomer,IdJaminan,Biaya")] Peminjaman peminjaman)
        {
            if (id != peminjaman.IdPeminjaman)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(peminjaman);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeminjamanExists(peminjaman.IdPeminjaman))
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
            ViewData["IdCustomer"] = new SelectList(_context.Customer, "IdCustomer", "IdCustomer", peminjaman.IdCustomer);
            ViewData["IdJaminan"] = new SelectList(_context.Jaminan, "IdJaminan", "IdJaminan", peminjaman.IdJaminan);
            ViewData["IdKendaraan"] = new SelectList(_context.Kendaraan, "IdKendaraan", "IdKendaraan", peminjaman.IdKendaraan);
            return View(peminjaman);
        }

        // GET: Peminjamen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peminjaman = await _context.Peminjaman
                .Include(p => p.IdCustomerNavigation)
                .Include(p => p.IdJaminanNavigation)
                .Include(p => p.IdKendaraanNavigation)
                .FirstOrDefaultAsync(m => m.IdPeminjaman == id);
            if (peminjaman == null)
            {
                return NotFound();
            }

            return View(peminjaman);
        }

        // POST: Peminjamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var peminjaman = await _context.Peminjaman.FindAsync(id);
            _context.Peminjaman.Remove(peminjaman);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeminjamanExists(int id)
        {
            return _context.Peminjaman.Any(e => e.IdPeminjaman == id);
        }
    }
}
