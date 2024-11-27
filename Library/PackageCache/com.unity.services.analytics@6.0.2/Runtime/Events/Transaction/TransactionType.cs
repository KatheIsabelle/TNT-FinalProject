namespace Unity.Services.Analytics
{
    /// <summary>
    /// The type of the transaction.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        INVALID = 0,
        /// <summary>
        /// Sale.
        /// </summary>
        SALE = 1,
        /// <summary>
        /// Purchase.
        /// </summary>
        PURCHASE = 2,
        /// <summary>
        /// Trade.
        /// </summary>
        TRADE = 3
    }
}
