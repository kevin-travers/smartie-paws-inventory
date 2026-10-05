using Microsoft.AspNetCore.Identity;

namespace SmartiePaws.Inventory.Core.Entities.Identity;

/// <summary>
/// Extends ASP.NET Core Identity's <see cref="IdentityUser{TKey}"/> with the fields this
/// application needs. A user can belong to multiple organizations and Spaces via
/// <see cref="OrganizationMembership"/> and <see cref="SpaceMembership"/>.
/// </summary>
public class User : IdentityUser<Guid>
{
    /// <summary>
    /// The user's full display name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp set by the database when this row was first inserted.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// UTC timestamp set by the database on insert and advanced on every subsequent update.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// UTC timestamp of a soft delete. Null while the user is active.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }
}
