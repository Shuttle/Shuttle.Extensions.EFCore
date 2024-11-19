using System.ComponentModel.DataAnnotations;

namespace Shuttle.Extensions.EFCore.Tests.Models;

public class FixtureEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [StringLength(65)]
    public string Name { get; set; } = string.Empty;
}