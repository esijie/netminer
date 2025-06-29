using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using NetMiner.Resource;

namespace NetMiner.Common.Tool
{
    #region WatermarkPosition
    public enum WatermarkPosition
    {
        Absolute,
        TopLeft,
        TopRight,
        TopMiddle,
        BottomLeft,
        BottomRight,
        BottomMiddle,
        MiddleLeft,
        MiddleRight,
        Center
    }
    #endregion

    public class Watermark
    {
        private PixelFormat[] indexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare,
            PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed,
            PixelFormat.Format8bppIndexed
            };


        // <summary>
        /// 加图片水印
        /// </summary>
        /// <param name="img">要加水印的原图﻿(System.Drawing)</param>
        /// <param name="filename">文件名</param>
        /// <param name="watermarkFilename">水印文件名</param>
        /// <param name="watermarkStatus">图片水印位置1=左上 2=中上 3=右上 4=左中 5=中中 6=右中 7=左下 8=右中 9=右下</param>
        /// <param name="quality">加水印后的质量0~100,数字越大质量越高</param>
        /// <param name="watermarkTransparency">水印图片的透明度1~10,数字越小越透明,10为不透明</param>
        private  void ImageWaterMarkPic(Image img, string filename, string watermarkFilename, 
            int watermarkStatus, int quality, int watermarkTransparency)
        {
            Graphics g = Graphics.FromImage(img);
            //设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 www.keleyi.com
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkFilename);

            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
                return;

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
new float[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f},
new float[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f},
new float[] {0.0f, 0.0f, 0.0f, transparency, 0.0f},
new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(filename, ici, encoderParams);
            else
                img.Save(filename);

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 增加图片文字水印
        /// </summary>
        /// <param name="img">要加水印的原图﻿(﻿System.Drawing)</param>
        /// <param name="filename">文件名</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkStatus">图片水印位置1=左上 2=中上 3=右上 4=左中 5=中中 6=右中 7=左下 8=右中 9=右下</param>
        /// <param name="quality">加水印后的质量0~100,数字越大质量越高</param>
        /// <param name="fontname">水印的字体</param>
        /// <param name="fontsize">水印的字号</param>
        public Image ImageWaterMarkText(Image img, string filename, string watermarkText, 
            cGlobalParas.WatermarkPOS watermarkStatus, int quality, string fontname, int fontsize,
            bool isBold, bool isItalic,Color fontC)
        {
            Graphics g=null;

            if (IsPixelFormatIndexed(img.PixelFormat))
            {
                Bitmap bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format16bppRgb555);
                using (Graphics g1 = Graphics.FromImage(bmp))
                {
                    g1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g1.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g1.DrawImage(img, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                        new System.Drawing.Rectangle(0, 0, img.Width, img.Height),
                        System.Drawing.GraphicsUnit.Pixel);
                    
                }


                g = Graphics.FromImage(bmp);
                
               
            }
            else
               g = Graphics.FromImage(img);

            Font drawFont;
            if (isBold ==true && isItalic ==true )
                drawFont = new Font(fontname, fontsize, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
            else if (isBold ==true )
                drawFont = new Font(fontname, fontsize, FontStyle.Bold, GraphicsUnit.Pixel);
            else if (isItalic ==true )
                drawFont = new Font(fontname, fontsize,  FontStyle.Italic, GraphicsUnit.Pixel);
            else
                drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);

            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {
                case cGlobalParas.WatermarkPOS.LeftTop:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case cGlobalParas.WatermarkPOS.CenterTop:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case cGlobalParas.WatermarkPOS.RightTop:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case cGlobalParas.WatermarkPOS.LeftMiddle:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case cGlobalParas.WatermarkPOS.CenterMiddle:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case cGlobalParas.WatermarkPOS.RightMiddle:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case cGlobalParas.WatermarkPOS.LeftBottom:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case cGlobalParas.WatermarkPOS.CenterBottom:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case cGlobalParas.WatermarkPOS.RightBottom:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            //g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);文字阴影 www.keleyi.com
            g.DrawString(watermarkText, drawFont, new SolidBrush(fontC), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (filename != "")
            {
                if (ici != null)
                    img.Save(filename, ici, encoderParams);
                else
                    img.Save(filename);
            }

            g.Dispose();
            
            return img;
            //img.Dispose();
        }

        private bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }

