namespace BS.Components.Data.DataProvider
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;

    public class ImageHelper
    {
        public static Bitmap BlackRegion(ref Bitmap bitMap, Color color, int x, int y, int width, int height)
        {
            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.FillRectangle(new SolidBrush(color), new Rectangle(x, y, width, height));
            graphics.Dispose();
            return bitMap;
        }

        public static Bitmap CutImage(Bitmap bitMap, int x, int y, int width, int height)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(image);
            graphics.DrawImage(bitMap, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
            graphics.Dispose();
            return image;
        }

        public static Bitmap DrawImage(string text, int width, int height, float fontSize, Color fontColor, string fontName, FontStyle fontStyle, Color bgColor)
        {
            Bitmap bitmap;
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile(fontName);
            FontFamily family = fonts.Families[0];
            Font font = new Font(family, fontSize, fontStyle, GraphicsUnit.Point);
            SolidBrush brush = new SolidBrush(fontColor);
        Label_002D:
            bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            if (bgColor == Color.Transparent)
            {
                bitmap.MakeTransparent();
            }
            Graphics graphics = Graphics.FromImage(bitmap);
            SizeF ef = graphics.MeasureString(text, font);
            int num = (int) Math.Ceiling((double) ef.Width);
            int num2 = (int) Math.Ceiling((double) ef.Height);
            if (num > width)
            {
                width = num;
                bitmap.Dispose();
                graphics.Dispose();
                goto Label_002D;
            }
            if (num2 > height)
            {
                height = num2;
                bitmap.Dispose();
                graphics.Dispose();
                goto Label_002D;
            }
            float y = ((float) (height - num2)) / 2f;
            if (bgColor != Color.Transparent)
            {
                graphics.Clear(bgColor);
            }
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawString(text, font, brush, 0f, y);
            return bitmap;
        }

        private static ImageFormat GetImageFormat(string ext)
        {
            switch (ext.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;

                case ".gif":
                    return ImageFormat.Gif;

                case ".bmp":
                    return ImageFormat.Bmp;

                case ".png":
                    return ImageFormat.Png;
            }
            return ImageFormat.Jpeg;
        }

        private static void GetPosition(ref float posX, ref float posY, int waterMakrPos, int phWidth, int phHeight, float tgWidth, float tgHeight)
        {
            int num = (int) (phWidth * 0.02);
            int num2 = (int) (phHeight * 0.02);
            switch (waterMakrPos)
            {
                case 1:
                    posX = num;
                    posY = num2;
                    break;

                case 2:
                    posX = (phWidth - tgWidth) / 2f;
                    posY = num2;
                    break;

                case 3:
                    posX = (phWidth - tgWidth) - num;
                    posY = num2;
                    break;

                case 4:
                    posX = num;
                    posY = (phHeight - tgHeight) / 2f;
                    break;

                case 5:
                    posX = (phWidth - tgWidth) / 2f;
                    posY = (phHeight - tgHeight) / 2f;
                    break;

                case 6:
                    posX = (phWidth - tgWidth) - num;
                    posY = (phHeight - tgHeight) / 2f;
                    break;

                case 7:
                    posX = num;
                    posY = (phHeight - tgHeight) - num2;
                    break;

                case 8:
                    posX = (phWidth - tgWidth) / 2f;
                    posY = (phHeight - tgHeight) - num2;
                    break;

                case 9:
                    posX = (phWidth - tgWidth) - num;
                    posY = (phHeight - tgHeight) - num2;
                    break;
            }
        }

        public static Bitmap GetThumbImage(Image imgPhoto, int maxWidth, int maxHeight, int thumbType)
        {
            int x = 0;
            int y = 0;
            int width = maxWidth;
            int height = maxHeight;
            int num5 = 0;
            int num6 = 0;
            int num7 = imgPhoto.Width;
            int num8 = imgPhoto.Height;
            double num9 = ((double) num7) / ((double) num8);
            double num10 = ((double) maxWidth) / ((double) maxHeight);
            switch (thumbType)
            {
                case 1:
                    if (num9 > num10)
                    {
                        maxHeight = (maxWidth * num8) / num7;
                        height = maxHeight;
                        break;
                    }
                    maxWidth = (maxHeight * num7) / num8;
                    width = maxWidth;
                    break;

                case 2:
                {
                    if (num9 > num10)
                    {
                        int num12 = (num8 * maxWidth) / maxHeight;
                        num5 = (num7 - num12) / 2;
                        num7 = num12;
                        break;
                    }
                    int num11 = (num7 * maxHeight) / maxWidth;
                    num6 = (num8 - num11) / 2;
                    num8 = num11;
                    break;
                }
                case 3:
                    if (num9 > num10)
                    {
                        height = (maxWidth * num8) / num7;
                        y = (maxHeight - height) / 2;
                        break;
                    }
                    width = (maxHeight * num7) / num8;
                    x = (maxWidth - width) / 2;
                    break;
            }
            if (((thumbType == 1) || (thumbType == 2)) && ((num7 < width) && (num8 < height)))
            {
                return (Bitmap) imgPhoto.Clone();
            }
            Bitmap image = new Bitmap(maxWidth, maxHeight);
            Graphics graphics = Graphics.FromImage(image);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Color.White);
            graphics.DrawImage(imgPhoto, new Rectangle(x, y, width, height), new Rectangle(num5, num6, num7, num8), GraphicsUnit.Pixel);
            graphics.Dispose();
            return image;
        }

        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            PixelFormat pixelFormat;
            int width = bmp.Width;
            int height = bmp.Height;
            if (bkColor == Color.Transparent)
            {
                pixelFormat = PixelFormat.Format32bppArgb;
            }
            else
            {
                pixelFormat = bmp.PixelFormat;
            }
            Bitmap image = new Bitmap(width, height, pixelFormat);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(bkColor);
            graphics.DrawImageUnscaled(bmp, 1, 1);
            graphics.Dispose();
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, (float) width, (float) height));
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            RectangleF bounds = path.GetBounds(matrix);
            Bitmap bitmap2 = new Bitmap((int) bounds.Width, (int) bounds.Height, pixelFormat);
            graphics = Graphics.FromImage(bitmap2);
            graphics.Clear(bkColor);
            graphics.TranslateTransform(-bounds.X, -bounds.Y);
            graphics.RotateTransform(angle);
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            graphics.Dispose();
            image.Dispose();
            return bitmap2;
        }

        public static void SaveImage(Image imgPhoto, string fileName, string fileExt)
        {
            imgPhoto.Save(fileName, GetImageFormat(fileExt));
        }

        public static void SaveImageWithPictureMark(Image imgPhoto, string fileName, string fileExt, string markPictureSrc, int markPosition, int markRate)
        {
            if (!File.Exists(markPictureSrc))
            {
                imgPhoto.Save(fileName, GetImageFormat(fileExt));
            }
            else
            {
                float posX = 5f;
                float posY = 5f;
                int width = imgPhoto.Width;
                int height = imgPhoto.Height;
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                bitmap.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.DrawImage(imgPhoto, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);
                Image image = new Bitmap(markPictureSrc);
                int srcWidth = image.Width;
                int srcHeight = image.Height;
                if ((width > srcWidth) && (height > srcHeight))
                {
                    GetPosition(ref posX, ref posY, markPosition, width, height, (float) srcWidth, (float) srcHeight);
                    ImageAttributes imageAttr = new ImageAttributes();
                    float num7 = ((float) markRate) / 100f;
                    float[][] numArray = new float[5][];
                    float[] numArray2 = new float[5];
                    numArray2[0] = 1f;
                    numArray[0] = numArray2;
                    numArray2 = new float[5];
                    numArray2[1] = 1f;
                    numArray[1] = numArray2;
                    numArray2 = new float[5];
                    numArray2[2] = 1f;
                    numArray[2] = numArray2;
                    numArray2 = new float[5];
                    numArray2[3] = num7;
                    numArray[3] = numArray2;
                    numArray2 = new float[5];
                    numArray2[4] = 1f;
                    numArray[4] = numArray2;
                    float[][] newColorMatrix = numArray;
                    ColorMatrix matrix = new ColorMatrix(newColorMatrix);
                    imageAttr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    graphics.DrawImage(image, new Rectangle((int) posX, (int) posY, srcWidth, srcHeight), 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel, imageAttr);
                }
                graphics.Flush();
                graphics.Dispose();
                bitmap.Save(fileName, GetImageFormat(fileExt));
                bitmap.Dispose();
            }
        }

        public static Bitmap SaveImageWithTextMark(Image imgPhoto, string fileExt, string markText, int markPosition)
        {
            float posX = 5f;
            float posY = 5f;
            int width = imgPhoto.Width;
            int height = imgPhoto.Height;
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            image.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawImage(imgPhoto, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);
            int[] numArray = new int[] { 0x10, 14, 12, 10, 8, 6, 4 };
            Font font = null;
            SizeF ef = new SizeF();
            for (int i = 0; i < 7; i++)
            {
                font = new Font("arial", numArray[i] * (72f / graphics.DpiX), FontStyle.Bold);
                ef = graphics.MeasureString(markText, font);
                if (((ushort) ef.Width) < ((ushort) width))
                {
                    break;
                }
            }
            GetPosition(ref posX, ref posY, markPosition, width, height, ef.Width, ef.Height);
            SolidBrush brush = new SolidBrush(Color.FromArgb(0x99, 0, 0, 0));
            graphics.DrawString(markText, font, brush, new PointF(posX + 1f, posY + 1f));
            SolidBrush brush2 = new SolidBrush(Color.FromArgb(0x99, 0xff, 0xff, 0xff));
            graphics.DrawString(markText, font, brush2, new PointF(posX, posY));
            graphics.Flush();
            graphics.Dispose();
            return image;
        }

        public static void SaveImageWithTextMark(Image imgPhoto, string fileName, string fileExt, string markText, int markPosition)
        {
            if ((markText == null) || (markText.Length == 0))
            {
                imgPhoto.Save(fileName, GetImageFormat(fileExt));
            }
            else
            {
                Bitmap bitmap = SaveImageWithTextMark(imgPhoto, fileExt, markText, markPosition);
                bitmap.Save(fileName, GetImageFormat(fileExt));
                bitmap.Dispose();
            }
        }

        public static byte[] SaveThumbImage(Image imgPhoto, int maxWidth, int maxHeight, int thumbType)
        {
            Bitmap bitmap = GetThumbImage(imgPhoto, maxWidth, maxHeight, thumbType);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, imgPhoto.RawFormat);
            byte[] buffer = stream.ToArray();
            bitmap.Dispose();
            stream.Close();
            return buffer;
        }

        public static void SaveThumbImage(Image imgPhoto, string fileName, string fileExt, int maxWidth, int maxHeight, int thumbType)
        {
            Image image = GetThumbImage(imgPhoto, maxWidth, maxHeight, thumbType);
            image.Save(fileName, GetImageFormat(fileExt));
            image.Dispose();
        }
    }
}

