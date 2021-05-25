using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Text;
using System.Drawing.Imaging;
using System.Linq;

namespace TEXView
{
    class DTXReader : ParserBase
    {
        private DTXFile _DTXFile;
        public TEXView.DTXFile DTXFile
        {
            get { return _DTXFile; }
            set { _DTXFile = value; }
        }
        public DTXReader(DTXFile pD2MFile)
        {
            DTXFile = pD2MFile;
        }

        private T ReadImageDefine<T>(Stream _stream)
        {
            T _ObjStruct = (T)Activator.CreateInstance(typeof(T));
            _ObjStruct = ReadStreamStruct<T>(_stream);
            return _ObjStruct;
        }

        private ImgInfo ReadImageT3(Stream _stream,ref ImgInfo _Info)
        {
            _Info.ChunkList = new List<Object>();
            Int32 ChunkCount = ReadInt32(_stream);
            _Info.Info.ChunkCount = ChunkCount;

            if (ChunkCount - 1 >= 0)
            {
                for (int ChkIdx = 0; ChkIdx < ChunkCount; ++ChkIdx)
                {
                    ChunkInfoT3 nImg = ReadStreamStruct<ChunkInfoT3>(_stream);
                    int _Row = nImg.Row;
                    int _Pos = nImg.Pos;
                    _Info.ChunkList.Add(nImg);
                }

            }
            return _Info;
        }

        private ImgInfo ReadImage(Stream _stream)
        {
            ImgInfo _Info = new ImgInfo();
            _Info.Info = new ImgHeader();
            _Info.ChunkList = new List<Object>();

            if (DTXFile.Header.Version < 7)
            {
                _Info.Info._Short1 = ReadInt16(_stream);
                _Info.Info._Short2 = ReadInt16(_stream);
                _Info.Info._Short3 = ReadInt16(_stream);
                _Info.Info._Short4 = ReadInt16(_stream);
            }
            if (DTXFile.Header.Version == 7 )
            {
               Int32 NameSize = ReadInt32(_stream);
                Byte[] Raw = new Byte[NameSize];
                ReadBytes(_stream, (uint)NameSize, ref Raw);
                _Info.Name = ASCIIEncoding.ASCII.GetString(Raw);

                _Info.Info._Short1 = ReadInt16(_stream);
                _Info.Info._Short2 = ReadInt16(_stream);
                _Info.Info._Short3 = ReadInt16(_stream);
                _Info.Info._Short4 = ReadInt16(_stream);

                _Info.Info._SubStrSize = ReadInt32(_stream);

                for (int j = 0; j < _Info.Info._SubStrSize; ++j)
                {
                    Int32 tNameSize = ReadInt32(_stream);
                    Byte[] Raw2 = new Byte[tNameSize];
                    ReadBytes(_stream, (uint)tNameSize, ref Raw2);

                    int A = ReadInt32(_stream);
                    int B = ReadInt32(_stream);
                }

                _Info.Info._Short5 = ReadInt16(_stream);
                _Info.Info._Short6 = ReadInt16(_stream);
                _Info.Info._Short7 = ReadInt16(_stream);
                _Info.Info._Short8 = ReadInt16(_stream);
            }
            else if (DTXFile.Header.Version == 8)
            {
                Int32 NameSize = ReadInt32(_stream);
                Byte[] Raw = new Byte[NameSize];
                ReadBytes(_stream, (uint)NameSize, ref Raw);
                _Info.Name = ASCIIEncoding.ASCII.GetString(Raw);

                _Info.Info._Short1 = ReadInt16(_stream);
                _Info.Info._Short2 = ReadInt16(_stream);
                _Info.Info._Short3 = ReadInt16(_stream);
                _Info.Info._Short4 = ReadInt16(_stream);
                _Info.Info._Short5 = ReadInt16(_stream);
                _Info.Info._Short6 = ReadInt16(_stream);

                _Info.Info.Float1 = ReadFloat(_stream);
                _Info.Info.Float2 = ReadFloat(_stream);
                _Info.Info._SubStrSize = ReadInt32(_stream);

                for (int j = 0; j < _Info.Info._SubStrSize; ++j)
                {
                   Int32 tNameSize = ReadInt32(_stream);
                    Byte[] Raw2 = new Byte[tNameSize];
                    ReadBytes(_stream, (uint)tNameSize, ref Raw2);

                    int A = ReadInt32(_stream);
                    int B = ReadInt32(_stream);
                    int C = ReadInt32(_stream);
                    float D = ReadFloat(_stream);
                    float E = ReadFloat(_stream);
                }

                _Info.Info._Short13 = ReadInt16(_stream);
                _Info.Info._Short14 = ReadInt16(_stream);
                _Info.Info._Short15 = ReadInt16(_stream);
                _Info.Info._Short16 = ReadInt16(_stream);
            }

            _Info.Info._2D = ReadInt32(_stream);
            _Info.Info._After2D = ReadInt16(_stream);

            _Info.Info.Width = ReadInt32(_stream);
            _Info.Info.Height = ReadInt32(_stream);
            _Info.Info.ChunkCount = ReadInt32(_stream);
            _Info.Info.X2Size = ReadInt32(_stream);

            for (int ChkIdx = 0; ChkIdx < _Info.Info.ChunkCount; ++ChkIdx)
            {
                ChunkInfo nImg = new ChunkInfo();
                Read<ChunkInfo>(_stream, ref nImg);
                _Info.ChunkList.Add(nImg);
            }

            return _Info;
        }

