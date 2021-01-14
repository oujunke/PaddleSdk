using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PaddleSdk.PaddleFluidCLib;

namespace PaddleSdk
{
    public class Paddle : IDisposable
    {
        internal IntPtr _intPtr;
        internal Config _config;
        public IntPtr Handle { get => _intPtr; }
        public Paddle(Config config)
        {
            _config = config;
            _intPtr = PD_NewPredictor(config.Handle);
            byte[,] b1 = new byte[2, 3];
            byte[] b2 = new byte[6];
        }
        /// <summary>
        /// 获得输入名称
        /// </summary>
        /// <returns></returns>
        public string GetInputName(int index)
        {
            var namePtr = PD_GetInputName(Handle, index);
            return Marshal.PtrToStringAnsi(namePtr);
        }
        /// <summary>
        /// 获得输出名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetOutputName(int index)
        {
            var namePtr = PD_GetOutputName(Handle, index);
            return Marshal.PtrToStringAnsi(namePtr);
        }
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="pD_ZeroCopyTensors"></param>
        /// <returns></returns>
        public DataRead[] Prediction(params PD_ZeroCopyTensor[] pD_ZeroCopyTensors)
        {
            for (int i = 0; i < pD_ZeroCopyTensors.Length; i++)
            {
                PD_SetZeroCopyInput(_intPtr, ref pD_ZeroCopyTensors[i]);
            }
            PD_ZeroCopyRun(_intPtr);
            var outNum = PD_GetOutputNum(_intPtr);
            DataRead[] dataReads = new DataRead[outNum];
            for (int i = 0; i < outNum; i++)
            {
                // 获取预测输出
                PD_ZeroCopyTensor output = new PD_ZeroCopyTensor();
                output.Name = GetOutputName(i);
                // 获取 output 之后，可以通过该数据结构，读取到 data, shape 等信息
                PD_GetZeroCopyOutput(_intPtr, ref output);
                dataReads[i] = output.GetValue() as DataRead;
            }
            return dataReads;
        }
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="data"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public DataRead[] Prediction(byte[] data, int[] shape)
        {
            PD_ZeroCopyTensor input = new PD_ZeroCopyTensor();
           
            // 设置输入的名称
            input.Name = GetInputName(0);
            // 设置输入的数据大小
            input.Data.Capacity = (ulong)data.Length;
            input.Data.Length = input.Data.Capacity;
            GCHandle gcHandle = default;
            GCHandle gcShapeHandle = default;
            try
            {
                gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                input.Data.Data = gcHandle.AddrOfPinnedObject();
                var shapeLength = (sizeof(int) * shape.Length);
                List<byte> bsList2 = new List<byte>();
                foreach (var item in shape)
                {
                    bsList2.AddRange(BitConverter.GetBytes(item));
                }
                var shapeBs = bsList2.ToArray();
                gcShapeHandle = GCHandle.Alloc(shapeBs, GCHandleType.Pinned);
                input.Shape.Data = gcShapeHandle.AddrOfPinnedObject();
                input.Shape.Capacity = (uint)(sizeof(int) * shape.Length);
                input.Shape.Length = input.Shape.Capacity;
                // 设置输入数据的类型
                input.Dtype = PD_DataType.PD_FLOAT32;
                return Prediction(input);
            }
            finally
            {
                if (gcHandle.IsAllocated)
                {
                    gcHandle.Free();
                }
                if (gcShapeHandle.IsAllocated)
                {
                    gcShapeHandle.Free();
                }
            }
        }
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataRead[] Prediction(byte[] data)
        {
            if (_config.InputShape == null || _config.InputShape.Length == 0)
            {
                throw new Exception("Please set InputShape");
            }
            return Prediction(data, _config.InputShape);
        }
        public void Dispose()
        {
            PD_DeletePredictor(_intPtr);
        }
    }
}
