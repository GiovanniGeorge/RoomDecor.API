using Microsoft.AspNetCore.Mvc;
using RoomDecor.API.Models;
using System.Text.Json;
using System.Text;

namespace RoomDecor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;

        public RoomsController(HttpClient httpClient, IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _env = env;
        }

        [HttpPost("recommend")]
        public async Task<IActionResult> RecommendRooms([FromBody] RoomSearchRequest request)
        {
            if (request.Area == 0 && request.Width > 0 && request.Length > 0)
            {
                request.Area = request.Width * request.Length;
            }

            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/recommend", content);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "AI Server Error");

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AIResponseRoot>(jsonResponse);

                if (result == null || result.RecommendedRooms == null)
                    return NotFound("No recommendations found.");

                var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";

                foreach (var room in result.RecommendedRooms)
                {
                    room.ImageUrl = $"{baseUrl}{room.Id}";
                    if (room.SimilarImages != null)
                    {
                        foreach (var sim in room.SimilarImages)
                        {
                            sim.ImageUrl = $"{baseUrl}{sim.Id}";
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("vision")]
        public async Task<IActionResult> GetSimilarImages([FromBody] VisionRequestDto request)
        {
            if (request.TopN <= 0)
            {
                request.TopN = 5;
            }

            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/vision", content);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Vision AI Server Error");

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<VisionResponseRoot>(jsonResponse);

                if (result == null || result.SimilarImages == null)
                    return NotFound("No similar images found.");

                var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";

                if (!string.IsNullOrEmpty(result.SelectedImage))
                {
                    result.SelectedImage = $"{baseUrl}{result.SelectedImage}";
                }

                foreach (var item in result.SimilarImages)
                {
                    item.ImageUrl = $"{baseUrl}{item.Id}";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("vision/upload")]
        public async Task<IActionResult> UploadAndGetSimilarImages([FromForm] VisionUploadRequestDto request)
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("No image uploaded.");

            if (request.TopN <= 0)
            {
                request.TopN = 5;
            }

            try
            {
                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(webRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }

                using var multipartFormContent = new MultipartFormDataContent();
                
                var imageStream = request.Image.OpenReadStream();
                var fileStreamContent = new StreamContent(imageStream);
                if (!string.IsNullOrEmpty(request.Image.ContentType))
                {
                    fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.Image.ContentType);
                }
                
                multipartFormContent.Add(fileStreamContent, name: "image", fileName: fileName);
                multipartFormContent.Add(new StringContent(request.TopN.ToString()), name: "top_n");

                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/vision/upload", multipartFormContent);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Vision AI Server Error");

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<VisionResponseRoot>(jsonResponse);

                if (result == null || result.SimilarImages == null)
                    return NotFound("No similar images found.");

                var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";

                // Set the SelectedImage to the URL of the uploaded file
                result.SelectedImage = $"{baseUrl}{fileName}";

                foreach (var item in result.SimilarImages)
                {
                    item.ImageUrl = $"{baseUrl}{item.Id}";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}