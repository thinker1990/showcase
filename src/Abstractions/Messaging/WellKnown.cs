namespace Abstractions.Messaging;

/// <summary>
/// Contains well-known topics and messages for the messaging system.
/// </summary>
public static class WellKnown
{
    /// <summary>
    /// Contains well-known topics.
    /// </summary>
    public static class Topic
    {
        /// <summary>
        /// The topic for switching languages.
        /// </summary>
        public const string SwitchLanguage = "SiriusClient:Broadcast:SwitchLanguage";
    }

    /// <summary>
    /// Contains well-known messages.
    /// </summary>
    public static class Message
    {
        /// <summary>
        /// The message for English language.
        /// </summary>
        public const string English = "en-US";

        /// <summary>
        /// The message for Simplified Chinese language.
        /// </summary>
        public const string ChineseSimplified = "zh-CN";

        /// <summary>
        /// The message for Traditional Chinese language.
        /// </summary>
        public const string ChineseTraditional = "zh-HK";
    }
}
