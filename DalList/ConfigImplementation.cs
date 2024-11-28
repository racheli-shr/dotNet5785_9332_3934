using DalApi;
using DO;
using System;
namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public int NextAssignmentId
    {
        get => Config.NextAssignmentId;
        set => Config.NextAssignmentId = value;
    }
    public int NextCallId
    {
        get => Config.NextCallId;
        set => Config.NextCallId = value;
    }
    public void Reset()
    {
        Config.Reset();
    }
}
//namespace Dal
//{
//    using DalApi;
//    using DO;
//    using System.Collections.Generic;

//    public class CallImplementation : ICall
//    {
//        public int Create(Call item)
//        {
//            Call newCall = item with { Id = Config.NextCallId };
//            Config.NextCallId++;
//            DataSource.Calls.Add(newCall);
//            return newCall.Id;
//        }

//        public void Delete(int id)
//        {
//            Call? c = Read(id);
//            if (c != null)
//            {
//                DataSource.Calls.Remove(c);
//            }
//            else
//            {
//                throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים");
//            }
//        }

//        public void DeleteAll()
//        {
//            DataSource.Calls.Clear();
//        }

//        public Call? Read(int id)
//        {
//            return DataSource.Calls.Find(call => call.Id == id);
//        }

//        public List<Call> ReadAll()
//        {
//            return new List<Call>(DataSource.Calls);
//        }

//        public void Update(Call item)
//        {
//            int index = DataSource.Calls.FindIndex(call => call.Id == item.Id);
//            if (index >= 0)
//            {
//                DataSource.Calls[index] = item;
//            }
//            else
//            {
//                throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים");
//            }
//        }
//    }
//}


