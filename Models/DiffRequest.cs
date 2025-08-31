using System.ComponentModel.DataAnnotations;

namespace StringDiff.Models;

public class DiffRequest
{
    [Required]
    public required string Input { get; set; }
}