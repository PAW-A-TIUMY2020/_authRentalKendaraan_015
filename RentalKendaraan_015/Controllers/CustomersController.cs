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
    public class CustomersController : Controller
    {
        private readonly Rental_KendaraanContext _context;

        public CustomersController(Rental_KendaraanContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string cust, string searchString, string sortOrder, string currentFilter, int? pageNumber )
        {
            //list menyimpan ketersediaan
            var custList = new List<string>();

            //query mengambil data
            var custQuery = from d in _context.Customer orderby d.Alamat select d.Alamat;

            custList.AddRange(custQuery.Distinct());

            //Menampilkan di view
            ViewBag.cust = new SelectList(custList);

            //memanggil db context
            var menu = from m in _context.Customer.Include(k => k.IdGenderNavigation) select m;

            //memilih dropdownlist Alamat
            if (!string.IsNullOrEmpty(cust))
            {
                menu = menu.Where(x => x.Alamat == cust);
            }

            //Search data
            if (!string.IsNullOrEmpty(searchString))
            {
                menu = menu.Where(s => s.NamaCustomer.Contains(searchString) || s.Nik.Contains(searchString)
                || s.NoHp.Contains(searchString));
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
            ViewData["NIKSortParam"] = sortOrder == "Nik" ? "nik_desc" : "Nik";
            ViewData["AlamatSortParam"] = sortOrder == "Alamat" ? "alamat_desc" : "Alamat";
            ViewData["HpSortParam"] = sortOrder == "Hp" ? "hp_desc" : "Hp";
            ViewData["GenderSortParam"] = sortOrder== "Gender" ? "gender_desc" : "Gender";


            switch (sortOrder)
            {
                case "name_desc":
                    menu = menu.OrderByDescending(s => s.NamaCustomer);
                    break;

                case "Nik":
                    menu = menu.OrderBy(s => s.Nik);
                    break;
                case "nik_desc":
                    menu = menu.OrderBy(s => s.Nik);
                    break;

                case "Alamat":
                    menu = menu.OrderBy(s => s.Alamat);
                    break;
                case "alamat_desc":
                    menu = menu.OrderByDescending(s => s.Alamat);
                    break;

                case "Hp":
                    menu = menu.OrderBy(s => s.NoHp);
                    break;
                case "hp_desc":
                    menu = menu.OrderByDescending(s => s.NoHp);
                    break;


                case "Gender":
                    menu = menu.OrderBy(s => s.IdGenderNavigation.NamaGender);
                    break;
                case "gender_desc":
                    menu = menu.OrderByDescending(s => s.IdGenderNavigation.NamaGender);
                    break;

                default:
                    menu = menu.OrderBy(s => s.NamaCustomer);
                    break;

            }


            return View(await PaginatedList<Customer>.CreateAsync(menu.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .Include(c => c.IdGenderNavigation)
                .FirstOrDefaultAsync(m => m.IdCustomer == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["IdGender"] = new SelectList(_context.Gender, "IdGender", "NamaGender");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCustomer,NamaCustomer,Nik,Alamat,NoHp,IdGender")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdGender"] = new SelectList(_context.Gender, "IdGender", "IdGender", customer.IdGender);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["IdGender"] = new SelectList(_context.Gender, "IdGender", "IdGender", customer.IdGender);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCustomer,NamaCustomer,Nik,Alamat,NoHp,IdGender")] Customer customer)
        {
            if (id != customer.IdCustomer)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.IdCustomer))
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
            ViewData["IdGender"] = new SelectList(_context.Gender, "IdGender", "IdGender", customer.IdGender);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .Include(c => c.IdGenderNavigation)
                .FirstOrDefaultAsync(m => m.IdCustomer == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.IdCustomer == id);
        }
    }
}
