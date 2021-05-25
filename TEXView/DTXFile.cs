using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Drawing;

namespace TEXView
{
    class DTXFile
    {
        DTXReader _DTXReader;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        List<ImgInfo> _ImgLists;
        public System.Collections.Generic.List<TEXView.ImgInfo> ImgLists
        {
            get { return _ImgLists; }
            set { _ImgLists = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        List<Object> _ImageDefineLists;
        public System.Collections.Generic.List<Object> ImageDefineLists
        {
            get { return _ImageDefineLists; }
            set { _ImageDefineLists = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        BitmapPalette _Palette;
        public System.Windows.Media.Imaging.BitmapPalette Palette
        {
            get { return _Palette; }
            set { _Palette = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 _DataStorageSize;
        public System.Int32 DataStorageSize
        {
            get { return _DataStorageSize; }
            set { _DataStorageSize = value; }
        }

        public Byte[] DataStorage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 _FuchsiaIdx;
        public System.Int32 FuchsiaIdx
        {
            get { return _FuchsiaIdx; }
            set { _FuchsiaIdx = value; }
        }

        public DTXHeader Header;

        public DTXFile()
        {

        }

        public bool ReadFromFile(string path)
        {
            Stream _D2MStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _DTXReader = new DTXReader(this);
            bool _ret = _DTXReader.Open(_D2MStreamSource);
            if (_ret)
            {
                FilePath = path;
            }

            Trace.WriteLine(string.Format("{0} Ver:{1} Type:{2}", path, Header.Version, Header.ImgType));

            _D2MStreamSource.Dispose();
            return _ret;
        }

        public bool ReadFromStream(Stream _D2MStreamSource, bool writeToFile = false, string filePath = "")
        {
            _DTXReader = new DTXReader(this);
            bool _ret = _DTXReader.Open(_D2MStreamSource, writeToFile, filePath);
            _D2MStreamSource.Dispose();
            return _ret;
        }

        public Stream ReadFromStreamAndDecompress(Stream _D2MStreamSource)
        {
            _DTXReader = new DTXReader(this);
            Stream _ret = _DTXReader.Decompress(_D2MStreamSource);
            _D2MStreamSource.Dispose();
            return _ret;
        }
    }
}
