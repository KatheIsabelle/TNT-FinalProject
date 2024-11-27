using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Services.Analytics
{
    /// <summary>
    /// Use this class to record Custom events that are specific to your game, without creating a specific
    /// sub-class of Event.
    ///
    /// The CustomEvent class is provided as a low-effort way to migrate to the new
    /// RecordEvent(...) API from the dictionary-based approach used by prior versions of the Analytics
    /// SDK (the CustomData(...) API). CustomEvent instances can be treated as write-only dictionaries
    /// through collection initialization and the string indexer, meaning the code changes to migrate to
    /// the new API should be minimal.
    ///
    /// For more information about custom events, see the documentation page:
    /// https://docs.unity.com/ugs/en-us/manual/analytics/manual/record-custom-events
    /// </summary>
    public class CustomEvent : Event, IEnumerable
    {
        /// <summary>
        /// Creates a new CustomEvent instance for the given event type that you can populate with the relevant data.
        /// </summary>
        /// <param name="name">The name of the schema this event will match.</param>
        public CustomEvent(string name) : base(name)
        {
        }

        /// <summary>
        /// Sets the parameter key to the given value. The value's type must match the type defined in the event schema. Only primitive values (string/int/bool/float) are acceptable.
        /// </summary>
        /// <param name="key">The name of this parameter, as defined in the event schema.</param>
        /// <exception cref="ArgumentException">Thrown if an unsupported type is given for the value.</exception>
        public object this[string key] { set { Add(key, value); } }

        /// <summary>
        /// Sets the parameter key to the given value.
        /// </summary>
        /// <param name="key">The name of this parameter, as defined in the event schema.</param>
        /// <param name="value">The value to store for this parameter, of the type defined in the event schema. Only primitive values (string/int/bool/float) are acceptable.</param>
        /// <exception cref="ArgumentException">Thrown if an unsupported type is given for the value.</exception>
        public void Add(string key, object value)
        {
            Type type = value.GetType();
            if (type == typeof(string))
            {
                SetParameter(key, (string)value);
            }
            else if (type == typeof(int))
            {
                SetParameter(key, (int)value);
            }
            else if (type == typeof(long))
            {
                SetParameter(key, (long)value);
            }
            else if (type == typeof(float))
            {
                SetParameter(key, (float)value);
            }
            else if (type == typeof(double))
            {
                SetParameter(key, (double)value);
            }
            else if (type == typeof(bool))
            {
                SetParameter(key, (bool)value);
            }
            else
            {
                throw new ArgumentException($"Values of type {type} cannot be included as event parameters.");
            }
        }

        /// <summary>
        /// Provided to allow collection initialization, not intended for public use.
        /// </summary>
        /// <returns>An enumerator over all the key/value pairs written to this event.</returns>
        public IEnumerator GetEnumerator()
        {
            // I don't like this because event classes are meant to be write-only containers, but we need
            // to implement IEnumerable to get collection initialisation. Thus, we must actually provide
            // a meaningful enumerator like a user would expect... just in case.
            // It's probably better than throwing a NotImplementedException? (Or even InvalidOperationException?)
            foreach (KeyValuePair<string, string> kvp in m_Strings)
            {
                yield return new KeyValuePair<string, object>(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, long> kvp in m_Integers)
            {
                yield return new KeyValuePair<string, object>(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, double> kvp in m_Floats)
            {
                yield return new KeyValuePair<string, object>(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, bool> kvp in m_Booleans)
            {
                yield return new KeyValuePair<string, object>(kvp.Key, kvp.Value);
            }
        }
    }
}
