using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorAPI
{
    public class ColorPalette
    {
        private readonly List<Color> colors = new List<Color>();
        
        public int ByteLength { get => colors.Count * 4; }

        public static ColorPalette Load(byte[] ba)
        {
            if (ba.Length % 4 != 0)
            {
                throw new ArgumentException("length % 4 must be 0 (valid rgba color)");
            }
            ColorPalette cp = new ColorPalette();
            for (int i = 0; i < ba.Length; i += 4)
            {
                cp.Add(new Color(ba[i], ba[i + 1], ba[i + 2], ba[i + 3]));
            }
            return cp;
        }

        public void Add(Color color)
        {
            colors.Add(color);
        }

        public Color Get(int i)
        {
            return colors[i];
        }

        public byte[] Serialize()
        {
            byte[] buf = new byte[colors.Count * 4];
            for (int i = 0; i < colors.Count; i++)
            {
                int bs = i * 4;
                buf[bs] = colors[i].r;
                buf[bs + 1] = colors[i].g;
                buf[bs + 2] = colors[i].b;
                buf[bs + 3] = colors[i].a;
            }
            return buf;
        }
    }
}
