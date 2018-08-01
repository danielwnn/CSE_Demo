using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfileManagerMvcApp01.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProfileManagerMvcApp01.Controllers
{
    /// <summary>
    /// Face Controller - provide Face REST API calls through FaceApiHelper class
    /// </summary>
    public class FaceController : Controller
    {
        private readonly FaceApiHelper _faceApiHelper;

        // inject services through controller's constructor
        public FaceController(FaceApiHelper faceApiHelper)
        {
            _faceApiHelper = faceApiHelper;
        }

        // POST: Face/Detect
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detect([Bind("PhotoFile")] IFormFile photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                using (var photoStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(photoStream);
                    byte[] photoData = photoStream.ToArray();
                    List<FaceInfo> faces = await _faceApiHelper.Detect(photoData);

                    return Json(faces);
                }
            }
            else
            {
                return BadRequest("No image file found.");
            }
        }
    }
}
