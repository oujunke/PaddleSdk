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
            _intPtr = PD_PredictorCreate(config.Handle);
            byte[,] b1 = new byte[2, 3];
            byte[] b2 = new byte[6];
        }
        /// <summary>
        /// 获得输入名称
        /// </summary>
        /// <returns></returns>
        public string[] GetInputNames()
        {
            var namePtr = PD_PredictorGetInputNames(Handle);
            var pD_OneDim = Marshal.PtrToStructure<PD_OneDimArrayCstr>(namePtr);
            return pD_OneDim.ReadData();
        }
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="data"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public DataRead[] Prediction(float[] data, int[] shape)
        {
            // 设置输入的名称
            var name = GetInputNames()[0];
            var t = new Tensor(Handle, name);
            t.SetInputData(data, shape);
            var res = PD_PredictorRun(Handle);
            if (res)
            {
                var outNames = GetOutNames();
                DataRead[] dataReads = new DataRead[outNames.Length];
                for (int i = 0; i < outNames.Length; i++)
                {
                    var outName = outNames[i];
                    var outputTensor = PD_PredictorGetOutputHandle(Handle, outName);
                    dataReads[i] = new Tensor(outName, outputTensor).GetDataRead();

                }
                return dataReads;
            }
            return null;
        }
        /// <summary>
        /// 获得输出名称
        /// </summary>
        /// <returns></returns>
        public string[] GetOutNames()
        {
            var namePtr = PD_PredictorGetOutputNames(Handle);
            var pD_OneDim = Marshal.PtrToStructure<PD_OneDimArrayCstr>(namePtr);
            var res = pD_OneDim.ReadData();
            return res;
        }
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataRead[] Prediction(float[] data)
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
