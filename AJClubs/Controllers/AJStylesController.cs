using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJClubs.Models;

namespace AJClubs.Controllers
{
    public class AJStylesController : Controller
    {
        private readonly AJClubsContext _context;

        public AJStylesController(AJClubsContext context)
        {
            _context = context;
        }

        // GET: AJStyles
        // return list of all the styles in DB

        public async Task<IActionResult> Index()
        {
            return View(await _context.Style.ToListAsync());
        }

        // GET: AJStyles/Details/5
        // return details of selected style

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var style = await _context.Style
                .FirstOrDefaultAsync(m => m.StyleName == id);
            if (style == null)
            {
                return NotFound();
            }

            return View(style);
        }

        // GET: AJStyles/Create
        // create new style

        public IActionResult Create()
        {
            return View();
        }

        // POST: AJStyles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StyleName,Description")] Style style)
        {
            if (ModelState.IsValid)
            {
                _context.Add(style);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(style);
        }

        // GET: AJStyles/Edit/5
        // edit the details if an existing style
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var style = await _context.Style.FindAsync(id);
            if (style == null)
            {
                return NotFound();
            }
            return View(style);
        }

        // POST: AJStyles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StyleName,Description")] Style style)
        {
            if (id != style.StyleName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(style);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StyleExists(style.StyleName))
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
            return View(style);
        }

        // GET: AJStyles/Delete/5
        // delete an existing style
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var style = await _context.Style
                .FirstOrDefaultAsync(m => m.StyleName == id);
            if (style == null)
            {
                return NotFound();
            }

            return View(style);
        }

        // POST: AJStyles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var style = await _context.Style.FindAsync(id);
            _context.Style.Remove(style);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StyleExists(string id)
        {
            return _context.Style.Any(e => e.StyleName == id);
        }
    }
}
