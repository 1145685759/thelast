using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using TheLast.Entities;

namespace TheLast.Dtos
{
    /// <summary>
    /// 寄存器DTO
    /// </summary>
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
        private bool isDisplay;
        public bool IsDisplay
        {
            get { return isDisplay; }
            set { SetProperty(ref isDisplay, value); }
        }
        private byte stationNum=1;
        public byte StationNum
        {
            get { return stationNum; }
            set { SetProperty(ref stationNum, value); }
        }
        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }
        private bool isHsData;
        public bool IsHsData
        {
            get { return isHsData; }
            set { SetProperty(ref isHsData, value); }
        }
        private int type;
        public int Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }
        private ushort allowableRangeDeviation;
        public ushort AllowableRangeDeviation
        {
            get { return allowableRangeDeviation; }
            set { SetProperty(ref allowableRangeDeviation, value); }
        }
        private int caste;
        public int Caste
        {
            get { return caste; }
            set { SetProperty(ref caste, value); }
        }
        private int fineTuning;
        public int FineTuning
        {
            get { return fineTuning; }
            set { SetProperty(ref fineTuning, value); }
        }
        private ushort accessAddress;
        public ushort AccessAddress
        {
            get { return accessAddress; }
            set { SetProperty(ref accessAddress, value); }
        }
    }
}
