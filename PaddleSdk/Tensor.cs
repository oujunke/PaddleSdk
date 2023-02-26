using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PaddleSdk.PaddleFluidCLib;

namespace PaddleSdk
{
    internal class Tensor : IDisposable
    {
        public nint Handle;
        public string Name;
        public int DataLength;
        public Tensor(nint paddle, string name)
        {
            Name = name;
            Handle = PaddleFluidCLib.PD_PredictorGetInputHandle(paddle, name);
        }
        public Tensor(string name, nint handle)
        {
            Name = name;
            Handle = handle;
        }

        public void Dispose()
        {
            PaddleFluidCLib.PD_TensorDestroy(Handle);
        }

        public void SetInputData(float[] data, int[] shape)
        {
            PaddleFluidCLib.PD_TensorReshape(Handle, shape.Length, shape);
            PaddleFluidCLib.PD_TensorCopyFromCpuFloat(Handle, data);
        }
        public int[] GetShape()
        {
            var tshape = PD_TensorGetShape(Handle);
            var count = Marshal.ReadInt32(tshape);
            var outShape = new int[count];
            DataLength = 1;
            for (int j = 0; j < count; j++)
            {
                outShape[j] = Marshal.ReadInt32(Marshal.ReadIntPtr(tshape + 8) + 4 * j);
                DataLength *= outShape[j];
            }
            return outShape;
        }
        public DataRead GetDataRead()
        {
            var outShape = GetShape();
            var type = PD_TensorGetDataType(Handle);
            DataRead read = null;
            switch (type)
            {
                case PD_DataType.PD_DATA_INT64:
                    var ls = new long[DataLength];
                    PD_TensorCopyToCpuInt64(Handle, ls);
                    read = new DataRead<long>(ls.ToList(), outShape);
                    break;
                case PD_DataType.PD_DATA_FLOAT32:
                    var fs = new float[DataLength];
                    PD_TensorCopyToCpuFloat(Handle, fs);
                    read = new DataRead<float>(fs.ToList(), outShape);
                    break;
            }
            return read;
        }
    }
}
