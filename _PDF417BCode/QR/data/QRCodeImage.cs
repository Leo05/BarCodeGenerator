using System;

namespace _PDF417BCode.QRCode.Codec.Data
{
	public interface QRCodeImage
	{
        int Width
        {
            get;

        }
        int Height
        {
            get;

        }
        int getPixel(int x, int y);
	}
}