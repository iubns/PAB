using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PAB.dll
{
    public static class DllManager
    {
        private static Dictionary<string, Assembly> dict = new Dictionary<string, Assembly>();
        public static bool LoadDll(string path)
        {
            AppDomain.CurrentDomain.AssemblyResolve += Assembly_Resolve;
            Assembly curAssm = Assembly.GetExecutingAssembly();
            var temp = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string appName = curAssm.GetName().Name;
            Assembly dllAssm = null;
            byte[] dllData;
            using (Stream s = curAssm.GetManifestResourceStream($"{appName}.{path}"))
            {
                if (s != null)
                {
                    dllData = new byte[s.Length];
                    s.Read(dllData, 0, (int)s.Length);
                    dllAssm = Assembly.Load(dllData);
                }
                else
                {
                    return false;
                }
            }
            dict.Add(dllAssm.FullName, dllAssm);
            return true;
        }

        private static Assembly Assembly_Resolve(object sender, ResolveEventArgs e)
        {
            if (dict.ContainsKey(e.Name))
            {
                Assembly assm = dict[e.Name];
                dict.Remove(e.Name);
                return assm;
            }
            return null;
        }

    }
}