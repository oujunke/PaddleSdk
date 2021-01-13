using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaddleSdk
{
    public abstract class DataRead
    {
        public int[] Shape;
        public int[] Lod;
        public abstract int[] ToInt();
        public abstract float[] ToFloat();
    }
    public class DataRead<T> : DataRead
    {
        public List<T> Data;
        public DataRead(List<T> data, int[] sharp, int[] lod)
        {
            Shape = sharp;
            Data = data;
            Lod = lod;
        }
        public T GetSingleItem(params int[] indexs)
        {
            if (Shape.Length != indexs.Length)
            {
                throw new Exception("indexs error");
            }
            var index = 0;
            for (int i = 0; i < indexs.Length - 1; i++)
            {
                index += indexs[i] * Shape[i + 1];
            }
            index += indexs[indexs.Length - 1];
            return Data[index];
        }
        public List<T> GetSingleItemList(params int[] indexs)
        {
            if (Shape.Length != indexs.Length + 1)
            {
                throw new Exception("indexs error");
            }
            var index = 0;
            for (int i = 0; i < indexs.Length; i++)
            {
                index += indexs[i] * Shape[i + 1];
            }
            var lenght = Shape[indexs.Length];
            return Data.Skip(index).Take(lenght).ToList();
        }
        public override float[] ToFloat()
        {
            return Data.Select(t => Convert.ToSingle(t)).ToArray();
        }

        public override int[] ToInt()
        {
            return Data.Select(t => Convert.ToInt32(t)).ToArray();
        }
    }
}
