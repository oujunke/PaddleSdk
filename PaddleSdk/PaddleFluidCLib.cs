using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace PaddleSdk
{
    public class PaddleFluidCLib
    {
        #region Config
        /// <summary>
        /// 创建配置
        /// </summary>
        /// <returns></returns>
        [DllImport("paddle_fluid_c.dll")]
        public static extern IntPtr PD_NewAnalysisConfig(); 
        /// <summary>
        /// 释放配置文件
        /// </summary>
        /// <returns></returns>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_DeleteAnalysisConfig(IntPtr comfig);
        /// <summary>
        /// 关闭gpu
        /// </summary>
        /// <param name="comfig"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_DisableGpu(IntPtr comfig);
        /// <summary>
        /// 关闭日记输出
        /// </summary>
        /// <param name="comfig"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_DisableGlogInfo(IntPtr comfig);
        /// <summary>
        /// 开启cudnn
        /// </summary>
        /// <param name="comfig"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_EnableCUDNN(IntPtr comfig);
        /// <summary>
        /// 开启mkldnn
        /// </summary>
        /// <param name="comfig"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_EnableMKLDNN(IntPtr comfig);
        /// <summary>
        /// 获得当前线程数
        /// </summary>
        /// <param name="comfig"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern int PD_CpuMathLibraryNumThreads(IntPtr comfig); 
        /// <summary>
        /// 设置当前线程数
        /// </summary>
        /// <param name="comfig"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern int PD_SetCpuMathLibraryNumThreads(IntPtr comfig,int num);
        /// <summary>
        /// 开启cudnn
        /// </summary>
        /// <param name="comfig"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern bool PD_CudnnEnabled(IntPtr comfig);
        /// <summary>
        /// 是否切换指定输入名称
        /// </summary>
        /// <param name="comfig"></param>
        /// <param name="open"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_SwitchSpecifyInputNames(IntPtr comfig, bool open);
        /// <summary>
        /// 是否切换使用Feed获取操作
        /// </summary>
        /// <param name="comfig"></param>
        /// <param name="open"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_SwitchUseFeedFetchOps(IntPtr comfig, bool open);
        /// <summary>
        /// 设置模型
        /// </summary>
        /// <param name="comfig"></param>
        /// <param name="modelPath"></param>
        /// <param name="paramsPath"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_SetModel(IntPtr comfig, string modelPath, string paramsPath);
        /// <summary>
        /// 创建预测器
        /// </summary>
        /// <param name="comfig"></param>
        /// <returns></returns>
        [DllImport("paddle_fluid_c.dll")]
        public static extern IntPtr PD_NewPredictor(IntPtr comfig);
        #endregion
        #region
        /// <summary>
        /// 删除预测器
        /// </summary>
        /// <param name="comfig"></param>
        /// <returns></returns>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_DeletePredictor(IntPtr predictor);
        /// <summary>
        /// 获得预测器输入参数个数
        /// </summary>
        /// <param name="predictor"></param>
        /// <returns></returns>

        [DllImport("paddle_fluid_c.dll")]
        public static extern int PD_GetInputNum(IntPtr predictor);
        /// <summary>
        /// 获得预测器输出参数个数
        /// </summary>
        /// <param name="predictor"></param>
        /// <returns></returns>

        [DllImport("paddle_fluid_c.dll")]
        public static extern int PD_GetOutputNum(IntPtr predictor);
        /// <summary>
        /// 开始预测
        /// </summary>
        /// <param name="predictor"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_ZeroCopyRun(IntPtr predictor);
        /// <summary>
        /// 获得输入参数名称
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [DllImport("paddle_fluid_c.dll")]
        public static extern string PD_GetInputName(IntPtr predictor, int index);
        /// <summary>
        /// 获得输出参数名称
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [DllImport("paddle_fluid_c.dll")]
        public static extern string PD_GetOutputName(IntPtr predictor, int index);
        /// <summary>
        /// 设置输入参数
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_SetZeroCopyInput(IntPtr predictor, ref PD_ZeroCopyTensor zeroCopyTensor);
        /// <summary>
        /// 获得输出参数
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_fluid_c.dll")]
        public static extern void PD_GetZeroCopyOutput(IntPtr predictor, ref PD_ZeroCopyTensor zeroCopyTensor);
        #endregion
        #region
        /// <summary>
        /// 创建预测器
        /// </summary>
        /// <param name="comfig"></param>
        /// <returns></returns>
        [DllImport("paddle_fluid_c.dll")]
        public static extern IntPtr PD_DeleteZeroCopyTensor(IntPtr comfig);
        #endregion
        public enum PD_DataType
        {
            PD_FLOAT32, PD_INT32, PD_INT64, PD_UINT8, PD_UNKDTYPE
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PD_Buffer:IDisposable
        {
            public IntPtr Data;
            public ulong Length;
            public ulong Capacity;

            public void Dispose()
            {
                try
                {
                    Marshal.FreeHGlobal(Data);
                }
                catch
                {
                }
            }

            public byte[] ReadData()
            {
                var result = new byte[Capacity];
                Marshal.Copy(Data, result, 0, result.Length);
                return result;
            }
            public void WriteData(byte[] data)
            {
                Capacity = (ulong)data.Length;
                Length = Capacity;
                if (Data == IntPtr.Zero)
                {
                    Data = Marshal.AllocHGlobal(data.Length);
                }
                Marshal.Copy(data, 0, Data, data.Length);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PD_ZeroCopyTensor
        {
            public PD_Buffer Data;
            public PD_Buffer Shape;
            public PD_Buffer Lod;
            public PD_DataType Dtype;
            public string Name;            
        }
    }
}
