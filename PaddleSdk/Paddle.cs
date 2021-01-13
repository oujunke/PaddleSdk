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
            _intPtr = PaddleFluidCLib.PD_NewPredictor(config.Handle);
            byte[,] b1 = new byte[2, 3];
            byte[] b2 = new byte[6];
        }
        public DataRead[] Prediction(params PaddleFluidCLib.PD_ZeroCopyTensor[] pD_ZeroCopyTensors)
        {
            for (int i = 0; i < pD_ZeroCopyTensors.Length; i++)
            {
                PaddleFluidCLib.PD_SetZeroCopyInput(_intPtr, ref pD_ZeroCopyTensors[i]);
            }
            PaddleFluidCLib.PD_ZeroCopyRun(_intPtr);
            var outNum = PaddleFluidCLib.PD_GetOutputNum(_intPtr);
            DataRead[] dataReads = new DataRead[outNum];
            for (int i = 0; i < outNum; i++)
            {
                // 获取预测输出
                PaddleFluidCLib.PD_ZeroCopyTensor output = new PaddleFluidCLib.PD_ZeroCopyTensor();
                output.Name = PaddleFluidCLib.PD_GetOutputName(_intPtr, i);
                // 获取 output 之后，可以通过该数据结构，读取到 data, shape 等信息
                PaddleFluidCLib.PD_GetZeroCopyOutput(_intPtr, ref output);
                dataReads[i]=output.GetValue() as DataRead;
            }
            return dataReads;
        }
        public void Dispose()
        {
            PaddleFluidCLib.PD_DeletePredictor(_intPtr);
        }
    }
}
