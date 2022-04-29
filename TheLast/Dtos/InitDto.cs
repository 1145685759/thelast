using Prism.Mvvm;
using SqlSugar;
using TheLast.Entities;

namespace TheLast.Dtos
{
    /// <summary>
    /// 初始化DTO
    /// </summary>
    public class InitDto: BindableBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
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
        private string writeValue;
        public string WriteValue
        {
            get { return writeValue; }
            set { SetProperty(ref writeValue, value); }
        }
        private int testStepId;
        public int TestStepId
        {
            get { return testStepId; }
            set { SetProperty(ref testStepId, value); }
        }
        private TestStep testStep;
        public TestStep TestStep
        {
            get { return testStep; }
            set { SetProperty(ref testStep, value); }
        }
        private string address;
        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }
        private string displayValue;
        public string DisplayValue
        {
            get { return displayValue; }
            set { SetProperty(ref displayValue, value); }
        }
        private byte stationNum ;
        public byte StationNum
        {
            get { return stationNum; }
            set { SetProperty(ref stationNum, value); }
        }
    }
}
