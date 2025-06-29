using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;

namespace SoukeyDataPublish
{
    // �����¼�
    public class cToolboxEvent : EventArgs
    {

        public cToolboxEvent()
        {

        }

        /// <param name="cancel">�Ƿ�ȡ���¼�</param>
        public cToolboxEvent(bool cancel)
        {
            m_Cancel = cancel;
        }

        private bool m_Cancel;
        /// <summary>
        /// �Ƿ�ȡ���¼�
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
