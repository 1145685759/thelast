using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using TheLast.Entities;

namespace TheLast.Dtos
{
    /// <summary>
    /// 值字典DTO
    /// </summary>
    public class ValueDictionaryDto: BindableBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private string displayValue;
        public string DisplayValue
        {
            get { return displayValue; }
            set { SetProperty(ref displayValue, value); }
        }
        private string realValue;
        public string RealValue
        {
            get { return realValue; }
            set { SetProperty(ref realValue, value); }
        }
        private int registerId;
        public int RegisterId
        {
            get { return registerId; }
            set { SetProperty(ref registerId, value); }
        }
        private Register register;
        public Register Register
        {
            get { return register; }
            set { SetProperty(ref register, value); }
        }
    }
}
