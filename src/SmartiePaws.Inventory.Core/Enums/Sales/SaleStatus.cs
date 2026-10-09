namespace SmartiePaws.Inventory.Core.Enums.Sales;

/// <summary>
/// The status of a sale.
/// </summary>
public enum SaleStatus
{
  /// <summary>
  /// The sale is being built -- line items are still being added/removed and nothing has been
  /// paid yet.
  /// </summary>
  InProgress,

  /// <summary>
  /// The sale is finalized and the customer has paid. Counts toward revenue and cost-of-goods
  /// reporting.
  /// </summary>
  Completed,

  /// <summary>
  /// The sale was called off -- either abandoned while InProgress, or undone after being
  /// Completed. Does not count toward revenue; the row is kept for audit rather than deleted.
  /// </summary>
  Voided,
}
