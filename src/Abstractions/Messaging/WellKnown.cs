namespace Abstractions.Messaging;

public static class WellKnown
{
    public static class Topic
    {
        public const string SwitchLanguage = "SiriusClient:Broadcast:SwitchLanguage";
    }

    public static class Message
    {
        public const string English = "en-US";

        public const string ChineseSimplified = "zh-CN";

        public const string ChineseTraditional = "zh-HK";
    }
}