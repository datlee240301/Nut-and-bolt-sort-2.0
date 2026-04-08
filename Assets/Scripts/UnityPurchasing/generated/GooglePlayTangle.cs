// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("9FfPre8iUCb4pMwCS7OIBWQFJkyGaslpmxc3xJ9FSzdNU++2yrcBIImhb6pMgftedc7dwTdml1oDu43QoSIsIxOhIikhoSIiI7skZwd3GaK6TwJK2Lb8z3dYpmZY+9Wfv7im7NjOOTwBOHwAQeIuYn2I7iBDQpeNJ619A896eCqKVHFdEd2UTndZto141bG6MshJSN89huNWKGepD23W4XmDpBXHzQ0GWqX75+fla/Ndx+9LE9tirHaUSIlw2BkdyrcOXeQv43HCoyYq5Est4ePSHpUy88SnN08nENaF/61cU2bZQrMqCKBhg3KGNUgOE6EiARMuJSoJpWul1C4iIiImIyAj7QppVQDW8mIfjciJJ7cBM7wQ2eFMddWbXflQ8iEgIiMi");
        private static int[] order = new int[] { 4,2,9,9,11,13,13,10,13,10,13,13,13,13,14 };
        private static int key = 35;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
