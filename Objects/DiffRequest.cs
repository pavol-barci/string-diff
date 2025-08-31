using System.ComponentModel.DataAnnotations;

namespace StringDiff.Objects;

public class DiffRequest
{
    [Required]
    public required string Input { get; set; }
}