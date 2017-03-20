using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;


// Make your ListView columns sortable in six easy-to-follow steps:
// 1.  Make sure the property ListView.HeaderStyle is set to 
//     ColumnHeaderStyle.Clickable
// 2.  Add an image list to the parent control/window.  The ImageList should be 
//     8x8.  Image 0 is transparent.  Image 1 is an up-arrow.  Image 2 is a 
//     down-arrow.
// 3.  Go to the properties for the ListView that you want sorted, and set the
//     SmallImageList property to point to your image list.
// 4.  In the class for the control/window that contains the ListView, create a
//     new instance of ListViewColumnTextSorter:
//         private ListViewColumnTextSorter _mySorter = new ListViewColumnTextSorter();
// 5.  Look for where your control/window calls InitializeComponent().  Insert a 
//     call to the Bind() method.  You'll supply a reference to the ListView that 
//     you want made sortable, followed by a list of types for each column.  If you
//     do not supply a type, then System.String is assumed.
//         _mySorter.Bind(MyLview, typeof(int), typeof(string));
// 6.  Every time you clear the ListViewItem collection, you must call the Reset() 
//     method.  (NOTE: When you add a list-view item, it will automatically get 
//     sorted by the correct column.)
//         _mySorter.Reset();
//         MyLview.Items.Clear();
//
// The Bind() method will wire up header-click event handler, and take care of
// everything else for you.  It's really kinda slick.


namespace FeralNerd.WinForms
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A class for sorting columns of a ListView by their text contents.
    ///     You must set the ListView.SmallImageList property with an ImageList
    ///     (8x8 or 16x16) that contains a blank image, an up-arrow, and a down-arrow,
    ///     in that order.<para/>
    ///     <para/>
    ///     After the call to InitializeComponent, you must call the Bind() method.
    ///     Any time you modify the ListView's items (add/clear/etc.) you need to call the Reset()
    ///     method.<para/>
    ///     <para/>
    ///     And that's really all there is to it.
    /// </summary>
    public class ListViewColumnTextSorter: IComparer
    {
        private ListView _lview = null;
        private string _sortedType;
        private List<string> _colTypeList = new List<string>();

        #region ### IComparer ##################################################

        int IComparer.Compare(object x, object y)
        {
            if (SortedColumn < 0)
                return 0;

            ListViewItem itemx = x as ListViewItem;
            ListViewItem itemy = y as ListViewItem;

            int ord = 0;
            switch (_sortedType)
            {
                case "System.Int32":
                    int intx, inty;
                    if (int.TryParse(itemx.SubItems[SortedColumn].Text, out intx) && int.TryParse(itemy.SubItems[SortedColumn].Text, out inty))
                        ord = intx - inty;
                    break;

                default:
                    ord = string.Compare(itemx.SubItems[SortedColumn].Text, itemy.SubItems[SortedColumn].Text);
                    break;
            }
            
            if (SortOrder == SortOrder.Descending)
                return -ord;
            return ord;
        }

        #endregion


        #region ### PUBLIC #####################################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Creates a ListViewColumnTextSorter.
        /// </summary>
        public ListViewColumnTextSorter()
        {
            Reset();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Call this method to bind the ListViewColumnTextSorter to a 
        ///     ListView control.
        /// </summary>
        /// <param name="lview">
        ///     The ListView you want to enable sorting for.
        /// </param>
        /// <param name="typeList">
        ///     A optional list of types for each column.  If you don't 
        ///     supply a list of types, then System.String will be assumed.
        /// </param>
        public void Bind(ListView lview, params Type[] typeList)
        {
            _lview = lview;
            _lview.ListViewItemSorter = this;
            _lview.ColumnClick += __Lview_ColumnClick;

            foreach (Type type in typeList)
                _colTypeList.Add(type.ToString());

            while (_lview.Columns.Count > _colTypeList.Count)
                _colTypeList.Add(typeof(string).ToString());
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Call this method whenever the Items collection for your ListView
        ///     is modified in any way.  This will reset the sorting.
        /// </summary>
        public void Reset()
        {
            SortedColumn = -1;
            SortOrder = SortOrder.None;

            if (_lview != null)
            {
                foreach (ColumnHeader col in _lview.Columns)
                    col.ImageIndex = -1;
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Use this to find out what column is currently sorted.  -1 indicates no column.
        /// </summary>
        public int SortedColumn { get; private set; }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the current sort order.
        /// </summary>
        public SortOrder SortOrder { get; private set; }

        #endregion

        
        private void __Lview_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            foreach (ColumnHeader header in _lview.Columns)
                header.ImageIndex = 0;

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == SortedColumn)
            {
                // Reverse the current sort direction for this column.
                if (SortOrder == SortOrder.Ascending)
                {
                    SortOrder = SortOrder.Descending;
                    _lview.Columns[e.Column].ImageIndex = 2;
                }
                else
                {
                    SortOrder = SortOrder.Ascending;
                    _lview.Columns[e.Column].ImageIndex = 1;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                SortedColumn = e.Column;
                _sortedType = _colTypeList[e.Column].ToString();
                SortOrder = SortOrder.Ascending;
                _lview.Columns[e.Column].ImageIndex = 1;
            }

            // Perform the sort with these new sort options.
            _lview.Sort();
            if (_lview.SelectedItems.Count > 0)
                _lview.SelectedItems[0].EnsureVisible();
        }
    }
}
