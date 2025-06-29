using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Gather.Radar
{
    public class cRadarControl : IDisposable
    {
        private cRadarManage m_RadarManage;
        private string m_workPath;
        #region 构造 析构
        public cRadarControl(string workPath)
        {
            m_workPath = workPath;
            m_RadarManage = new cRadarManage(workPath);
            //m_RadarManage.RadarStarted += this.Radar_Started;
            //m_RadarManage.RadarStop += this.Radar_Stop;
            //m_RadarManage.RadarWarning += this.Radar_Warning;

            //cXmlSConfig sc = new cXmlSConfig();

            //cGlobalParas.DatabaseType dtype = (cGlobalParas.DatabaseType)sc.DataType;
            //string conn = sc.DataConnection;

            //sc = null;

            //将存储url的数据库信息传入
            //m_RadarManage.DatabaseType = dtype;
            //m_RadarManage.dbCon = conn;

        }

        ~cRadarControl()
        {
            //m_RadarManage.RadarStarted -= this.Radar_Started;
            //m_RadarManage.RadarStop -= this.Radar_Stop;
            //m_RadarManage.RadarWarning -= this.Radar_Warning;

            m_RadarManage = null;
        }

        #endregion

        #region 属性

        public cRadarManage RadarManage
        {
            get { return m_RadarManage; }
        }

        #endregion

        #region 处理事件
        //private void Radar_Started(object sender, cRadarStartedArgs e)
        //{
            
        //}

        //private void Radar_Stop(object sender, cRadarStopArgs e)
        //{
            
        //}

        //private void Radar_Warning(object sender, cRadarMonitorWaringArgs e)
        //{
        //    switch (e.wType)
        //    {
        //        case cGlobalParas.WarningType .ByEmail :
        //            break;
        //        case cGlobalParas.WarningType .ByTrayIcon :
        //            break;
        //        case cGlobalParas.WarningType.NoWaring :
        //            break;
        //    }
        //}
        #endregion


        public void StartRadar()
        {
            m_RadarManage.StartRadar();
        }

        public void StopRadar()
        {
            m_RadarManage.StopRadar();
        }


        #region IDisposable 成员
        private bool m_disposed;
        /// <summary>
        /// 释放由 Download 的当前实例使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {


                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion
        

    }
}
