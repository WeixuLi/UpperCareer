using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    abstract public class UpperInfoBaseDomain
    {
        public string ObjectID { get; set; }

        public User CreatedBy { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int SubPostCount { get; set; }

        public int VoteDiff { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime DateUpdate { get; set; }

        protected void GenerateInfoObject(AVObject obj)
        {
            ObjectID = obj.ObjectId;
            CreatedBy = obj.ContainsKey("createdBy") ? new User(obj.Get<AVUser>("createdBy")) : null;
            Title = obj.ContainsKey("title") ? obj.Get<string>("title") : null;
            Content = obj.ContainsKey("content") ? obj.Get<string>("content") : null;
            SubPostCount = obj.ContainsKey("subPostCount") ? obj.Get<int>("subPostCount") : default(int);
            VoteDiff = obj.ContainsKey("voteDiff") ? obj.Get<int>("voteDiff") : default(int);
            DateCreate = Convert.ToDateTime(obj.CreatedAt);
            DateUpdate = Convert.ToDateTime(obj.UpdatedAt);
        }
    }

    abstract public class UpperQACBaseDomain
    {
        public string ObjectID { get; set; }

        public string Content { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime DateUpdate { get; set; }

        public User CreatedBy { get; set; }

        protected void GenerateQACObject(AVObject obj)
        {
            ObjectID = obj.ObjectId;
            Content = obj.ContainsKey("content") ? obj.Get<string>("content") : null;
            DateCreate = Convert.ToDateTime(obj.CreatedAt);
            DateUpdate = Convert.ToDateTime(obj.UpdatedAt);
            CreatedBy = obj.ContainsKey("createdBy") ? new User(obj.Get<AVUser>("createdBy")) : null;
        }
    }

    abstract public class UpperVoteBaseDomain
    {
        public string ObjectID { get; set; }

        public bool IsLike { get; set; }

        public User VoteBy { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime DateUpdate { get; set; }

        protected void GenerateVoteObject(AVObject obj)
        {
            ObjectID = obj.ObjectId;
            IsLike = obj.ContainsKey("isLike") ? obj.Get<bool>("isLike") : false;
            VoteBy = obj.ContainsKey("voteBy") ? new User(obj.Get<AVUser>("voteBy")) : null;
            DateCreate = Convert.ToDateTime(obj.CreatedAt);
            DateUpdate = Convert.ToDateTime(obj.UpdatedAt);
        }
    }

    abstract public class UpperArticleBaseDomain
    {
        public string ObjectID { get; set; }

        public string Title { get; set; }

        public string Cover { get; set; }

        public string Author { get; set; }

        public int Index { get; set; }

        public int ViewCount { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime DateUpdate { get; set; }

        protected void GenerateArticleBaseObject(AVObject obj)
        {
            if (obj.ClassName != "Article")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是资讯类object。对象类型：{1}", obj.ObjectId, obj.ClassName));
            }

            ObjectID = obj.ObjectId;
            Title = obj.ContainsKey("title") ? obj.Get<string>("title") : default(string);
            Cover = obj.ContainsKey("cover") ? obj.Get<string>("cover") : default(string);
            Author = obj.ContainsKey("author") ? obj.Get<string>("author") : default(string);
            Index = obj.ContainsKey("index") ? obj.Get<int>("index") : default(int);
            ViewCount = obj.ContainsKey("viewCount") ? obj.Get<int>("viewCount") : default(int);
            DateCreate = Convert.ToDateTime(obj.CreatedAt);
            DateUpdate = Convert.ToDateTime(obj.UpdatedAt);
        }
    }
}
