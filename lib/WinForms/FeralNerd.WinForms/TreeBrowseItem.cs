using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeralNerd.WinForms
{
    public enum TreeBrowseItemCheckstate
    {
        False,
        Intermediate,
        True
    }

    public class TreeBrowseItem
    {
        public string Text = null;
        public string FullPath = null;
        public int IconIndex = -1;
        public bool IsFolder = false;
        public TreeBrowseItemCheckstate CheckState = TreeBrowseItemCheckstate.False;

        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();

        public string GetProperty(string propName, string defaultValue)
        {
            if (Properties.ContainsKey(propName))
                return Properties[propName];

            return defaultValue;
        }
    }
}
