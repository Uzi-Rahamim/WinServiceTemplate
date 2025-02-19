using Microsoft.Win32;

namespace Utilities
{
    public class RegistryUtils
    {
        public static List<string> GetKeySubStringValues(string Key)
        {
            List<string> pathList = new List<string>();
            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(Key))
            {
                if (key != null)
                {
                    var subKeyNames = key.GetValueNames();
                    foreach (string subkeyName in subKeyNames)
                    {

                        var path = key.GetValue(subkeyName);
                        if (path == null)
                            continue;

                        pathList.Add((string)path);
                    }
                }
            }
            return pathList;
        }
    }
}
