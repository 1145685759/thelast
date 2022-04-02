﻿using Prism.Mvvm;
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
        private bool isVisible = false;
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
        private ushort type;
        public ushort Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }
        private ushort caste;
        public ushort Caste
        {
            get { return caste; }
            set { SetProperty(ref caste, value); }
        }
        private ushort startAddress;
        public ushort StartAddress
        {
            get { return startAddress; }
            set { SetProperty(ref startAddress, value); }
        }
    }
}
