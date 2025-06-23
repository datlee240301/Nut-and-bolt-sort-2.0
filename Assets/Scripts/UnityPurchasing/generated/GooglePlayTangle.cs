// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("rGTdE8kr9zbPZ6aidQix4luQXM59HJmVW/SSXlxtoSqNTHsYiPCYr6wenb6skZqVthrUGmuRnZ2dmZyfNh7QFfM+ROHKcWJ+iNko5bwEMm9L6HASUJ3vmUcbc730DDe627qZ8wXwvfVnCUNwyOcZ2edEaiAABxlTxjwbqnhysrnlGkRYWFrUTOJ4UPSYEsK8cMXHlTXrzuKuYivxyOYJMsdqDgWNd/b3YII5XOmX2Baw0mleOdV21iSoiHsg+vSI8uxQCXUIvp8enZOcrB6dlp4enZ2cBJvYuMimHZxStdbqv2lN3aAydzaYCL6MA69maTpAEuPs2Wb9DJW3H948zTmK97FncYaDvofDv/5dkd3CN1Gf/P0oMl7zymok4kbvTZ6fnZyd");
        private static int[] order = new int[] { 7,8,7,9,4,11,6,11,10,11,10,12,12,13,14 };
        private static int key = 156;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
