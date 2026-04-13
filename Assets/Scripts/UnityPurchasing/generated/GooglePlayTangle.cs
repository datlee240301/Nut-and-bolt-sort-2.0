// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("0ykOv21np6zwD1FNTU/BWfdtReEjC8UA5itR9N9kd2udzD3wqREneizAY8MxvZ1uNe/hnef5RRxgHauKC4iGibkLiIOLC4iIiRGOza3dswiJR6DD/6p8WMi1J2IjjR2rmRa6c9J/GxCYYuPidZcsSfyCzQOlx3xLXv1lB0WI+oxSDmao4Rkir86vjOZoCYyATuGHS0l4tD+YWW4NneWNunwvVQf2+cxz6BmAogrLKdgsn+KkEOWo4HIcVmXd8gzM8lF/NRUSDEa5ccgG3D7iI9pys7dgHaT3ToVJ27kLiKu5hI+Aow/BD36EiIiIjImKjQfXqWXQ0oAg/tv3u3c+5N3zHCdyZJOWq5LWqutIhMjXIkSK6eg9J0vm338x91P6WIuKiImI");
        private static int[] order = new int[] { 6,9,2,9,12,10,12,8,9,11,11,12,12,13,14 };
        private static int key = 137;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
