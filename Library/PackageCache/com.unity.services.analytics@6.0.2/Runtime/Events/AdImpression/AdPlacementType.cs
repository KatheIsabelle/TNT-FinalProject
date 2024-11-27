namespace Unity.Services.Analytics
{
    /// <summary>
    /// Indicates what type of ad is shown.
    /// </summary>
    public enum AdPlacementType
    {
        /// <summary>
        /// A banner ad.
        /// </summary>
        BANNER = 0,
        /// <summary>
        /// A rewarded ad.
        /// </summary>
        REWARDED = 1,
        /// <summary>
        /// An interstitial ad.
        /// </summary>
        INTERSTITIAL = 2,
        /// <summary>
        /// Some other kind of ad.
        /// </summary>
        OTHER = 3
    }
}
