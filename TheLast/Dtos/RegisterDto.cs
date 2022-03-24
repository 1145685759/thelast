using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using TheLast.Entities;

namespace TheLast.Dtos
{
    public class RegisterDto: BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private ushort address;
        public ushort Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }
        private string registerType;
        public string RegisterType
        {
            get { return registerType; }
            set { SetProperty(ref registerType, value); }
        }
        private bool isEnable=false;
        public bool IsEnable
        {
            get { return isEnable; }
            set { SetProperty(ref isEnable, value); }
        }
    }
}
