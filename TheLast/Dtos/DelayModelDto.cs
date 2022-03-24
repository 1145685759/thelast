using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    public class DelayModelDto: BindableBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private string mode;
        public string Mode
        {
            get { return mode; }
            set { SetProperty(ref mode, value); }
        }
    }
}
