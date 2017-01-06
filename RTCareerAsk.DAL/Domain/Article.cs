using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class ArticleInfo : UpperArticleBaseDomain
    {
        public ArticleInfo() { }

        public ArticleInfo(AVObject obj)
        {
            GenerateArticleBaseObject(obj);
        }
    }

    public class Article : UpperArticleBaseDomain
    {
        public Article()
        {
            Comments = new List<ArticleComment>();
        }

        public Article(AVObject obj)
        {
            Comments = new List<ArticleComment>();

            GenerateArticleObject(obj);
        }

        public string Content { get; set; }

        public User Editor { get; set; }

        public Answer Reference { get; set; }

        public bool HasReference { get; set; }

        public List<ArticleComment> Comments { get; set; }

        private void GenerateArticleObject(AVObject obj)
        {
            GenerateArticleBaseObject(obj);
            Content = obj.ContainsKey("content") ? obj.Get<string>("content") : default(string);
            Editor = obj.ContainsKey("editor") ? new User(obj.Get<AVUser>("editor")) : default(User);
            Reference = obj.ContainsKey("reference") && obj.Get<AVObject>("reference") != null ? new Answer(obj.Get<AVObject>("reference")) : default(Answer);
            HasReference = obj.ContainsKey("reference") && obj.Get<AVObject>("reference") != null;
        }

        public Article SetComments(IEnumerable<AVObject> cmts)
        {
            if (cmts.Count() > 0 && cmts.First().ClassName != "Comment_Article")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是资讯评论类object。对象类型：{1}", cmts.First().ObjectId, cmts.First().ClassName));
            }

            Comments.AddRange(cmts.Select(x => new ArticleComment(x)));

            return this;
        }

        public AVObject CreateArticleObjectForSave()
        {
            AVObject atcl = new AVObject("Article");

            atcl.Add("title", Title);
            atcl.Add("cover", Cover);
            atcl.Add("author", Author);
            atcl.Add("index", Index);
            atcl.Add("content", Content);
            atcl.Add("editor", Editor.LoadUserObject());
            atcl.Add("reference", Reference != default(Answer) ? Reference.LoadAnswerObject() : null);

            return atcl;
        }

        public AVObject LoadArticleObject()
        {
            return AVObject.CreateWithoutData("Article", ObjectID);
        }
    }

    public class ArticleComment
    {
        public ArticleComment() { }

        public ArticleComment(AVObject obj)
        {
            GenerateArticleCommentObject(obj);
        }

        public string ObjectID { get; set; }

        public string Content { get; set; }

        public Article ForArticle { get; set; }

        public User Creator { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime DateUpdate { get; set; }

        private void GenerateArticleCommentObject(AVObject obj)
        {
            if (obj.ClassName != "Comment_Article")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是资讯评论类object。对象类型：{1}", obj.ObjectId, obj.ClassName));
            }

            ObjectID = obj.ObjectId;
            Content = obj.Get<string>("content");
            ForArticle = obj.ContainsKey("forArticle") ? new Article() { ObjectID = obj.Get<AVObject>("forArticle").ObjectId } : default(Article);
            Creator = obj.ContainsKey("createdBy") ? new User(obj.Get<AVUser>("createdBy")) : default(User);
            DateCreate = Convert.ToDateTime(obj.CreatedAt);
            DateUpdate = Convert.ToDateTime(obj.UpdatedAt);
        }

        public AVObject CreateArticleCommentObjectForSave()
        {
            AVObject acmt = new AVObject("Comment_Article");

            acmt.Add("content", Content);
            acmt.Add("createdBy", Creator.LoadUserObject());
            acmt.Add("forArticle", ForArticle.LoadArticleObject());

            return acmt;
        }
    }

    public class ArticleReference
    {
        public ArticleReference()
        {
            TopArticles = new List<ArticleInfo>();
        }

        public ArticleReference(AVObject ans)
        {
            TopArticles = new List<ArticleInfo>();

            GenerateArticleReferenceObject(ans);
        }

        public Answer Reference { get; set; }

        public List<ArticleInfo> TopArticles { get; set; }

        private void GenerateArticleReferenceObject(AVObject ans)
        {
            if (ans.ClassName != "Answer")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是答案类object。对象类型：{1}", ans.ObjectId, ans.ClassName));
            }

            Reference = new Answer(ans);
        }

        public ArticleReference SetTopArticles(IEnumerable<AVObject> atcls)
        {
            if (atcls.Count() > 0 && atcls.First().ClassName != "Article")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是资讯类object。对象类型：{1}", atcls.First().ObjectId, atcls.First().ClassName));
            }

            TopArticles.AddRange(atcls.Select(x => new ArticleInfo(x)));

            return this;
        }
    }
}