            return false;
        }


         #region Private Fields
        private Image m_image;
        private Image m_originalImage;
        private Image m_watermark;
        private float m_opacity = 1.0f;
        private cGlobalParas.WatermarkPOS m_position = cGlobalParas.WatermarkPOS.LeftTop;
        private int m_x = 0;
        private int m_y = 0;
        private Color m_transparentColor = Color.Empty;
        private RotateFlipType m_rotateFlip = RotateFlipType.RotateNoneFlipNone;
        private Padding m_margin = new Padding(0);
        private Font m_font = new Font(FontFamily.GenericSansSerif, 10);
        private Color m_fontColor = Color.Black;
        private float m_scaleRatio = 1.0f;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the image with drawn watermarks
        /// </summary>
        [Browsable(false)]
        public Image Image { get { return m_image; } }

        /// <summary>
        /// Watermark position relative to the image sizes. 
        /// If Absolute is chosen, watermark positioning is being done via PositionX and PositionY 
        /// properties (0 by default)\n
        /// </summary>        
        public cGlobalParas.WatermarkPOS  Position { get { return m_position; } set { m_position = value; } }

        /// <summary>
        /// Watermark X coordinate (works if Position property is set to WatermarkPosition.Absolute)
        /// </summary>
        public int PositionX { get { return m_x; } set { m_x = value; } }

        /// <summary>
        /// Watermark Y coordinate (works if Position property is set to WatermarkPosition.Absolute)
        /// </summary>
        public int PositionY { get { return m_y; } set { m_y = value; } }

        /// <summary>
        /// Watermark opacity. Can have values from 0.0 to 1.0
        /// </summary>
        public float Opacity { get { return m_opacity; } set { m_opacity = value; } }

        /// <summary>
        /// Transparent color
        /// </summary>
        public Color TransparentColor { get { return m_transparentColor; } set { m_transparentColor = value; } }

        /// <summary>
        /// Watermark rotation and flipping
        /// </summary>
        public RotateFlipType RotateFlip { get { return m_rotateFlip; } set { m_rotateFlip = value; } }

        /// <summary>
        /// Spacing between watermark and image edges
        /// </summary>
        public Padding Margin { get { return m_margin; } set { m_margin = value; } }

        /// <summary>
        /// Watermark scaling ratio. Must be greater than 0. Only for image watermarks
        /// </summary>
        public float ScaleRatio { get { return m_scaleRatio; } set { m_scaleRatio = value; } }

        /// <summary>
        /// Font of the text to add
        /// </summary>
        public Font Font { get { return m_font; } set { m_font = value; } }

        /// <summary>
        /// Color of the text to add
        /// </summary>
        public Color FontColor { get { return m_fontColor; } set { m_fontColor = value; } }

        
        #endregion

        #region Constructors
        public Watermark(Image image) {
            LoadImage(image);
        }

        public Watermark(string filename)
        {
            LoadImage(Image.FromFile(filename));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets image, clearing all drawn watermarks
        /// </summary>
        public void ResetImage() {
            m_image = new Bitmap(m_originalImage);
        }

        public void DrawImage(string filename) {
            DrawImage(Image.FromFile(filename));
        }

        public void DrawImage(Image watermark) {

            if (watermark == null)
                throw new ArgumentOutOfRangeException("Watermark");

            if (m_opacity < 0 || m_opacity > 1)
                throw new ArgumentOutOfRangeException("Opacity");

            if (m_scaleRatio <= 0)
                throw new ArgumentOutOfRangeException("ScaleRatio");

            // Creates a new watermark with margins (if margins are not specified returns the original watermark)
            m_watermark = GetWatermarkImage(watermark);

            // Rotates and/or flips the watermark
            m_watermark.RotateFlip(m_rotateFlip);

            // Calculate watermark position
            Point waterPos = GetWatermarkPosition();

            // Watermark destination rectangle
            Rectangle destRect = new Rectangle(waterPos.X, waterPos.Y, m_watermark.Width, m_watermark.Height);

            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][] { 
                    new float[] { 1, 0f, 0f, 0f, 0f},
                    new float[] { 0f, 1, 0f, 0f, 0f},
                    new float[] { 0f, 0f, 1, 0f, 0f},
                    new float[] { 0f, 0f, 0f, m_opacity, 0f},
                    new float[] { 0f, 0f, 0f, 0f, 1}                    
                });

            ImageAttributes attributes = new ImageAttributes();

            // Set the opacity of the watermark
            attributes.SetColorMatrix(colorMatrix);

            // Set the transparent color 
            if (m_transparentColor != Color.Empty) {
                attributes.SetColorKey(m_transparentColor, m_transparentColor);
            }

            // Draw the watermark
            using (Graphics gr = Graphics.FromImage(m_image)) {
                gr.DrawImage(m_watermark, destRect, 0, 0, m_watermark.Width, m_watermark.Height, GraphicsUnit.Pixel, attributes);
            }
        }

        //释放原始文件
        public void ClearImage()
        {
            m_originalImage.Dispose();
            
        }

        public void DrawText(string text) {
            // Convert text to image, so we can use opacity etc.
            Image textWatermark = GetTextWatermark(text);

            DrawImage(textWatermark);
        }
        #endregion

        #region Private Methods
        private void LoadImage(Image image) {
            m_originalImage = image;
            ResetImage();
        }

        private Image GetTextWatermark(string text) {

            Brush brush = new SolidBrush(m_fontColor);
            SizeF size;

            // Figure out the size of the box to hold the watermarked text
            using (Graphics g = Graphics.FromImage(m_image)) {
                size = g.MeasureString(text, m_font);
            }

            // Create a new bitmap for the text, and, actually, draw the text
            Bitmap bitmap = new Bitmap((int)size.Width, (int)size.Height);
            bitmap.SetResolution(m_image.HorizontalResolution, m_image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bitmap)) {
                g.DrawString(text, m_font, brush, 0, 0);
            }

            return bitmap;
        }

        private Image GetWatermarkImage(Image watermark) {

            // If there are no margins specified and scale ration is 1, no need to create a new bitmap
            if (m_margin.All == 0 && m_scaleRatio == 1.0f)
                return watermark;
                        
            // Create a new bitmap with new sizes (size + margins) and draw the watermark
            int newWidth = Convert.ToInt32(watermark.Width * m_scaleRatio);
            int newHeight = Convert.ToInt32(watermark.Height * m_scaleRatio);

            Rectangle sourceRect = new Rectangle(m_margin.Left, m_margin.Top, newWidth, newHeight);
            Rectangle destRect = new Rectangle(0, 0, watermark.Width, watermark.Height);

            Bitmap bitmap = new Bitmap(newWidth + m_margin.Left + m_margin.Right, newHeight + m_margin.Top + m_margin.Bottom);
            bitmap.SetResolution(watermark.HorizontalResolution, watermark.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bitmap)) {
                g.DrawImage(watermark, sourceRect,destRect,GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        private Point GetWatermarkPosition() {
            int x = 0;
            int y = 0;

            switch (m_position) {
              
                case cGlobalParas.WatermarkPOS.LeftTop:
                    x = 0; y = 0;
                    break;
                case  cGlobalParas.WatermarkPOS.RightTop:
                    x = m_image.Width - m_watermark.Width; y = 0;
                    break;
                case  cGlobalParas.WatermarkPOS.CenterTop:
                    x = (m_image.Width - m_watermark.Width) / 2; y = 0;
                    break;
                case  cGlobalParas.WatermarkPOS.LeftBottom:
                    x = 0; y = m_image.Height - m_watermark.Height;
                    break;
                case  cGlobalParas.WatermarkPOS.RightBottom:
                    x = m_image.Width - m_watermark.Width; y = m_image.Height - m_watermark.Height;
                    break;
                case  cGlobalParas.WatermarkPOS.CenterBottom:
                    x = (m_image.Width - m_watermark.Width) / 2; y = m_image.Height - m_watermark.Height;
                    break;
                case  cGlobalParas.WatermarkPOS.LeftMiddle:
                    x = 0; y = (m_image.Height - m_watermark.Height) / 2;
                    break;
                case  cGlobalParas.WatermarkPOS.RightMiddle:
                    x = m_image.Width - m_watermark.Width; y = (m_image.Height - m_watermark.Height) / 2;
                    break;
                case  cGlobalParas.WatermarkPOS.CenterMiddle:
                    x = (m_image.Width - m_watermark.Width) / 2; y = (m_image.Height - m_watermark.Height) / 2;
                    break;
                default:
                    break;
            }

            return new Point(x, y);
        }
        #endregion


    }
}
