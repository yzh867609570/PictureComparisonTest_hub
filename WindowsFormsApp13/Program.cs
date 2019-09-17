using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp13
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //One();
            //Two();

            var bmp1 = new Bitmap(Image.FromFile(@"D:\Contrast\Contrast(85).jpg"), 256, 256);
            var bmp2 = new Bitmap(Image.FromFile(@"D:\Contrast\Contrast(86).jpg"), 256, 256);

            MessageBox.Show(GetResult(GetHisogram(bmp1), GetHisogram(bmp2)).ToString());

            //MessageBox.Show(ComparePic(bmp1, bmp2, 4, true, 0.8).ToString());

            //MessageBox.Show(dHash_getHammingdis(bmp1, bmp2).ToString());

            //MessageBox.Show(getHistogram_Bhattacharyya(bmp1, bmp2).ToString());

            //var similarPhoto1 = new SimilarPhoto(@"D:\Contrast\Contrast(27).jpg").GetHash();
            //var similarPhoto2 = new SimilarPhoto(@"D:\Contrast\Contrast(28).jpg").GetHash();
            //var compareHash = SimilarPhoto.CalcSimilarDegree(similarPhoto1, similarPhoto2);
            //MessageBox.Show(compareHash.ToString());
        }

        private static void One()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int countSame = 0;
            int countDifferent = 0;
            int sum = 0;

            Bitmap bitmapSource = new Bitmap(Image.FromFile(@"D:\Contrast\Contrast(27).jpg"), 256, 256);
            bitmapSource = GrayReverse(ToGray(bitmapSource));

            Bitmap bitmapTarget = new Bitmap(Image.FromFile(@"D:\Contrast\Contrast(29).jpg"), 256, 256);
            bitmapTarget = GrayReverse(ToGray(bitmapTarget));

            //照片尺寸必须一样
            //for (int i = 0; i < bitmapTarget.Width; i++)
            //{
            //    for (int j = 0; j < bitmapTarget.Height; j++)
            //    {
            //        try
            //        {
            //            if (bitmapSource.GetPixel(0, 0).Equals(bitmapTarget.GetPixel(i, j))
            //                && bitmapSource.GetPixel(1, 0).Equals(bitmapTarget.GetPixel(i + 1, j))
            //            )
            //            {
            //                sum++;
            //            }
            //        }
            //        catch (Exception)
            //        {
            //        }

            //        if (bitmapSource.GetPixel(i, j).Equals(bitmapTarget.GetPixel(i, j)))
            //        {
            //            countSame++;
            //        }
            //        else
            //        {
            //            countDifferent++;
            //        }
            //    }
            //}

            for (int i = 0; i < bitmapTarget.Width; i++)
            {
                for (int j = 0; j < bitmapTarget.Height; j++)
                {
                    if (bitmapSource.GetPixel(0, 0).Equals(bitmapTarget.GetPixel(i, j)))
                    {
                        int m = 0;
                        for (int k = i; k < bitmapTarget.Width; k++)
                        {
                            int n = 0;
                            for (int l = j; l < bitmapTarget.Height; l++)
                            {
                                if (bitmapSource.GetPixel(m, n++).Equals(bitmapTarget.GetPixel(k, l)))
                                {
                                    countSame++;
                                }
                                else
                                {
                                    countDifferent++;
                                }
                            }
                            m++;
                        }
                        goto one;
                    }
                }
            }
        one:
            stopwatch.Stop();
            MessageBox.Show("相同像素个数：" + countSame + "，不同像素个数：" + countDifferent + "用时：" + stopwatch.ElapsedMilliseconds + " 毫秒, 相似度: " + ((Convert.ToDouble(countSame) / Convert.ToDouble(countSame + countDifferent))).ToString() + " sum: " + sum);
        }

        static List<string> _paths = new List<string>();
        private static void Two()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string targetImageString = GetImageBase64String(@"D:\Contrast\Contrast(27).jpg");
            //待比较的图片文件路径集合
            ProcessDirectory(@"D:\Contrast");

            var same = 0;
            var different = 0;

            foreach (var filePath in _paths)
            {
                string sourceImageString = GetImageBase64String(filePath);

                if (targetImageString.Equals(sourceImageString))
                {
                    same++;
                    Console.WriteLine("same");
                    MessageBox.Show("相同：" + filePath);
                }
                else
                {
                    different++;
                    Console.WriteLine("different");
                }
            }

            stopwatch.Stop();
            Console.WriteLine("比较了 " + _paths.Count + " 张照片，总用时：" + stopwatch.ElapsedMilliseconds + " 毫秒");
            MessageBox.Show("比较了 " + _paths.Count + " 张照片，总用时：" + stopwatch.ElapsedMilliseconds + " 毫秒.相同：" + same + " 张，不同：" + different + "张");
        }
        // 根据传入的文件夹路径，递归查出它包含的所有文件和文件夹，并处理各个文件夹下包含的文件
        public static void ProcessDirectory(string targetDirectory)
        {
            // 处理 targetDirectory 路径下的文件列表
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }

            // 递归到 targetDirectory 路径下的子路径中
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory);
            }
        }
        // 这里添加如何处理找到的文件的逻辑
        public static void ProcessFile(string path)
        {
            if (!path.Contains(".db"))
                _paths.Add(path);
        }
        static string GetImageBase64String(string imagePath)
        {
            string imageString = string.Empty;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageString = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            return imageString;
        }

        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }
        /// <summary>
        /// 图像灰度反转
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayReverse(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    Color newColor = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }
        /// <summary>
        /// 图像二值化1：取图片的平均灰度作为阈值，低于该值的全都为0，高于该值的全都为255
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255,

