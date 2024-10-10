// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("NEvZ4UI2n6LfZB9HV8ZX/nu2HdHfSSi7WvhhFRfUApDpM4MVoans9QYgQGslYQfRiARLXsni0w8ZlncdK9AC22pwZAPGxOx2B4oJM4PC3FAHVW/qdb9IWRY3acdF3+/DuyEg+J5LheJ8kP9bUVOj7U0cS61hpCf0AD8NwjsT39/ftrVWUW5l2yDP+fQ9rWNgUn4VHCEaHdty4QMQds4+qxfhoAGKEJN4e69m0MfWVNJkMZM+6Wpka1vpamFp6Wpqa7qVPxF7VQQ2jBRCHe9HuGQcNJGjuEu3bGSjiJyugVBoO/sHIfe9wkEmnZwOmgc+W+lqSVtmbWJB7SPtnGZqampua2ivyjCvxBD/oalz8PNddoc5Fjt4MNXst+DfiXBGRmloamtq");
        private static int[] order = new int[] { 13,6,10,11,7,11,8,10,9,9,10,12,13,13,14 };
        private static int key = 107;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
