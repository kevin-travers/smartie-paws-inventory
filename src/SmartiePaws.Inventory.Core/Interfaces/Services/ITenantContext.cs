namespace SmartiePaws.Inventory.Core.Interfaces.Services;

/// <summary>
/// Resolves which organization/Space/user the current request is acting as. Implemented in the
/// Service project from JWT claims (not yet wired up); consumed by <c>InventoryDbContext</c> in
/// the Data project to build global query filters. Filters fail closed: if a property here is
/// null, the corresponding filter matches zero rows rather than skipping the filter, so a missing
/// tenant context can never accidentally expose data across organizations/Spaces. Deliberate
/// cross-tenant access (e.g. a future background job) must use <c>.IgnoreQueryFilters()</c>
/// explicitly at the query site rather than relying on this returning null.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// The organization the current request is scoped to, if any.
    /// </summary>
    Guid? CurrentOrganizationId { get; }

    /// <summary>
    /// The Space the current request is scoped to, if any.
    /// </summary>
    Guid? CurrentSpaceId { get; }

    /// <summary>
    /// The authenticated user making the current request, if any.
    /// </summary>
    Guid? CurrentUserId { get; }
}
