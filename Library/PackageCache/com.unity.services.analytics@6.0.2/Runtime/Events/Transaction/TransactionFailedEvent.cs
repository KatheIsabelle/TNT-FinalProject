using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace Unity.Services.Analytics
{
    /// <summary>
    /// Use this class to record transactionFailed events.
    ///
    /// For more information about the transactionFailed event, see the documentation page:
    /// https://docs.unity.com/ugs/en-us/manual/analytics/manual/record-transaction-events
    /// </summary>
    public class TransactionFailedEvent : TransactionEvent
    {
        /// <summary>
        /// Creates a new TransactionFailedEvent instance that you can populate with the relevant data.
        /// </summary>
        public TransactionFailedEvent() : base("transactionFailed")
        {
        }

        /// <summary>
        /// (Required) The reason why this transaction failed.
        /// </summary>
        public string FailureReason { set { SetParameter("failureReason", value); } }

        /// <summary>
        /// The Validate method is called internally during serialization to ensure that all required parameters have been set.
        /// If any required parameters are missing, warnings are recorded to the console.
        /// </summary>
        public override void Validate()
        {
            base.Validate();

            if (!ParameterHasBeenSet("failureReason"))
            {
                Debug.LogWarning("A value for the FailureReason parameter is required for a TransactionFailed event.");
            }
        }
    }
}
