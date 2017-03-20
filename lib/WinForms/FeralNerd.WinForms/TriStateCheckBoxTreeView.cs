#region Copyright © 2010 ViCon GmbH / Sebastian Grote. All Rights Reserved.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

// Things you must do:
// 1.  Make an ImageList.  Populate it with 0 as an empty square, 1 with a
//     square that has a check mark, and 2 with a square that has a box 
//     (intermediate check-state).  In the form's design-view, set the 
//     StateImageList property to the image-list you just created.
#if __SampleCode



#endif

namespace FeralNerd.WinForms
{
	
	/// <summary>
	/// Provides a tree view
	/// control supporting
	/// tri-state checkboxes.
	/// </summary>
	public class TriStateTreeView : TreeView
	{

		// ~~~ fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        ImageList _ilStateImages;
		bool _bUseTriState;
		bool _bCheckBoxesVisible;

		// ~~~ constructor ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		/// <summary>
		/// Creates a new instance
		/// of this control.
		/// </summary>
		public TriStateTreeView()
			: base()
		{
		 CheckBoxState cbsState;
		 Graphics gfxCheckBox;
		 Bitmap bmpCheckBox;

			_ilStateImages = new ImageList();											// first we create our state image
			cbsState = CheckBoxState.UncheckedNormal;									// list and pre-init check state.

			for (int i = 0; i <= 2; i++) {												// let's iterate each tri-state
				bmpCheckBox = new Bitmap(16, 16);										// creating a new checkbox bitmap
				gfxCheckBox = Graphics.FromImage(bmpCheckBox);							// and getting graphics object from
				switch (i) {															// it...
					case 0: cbsState = CheckBoxState.UncheckedNormal; break;
					case 1: cbsState = CheckBoxState.CheckedNormal; break;
					case 2: cbsState = CheckBoxState.MixedNormal; break;
				}
				CheckBoxRenderer.DrawCheckBox(gfxCheckBox, new Point(2, 2), cbsState);	// ...rendering the checkbox and...
				gfxCheckBox.Save();
				_ilStateImages.Images.Add(bmpCheckBox);									// ...adding to sate image list.
				
				_bUseTriState = true;
			}
		}

		// ~~~ properties ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		/// <summary>
		/// Gets or sets to display
		/// checkboxes in the tree
		/// view.
		/// </summary>
		[Category("Appearance")]
		[Description("Sets tree view to display checkboxes or not.")]
		[DefaultValue(false)]
		public new bool CheckBoxes
		{
			get { return _bCheckBoxesVisible; }
			set
			{
				_bCheckBoxesVisible = value;
				base.CheckBoxes = _bCheckBoxesVisible;
				this.StateImageList = _bCheckBoxesVisible ? _ilStateImages : null;
			}
		}

        [Category("Appearance")]
        [Description("Sets the images used for the check-state of each TriStateTreeNode item.")]
        [DefaultValue(null)]
        public new ImageList StateImageList
		{
			get { return base.StateImageList; }
			set { base.StateImageList = value; }
		}

		/// <summary>
		/// Gets or sets to support
		/// tri-state in the checkboxes
		/// or not.
		/// </summary>
		[Category("Appearance")]
		[Description("Sets tree view to use tri-state checkboxes or not.")]
		[DefaultValue(true)]
		public bool CheckBoxesTriState
		{
			get { return _bUseTriState; }
			set { _bUseTriState = value; }
		}

		// ~~~ functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		/// <summary>
		/// Refreshes this
		/// control.
		/// </summary>
		public override void Refresh()
		{
		 Stack<TreeNode> stNodes;
		 TreeNode tnStacked;

			base.Refresh();

			if (!CheckBoxes)												// nothing to do here if
				return;														// checkboxes are hidden.

			base.CheckBoxes = false;										// hide normal checkboxes...

			stNodes = new Stack<TreeNode>(this.Nodes.Count);				// create a new stack and
			foreach (TreeNode tnCurrent in this.Nodes)						// push each root node.
				stNodes.Push(tnCurrent);

			while (stNodes.Count > 0) {										// let's pop node from stack,
				tnStacked = stNodes.Pop();									// set correct state image
				if (tnStacked.StateImageIndex == -1)						// index if not already done
					tnStacked.StateImageIndex = tnStacked.Checked ? 1 : 0;	// and push each child to stack
				for (int i = 0; i < tnStacked.Nodes.Count; i++)				// too until there are no
					stNodes.Push(tnStacked.Nodes[i]);						// nodes left on stack.
			}
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);

