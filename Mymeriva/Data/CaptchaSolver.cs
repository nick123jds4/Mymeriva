using AForge.Imaging.Filters;
using CaptchaSolverLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mymeriva.Data
{
    class CaptchaSolver
    {
        Bitmap _bitMapCopy;

        public string Resolver(PictureBox pictureBox1, Label labelAnsCaptcha) {
            try
            {
                if (_bitMapCopy != null) _bitMapCopy.Dispose();
                 
                var pathToImg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ERRORS", "captcha.jpg");
                if (!File.Exists(pathToImg)) return null;

                #region save image to byte array

                byte[] arr;
                using (FileStream fs = new FileStream(pathToImg, FileMode.Open, FileAccess.Read))
                {
                    arr = new byte[fs.Length];
                    using (MemoryStream mem = new MemoryStream(arr))
                    {
                        fs.CopyTo(mem);
                    }
                }

                using (MemoryStream mem = new MemoryStream(arr))
                {
                    _bitMapCopy = new Bitmap(mem, false);
                }

                pictureBox1.Image = _bitMapCopy;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

                #endregion


                using (Bitmap image = new Bitmap(pathToImg))
                {
                    //++++ 
                    ResizeNearestNeighbor filter = new ResizeNearestNeighbor(image.Width * 2, image.Height * 2);
                    using (Bitmap clone = image.Clone(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                    {
                        using (Bitmap afterFilter = filter.Apply(clone))
                        {

                            string text = MethodsCaptchaSolver.OCR(image);
                            labelAnsCaptcha.Text = text;
                            return text;
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
             
        }
    }



}
