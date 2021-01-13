using PaddleSdk;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using static PaddleSdk.PaddleFluidCLib;
using System.Linq;
namespace PaddleSdkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var paddle = Config.GetDefault("Model/model", "Model/params").GetPaddle();
            var results = Prediction(paddle, Image.FromFile("99.jpg") as Bitmap);
            string labelString = "0123456789abcdefghijklmnopqrstuvwxyz";
            var recognitionResult = results[0].ToInt().Select(n => labelString[n]).Aggregate<char, string>((c1, c2) => $"{c1}{c2}");
            Console.WriteLine($"识别结果为:{recognitionResult}");
            var probs = results[1] as DataRead<float>;
            List<int> ind = new List<int>();
            for (int i = 0; i < probs.Shape[0]; i++)
            {
                var temp = probs.GetSingleItemList(i);
                ind.Add(temp.IndexOf(temp.Max()));
            }
            var blank = probs.Shape[1] - 1;
            var score = 0.0;
            var count = 0;
            for (int i = 0; i < ind.Count; i++)
            {
                var num = ind[i];
                if (num == blank)
                {
                    continue;
                }
                score += probs.GetSingleItem(i,num);
                count++;
            }
            score /= count;
            Console.WriteLine($"识别结果评分为:{score}");
            Console.Read();
        }
        static DataRead[] Prediction(Paddle paddle, Bitmap bitmap)
        {
            PD_ZeroCopyTensor input = new PD_ZeroCopyTensor();
            // 设置输入的名称
            input.Name = PD_GetInputName(paddle.Handle, 0);
            // 设置输入的数据大小
            var shape = new[] { 1, 3, 32, 320 };
            input.Data.Capacity = (ulong)(sizeof(float) * shape[0] * shape[1] * shape[2] * shape[3]);
            input.Data.Length = input.Data.Capacity;
            GCHandle gcHandle = default;
            GCHandle gcShapeHandle = default;
            try
            {
                var ratio = bitmap.Width * 1.0 / bitmap.Height;
                var resized_w = 0;
                var imgH = shape[2];
                var imgW = shape[3];
                if (Math.Ceiling(imgH * ratio) > imgW)
                {
                    resized_w = imgW;
                }
                else
                {
                    resized_w = (int)Math.Ceiling(imgH * ratio);
                }
                var newBit = Utils.Compression(bitmap, new Size(resized_w, imgH));
                var wheel = shape[2] * shape[3];
                var fdata = new float[shape[0] * shape[1] * shape[2] * shape[3]];
                for (int h = 0; h < newBit.Height; h++)
                {
                    for (int w = 0; w < newBit.Width; w++)
                    {
                        var c = newBit.GetPixel(w, h);
                        var index = h * 320 + w;
                        fdata[index] = (float)((c.R / 255.0 - 0.5) / 0.5);
                        fdata[wheel + index] = (float)((c.G / 255.0 - 0.5) / 0.5);
                        fdata[2 * wheel + index] = (float)((c.B / 255.0 - 0.5) / 0.5);
                    }
                }
                List<byte> bsList = new List<byte>((int)input.Data.Length);
                foreach (var f in fdata)
                {
                    bsList.AddRange(BitConverter.GetBytes(f));
                }
                var data = bsList.ToArray();
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
                return paddle.Prediction(input);
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
    }
}
