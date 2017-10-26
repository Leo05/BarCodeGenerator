using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;


[assembly: InternalsVisibleTo("Friend1, PublicKey=002400000480000094" +
                              "0000000602000000240000525341310004000" +
                              "001000100bf8c25fcd44838d87e245ab35bf7" +
                              "3ba2615707feea295709559b3de903fb95a93" +
                              "3d2729967c3184a97d7b84c7547cd87e435b5" +
                              "6bdf8621bcb62b59c00c88bd83aa62c4fcdd4" +
                              "712da72eec2533dc00f8529c3a0bbb4103282" +
                              "f0d894d5f34e9f0103c473dce9f4b457a5dee" +
                              "fd8f920d8681ed6dfcb0a81e96bd9b176525a" +
                              "26e0b3")]

namespace _PDF417BCode
{
    [AttributeUsageAttribute(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class BCode : Attribute, IDisposable
    {
        #region Managed Resorces
        /// <summary>
        /// Managed Resurces
        /// </summary>
        private int IndexListe, I, J, K, IndexChaine, Mode, Diviseur, CodeASCII;
        private string ChaineMod, ChaineMult, Dummy;
        private IList<IntArray> Liste = new List<IntArray>();
        private IList<IntArray> ListeT = new List<IntArray>();
        private PrivateFontCollection m_PFC;
        private GCHandle m_pFont;
        private IntPtr m_hFont;
        #endregion
        #region Public Enum
        public enum BCodeType { CODE39, CODE128, PDF417, QR, DATAMATRIX }
        public enum BCodeImageType { PNG, BMP, GIF, JPEG, TIFF }
        public enum QRENCODE_MODE { BYTE, ALPHANUMERIC, NUMERIC }
        public enum QRERROR_CORRECTION { L, M, Q, H }
        #endregion
        #region UnManaged Resoruces
        [DllImport("gdi32")]
        private static extern IntPtr
         AddFontMemResourceEx(IntPtr pbFont,
         uint cbFont,
         IntPtr pdv,
         [In] ref uint pcFonts);

        [DllImport("gdi32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool
         RemoveFontMemResourceEx(IntPtr fh);
        #endregion
        #region Class Initilizer
        /// <summary>
        /// XLPDF417BCode
        /// </summary>
        /// <return>
        ///   'Return : * a string which, printed with the PDF417 Barode.
        ///   '         * an empty string if the given parameters aren't good.
        ///   '         * SecurityLevel contain le really used sécurity level.
        ///   '         * TotalColumns contain the really used number of data CW columns.
        ///   '         * ErrorCode is 0 if no error occured, else :
        ///   '           0  : No error
        ///   '           1  : sTringToEncode is empty
        ///   '           2  : sTringToEncode contain too many datas, we go beyong the 928 CWs.
        ///   '           3  : Number of CWs per row too small, we go beyong 90 rows.
        ///   '           10 : The security level has being lowers not to exceed the 928 CWs. (Its not an error, only a warning.)
        /// </return>
        public BCode()
        {

        }
        /// <summary>
        /// XLPDF417BCode
        /// </summary>
        /// <param name="sTringToEncode">The string to encode</param>
        /// <return>
        ///   'Return : * a string which, printed with the PDF417 Barode.
        ///   '         * an empty string if the given parameters aren't good.
        ///   '         * XYRatio character separation level
        ///   '         * TotalColumns contain the really used number of data CW columns.
        ///   '         * ErrorCode is 0 if no error occured, else :
        ///   '           0  : No error
        ///   '           1  : sTringToEncode is empty
        ///   '           2  : sTringToEncode contain too many datas, we go beyong the 928 CWs.
        ///   '           3  : Number of CWs per row too small, we go beyong 90 rows.
        ///   '           10 : The security level has being lowers not to exceed the 928 CWs. (Its not an error, only a warning.)
        /// </return>
        public BCode(string sTringToEncode)
        {
            _sTringToEncode = sTringToEncode;
        }
        /// <summary>
        /// XLPDF417BCode
        /// </summary>
        /// <param name="sTringToEncode">The string to encode</param>
        /// <param name="XYRatio">The hoped character separatio level, -1 = automatic</param>
        /// <return>
        ///   'Return : * a string which, printed with the PDF417 Barode.
        ///   '         * an empty string if the given parameters aren't good.
        ///   '         * XYRatio character separation level
        ///   '         * TotalColumns contain the really used number of data CW columns.
        ///   '         * ErrorCode is 0 if no error occured, else :
        ///   '           0  : No error
        ///   '           1  : sTringToEncode is empty
        ///   '           2  : sTringToEncode contain too many datas, we go beyong the 928 CWs.
        ///   '           3  : Number of CWs per row too small, we go beyong 90 rows.
        ///   '           10 : The security level has being lowers not to exceed the 928 CWs. (Its not an error, only a warning.)
        /// </return>
        public BCode(string sTringToEncode, int XYRatio)
        {
            _sTringToEncode = sTringToEncode;
            _SecurityLevel = XYRatio;
        }
        /// <summary>
        /// XLPDF417BCode
        /// </summary>
        /// <param name="sTringToEncode">The string to encode</param>
        /// <param name="SecurityLevel">The hoped character separation level, -1 = automatic</param>
        /// <param name="TotalColumns">The hoped number of data MC columns, -1 = automatic</param>
        /// <return>
        ///   'Return : * a string which, printed with the PDF417 Barode.
        ///   '         * an empty string if the given parameters aren't good.
        ///   '         * XYRatio character separation level
        ///   '         * TotalColumns contain the really used number of data CW columns.
        ///   '         * ErrorCode is 0 if no error occured, else :
        ///   '           0  : No error
        ///   '           1  : sTringToEncode is empty
        ///   '           2  : sTringToEncode contain too many datas, we go beyong the 928 CWs.
        ///   '           3  : Number of CWs per row too small, we go beyong 90 rows.
        ///   '           10 : The security level has being lowers not to exceed the 928 CWs. (Its not an error, only a warning.)
        /// </return>
        public BCode(string sTringToEncode, int XYRatio, int TotalColumns)
        {
            _sTringToEncode = sTringToEncode;
            _SecurityLevel = XYRatio;
            _TotalColumns = TotalColumns;
        }
        /// <summary>
        /// XLPDF417BCode
        /// </summary>
        /// <param name="sTringToEncode">The string to encode</param>
        /// <param name="XYRatio">The hoped separation level, -1 = automatic</param>
        /// <param name="TotalColumns">The hoped number of data MC columns, -1 = automatic</param>
        /// <param name="ErrorCode">A variable which will can retrieve an error number</param>
        /// <return>
        ///   'Return : * a string which, printed with the PDF417 Barode.
        ///   '         * an empty string if the given parameters aren't good.
        ///   '         * XYRatio character separation level
        ///   '         * TotalColumns contain the really used number of data CW columns.
        ///   '         * ErrorCode is 0 if no error occured, else :
        ///   '           0  : No error
        ///   '           1  : sTringToEncode is empty
        ///   '           2  : sTringToEncode contain too many datas, we go beyong the 928 CWs.
        ///   '           3  : Number of CWs per row too small, we go beyong 90 rows.
        ///   '           10 : The security level has being lowers not to exceed the 928 CWs. (Its not an error, only a warning.)
        /// </return>
        public BCode(string sTringToEncode, int XYRatio, int TotalColumns, int ErrorCode)
        {
            _sTringToEncode = sTringToEncode;
            _SecurityLevel = XYRatio;
            _TotalColumns = TotalColumns;
            _ErrorCode = ErrorCode;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// String to be encoded
        /// </summary>
        private string _sTringToEncode = string.Empty;
        public string StringToEncode
        {
            get { return _sTringToEncode; }
            set { _sTringToEncode = value; }
        }
        /// <summary>
        /// The hoped character separation level, -1 = automatic
        /// </summary>
        private int _SecurityLevel = -1;
        public int XYRatio
        {
            get { return _SecurityLevel; }
            set { _SecurityLevel = value; }
        }
        /// <summary>
        /// TotalColumns contain the really used number of data CW columns.
        /// </summary>
        private int _TotalColumns = 0;
        public int TotalColumns
        {
            get { return _TotalColumns; }
            set { _TotalColumns = value; }
        }
        /// <summary>
        /// ErrorCode is 0 if no error occured, else :
        /// </summary>
        private int _ErrorCode = 0;
        public int ErrorCode
        {
            get { return _ErrorCode; }
            set { _ErrorCode = value; }
        }
        /// <summary>
        /// Add Label BarCode 128 Type
        /// </summary>
        private Boolean _addLabel = true;
        public Boolean BarCode118AddLabel
        {
            get { return _addLabel; }
            set { _addLabel = value; }
        }
        /// <summary>
        /// BarCode Type CODE128, PDF417
        /// </summary>
        private BCodeType _BCodeType = BCodeType.PDF417;
        public BCodeType BarCodeType
        {
            get { return _BCodeType; }
            set { _BCodeType = value; }
        }
        /// <summary>
        /// Image format
        /// </summary>
        private BCodeImageType _BCodeImageType = BCodeImageType.PNG;
        public BCodeImageType BarCodeImageType
        {
            get { return _BCodeImageType; }
            set { _BCodeImageType = value; }
        }
        /// <summary>
        /// Image Name to be created, please add path
        /// PATH + NAME
        /// </summary>
        private string _imagename = string.Empty;
        public string BarCodeImageName
        {
            get { return _imagename; }
            set { _imagename = value; }
        }
        private QRENCODE_MODE _qrencoding = QRENCODE_MODE.BYTE;
        public QRENCODE_MODE QREncoding
        {
            get { return _qrencoding; }
            set { _qrencoding = value; }
        }
        private QRERROR_CORRECTION _qrerrorc = QRERROR_CORRECTION.M;
        public QRERROR_CORRECTION QRError_c
        {
            get { return _qrerrorc; }
            set { _qrerrorc = value; }
        }
        private Int16  _qrversion = 7;
        public Int16 QRVersion
        {
            get { return _qrversion; }
            set { _qrversion = value; }
        }
        private Int16 _qrscale = 4;
        public Int16 QRScale
        {
            get { return _qrscale; }
            set { _qrscale = value; }
        }
        private Int16 _matrixcodewidth = 5;
        public Int16 MatrixCodeWidth
        {
            get { return _matrixcodewidth; }
            set { _matrixcodewidth = value; }
        }
        private Int16 _matrixcodeheight = 25;
        public Int16 MatrixCodeHeight
        {
            get { return _matrixcodeheight; }
            set { _matrixcodeheight = value; }
        }
        #endregion
        #region Public Members
        /// <summary>
        /// TextToImage
        /// </summary>
        public void TextToImage()
        {
            if (_sTringToEncode != string.Empty)
            {
                if (@_imagename != string.Empty)
                {
                    ImageFormat imgFt = null;
                    switch (_BCodeImageType)
                    {
                        case BCodeImageType.BMP:
                            imgFt = ImageFormat.Bmp;
                            break;
                        case BCodeImageType.GIF:
                            imgFt = ImageFormat.Gif;
                            break;
                        case BCodeImageType.JPEG:
                            imgFt = ImageFormat.Jpeg;
                            break;
                        case BCodeImageType.PNG:
                            imgFt = ImageFormat.Png;
                            break;
                        case BCodeImageType.TIFF:
                            imgFt = ImageFormat.Tiff;
                            break;
                        default:
                            imgFt = ImageFormat.Png;
                            break;
                    }
                    if (_BCodeType == BCodeType.QR)
                    {
                        _TextToImage(imgFt, true);
                    }
                    else if(_BCodeType == BCodeType.DATAMATRIX)
                    {
                        _TextToImageDataMatrix(imgFt);
                    }
                    else if (_BCodeType == BCodeType.CODE39)
                    {
                        _TextToImageCode39(imgFt);
                    }
                    else
                    {
                        _TextToImage(imgFt);
                    }
                }
            }
        }
        /// <summary>
        /// TextToImage
        /// </summary>
        public Byte[] TextToStream()
        {
            Byte[] ReturnBytes = null;
            if (_sTringToEncode != string.Empty)
            {
                ImageFormat imgFt = null;
                switch (_BCodeImageType)
                {
                    case BCodeImageType.BMP:
                        imgFt = ImageFormat.Bmp;
                        break;
                    case BCodeImageType.GIF:
                        imgFt = ImageFormat.Gif;
                        break;
                    case BCodeImageType.JPEG:
                        imgFt = ImageFormat.Jpeg;
                        break;
                    case BCodeImageType.PNG:
                        imgFt = ImageFormat.Png;
                        break;
                    case BCodeImageType.TIFF:
                        imgFt = ImageFormat.Tiff;
                        break;
                    default:
                        imgFt = ImageFormat.Png;
                        break;
                }
                if (_BCodeType == BCodeType.QR)
                {
                    ReturnBytes = _TextToStream(imgFt, true);
                }
                else if (_BCodeType == BCodeType.DATAMATRIX)
                {
                    ReturnBytes = _TextToStreamDataMatrix(imgFt);
                }
                else if (_BCodeType == BCodeType.CODE39)
                {
                    ReturnBytes = _TextToStreamCode39(imgFt);
                }
                else
                {
                    ReturnBytes = _TextToStream(imgFt);
                }
            }
            return ReturnBytes;
        }
        #endregion
        #region Private Members
        private string GetTextEncoded()
        {
            if (_BCodeType == BCodeType.PDF417)
            {
                return pdf417();
            }
            {
                return code128();
            }
        }
        private void _TextToImage(ImageFormat imgFt)
        {
            Bitmap newBitmap = null;
            Graphics g = null;
            try
            {
                m_PFC = new PrivateFontCollection();
                int rsxLen = 0;
                Single FontSize = 0;
                if (_BCodeType == BCodeType.PDF417)
                {
                    rsxLen = Properties.Resources.pdf417.Length;
                    m_pFont = GCHandle.Alloc(Properties.Resources.pdf417, GCHandleType.Pinned);
                    FontSize = 30f;
                }
                else
                {
                    rsxLen = Properties.Resources.code128.Length;
                    m_pFont = GCHandle.Alloc(Properties.Resources.code128, GCHandleType.Pinned);
                    FontSize = 110f;
                }
                m_PFC.AddMemoryFont(m_pFont.AddrOfPinnedObject(), rsxLen);
                uint rsxCnt = 1;
                m_hFont = AddFontMemResourceEx(m_pFont.AddrOfPinnedObject(), (uint)rsxLen, IntPtr.Zero, ref rsxCnt);
                Font m_Font = null;
                FontFamily ff = m_PFC.Families[0];
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    m_Font = new Font(ff, FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                }

                string str = GetTextEncoded();

                newBitmap = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
                g = Graphics.FromImage(newBitmap);
                SizeF stringSize = g.MeasureString(str, m_Font);
                int nWidth = (int)stringSize.Width;
                int nHeight = (int)stringSize.Height;
                if (_BCodeType == BCodeType.CODE128 && _addLabel) nHeight += 10;
                g.Dispose();
                newBitmap.Dispose();
                newBitmap = new Bitmap(nWidth, nHeight, PixelFormat.Format32bppArgb);
                g = Graphics.FromImage(newBitmap);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, nWidth, nHeight));
                TextRenderer.DrawText(g, str, m_Font, new Rectangle(0, 0, nWidth, nHeight), Color.Black, TextFormatFlags.PathEllipsis | TextFormatFlags.PreserveGraphicsClipping); // clips
                if (_BCodeType == BCodeType.CODE128 && _addLabel)
                {
                    using (Font myFont = new Font("Arial", 12))
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Far;
                        g.DrawString(_sTringToEncode, myFont, Brushes.Black, new Rectangle(0, 0, nWidth, nHeight), stringFormat);
                    }
                }
                newBitmap.Save(@_imagename, imgFt);
                m_PFC.Dispose();
                m_pFont.Free();
                RemoveFontMemResourceEx(m_hFont);
                GC.Collect();
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (null != g) g.Dispose();
                if (null != newBitmap) newBitmap.Dispose();
            }
        }
        private void _TextToImage(ImageFormat imgFt, Boolean BCT)
        {
            _PDF417BCode.QRCode.Codec.QRCodeEncoder qrCodeEncoder = new _PDF417BCode.QRCode.Codec.QRCodeEncoder();
            string encoding = QREncoding.ToString().ToLower();
            if (encoding == "byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "alphanumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCode.Codec.QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCode.Codec.QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }

            int scale = QRScale;
            qrCodeEncoder.QRCodeScale = scale;

            int version = QRVersion;
            qrCodeEncoder.QRCodeVersion = version;

            string errorCorrect = _qrerrorc.ToString();
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;
            String data = _sTringToEncode;
            image = qrCodeEncoder.Encode(data);
            image.Save(@_imagename, imgFt);
        }
        private void _TextToImageDataMatrix(ImageFormat imgFt)
        {
            _PDF417BCode.Internal.DataMatrix dm = new _PDF417BCode.Internal.DataMatrix(_sTringToEncode);
            _PDF417BCode.Internal.DMImgUtility.SimpleResizeBmp(dm.Image, MatrixCodeWidth, MatrixCodeHeight).Save(@_imagename, imgFt);
        }
        private void _TextToImageCode39(ImageFormat imgFt)
        {
            _PDF417BCode.Code39.Code39 code = new _PDF417BCode.Code39.Code39(_sTringToEncode);
            code.Paint().Save(@_imagename, imgFt);
        }
        /// <summary>
        /// TextToStreamPng
        /// </summary>
        /// <return>
        ///   Return : ByteArray of Barcode Image
        /// </return>
        private Byte[] _TextToStream(ImageFormat imgFt)
        {
            Bitmap newBitmap = null;
            Graphics g = null;
            byte[] byteArray = null;
            try
            {
                m_PFC = new PrivateFontCollection();
                //int rsxLen = Properties.Resources.pdf417.Length;
                //m_pFont = GCHandle.Alloc(Properties.Resources.pdf417, GCHandleType.Pinned);
                int rsxLen = 0;
                Single FontSize = 0;
                if (_BCodeType == BCodeType.PDF417)
                {
                    rsxLen = Properties.Resources.pdf417.Length;
                    m_pFont = GCHandle.Alloc(Properties.Resources.pdf417, GCHandleType.Pinned);
                    FontSize = 30f;
                }
                else
                {
                    rsxLen = Properties.Resources.code128.Length;
                    m_pFont = GCHandle.Alloc(Properties.Resources.code128, GCHandleType.Pinned);
                    FontSize = 110f;
                }
                m_PFC.AddMemoryFont(m_pFont.AddrOfPinnedObject(), rsxLen);
                uint rsxCnt = 1; /* We're only installing one font. */
                // This is where we do the actual "registration" to get the font handle
                m_hFont = AddFontMemResourceEx(m_pFont.AddrOfPinnedObject(), (uint)rsxLen, IntPtr.Zero, ref rsxCnt);
                Font m_Font = null;
                FontFamily ff = m_PFC.Families[0]; /* I know, bad practice. */
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    m_Font = new Font(ff, FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                }
                string str = GetTextEncoded();
                newBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                g = Graphics.FromImage(newBitmap);
                SizeF stringSize = g.MeasureString(str, m_Font);
                int nWidth = (int)stringSize.Width;
                int nHeight = (int)stringSize.Height;
                if (_BCodeType == BCodeType.CODE128 && _addLabel) nHeight += 10;
                g.Dispose();
                newBitmap.Dispose();
                newBitmap = new Bitmap(nWidth, nHeight, PixelFormat.Format32bppArgb);
                g = Graphics.FromImage(newBitmap);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, nWidth, nHeight));
                TextRenderer.DrawText(g, str, m_Font, new Rectangle(0, 0, nWidth, nHeight), Color.Black, TextFormatFlags.PathEllipsis | TextFormatFlags.PreserveGraphicsClipping); // clips
                if (_BCodeType == BCodeType.CODE128 && _addLabel)
                {
                    using (Font myFont = new Font("Arial", 12))
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Far;
                        g.DrawString(_sTringToEncode, myFont, Brushes.Black, new Rectangle(0, 0, nWidth, nHeight), stringFormat);
                    }
                }
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    newBitmap.Save(ms, imgFt);
                    byteArray = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));
                }
                m_PFC.Dispose();
                m_pFont.Free();
                RemoveFontMemResourceEx(m_hFont);
                GC.Collect();
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (null != g) g.Dispose();
                if (null != newBitmap) newBitmap.Dispose();
            }
            return byteArray;
        }
        private Byte[] _TextToStream(ImageFormat imgFt, Boolean BCT)
        {
            _PDF417BCode.QRCode.Codec.QRCodeEncoder qrCodeEncoder = new _PDF417BCode.QRCode.Codec.QRCodeEncoder();
            string encoding = QREncoding.ToString().ToLower();
            if (encoding == "byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "alphanumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCode.Codec.QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCode.Codec.QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }

            int scale = QRScale;
            qrCodeEncoder.QRCodeScale = scale;

            int version = QRVersion;
            qrCodeEncoder.QRCodeVersion = version;

            string errorCorrect = _qrerrorc.ToString();
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;
            String data = _sTringToEncode;
            image = qrCodeEncoder.Encode(data);
            byte[] byteArray = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                image.Save(ms, imgFt);
                byteArray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));
            }

            return byteArray;
        }
        private Byte[] _TextToStreamDataMatrix(ImageFormat imgFt)
        {
            _PDF417BCode.Internal.DataMatrix dm = new _PDF417BCode.Internal.DataMatrix(_sTringToEncode);
            Image image = _PDF417BCode.Internal.DMImgUtility.SimpleResizeBmp(dm.Image, MatrixCodeWidth, MatrixCodeHeight);
            byte[] byteArray = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                image.Save(ms, imgFt);
                byteArray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));
            }
            return byteArray;
        }
        private Byte[] _TextToStreamCode39(ImageFormat imgFt)
        {
            _PDF417BCode.Code39.Code39 code = new _PDF417BCode.Code39.Code39(_sTringToEncode);
            Image image = code.Paint();
            byte[] byteArray = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                image.Save(ms, imgFt);
                byteArray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));
            }
            return byteArray;
        }
        #endregion
        #region CODE128
        private string code128()
        {
            string StringEncoded = string.Empty;
            int i, dummy, mini;
            int checksum = 0;
            Boolean tableB = true;
            if (_sTringToEncode.Length > 0)
            {
                //'Check for valid characters
                for (i = 0; i <= _sTringToEncode.Length - 1; i++)
                {
                    char tChar = Convert.ToChar(Mid(_sTringToEncode, i, 1));
                    if ((Asc(tChar) < 32 && Asc(tChar) > 126) || tChar == 203)
                    {
                        i = 0;
                        break;
                    }
                }
                //Calculation of the code string with optimized use of tables B and C
                if (i > 0)
                {
                    i = 0; // i become the string index
                    do
                    {
                        if (tableB)
                        {
                            //See if interesting to switch to table C
                            //yes for 4 digits at start or end, else if 6 digits
                            mini = i == 0 || i + 3 == _sTringToEncode.Length - 1 ? 4 : 6;
                            mini = testnum(i, mini);
                            if (mini < 0) //Choice of table C
                            {
                                if (i == 0) // Starting with table C
                                {
                                    //code128$ = Chr$(210)
                                    StringEncoded = Chr(210).ToString();
                                }
                                else // Switch to table C
                                {
                                    //code128$ = code128$ & Chr$(204)
                                    StringEncoded += Chr(204).ToString();
                                }
                                tableB = false;
                            }
                            else
                            {
                                if (i == 0)
                                    StringEncoded = Chr(209).ToString(); //Starting with table B
                            }
                        }
                        if (!tableB)
                        {
                            //We are on table C, try to process 2 digits
                            mini = 2;
                            mini = testnum(i, mini);
                            if (mini < 0) //OK for 2 digits, process it
                            {
                                dummy = Convert.ToInt32(Mid(_sTringToEncode, i, 2));
                                dummy = dummy < 95 ? dummy + 32 : dummy + 105;
                                StringEncoded += Convert.ToChar(dummy).ToString();
                                i += 2;
                            }
                            else //We haven't 2 digits, switch to table B
                            {
                                StringEncoded += Chr(205).ToString();
                                tableB = true;
                            }
                        }
                        if (tableB)
                        {
                            // Process 1 digit with table B
                            StringEncoded += Mid(_sTringToEncode, i, 1).ToString();
                            i++;
                        }
                    }
                    while (i <= _sTringToEncode.Length - 1);
                    // Calculation of the checksum
                    for (i = 0; i <= StringEncoded.Length - 1; i++)
                    {
                        char tChar = Convert.ToChar(Mid(StringEncoded, i, 1));
                        dummy = Asc(tChar);
                        dummy = dummy < 127 ? dummy - 32 : dummy - 105;
                        if (i == 0)
                        {
                            checksum = dummy;
                            checksum = (checksum + (i) * dummy) % 103;
                        }
                        else { checksum = (checksum + (i) * dummy) % 103; }
                    }
                    //Calculation of the checksum ASCII code
                    checksum = checksum < 95 ? checksum + 32 : checksum + 105;
                    //Add the checksum and the STOP
                    StringEncoded += Convert.ToChar(checksum).ToString() + Chr(211).ToString();
                }
            }
            return StringEncoded;
        }

        private int testnum(int i, int mini)
        {
            //'if the mini% characters from i% are numeric, then mini%=0
            int VarRet = mini;
            VarRet--;
            if (i + VarRet <= _sTringToEncode.Length - 1)
            {
                do
                {
                    char tChar = Convert.ToChar(Mid(_sTringToEncode, i + VarRet, 1));
                    if (Asc(tChar) < 48 || Asc(tChar) > 57) { break; }
                    VarRet--;
                }
                while (VarRet >= 0);
            }
            return VarRet;
        }
        #endregion
        #region PDF417
        private string pdf417()
        {
            string StringEncoded = string.Empty;
            //Global variables
            Boolean flag;
            //Splitting into blocks
            //int[,] Liste;
            //Data compaction
            double Longueur;
            int Total;
            //"text" mode processing
            //string[,] ListeT;
            int IndexListeT, CurTable, NewTable;
            string ChaineT, ChaineMC = "";
            //Reed Solomon codes
            //int[] MCcorrection;
            ///Left and right side CWs
            double C1, C2, C3;
            //Sub routine QuelMode
            //Tables
            string ASCII;
            //This string describe the ASCII code for the "text" mode.
            //ASCII$ contain 95 fields of 4 digits which correspond to char. ASCII values 32 to 126. These fields are :
            //  2 digits indicating the table(s) (1 or several) where this char. is located. (Table numbers : 1, 2, 4 and 8)
            //  2 digits indicating the char. number in the table
            //  Sample : 0726 at the beginning of the string : The Char. having code 32 is in the tables 1, 2 and 4 at row 26
            ASCII = "07260810082004151218042104100828082308241222042012131216121712190400040104020403040404050406040704080409121408000801042308020825080301000101010201030104010501060107010801090110011101120113011401150116011701180119012001210122012301240125080408050806042408070808020002010202020302040205020602070208020902100211021202130214021502160217021802190220022102220223022402250826082108270809";
            string[] CoefRS = new string[9];
            //CoefRS contain 8 strings describing the factors of the polynomial equations for the reed Solomon codes.
            CoefRS[0] = "027917";
            CoefRS[1] = "522568723809";
            CoefRS[2] = "237308436284646653428379";
            CoefRS[3] = "274562232755599524801132295116442428295042176065";
            CoefRS[4] = "361575922525176586640321536742677742687284193517273494263147593800571320803133231390685330063410";
            CoefRS[5] = "539422006093862771453106610287107505733877381612723476462172430609858822543376511400672762283184440035519031460594225535517352605158651201488502648733717083404097280771840629004381843623264543";
            CoefRS[6] = "521310864547858580296379053779897444400925749415822093217208928244583620246148447631292908490704516258457907594723674292272096684432686606860569193219129186236287192775278173040379712463646776171491297763156732095270447090507048228821808898784663627378382262380602754336089614087432670616157374242726600269375898845454354130814587804034211330539297827865037517834315550086801004108539";
            CoefRS[7] = "524894075766882857074204082586708250905786138720858194311913275190375850438733194280201280828757710814919089068569011204796605540913801700799137439418592668353859370694325240216257284549209884315070329793490274877162749812684461334376849521307291803712019358399908103511051008517225289470637731066255917269463830730433848585136538906090002290743199655903329049802580355588188462010134628320479130739071263318374601192605142673687234722384177752607640455193689707805641048060732621895544261852655309697755756060231773434421726528503118049795032144500238836394280566319009647550073914342126032681331792620060609441180791893754605383228749760213054297134054834299922191910532609829189020167029872449083402041656505579481173404251688095497555642543307159924558648055497010";
            CoefRS[8] = "352077373504035599428207409574118498285380350492197265920155914299229643294871306088087193352781846075327520435543203666249346781621640268794534539781408390644102476499290632545037858916552041542289122272383800485098752472761107784860658741290204681407855085099062482180020297451593913142808684287536561076653899729567744390513192516258240518794395768848051610384168190826328596786303570381415641156237151429531207676710089168304402040708575162864229065861841512164477221092358785288357850836827736707094008494114521002499851543152729771095248361578323856797289051684466533820669045902452167342244173035463651051699591452578037124298332552043427119662777475850764364578911283711472420245288594394511327589777699688043408842383721521560644714559062145873663713159672729";
            CoefRS[8] += "624059193417158209563564343693109608563365181772677310248353708410579870617841632860289536035777618586424833077597346269757632695751331247184045787680018066407369054492228613830922437519644905789420305441207300892827141537381662513056252341242797838837720224307631061087560310756665397808851309473795378031647915459806590731425216548249321881699535673782210815905303843922281073469791660162498308155422907817187062016425535336286437375273610296183923116667751353062366691379687842037357720742330005039923311424242749321054669316342299534105667488640672576540316486721610046656447171616464190531297321762752533175134014381433717045111020596284736138646411877669141919045780407164332899165726600325498655357752768223849647063310863251366304282738675410389244031121303263";
            string[] CodageMC = new string[3];
            //CodageMC contain the 3 sets of the 929 MCs. Each MC is described in the PDF417.TTF font by 3 char. composing 3 time 5 bits. The first bit which is always 1
            //and the last one which is always 0 are into the separator character.
            CodageMC[0] = "urAxfsypyunkxdwyozpDAulspBkeBApAseAkprAuvsxhypnkutwxgzfDAplsfBkfrApvsuxyfnkptwuwzflspsyfvspxyftwpwzfxyyrxufkxFwymzonAudsxEyolkucwdBAoksucidAkokgdAcovkuhwxazdnAotsugydlkoswugjdksosidvkoxwuizdtsowydswowjdxwoyzdwydwjofAuFsxCyodkuEwxCjclAocsuEickkocgckcckEcvAohsuayctkogwuajcssogicsgcsacxsoiycwwoijcwicyyoFkuCwxBjcdAoEsuCicckoEguCbcccoEaccEoEDchkoawuDjcgsoaicggoabcgacgDobjcibcFAoCsuBicEkoCguBbcEcoCacEEoCDcECcascagcaacCkuAroBaoBDcCBtfkwpwyezmnAtdswoymlktcwwojFBAmksFAkmvkthwwqzFnAmtstgyFlkmswFksFkgFvkmxwtizFtsmwyFswFsiFxwmyzFwyFyzvfAxpsyuyvdkxowyujqlAvcsxoiqkkvcgxobqkcvcamfAtFswmyqvAmdktEwwmjqtkvgwxqjhlAEkkmcgtEbhkkqsghkcEvAmhstayhvAEtkmgwtajhtkqwwvijhssEsghsgExsmiyhxsEwwmijhwwqyjhwiEyyhyyEyjhyjvFkxmwytjqdAvEsxmiqckvEgxmbqccvEaqcEqcCmFktCwwljqhkmEstCigtAEckvaitCbgskEccmEagscqgamEDEcCEhkmawtDjgxkEgsmaigwsqiimabgwgEgaEgDEiwmbjgywEiigyiEibgybgzjqFAvCsxliqEkvCgxlbqEcvCaqEEvCDqECqEBEFAmCstBighAEEkmCgtBbggkqagvDbggcEEEmCDggEqaDgg";
            CodageMC[0] += "CEasmDigisEagmDbgigqbbgiaEaDgiDgjigjbqCkvBgxkrqCcvBaqCEvBDqCCqCBECkmBgtArgakECcmBagacqDamBDgaEECCgaCECBEDggbggbagbDvAqvAnqBBmAqEBEgDEgDCgDBlfAspsweyldksowClAlcssoiCkklcgCkcCkECvAlhssqyCtklgwsqjCsslgiCsgCsaCxsliyCwwlijCwiCyyCyjtpkwuwyhjndAtoswuincktogwubncctoancEtoDlFksmwwdjnhklEssmiatACcktqismbaskngglEaascCcEasEChklawsnjaxkCgstrjawsniilabawgCgaawaCiwlbjaywCiiayiCibCjjazjvpAxusyxivokxugyxbvocxuavoExuDvoCnFAtmswtirhAnEkxviwtbrgkvqgxvbrgcnEEtmDrgEvqDnEBCFAlCssliahACEklCgslbixAagknagtnbiwkrigvrblCDiwcagEnaDiwECEBCaslDiaisCaglDbiysaignbbiygrjbCaDaiDCbiajiCbbiziajbvmkxtgywrvmcxtavmExtDvmCvmBnCktlgwsrraknCcxtrracvnatlDraEnCCraCnCBraBCCklBgskraakCCclBaiikaacnDalBDiicrbaCCCiiEaaCCCBaaBCDglBrabgCDaijgabaCDDijaabDCDrijrvlcxsqvlExsnvlCvlBnBctkqrDcnBEtknrDEvlnrDCnBBrDBCBclAqaDcCBElAnibcaDEnBnibErDnCBBibCaDBibBaDqibqibnxsfvkltkfnAmnAlCAoaBoiDoCAlaBlkpkBdAkosBckkogsebBcckoaBcEkoDBhkkqwsfjBgskqiBggkqbBgaBgDBiwkrjBiiBibBjjlpAsuswhil";
            CodageMC[0] += "oksuglocsualoEsuDloCBFAkmssdiDhABEksvisdbDgklqgsvbDgcBEEkmDDgElqDBEBBaskniDisBagknbDiglrbDiaBaDBbiDjiBbbDjbtukwxgyirtucwxatuEwxDtuCtuBlmkstgnqklmcstanqctvastDnqElmCnqClmBnqBBCkklgDakBCcstrbikDaclnaklDbicnraBCCbiEDaCBCBDaBBDgklrDbgBDabjgDbaBDDbjaDbDBDrDbrbjrxxcyyqxxEyynxxCxxBttcwwqvvcxxqwwnvvExxnvvCttBvvBllcssqnncllEssnrrcnnEttnrrEvvnllBrrCnnBrrBBBckkqDDcBBEkknbbcDDEllnjjcbbEnnnBBBjjErrnDDBjjCBBqDDqBBnbbqDDnjjqbbnjjnxwoyyfxwmxwltsowwfvtoxwvvtmtslvtllkossfnlolkmrnonlmlklrnmnllrnlBAokkfDBolkvbDoDBmBAljbobDmDBljbmbDljblDBvjbvxwdvsuvstnkurlurltDAubBujDujDtApAAokkegAocAoEAoCAqsAqgAqaAqDAriArbkukkucshakuEshDkuCkuBAmkkdgBqkkvgkdaBqckvaBqEkvDBqCAmBBqBAngkdrBrgkvrBraAnDBrDAnrBrrsxcsxEsxCsxBktclvcsxqsgnlvEsxnlvCktBlvBAlcBncAlEkcnDrcBnEAlCDrEBnCAlBDrCBnBAlqBnqAlnDrqBnnDrnwyowymwylswotxowyvtxmswltxlksosgfltoswvnvoltmkslnvmltlnvlAkokcfBloksvDnoBlmAklbroDnmBllbrmDnlAkvBlvDnvbrvyzeyzdwyexyuwydxytswetwuswdvxutwtvxtkselsuksdntulstrvu";
            CodageMC[1] = "ypkzewxdAyoszeixckyogzebxccyoaxcEyoDxcCxhkyqwzfjutAxgsyqiuskxggyqbuscxgausExgDusCuxkxiwyrjptAuwsxiipskuwgxibpscuwapsEuwDpsCpxkuywxjjftApwsuyifskpwguybfscpwafsEpwDfxkpywuzjfwspyifwgpybfwafywpzjfyifybxFAymszdixEkymgzdbxEcymaxEEymDxECxEBuhAxasyniugkxagynbugcxaaugExaDugCugBoxAuisxbiowkuigxbbowcuiaowEuiDowCowBdxAoysujidwkoygujbdwcoyadwEoyDdwCdysozidygozbdyadyDdzidzbxCkylgzcrxCcylaxCEylDxCCxCBuakxDgylruacxDauaExDDuaCuaBoikubgxDroicubaoiEubDoiCoiBcykojgubrcycojacyEojDcyCcyBczgojrczaczDczrxBcykqxBEyknxBCxBBuDcxBquDExBnuDCuDBobcuDqobEuDnobCobBcjcobqcjEobncjCcjBcjqcjnxAoykfxAmxAluBoxAvuBmuBloDouBvoDmoDlcbooDvcbmcblxAexAduAuuAtoBuoBtwpAyeszFiwokyegzFbwocyeawoEyeDwoCwoBthAwqsyfitgkwqgyfbtgcwqatgEwqDtgCtgBmxAtiswrimwktigwrbmwctiamwEtiDmwCmwBFxAmystjiFwkmygtjbFwcmyaFwEmyDFwCFysmziFygmzbFyaFyDFziFzbyukzhghjsyuczhahbwyuEzhDhDyyuCyuBwmkydgzErxqkwmczhrxqcyvaydDxqEwmCxqCwmBxqBtakwngydrviktacwnavicxrawnDviEtaCviCtaBviBmiktbgwnrqykmictb";
            CodageMC[1] += "aqycvjatbDqyEmiCqyCmiBqyBEykmjgtbrhykEycmjahycqzamjDhyEEyChyCEyBEzgmjrhzgEzahzaEzDhzDEzrytczgqgrwytEzgngnyytCglzytBwlcycqxncwlEycnxnEytnxnCwlBxnBtDcwlqvbctDEwlnvbExnnvbCtDBvbBmbctDqqjcmbEtDnqjEvbnqjCmbBqjBEjcmbqgzcEjEmbngzEqjngzCEjBgzBEjqgzqEjngznysozgfgfyysmgdzyslwkoycfxloysvxlmwklxlltBowkvvDotBmvDmtBlvDlmDotBvqbovDvqbmmDlqblEbomDvgjoEbmgjmEblgjlEbvgjvysegFzysdwkexkuwkdxkttAuvButAtvBtmBuqDumBtqDtEDugbuEDtgbtysFwkFxkhtAhvAxmAxqBxwekyFgzCrwecyFaweEyFDweCweBsqkwfgyFrsqcwfasqEwfDsqCsqBliksrgwfrlicsraliEsrDliCliBCykljgsrrCycljaCyEljDCyCCyBCzgljrCzaCzDCzryhczaqarwyhEzananyyhCalzyhBwdcyEqwvcwdEyEnwvEyhnwvCwdBwvBsncwdqtrcsnEwdntrEwvntrCsnBtrBlbcsnqnjclbEsnnnjEtrnnjClbBnjBCjclbqazcCjElbnazEnjnazCCjBazBCjqazqCjnaznzioirsrfyziminwrdzzililyikzygozafafyyxozivivyadzyxmyglitzyxlwcoyEfwtowcmxvoyxvwclxvmwtlxvlslowcvtnoslmvrotnmsllvrmtnlvrllDoslvnbolDmrjonbmlDlrjmnblrjlCbolDvajoCbmizoajmCblizmajlizlCbvajvzieifwrFzzididyiczygeaFzywuy";
            CodageMC[1] += "gdihzywtwcewsuwcdxtuwstxttskutlusktvnutltvntlBunDulBtrbunDtrbtCDuabuCDtijuabtijtziFiFyiEzygFywhwcFwshxsxskhtkxvlxlAxnBxrDxCBxaDxibxiCzwFcyCqwFEyCnwFCwFBsfcwFqsfEwFnsfCsfBkrcsfqkrEsfnkrCkrBBjckrqBjEkrnBjCBjBBjqBjnyaozDfDfyyamDdzyalwEoyCfwhowEmwhmwElwhlsdowEvsvosdmsvmsdlsvlknosdvlroknmlrmknllrlBboknvDjoBbmDjmBblDjlBbvDjvzbebfwnpzzbdbdybczyaeDFzyiuyadbhzyitwEewguwEdwxuwgtwxtscustuscttvustttvtklulnukltnrulntnrtBDuDbuBDtbjuDbtbjtjfsrpyjdwrozjcyjcjzbFbFyzjhjhybEzjgzyaFyihyyxwEFwghwwxxxxschssxttxvvxkkxllxnnxrrxBBxDDxbbxjFwrmzjEyjEjbCzjazjCyjCjjBjwCowCmwClsFowCvsFmsFlkfosFvkfmkflArokfvArmArlArvyDeBpzyDdwCewauwCdwatsEushusEtshtkdukvukdtkvtAnuBruAntBrtzDpDpyDozyDFybhwCFwahwixsEhsgxsxxkcxktxlvxAlxBnxDrxbpwnuzboybojDmzbqzjpsruyjowrujjoijobbmyjqybmjjqjjmwrtjjmijmbbljjnjjlijlbjkrsCusCtkFukFtAfuAftwDhsChsaxkExkhxAdxAvxBuzDuyDujbuwnxjbuibubDtjbvjjusrxijugrxbjuajuDbtijvibtbjvbjtgrwrjtajtDbsrjtrjsqjsnBxjDxiDxbbxgnyrbxabxDDwrbxrbwqbwn";
            CodageMC[2] = "pjkurwejApbsunyebkpDwulzeDspByeBwzfcfjkprwzfEfbspnyzfCfDwplzzfBfByyrczfqfrwyrEzfnfnyyrCflzyrBxjcyrqxjEyrnxjCxjBuzcxjquzExjnuzCuzBpzcuzqpzEuznpzCdjAorsufydbkonwudzdDsolydBwokzdAyzdodrsovyzdmdnwotzzdldlydkzynozdvdvyynmdtzynlxboynvxbmxblujoxbvujmujlozoujvozmozlcrkofwuFzcnsodyclwoczckyckjzcucvwohzzctctycszylucxzyltxDuxDtubuubtojuojtcfsoFycdwoEzccyccjzchchycgzykxxBxuDxcFwoCzcEycEjcazcCycCjFjAmrstfyFbkmnwtdzFDsmlyFBwmkzFAyzFoFrsmvyzFmFnwmtzzFlFlyFkzyfozFvFvyyfmFtzyflwroyfvwrmwrltjowrvtjmtjlmzotjvmzmmzlqrkvfwxpzhbAqnsvdyhDkqlwvczhBsqkyhAwqkjhAiErkmfwtFzhrkEnsmdyhnsqtymczhlwEkyhkyEkjhkjzEuEvwmhzzhuzEthvwEtyzhthtyEszhszyduExzyvuydthxzyvtwnuxruwntxrttbuvjutbtvjtmjumjtgrAqfsvFygnkqdwvEzglsqcygkwqcjgkigkbEfsmFygvsEdwmEzgtwqgzgsyEcjgsjzEhEhyzgxgxyEgzgwzycxytxwlxxnxtDxvbxmbxgfkqFwvCzgdsqEygcwqEjgcigcbEFwmCzghwEEyggyEEjggjEazgizgFsqCygEwqCjgEigEbECygayECjgajgCwqBjgCigCbEBjgDjgBigBbCrklfwspzCnsldyClwlczCkyCkjzCuCvwlhzzCtCtyCszyFuCx";
            CodageMC[2] += "zyFtwfuwftsrusrtljuljtarAnfstpyankndwtozalsncyakwncjakiakbCfslFyavsCdwlEzatwngzasyCcjasjzChChyzaxaxyCgzawzyExyhxwdxwvxsnxtrxlbxrfkvpwxuzinArdsvoyilkrcwvojiksrciikgrcbikaafknFwtmzivkadsnEyitsrgynEjiswaciisiacbisbCFwlCzahwCEyixwagyCEjiwyagjiwjCazaiziyzifArFsvmyidkrEwvmjicsrEiicgrEbicaicDaFsnCyihsaEwnCjigwrajigiaEbigbCCyaayCCjiiyaajiijiFkrCwvljiEsrCiiEgrCbiEaiEDaCwnBjiawaCiiaiaCbiabCBjaDjibjiCsrBiiCgrBbiCaiCDaBiiDiaBbiDbiBgrAriBaiBDaAriBriAqiAnBfskpyBdwkozBcyBcjBhyBgzyCxwFxsfxkrxDfklpwsuzDdsloyDcwlojDciDcbBFwkmzDhwBEyDgyBEjDgjBazDizbfAnpstuybdknowtujbcsnoibcgnobbcabcDDFslmybhsDEwlmjbgwDEibgiDEbbgbBCyDayBCjbiyDajbijrpkvuwxxjjdArosvuijckrogvubjccroajcEroDjcCbFknmwttjjhkbEsnmijgsrqinmbjggbEajgabEDjgDDCwlljbawDCijiwbaiDCbjiibabjibBBjDDjbbjjjjjFArmsvtijEkrmgvtbjEcrmajEErmDjECjEBbCsnlijasbCgnlbjagrnbjaabCDjaDDBibDiDBbjbibDbjbbjCkrlgvsrjCcrlajCErlDjCCjCBbBgnkrjDgbBajDabBDjDDDArbBrjDrjBcrkqjBErknjBCjBBbAqjBqbAnjBnjAorkfjAmjAlb";
            CodageMC[2] += "AfjAvApwkezAoyAojAqzBpskuyBowkujBoiBobAmyBqyAmjBqjDpkluwsxjDosluiDoglubDoaDoDBmwktjDqwBmiDqiBmbDqbAljBnjDrjbpAnustxiboknugtxbbocnuaboEnuDboCboBDmsltibqsDmgltbbqgnvbbqaDmDbqDBliDniBlbbriDnbbrbrukvxgxyrrucvxaruEvxDruCruBbmkntgtwrjqkbmcntajqcrvantDjqEbmCjqCbmBjqBDlglsrbngDlajrgbnaDlDjrabnDjrDBkrDlrbnrjrrrtcvwqrtEvwnrtCrtBblcnsqjncblEnsnjnErtnjnCblBjnBDkqblqDknjnqblnjnnrsovwfrsmrslbkonsfjlobkmjlmbkljllDkfbkvjlvrsersdbkejkubkdjktAeyAejAuwkhjAuiAubAdjAvjBuskxiBugkxbBuaBuDAtiBviAtbBvbDuklxgsyrDuclxaDuElxDDuCDuBBtgkwrDvglxrDvaBtDDvDAsrBtrDvrnxctyqnxEtynnxCnxBDtclwqbvcnxqlwnbvEDtCbvCDtBbvBBsqDtqBsnbvqDtnbvnvyoxzfvymvylnwotyfrxonwmrxmnwlrxlDsolwfbtoDsmjvobtmDsljvmbtljvlBsfDsvbtvjvvvyevydnwerwunwdrwtDsebsuDsdjtubstjttvyFnwFrwhDsFbshjsxAhiAhbAxgkirAxaAxDAgrAxrBxckyqBxEkynBxCBxBAwqBxqAwnBxnlyoszflymlylBwokyfDxolyvDxmBwlDxlAwfBwvDxvtzetzdlyenyulydnytBweDwuBwdbxuDwtbxttzFlyFnyhBwFDwhbwxAiqAinAyokjfAymAylAifAyvkzekzdAyeByuAydBytszp";
            _ErrorCode = 0;
            if (_sTringToEncode == "")
            {
                _ErrorCode = 1;
                return StringEncoded;
            }
            //Split the string in character blocks of the same type : numeric , text, byte
            //The first column of the array Liste contain the char. number, the second one contain the mode switch
            IndexChaine = 0;
            CodeASCII = 0;
            Mode = 0;
            QuelMode();
            for (IndexListe = 0; IndexChaine <= _sTringToEncode.Length - 1; IndexListe++)
            {
                SetValue(Liste, 1, IndexListe, Mode);
                while (GetValue(Liste, 1, IndexListe) == Mode)
                {
                    SetValue(Liste, 0, IndexListe, GetValue(Liste, 0, IndexListe) + 1);
                    IndexChaine += 1;
                    if (IndexChaine >= _sTringToEncode.Length) { IndexChaine += 1; break; }
                    QuelMode();
                }
            }
            //DebugList(Liste);

            //We retain "numeric" mode only if it's earning, else "text" mode or even "byte" mode
            //The efficiency limits have been pre-defined according to the previous mode and/or the next mode.
            for (I = 0; I <= (IndexListe - 1); I++)
            {
                if (GetValue(Liste, 1, I) == 902)
                {
                    if (I == 0) //It's the first block
                    {
                        if (IndexListe > 1) //And there is other blocks behind
                        {
                            if (GetValue(Liste, 1, I + 1) == 900)
                            {
                                //First block and followed by a "text" type block
                                if (GetValue(Liste, 0, I) < 8) { SetValue(Liste, 1, I, 900); }
                            }
                            else if (GetValue(Liste, 1, I + 1) == 901)
                            {
                                //First block and followed by a "byte" type block
                                if (GetValue(Liste, 0, I) == 1) { SetValue(Liste, 1, I, 901); }
                            }
                        }
                    }
                    else
                    {
                        //It's not the first block
                        if (I == IndexListe - 1)
                        {
                            //It's the last one
                            if (GetValue(Liste, 1, I - 1) == 900)
                            {
                                //It's  preceded by a "text" type block
                                if (GetValue(Liste, 0, I) < 7) { SetValue(Liste, 1, I, 900); }
                            }
                            else if (GetValue(Liste, 1, I - 1) == 901)
                            {
                                //It's  preceded by a "byte" type block
                                if (GetValue(Liste, 0, I) == 1) { SetValue(Liste, 1, I, 901); }
                            }
                        }
                        else
                        {
                            //It's not the last block
                            if (GetValue(Liste, 1, I - 1) == 901 && GetValue(Liste, 1, I + 1) == 901)
                            {
                                //Encadré par des blocs de type "octet" / Framed by "byte" type blocks
                                if (GetValue(Liste, 0, I) < 4) { SetValue(Liste, 1, I, 901); }
                            }
                            else if (GetValue(Liste, 1, I - 1) == 900 && GetValue(Liste, 1, I + 1) == 901)
                            {
                                //Preceded by "text" and followed by "byte" (If the reverse it's never interesting to change)
                                if (GetValue(Liste, 0, I) < 5) { SetValue(Liste, 1, I, 900); }
                            }
                            else if (GetValue(Liste, 1, I - 1) == 900 && GetValue(Liste, 1, I + 1) == 900)
                            {
                                //Framed by "text" type blocks
                                if (GetValue(Liste, 0, I) < 8) { SetValue(Liste, 1, I, 900); }
                            }
                        }
                    }
                }
            }
            ReGroup();
            //DebugList(Liste);
            //Maintain "text" mode only if it's earning
            for (I = 0; I <= IndexListe - 1; I++)
            {
                if (GetValue(Liste, 1, I) == 900 && I > 0)
                {
                    //It's not the first (If first, never interesting to change)
                    if (I == IndexListe - 1) // It's the last one
                    {
                        if (GetValue(Liste, 1, I - 1) == 901)
                        {
                            //It's  preceded by a "byte" type block
                            if (GetValue(Liste, 0, I) == 1) { SetValue(Liste, 1, I, 901); }
                        }
                    }
                    else
                    {
                        //It's not the last one
                        if (GetValue(Liste, 1, I - 1) == 901 && GetValue(Liste, 1, I + 1) == 901)
                        {
                            //Framed by "byte" type blocks
                            if (GetValue(Liste, 0, I) < 5) { SetValue(Liste, 1, I, 901); }
                        }
                        else if ((GetValue(Liste, 1, I - 1) == 901 && GetValue(Liste, 1, I + 1) != 901) || (GetValue(Liste, 1, I - 1) != 901 && GetValue(Liste, 1, I + 1) == 901))
                        {
                            // A "byte" block ahead or behind
                            if (GetValue(Liste, 0, I) < 3) { SetValue(Liste, 1, I, 901); }
                        }
                    }
                }
            }
            ReGroup();
            //DebugList(Liste);
            //Now we compress datas into the MCs, the MCs are stored in 3 char. in a large string : ChaineMC$
            IndexChaine = 0;
            for (I = 0; I <= IndexListe - 1; I++)
            {
                switch (GetValue(Liste, 1, I))
                {
                    case 900:
                        for (IndexListeT = 0; IndexListeT <= GetValue(Liste, 0, I); IndexListeT++)
                        {
                            char tChar;
                            tChar = Convert.ToChar(Mid(_sTringToEncode, IndexChaine + IndexListeT, 1));
                            CodeASCII = (int)Asc(tChar);
                            switch (CodeASCII)
                            {
                                case 9: //HT
                                    SetValue(ListeT, 0, IndexListeT, 12);
                                    SetValue(ListeT, 1, IndexListeT, 12);
                                    break;
                                case 10: //LF
                                    SetValue(ListeT, 0, IndexListeT, 8);
                                    SetValue(ListeT, 1, IndexListeT, 15);
                                    break;
                                case 13: //CR
                                    SetValue(ListeT, 0, IndexListeT, 12);
                                    SetValue(ListeT, 1, IndexListeT, 11);
                                    break;
                                default:
                                    SetValue(ListeT, 0, IndexListeT, Convert.ToInt32(Mid(ASCII, CodeASCII * 4 - 128, 2)));
                                    SetValue(ListeT, 1, IndexListeT, Convert.ToInt32(Mid(ASCII, CodeASCII * 4 - 126, 2)));
                                    break;
                            }
                        }
                        //Console.Clear();
                        //DebugList(ListeT);
                        CurTable = 1; //Default table
                        ChaineT = "";
                        //Datas are stored in 2 char. in the string TableT$
                        for (J = 0; J <= GetValue(Liste, 0, I) - 1; J++)
                        {
                            if ((GetValue(ListeT, 0, J) & CurTable) > 0)
                            {
                                //'The char. is in the current table
                                ChaineT += GetValue(ListeT, 1, J).ToString("00");
                            }
                            else
                            {
                                //'Obliged to change the table
                                flag = false; //True if we change the table only for 1 char.
                                if (J == GetValue(Liste, 0, I) - 1)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    if ((GetValue(ListeT, 0, J) & GetValue(ListeT, 0, J + 1)) == 0) { flag = true; } //'No common table with the next char.
                                }
                                if (flag)
                                {
                                    //We change only for 1 char., Look for a temporary switch
                                    //if (GetValue(ListeT,0, J) && 1) > 0 && CurTable = 2)
                                    if (((GetValue(ListeT, 0, J) & 1) > 0) && (CurTable == 2))
                                    {
                                        //Table 2 to 1 for 1 char. --> T_UPP
                                        ChaineT += "27" + GetValue(ListeT, 1, J).ToString("00");
                                    }
                                    //else if (GetValue(ListeT,0, J) && 8) > 0 Then
                                    else if ((GetValue(ListeT, 0, J) & 8) > 0)
                                    {
                                        //Table 1 or 2 or 4 to table 8 for 1 char. --> T_PUN
                                        ChaineT += "29" + GetValue(ListeT, 1, J).ToString("00");
                                    }
                                    else
                                    {
                                        //No temporary switch available
                                        flag = false;
                                    }
                                }
                                if (!flag) //We test again flag which is perhaps changed ! Impossible tio use ELSE statement
                                {
                                    //We must use a bi-state switch
                                    //Looking for the new table to use
                                    if (J == GetValue(Liste, 0, I) - 1)
                                    {
                                        NewTable = GetValue(ListeT, 0, J);
                                    }
                                    else
                                    {
                                        //NewTable% = IIf((ListeT%(0, J%) And ListeT%(0, J% + 1)) = 0, ListeT%(0, J%), ListeT%(0, J%) And ListeT%(0, J% + 1))
                                        NewTable = (GetValue(ListeT, 0, J) & GetValue(ListeT, 0, J + 1)) == 0 ? GetValue(ListeT, 0, J) : (GetValue(ListeT, 0, J) & GetValue(ListeT, 0, J + 1));
                                    }
                                    //Maintain the first if several tables are possible
                                    switch (NewTable)
                                    {
                                        case 3:
                                        case 5:
                                        case 7:
                                        case 9:
                                        case 11:
                                        case 13:
                                        case 15:
                                            NewTable = 1;
                                            break;
                                        case 6:
                                        case 10:
                                        case 14:
                                            NewTable = 2;
                                            break;
                                        case 12:
                                            NewTable = 4;
                                            break;
                                    }
                                    //  'Select the switch, on occasion we must use 2 switchs consecutively
                                    switch (CurTable)
                                    {
                                        case 1:
                                            switch (NewTable)
                                            {
                                                case 2:
                                                    ChaineT = ChaineT + "27";
                                                    break;
                                                case 4:
                                                    ChaineT = ChaineT + "28";
                                                    break;
                                                case 8:
                                                    ChaineT = ChaineT + "2825";
                                                    break;
                                            }
                                            break;
                                        case 2:
                                            switch (NewTable)
                                            {
                                                case 1:
                                                    ChaineT = ChaineT + "2828";
                                                    break;
                                                case 4:
                                                    ChaineT = ChaineT + "28";
                                                    break;
                                                case 8:
                                                    ChaineT = ChaineT + "2825";
                                                    break;
                                            }
                                            break;
                                        case 4:
                                            switch (NewTable)
                                            {
                                                case 1:
                                                    ChaineT = ChaineT + "28";
                                                    break;
                                                case 2:
                                                    ChaineT = ChaineT + "27";
                                                    break;
                                                case 8:
                                                    ChaineT = ChaineT + "25";
                                                    break;
                                            }
                                            break;
                                        case 8:
                                            switch (NewTable)
                                            {
                                                case 1:
                                                    ChaineT = ChaineT + "29";
                                                    break;
                                                case 2:
                                                    ChaineT = ChaineT + "2927";
                                                    break;
                                                case 4:
                                                    ChaineT = ChaineT + "2928";
                                                    break;
                                            }
                                            break;
                                    }
                                    CurTable = NewTable;
                                    ChaineT += GetValue(ListeT, 1, J).ToString("00"); //At last we add the char.
                                }
                            }
                        }
                        if (_sTringToEncode.Length % 4 > 0) { ChaineT += "29"; } //Padding if number of char. is odd
                        //Now translate the string ChaineT into CWs
                        if (I > 0) { ChaineMC += "900"; } //Set up the switch exept for the first block because "text" is the default
                        for (J = 0; J <= ChaineT.Length - 1; J += 4)
                        {
                            int t1 = Convert.ToInt32(Mid(ChaineT, J, 2)) * 30;
                            int t2 = Convert.ToInt32(Mid(ChaineT, J + 2, 2));
                            int sSum = t1 + t2;
                            ChaineMC += sSum.ToString("000");
                        }
                        break;
                    case 901:
                        //Select the switch between the 3 possible
                        if (GetValue(Liste, 0, I) == 1)
                        {
                            char tChar = Convert.ToChar(Mid(_sTringToEncode, IndexChaine, 1));
                            ChaineMC += "913" + Convert.ToInt32(Asc(tChar)).ToString("000");
                            //ChaineMC += "913" & Format(Asc(Mid$(Chaine$, IndexChaine%, 1)), "000")
                        }
                        else
                        {
                            //Select the switch for perfect multiple of 6 bytes or no
                            if (GetValue(Liste, 0, I) % 6 == 0) ChaineMC += "924";
                            else ChaineMC += "901";
                            J = 0;
                            do
                            {
                                Longueur = GetValue(Liste, 0, I) - J;
                                if (Longueur >= 6)
                                {
                                    //Take groups of 6
                                    Longueur = 6;
                                    Total = 0;
                                    for (K = 0; K == Longueur - 1; K++)
                                    {
                                        char tChar = Convert.ToChar(Mid(_sTringToEncode, IndexChaine + J + K, 1));
                                        Total += Convert.ToInt32(Math.Pow(Convert.ToDouble(Asc(tChar) * 256), Convert.ToDouble(Longueur - 1 - K)));
                                        //Total += Total + (Asc(Mid$(Chaine$, IndexChaine% + J% + K%, 1)) * 256 ^ (Longueur% - 1 - K%))
                                    }
                                    ChaineMod = Total.ToString("general number");
                                    Dummy = "";
                                    do
                                    {
                                        Diviseur = 900;
                                        Modulo();
                                        Dummy = Diviseur.ToString("000") + Dummy;
                                        ChaineMod = ChaineMult;
                                        if (ChaineMult == "") break;
                                    } while (ChaineMult != "");
                                    ChaineMC += Dummy;
                                }
                                else
                                {
                                    //If it remain a group of less than 6 bytes
                                    for (K = 0; K <= Longueur - 1; K++)
                                    {
                                        char tChar = Convert.ToChar(Mid(_sTringToEncode, IndexChaine + J + K, 1));
                                        ChaineMC += Asc(tChar).ToString("000");
                                        //ChaineMC += Format(Asc(Mid$(Chaine$, IndexChaine% + J% + K%, 1)), "000")
                                    }
                                }
                                J = J + (int)Longueur;
                            } while (J < GetValue(Liste, 0, I));
                        }
                        break;
                    case 902:
                        ChaineMC += "902";
                        J = 0;
                        while (J < GetValue(Liste, 0, I))
                        {
                            Longueur = GetValue(Liste, 0, I) - J;
                            if (Longueur > 44) Longueur = 44;
                            ChaineMod = "1" + Mid(_sTringToEncode, IndexChaine + J, (int)Longueur);
                            Dummy = "";
                            do
                            {
                                Diviseur = 900;
                                Modulo();
                                Dummy = Diviseur.ToString("000") + Dummy;
                                ChaineMod = ChaineMult;
                                if (ChaineMult == "") break;
                            } while (ChaineMult != "");
                            ChaineMC += Dummy;
                            J = J + (int)Longueur;
                        }
                        break;
                }
                IndexChaine += GetValue(Liste, 0, I);
            }
            //ChaineMC$ contain the MC list (on 3 digits) depicting the datas
            //Now we take care of the correction level
            Longueur = ChaineMC.Length / 3;
            if (_SecurityLevel < 0)
            {
                //Fixing auto. the correction level according to the standard recommendations
                if (Longueur < 41) _SecurityLevel = 2;
                else if (Longueur < 161) _SecurityLevel = 3;
                else if (Longueur < 321) _SecurityLevel = 4;
                else _SecurityLevel = 4;
            }
            //Now we take care of the number of CW per row
            Longueur = Longueur + 1 + Math.Pow(2, (_SecurityLevel + 1));
            if (_TotalColumns > 30) _TotalColumns = 30;
            if (_TotalColumns < 1)
            {
                //  //With a 3 modules high font, for getting a "square" bar code
                //  //x = nb. of col. | Width by module = 69 + 17x | Height by module = 3t / x (t is the total number of MCs)
                //  //Thus we have 69 + 17x = 3t/x <=> 17x²+69x-3t=0 - Discriminant is 69²-4*17*-3t = 4761+204t thus x=SQR(discr.)-69/2*17
                //  //_TotalColumns = (System.Math.sqrt(204# * Longueur% + 4761) - 69) / (34 / 1.3)   //1.3 = balancing factor determined at a guess after tests
                double t1 = System.Math.Sqrt(204 * Longueur + 4761) - 69;
                double t2 = 34 / 1.3;
                _TotalColumns = Convert.ToInt32(t1 / t2);   //1.3 = balancing factor determined at a guess after tests
                if (_TotalColumns == 0) _TotalColumns = 1;
            }
            do
            {
                //Calculation of the total number of CW with the padding
                Longueur = ChaineMC.Length / 3 + 1 + Math.Pow(2, (_SecurityLevel + 1));
                int Var = Longueur % _TotalColumns > 0 ? 1 : 0;
                Longueur = (Longueur / _TotalColumns + Var) * _TotalColumns;
                if (Longueur < 929) break;
                //'We must reduce security level
                _SecurityLevel--;
                _ErrorCode = 10;
            } while (_SecurityLevel > 0);

            if (Longueur > 928)
            {
                _ErrorCode = 2;
                return "";
            }
            if (Longueur / _TotalColumns > 90)
            {
                _ErrorCode = 3;
                return "";
            }

            //Padding calculation
            Longueur = ChaineMC.Length / 3 + 1 + Math.Pow(2, (_SecurityLevel + 1));
            I = 0;
            if (Longueur / _TotalColumns < 3) I = Convert.ToInt32(_TotalColumns * 3 - Longueur);   //A bar code must have at least 3 row
            else if (Longueur % _TotalColumns > 0) I = Convert.ToInt32(_TotalColumns - (Longueur % _TotalColumns));
            //We add the padding
            while (I > 0)
            {
                ChaineMC += "900";
                I--;
            }
            //We add the length descriptor
            ChaineMC = (ChaineMC.Length / 3 + 1).ToString("000") + ChaineMC;
            //Now we take care of the Reed Solomon codes
            Longueur = ChaineMC.Length / 3;
            K = Convert.ToInt32(Math.Pow(2, (_SecurityLevel + 1)));
            int[] MCcorrection = new int[K];
            Total = 0;
            for (I = 0; I <= Longueur - 1; I++)
            {
                int t1 = Convert.ToInt32(ChaineMC.Substring(I * 3, 3));
                int t2 = t1 + Convert.ToInt32(MCcorrection[K - 1]);
                Total = t2 % 929;
                for (J = K - 1; J >= 0; J--)
                {
                    if (J == 0)
                    {
                        int u0, u1, u2, u3, u4;
                        u0 = Convert.ToInt32(CoefRS[_SecurityLevel].Substring(J * 3, 3));
                        u1 = (Total * u0) % 929;
                        u2 = 929;
                        u3 = u2 - u1;
                        u4 = (u3 % 929);
                        MCcorrection[J] = u4;
                    }
                    else
                    {
                        int u0, u1, u2, u3, u4;
                        u0 = Convert.ToInt32(CoefRS[_SecurityLevel].Substring(J * 3, 3));
                        u1 = (Total * u0) % 929;
                        u2 = MCcorrection[J - 1] + 929;
                        u3 = u2 - u1;
                        u4 = (u3 % 929);
                        MCcorrection[J] = u4;
                    }
                }
            }
            for (J = 0; J <= K - 1; J++)
            {
                if (MCcorrection[J] != 0) MCcorrection[J] = 929 - MCcorrection[J];
            }
            //We add theses codes to the string
            for (I = K - 1; I >= 0; I--) { ChaineMC += MCcorrection[I].ToString("000"); }
            //The CW string is finished
            //Calculation of parameters for the left and right side CWs
            C1 = (ChaineMC.Length / 3 / _TotalColumns - 1) / 3; /////  OJO
            C2 = _SecurityLevel * 3 + (ChaineMC.Length / 3 / _TotalColumns - 1) % 3;
            C3 = _TotalColumns - 1;
            //We encode each row
            for (I = 0; I <= ChaineMC.Length / 3 / _TotalColumns - 1; I++)
            {
                Dummy = ChaineMC.Substring(I * (int)_TotalColumns * 3, (int)_TotalColumns * 3);
                K = (I / 3) * 30;///// OJO
                switch (I % 3)
                {
                    case 0:
                        Dummy = (K + C1).ToString("000") + Dummy + (K + C3).ToString("000");
                        break;
                    case 1:
                        Dummy = (K + C2).ToString("000") + Dummy + (K + C1).ToString("000");
                        break;
                    case 2:
                        Dummy = (K + C3).ToString("000") + Dummy + (K + C2).ToString("000");
                        break;
                }

                StringEncoded += "+*"; //Commencer par car. de start et séparateur / Start with a start char. and a separator
                for (J = 0; J < Dummy.Length / 3; J++)
                {
                    int i1 = Convert.ToInt32(Dummy.Substring(J * 3, 3));
                    StringEncoded += CodageMC[I % 3].Substring(i1 * 3, 3) + "*";
                }
                StringEncoded += "-" + Environment.NewLine;  //Add a stop char. and a CRLF
            }
            return StringEncoded;
        }

        private void QuelMode()
        {
            char tChar;
            if (_sTringToEncode.Length > IndexChaine) tChar = Convert.ToChar(_sTringToEncode.Substring(IndexChaine, 1));
            else tChar = Convert.ToChar(_sTringToEncode.Substring(IndexChaine - 1, 1));
            //tChar = Convert.ToChar(_sTringToEncode.Substring(IndexChaine, 1));
            int iChar = (int)Asc(tChar);
            if (iChar >= 48 && iChar <= 57)
            {
                Mode = 902;
            }
            else if ((iChar == 9 || iChar == 10 || iChar == 13) || (iChar >= 32 && iChar <= 126))
            {
                Mode = 900;
            }
            else
            {
                Mode = 901;
            }
        }

        private void ReGroup()
        {
            //Bring together same type blocks
            if (IndexListe > 1)
            {
                int I = 1;
                int J = 0;
                while (I < IndexListe)
                {
                    if (GetValue(Liste, 1, I - 1) == GetValue(Liste, 1, I))
                    {
                        //Bringing together
                        SetValue(Liste, 0, I - 1, GetValue(Liste, 0, I - 1) + GetValue(Liste, 0, I));
                        J = I + 1;
                        //Decrease the list
                        while (J < IndexListe)
                        {
                            SetValue(Liste, 0, J - 1, GetValue(Liste, 0, J));
                            SetValue(Liste, 1, J - 1, GetValue(Liste, 1, J));
                            J++;
                        }
                        IndexListe--;
                        I--;
                    }
                    I++;
                }
            }
        }

        private void Modulo()
        {
            //ChaineMod depict a very large number having more than 9 digits
            //Diviseur is the divisor, contain the result after return
            //ChaineMult contain after return the result of the integer division
            ChaineMult = "";
            int Nombre = 0;
            while (ChaineMod != "")
            {
                Nombre = Nombre * 10 + Convert.ToInt32(Left(ChaineMod, 1)); //Put down a digit
                ChaineMod = Mid(ChaineMod, 1);
                if (Nombre < Diviseur) { if (ChaineMult != "") ChaineMult += "0"; }
                else { ChaineMult += Nombre / Diviseur; }
                Nombre = Nombre % Diviseur; //get the remainder
            }
            Diviseur = Nombre;
        }
        #endregion
        #region Private Tools
        private int MNI(string num)
        {
            return Convert.ToInt16(num);
        }
        private byte Asc(char src)
        {
            return (System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(src + "")[0]);
        }

        private char Chr(byte src)
        {
            return (System.Text.Encoding.GetEncoding("iso-8859-1").GetChars(new byte[] { src })[0]);
        }

        private string Left(string param, int length)
        {
            string result = param.Substring(0, length);
            return result;
        }

        private static string Right(string param, int length)
        {
            int value = param.Length - length;
            string result = param.Substring(value, length);
            return result;
        }

        private static string Mid(string param, int startIndex, int length)
        {
            string result;
            if (param.Length == startIndex) result = param.Substring(startIndex - length, length);
            else result = param.Substring(startIndex, length);
            return result;
        }

        private static string Mid(string param, int startIndex)
        {
            string result = param.Substring(startIndex);
            return result;
        }

        private string GetAdjustedString(string src, string originalSrc, float adjustHeight, Font font, StringFormat format)
        {
            SizeF sz = (Graphics.FromImage(new Bitmap(500, 200)).MeasureString(src, font, Point.Empty, format));
            int lastIndexOfSpace = src.LastIndexOf(" ");
            if (sz.Height > adjustHeight && lastIndexOfSpace > 0)
            {
                return GetAdjustedString(src.Substring(0, lastIndexOfSpace), originalSrc, adjustHeight, font, format);
            }
            else
            {
                if (originalSrc.Length > src.Length)
                {
                    originalSrc = originalSrc.Substring(src.Length + 1, originalSrc.Length - src.Length - 1);
                    return src + "\r\n" + GetAdjustedString(originalSrc, originalSrc, adjustHeight, font, format);
                }
                else
                {
                    return src;
                }
            }
        }
        #endregion
        #region Arrays Handlers
        public int this[int x, int y]
        {
            get { return GetValue(Liste, x, y); }
            set { SetValue(Liste, x, y, value); }
        }

        private void SetValue(IList<IntArray> List, int x, int y, int value)
        {
            IntArray itm = null;
            foreach (IntArray item in List)
            {
                if (item.X == x && item.Y == y)
                {
                    itm = item;
                    break;
                }
            }

            if (itm == null)
            {
                IntArray item = new IntArray();
                item.X = x;
                item.Y = y;
                item.Value = value;
                List.Add(item);
            }
            else
            {
                itm.Value = value;
            }
        }
        private void SetValue(IList<StrArray> List, int x, int y, string value)
        {
            StrArray itm = null;
            foreach (StrArray item in List)
            {
                if (item.X == x && item.Y == y)
                {
                    itm = item;
                    break;
                }
            }

            if (itm == null)
            {
                StrArray item = new StrArray();
                item.X = x;
                item.Y = y;
                item.Value = value;
                List.Add(item);
            }
            else
            {
                itm.Value = value;
            }
        }
        private int GetValue(IEnumerable<IntArray> List, int x, int y)
        {
            int _value = 0;
            foreach (IntArray item in List)
            {
                if (item.X == x && item.Y == y)
                {
                    _value = item.Value;
                    break;
                }
            }
            return _value;
        }
        private string GetValue(IEnumerable<StrArray> List, int x, int y)
        {
            string _value = string.Empty;
            foreach (StrArray item in List)
            {
                if (item.X == x && item.Y == y)
                {
                    _value = item.Value;
                    break;
                }
            }
            return _value;
        }
        #endregion
        #region Class IntArray
        private class IntArray
        {
            private int _x;
            private int _y;
            private int _value;

            public int X
            {
                get { return _x; }
                set { _x = value; }
            }
            public int Y
            {
                get { return _y; }
                set { _y = value; }
            }
            public int Value
            {
                get { return _value; }
                set { _value = value; }
            }
        }
        #endregion
        #region Class StringArray
        private class StrArray
        {
            private int _x;
            private int _y;
            private string _value;

            public int X
            {
                get { return _x; }
                set { _x = value; }
            }
            public int Y
            {
                get { return _y; }
                set { _y = value; }
            }
            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }
        }
        #endregion
        #region IDisposable Methods
        // Track whether Dispose has been called.
        private bool disposed = false;
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (m_PFC != null) m_PFC.Dispose();
                    Liste.Clear();
                    Liste = null;
                    ListeT.Clear();
                    ListeT = null;
                    // Note disposing has been done.
                    disposed = true;
                }
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                //CloseHandle(m_hFont);
                m_hFont = IntPtr.Zero;
                // If disposing is false,
                // only the following code is executed.
                // Note disposing has been done.
                disposed = true;
            }
        }

        // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        #endregion
    }
}
