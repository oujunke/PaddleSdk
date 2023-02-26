using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaddleSdk.PaddleFluidCLib;

namespace PaddleSdk
{
   public static class Expands
    {

        public static T2 Aggregate<T1,T2>(this IEnumerable<T1> source, Func<T2, T1, T2> func)
        {
            T2 t2 = default(T2);
            foreach (var item in source)
            {
                t2 = func(t2, item);
            }
            return t2;
        }
    }
}
