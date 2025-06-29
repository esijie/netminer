using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Gather.Radar
{
    public class cRadarControl : IDisposable
    {
        private cRadarManage m_RadarManage;
        private string m_workPath;
        #region ���� ����
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

            //���洢url�����ݿ���Ϣ����
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

        #region ����

        public cRadarManage RadarManage
        {
            get { return m_RadarManage; }
        }

        #endregion

        #region �����¼�
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


        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� Download �ĵ�ǰʵ��ʹ�õ�������Դ
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

                // �ڴ��ͷŷ��й���Դ

                m_disposed = true;
            }
        }


        #endregion
        

    }
}
