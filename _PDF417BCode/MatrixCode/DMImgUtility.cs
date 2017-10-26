using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace _PDF417BCode.Internal
{
    public static class DMImgUtility
    {
        /// <summary>
        /// ....
        /// </summary>
        public static Bitmap SimpleResizeBmp(Bitmap inBmp, int resizeFactor, int boderSize)
        {
            int drawAreaW = resizeFactor * inBmp.Width;
            int drawAreaH = resizeFactor * inBmp.Height;
            Bitmap outBmp = new Bitmap(drawAreaW + 2 * boderSize, drawAreaH + 2 * boderSize);
            Graphics g = Graphics.FromImage(outBmp);

            // Imposta parametri per il resizing 
            // (Attenzione: senza PixelOffsetMode a HighQuality viene tagliato un pezzo dell'immagine)
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            // Sfondo bianco
            g.FillRectangle(Brushes.White, 0, 0, outBmp.Width, outBmp.Height);

            // Disegna immagine
            g.DrawImage(inBmp, new Rectangle(boderSize, boderSize, drawAreaW, drawAreaH), 0, 0, inBmp.Width, inBmp.Height, GraphicsUnit.Pixel);

            g.Dispose();
            return outBmp;
        }
    }
}
