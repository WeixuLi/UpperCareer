using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    abstract public class UpperInfoBaseModel
    {
        public string ID { get; set; }

        public UserModel Creator { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string VoteDiff { get; set; }

        public string SubPostCount { get; set; }

        public string CreateBefore { get; set; }

        public string UpdateBefore { get; set; }

        protected void ConvertInfoObjectToModel(UpperInfoBaseDomain obj)
        {
            ID = obj.ObjectID;
            Creator = obj.CreatedBy != null ? new UserModel(obj.CreatedBy) : null;
            Content = obj.Content;
            Title = obj.Title;
            VoteDiff = ProcessLargeNumDisplay(obj.VoteDiff);
            SubPostCount = ProcessLargeNumDisplay(obj.SubPostCount);
            CreateBefore = GenerateTimeDisplay(obj.DateCreate);
            UpdateBefore = GenerateTimeDisplay(obj.DateUpdate);
        }

        protected string GenerateTimeDisplay(DateTime date)
        {
            if (date != default(DateTime))
            {
                TimeSpan diff = DateTime.Now.Subtract(date);
                return diff.Days > 365 ? string.Format("{0}年前", diff.Days / 365) : diff.Days > 30 ? string.Format("{0}个月前", diff.Days / 30) : diff.Days > 0 ? string.Format("{0}天前", diff.Days) : diff.Hours > 0 ? string.Format("{0}小时前", diff.Hours) : string.Format("{0}分钟前", diff.Minutes);
            }

            return "未知时间";
        }

        protected string ProcessLargeNumDisplay(int num)
        {
            return num > 1000 ? string.Format("{0}K", num / 1000) : num.ToString();
        }
    }

    abstract public class UpperQACBaseModel
    {
        public string ID { get; set; }

        public UserModel Creator { get; set; }

        public string Content { get; set; }

        public string CreateBefore { get; set; }

        public string UpdateBefore { get; set; }

        protected void ConvertQACObjectToModel(UpperQACBaseDomain obj)
        {
            ID = obj.ObjectID;
            Creator = obj.CreatedBy != null ? new UserModel(obj.CreatedBy) : null;
            Content = obj.Content;
            CreateBefore = GenerateTimeDisplay(obj.DateCreate);
            UpdateBefore = GenerateTimeDisplay(obj.DateUpdate);
        }

        protected string GenerateTimeDisplay(DateTime date)
        {
            if (date != default(DateTime))
            {
                TimeSpan diff = DateTime.Now.Subtract(date);
                return diff.Days > 365 ? string.Format("{0}年前", diff.Days / 365) : diff.Days > 30 ? string.Format("{0}个月前", diff.Days / 30) : diff.Days > 0 ? string.Format("{0}天前", diff.Days) : diff.Hours > 0 ? string.Format("{0}小时前", diff.Hours) : string.Format("{0}分钟前", diff.Minutes);
            }

            return "未知时间";
        }

        protected string ProcessLargeNumDisplay(int num)
        {
            return num > 1000 ? string.Format("{0}K", num / 1000) : num.ToString();
        }
    }

    abstract public class UpperArticleBaseModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string Cover { get; set; }

        public string Author { get; set; }

        public int Index { get; set; }

        public int ViewCount { get; set; }

        public string DateCreate { get; set; }

        public string DateUpdate { get; set; }

        protected virtual void ConvertArticleObjectToModel(UpperArticleBaseDomain obj)
        {
            Random rd = new Random();

            ID = obj.ObjectID;
            Title = obj.Title;
            Cover = obj.Cover;
            Author = obj.Author;
            Index = obj.Index;
            ViewCount = obj.ViewCount * 31 + rd.Next(80, 100);
            DateCreate = obj.DateCreate.ToString("yyyy-M-d");
            DateUpdate = obj.DateUpdate.ToString("yyyy-M-d");
        }
    }

    abstract public class UpperHistoryBaseModel
    {
        public string ID { get; set; }

        public UserModel From { get; set; }

        public string[] NameStrings { get; set; }

        public string[] InfoStrings { get; set; }

        public string DateCreate { get; set; }

        public string DateUpdate { get; set; }

        protected string GenerateTimeDisplay(DateTime date)
        {
            if (date != default(DateTime))
            {
                TimeSpan diff = DateTime.Now.Subtract(date);
                return diff.Days > 365 ? string.Format("{0}年前", diff.Days / 365) : diff.Days > 30 ? string.Format("{0}个月前", diff.Days / 30) : diff.Days > 0 ? string.Format("{0}天前", diff.Days) : diff.Hours > 0 ? string.Format("{0}小时前", diff.Hours) : string.Format("{0}分钟前", diff.Minutes);
            }

            return "未知时间";
        }
    }

    abstract public class UpperFeedBaseModel
    {
        public string ID { get; set; }

        public int MyProperty { get; set; }
    }
}