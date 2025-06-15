// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Za0U2gDiPv8Grm9rvMF4K5JZlQfMOXQ8rsCKuQEu0BAujaPpyc7QmqDzidsqJRCvNMVcftYX9QTwQz54DqPHzES+Pz6pS/CVIF4R33kboJfwHL8f7WFBsukzPUE7JZnAvMF3Vv/XGdw6940oA7irt0EQ4Sx1zfumZddUd2VYU1x/0x3TolhUVFRQVVYP9dJjsbt7cCzTjZGRkx2FK7GZPbTVUFySPVuXlaRo40SFstFBOVFmgiG525lUJlCO0rp0PcX+cxJzUDquuE9Kd04KdjeUWBQL/phWNTTh+1WbfB8jdqCEFGn7vv9RwXdFymav11RaVWXXVF9X11RUVc1SEXEBb9RR2wt1uQwOXPwiBytnq+I4AS/A+5c6A6PtK48mhFdWVFVU");
        private static int[] order = new int[] { 7,11,3,10,10,9,7,7,8,10,13,12,12,13,14 };
        private static int key = 85;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
