using System.ComponentModel.DataAnnotations;

namespace FindFi.Bll.DTOs;

public class CreateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}