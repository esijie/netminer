using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;

namespace SoukeyDataPublish
{
    // 任务事件
    public class cToolboxEvent : EventArgs
    {

        public cToolboxEvent()
        {

        }

        /// <param name="cancel">是否取消事件</param>
        public cToolboxEvent(bool cancel)
        {
            m_Cancel = cancel;
        }

        private bool m_Cancel;
        /// <summary>
        /// 是否取消事件
        /// </summary>
        public bool Cancel
        {
            get { return m_Cancel; }
            set { m_Cancel = value; }
        }
    }

    public class LoadDataEventArgs : cToolboxEvent
    {
        public LoadDataEventArgs(TreeNode node)
        {
            m_node = node;
        }

        private TreeNode m_node;
        public TreeNode node
        {
            get { return m_node; }
            set { m_node = value; }
        }

    }
}
