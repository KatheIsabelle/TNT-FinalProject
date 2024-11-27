namespace Unity.Services.Analytics
{
    /// <summary>
    /// The server to use for receipt verification, if applicable.
    /// </summary>
    public enum TransactionServer
    {
        /// <summary>
        /// Apple
        /// </summary>
        APPLE = 0,
        /// <summary>
        /// Amazon
        /// </summary>
        AMAZON = 1,
        /// <summary>
        /// Google
        /// </summary>
        GOOGLE = 2,
        /// <summary>
        /// Valve
        /// </summary>
        VALVE = 3
    }
}
