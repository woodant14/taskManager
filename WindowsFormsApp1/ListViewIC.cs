using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class ListViewIC : IComparer
    {
        private int _columIndex;
        public int ColumnIndex
        {
            get { return _columIndex; }
            set { _columIndex = value; }
        }
        private SortOrder _sortDirection;
        public SortOrder SortDirection
        {
            get
            {
                return _sortDirection;
            }
            set
            {
                _sortDirection = value;
            }
        }

        public int Compare(object x, object y)
        {
            ListViewItem LVI_x = x as ListViewItem;
            ListViewItem LVI_y = y as ListViewItem;

            int result;
            switch (_columIndex)
            {
                case 0:
                    result = string.Compare(LVI_x.SubItems[_columIndex].Text,
                        LVI_y.SubItems[_columIndex].Text, false);
                    break;
                case 1:
                    double valX = double.Parse(LVI_x.SubItems[_columIndex].Text);
                    double valY = double.Parse(LVI_y.SubItems[_columIndex].Text);
                    result = valX.CompareTo(valY);
                    break;
                default:
                    result = string.Compare(LVI_x.SubItems[_columIndex].Text,
                        LVI_y.SubItems[_columIndex].Text, false);
                    break;
            }
            if (_sortDirection == SortOrder.Descending)
            {
                return -result;
            }
            else
            {
                return result;
            }
        }
    }
}
