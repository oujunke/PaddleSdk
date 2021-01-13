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
        /// <summary>
        /// 读取PD_ZeroCopyTensor类型
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        /// <returns></returns>
        public static object GetValue(this PD_ZeroCopyTensor zeroCopyTensor)
        {
            var bs = zeroCopyTensor.Data.ReadData();
            if (zeroCopyTensor.Shape.Length > 0)
            {
                var shape = GetInt32(zeroCopyTensor.Shape.ReadData());
                var lod = GetInt32(zeroCopyTensor.Lod.ReadData());
                var numLength = 0;
                var length = 0;
                switch (zeroCopyTensor.Dtype)
                {
                    case PD_DataType.PD_FLOAT32:
                        {
                            numLength = sizeof(float);
                            length = bs.Length / numLength;
                            List<float> data = new List<float>();
                            for (int i = 0; i < length; i++)
                            {
                                data.Add(BitConverter.ToSingle(bs, i * numLength));
                            }
                            return new DataRead<float>(data, shape, lod);
                        }
                    case PD_DataType.PD_INT32:
                        {
                            numLength = sizeof(int);
                            length = bs.Length / numLength;
                            List<int> data = new List<int>();
                            for (int i = 0; i < length; i++)
                            {
                                data.Add(BitConverter.ToInt32(bs, i * numLength));
                            }
                            return new DataRead<int>(data, shape, lod);
                        }
                    case PD_DataType.PD_INT64:
                        {
                            numLength = sizeof(long);
                            length = bs.Length / numLength;
                            List<long> data = new List<long>();
                            for (int i = 0; i < length; i++)
                            {
                                data.Add(BitConverter.ToInt64(bs, i * numLength));
                            }
                            return new DataRead<long>(data, shape, lod);
                        }
                }
            }
            else
            {
                return bs;
            }
            return null;
        }
        /// <summary>
        /// 从字节数组中获取int数组
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static int[] GetInt32(byte[] bs)
        {
            var numLength = sizeof(int);
            var length = bs.Length / numLength;
            var result = new int[length];
            for (int i = 0; i < length;i++)
            {
                result[i] = BitConverter.ToInt32(bs,i*numLength);
            }
            return result;
        }

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
