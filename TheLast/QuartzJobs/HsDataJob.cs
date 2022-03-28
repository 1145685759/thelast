using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheLast.Entities;

namespace TheLast.QuartzJobs
{
    public class HsDataJob : IJob
    {
        private readonly ISqlSugarClient sqlSugarClient;

        public async Task Execute(IJobExecutionContext context)
        {
            var registers= await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true).ToListAsync();
            foreach (var item in registers)
            {
               await sqlSugarClient.Insertable(new HsData {DateTime=DateTime.Now,Register=item,RegisterId=item.Id,RealValue=(await App.ModbusSerialMaster.ReadHoldingRegistersAsync(item.StationNum,item.Address,1))[0]}).ExecuteCommandAsync();
            }
            
        }
        public HsDataJob(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }
    }
}
