using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecureSite.Data;
using SecureSite.Models;

namespace SecureSite.Controllers
{
    public class SiteFilesController : Controller
    {
        private readonly SecureSiteContext _context;

        public SiteFilesController(SecureSiteContext context)
        {
            _context = context;
        }

        // GET: SiteFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.SiteFile.ToListAsync());
        }

        // GET: SiteFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteFile = await _context.SiteFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteFile == null)
            {
                return NotFound();
            }

            return View(siteFile);
        }

        // GET: SiteFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SiteFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UntrustedName,Time,Size,Content")] SiteFile siteFile)
        {
            if (ModelState.IsValid)
            {
                siteFile.Id = Guid.NewGuid();
                _context.Add(siteFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteFile);
        }

        // GET: SiteFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteFile = await _context.SiteFile.FindAsync(id);
            if (siteFile == null)
            {
                return NotFound();
            }
            return View(siteFile);
        }

        // POST: SiteFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UntrustedName,Time,Size,Content")] SiteFile siteFile)
        {
            if (id != siteFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteFileExists(siteFile.Id))
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
            return View(siteFile);
        }

        // GET: SiteFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteFile = await _context.SiteFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteFile == null)
            {
                return NotFound();
            }

            return View(siteFile);
        }

        // POST: SiteFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var siteFile = await _context.SiteFile.FindAsync(id);
            _context.SiteFile.Remove(siteFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteFileExists(Guid id)
        {
            return _context.SiteFile.Any(e => e.Id == id);
        }
    }
}
