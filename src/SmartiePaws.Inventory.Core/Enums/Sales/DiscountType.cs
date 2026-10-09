namespace SmartiePaws.Inventory.Core.Enums.Sales;


/// <summary>
/// How a discount's paired value should be turned into an actual dollar amount.
/// </summary>
public enum DiscountType
{
  /// <summary>
  /// The discount amount is calculated as a percentage of the amount being discounted
  /// (e.g. a value of 10 means 10% off).
  /// </summary>
  Percent,

  /// <summary>
  /// The discount amount is the value itself, as a fixed dollar amount subtracted directly
  /// (e.g. a value of 10 means $10 off), regardless of the amount being discounted.
  /// </summary>
  Flat
}
