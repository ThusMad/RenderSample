using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalpio.Render.Data
{
    public struct Rectangle4F
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }

        public Rectangle4F(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
