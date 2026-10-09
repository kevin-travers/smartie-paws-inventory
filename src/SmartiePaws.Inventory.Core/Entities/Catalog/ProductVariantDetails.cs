namespace SmartiePaws.Inventory.Core.Entities.Catalog;

/// <summary>
/// Rich, per-product data that varies unpredictably by product/category (nutrition facts,
/// unique specs, anything without a fixed shape across the whole catalog). Kept in its own
/// one-to-one table rather than a column on <see cref="ProductVariant"/> so the frequently
/// queried variant table (hit on every sale and stock check) stays lean -- this table only gets
/// joined in when something actually needs the extended data, e.g. a product detail page. Not
/// part of the MVP yet -- planned ahead so <see cref="ProductVariant"/> doesn't need a
/// disruptive schema change later.
/// </summary>
public class ProductVariantDetails
{
  /// <summary>
  /// Primary key and foreign key, shared with <see cref="ProductVariant"/> rather than its own
  /// separate Id -- this is a strict one-to-one relationship, not a one-to-many.
  /// </summary>
  public Guid ProductVariantId { get; set; }

  /// <summary>
  /// Navigation to the variant this extends. Only populated when explicitly requested.
  /// </summary>
  public ProductVariant ProductVariant { get; set; } = null!;

  /// <summary>
  /// Arbitrary per-product data with no fixed shape across the catalog -- nutrition facts for
  /// one product, dimensions for another, whatever is category-specific. Persisted as a Postgres
  /// jsonb column (indexable and queryable despite being schemaless) rather than a separate
  /// NoSQL database, so everything stays in one transactional store.
  /// </summary>
  public Dictionary<string, object>? CustomAttributes { get; set; }

  /// <summary>
  /// UTC timestamp set by the database when this row was first inserted.
  /// </summary>
  public DateTimeOffset CreatedAt { get; set; }

  /// <summary>
  /// UTC timestamp set by the database on insert and advanced on every subsequent update.
  /// </summary>
  public DateTimeOffset UpdatedAt { get; set; }

  // No DeletedAt -- this row's lifecycle is tied directly to its ProductVariant; there's no
  // independent "deactivate just the extra details" concept.
}