255, 255);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }
        /// <summary>
        /// 图像二值化2
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp2(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite,PixelFormat.Format1bppIndexed);
            for (int y = 0; y < h; y++)
            {
                byte[] scan = new byte[(w + 7) / 8];
                for (int x = 0; x < w; x++)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
            }
            return bmp;
        }

        /// <summary>
        /// 两张图片的对比
        /// </summary>
        /// <param name="firstMap">第一张图片</param>
        /// <param name="secondMap">第二张图片</param>
        /// <param name=" LimitedValue ">色差值</param>
        /// <param name=" colorType ">色差方式，true表示单色，false表示总色差</param>
        /// <param name=" LimitedRatio ">限制比例值</param>
        /// <returns>是否相同</returns>
        public static bool ComparePic(Bitmap firstMap, Bitmap secondMap, int LimitedValue, bool colorType, double LimitedRatio)
        {
            bool result = false;
            int destWidth, destHeight;
            destWidth = firstMap.Width;
            destHeight = firstMap.Height;
            int tempR, tempG, tempB;
            Color color01, color02;
            int i, j, tempCount = 0, total = destHeight * destWidth, limitedValue3 = 3 * LimitedValue;
            for (i = 0; i < destWidth; i++)
            {
                for (j = 0; j < destHeight; j++)
                {
                    color01 = firstMap.GetPixel(i, j);
                    color02 = secondMap.GetPixel(i, j);
                    tempR = Math.Abs((int)color01.R - (int)color02.R);
                    tempG = Math.Abs((int)color01.G - (int)color02.G);
                    tempB = Math.Abs((int)color01.B - (int)color02.B);
                    if (colorType)
                    {
                        if (tempR > LimitedValue)
                        {
                            tempCount++;
                        }
                        else if (tempG > LimitedValue)
                        {
                            tempCount++;
                        }
                        else if (tempB > LimitedValue)
                        {
                            tempCount++;
                        }
                    }
                    else
                    {
                        if (tempR + tempG + tempB > limitedValue3)
                            tempCount++;
                    }
                }
            }
            double ratio = (double)tempCount / total;
            if (ratio > LimitedRatio)
                result = false;
            else
                result = true;
            return result;
        }

        public static int dHash_getHammingdis(Bitmap img1, Bitmap img2)
        {
            Bitmap bmp1 = new Bitmap((Bitmap)img1, 8, 9);
            Bitmap bmp2 = new Bitmap((Bitmap)img2, 8, 9);

            bmp1 = ToGray(bmp1);
            bmp2 = ToGray(bmp2);

            UInt64 Hashdis1 = 0, Hashdis2 = 0, mask = 1;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    try
                    {
                        int m = y * 8 + x;
                        Color c1 = bmp1.GetPixel(x, y);
                        Color c1_2 = bmp1.GetPixel(x + 1, y);
                        if (c1.R >= c1_2.R)
                        {
                            Hashdis1 |= mask << m;
                        }
                        Color c2 = bmp1.GetPixel(x, y);
                        Color c2_2 = bmp1.GetPixel(x + 1, y);

                        if (c2.R >= c2_2.R)
                        {
                            Hashdis2 |= mask << m;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            UInt64 hash = Hashdis1 ^ Hashdis2;
            int Hammingdis = 0;
            for (int i = 0; i < 64; i++)
            {
                if ((hash & (mask << i)) != 0)
                    Hammingdis++;
            }
            return Hammingdis;
        }

        public static Double getHistogram_Bhattacharyya(Bitmap img1, Bitmap img2)
        {
            int MinWidth = Math.Min(img1.Width, img2.Width);
            int MinHeight = Math.Min(img2.Height, img1.Height);
            Bitmap bmp1 = new Bitmap((Bitmap)img1, MinHeight, MinWidth);
            Bitmap bmp2 = new Bitmap((Bitmap)img2, MinHeight, MinWidth);
            bmp1 = ToGray(bmp1);
            bmp2 = ToGray(bmp2);

            Double[] His1 = getHistogram(bmp1);
            Double[] His2 = getHistogram(bmp2);
            //DB(p,q) = -ln(BC(p,q))
            //BC(p,q) = ∑√p(x)q(x)
            Double BC = 0;
            for (int i = 0; i < 256; i++)
            {
                BC += Math.Sqrt(His1[i] * His2[i]);
            }
            Double DB = -Math.Log10(BC);
            return BC;
        }
        private static Double[] getHistogram(Image img)
        {
            Bitmap bmp = (Bitmap)img;
            Double[] Histogram = new Double[256];
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color c = bmp.GetPixel(x, y);
                    Histogram[c.R]++;
                }
            }
            for (int i = 0; i < 256; i++)//直方图均衡化
            {
                Histogram[i] /= (img.Width * img.Height);
            }
            return Histogram;
        }

        public static Bitmap Resize(string imageFile, string newImageFile)
        {
            Image img = Image.FromFile(imageFile);

            Bitmap imgOutput = new Bitmap(img, 256, 256);

            imgOutput.Save(newImageFile, System.Drawing.Imaging.ImageFormat.Jpeg);

            imgOutput.Dispose();

            return (Bitmap)Image.FromFile(newImageFile);
        }
        public static int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean]++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);

            return histogram;
        }
        private static float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);

            if (result == 0)
                result = 1;

            return abs / result;
        }
        //最终计算结果
        public static float GetResult(int[] firstNum, int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
                return 0;
            else
            {
                float result = 0;
                int j = firstNum.Length;

                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);
                }

                return result / j;
            }
        }

    }
}
