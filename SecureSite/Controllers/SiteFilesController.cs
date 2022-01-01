using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using SecureSite.Data;
using SecureSite.Models;
using SecureSite.Utilities;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace SecureSite.Controllers
{
    public class SiteFilesController : Controller
    {
        private readonly SecureSiteContext _context;
        private long _maxFileSize = 3145728;
        private string[] _allowedExtensions = { ".png", ".jpg", ".jpeg" };

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

        [HttpPost]
        [Route(nameof(UploadFile))]
        public async Task<IActionResult> UploadFile()
        {
            var theWebRequest = HttpContext.Request;

            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!theWebRequest.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(theWebRequest.ContentType, out var theMediaTypeHeader) ||
                string.IsNullOrEmpty(theMediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(theMediaTypeHeader.Boundary.Value, theWebRequest.Body);
            var section = await reader.ReadNextSectionAsync();

            // This sample try to get the first file from request and save it
            // Make changes according to your needs in actual use
            while (section != null)
            {
                var DoesItHaveContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var theContentDisposition);

                if (DoesItHaveContentDispositionHeader && theContentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(theContentDisposition.FileName.Value))
                {
                    // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                    // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                    // In short, it is necessary to restrict and verify the upload
                    // Here, we just use the temporary folder and a random file name

                    SiteFile siteFile = new();
                    siteFile.UntrustedName = HttpUtility.HtmlEncode(theContentDisposition.FileName.Value);
                    siteFile.Time = DateTime.Now;

                    siteFile.Content =
                            await FileHelpers.ProcessStreamedFile(section, theContentDisposition,
                                ModelState, _allowedExtensions, _maxFileSize);
                    if (siteFile.Content.Length == 0)
                    {
                        return RedirectToAction("Index", "SiteFiles");
                    }
                    siteFile.Size = siteFile.Content.Length;

                    await _context.SiteFile.AddAsync(siteFile);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "SiteFiles");
                }

                section = await reader.ReadNextSectionAsync();
            }

            // If the code runs to this location, it means that no files have been saved
            return BadRequest("No files data in the request.");
        }

        // POST: SiteFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,UntrustedName,Time,Size,Content")] SiteFile siteFile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        siteFile.Id = Guid.NewGuid();
        //        _context.Add(siteFile);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(siteFile);
        //}

        // GET: SiteFiles/Download/5
        public async Task<IActionResult> Download(Guid? id)
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
            return File(siteFile.Content, MediaTypeNames.Application.Octet, siteFile.UntrustedName);
        }

        // POST: SiteFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,UntrustedName,Time,Size,Content")] SiteFile siteFile)
        //{
        //    if (id != siteFile.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(siteFile);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!SiteFileExists(siteFile.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(siteFile);
        //}

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