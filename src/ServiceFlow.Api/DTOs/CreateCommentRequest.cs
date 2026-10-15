using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Api.DTOs;

public class CreateCommentRequest
{
    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;
}