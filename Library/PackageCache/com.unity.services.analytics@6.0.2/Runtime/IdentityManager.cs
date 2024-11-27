using System;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;

namespace Unity.Services.Analytics.Internal
{
    internal interface IIdentityManager
    {
        string UserId { get; }
        string InstallId { get; }
        string PlayerId { get; }
        string ExternalId { get; }

        bool IsNewPlayer { get; }

        event Action OnPlayerChanged;

        void Initialize();
    }

    internal class IdentityManager : IIdentityManager
    {
        internal const string k_UnityAnalyticsInstallationIdKey = "UnityAnalyticsInstallationId";
        internal const string k_UnityAnalyticsUserIdKey = "UnityAnalyticsUserId";

        readonly IPlayerId m_PlayerId;
        readonly IExternalUserId m_ExternalIdProvider;
        readonly IPersistence m_Persistence;

        bool m_Initialized;

        public string UserId { get; private set; }
        public string InstallId { get; private set; }
        public string PlayerId { get { return m_PlayerId?.PlayerId; } }
        public string ExternalId { get; private set; }

        public bool IsNewPlayer { get; private set; }

        public event Action OnPlayerChanged;

        public IdentityManager(IInstallationId installId, IPlayerId playerId, IExternalUserId externalId, IPersistence persistence)
        {
            InstallId = installId.GetOrCreateIdentifier();
            m_PlayerId = playerId;
            m_ExternalIdProvider = externalId;
            m_Persistence = persistence;

            if (m_ExternalIdProvider != null)
            {
                m_ExternalIdProvider.UserIdChanged += ExternalUserIdChanged;
                ExternalId = m_ExternalIdProvider.UserId;
            }
            // Even though we'll deal with this properly if/when we Initialize, we need to set it up now
            // so that the Debug Panel will display the correct ID(s).
            UserId = !String.IsNullOrEmpty(ExternalId) ? ExternalId : InstallId;
        }

        public void Initialize()
        {
            if (!m_Initialized)
            {
                string lastUserId = m_Persistence.LoadString(k_UnityAnalyticsUserIdKey);
                bool migrating = false;
                if (String.IsNullOrEmpty(lastUserId))
                {
                    // Backwards compatibility while we phase out the old key.
                    lastUserId = m_Persistence.LoadString(k_UnityAnalyticsInstallationIdKey);
                    m_Persistence.ClearValue(k_UnityAnalyticsInstallationIdKey);
                    migrating = !String.IsNullOrEmpty(lastUserId);
                }

                if (m_ExternalIdProvider != null)
                {
                    ExternalId = m_ExternalIdProvider.UserId;
                }

                UserId = String.IsNullOrEmpty(ExternalId) ? InstallId : ExternalId;

                // "Have we initialized as a different user than last time?"
                IsNewPlayer = String.IsNullOrEmpty(lastUserId) || !lastUserId.Equals(UserId, StringComparison.Ordinal);
                if (IsNewPlayer || migrating)
                {
                    m_Persistence.SaveValue(k_UnityAnalyticsUserIdKey, UserId);
                }

                m_Initialized = true;
            }
        }

        private void ExternalUserIdChanged(string newName)
        {
            // If we have not yet been initialized, then Initialize() above will examine
            // the initial state and handle it there. This method is for runtime/post-first-enablement changes.
            if (m_Initialized)
            {
                // Changing the ID at runtime/after initialisation.

                if (UserId.Equals(newName, StringComparison.Ordinal))
                {
                    // Do nothing, you have not changed the ID.
                    // Potentially weird edge case: what if they set the external ID to be the installation ID?
                    // Would that make a blind bit of difference to anything? No, but it could be confusing.
                }
                else
                {
                    if (String.IsNullOrEmpty(newName))
                    {
                        // We are clearing our externalID and falling back to installationID.
                        // An externalID was set before but is null now.
                        UserId = InstallId;
                    }
                    else if (String.IsNullOrEmpty(ExternalId))
                    {
                        // We are changing from installationID to externalID.
                        // An externalID was not set before but is set now.
                        UserId = newName;
                    }
                    else if (!ExternalId.Equals(newName, StringComparison.Ordinal))
                    {
                        // We have changed our externalID to a different externalID.
                        UserId = newName;
                    }

                    m_Persistence.SaveValue(k_UnityAnalyticsUserIdKey, UserId);
                    ExternalId = newName;
                    IsNewPlayer = true;

                    OnPlayerChanged?.Invoke();
                }
            }
            else
            {
                // Even though we'll deal with this properly if/when we Initialize, we need to set it up now
                // so that the Debug Panel will display the correct ID(s).
                ExternalId = newName;
                UserId = !String.IsNullOrEmpty(ExternalId) ? ExternalId : InstallId;
            }
        }
    }
}
