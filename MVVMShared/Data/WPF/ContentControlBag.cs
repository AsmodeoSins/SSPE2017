using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMShared.Data.WPF
{
    public class ContentControlBag
    {
        public ContentControlBag (object _view, object _viewModel)
        {
            ViewModel = _viewModel;
            View = _view;
        }
        private object viewModel;
        public object ViewModel{
            get { return viewModel; }
            set { viewModel = value; }
        }

        private object view;
        public object View
        {
            get { return view; }
            set { view = value; }
        }
    }
}
