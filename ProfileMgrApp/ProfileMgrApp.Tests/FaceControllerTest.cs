using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProfileMgrApp.Controllers;
using ProfileMgrApp.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace ProfileMgrApp.Tests
{
    /// <summary>
    ///  FaceController Test
    /// </summary>
    public class FaceControllerTest
    {
        private FaceController _controller;

        public FaceControllerTest()
        {
            var client = new HttpClient();

            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\appsettings.json");
            var builder = new ConfigurationBuilder().AddJsonFile(jsonFile);
            var helper = new FaceApiHelper(client, null, builder.Build());

            _controller = new FaceController(helper);
        }

        [Fact]        
        public async void TestDetect()
        {
            // test no face image
            IFormFile formFile = CreateFormFile("test-none.jpg");
            var result = await _controller.Detect(formFile);
            var jsonResult = Assert.IsType<JsonResult>(result);
            List<FaceInfo> faceList = (List<FaceInfo>) jsonResult.Value;
            Assert.Empty(faceList);

            // test one face image 
            formFile = CreateFormFile("test-one.jpg");
            result = await _controller.Detect(formFile);
            jsonResult = Assert.IsType<JsonResult>(result);
            faceList = (List<FaceInfo>) jsonResult.Value;
            Assert.Single(faceList);

            // test multi-face image
            formFile = CreateFormFile("test-three.jpg");
            result = await _controller.Detect(formFile);
            jsonResult = Assert.IsType<JsonResult>(result);
            faceList = (List<FaceInfo>)jsonResult.Value;
            Assert.Equal(3, faceList.Count());
        }

        private IFormFile CreateFormFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\images\\", fileName);
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IFormFile formFile = new FormFile(file, 0, file.Length, "ImageFile", fileName);

            return formFile;
        }
    }
}
