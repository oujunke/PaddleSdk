#define Save
using Clipper2Lib;
using Emgu.CV;
using Emgu.CV.Rapid;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using PaddleSdk;
using System;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaddleOcr
{
    public class PaddleOcrServer
    {
        public string Dir;
        public Paddle Det;
        public Paddle Rec;
        public Size DetMaxSize;
        public Size RecMaxSize;
        public List<char> Chars;
        public PaddleOcrServer(string dir)
        {
            Dir = dir;
            Det = GetPaddle("det_infer");
            DetMaxSize = new Size(960, 960);
            Rec = GetPaddle("rec_infer");
            RecMaxSize = new Size(320, 48);
            Chars = File.ReadLines($"{Dir}/ppocr_keys_v1.txt").Select(s => s[0]).ToList();
        }
        public Paddle GetPaddle(string dirName)
        {
            var config = Config.GetDefault($"{Dir}/{dirName}/inference.pdmodel", $"{Dir}/{dirName}/inference.pdiparams", null);
            //config.DisableGlogInfo();
            return config.GetPaddle();
        }
        public void Ocr(string path)
        {
            UMat mat = new UMat(path, Emgu.CV.CvEnum.ImreadModes.Color);
            Ocr(mat);
        }
        public void Recognizer(string path)
        {
            UMat mat = new UMat(path, Emgu.CV.CvEnum.ImreadModes.Color);
            var data = GetRecData(mat);
            Recognizer(data);
        }
        public List<(Rectangle, double)> Detector(UMat mat)
        {
            mat.ConvertTo(mat, Emgu.CV.CvEnum.DepthType.Cv32F);
            var mat4 = new UMat();
            CvInvoke.Divide((mat * 0.003921569 - new MCvScalar(0.485, 0.456, 0.406)), new ScalarArray(new MCvScalar(0.229, 0.224, 0.225)), mat4);
            Console.WriteLine(mat4.Row(0).Col(0).Bytes[2]);
            var data = GetData(mat4);
            var drs = Det.Prediction(data, new int[] { 1, 3, mat4.Rows, mat4.Cols });
            UMat uMat = new UMat(mat4.Rows, mat4.Cols, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
            uMat.SetTo(drs[0].ToFloat());
            var uMat1 = UMatEvel(uMat);
            //var umat2 = new UMat();
            //CvInvoke.FindContours(uMat1, umat2,)
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat hier = new Mat();
            var fcUmat = uMat1.ToUMat();
            fcUmat.ConvertTo(fcUmat, Emgu.CV.CvEnum.DepthType.Cv8U);
            CvInvoke.FindContours(fcUmat, contours, hier, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            //CvInvoke.DrawContours(uMat1, contours, 0, new MCvScalar(255, 0, 0), 2);
            List<(Rectangle, double)> res = new List<(Rectangle, double)>();
            var numContours = Math.Min(contours.Size, 1000);
            var uImg = uMat1.ToUMat();
            for (int i = 0; i < numContours; i++)
            {
                var contour = contours[i];
                var (isSucc, score, rec) = GetMinBox(uImg, uMat, contour, true);
                if (!isSucc)
                {
                    continue;
                }
                res.Add((rec, score));
                if (res.Count == 168)
                {

                }
            }
            return res;
        }
        public List<(Rectangle, double)> BoxesFromUMat(UMat mat)
        {
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat hier = new Mat();

            CvInvoke.FindContours(mat, contours, hier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            List<(Rectangle, double)> res = new List<(Rectangle, double)>();
            var numContours = Math.Min(contours.Size, 1000);

            return res;
        }
        public void Ocr(UMat mat)
        {
            var (umat1, rec) = ResizeImage(mat);
            OcrResult result = new OcrResult { OldMat = mat, RecMat = umat1, Ocrs = new List<OcrResult.OcrModel>() };
            var res = Detector(umat1);
            var size = 6;
            var dw = mat.Cols * 1.0 / rec.Width;
            var dh = mat.Rows * 1.0 / rec.Height;
            for (int i = 0; i < res.Count; i += size)
            {
                var recs = res.Skip(i).Take(size).Select(r =>
                {
                    var x = (int)(Math.Round(r.Item1.X * dw));
                    var y = (int)(Math.Round(r.Item1.Y * dh));
                    var w = (int)(Math.Round(r.Item1.Right * dw)) - x;
                    var h = (int)(Math.Round(r.Item1.Bottom * dh)) - y;
                    return new Rectangle(x, y, w, h);
                }).ToList();
                var data = GetRecData(mat, recs);
                Recognizer(data, recs, mat, result.Ocrs);
            }
            result.Ocrs.LastOrDefault().RecMat.Save("t11.jpeg");
        }
        private int Num;
        public UMat GetRecData(UMat mat)
        {
            var ratio = mat.Cols * 1.0 / mat.Rows;
            var resizedW = (int)Math.Ceiling(RecMaxSize.Height * ratio);
            if (resizedW > RecMaxSize.Width)
            {
                resizedW = RecMaxSize.Width;
            }
            UMat mat1 = new UMat();
            CvInvoke.Resize(mat, mat1, new Size(resizedW, RecMaxSize.Height), 0, 0, Emgu.CV.CvEnum.Inter.Cubic);
            CvInvoke.CopyMakeBorder(mat1, mat1, 0, 0, 0, RecMaxSize.Width - resizedW, Emgu.CV.CvEnum.BorderType.Constant, new MCvScalar(127, 127, 127));
            mat1.ConvertTo(mat1, Emgu.CV.CvEnum.DepthType.Cv32F);
            mat1 /= 255;
            mat1 -= 0.5;
            mat1 /= 0.5;
            return mat1;
        }
        public List<UMat> GetRecData(UMat mat, IEnumerable<Rectangle> recs)
        {
            var maxWhRatio = recs.Select(r => r.Width * 1.0 / r.Height).Max();
            var imgW = RecMaxSize.Height * maxWhRatio;
            List<UMat> mats = new List<UMat>();
            foreach (var rec in recs)
            {
                var ratio = rec.Width * 1.0 / rec.Height;

                var resizedW = (int)Math.Ceiling(RecMaxSize.Height * ratio);
                if (resizedW > RecMaxSize.Width)
                {
                    resizedW = RecMaxSize.Width;
                }
                UMat mat1 = new UMat();
                CvInvoke.Resize(new UMat(mat, rec), mat1, new Size(resizedW, RecMaxSize.Height), 0, 0, Emgu.CV.CvEnum.Inter.Cubic);
                CvInvoke.CopyMakeBorder(mat1, mat1, 0, 0, 0, RecMaxSize.Width - resizedW, Emgu.CV.CvEnum.BorderType.Constant, new MCvScalar(127, 127, 127));
                mat1.ConvertTo(mat1, Emgu.CV.CvEnum.DepthType.Cv32F);
                mat1 /= 255;
                mat1 -= 0.5;
                mat1 /= 0.5;
                mats.Add(mat1);
            }
            return mats;
        }
        public void Recognizer(UMat mat)
        {
            var data = GetData(mat);
            var drs = Rec.Prediction(data.ToArray(), new int[] { 1, 3, mat.Rows, mat.Cols });
            var dr = drs[0];
            var floats = dr.ToFloat();

            List<int> ints = new List<int>();
            List<float> scores = new List<float>();
            int lastValue = 0;
            for (int j = 0; j < dr.Shape[1]; j++)
            {
                var start = j * dr.Shape[2];
                int maxIndex = 0;
                float maxScore = 0;
                for (int z = 0; z < dr.Shape[2]; z++)
                {
                    var index = start + z;
                    var score = floats[index];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxIndex = z;
                    }
                }

                if (maxIndex > 0 && maxIndex != lastValue && maxIndex <= Chars.Count)
                {
                    ints.Add(maxIndex - 1);
                    scores.Add(maxScore);
                }
                lastValue = maxIndex;
            }
            if (ints.Count > 0)
            {
                var chars = ints.Select(i => Chars[i]).ToList();
                var text = ints.Select(i => Chars[i].ToString()).Aggregate((s1, s2) => s1 + s2);
                var score2 = scores.Sum() / scores.Count;
            }
        }
        public void Recognizer(List<UMat> mats, List<Rectangle> recs, UMat oldMat, List<OcrResult.OcrModel> ocrs)
        {
            List<float> data = new List<float>();
            foreach (var mat in mats)
            {
                data.AddRange(GetData(mat));
            }
            var drs = Rec.Prediction(data.ToArray(), new int[] { mats.Count, 3, mats[0].Rows, mats[0].Cols });
            var dr = drs[0];
            var floats = dr.ToFloat();
            for (int i = 0; i < mats.Count; i++)
            {
                List<int> ints = new List<int>();
                List<float> scores = new List<float>();
                int lastValue = 0;
                for (int j = 0; j < dr.Shape[1]; j++)
                {
                    var start = i * dr.Shape[1] * dr.Shape[2] + j * dr.Shape[2];
                    int maxIndex = 0;
                    float maxScore = 0;
                    for (int z = 0; z < dr.Shape[2]; z++)
                    {
                        var index = start + z;
                        var score = floats[index];
                        if (score > maxScore)
                        {
                            maxScore = score;
                            maxIndex = z;
                        }
                    }
                    if (maxIndex > 0 && maxIndex != lastValue && maxIndex <= Chars.Count)
                    {
                        ints.Add(maxIndex - 1);
                        scores.Add(maxScore);
                    }
                    lastValue = maxIndex;
                }
                if (ints.Count > 0)
                {
                    ocrs.Add(new OcrResult.OcrModel { Rec = recs[i], Indexs = ints, Scores = scores, RecMat = new UMat(oldMat, recs[i]), Chars = ints.Select(i => Chars[i]).ToList(), Text = ints.Select(i => Chars[i].ToString()).Aggregate((s1, s2) => s1 + s2), Score = scores.Sum() / scores.Count });
                }
            }
        }
        public (UMat, Size) ResizeImage(UMat mat)
        {
            var ratio = mat.Rows > DetMaxSize.Height || mat.Cols > DetMaxSize.Width ? (Math.Min(DetMaxSize.Width * 1.0 / mat.Cols, DetMaxSize.Height * 1.0 / mat.Rows)) : 1;
            var rh = (int)(mat.Rows * ratio);
            var rw = (int)(mat.Cols * ratio);
            rh = Math.Max((int)(Math.Round(rh * 1.0 / 32) * 32), 32);
            rw = Math.Max((int)(Math.Round(rw * 1.0 / 32) * 32), 32);
            UMat mat1 = new UMat(rh, rw, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            var size = new Size(rw, rh);
            CvInvoke.Resize(mat, mat1, size);
            return (mat1, size);
        }
        public (bool, double, Rectangle) GetMinBox(UMat mat, UMat old, IInputArray contour, bool isClipperOffset = false, bool isScore = true)
        {
            var boundingBox = CvInvoke.MinAreaRect(contour);
            return GetMinBox(mat, old, boundingBox, isClipperOffset, isScore);
        }
        public (bool, double, Rectangle) GetMinBox(UMat mat, UMat old, PointF[] ps, bool isClipperOffset = false, bool isScore = true)
        {
            var boundingBox = CvInvoke.MinAreaRect(ps);
            return GetMinBox(mat, old, boundingBox, isClipperOffset, isScore);
        }
        public (bool, double, Rectangle) GetMinBox(UMat mat, UMat old, RotatedRect boundingBox, bool isClipperOffset = false, bool isScore = true)
        {
            var sside = Math.Min(boundingBox.Size.Width, boundingBox.Size.Height);
            if (sside < 3)
            {
                return default;
            }
            var points = CvInvoke.BoxPoints(boundingBox);
            var minX = Math.Min(Math.Max((int)points.Min(p => p.X), 0), mat.Cols - 1);
            var maxX = Math.Min(Math.Max((int)points.Max(p => p.X), 0), mat.Cols - 1);
            var minY = Math.Min(Math.Max((int)points.Min(p => p.Y), 0), mat.Cols - 1);
            var maxY = Math.Min(Math.Max((int)points.Max(p => p.Y), 0), mat.Cols - 1);
            var rec = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            var mask = new UMat(rec.Height, rec.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            VectorOfPoint pointVector = new VectorOfPoint(points.Select(p => new Point((int)(p.X - minX), (int)(p.Y - minY))).ToArray());
            CvInvoke.FillPoly(mask, pointVector, new MCvScalar(1));
            var scalar = CvInvoke.Mean(new UMat(old, rec), mask);
            var score = scalar.V0;
            if (isScore && score < 0.2)
            {
                return default;
            }
            if (isClipperOffset)
            {
                ClipperOffset clipperOffset = new ClipperOffset();
                clipperOffset.AddPath(new Path64(points.Select(po => new Point64(po.X * 1.0, po.Y * 1.0))), JoinType.Round, EndType.Polygon);
                var distance = GetContourArea(points);
                var point64s = clipperOffset.Execute(distance);
                var res = GetMinBox(mat, old, point64s[0].Select(po => new PointF(po.X, po.Y)).ToArray(), false, false);
                return (res.Item1, score, new Rectangle(res.Item3.X, res.Item3.Y, res.Item3.Width - 1, res.Item3.Height - 1));
            }
            return (true, score, rec);
        }
        public double GetContourArea(PointF[] box)
        {
            int pts_num = 4;
            double area = 0.0;
            double dist = 0.0;
            var distance = 1.0;
            for (int i = 0; i < pts_num; i++)
            {
                area += box[i].X * box[(i + 1) % pts_num].Y -
                        box[i].Y * box[(i + 1) % pts_num].X;
                dist += Math.Sqrt((box[i].X - box[(i + 1) % pts_num].X) *
                                  (box[i].X - box[(i + 1) % pts_num].X) +
                              (box[i].Y - box[(i + 1) % pts_num].Y) *
                                  (box[i].Y - box[(i + 1) % pts_num].Y));
            }
            area = Math.Abs(area / 2.0);

            distance = area * 1.5 / dist;
            return distance;
        }
        public float[] GetData(UMat mat)
        {
            var num = mat.Rows * mat.Cols * 3;
            var arr = mat.GetData() as float[,,];
            var ls = new float[num];
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        ls[z * mat.Cols * mat.Rows + i * mat.Cols + j] = arr[i, j, z];
                    }
                }
            }
            return ls;
        }
        public Image<Gray, byte> UMatEvel(UMat mat)
        {
            var arr = mat.GetData() as float[,];
            var bs = new byte[mat.Rows, mat.Cols, 1];
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    //bs[i, j, 0] = (byte)(arr[i, j] > 0.3 ? 1 : 0);
                    bs[i, j, 0] = (byte)(arr[i, j] > 0.3 ? 1 : 0);
                }
            }
            Image<Gray, byte> image = new Image<Gray, byte>(bs);
            return image;
        }
    }
}