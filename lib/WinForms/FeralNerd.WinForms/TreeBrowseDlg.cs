using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FeralNerd.WinForms
{

    public delegate List<TreeBrowseItem> GetFilesHandler(string path);

    public partial class TreeBrowseDlg : Form
    {
        private const string _nullChild = @"\NullChild\";
        List<string> _checkedItems = null;


        #region ### Public Properties ##########################################

        public TreeBrowseDlg()
        {
            InitializeComponent();
        }

        public event GetFilesHandler OnGetFiles;
        public event GetFilesHandler OnGetAllFiles;
        public string RootFolderName = null;

        public string Prompt { get; set; }

        public List<string> CheckedItems
        {
            get
            {
                List<string> outList = new List<string>();

                __GetCheckedNodes(ItemsTview.Nodes, outList);

                return outList;
            }

            set
            {
                _checkedItems = value;
            }
        }

        #endregion


        #region ### Private Event Handlers #####################################

        private void TreeBrowseDlg_Load(object sender, EventArgs e)
        {
            PromptLabel.Text = Prompt;

            ItemsTview.Nodes.Clear();
            if (!string.IsNullOrEmpty(RootFolderName))
            {
                TreeNode root = new TreeNode(RootFolderName);
                root.ImageIndex = 0;
                ItemsTview.Nodes.Add(root);
            }

            if (OnGetAllFiles != null && OnGetFiles != null)
                throw new WinFormsException("Cannot have both TreeBrowseDlg.OnGetAllFiles and TreeBrowseDlg.OnGetFiles set.  Must pick one or the other.");

            // Try to get all the files in one fell swoop
            if (OnGetAllFiles != null)
            {
                List<TreeBrowseItem> fileList = OnGetAllFiles("");
                if (fileList == null)
                    return;

                // Do a deep-add for each file we got.
                foreach (TreeBrowseItem item in fileList)
                {
                    __DeepAddNode(
                        string.IsNullOrEmpty(RootFolderName)? ItemsTview.Nodes: ItemsTview.Nodes[0].Nodes, 
                        item);
                }

                if (!string.IsNullOrEmpty(RootFolderName))
                    ItemsTview.Nodes[0].Expand();

                return;
            }

            // If that isn't set up, then get stuff for the top directory only.
            __SetNodeItems(ItemsTview.Nodes, "");
        }

        private void ItemsTview_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            __SetNodeItems(e.Node.Nodes, e.Node.FullPath);
        }

        private void ItemsTview_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //if (_checkPending)
            //    return;
            //_checkPending = true;

            //__CheckAllChildren(e.Node, e.Node.Checked);

            //if (!e.Node.Checked)
            //{
            //    for (TreeNode parent = e.Node.Parent; parent != null; parent = parent.Parent)
            //        parent.Checked = false;
            //}
            //else
            //{
            //    for (TreeNode parent = e.Node.Parent; parent != null; parent = parent.Parent)
            //    {
            //        parent.Checked = true;
            //        parent.StateImageIndex = 2;
            //    }
            //}

            //_checkPending = false;
        }

        #endregion


        #region ### Private methods ############################################

        private void __CheckAllChildren(TreeNode treeNode, bool checkState)
        {
            foreach (TreeNode child in treeNode.Nodes)
            {
                child.Checked = checkState;

                if (__HasChildren(child))
                    __CheckAllChildren(child, checkState);
            }
        }

        private void __DeepAddNode(TreeNodeCollection treeNodeCollection, TreeBrowseItem newItem)
        {
            TreeNodeCollection currCollection = treeNodeCollection;
            string[] pathList = newItem.FullPath.Split('\\');
            string currentFullPath = "";
            for (int i = 0; i < pathList.Length; i++)
            {
                string currentName = pathList[i];
                TreeNode tnCurrent = __GetNode(currCollection, currentName);
                if (tnCurrent == null)
                {
                    // Get existing or create new node
                    tnCurrent = new TreeNode(currentName);
                    if (i < pathList.Length - 1)
                    {
                        // This is a new folder
                        tnCurrent.ImageIndex = 0;

                        if (_checkedItems != null)
                        {
                            tnCurrent.Checked = _checkedItems.Contains(Path.Combine(currentFullPath, "*"));
                        }
                        else
                            tnCurrent.Checked = false;
                    }
                    else
                    {
                        // We're at the end of the line, so this is a file
                        if (newItem.IconIndex >= 0)
                        {
                            tnCurrent.ImageIndex = newItem.IconIndex;
                            tnCurrent.SelectedImageIndex = tnCurrent.ImageIndex;
                        }

                        if (_checkedItems != null)
                            tnCurrent.Checked = _checkedItems.Contains(newItem.FullPath);
                        else
                            tnCurrent.Checked = false;

                        tnCurrent.Tag = tnCurrent;
                    }

                    // Add whatever we got to the current collection
                    currCollection.Add(tnCurrent);
                }

                // Get the node-list one level down
                currCollection = tnCurrent.Nodes;
                currentFullPath = Path.Combine(currentFullPath, currentName);
            }
        }


        private void __GetCheckedNodes(TreeNodeCollection treeNodeCollection, List<string> outList)
        {
            // Go through the tree
            // If it's a checked folder, stop and return "full\path\*"
            // if it's a 
            foreach (TreeNode tn in treeNodeCollection)
            {
                string fullPath = tn.FullPath;
                if (!string.IsNullOrEmpty(RootFolderName) && fullPath.StartsWith(RootFolderName))
                {
                    fullPath = fullPath.Substring(RootFolderName.Length);
                    fullPath = fullPath.TrimStart('\\');
                }

                // Is this a folder?
                if (tn.Nodes.Count > 0)
                {
                    // If the folder is checked, include everything, else dive in and get all
                    // checked children
                    if (tn.Checked)
                        outList.Add(Path.Combine(fullPath, "*"));
                    else
                        __GetCheckedNodes(tn.Nodes, outList);
                }
                else
                {
                    if (tn.Checked)
                        outList.Add(fullPath);
                }
            }
        }

        private TreeNode __GetNode(TreeNodeCollection collection, string item)
        {
            foreach (TreeNode node in collection)
            {
                if (string.Compare(node.Text, item, true) == 0)
                    return node;
            }

            return null;
        }

        private bool __HasChildren(TreeNode node)
        {
            if (node.Nodes.Count > 1)
                return true;

            if (node.Nodes.Count == 1 && node.Nodes[0].Text != _nullChild)
                return true;

            return false;
        }

        private void __SetNodeItems(TreeNodeCollection treeNodeCollection, string path)
        {
            if (OnGetFiles == null)
                return;

            List<TreeBrowseItem> fileList = OnGetFiles(path);
            if (fileList == null)
                return;

            treeNodeCollection.Clear();
            foreach (TreeBrowseItem item in fileList)
            {
                TreeNode tn = new TreeNode();
                tn.Text = item.Text;

                if (item.IsFolder)
                {
                    tn.Nodes.Add(new TreeNode(_nullChild));
                    tn.ImageIndex = 0;
                    tn.Collapse(true);
                }
                else if (item.IconIndex >= 0)
                    tn.ImageIndex = item.IconIndex;

                tn.Checked = (item.CheckState == TreeBrowseItemCheckstate.True);
                treeNodeCollection.Add(tn);
            }
        }

        #endregion



    }
}
