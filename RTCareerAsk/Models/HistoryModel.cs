using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;
using RTCareerAsk.App_DLL;

namespace RTCareerAsk.Models
{
    public class HistoryModel
    {
        public HistoryModel() { }

        public HistoryModel(History hsty)
        {
            ConvertHistoryObjectToModel(hsty);
        }

        public string ID { get; set; }

        public UserModel User { get; set; }

        public UserModel Target { get; set; }

        public HistoryType Type { get; set; }

        public bool IsNew { get; set; }

        public string[] NameStrings { get; set; }

        public string[] InfoStrings { get; set; }

        public string DateCreate { get; set; }

        public string DateUpdate { get; set; }

        private void ConvertHistoryObjectToModel(History hsty)
        {
            ID = hsty.ObjectID;
            User = new UserModel(hsty.FromUser);
            Target = new UserModel(hsty.ForUser);
            Type = (HistoryType)hsty.Type;
            IsNew = hsty.IsNew;
            NameStrings = hsty.CompoundNameString.Split(';');
            InfoStrings = hsty.CompoundInfoString.Split(';');
            DateCreate = hsty.DateCreate.ToString("yyyy/MM/dd");
            DateUpdate = hsty.DateUpdate.ToString("yyyy/MM/dd");
        }

        public History CreateHistoryForSave()
        {
            if (Target.UserID == User.UserID)
            {
                return null;
            }
            else if (NameStrings.Length != InfoStrings.Length)
            {
                throw new InvalidOperationException("名称变量数与信息变量数不匹配。");
            }

            string compoundedName = "";
            string compoundedInfo = "";

            for (int i = 0; i < NameStrings.Length; i++)
            {
                compoundedName += NameStrings[i] + ";";
                compoundedInfo += InfoStrings[i] + ";";
            }

            return new History()
            {
                FromUser = new User() { ObjectID = User.UserID },
                ForUser = new User() { ObjectID = Target.UserID },
                Type = (int)Type,
                CompoundNameString = compoundedName,
                CompoundInfoString = compoundedInfo
            };
        }
    }

    public class NotificationModel : UpperHistoryBaseModel
    {
        public NotificationModel() { }

        public NotificationModel(History hsty)
        {
            ConvertHistoryObjectToNotificationModel(hsty);
        }

        public NotificationType Type { get; set; }

        public bool IsNew { get; set; }

        private void ConvertHistoryObjectToNotificationModel(History hsty)
        {
            ID = hsty.ObjectID;
            From = new UserModel(hsty.FromUser);
            Type = (NotificationType)hsty.Type;
            IsNew = hsty.IsNew;
            NameStrings = hsty.CompoundNameString.Split(';');
            InfoStrings = hsty.CompoundInfoString.Split(';');
            DateCreate = GenerateTimeDisplay(hsty.DateCreate);
            DateUpdate = GenerateTimeDisplay(hsty.DateUpdate);
        }
    }

    public class FeedModel : UpperHistoryBaseModel
    {
        public FeedModel() { }

        public FeedModel(History hsty)
        {
            ConvertHistoryObjectToFeedModel(hsty);
        }

        public FeedType Type { get; set; }

        public UserModel ForUser { get; set; }

        public UpperInfoBaseModel Content { get; set; }

        private void ConvertHistoryObjectToFeedModel(History hsty)
        {
            ID = hsty.ObjectID;
            From = new UserModel(hsty.FromUser);
            ForUser = new UserModel(hsty.ForUser);
            Type = (FeedType)hsty.Type;
            NameStrings = hsty.CompoundNameString.Split(';');
            InfoStrings = hsty.CompoundInfoString.Split(';');
            DateCreate = GenerateTimeDisplay(hsty.DateCreate);
            DateUpdate = GenerateTimeDisplay(hsty.DateUpdate);
        }
    }
}