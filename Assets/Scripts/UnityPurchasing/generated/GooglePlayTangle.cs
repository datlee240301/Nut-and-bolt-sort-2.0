// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("1x+maLJQjE20HN3ZDnPKmSDrJ7V+i8aOHHI4C7OcYqKcPxFbe3xiKE1lq26IRT+asQoZBfOiU57Hf0kUQq4NrV/T8wBbgY/ziZcrcg5zxeRl5ujn12Xm7eVl5ubnf+Cjw7PdZuNpuccLvrzuTpC1mdUZUIqznXJJ5ynOrZHEEjam20kMTeNzxfd41B3XZebF1+rh7s1hr2EQ6ubm5uLn5BJBO2mYl6IdhnfuzGSlR7ZC8YzKMJMLaSvmlOI8YAjGj3dMwaDB4oi8EXV+9gyNjBv5QieS7KNty6kSJb1HYNEDCcnCnmE/IyMhrzeZAyuPBmfi7iCP6SUnFtpR9jcAY/OL49QcCv34xfy4xIUm6qa5TCrkh4ZTSSWIsRFfmT2UNuXk5ufm");
        private static int[] order = new int[] { 7,11,9,9,11,5,12,7,9,11,10,12,12,13,14 };
        private static int key = 231;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
