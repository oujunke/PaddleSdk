# PaddleSdk
## 使用Paddle的C导出dll,可自行编译Paddle官方预测库源码
### Demo的模型改至PaddleOCR自己训练的识别模型.
#### 当前版本DLL库导出版本所有导出方法有
```
PD_CpuMathLibraryNumThreads
PD_CudnnEnabled
PD_DeleteAnalysisConfig
PD_DeletePaddleBuf
PD_DeletePaddleTensor
PD_DeletePass
PD_DeletePredictor
PD_DeleteZeroCopyTensor
PD_DestroyZeroCopyTensor
PD_DisableGlogInfo
PD_DisableGpu
PD_EnableCUDNN
PD_EnableMKLDNN
PD_EnableMemoryOptim
PD_EnableMkldnnBfloat16
PD_EnableMkldnnQuantizer
PD_EnableProfile
PD_EnableTensorRtEngine
PD_EnableUseGpu
PD_FractionOfGpuMemoryForPool
PD_GetInputName
PD_GetInputNum
PD_GetOutputName
PD_GetOutputNum
PD_GetPaddleTensorDType
PD_GetPaddleTensorData
PD_GetPaddleTensorName
PD_GetPaddleTensorShape
PD_GetZeroCopyOutput
PD_GpuDeviceId
PD_InitZeroCopyTensor
PD_IrOptim
PD_IsValid
PD_MemoryOptimEnabled
PD_MemoryPoolInitSizeMb
PD_MkldnnBfloat16Enabled
PD_MkldnnEnabled
PD_MkldnnQuantizerEnabled
PD_ModelDir
PD_ModelFromMemory
PD_NewAnalysisConfig
PD_NewPaddleBuf
PD_NewPaddleTensor
PD_NewPredictor
PD_NewZeroCopyTensor
PD_PaddleBufData
PD_PaddleBufEmpty
PD_PaddleBufLength
PD_PaddleBufReset
PD_PaddleBufResize
PD_ParamsFile
PD_PredictorRun
PD_PredictorZeroCopyRun
PD_ProfileEnabled
PD_ProgFile
PD_SetCpuMathLibraryNumThreads
PD_SetInValid
PD_SetMkldnnCacheCapacity
PD_SetModel
PD_SetModelBuffer
PD_SetOptimCacheDir
PD_SetPaddleTensorDType
PD_SetPaddleTensorData
PD_SetPaddleTensorName
PD_SetPaddleTensorShape
PD_SetParamsFile
PD_SetProgFile
PD_SetZeroCopyInput
PD_SpecifyInputName
PD_SwitchIrDebug
PD_SwitchIrOptim
PD_SwitchSpecifyInputNames
PD_SwitchUseFeedFetchOps
PD_TensorrtEngineEnabled
PD_UseFeedFetchOpsEnabled
PD_UseGpu
PD_ZeroCopyRun
```
## 具体可参考paddle_c_api.h
