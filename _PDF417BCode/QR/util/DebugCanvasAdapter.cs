using System;
using Line = _PDF417BCode.QRCode.Geom.Line;
using Point = _PDF417BCode.QRCode.Geom.Point;

namespace _PDF417BCode.QRCode.Codec.Util
{
	/* 
	* This class must be a "edition independent" class for debug information controll.
	* I think it's good idea to modify this class with a adapter pattern
	*/
	public class DebugCanvasAdapter : DebugCanvas
	{
		public virtual void  println(String string_Renamed)
		{
		}
		
		public virtual void  drawPoint(Point point, int color)
		{
		}
		
		public virtual void  drawCross(Point point, int color)
		{
		}
		
		public virtual void  drawPoints(Point[] points, int color)
		{
		}
		
		public virtual void  drawLine(Line line, int color)
		{
		}
		
		public virtual void  drawLines(Line[] lines, int color)
		{
		}
		
		public virtual void  drawPolygon(Point[] points, int color)
		{
		}
		
		public virtual void  drawMatrix(bool[][] matrix)
		{
		}
		
	}
}