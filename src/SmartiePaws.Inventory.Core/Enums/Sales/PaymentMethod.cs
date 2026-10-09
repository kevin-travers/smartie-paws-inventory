namespace SmartiePaws.Inventory.Core.Enums.Sales;

/// <summary>
/// How a customer paid for a sale
/// </summary>
public enum PaymentMethod
{
  /// <summary>
  /// Cash, cash is king baby the pefered way of payment
  /// </summary>
  Cash,

  /// <summary>
  /// Venmo cash app used to pay
  /// </summary>
  Venmo,

  /// <summary>
  /// Zelle everyone should have this way to pay
  /// </summary>
  Zelle,

  /// <summary>
  /// I did not know people still used checks
  /// </summary>
  Check,

  /// <summary>
  /// Bitcoin is going to the moon so we accecpt it as payment
  /// </summary>
  Coinbase,

  /// <summary>
  /// If pizza delivery boys can accept a service for pizza so can we :)
  /// </summary>
  Service,

  /// <summary>
  /// If customer wants to pay with something like their first bordn child
  /// </summary>
  Unknown,

}
