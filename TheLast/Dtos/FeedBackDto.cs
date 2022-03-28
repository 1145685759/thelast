using Prism.Mvvm;
using TheLast.Entities;

namespace TheLast.Dtos
{
    public class FeedBackDto: BindableBase
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
        private string tagetValue;
        public string TagetValue
        {
            get { return tagetValue; }
            set { SetProperty(ref tagetValue, value); }
        }
        private int testStepId;
        public int TestStepId
        {
            get { return testStepId; }
            set { SetProperty(ref testStepId, value); }
        }
        private string displayDelayMode;
        public string DisplayDelayMode
        {
            get { return displayDelayMode; }
            set { SetProperty(ref displayDelayMode, value); }
        }
        private string displayTagetValue;
        public string DisplayTagetValue
        {
            get { return displayTagetValue; }
            set { SetProperty(ref displayTagetValue, value); }
        }
        private TestStep testStep;
        public TestStep TestStep
        {
            get { return testStep; }
            set { SetProperty(ref testStep, value); }
        }
        private int delayTime;
        public int DelayTime
        {
            get { return delayTime; }
            set { SetProperty(ref delayTime, value); }
        }
        private int delayModeId;
        public int DelayModeId
        {
            get { return delayModeId; }
            set { SetProperty(ref delayModeId, value); }
        }
        private DelayModel delayModel;
        public DelayModel DelayModel
        {
            get { return delayModel; }
            set { SetProperty(ref delayModel, value); }
        }
        private string address;
        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }
        private byte stationNum=1;
        public byte StationNum
        {
            get { return stationNum; }
            set { SetProperty(ref stationNum, value); }
        }
    }
}
