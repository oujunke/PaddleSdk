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
            //此模型要求输入3*32*320数据
            var config = Config.GetDefault("Model/model", "Model/params", new[] { 1, 3, 32, 320 });
            PD_EnableMKLDNN(config.Handle);
            config.DisableGlogInfo();
            var paddle = config.GetPaddle();
            var data = GetData(Image.FromFile("99.jpg") as Bitmap, 320, 32);
            var results = paddle.Prediction(data);
            // 根据训练时数据处理方式和模型定义(不要问我为什么要这样写)
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
                score += probs.GetSingleItem(i, num);
                count++;
            }
            score /= count;
            Console.WriteLine($"识别结果评分为:{score}");
            Console.Read();
        }
        /// <summary>
        /// 根据训练时数据处理方式定义(不要问我为什么要这样写)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        static byte[] GetData(Bitmap bitmap, int width, int height)
        {
            var ratio = bitmap.Width * 1.0 / bitmap.Height;
            var resized_w = 0;
            if (Math.Ceiling(height * ratio) > width)
            {
                resized_w = width;
            }
            else
            {
                resized_w = (int)Math.Ceiling(height * ratio);
            }
            var newBit = Utils.Compression(bitmap, new Size(resized_w, height));
            var wheel = width * height;
            var fdata = new float[3 * width * height];
            for (int h = 0; h < newBit.Height; h++)
            {
                for (int w = 0; w < newBit.Width; w++)
                {
                    var c = newBit.GetPixel(w, h);
                    var index = h * width + w;
                    fdata[index] = (float)((c.R / 255.0 - 0.5) / 0.5);
                    fdata[wheel + index] = (float)((c.G / 255.0 - 0.5) / 0.5);
                    fdata[2 * wheel + index] = (float)((c.B / 255.0 - 0.5) / 0.5);
                }
            }
            List<byte> bsList = new List<byte>(fdata.Length * sizeof(float));
            foreach (var f in fdata)
            {
                bsList.AddRange(BitConverter.GetBytes(f));
            }
            return bsList.ToArray();
        }
    }
}
