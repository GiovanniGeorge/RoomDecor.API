using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoomDecor.API.Models
{
    public class RegisterDto
    {
        [Required] public string DisplayName { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class LoginDto
    {
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class RoomSearchRequest
    {
        [JsonPropertyName("style")] public string Style { get; set; }
        [JsonPropertyName("width")] public decimal Width { get; set; }
        [JsonPropertyName("length")] public decimal Length { get; set; }
        [JsonPropertyName("area")] public decimal Area { get; set; }
    }

    public class AIResponseRoot
    {
        [JsonPropertyName("recommended_rooms")]
        public List<RoomResponse> RecommendedRooms { get; set; }
    }

    public class RoomResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("style")] public string Style { get; set; }
        [JsonPropertyName("area")] public decimal Area { get; set; }
        [JsonPropertyName("width")] public decimal Width { get; set; }
        [JsonPropertyName("length")] public decimal Length { get; set; }
        [JsonPropertyName("score")] public double Score { get; set; }
        [JsonPropertyName("similar_images")] public List<SimilarImageDto> SimilarImages { get; set; }
        public string ImageUrl { get; set; }
    }

    public class SimilarImageDto
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("similarity")] public double Similarity { get; set; }
        public string ImageUrl { get; set; }
    }

    public class VisionRequestDto
    {
        [JsonPropertyName("image_id")] public string ImageId { get; set; }
        [JsonPropertyName("top_n")] public int TopN { get; set; } = 5;
    }

    public class VisionResponseRoot
    {
        [JsonPropertyName("selected_image")]
        public string SelectedImage { get; set; }

        [JsonPropertyName("foreground_image")]
        public string ForegroundImage { get; set; }

        [JsonPropertyName("similar_images")]
        public List<VisionItemDto> SimilarImages { get; set; }
    }

    public class VisionItemDto
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("similarity_score")] public double SimilarityScore { get; set; }
        public string ImageUrl { get; set; }
    }
}