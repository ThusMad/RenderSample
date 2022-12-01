using Scalpio.Render.Data;
using System.Windows.Interop;
using Scalpio;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using System.Windows;

namespace Scalpio.Render.DirectX
{
    public class DXGraphics2DWrapper
    {
        public DXGraphics2D? Graphics2D { get; private set; }

        public DXGraphics2DWrapper()
        {
            Graphics2D = new DXGraphics2D();
        }

        public void Initialize(IntPtr target)
        {
            if (Graphics2D != null)
            {
                lock (Graphics2D)
                {
                    unsafe
                    {
                        Graphics2D.Init(target.ToPointer());
                    }
                }
            }
        }

        public void LoadFontFace(string fileName)
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            var fontPath = Path.Combine(strWorkPath, "Resources", "ext_fonts", fileName);

            if(!string.IsNullOrEmpty(fontPath))
            {
                if(File.Exists(fontPath))
                {
                    unsafe
                    {
                        if (Graphics2D != null)
                        {
                            lock (Graphics2D)
                            {
                                byte[] fontPathBytes = Encoding.ASCII.GetBytes(fontPath);

                                fixed (byte* fontPathBytesArray = fontPathBytes)
                                {
                                    sbyte* fontPathSBytesArray = (sbyte*)fontPathBytesArray;

                                    Graphics2D?.LoadFontFace(fontPathSBytesArray);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CreateTextFormat(string resourceName, string fontName, int fontWeight, int fontStyle, int fontStretch, float fontSize)
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    lock (Graphics2D)
                    {
                        byte[] fontNameBytes = Encoding.ASCII.GetBytes(fontName);
                        byte[] resourceNameBytes = Encoding.ASCII.GetBytes(resourceName);
                        
                        fixed (byte* fontNameBytesArray = fontNameBytes)
                        {
                            fixed (byte* resourceNameBytesArray = resourceNameBytes)
                            {
                                sbyte* fontNameSBytesArray = (sbyte*)fontNameBytesArray;
                                sbyte* resourceNameSBytesArray = (sbyte*)resourceNameBytesArray;

                                Graphics2D?.CreateTextFormat(resourceNameSBytesArray, fontNameSBytesArray, fontWeight, fontStyle, fontStretch, fontSize);
                            }
                        }
                    }
                }
            }
        }

        public void CreateSolidColorBrush(Color4F color, string brushName)
        {
            if (string.IsNullOrEmpty(brushName))
            {
                throw new NullReferenceException(nameof(brushName));
            }

            unsafe
            {
                if (Graphics2D != null)
                {
                    lock (Graphics2D)
                    {
                        byte[] brushNameBytes = Encoding.ASCII.GetBytes(brushName);

                        fixed (byte* brushNameBytesArray = brushNameBytes)
                        {
                            sbyte* brushNameSBytesArray = (sbyte*)brushNameBytesArray;

                            Graphics2D?.CreateSolidColorBrush(brushNameSBytesArray, color.R, color.G, color.B, color.A);
                        }
                    }
                }
            }
        }

        public void ClearScreen(Color4F color)
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    Graphics2D?.ClearScreen(color.R, color.G, color.B, color.A);
                }
            }
        }

        public void DrawLine(Point startPoint, Point endPoint, string brushName)
        {
            if (string.IsNullOrEmpty(brushName))
            {
                throw new NullReferenceException(nameof(brushName));
            }

            unsafe
            {
                if (Graphics2D != null)
                {
                    lock (Graphics2D)
                    {
                        byte[] brushNameBytes = Encoding.ASCII.GetBytes(brushName);

                        fixed (byte* brushNameBytesArray = brushNameBytes)
                        {
                            sbyte* brushNameSBytesArray = (sbyte*)brushNameBytesArray;

                            Graphics2D?.DrawLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, brushNameSBytesArray, 1.0F);
                        }
                    }
                }
            }
        }

        public void DrawText(string text, string format, string brush, Rectangle4F rect)
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    lock (Graphics2D)
                    {
                        byte[] textBytes = Encoding.ASCII.GetBytes(text);
                        byte[] formatBytes = Encoding.ASCII.GetBytes(format);
                        byte[] brushBytes = Encoding.ASCII.GetBytes(brush);

                        fixed (byte* textBytesArray = textBytes)
                        {
                            fixed (byte* formatBytesArray = formatBytes)
                            {
                                fixed (byte* brushBytesArray = brushBytes)
                                {
                                    sbyte* textSBytesArray = (sbyte*)textBytesArray;
                                    sbyte* formatSBytesArray = (sbyte*)formatBytesArray;
                                    sbyte* brushSBytesArray = (sbyte*)brushBytesArray;

                                    Graphics2D?.DrawText(textSBytesArray, text.Length, formatSBytesArray, rect.Left, rect.Top, rect.Right, rect.Bottom, brushSBytesArray);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawRectangle(Rectangle4F rect, string brushName)
        {
            if (string.IsNullOrEmpty(brushName))
            {
                throw new NullReferenceException(nameof(brushName));
            }

            unsafe
            {
                if (Graphics2D != null)
                {
                    lock (Graphics2D)
                    {
                        byte[] brushNameBytes = Encoding.ASCII.GetBytes(brushName);

                        fixed (byte* brushNameBytesArray = brushNameBytes)
                        {
                            sbyte* brushNameSBytesArray = (sbyte*)brushNameBytesArray;

                            Graphics2D?.DrawRectangle(rect.Left, rect.Top, rect.Right, rect.Bottom, brushNameSBytesArray, 1.0F);
                        }
                    }
                }
            }
        }

        public void FillRectangle(Rectangle4F rect, string brushName)
        {
            if (string.IsNullOrEmpty(brushName))
            {
                throw new NullReferenceException(nameof(brushName));
            }

            unsafe
            {
                if (Graphics2D != null)
                {
                    lock (Graphics2D)
                    {
                        byte[] brushNameBytes = Encoding.ASCII.GetBytes(brushName);

                        fixed (byte* brushNameBytesArray = brushNameBytes)
                        {
                            sbyte* brushNameSBytesArray = (sbyte*)brushNameBytesArray;

                            Graphics2D?.FillRectangle(rect.Left, rect.Top, rect.Right, rect.Bottom, brushNameSBytesArray);
                        }
                    }
                }
            }
        }

        public void Resize(int width, int height)
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    Graphics2D?.Resize(width, height);
                }
            }
        }

        public void BeginDraw()
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    Graphics2D?.BeginDraw();
                }
            }
        }

        public void EndDraw()
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    Graphics2D?.EndDraw();
                }
            }
        }

        public void Present()
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    Graphics2D?.Present();
                }
            }
        }

        public void Flush()
        {
            unsafe
            {
                if (Graphics2D != null)
                {
                    Graphics2D?.Flush();
                }
            }
        }
    }
}
