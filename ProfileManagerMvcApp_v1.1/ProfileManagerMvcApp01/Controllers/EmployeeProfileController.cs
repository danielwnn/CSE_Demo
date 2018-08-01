using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileManagerMvcApp01.Data;
using ProfileManagerMvcApp01.Helpers;
using ProfileManagerMvcApp01.Models;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SixLabors.Primitives;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace ProfileManagerMvcApp01.Controllers
{
    public class EmployeeProfileController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly EmployeeProfileDbContext _context;

        // inject services through controller's constructor
        public EmployeeProfileController(IHostingEnvironment env, EmployeeProfileDbContext context)
        {
            _env = env;
            _context = context;
        }

        #region List Profile
        // GET: EmployeeProfile
        public async Task<IActionResult> Index()
        {
            List<EmployeeProfile> result = await (
                from x in _context.EmployeeProfiles select new EmployeeProfile {
                    ID         = x.ID,
                    FirstName  = x.FirstName,
                    LastName   = x.LastName,
                    Title      = x.Title,
                    Department = x.Department,
                    PhotoType  = x.PhotoType,
                    ThumbnailBase64 = x.ThumbnailBase64
                }).ToListAsync();

            return View(result);
        }
        #endregion

        #region Create Profile
        // GET: EmployeeProfile/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeProfile/Create
        // To protect from overposting attacks, enable the specific properties to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,FirstName,LastName,Department")] EmployeeProfile employeeProfile, [Bind("PhotoFile")] IFormFile photoFile)
        {
            if (ModelState.IsValid)
            {
                if (PhotoFileExists(photoFile))
                {
                    await CopyPhotoData(employeeProfile, photoFile);
                }

                // save data 
                _context.Add(employeeProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeProfile);
        }
        #endregion

        #region Edit Profile
        // GET: EmployeeProfile/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProfile = await _context.EmployeeProfiles.FindAsync(id);
            if (employeeProfile == null)
            {
                return NotFound();
            }
            return View(employeeProfile);
        }

        // POST: EmployeeProfile/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,FirstName,LastName,Department")] EmployeeProfile employeeProfile, [Bind("PhotoFile")] IFormFile photoFile)
        {
            if (id != employeeProfile.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // fetch old data from db 
                    var existingProfile = _context.EmployeeProfiles.Find(id);

                    if (PhotoFileExists(photoFile))
                    {
                        await CopyPhotoData(employeeProfile, photoFile);
                    }
                    else
                    {
                        // if no new photo file, keep the old data
                        employeeProfile.PhotoType = existingProfile.PhotoType;
                        employeeProfile.PhotoData = existingProfile.PhotoData;
                        employeeProfile.ThumbnailBase64 = existingProfile.ThumbnailBase64;
                    }

                    _context.Entry(existingProfile).CurrentValues.SetValues(employeeProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeProfileExists(employeeProfile.ID))
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
            return View(employeeProfile);
        }
        #endregion

        #region Delete Profile
        // GET: EmployeeProfile/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProfile = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employeeProfile == null)
            {
                return NotFound();
            }

            return View(employeeProfile);
        }

        // POST: EmployeeProfile/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeProfile = await _context.EmployeeProfiles.FindAsync(id);
            _context.EmployeeProfiles.Remove(employeeProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region private section
        private bool EmployeeProfileExists(int id)
        {
            return _context.EmployeeProfiles.Any(e => e.ID == id);
        }

        private bool PhotoFileExists(IFormFile photoFile)
        {
            return photoFile != null && photoFile.Length > 0;
        }

        private async Task CopyPhotoData(EmployeeProfile employeeProfile, IFormFile photoFile)
        {
            using (var photoStream = new MemoryStream())
            {
                await photoFile.CopyToAsync(photoStream);
                employeeProfile.PhotoData = photoStream.ToArray();
                employeeProfile.ThumbnailBase64 = ImageHelper.Resize(employeeProfile.PhotoData, 120);
                employeeProfile.PhotoType = photoFile.ContentType;
            }
        }

        // this method is not used
        private async void CreateThrumbnailFile(EmployeeProfile employeeProfile, IFormFile photoFile)
        {
            // check if photo is present or not
            if (photoFile != null && photoFile.Length > 0)
            {
                // var fileExt = Path.GetExtension(photoFile.FileName);
                var fileName = $"{employeeProfile.FirstName}_{employeeProfile.LastName}_thrumbnail.jpg";
                var filePath = Path.Combine(_env.WebRootPath, "images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(fileStream);
                }
            }
        }
        #endregion
    }
}
