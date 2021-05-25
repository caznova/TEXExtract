using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Collections;

namespace TEXView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        //horrible and lazy error checking. Sorry.
        if (txtDestFolder.Text == string.Empty || 
              (!rbDumpDTX.Checked && !rbDumpDTXZipFiles.Checked && !rbImagesSeparateFolder.Checked && !rbImagesSingleFolder.Checked)) {
                return;
        }
            string destFolderPath = txtDestFolder.Text;
            bool dumpImagesSeparateFolders = rbImagesSeparateFolder.Checked;
            bool dumpImagesSingleFolder = rbImagesSingleFolder.Checked;
            bool dumpDTXFiles = rbDumpDTX.Checked;
            bool dumpDTXZipFiles = rbDumpDTXZipFiles.Checked;

            Hashtable _hpacks = new Hashtable();
            OpenFileDialog _openDlg = new OpenFileDialog();
            _openDlg.Filter = "TEX.DAT|TEX.DAT|All Files (*.*)|*.*";
            _openDlg.FilterIndex = 1;
            DialogResult result = _openDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string _directory = Path.GetDirectoryName(_openDlg.FileName);
                Stream _datsource = new FileStream(_openDlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader br = new BinaryReader(_datsource);
                ParserBase pb = new ParserBase();
                Byte[] _header = br.ReadBytes(24); //!--don't care
                Int32 _filenum = 0;
                do
                {
                    DatFileInfo d = pb.ReadStreamStruct<DatFileInfo>(_datsource);
                    if( !_hpacks.ContainsKey( d.PackID ) )
                    {
                        Stream _sdatsource = new FileStream(_directory  + string.Format("\\TEX{0}.DAT", d.PackID), FileMode.Open, FileAccess.Read, FileShare.Read);
                        _hpacks[d.PackID] = _sdatsource;
                    }

                    Stream _sf = (Stream)_hpacks[d.PackID];
                    _sf.Seek(d.Pos, SeekOrigin.Begin);
                    Byte[] _filedata = new Byte[d.CompressSize];
                    //_sf.Read(_filedata, 0, d.CompressSize);
                    string DTXZipOutPath = destFolderPath + "\\DTXZipOut";
                    
                    int fullSize = d.CompressSize + 4;
                    int uncompSize = d.UnCompressSize;
                    _filedata = new Byte[fullSize];

                    if (d.CompressSize > 0)
                    {
                        byte[] b = new byte[4];
                        b[0] = (byte)(((uint)uncompSize) & 0xFF);
                        b[1] = (byte)(((uint)uncompSize >> 8) & 0xFF);
                        b[2] = (byte)(((uint)uncompSize >> 16) & 0xFF);
                        b[3] = (byte)(((uint)uncompSize >> 24) & 0xFF);

                        _filedata[0] = b[0];
                        _filedata[1] = b[1];
                        _filedata[2] = b[2];
                        _filedata[3] = b[3];
                        _sf.Read(_filedata, 4, d.CompressSize);

                        if (dumpDTXZipFiles)
                        {
                            if (!Directory.Exists(DTXZipOutPath))
                            {
                                Directory.CreateDirectory(DTXZipOutPath);
                            }
                            Stream outStream = new FileStream(DTXZipOutPath + "\\" + _filenum.ToString("D5") + ".dtx.zip", FileMode.Create,
                                                              FileAccess.Write, FileShare.Write);
                            outStream.Write(_filedata, 0, d.CompressSize + 4);
                            outStream.Flush();
                            outStream.Dispose();
                        }
                        Console.WriteLine(_filenum.ToString("D5"));
                    }

                    Byte[] _filedata2 = _filedata.Skip(4).ToArray();
                    Byte[] _filedata3 = _filedata.Skip(4).ToArray();
                    DTXFile _f = new DTXFile();

                    string _dirname = string.Format("\\DTX_{0}", _filenum);
                    string ImgDir = destFolderPath + _dirname;
                    
                    MemoryStream _mf = new MemoryStream(_filedata2);
                    MemoryStream _mf2 = new MemoryStream(_filedata3);

                    string DTXOutPath = destFolderPath + "\\DTXOut";
                    if (!Directory.Exists(DTXOutPath))
                    {
                        Directory.CreateDirectory(DTXOutPath);
                    }
                    DTXFile _dtxout = new DTXFile();
                    Stream dtxOutStream = _dtxout.ReadFromStreamAndDecompress(_mf);
                    Byte[] dtxBytes = new Byte[dtxOutStream.Length];
                    dtxOutStream.Read(dtxBytes, 0, (int)dtxOutStream.Length);
                    dtxOutStream.Dispose();
                    if (dumpDTXFiles)
                    {
                        Stream dtxWriter = new FileStream(DTXOutPath + "\\" + _filenum.ToString("D5") + ".dtx", FileMode.Create,
                                                             FileAccess.Write, FileShare.Write);
                        dtxWriter.Write(dtxBytes, 0, dtxBytes.Length);
                        dtxWriter.Flush();
                        dtxWriter.Dispose();
                    }
                    //Console.WriteLine(_filenum.ToString("D4") + ".dtx");

                    if (dumpImagesSeparateFolders || dumpImagesSingleFolder) {
                        if (_f.ReadFromStream(_mf2))
                        {
                            string SingleFolderOutPath = destFolderPath + "\\SingleFolderOutput";                            
                            for (int imgidx = 0; imgidx < _f.ImgLists.Count; ++imgidx)
                            {
                                ImgInfo _i = _f.ImgLists[imgidx];
                                if (dumpImagesSeparateFolders)
                                {
                                    if (!Directory.Exists(ImgDir))
                                    {
                                        Directory.CreateDirectory(ImgDir);
                                    }
                                    _i.Img.Save(ImgDir + string.Format("\\{0}.png", imgidx), ImageFormat.Png);
                                }
                                if (dumpImagesSingleFolder)
                                {
                                    if (!Directory.Exists(SingleFolderOutPath))
                                    {
                                        Directory.CreateDirectory(SingleFolderOutPath);
                                    }
                                    _i.Img.Save(SingleFolderOutPath + "\\" + _filenum.ToString("D5") + "-" + string.Format("{0}.png", imgidx), ImageFormat.Png);
                                }
                                _i.Img.Dispose();
                            }
                            _f.ImgLists.Clear();
                        }
                        //Console.WriteLine(string.Format("Filenum {0} Ver:{1} Type:{2}", _filenum++,_f.Header.Version, _f.Header.ImgType));
                    }
                    _filenum++;
                    _mf.Dispose();
                    _mf2.Dispose();
                }
                while (_datsource.Position < _datsource.Length);

                br.Close();
                _datsource.Close();
                foreach ( Stream spack in _hpacks.Values)
                {
                    spack.Close();
                }
            }
            _hpacks.Clear();
            _openDlg.Dispose();

            //!--Test
            //var files = Directory.GetFiles("C:\\TEX", "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".zip"));
            //foreach (string f in files)
            //{
            //    string filename = Path.GetFileNameWithoutExtension(f);
            //    string directory = Path.GetDirectoryName(f);
            //    string ImgDir = "C:\\TEX3\\" + filename;
            //    DTXFile _f = new DTXFile();
            //    if (_f.ReadFromFile(f))
            //    {
            //        if (!Directory.Exists(ImgDir))
            //        {
            //            Directory.CreateDirectory(ImgDir);
            //        }
            //        for (int imgidx = 0; imgidx < _f.ImgLists.Count; ++imgidx)
            //        {
            //            ImgInfo _i = _f.ImgLists[imgidx];
            //            _i.Img.Save(ImgDir + string.Format("\\{0}.bmp", imgidx), ImageFormat.Bmp);
            //        }
            //    }
            //}
        }

        private void btnDestBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog destSrc = new FolderBrowserDialog();
            DialogResult destSrcResult = destSrc.ShowDialog();
            if (destSrcResult == DialogResult.OK)
            {
                txtDestFolder.Text = destSrc.SelectedPath.ToString();
            }
            else
            {
                txtDestFolder.Text = String.Empty;
            }
            destSrc.Dispose();
        }

        private void rbImagesSeparateFolder_Click(object sender, EventArgs e)
        {
            rbDumpDTX.Checked = false;
            rbDumpDTXZipFiles.Checked = false;
            rbImagesSingleFolder.Checked = false;
        }

        private void rbImagesSingleFolder_Click(object sender, EventArgs e)
        {
            rbDumpDTX.Checked = false;
            rbDumpDTXZipFiles.Checked = false;
            rbImagesSeparateFolder.Checked = false;
        }

        private void rbDumpDTX_Click(object sender, EventArgs e)
        {
            rbDumpDTXZipFiles.Checked = false;
            rbImagesSeparateFolder.Checked = false;
            rbImagesSingleFolder.Checked = false;
        }

        private void rbDumpDTXZipFiles_Click(object sender, EventArgs e)
        {
            rbDumpDTX.Checked = false;
            rbImagesSeparateFolder.Checked = false;
            rbImagesSingleFolder.Checked = false;
        }
    }
}
