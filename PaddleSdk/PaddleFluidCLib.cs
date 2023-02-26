using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace PaddleSdk
{
    public class PaddleFluidCLib
    {
        #region Config
        /// <summary>
        /// 创建配置
        /// </summary>
        /// <returns></returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern IntPtr PD_ConfigCreate();
        /// <summary>
        /// 释放配置文件
        /// </summary>
        /// <returns></returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigDestroy(IntPtr config);
        /// <summary>
        /// 关闭gpu
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigDisableGpu(IntPtr config);
        /// <summary>
        /// gpu是否使用状态
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern bool PD_ConfigUseGpu(IntPtr config);
        /// <summary>
        /// 控制是否执行IR图优化。
        /// </summary>
        /// <param name="config"></param>
        /// <param name="isOpen"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigSwitchIrOptim(IntPtr config, bool isOpen);
        /// <summary>
        /// 开启内存优化。
        /// </summary>
        /// <param name="config"></param>
        /// <param name="isOpen"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigEnableMemoryOptim(IntPtr config, bool isOpen);
        /// <summary>
        /// 打开TensorRT发动机。TensorRT引擎将加速原始Fluid中的一些子图,计算图形。在一些型号中，如resnet50、GoogleNet等，它获得了显著的性能加速。
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="workspace_size">用于TensorRT的内存大小（字节）工作区</param>
        /// <param name="max_batch_size">此预测任务的最大批大小，设置越小越好，性能损失越小。</param>
        /// <param name="min_subgraph_size">所需的最小TensorRT子图大小，如果子图小于此值，它将不会传输到TensorRT,发动机。</param>
        /// <param name="precision">TensorRT中使用的精度</param>
        /// <param name="use_static">将优化信息序列化到的磁盘重复使用。</param>
        /// <param name="use_calib_mode">用TRT int8校准（培训后量化）</param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigEnableTensorRtEngine(nint config, int workspace_size,
    int max_batch_size, int min_subgraph_size,
    PD_PrecisionType precision, bool use_static, bool use_calib_mode);
        /// <summary>
        /// 开启gpu
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigEnableUseGpu(IntPtr config, ulong memoryPoolInitSizeMb, ushort deviceId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigEnableGpuMultiStream(IntPtr config);
        /// <summary>
        /// 关闭日记输出
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigDisableGlogInfo(IntPtr config);
        /// <summary>
        /// 开启cudnn
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigEnableCudnn(IntPtr config);
        /// <summary>
        /// 开启mkldnn
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigEnableMKLDNN(IntPtr config);
        /// <summary>
        /// 获得当前线程数
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern int PD_ConfigGetCpuMathLibraryNumThreads(IntPtr config);
        /// <summary>
        /// 设置当前线程数
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigSetCpuMathLibraryNumThreads(IntPtr config, int num);
        /// <summary>
        /// 开启cudnn
        /// </summary>
        /// <param name="config"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern bool PD_ConfigCudnnEnabled(IntPtr config);
        /// <summary>
        /// 查询当前Mkldnn是否开启
        /// </summary>
        /// <param name="config"></param>
        /// <param name="open"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern bool PD_ConfigMkldnnEnabled(IntPtr config);
        /// <summary>
        /// 设置模型
        /// </summary>
        /// <param name="config"></param>
        /// <param name="modelPath"></param>
        /// <param name="paramsPath"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_ConfigSetModel(IntPtr config, string modelPath, string paramsPath);
        /// <summary>
        /// 创建预测器
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern IntPtr PD_PredictorCreate(IntPtr config);
        #endregion
        #region
        /// <summary>
        /// 删除预测器
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_DeletePredictor(IntPtr predictor);
        /// <summary>
        /// 获得预测器输入参数个数
        /// </summary>
        /// <param name="predictor"></param>
        /// <returns></returns>

        [DllImport("paddle_inference_c.dll")]
        public static extern int PD_PredictorGetInputNum(IntPtr predictor);
        /// <summary>
        /// 获得预测器输出参数个数
        /// </summary>
        /// <param name="predictor"></param>
        /// <returns></returns>

        [DllImport("paddle_inference_c.dll")]
        public static extern int PD_PredictorGetOutputNum(IntPtr predictor);
        /// <summary>
        /// 获得输入参数名称
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern IntPtr PD_PredictorGetInputNames(IntPtr predictor);
        /// <summary>
        /// 获得输出参数名称
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern IntPtr PD_PredictorGetOutputNames(IntPtr predictor);
        /// <summary>
        /// 设置输入参数
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern nint PD_PredictorGetInputHandle(nint predictor, string name);
        /// <summary>
        /// 获取推理输出 Tensor 信息
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern nint PD_TensorGetShape(nint tensor);
        /// <summary>
        /// 释放Tensor
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_TensorDestroy(nint tensor);
        /// <summary>
        /// 执行预测
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern bool PD_PredictorRun(nint predictor);
        /// <summary>
        /// 设置输入参数形状
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_TensorReshape(nint tensor,
                                                int shape_size,
                                                int[] shape);
        /// <summary>
        /// 复制预测数据
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_TensorCopyFromCpuFloat(
     nint tensor, float[] data);
        /// <summary>
        /// 复制预测结果数据
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_TensorCopyToCpuFloat(
     nint tensor, float[] data);
        /// <summary>
        /// 复制预测结果数据
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_TensorCopyToCpuInt32(
     nint tensor, int[] data);
        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns> PD_DATA_FLOAT32, PD_DATA_INT32, PD_DATA_INT64,    PD_DATA_UINT8,   PD_DATA_INT8</returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern PD_DataType PD_TensorGetDataType(nint tensor);
        /// <summary>
        /// 复制预测结果数据
        /// </summary>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern void PD_TensorCopyToCpuInt64(
     nint tensor, long[] data);
        /// <summary>
        /// 获得输出参数
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="zeroCopyTensor"></param>
        [DllImport("paddle_inference_c.dll")]
        public static extern nint PD_PredictorGetOutputHandle(nint predictor, string name);
        #endregion
        #region
        /// <summary>
        /// 创建预测器
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [DllImport("paddle_inference_c.dll")]
        public static extern IntPtr PD_DeleteZeroCopyTensor(IntPtr config);
        #endregion
        [StructLayout(LayoutKind.Sequential)]
        public struct PD_OneDimArrayCstr : IDisposable
        {
            public uint Size;
            public IntPtr Data;
            public void Dispose()
            {
                try
                {
                    //Marshal.FreeHGlobal(Data);
                }
                catch
                {
                }
            }
            public string[] ReadData()
            {
                string[] datas = new string[Size];
                for (int i = 0; i < Size; i++)
                {
                    datas[i] = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(Data + i * 8));
                }
                return datas;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PD_Buffer : IDisposable
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
        public enum PD_DataType
        {
            PD_DATA_UNK = -1, PD_DATA_FLOAT32, PD_DATA_INT32,
            PD_DATA_INT64, PD_DATA_UINT8, PD_DATA_INT8,
        }
        public enum PD_PrecisionType
        {
            PD_PRECISION_FLOAT32 = 0, 
            PD_PRECISION_INT8,
            PD_PRECISION_HALF
        }
    }
}
