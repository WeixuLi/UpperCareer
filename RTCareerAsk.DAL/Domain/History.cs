using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class History
    {
        public History() { }

        public History(AVObject obj)
        {
            GenerateNotificationObject(obj);
        }

        public History(string userId, int type, string nameString, string infoString)
        {
            GenerateNotificationObject(userId, type, nameString, infoString);
        }

        public string ObjectID { get; set; }

        public User ForUser { get; set; }

        public User FromUser { get; set; }

        public int Type { get; set; }

        public bool IsNew { get; set; }

        public string CompoundNameString { get; set; }

        public string CompoundInfoString { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime DateUpdate { get; set; }

        private void GenerateNotificationObject(AVObject obj)
        {
            if (obj.ClassName != "History")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是提醒类object。对象类型：{1}", obj.ObjectId, obj.ClassName));
            }

            ObjectID = obj.ObjectId;
            ForUser = obj.ContainsKey("forUser") ? new User(obj.Get<AVUser>("forUser")) : default(User);
            FromUser = obj.ContainsKey("from") ? new User(obj.Get<AVUser>("from")) : default(User);
            Type = obj.Get<int>("type");
            IsNew = obj.Get<bool>("isNew");
            CompoundNameString = !string.IsNullOrEmpty(obj.Get<string>("nameString")) ? obj.Get<string>("nameString") : string.Empty;
            CompoundInfoString = !string.IsNullOrEmpty(obj.Get<string>("infoString")) ? obj.Get<string>("infoString") : string.Empty;
            DateCreate = Convert.ToDateTime(obj.CreatedAt);
            DateUpdate = Convert.ToDateTime(obj.UpdatedAt);
        }

        private void GenerateNotificationObject(string userId, int type, string nameString, string infoString)
        {
            if (type < 5 || type > 8)
            {
                throw new ArgumentOutOfRangeException("此构建函数仅支持指定类型提醒记录，输入类型：" + type.ToString());
            }

            switch (type)
            {
                case 6:
                    ForUser = new User() { ObjectID = userId };
                    CompoundNameString = nameString;
                    CompoundInfoString = infoString;
                    break;
                case 7:
                    ForUser = new User() { ObjectID = userId };
                    FromUser = new User() { ObjectID = infoString };
                    break;
                case 8:
                    FromUser = new User() { ObjectID = infoString };
                    CompoundNameString = nameString;
                    CompoundInfoString = infoString;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("此构建函数仅支持指定类型提醒记录，输入类型：" + type.ToString());
            }

            Type = type;
        }

        public AVObject CreateNotificationObjectForSave()
        {
            AVObject notification = new AVObject("History");

            notification.Add("from", FromUser != null ? FromUser.LoadUserObject() : null);
            notification.Add("forUser", ForUser != null ? ForUser.LoadUserObject() : null);
            notification.Add("type", Type);
            notification.Add("isNew", true);
            notification.Add("nameString", CompoundNameString);
            notification.Add("infoString", CompoundInfoString);

            return notification;
        }

        public History UpdateInfoString(string info)
        {
            CompoundInfoString = info + ";";

            return this;
        }

        public string ReadInfoStringByIndex(int index)
        {
            if (!string.IsNullOrEmpty(CompoundInfoString))
            {
                return CompoundInfoString.Split(';')[index];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
