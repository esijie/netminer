using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Net.Common;
using NetMiner.Resource;

namespace NetMiner.Net.Interface
{
    public interface iHttpUnity
    {
        eResponse RequestUri(Uri rUri,eRequest request,cGlobalParas.RequestMethod Method);
        void DownLoadFile();
        void UploadFile();
       
    }
}
