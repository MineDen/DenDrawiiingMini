﻿using ColorAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing
{
    public static class GlobalSettings
    {
        private static readonly string[] directories =
        {
            "themes",
            "stamps"
        };

        public static string[] themes;
        public static string currentTheme = "default";
        public static ColorPalette themeColorPalette;
        public static void Load()
        {
            // create directories if they don't exist
            for (int i = 0; i < directories.Length; i++)
            {
                if (!Directory.Exists(directories[i]))
                    Directory.CreateDirectory(directories[i]);
            }
            themes = Directory.GetFiles("themes", "*.ddp");
            if (!File.Exists("themes\\default.ddp"))
            {
                ColorPalette cp = new ColorPalette();
                cp.Add(new Color(0xf8, 0xf8, 0xf8)); // workspace; background
                cp.Add(new Color(0xff, 0xff, 0xff)); // toolbar
                cp.Add(new Color(0x20, 0x20, 0x20, 0x36)); // splitter
                cp.Add(new Color(0xdb, 0x3f, 0x3f)); // icons
                using (FileStream stream = new FileStream("themes\\default.ddp", FileMode.CreateNew))
                {
                    stream.Write(cp.Serialize(), 0, cp.ByteLength);
                }
            }
            if (!themes.Contains(currentTheme + ".ddp"))
                currentTheme = "default";
            themeColorPalette = ColorPalette.Load(File.ReadAllBytes("themes\\" + currentTheme + ".ddp"));
        }

        public static Color GetColor(int i)
        {
            return themeColorPalette.Get(i);
        }
    }
}
