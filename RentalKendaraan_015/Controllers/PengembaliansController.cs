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
    public class PengembaliansController : Controller
    {
        private readonly Rental_KendaraanContext _context;

        public PengembaliansController(Rental_KendaraanContext context)
        {
            _context = context;
        }

        // GET: Pengembalians
        public async Task<IActionResult> Index(string kmbl, string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            //list menyimpan ketersediaan
            var kmblList = new List<string>();

            //query mengambil data
            var kmblQuery = from d in _context.Pengembalian orderby d.Denda select d.Denda.ToString();

            kmblList.AddRange(kmblQuery.Distinct());

            //Menampilkan di view
            ViewBag.kmbl = new SelectList(kmblList);

            //memanggil db context
            var menu = from m in _context.Pengembalian.Include(k => k.IdKondisiNavigation).Include(k => k.IdPeminjamanNavigation) select m;

            //memilih dropdownlist Denda
            if (!string.IsNullOrEmpty(kmbl))
            {
                menu = menu.Where(x => x.Denda.ToString() == kmbl);
            }

            //Search data
            if (!string.IsNullOrEmpty(searchString))
            {
                menu = menu.Where(s => s.TglPengembalian.ToString().Contains(searchString) || s.Denda.ToString().Contains(searchString)
                || s.IdKondisiNavigation.NamaKondisi.Contains(searchString) || s.IdPeminjamanNavigation.IdPeminjaman.ToString().Contains(searchString));
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
            ViewData["DateSortParam"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["DendaSortParam"] = sortOrder == "Denda" ? "denda_desc" : "Denda";
            ViewData["KondisiSortParam"] = sortOrder == "Kondisi" ? "kondisi_desc" : "Kondisi";
            ViewData["IdSortParam"] = sortOrder == "Id" ? "id_desc" : "Id";

            switch (sortOrder)
            {
                case "date_desc":
                    menu = menu.OrderByDescending(s => s.TglPengembalian);
                    break;

                case "Denda":
                    menu = menu.OrderBy(s => s.Denda);
                    break;

                case "denda_desc":
                    menu = menu.OrderByDescending(s => s.Denda);
                    break;

                case "Id":
                    menu = menu.OrderBy(s => s.IdPeminjaman);
                    break;

                case "id_desc":
                    menu = menu.OrderByDescending(s => s.IdPeminjaman);
                    break;

                case "Kondisi":
                    menu = menu.OrderBy(s => s.IdKondisiNavigation.NamaKondisi);
                    break;

                case "kondisi_desc":
                    menu = menu.OrderByDescending(s => s.IdKondisiNavigation.NamaKondisi);
                    break;

                default:
                    menu = menu.OrderBy(s => s.TglPengembalian);
                    break;
            }


            return View(await PaginatedList<Pengembalian>.CreateAsync(menu.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Pengembalians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pengembalian = await _context.Pengembalian
                .Include(p => p.IdKondisiNavigation)
                .Include(p => p.IdPeminjamanNavigation)
                .FirstOrDefaultAsync(m => m.IdPengembalian == id);
            if (pengembalian == null)
            {
                return NotFound();
            }

            return View(pengembalian);
        }

        // GET: Pengembalians/Create
        public IActionResult Create()
        {
            ViewData["IdKondisi"] = new SelectList(_context.KondisiKendaraan, "IdKondisi", "NamaKondisi");
            ViewData["IdPeminjaman"] = new SelectList(_context.Peminjaman, "IdPeminjaman", "IdPeminjaman");
            return View();
        }

        // POST: Pengembalians/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPengembalian,TglPengembalian,IdPeminjaman,IdKondisi,Denda")] Pengembalian pengembalian)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pengembalian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKondisi"] = new SelectList(_context.KondisiKendaraan, "IdKondisi", "IdKondisi", pengembalian.IdKondisi);
            ViewData["IdPeminjaman"] = new SelectList(_context.Peminjaman, "IdPeminjaman", "IdPeminjaman", pengembalian.IdPeminjaman);
            return View(pengembalian);
        }

        // GET: Pengembalians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pengembalian = await _context.Pengembalian.FindAsync(id);
            if (pengembalian == null)
            {
                return NotFound();
            }
            ViewData["IdKondisi"] = new SelectList(_context.KondisiKendaraan, "IdKondisi", "IdKondisi", pengembalian.IdKondisi);
            ViewData["IdPeminjaman"] = new SelectList(_context.Peminjaman, "IdPeminjaman", "IdPeminjaman", pengembalian.IdPeminjaman);
            return View(pengembalian);
        }

        // POST: Pengembalians/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPengembalian,TglPengembalian,IdPeminjaman,IdKondisi,Denda")] Pengembalian pengembalian)
        {
            if (id != pengembalian.IdPengembalian)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pengembalian);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PengembalianExists(pengembalian.IdPengembalian))
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
            ViewData["IdKondisi"] = new SelectList(_context.KondisiKendaraan, "IdKondisi", "IdKondisi", pengembalian.IdKondisi);
            ViewData["IdPeminjaman"] = new SelectList(_context.Peminjaman, "IdPeminjaman", "IdPeminjaman", pengembalian.IdPeminjaman);
            return View(pengembalian);
        }

        // GET: Pengembalians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pengembalian = await _context.Pengembalian
                .Include(p => p.IdKondisiNavigation)
                .Include(p => p.IdPeminjamanNavigation)
                .FirstOrDefaultAsync(m => m.IdPengembalian == id);
            if (pengembalian == null)
            {
                return NotFound();
            }

            return View(pengembalian);
        }

        // POST: Pengembalians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pengembalian = await _context.Pengembalian.FindAsync(id);
            _context.Pengembalian.Remove(pengembalian);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PengembalianExists(int id)
        {
            return _context.Pengembalian.Any(e => e.IdPengembalian == id);
        }
    }
}
