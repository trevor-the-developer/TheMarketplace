using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.Data.Entities;

public class UserProfile : BaseEntity
{
    [Required] public required string DisplayName { get; set; }

    public required string Bio { get; set; }

    public required string SocialMedia { get; set; }

    // navigation properties
    [Key]
    [ForeignKey("ApplicationUser")]
    [Length(maximumLength: 5000, minimumLength: 5000)]
    public required string ApplicationUserId { get; set; }

    public virtual ApplicationUser? ApplicationUser { get; set; }
}