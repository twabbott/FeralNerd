using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeralNerdLib.WinForms
{
    public static class TreeViewExtensions
    {
        public static bool AddInOrder(this TreeNodeCollection self, TreeNode child, bool ignoreCase = true, bool allowDuplicates = false)
        {
            for (int i = 0; i < self.Count; i++)
            {
                int cmp = string.Compare(child.Text, self[i].Text, ignoreCase);
                if (cmp <= 0)
                {
                    if (cmp == 0 && !allowDuplicates)
                        return false;

                    self.Insert(i, child);
                    return true;
                }
            }

            self.Add(child);
            return true;
        }

        public static TreeNode Find(this TreeNodeCollection self, string text, bool ignoreCase = true)
        {
            foreach (TreeNode tn in self)
            {
                if (string.Compare(tn.Text, text, ignoreCase) == 0)
                    return tn;
            }

            return null;
        }
    }
}
