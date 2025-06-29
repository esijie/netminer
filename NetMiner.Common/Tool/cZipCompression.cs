using System;
using System.Collections.Generic;
using System.Text;
using Ionic;
using Ionic.Zip;
using System.IO;
using NetMiner.Resource;

namespace NetMiner.Common.Tool
{
    /// <summary>
    /// 压缩解压缩类
    /// </summary>
    public class cZipCompression
    {

        public bool CompressZIP(Dictionary<string, int> fs,string tmpFile)
        {
            try
            {
                using (ZipFile zip = new ZipFile(Encoding.GetEncoding("gb2312")))
                {
                    foreach (KeyValuePair<string,int> f in fs)
                    {
                        if (f.Value == (int)cGlobalParas.FileType.File)
                        {
                            if (File.Exists(f.Key))
                                zip.AddFile(f.Key, "");
                        }
                        else if (f.Value == (int)cGlobalParas.FileType.Directory)
                        {
                            if (Directory.Exists(f.Key))
                            {
                                //获取路径中的最后一段
                                string tPath = f.Key;
                                if (tPath.EndsWith("\\"))
                                {
                                    tPath=tPath.Substring (0,tPath.Length -1);
                                    tPath = tPath.Substring(tPath.LastIndexOf("\\"), tPath.Length - tPath.LastIndexOf("\\"));
                                }
                                else
                                    tPath = tPath.Substring(tPath.LastIndexOf("\\"), tPath.Length - tPath.LastIndexOf("\\"));
                                tPath += "\\";
                                zip.AddDirectory(f.Key, tPath);
                            }
                                
                        }

                    }
                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);
                    zip.Save(tmpFile);
                }
            }
            catch { return false; }

            return true;
        }

        public bool DeCompressZIP(string zipFile, string tmpPath)
        {
            try
            {
                string zipToUnpack = zipFile;
                string unpackDirectory = tmpPath + "\\" + Path.GetFileNameWithoutExtension(zipFile);

                if (Directory.Exists(unpackDirectory))
                    Directory.Delete(unpackDirectory, true);

                using (ZipFile zip1 = ZipFile.Read(zipToUnpack,Encoding.GetEncoding("gb2312")))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch { return false; }

            return true;
        }
    }
}
