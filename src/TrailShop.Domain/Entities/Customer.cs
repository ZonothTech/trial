using System.ComponentModel.DataAnnotations;

namespace TrailShop.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(128)]
    public string DisplayName { get; set; } = string.Empty;

    public LoyaltyTier LoyaltyTier { get; set; } = LoyaltyTier.None;

    public bool IsAdmin { get; set; }
}
