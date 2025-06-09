// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("FNxlq3GTT4533x4azbAJWuMo5HbfyT47Bj97B0blKWV6j+knREWQihSmJQYUKSItDqJsotMpJSUlISQnf9K2vTXPTk/YOoHkUS9grghq0ebzUMiq6CVXIf+jywVMtI8CYwIhS71IBU3fsfvIcF+hYV/80pi4v6HrIKp6BMh9fy2NU3ZaFtqTSXBesYrFpCEt40wq5uTVGZI19MOgMEggF36EoxLAygoBXaL84ODibPRawOhM0YL4qltUYd5FtC0Pp2aEdYEyTwmOpmitS4b8WXLJ2sYwYZBdBLyK16YlKyQUpiUuJqYlJSS8I2AAcB6lgW3ObpwQMMOYQkwwSlTosc2wBick6g1uUgfR9WUYis+OILAGNLsX3uZLctKcWv5X9SYnJSQl");
        private static int[] order = new int[] { 7,13,7,10,4,11,11,8,11,10,10,13,13,13,14 };
        private static int key = 36;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
