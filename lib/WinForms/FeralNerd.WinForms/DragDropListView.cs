using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

/*  Drag/drop order of method calls in DragDropListView
     -  OnItemDrag: calls GetDataForDragDrop(), which makes a list of selected LVIs
     -  OnDragEnter
     -  OnDragOver (this is where we add recognition for the file types)
     -  OnDragDrop
*/


namespace FeralNerdLib.WinForms
{
    public delegate void DragEnterEvent(DragEventArgs args);
    public delegate void DragDropEvent(DragEventArgs dragArgs, int hoverItemIdx, bool isInternal);

    public class DragDropListView : ListView
	{
		#region Private Members

		private ListViewItem m_previousItem;
		private bool m_allowReorder;
		private Color m_lineColor;

		#endregion

		#region Public Properties

		[Category("Behavior")]
		public bool AllowReorder
		{
			get { return m_allowReorder; }
			set { m_allowReorder = value; }
		}

		[Category("Appearance")]
		public Color LineColor
		{
			get { return m_lineColor; }
			set { m_lineColor = value; }
		}

		#endregion

		#region Protected and Public Methods

		public DragDropListView() : base()
		{
			m_allowReorder = true;
			m_lineColor = Color.Red;
		}

        public DragDropEvent DragDropHandler;

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
            Debug.WriteLine("DragDropListView.OnDragDrop");
			if(!m_allowReorder)
			{
				base.OnDragDrop(drgevent);
				return;
			}

