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
            _intPtr = PaddleFluidCLib.PD_NewAnalysisConfig();
        }
        /// <summary>
        /// 关闭gpu
        /// </summary>
        public void DisableGpu()
        {
            PaddleFluidCLib.PD_DisableGpu(_intPtr);
        }
        /// <summary>
        /// 关闭日记输出
        /// </summary>
        public void DisableGlogInfo()
        {
            PaddleFluidCLib.PD_DisableGlogInfo(_intPtr);
        }
        /// <summary>
        /// 是否切换指定输出
        /// </summary>
        public void SwitchSpecifyInputNames(bool x)
        {
            PaddleFluidCLib.PD_SwitchSpecifyInputNames(_intPtr, x);
        }
        /// <summary>
        /// 是否切换使用Feed获取操作
        /// </summary>
        public void SwitchUseFeedFetchOps(bool x)
        {
            PaddleFluidCLib.PD_SwitchUseFeedFetchOps(_intPtr, x);
        }
        /// <summary>
        /// 设置模型
        /// </summary>
        public void SetModel(string modelPath, string paramsPath)
        {
            PaddleFluidCLib.PD_SetModel(_intPtr, modelPath, paramsPath);
        }
        /// <summary>
        /// 获得默认配置
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="paramsPath"></param>
        /// <returns></returns>
        public static Config GetDefault(string modelPath, string paramsPath,int[] shape=null)
        {
            Config config = new Config { InputShape=shape };
            config.DisableGpu();
            config.SwitchSpecifyInputNames(true);
            config.SwitchUseFeedFetchOps(false);
            config.SetModel(modelPath, paramsPath);
            return config;
        }
        public Paddle GetPaddle()
        {
            return new Paddle(this);
        }
        public void Dispose()
        {
            PaddleFluidCLib.PD_DeleteAnalysisConfig(_intPtr);
        }
        // PaddleFluidCLib.PD_SwitchSpecifyInputNames(config, true);
        // PaddleFluidCLib.PD_SwitchUseFeedFetchOps(config, false);
        //    PaddleFluidCLib.PD_SetModel(config, model_path, params_path);
    }
}