			Refresh();
		}

		protected override void OnAfterExpand(TreeViewEventArgs e)
		{
			base.OnAfterExpand(e);

            foreach (TreeNode tn in e.Node.Nodes)					// set tree state image
            {
                TriStateTreeNode tnCurrent = tn as TriStateTreeNode;
                tnCurrent.internal_UpdateCheckStateImage();
            }
		}

		protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
		{
            base.OnNodeMouseClick(e);

            TriStateTreeNode tn = e.Node as TriStateTreeNode;

            int iSpacing = ImageList == null ? 0 : 18;						// if user clicked area
            if (e.X > e.Node.Bounds.Left - iSpacing ||						// *not* used by the state
                e.X < e.Node.Bounds.Left - (iSpacing + 16))					// image we can leave here.
            { return; }

            // Toggle check-state
            tn.internal_SetSelfCheckState(!tn.Checked);
            tn.internal_SetChildCheckState();
            tn.internal_SetParentCheckState();
		}


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Finds a node based on a given path.  Use '\' as a path separator
        ///     character.
        /// </summary>
        /// 
        /// <param name="nodePath">
        ///     Path to the node you want to retrieve, with or without a leading '\'
        ///     character.
        /// </param>
        /// 
        /// <returns>
        ///     The first instance of the node matching the search path, or null.
        /// </returns>
        public TriStateTreeNode FindNode(string nodePath)
        {
            return Nodes.FindChild(nodePath);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets all nodes where the TriStateCheck property is set to
        ///     TriStateCheck.Checked.
        /// </summary>
        /// 
        /// <param name="deep">
        ///     If this parameter is set to true, this method will give an 
        ///     inclusive list of all leaf-nodes.  If this parameter is set to
        ///     false, this method will return folders where the TriStateCheck
        ///     property is set to TriStateCheck.Checked, and will not
        ///     recurse deeper.
        /// </param>
        /// <returns></returns>
        public List<TriStateTreeNode> GetCheckedNodes(bool deep)
        {
            Stack<TriStateTreeNode> stack = new Stack<TriStateTreeNode>();
            List<TriStateTreeNode> output = new List<TriStateTreeNode>();

            // Load the stack with all children.  Push in reverse order, so that
            // the final listing doesn't come out all wonky.
            for (int i = Nodes.Count - 1; i >= 0; i--)
                stack.Push(Nodes[i] as TriStateTreeNode);

            // Loop while we got stuff on the stack
            while (stack.Count > 0)
            {
                TriStateTreeNode node = stack.Pop();
                if (node == null) // safety-check
                    continue;

                // Trivial reject
                if (node.TriCheckState == TriStateCheck.NotChecked)
                    continue;

                if (deep)
                {
                    if (node.TriCheckState == TriStateCheck.Checked || node.TriCheckState == TriStateCheck.Mixed)
                    {
                        if (node.Nodes.Count > 1)
                        {
                            // This is a parent node.  Scan all children.
                            for (int i = node.Nodes.Count - 1; i >= 0; i--)
                                stack.Push(node.Nodes[i] as TriStateTreeNode);
                        }
                        else if (node.TriCheckState == TriStateCheck.Checked)
                        {
                            // This is a leaf node.  Add it to the list.
                            output.Add(node);
                        }
                    }
                }
                else
                {
                    if (node.TriCheckState == TriStateCheck.Checked)
                    {
                        // The node is checked.  Add it to the list.  The 
                        // caller can figure out if this is a folder or 
                        // not.
                        output.Add(node);
                    }
                    else if (node.TriCheckState == TriStateCheck.Mixed)
                    { 
                        // Check the children
                        for (int i = node.Nodes.Count - 1; i >= 0; i--)
                            stack.Push(node.Nodes[i] as TriStateTreeNode);
                    }
                }
            }

            return output;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Adds a node, inserting as many folders and sub-folders as is
        ///     necessary.  Duplicate folder names are merged.  Duplicate leaf
        ///     nodes are allowed.<para/>
        ///     <para/>
        ///     The ImageIndex for folder nodes will be set to 0. 
        /// </summary>
        /// 
        /// <param name="path">
        ///     The path to this node.  This value MUST include the name of the
        ///     leaf node (i.e., "folder\subfolder\leaf").
        /// </param>
        /// <param name="newChildNode">
        ///     The TriStateTreeNode you want added.
        /// </param>
        /// <param name="folderView">
        ///     If this is set to true, then folders will be grouped at the start of each
        ///     level in the tree-view.  If set to false, the folders will be added in
        ///     the order that you call DeepAddNode.
        /// </param>
        /// <param name="ignoreCase">
        ///     Ignore case when comparing folder names.  This value has no 
        ///     effect on the leaf node, and duplicate leafs are allowed.
        /// </param>
        public void DeepAddNode(string path, TriStateTreeNode newChildNode, bool folderView, bool ignoreCase = true)
        {
            Nodes.DeepAddNode(path, newChildNode, folderView, ignoreCase);
        }
    }

    public enum TriStateCheck
    {
        NotChecked,
        Mixed,
        Checked
    }

    public class TriStateTreeNode: TreeNode
    {
        public TriStateTreeNode()
        {
            internal_SetSelfCheckState(false);
        }

        public TriStateTreeNode(string text)
            : this()
        {
            this.Text = text;
        }


        private TriStateCheck _checkState = TriStateCheck.NotChecked;

        internal void internal_UpdateCheckStateImage()
        {
            switch (_checkState)
            {
                case TriStateCheck.NotChecked:
                    StateImageIndex = 0;
                    break;

                case TriStateCheck.Mixed:
                    StateImageIndex = 2;
                    break;

                case TriStateCheck.Checked:
                    StateImageIndex = 1;
                    break;
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the tri check-state.  To set the check-state, call the 
        ///     DeepSetCheckState() method.
        /// </summary>
        public TriStateCheck TriCheckState
        {
            get { return _checkState; }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the normal check-state of the node.  To set the check-
        ///     state, call the DeepSetCheckState() method.
        /// </summary>
        public new bool Checked
        {
            get
            {
                return base.Checked;
            }

            //set
            //{
            //    internal_SetSelfCheckState(value);   // Set the check-state
            //}
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Sets the check-state of the current node, and updates the
        ///     check-state of all child and parent nodes.
        /// </summary>
        /// <param name="isChecked"></param>
        public void DeepSetCheckState(bool isChecked)
        {
            internal_SetSelfCheckState(isChecked);   // Set the check-state
            internal_SetChildCheckState();             // Bulk-set all child check-states
            internal_SetParentCheckState();            // Walk up the parent chain and set all check-states.
        }

        public void ExpandAllParents()
        {
            for (TreeNode tnParent = Parent; tnParent != null; tnParent = tnParent.Parent)
                tnParent.Expand();
        }

        internal void internal_SetSelfCheckState(bool isChecked)
        {
            base.Checked = isChecked;
            StateImageIndex = isChecked ? 1 : 0;
            _checkState = isChecked ? TriStateCheck.Checked : TriStateCheck.NotChecked;
        }

        private TriStateTreeNode GetChild(string item)
        {
            foreach (TreeNode tn in Nodes)
            {
                TriStateTreeNode node = tn as TriStateTreeNode;
                if (string.Compare(node.Text, item, true) == 0)
                    return node;
            }

            return null;
        }


        internal void internal_SetChildCheckState()
        {
            // This routine uses a breadth-first approach to set the check-state of
            // all children based on the check-state of a single parent.

            // Get the check-state of the original parent.
            bool isChecked = this.Checked;

            // Make a new stack and push the current node.  The first pass through the
            // loop is redundant, but we don't care.
            TriStateTreeNode tnNode = this;
            Stack<TriStateTreeNode> stNodes = new Stack<TriStateTreeNode>();
            stNodes.Push(tnNode);
            while (stNodes.Count > 0)
            {
                // Pop a node and set it's check-state
                tnNode = stNodes.Pop();
                tnNode.internal_SetSelfCheckState(isChecked);

                // Push any and all children onto the stack
                for (int i = 0; i < tnNode.Nodes.Count; i++)
                    stNodes.Push(tnNode.Nodes[i] as TriStateTreeNode);
            }
        }

        internal void internal_SetParentCheckState()
        {
            // Set check-state for all parent nodes.  Some are partially checked, and 
            // some are fully checked.
            for (TriStateTreeNode tnParent = Parent as TriStateTreeNode; tnParent != null; tnParent = tnParent.Parent as TriStateTreeNode)
            {
                // Count all fully-checked nodes
                int fullCheckCount = 0;
                int partialCheckCount = 0;
                foreach (TreeNode tn in tnParent.Nodes)
                {
                    TriStateTreeNode tnSibling = tn as TriStateTreeNode;
                    if (tnSibling.TriCheckState == TriStateCheck.Checked)
                        fullCheckCount++;
                    if (tnSibling.TriCheckState == TriStateCheck.Mixed)
                        partialCheckCount++;
                }

                if (fullCheckCount == tnParent.Nodes.Count)
                {
                    // All siblings are fully-checked.  Fully-check the parent
                    ((TreeNode)tnParent).Checked = true;
                    tnParent._checkState = TriStateCheck.Checked;
                    tnParent.StateImageIndex = 1;
                }
                else if (fullCheckCount > 0 || partialCheckCount > 0)
                {
                    // Some nodes are fully-checked, and some are partially-checked
                    ((TreeNode)tnParent).Checked = true;
                    tnParent._checkState = TriStateCheck.Mixed;
                    tnParent.StateImageIndex = 2;
                }
                else
                {
                    // No nodes are checked
                    ((TreeNode)tnParent).Checked = false;
                    tnParent._checkState = TriStateCheck.NotChecked;
                    tnParent.StateImageIndex = 0;
                }
            }
        }

        public TriStateTreeNode FindChild(string nodePath)
        {
            return Nodes.FindChild(nodePath);
        }
    }


    public enum RecursiveForeachType
    {
        PostOrder,
        PreOrder,
        LeafsOnly
    }

    public delegate bool RecursiveForeachHandler(TreeNode node);

    public static class TreeNodeExtensions
    {
        private static char[] _pathSeparator = new char[] { '\\' };

        public static TriStateTreeNode FindChild(this TreeNodeCollection self, string nodePath)
        {
            if (string.IsNullOrEmpty(nodePath))
                return null;

            // Strip off the leading '\'
            nodePath = nodePath.TrimStart('\\');
            if (string.IsNullOrEmpty(nodePath))
                return null;

            // Break off the first part of the path
            string[] split = nodePath.Split(new char[] { '\\' }, 2);

            // Go through all child nodes
            foreach (TreeNode tn in self)
            {
                TriStateTreeNode tnChild = tn as TriStateTreeNode;

                // If the current node matches what we're looking for...
                if (tnChild.Text == split[0])
                {
                    // Okay, we found the node.  Do we have any more path parts to go?
                    if (split.Length > 1)
                    {
                        // If we're out of children, then return a null
                        if (tnChild.Nodes.Count == 0)
                            return null;

                        // Okay, go one more level down
                        return FindChild(tnChild.Nodes, split[1]);
                    }

                    // This is the node we want
                    return tnChild;
                }
            }

            // Not found.  Return null.
            return null;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Adds a node using a path that you supply.
        /// </summary>
        /// 
        /// <param name="self">
        ///     The TreeNodeCollection that you want to start with.  This can be the Nodes
        ///     property on your TreeView, or on your TreeViewNode.
        /// </param>
        /// <param name="path">
        ///     The full path of the node you're inserting, including the name of the
        ///     node, itself (e.g., "SomeDir\AnotherDir\myNewNode").  Do not include 
        ///     a leading back-slash.
        /// </param>
        /// <param name="newChildNode">
        ///     The node that you want to insert.
        /// </param>
        /// <param name="folderView">
        ///     If this is set to true, then folders will be grouped at the start of each
        ///     level in the tree-view.  If set to false, the folders will be added in
        ///     the order that you call DeepAddNode.
        /// </param>
        /// <param name="ignoreCase">
        ///     Set this to true if you want to ignore case when adding folders.
        /// </param>
        public static void DeepAddNode(this TreeNodeCollection self, string path, TriStateTreeNode newChildNode, bool folderView, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(path))
                return;

            string[] parts = path.Split(_pathSeparator, 2);

            if (parts.Length == 1)
            {
                // This is a new leaf-node.  Go ahead and add it (does not 
                // check for duplicates).
                if (folderView)
                    self.InsertNodeAsFile(newChildNode, ignoreCase);
                else
                    self.Add(newChildNode);
                return;
            }

            // See if the folder has already been added, if so, recurse into
            // that folder.
            TreeNode folder = self.FindFolder(parts[0], ignoreCase);
            if (folder == null)
            {
                // This is a new folder.
                folder = new TriStateTreeNode(parts[0]);
                folder.ImageIndex = folder.SelectedImageIndex = 0;

                if (folderView)
                    self.InsertNodeAsFolder(folder, ignoreCase);
                else
                    self.Add(folder);
            }

            folder.Nodes.DeepAddNode(parts[1], newChildNode, folderView, ignoreCase);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Finds the first folder matching the name given.  A "folder" is 
        ///     defined as a TreeNode at the beginning of the set of TreeNodeCollection
        ///     that contains one or more children.  If no folder is found, then
        ///     null is returned.
        /// </summary>
        /// 
        /// <param name="self">
        ///     The collection you want searched.
        /// </param>
        /// <param name="folderName">
        ///     Name of the folder you want to find.
        /// </param>
        /// <param name="ignoreCase">
        ///     Set this to true if you want to ignore case when adding folders.
        /// </param>
        public static TreeNode FindFolder(this TreeNodeCollection self, string folderName, bool ignoreCase = true)
        {
            foreach (TriStateTreeNode node in self)
            {
                if (node.Nodes.Count < 1)
                    break; // Looks like we've already passed all folders

                if (string.Compare(node.Text, folderName, ignoreCase) == 0)
                {
                    return node;
                }
            }

            return null;
        }

        public static void InsertNodeAsFolder(this TreeNodeCollection self, TreeNode newNode, bool ignoreCase = true)
        {
            int idx = 0;
            for (int i = 0; i < self.Count; i++)
            {
                TreeNode currentNode = self[i];
                if (currentNode.Nodes.Count < 1)
                {
                    // We've passed the list of folders, so insert here.
                    idx = i;
                    break;
                }

                if (string.Compare(newNode.Text, currentNode.Text, ignoreCase) < 0)
                {
                    // The current folder comes after the new folder, alphabetically, so
                    // insert here.
                    idx = i;
                    break;
                }
            }

            self.Insert(idx, newNode);
        }

        public static void InsertNodeAsFile(this TreeNodeCollection self, TreeNode newNode, bool ignoreCase = true)
        {
            int idx = 0;

            // Skip over any folders
            for (idx = 0; idx < self.Count; idx++)
            {
                TreeNode currentNode = self[idx];
                if (currentNode.Nodes.Count < 1)
                    break;
            }

            // Now skip down to the first folder that comes after newNode alphabetically
            for (; idx < self.Count; idx++)
            {
                TreeNode currentNode = self[idx];
                if (string.Compare(newNode.Text, currentNode.Text, ignoreCase) < 0)
                {
                    // The current folder comes after the new folder, alphabetically, so
                    // insert here.
                    break;
                }
            }

            self.Insert(idx, newNode);
        }

        public static void SetCheckState(this TreeNodeCollection self, bool isChecked)
        {
            foreach (TriStateTreeNode node in self)
                node.DeepSetCheckState(isChecked);
        }

        public static bool RecursiveForeach(this TreeNodeCollection self, RecursiveForeachType searchType, RecursiveForeachHandler onForeach)
        {
            if (onForeach == null || self.Count < 1)
                return true;

            if (searchType == RecursiveForeachType.LeafsOnly)
            {
                foreach (TreeNode node in self)
                {
                    if (node.Nodes.Count > 0)
                    {
                        if (!node.Nodes.RecursiveForeach(RecursiveForeachType.LeafsOnly, onForeach))
                            return false;
                    }
                    else if (!onForeach(node))
                        return false;
                }
            }
            else if (searchType == RecursiveForeachType.PostOrder)
            {
                foreach (TreeNode node in self)
                {
                    if (!self.RecursiveForeach(RecursiveForeachType.PostOrder, onForeach))
                        return false;
                    if (!onForeach(node))
                        return false;
                }
            }
            else
            {
                foreach (TreeNode node in self)
                {
                    if (!onForeach(node))
                        return false;
                    if (!self.RecursiveForeach(RecursiveForeachType.PreOrder, onForeach))
                        return false;
                }
            }

            return true;
        }
    }
}
