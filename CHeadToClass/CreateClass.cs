using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHeadToClass
{
    internal class CreateClass
    {
        public static string Create()
        {
            StringBuilder builder = new StringBuilder("""
                using System;
                using System.Runtime.InteropServices;

                namespace PaddleSdk
                {
                    public class PaddleFluidCLib
                    {
                """);
            var cl = GetCHeadClasses();
            foreach (var c in cl)
            {
                builder.AppendLine("#region " + c.Name);
                
                builder.AppendLine("#endregion");
            }
            builder.Append("""
                    }
                }
                """);
            return builder.ToString();
        }
        public static List<CHeadClass> GetCHeadClasses()
        {
            List<CHeadClass> result = new List<CHeadClass>();
            foreach (var fi in new DirectoryInfo("CHead").GetFiles("*.h"))
            {
                int status = 0;
                foreach (var lin in File.ReadAllLines(fi.FullName))
                {
                    if (lin[0] == '#')
                    {
                        continue;
                    }
                }
            }
            return result;
        }
        public class CHeadClass
        {
            public string Name { set; get; }
            public string Desc { set; get; }
            public List<CHeadMemth> Memths { set; get; }
        }
        public class CHeadMemth
        {
            public string ResultType { set; get; }
            public string Name { set; get; }
            public string Desc { set; get; }
            public List<CHeadPar> Pars { set; get; }
        }
        public class CHeadPar
        {
            public string Name { set; get; }
            public string Desc { set; get; }
            public string Type { set; get; }
        }
    }
}