        private bool BuildImage()
        {
            MemoryStream ms = null;
            if (DTXFile.Header.ImgType != 3)
            {
                ms = new MemoryStream(DTXFile.DataStorage);
            }

            for (int imgidx = 0; imgidx < DTXFile.Header.ImgCount; ++imgidx)
                              {
                ImgInfo _i = DTXFile.ImgLists[imgidx];

                if(_i.Info.Width == 0 || _i.Info.Height == 0 )
                {
                    //!--Error
                    _i.Img = new System.Drawing.Bitmap(1, 1, PixelFormat.Format24bppRgb);
                    DTXFile.ImgLists[imgidx] = _i;
                    continue;
                }

                if(DTXFile.Header.ImgType == 0)
                {
                    _i.Img = BitmapExtensions.BitmapSourceFromArray(DTXFile.DataStorage, _i.Info.Width, _i.Info.Height, 16);
                }
                else
                {
                    Byte[] ImgD;
                    if (DTXFile.Header.ImgType == 1 && DTXFile.Header.Version != 8 && DTXFile.Header.Version != 7)
                    {
                        ImgD = Enumerable.Repeat((byte)DTXFile.FuchsiaIdx, _i.Info.Width * _i.Info.Height * (DTXFile.Header.ImgType * 2)).ToArray();
                    }
                    else
                    {
                        ImgD = Enumerable.Repeat((byte)DTXFile.FuchsiaIdx, _i.Info.Width * _i.Info.Height * DTXFile.Header.ImgType).ToArray();
                    }
                    System.Drawing.Bitmap ImgP = new System.Drawing.Bitmap(_i.Info.Width, _i.Info.Height, PixelFormat.Format24bppRgb);
                    for (int ChkIdx = 0; ChkIdx < _i.Info.ChunkCount; ++ChkIdx)
                    {
                        if (DTXFile.Header.ImgType == 1)
                        {
                            ChunkInfo nChk = (ChunkInfo)_i.ChunkList[ChkIdx];
                            int _Row = nChk.Row;
                            int _Pos = nChk.Pos;

                            Byte[] Raw = new Byte[nChk.ChunkSize * 2];
                            ReadBytes(ms, (uint)nChk.ChunkSize * 2, ref Raw);
                            Raw.CopyTo(ImgD, (_Row * 2) * _i.Info.Width + (_Pos * 2));
                        }
                        
                        else if (DTXFile.Header.ImgType != 3)
                        {
                            ChunkInfo nChk = (ChunkInfo)_i.ChunkList[ChkIdx];
                            int _Row = nChk.Row;
                            int _Pos = nChk.Pos;

                            Byte[] Raw = new Byte[nChk.ChunkSize];
                            ReadBytes(ms, (uint)nChk.ChunkSize, ref Raw);

                            Raw.CopyTo(ImgD, _Row * _i.Info.Width + _Pos);
                        }
                        else
                        {
                            ChunkInfoT3 nChk = (ChunkInfoT3)_i.ChunkList[ChkIdx];
                            int _Row = nChk.Row;
                            int _Pos = nChk.Pos;

                            int Bidx = 0;
                            for (int x = nChk.Pos; x < nChk.Pos + nChk.Pixel; ++x)
                            {
                                int _Y = _Row - _i.Info.GlobalY;
                                int _X = x - _i.Info.GlobalX;
                                Byte R = nChk.Raw.Array[Bidx++];
                                Byte G = nChk.Raw.Array[Bidx++];
                                Byte B = nChk.Raw.Array[Bidx++];
                                ImgP.SetPixel(_X, _Y, Color.FromArgb(R, G, B));
                            }
                            //Trace.WriteLine(String.Format("{0}\t\t{1}\t\t{2}\t\t{3} ", nChk.Row, nChk.Pos, nChk.Pixel, nChk.Pos + nChk.Pixel));
                        }
                    }

                    if (DTXFile.Header.ImgType == 1)
                    {
                        Bitmap _bp = null;
                        if (DTXFile.Palette != null)
                        {
                            _bp = BitmapExtensions.BitmapSourceFromArrayIndex(ImgD, _i.Info.Width, _i.Info.Height, DTXFile.Header.ImgType * 16, DTXFile.Palette);
                        }
                        else
                        {
                            _bp = BitmapExtensions.BitmapSourceFromArray(ImgD, _i.Info.Width, _i.Info.Height, DTXFile.Header.ImgType * 16);
                        }

                        Bitmap bm = new Bitmap(_bp);
                        _i.Img = bm;
                        _bp.Dispose();
                    }
                    else if (DTXFile.Header.ImgType != 3)
                    {
                        Bitmap _bp = null;
                        if (DTXFile.Palette != null)
                        {
                            _bp = BitmapExtensions.BitmapSourceFromArrayIndex(ImgD, _i.Info.Width, _i.Info.Height, DTXFile.Header.ImgType * 8, DTXFile.Palette);
                        }
                        else
                        {
                            _bp = BitmapExtensions.BitmapSourceFromArray(ImgD, _i.Info.Width, _i.Info.Height, DTXFile.Header.ImgType * 8);
                        }

                        Bitmap bm = new Bitmap(_bp);
                        _i.Img = bm;
                        _bp.Dispose();
                    }
                    else
                    {
                        _i.Img = ImgP;
                    }
                }

                DTXFile.ImgLists[imgidx] = _i;
            }
            ms.Dispose();
            return true;
        }

