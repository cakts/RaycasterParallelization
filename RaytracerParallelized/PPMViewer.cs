using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Raytracer
{
    public class PpmViewer : Form
    {
        private Bitmap bitmap;

        public PpmViewer(string filename)
        {
            Text = Path.GetFileName(filename);
            bitmap = LoadPpm(filename);
            ClientSize = new Size(bitmap.Width, bitmap.Height);
            Paint += (s, e) => e.Graphics.DrawImage(bitmap, 0, 0);
        }

        private Bitmap LoadPpm(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string format = reader.ReadLine();
                if (format != "P3") throw new Exception("Invalid P3 File");
                string line;
                while ((line = reader.ReadLine()).StartsWith("#")) { }

                string[] dims = line.Split(' ');
                int width = int.Parse(dims[0]);
                int height = int.Parse(dims[1]);
                int maxVal = int.Parse(reader.ReadLine());
                Bitmap bmp = new Bitmap(width, height);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        string[] rgb = reader.ReadLine().Split(' ');
                        int r = int.Parse(rgb[0]);
                        int g = int.Parse(rgb[1]);
                        int b = int.Parse(rgb[2]);
                        bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                return bmp;
            }
        }

        [STAThread]
        public static void Show(string filename)
        {
            Application.EnableVisualStyles();
            Application.Run(new PpmViewer(filename));
        }
    }
}