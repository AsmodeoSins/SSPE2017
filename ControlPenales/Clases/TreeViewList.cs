using SSP.Servidor;
using System.Collections.Generic;

namespace ControlPenales.Clases
{
    public class TreeViewList : ViewModelBase
    {
        private object _Value;
        private string _Text;
        private string _Icon;
        private bool _IsRoot;
        private bool? _IsCheck;
        private bool _IsNodeExpanded;
        private List<TreeViewList> _Node;

        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                OnPropertyChanged("Text");
            }
        }

        public string Icon
        {
            get { return _Icon; }
            set
            {
                _Icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public bool IsRoot
        {
            get { return _IsRoot; }
            set
            {
                _IsRoot = value;
                OnPropertyChanged("IsRoot");
            }
        }

        public bool? IsCheck
        {
            get { return _IsCheck; }
            set
            {
                _IsCheck = value;
                OnPropertyChanged("IsCheck");
            }
        }

        public bool IsNodeExpanded
        {
            get { return _IsNodeExpanded; }
            set
            {
                _IsNodeExpanded = value;
                OnPropertyChanged("IsNodeExpanded");
            }
        }

        public List<TreeViewList> Node
        {
            get { return _Node; }
            set
            {
                _Node = value;
                OnPropertyChanged("Node");
            }
        }
    }

    public static class TreeViewExtensions
    {
        static bool checkexpand = false;

        public static void FindElement(this List<TreeViewList> list, INGRESO TreeViewItem)
        {
            foreach (var item in list)
            {
                if (item.HasChildren())
                    FindElement(item.Node, TreeViewItem);
                else
                {
                    if (((CAMA)item.Value).ID_CENTRO == TreeViewItem.ID_UB_CENTRO && ((CAMA)item.Value).ID_EDIFICIO == TreeViewItem.ID_UB_EDIFICIO && ((CAMA)item.Value).ID_SECTOR == TreeViewItem.ID_UB_SECTOR && ((CAMA)item.Value).ID_CELDA.Trim().ToUpper() == TreeViewItem.ID_UB_CELDA.Trim().ToUpper() && ((CAMA)item.Value).ID_CAMA == TreeViewItem.ID_UB_CAMA)
                        item.IsCheck = true;
                }
            }
        }

        public static void UnCheckAll(this List<TreeViewList> list)
        {
            foreach (var item in list)
            {
                if (item.HasChildren())
                {
                    UnCheckAll(item.Node);
                    item.IsCheck = false;
                }
                else
                    item.IsCheck = false;
            }
        }

        public static List<TreeViewList> Restart(this List<TreeViewList> item, bool isRestar = true)
        {
            checkexpand = !isRestar;
            return item;
        }

        public static void Expand(this List<TreeViewList> list)
        {
            foreach (var item in list)
            {
                if (checkexpand)
                    break;

                if (item.IsRoot)
                    checkexpand = false;

                if (item.HasChildren())
                {
                    Expand(item.Node);
                    if (checkexpand)
                    {
                        item.IsNodeExpanded = true;
                        item.IsCheck = null;
                    }
                }
                else
                {
                    if (item.IsCheck.HasValue)
                        if (item.IsCheck.Value)
                        {
                            checkexpand = true;
                            return;
                        }
                }
            }
        }

        public static void Collapse(this List<TreeViewList> list)
        {
            foreach (var item in list)
            {
                if (item.HasChildren())
                {
                    Collapse(item.Node);
                    item.IsNodeExpanded = false;
                }
                else
                    item.IsNodeExpanded = false;
            }
        }

        public static bool HasChildren(this TreeViewList item)
        {
            return item.Node != null ? item.Node.Count <= 0 ? false : true : false;
        }

        public static bool HasCheked(this List<TreeViewList> item)
        {
            return CountCheked(item);
        }

        internal static bool CountCheked(List<TreeViewList> list)
        {
            foreach (var item in list)
            {

                if (item.HasChildren())
                {
                    if (CountCheked(item.Node))
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (item.IsCheck.HasValue)
                        if (item.IsCheck.Value)
                        {
                            return true;
                        }
                }
            }
            return false;
        }
    }
}
