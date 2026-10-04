namespace SmartiePaws.Inventory.Core.Enums;

/// <summary>
/// A user's role within a single Space, assigned via <see cref="Entities.SpaceMembership"/>.
/// Only meaningful for a user whose <see cref="OrganizationRole"/> is <see cref="OrganizationRole.Member"/> —
/// an organization <see cref="OrganizationRole.Admin"/> or <see cref="OrganizationRole.Owner"/>
/// already has full access to every Space without one of these.
/// </summary>
public enum SpaceRole
{
    /// <summary>
    /// Read access and the ability to create transactions (sales, purchase entries) within this Space.
    /// Cannot update or delete product info, and cannot invite anyone.
    /// </summary>
    Member,

    /// <summary>
    /// Full control of this Space: update/delete product info, manage day-to-day operations,
    /// and invite/add people to this Space only.
    /// </summary>
    Manager
}
