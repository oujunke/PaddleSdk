using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaddleOcr
{
    public class OcrResult
    {
        public UMat OldMat;
        public UMat RecMat;
        public List<OcrModel> Ocrs;
        public class OcrModel
        {
            public Rectangle Rec;
            public UMat RecMat;
            public List<int> Indexs;
            public List<char> Chars;
            public List<float> Scores;
            public string Text;
            public float Score;
        }
    }
}
