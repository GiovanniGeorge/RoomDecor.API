using Microsoft.AspNetCore.Mvc;
using RoomDecor.API.Models;
using System.Text.Json;

namespace RoomDecor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;

        public ChatController(HttpClient httpClient, IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromForm] ChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            try
            {
                using var multipartFormContent = new MultipartFormDataContent();
                
                // Add the text message
                multipartFormContent.Add(new StringContent(request.Message), name: "message");

                // Process image if provided
                string? fileName = null;
                if (request.Image != null && request.Image.Length > 0)
                {
                    var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadsFolder = Path.Combine(webRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(stream);
                    }

                    var imageStream = request.Image.OpenReadStream();
                    var fileStreamContent = new StreamContent(imageStream);
                    if (!string.IsNullOrEmpty(request.Image.ContentType))
                    {
                        fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.Image.ContentType);
                    }
                    
                    multipartFormContent.Add(fileStreamContent, name: "image", fileName: fileName);
                }

                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/chat", multipartFormContent);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "AI Chat Server Error");

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ChatResponseDto>(jsonResponse);

                if (result == null || string.IsNullOrEmpty(result.Reply))
                    return NotFound("No reply from AI.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
