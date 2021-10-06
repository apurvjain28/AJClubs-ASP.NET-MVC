using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJClubs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AJClubs.Controllers
{
    public class AJProvincesController : Controller
    {
        private readonly AJClubsContext _context;

        public AJProvincesController(AJClubsContext context)
        {
            _context = context;
        }

        /// <summary>
        ///    if the country code is in the URL or QueryString, it is saved to a cookie or session variable
        ///    else a cookie or session variable is checked for it & that value is used, if found
        ///    else all provinces are displayed with messages that country is not selected
        /// </summary>
        /// <param name="id">countryCode</param>
        /// <returns>provinces of a country either in url or session. Also, if country not in url or session then list of countries with error message shown</returns>

        // GET: AJProvinces
        public IActionResult Index(string id)
        {
            if ( id != null)
            {
                // Storing value in session
                var countryName = _context.Country.FirstOrDefault( c => c.CountryCode == id).Name;
                HttpContext.Session.SetString("countryCode",id);
                HttpContext.Session.SetString("countryName", countryName);
                return View(_context.Province.Include(p => p.CountryCodeNavigation).Where(p => p.CountryCode.Equals(id)).OrderBy(p => p.Name).ToList());
            }

            string countryCode = Request.Query["CountryCode"].FirstOrDefault();
            string sessionCode = HttpContext.Session.GetString("countryCode");
            if (countryCode == null)
            {
                if (sessionCode == null)
                {
                    TempData["Message"] = "Please select a country to view provinces";
                    return RedirectToAction("Index", "AJCountries");
                }
                else
                {
                    return View(_context.Province.Include(p => p.CountryCodeNavigation).Where(p => p.CountryCode.Equals(sessionCode)).OrderBy(p => p.Name).ToList());
                }
            }
            else
            {
                return View(_context.Province.Include(p => p.CountryCodeNavigation).Where(p => p.CountryCode.Equals(countryCode)).OrderBy(p => p.Name).ToList());
            }

        }

        /// <summary>
        /// Details of the provinces
        /// </summary>
        /// <param name="id">provinceCode</param>
        /// <returns> all information of a province</returns>
        // GET: AJProvinces/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        /// <summary>
        /// Creates a province in the session country
        /// </summary>
        /// <returns>list of all the province</returns>
        // GET: AJProvinces/Create
        public IActionResult Create()
        {
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode");
            return View();
        }

        // POST: AJProvinces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvinceCode,Name,CountryCode,SalesTaxCode,SalesTax,IncludesFederalTax,FirstPostalLetter")] Province province)
        {
            var currentProvinceName = _context.Province.Where(a => a.Name.Equals(province.Name)).ToList();
            var currentProvinceCode = _context.Province.Where(a => a.ProvinceCode.Equals(province.ProvinceCode)).ToList();


            if (currentProvinceName.Any())
            {
                //Display error message if province name already exist
                ModelState.AddModelError("Name", $"The province {province.Name} already exists! Please try again" );
            }

            if (currentProvinceCode.Any())
            {
                //Display error message if province with provinceCode already exist
                ModelState.AddModelError("ProvinceCode", $"The province code {province.ProvinceCode} already exists! Please try again");
            }

            if (ModelState.IsValid)
            {
                _context.Add(province);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        /// <summary>
        /// enables to edit provinces already in the list
        /// </summary>
        /// <param name="id">provinceCode</param>
        /// <returns>add edited province in DB</returns>
        // GET: AJProvinces/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province.FindAsync(id);
            if (province == null)
            {
                return NotFound();
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        // POST: AJProvinces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProvinceCode,Name,CountryCode,SalesTaxCode,SalesTax,IncludesFederalTax,FirstPostalLetter")] Province province)
        {
            if (id != province.ProvinceCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(province.ProvinceCode))
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
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        // GET: AJProvinces/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        /// <summary>
        /// delete a province in the specified country
        /// </summary>
        /// <param name="id">provinceCode</param>
        /// <returns></returns>

        // POST: AJProvinces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var province = await _context.Province.FindAsync(id);
            _context.Province.Remove(province);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProvinceExists(string id)
        {
            return _context.Province.Any(e => e.ProvinceCode == id);
        }
    }
}