        public bool Open(Stream _stream, bool writeToFile = false, string filePath = "")
        {
            DTXFile.Header = new DTXHeader();
            DTXFile.ImgLists = new List<ImgInfo>();
            DTXFile.ImageDefineLists = new List<Object>();

            BinaryReader _br = new BinaryReader(_stream);
            //UInt32 _ZipSize = _br.ReadUInt32(); //!--Test
            Int16 Hzip = _br.ReadInt16();
            MemoryStream _decompress;
            DecompressData(_stream, out _decompress);
            
            _stream = _decompress;

            if (writeToFile)
            {
                Stream dtxWriter = new FileStream(filePath, FileMode.Create,
                                                             FileAccess.Write, FileShare.Write);
                //dtxWriter.Write(_stream., 0, _stream.Length);
                _stream.CopyTo(dtxWriter);
                _stream.Position = 0;
                dtxWriter.Flush();
                dtxWriter.Dispose();
            }

            Read<DTXHeader>(_stream, ref DTXFile.Header);

            try
            {
                DTXFile.FuchsiaIdx = 0;
                if (DTXFile.Header.ImgType == 2)
                {
                    Byte[] PaletteData = new Byte[512];
                    ReadBytes(_stream, 512, ref PaletteData);

                    List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
                    Bitmap _bitmapPal = BitmapExtensions.BitmapSourceFromArray(PaletteData, 16, 16, 16);
                    Bitmap bm2 = new Bitmap(_bitmapPal);
                    bool ff = false;
                    for (int i = 0; i < 16; ++i)
                    {
                        for (int j = 0; j < 16; ++j)
                        {
                            Color c = bm2.GetPixel(j, i);
                            if (ff == false)
                            {
                                if (c.ToArgb() == Color.Fuchsia.ToArgb())
                                {
                                    ff = true;
                                }
                                else
                                {
                                    DTXFile.FuchsiaIdx++;
                                }
                            }

                            colors.Add(System.Windows.Media.Color.FromRgb(c.R, c.G, c.B));
                        }
                    }
                    DTXFile.Palette = new BitmapPalette(colors);
                    //bm2.Save("E:\\TalesWeaver Tools\\jtw dump" + string.Format("\\{0}.bmp", DateTimeOffset.Now.ToUnixTimeMilliseconds()), ImageFormat.Bmp);
                }

                if (DTXFile.Header.ImgType ==0)
                {
                    for (int imgidx = 0; imgidx < DTXFile.Header.ImgCount; ++imgidx)
                    {
                        Object _ani = null;
                        if (DTXFile.Header.Version == 5 || DTXFile.Header.Version == 6)
                        {
                            _ani = ReadImageDefine<ImageDefineT50>(_stream);
                        }
                        else if (DTXFile.Header.Version == 7)
                        {
                            _ani = ReadImageDefine<ImageDefineT70>(_stream);
                        }

                        DTXFile.ImageDefineLists.Add(_ani);
                    }
                    //!--Save big image
                    Int32 _Width = ReadInt32(_stream);
                    Int32 _Height = ReadInt32(_stream);

                    ImgInfo _Info = new ImgInfo();
                    _Info.Info = new ImgHeader();
                    _Info.Info.Width = _Width;
                    _Info.Info.Height = _Height;
                    DTXFile.ImgLists.Add(_Info);
                    DTXFile.DataStorageSize = _Width * _Height * 2;
                    DTXFile.DataStorage = new Byte[DTXFile.DataStorageSize];
                    ReadBytes(_stream, (UInt32)DTXFile.DataStorageSize, ref DTXFile.DataStorage);

                    //!--This type all image as big image.
                    DTXFile.Header.ImgCount = 1;
                }
                else if (DTXFile.Header.ImgType == 3)
                {
                    for (int imgidx = 0; imgidx < DTXFile.Header.ImgCount; ++imgidx)
                    {
                        Object _ani = null;
                        if (DTXFile.Header.Version == 7)
                        {
                            _ani = ReadImageDefine<ImageDefineT73>(_stream);
                        }
                        else //!--8
                        {
                            _ani = ReadImageDefine<ImageDefineT83>(_stream);
                        }
                        
                        DTXFile.ImageDefineLists.Add(_ani);
                        ImgInfo _Info = new ImgInfo();
                        _Info.Info = new ImgHeader();
                        if (DTXFile.Header.Version == 7)
                        {
                            _Info.Info.Width = ((ImageDefineT73)_ani).Width;
                            _Info.Info.Height = ((ImageDefineT73)_ani).Height;
                            _Info.Info.GlobalY = ((ImageDefineT73)_ani).GlobalY;
                            _Info.Info.GlobalX = ((ImageDefineT73)_ani).GlobalX;
                            _Info.Name = ((ImageDefineT73)_ani).Name;
                        }
                        else
                        {
                            _Info.Info.Width = ((ImageDefineT83)_ani).Width;
                            _Info.Info.Height = ((ImageDefineT83)_ani).Height;
                            _Info.Info.GlobalY = ((ImageDefineT83)_ani).GlobalY;
                            _Info.Info.GlobalX = ((ImageDefineT83)_ani).GlobalX;
                            _Info.Name = ((ImageDefineT83)_ani).Name;
                        }
                        DTXFile.ImgLists.Add(_Info);
                    }

                    Int32 _Splitter = ReadInt32(_stream);
                    Int32 _UnknowByte = ReadByte(_stream);
                    Int32 _UnknowInt = ReadInt32(_stream);
                    Byte _UnknowByte2 = ReadByte(_stream);
                    Int16 _UnknowShort = ReadInt16(_stream);

                    for (int imgidx = 0; imgidx < DTXFile.Header.ImgCount; ++imgidx)
                    {
                        ImgInfo _Info = DTXFile.ImgLists[imgidx];
                        DTXFile.ImgLists[imgidx] = ReadImageT3(_stream,ref _Info);
                    }
                }
                else
                {
                    if (DTXFile.Header.ImgCount - 1 >= 0)
                    {
                        for (int imgidx = 0; imgidx < DTXFile.Header.ImgCount; ++imgidx)
                        {
                            DTXFile.ImgLists.Add(ReadImage(_stream));
                        }
                    }

                    DTXFile.DataStorageSize = ReadInt32(_stream);
                    DTXFile.DataStorage = new Byte[DTXFile.DataStorageSize];
                    ReadBytes(_stream, (UInt32)DTXFile.DataStorageSize, ref DTXFile.DataStorage);
                }

                BuildImage();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return false;
            }
           
            return true;
        }

        public Stream Decompress(Stream _stream) {

            DTXFile.Header = new DTXHeader();
            DTXFile.ImgLists = new List<ImgInfo>();
            DTXFile.ImageDefineLists = new List<Object>();

            BinaryReader _br = new BinaryReader(_stream);
            //UInt32 _ZipSize = _br.ReadUInt32(); //!--Test
            Int16 Hzip = _br.ReadInt16();
            MemoryStream _decompress;
            DecompressData(_stream, out _decompress);

            return _decompress;
        }
    }
}