			// get the currently hovered row that the items will be dragged to
			Point clientPoint = base.PointToClient(new Point(drgevent.X, drgevent.Y));
			ListViewItem hoverItem = base.GetItemAt(clientPoint.X, clientPoint.Y);
            int hoverIdx = (hoverItem != null) ? hoverItem.Index : base.Items.Count;

            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()))
            {
                if (DragDropHandler != null)
                    DragDropHandler(drgevent, hoverIdx, false);
                return;
            }

			// retrieve the drag item data
			DragItemData data = (DragItemData) drgevent.Data.GetData(typeof(DragItemData).ToString());
            if (data.ListView == null || data.DragItems.Count == 0)
                return;

            // Clear out whatever was selected
            base.SelectedItems.Clear();

            // Now move the dragged items to the new spot.
			if(hoverItem == null)
			{
				// The user has hovered past the last item in the list, so there is
                // no hoverItem.  In this case the user wants to drop the files at
                // the end of the list.
                foreach (ListViewItem lvi in data.DragItems)
                {
                    lvi.Remove();
                    base.Items.Add(lvi);
                }
			}
			else
			{
				// the user has dropped the items to somewhere in the middle of the
                // list.
                foreach (ListViewItem lvi in data.DragItems)
                {
                    lvi.Remove();
                    base.Items.Insert(hoverItem.Index, lvi);
                }
			}

            // Re-select all the items that were dragged, and make sure the
            // item at the top of the list is visible
            bool first = false;
            foreach (ListViewItem lvi in data.DragItems)
            {
                lvi.Selected = true;
                if (!first)
                {
                    lvi.EnsureVisible();
                    first = true;
                }
            }

			// set the back color of the previous item, then nullify it
			if(m_previousItem != null)
			{
				m_previousItem = null;
			}

			this.Invalidate();

			// call the base on drag drop to raise the event
			base.OnDragDrop (drgevent);
            if (DragDropHandler != null)
                DragDropHandler(drgevent, hoverIdx, true);
        }

		protected override void OnDragOver(DragEventArgs drgevent)
		{
            Debug.WriteLine("DragDropListView.OnDragOver");
            if (!m_allowReorder)
			{
				base.OnDragOver(drgevent);
				return;
			}

            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()) && !drgevent.Data.GetDataPresent("FileDrop"))
			{
				// the item(s) being dragged do not have any data associated
				drgevent.Effect = DragDropEffects.None;
				return;
			}

			if(base.Items.Count > 0)
			{
				// get the currently hovered row that the items will be dragged to
				Point clientPoint = base.PointToClient(new Point(drgevent.X, drgevent.Y));
				ListViewItem hoverItem = base.GetItemAt(clientPoint.X, clientPoint.Y);

				Graphics g = this.CreateGraphics();

				if(hoverItem == null)
				{
					// no item was found, which means the user is hovering somewhere
                    // past the last item in the ListView.
					drgevent.Effect = DragDropEffects.Move;

					if(m_previousItem != null)
					{
						m_previousItem = null;
						Invalidate();
					}

					hoverItem = base.Items[base.Items.Count - 1];
						
					if(this.View == View.Details || this.View == View.List)
					{
						g.DrawLine(new Pen(m_lineColor, 2), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + this.Bounds.Width, hoverItem.Bounds.Y + hoverItem.Bounds.Height));
						g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height + 5)});
						g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5), new Point(this.Bounds.Width - 9, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y + hoverItem.Bounds.Height + 5)});
					}
					else
					{
						g.DrawLine(new Pen(m_lineColor, 2), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y + hoverItem.Bounds.Height));
						g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width - 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width + 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y + 5)});
						g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width - 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width + 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5)});
					}

					// call the base OnDragOver event
					base.OnDragOver(drgevent);

					return;
				}

				// determine if the user is currently hovering over a new
				// item. If so, set the previous item's back color back
				// to the default color.
				if((m_previousItem != null && m_previousItem != hoverItem) || m_previousItem == null)
				{
					this.Invalidate();
				}
			
				// set the background color of the item being hovered
				// and assign the previous item to the item being hovered
				//hoverItem.BackColor = Color.Beige;
				m_previousItem = hoverItem;

				if(this.View == View.Details || this.View == View.List)
				{
					g.DrawLine(new Pen(m_lineColor, 2), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + this.Bounds.Width, hoverItem.Bounds.Y));
					g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y - 5), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + 5)});
					g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y - 5), new Point(this.Bounds.Width - 9, hoverItem.Bounds.Y), new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y + 5)});
				}
				else
				{
					g.DrawLine(new Pen(m_lineColor, 2), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height));
					g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(hoverItem.Bounds.X - 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + 5)});
					g.FillPolygon(new SolidBrush(m_lineColor), new Point[] {new Point(hoverItem.Bounds.X - 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5)});
				}

                // Disable dragging if the user is dragging within the ListView, 
                // and hovers over one of the selected items, 
                DragItemData data = (DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString());
                if (data != null && data.ListView != null && data.DragItems.Count != 0)
                {
                    foreach (ListViewItem lvi in data.DragItems)
                    {
                        if (hoverItem.Index == lvi.Index)
                        {
                            drgevent.Effect = DragDropEffects.None;
                            hoverItem.EnsureVisible();
                            return;
                        }
                    }
                }

				// ensure that the hover item is visible
				hoverItem.EnsureVisible();
			}

			// everything is fine, allow the user to move the items
			drgevent.Effect = DragDropEffects.Move;

			// call the base OnDragOver event
			base.OnDragOver(drgevent);
		}


        public event DragEnterEvent DragEnterHandler;

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
            Debug.WriteLine("DragDropListView.OnDragEnter");

			if(!m_allowReorder)
			{
				base.OnDragEnter(drgevent);
				return;
			}

            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()))
            {
                if (DragEnterHandler != null)
                {
                    DragEnterHandler(drgevent);
                    //base.OnDragEnter(drgevent);
                }
                else
                    drgevent.Effect = DragDropEffects.None;

                Debug.WriteLine("DragEnter: effect = {0}", drgevent.Effect);
                return;
            }
            else
            {
                // everything is fine, allow the user to move the items
                drgevent.Effect = DragDropEffects.Move;
            }

			// call the base OnDragEnter event
			base.OnDragEnter(drgevent);
		}

		protected override void OnItemDrag(ItemDragEventArgs e)
		{
            Debug.WriteLine("DragDropListView.OnItemDrag");

            if (!m_allowReorder)
			{
				base.OnItemDrag(e);
				return;
			}

			// call the DoDragDrop method
			base.DoDragDrop(GetDataForDragDrop(), DragDropEffects.Move);

			// call the base OnItemDrag event
			base.OnItemDrag(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			// reset the selected items background and remove the previous item
			ResetOutOfRange();

			Invalidate();

			// call the OnLostFocus event
			base.OnLostFocus(e);
		}

		protected override void OnDragLeave(EventArgs e)
		{
			// reset the selected items background and remove the previous item
			ResetOutOfRange();

			Invalidate();

			// call the base OnDragLeave event
			base.OnDragLeave(e);
		}

		#endregion

		#region Private Methods

		private DragItemData GetDataForDragDrop()
		{
			// create a drag item data object that will be used to pass along with the drag and drop
			DragItemData data = new DragItemData(this);

			// go through each of the selected items and 
			// add them to the drag items collection
			// by creating a clone of the list item
			foreach(ListViewItem item in this.SelectedItems)
			{
                data.DragItems.Add(item);
                //data.DragItems.Add(item.Clone());
			}

			return data;
		}

		private void ResetOutOfRange()
		{
			// determine if the previous item exists,
			// if it does, reset the background and release 
			// the previous item
			if(m_previousItem != null)
			{
				m_previousItem = null;
			}

		}

		#endregion

		#region DragItemData Class

		private class DragItemData
		{
			#region Private Members

			private DragDropListView m_listView;
			private ArrayList m_dragItems;

			#endregion

			#region Public Properties

			public DragDropListView ListView
			{
				get { return m_listView; }
			}

			public ArrayList DragItems
			{
				get { return m_dragItems; }
			}

			#endregion

			#region Public Methods and Implementation

			public DragItemData(DragDropListView listView)
			{
				m_listView = listView;
				m_dragItems = new ArrayList();
			}

			#endregion
		}

		#endregion
	}
}
