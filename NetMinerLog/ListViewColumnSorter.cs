using System.Collections;
using System.Windows.Forms;

/// <summary>
/// �̳���IComparer
/// </summary>
public class ListViewColumnSorter : IComparer
{
    /// <summary>
    /// ָ�������ĸ�������
    /// </summary>
    private int ColumnToSort;
    /// <summary>
    /// ָ������ķ�ʽ
    /// </summary>
    private SortOrder OrderOfSort;
    /// <summary>
    /// ����CaseInsensitiveComparer�����
    /// �μ�ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.2052/cpref/html/frlrfSystemCollectionsCaseInsensitiveComparerClassTopic.htm
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// ���캯��
    /// </summary>
    public ListViewColumnSorter()
    {
        // Ĭ�ϰ���һ������
        ColumnToSort = 0;

        // ����ʽΪ������
        OrderOfSort = SortOrder.None;

        // ��ʼ��CaseInsensitiveComparer�����
        ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// ��дIComparer�ӿ�.
    /// </summary>
    /// <param name="x">Ҫ�Ƚϵĵ�һ������</param>
    /// <param name="y">Ҫ�Ƚϵĵڶ�������</param>
    /// <returns>�ȽϵĽ��.�����ȷ���0�����x����y����1�����xС��y����-1</returns>
    public int Compare(object x, object y)
    {
        int compareResult;
        ListViewItem listviewX, listviewY;

        // ���Ƚ϶���ת��ΪListViewItem����
        listviewX = (ListViewItem)x;
        listviewY = (ListViewItem)y;

        // �Ƚ�
        compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

        // ��������ıȽϽ��������ȷ�ıȽϽ��
        if (OrderOfSort == SortOrder.Ascending)
        {
            // ��Ϊ��������������ֱ�ӷ��ؽ��
            return compareResult;
        }
        else if (OrderOfSort == SortOrder.Descending)
        {
            // ����Ƿ�����������Ҫȡ��ֵ�ٷ���
            return (-compareResult);
        }
        else
        {
            // �����ȷ���0
            return 0;
        }
    }

    /// <summary>
    /// ��ȡ�����ð�����һ������.
    /// </summary>
    public int SortColumn
    {
        set
        {
            ColumnToSort = value;
        }
        get
        {
            return ColumnToSort;
        }
    }

    /// <summary>
    /// ��ȡ����������ʽ.
    /// </summary>
    public SortOrder Order
    {
        set
        {
            OrderOfSort = value;
        }
        get
        {
            return OrderOfSort;
        }
    }

}