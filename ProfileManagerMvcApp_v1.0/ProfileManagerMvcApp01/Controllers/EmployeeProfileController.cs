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
    public class DetectFaceResult
    {
        public bool HasDetected { get; set; }  = false;
        public string Data { get; set; }
    }

    public class EmployeeProfileController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly EmployeeProfileDbContext _context;
        private readonly FaceApiHelper _faceApiHelper;

        // inject services through controller's constructor
        public EmployeeProfileController(IHostingEnvironment env, EmployeeProfileDbContext context, FaceApiHelper faceApiHelper)
        {
            _env = env;
            _context = context;
            _faceApiHelper = faceApiHelper;
        }

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

        // GET: EmployeeProfile/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProfile = await _context.EmployeeProfiles.FirstOrDefaultAsync(m => m.ID == id);
            if (employeeProfile == null)
            {
                return NotFound();
            }

            return View(employeeProfile);
        }

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

        // POST: EmployeeProfile/DetectFaces
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetectFaces([Bind("PhotoFile")] IFormFile photoFile)
        {
            using (var photoStream = new MemoryStream())
            {
                await photoFile.CopyToAsync(photoStream);
                byte[] photoData = photoStream.ToArray();
                var faces = await _faceApiHelper.Detect(photoData);

                var result = new DetectFaceResult();
                if (faces.Count == 0)
                {
                    result.Data = "No face is detected, please choose a face photo.";
                }
                else if (faces.Count > 1)
                {
                    result.Data = $"{faces.Count} faces are detected, please choose a photo with one face.";
                }
                else
                {
                    // mark the face in the image
                    result.HasDetected = true;

                    var FaceInfo = faces.First<FaceInfo>();
                    int left   = FaceInfo.faceRectangle.left;
                    int top    = FaceInfo.faceRectangle.top;
                    int width  = FaceInfo.faceRectangle.width;
                    int height = FaceInfo.faceRectangle.height;

                    string base64 = ImageHelper.DrawRectangle(photoData, new PointF[] {
                        new PointF(left, top),
                        new PointF(left, top + height),
                        new PointF(left + width, top + height),
                        new PointF(left + width, top)
                    });

                    result.Data = $"data:image/jpeg;base64,{base64}";
                }

                return Json(result);
            }
        }

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
    }
}
