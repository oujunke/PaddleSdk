using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaddleSdk
{
    public class Config : IDisposable
    {
        internal IntPtr _intPtr;
        public IntPtr Handle { get => _intPtr; }
        public int[] InputShape;
        public Config()
        {
            _intPtr = PaddleFluidCLib.PD_ConfigCreate();
        }
        /// <summary>
        /// 关闭gpu
        /// </summary>
        public void DisableGpu()
        {
            PaddleFluidCLib.PD_ConfigDisableGpu(_intPtr);
        }
        /// <summary>
        /// 开启gpu
        /// </summary>
        public bool EnableUseGpu(ushort deviceId, ulong memoryPoolInitSizeMb)
        {
            //PaddleFluidCLib.PD_ConfigEnableGpuMultiStream(_intPtr);
            //var res=PaddleFluidCLib.PD_ConfigUseGpu(_intPtr);
            PaddleFluidCLib.PD_ConfigEnableUseGpu(_intPtr, memoryPoolInitSizeMb, deviceId);
            PaddleFluidCLib.PD_ConfigSwitchIrOptim(_intPtr, true);
            PaddleFluidCLib.PD_ConfigEnableMemoryOptim(_intPtr, true);
            //PaddleFluidCLib.PD_ConfigEnableTensorRtEngine(_intPtr, 1<<30, 100, 3, PaddleFluidCLib.PD_PrecisionType.PD_PRECISION_FLOAT32, true, true);
            var res= PaddleFluidCLib.PD_ConfigUseGpu(_intPtr);
            return res;
        }
        /// <summary>
        /// 关闭日记输出
        /// </summary>
        public void DisableGlogInfo()
        {
            PaddleFluidCLib.PD_ConfigDisableGlogInfo(_intPtr);
        }
        /// <summary>
        /// 设置模型
        /// </summary>
        public void SetModel(string modelPath, string paramsPath)
        {
            PaddleFluidCLib.PD_ConfigSetModel(_intPtr, modelPath, paramsPath);
        }
        /// <summary>
        /// 获得默认配置
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="paramsPath"></param>
        /// <returns></returns>
        public static Config GetDefault(string modelPath, string paramsPath, int[] shape = null)
        {
            Config config = new Config { InputShape = shape };
            config.EnableUseGpu(0, 1024);
            config.SetModel(modelPath, paramsPath);
            return config;
        }
        public Paddle GetPaddle()
        {
            return new Paddle(this);
        }
        public void Dispose()
        {
            PaddleFluidCLib.PD_ConfigDestroy(_intPtr);
        }
    }
}
