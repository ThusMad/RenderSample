using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalpio.Render.Data
{
    public struct Color4F
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color4F()
        {
            R = 0f;
            G = 0f;
            B = 0f;
            A = 1F;
        }

        public Color4F(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            A = 1F;
        }

        public Color4F(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color4F(float c)
        {
            R = c;
            G = c;
            B = c;
            A = 1.0F;
        }

        public Color4F(int r, int g, int b, int a)
        {
            R = r / 255.0F;
            G = g / 255.0F;
            B = b / 255.0F;
            A = a / 255.0F;
        }

        public Color4F(int c)
        {
            R = c / 255.0F;
            G = c / 255.0F;
            B = c / 255.0F;
            A = 1.0F;
        }

        public Color4F(string hex)
        {
            Color color = ColorTranslator.FromHtml(hex);
            R = Convert.ToInt16(color.R) / 255.0F;
            G = Convert.ToInt16(color.G) / 255.0F;
            B = Convert.ToInt16(color.B) / 255.0F;
            A = Convert.ToInt16(color.A) / 255.0F;
        }
    }
}
