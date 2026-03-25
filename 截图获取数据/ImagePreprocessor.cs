using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 截图获取数据
{
    public class ImagePreprocessor
    {
        public static Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayBitmap = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    // 使用加权平均法计算灰度值
                    int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    grayBitmap.SetPixel(x, y, grayColor);
                }
            }
            return grayBitmap;
        }

        // 二值化处理
        public static Bitmap ApplyThreshold(Bitmap grayBitmap, int threshold = 128)
        {
            Bitmap binaryBitmap = new Bitmap(grayBitmap.Width, grayBitmap.Height);
            for (int y = 0; y < grayBitmap.Height; y++)
            {
                for (int x = 0; x < grayBitmap.Width; x++)
                {
                    Color pixelColor = grayBitmap.GetPixel(x, y);
                    // 判断灰度值是否低于阈值
                    Color newColor = pixelColor.R < threshold ? Color.Black : Color.White;
                    binaryBitmap.SetPixel(x, y, newColor);
                }
            }
            return binaryBitmap;
        }

        // 放大图片（例如，放大2倍）
        public static Bitmap ResizeImage(Bitmap original, double scale)
        {
            int newWidth = (int)(original.Width * scale);
            int newHeight = (int)(original.Height * scale);
            Bitmap resized = new Bitmap(original, newWidth, newHeight);
            return resized;
        }

        // 综合预处理流程
        public static Bitmap EnhanceImage(Bitmap input)
        {
            // 1. 灰度化
            Bitmap gray = ConvertToGrayscale(input);
            // 2. 二值化
            Bitmap binary = ApplyThreshold(gray, 128);
            // 3. 放大图片，帮助后续识别（可根据实际情况调整比例）
            Bitmap enhanced = ResizeImage(binary, 2.0);
            return enhanced;
        }

    }
}
