namespace SmartiePaws.Inventory.Core.Enums;

/// <summary>
/// A user's role within an Organization, assigned via <see cref="Entities.OrganizationMembership"/>.
/// Distinct from <see cref="SpaceRole"/>, which governs access within a single Space.
/// </summary>
public enum OrganizationRole
{
    /// <summary>
    /// Everything <see cref="Admin"/> can, plus irreversible actions: delete the organization,
    /// manage billing, transfer ownership.
    /// </summary>
    Owner,

    /// <summary>
    /// Implicit access to every Space in the organization, with no <see cref="Entities.SpaceMembership"/>
    /// row required. Can invite new users into the organization or into any Space.
    /// </summary>
    Admin,

    /// <summary>
    /// No organization-wide power. Has zero access to any Space until explicitly granted
    /// a <see cref="Entities.SpaceMembership"/>.
    /// </summary>
    Member
}
