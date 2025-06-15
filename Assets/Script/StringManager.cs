internal class StringManager
{
    internal static string ticketNumber = "TicketNumber";
    internal static string undoNumber = "undoNumber";
    internal static string addTubeNumber = "addTubeNumber";
    internal static string musicId = "MusicId";
    internal static string soundId = "SoundId";
    internal static string vibrateID = "VibrateID";
    internal static string hasBuyTheme1 = "HasBuyTheme1";
    internal static string hasBuyTheme2 = "HasBuyTheme2";
    internal static string hasBuyTheme3 = "HasBuyTheme3";
    internal static string hasBuyTheme4 = "HasBuyTheme4";
    internal static string hasBuyTheme5 = "HasBuyTheme5";
    internal static string hasBuyTheme6 = "HasBuyTheme6";
    internal static string hasBuyTheme7 = "HasBuyTheme7";
    internal static string hasBuyTheme8 = "HasBuyTheme8";
    internal static string hasBuyTheme9 = "HasBuyTheme9";
    internal static string useTheme1 = "UseTheme1";
    internal static string useTheme2 = "UseTheme2";
    internal static string useTheme3 = "UseTheme3";
    internal static string useTheme4 = "UseTheme4";
    internal static string useTheme5 = "UseTheme5";
    internal static string useTheme6 = "UseTheme6";
    internal static string useTheme7 = "UseTheme7";
    internal static string useTheme8 = "UseTheme8";
    internal static string useTheme9 = "UseTheme9";
    internal static string currentLevelId = "CurrentLevelId";
    internal static string pressPlayButton = "PressPlayButton";
    internal static string currentLevelIdLevelButton = "CurrentLevelIdLevelButton";
    internal static string pressLevelButton = "PressLevelButton";
    internal static string PackId = "PackId";

    public static string GetThemeKey(int themeId)
    {
        return $"HasBuyTheme{themeId}";
    }
}