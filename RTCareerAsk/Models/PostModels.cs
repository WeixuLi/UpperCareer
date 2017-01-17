using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RTCareerAsk.App_DLL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class QuestionPostModel
    {
        [Required(ErrorMessage = "请输入标题")]
        [StringLength(30, ErrorMessage = "标题请不要超过{1}字")]
        [DisplayName("标题：")]
        public string PostTitle { get; set; }
        [DisplayName("正文：")]
        [StringLength(1500, ErrorMessage = "超过字数上限，正文请不要超过{1}字")]
        public string PostContent { get; set; }

        public string UserID { get; set; }

        public Question CreatePostForSave()
        {
            return new Question()
            {
                Title = PostTitle,
                Content = PostContent,
                CreatedBy = new User() { ObjectID = UserID }
            };
        }
    }

    public class AnswerPostModel
    {
        [Required(ErrorMessage = "请您输入答案正文")]
        [DisplayName("答案内容：")]
        [StringLength(30000, ErrorMessage = "超过字数上限，答案请不要超过{1}字")]
        public string PostContent { get; set; }

        public string QuestionID { get; set; }

        public string QuestionTitle { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string NotifyUserID { get; set; }

        private HistoryModel GenerateNotification()
        {
            return new HistoryModel()
            {
                User = new UserModel() { UserID = UserID },
                Target = new UserModel() { UserID = NotifyUserID },
                Type = HistoryType.Answered,
                NameStrings = new string[] { QuestionTitle },
                InfoStrings = new string[] { QuestionID }
            };
        }

        public Answer CreatePostForSave()
        {
            return new Answer()
            {
                Content = PostContent,
                ForQuestion = new Question() { ObjectID = QuestionID },
                CreatedBy = new User() { ObjectID = UserID },
                Notification = GenerateNotification().CreateHistoryForSave()
            };
        }
    }

    public class CommentPostModel
    {
        [Required(ErrorMessage = "请您输入评论正文")]
        [DisplayName("评论内容：")]
        [StringLength(500, ErrorMessage = "超过字数上限，评论请不要超过{1}字")]
        public string PostContent { get; set; }

        public string AnswerID { get; set; }

        public string QuestionTitle { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string AuthorID { get; set; }

        public string NotifyUserID { get; set; }

        private HistoryModel GenerateNotification()
        {
            return new HistoryModel()
            {
                User = new UserModel() { UserID = UserID },
                Target = new UserModel() { UserID = string.IsNullOrEmpty(NotifyUserID) ? AuthorID : NotifyUserID },
                Type = string.IsNullOrEmpty(NotifyUserID) ? HistoryType.CommentAns : HistoryType.RepliedCmt,
                NameStrings = new string[] { QuestionTitle },
                InfoStrings = new string[] { AnswerID }
            };
        }

        public Comment CreatePostForSave()
        {
            return new Comment()
            {
                Content = PostContent,
                ForAnswer = new Answer() { ObjectID = AnswerID },
                CreatedBy = new User() { ObjectID = UserID },
                Notification = GenerateNotification().CreateHistoryForSave()
            };
        }
    }

    public class ArticlePostModel
    {
        public ArticlePostModel() { }

        public ArticlePostModel(ArticleReference refs)
        {
            TopArticles = new List<ArticleInfoModel>();

            ConvertReferenceObjectToReferenceInfo(refs);
        }

        [Required(ErrorMessage = "请输入文章标题")]
        [StringLength(50, ErrorMessage = "标题请不要超过{1}字")]
        [DisplayName("标题：")]
        public string Title { get; set; }
        [Required(ErrorMessage = "封面图不能为空")]
        [DisplayName("封面图片：")]
        public string Cover { get; set; }
        [Required(ErrorMessage = "请输入原作者名字")]
        [DisplayName("原作者：")]
        public string Author { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "请输入大于{1}的数字")]
        [DisplayName("序号：")]
        public int? Index { get; set; }
        [Required(ErrorMessage = "正文内容不能为空")]
        [StringLength(20000, ErrorMessage = "正文请不要超过{1}字")]
        public string Content { get; set; }

        public string EditorID { get; set; }

        public AnswerModel Reference { get; set; }

        public string ReferenceID { get; set; }

        public bool HasReference { get; set; }

        public List<ArticleInfoModel> TopArticles { get; set; }

        private void ConvertReferenceObjectToReferenceInfo(ArticleReference refs)
        {
            if (refs.Reference != null)
            {
                Reference = new AnswerModel(refs.Reference);
                ReferenceID = refs.Reference.ObjectID;
                Author = refs.Reference.CreatedBy.Name;
                HasReference = true;
            }

            if (refs.TopArticles != null && refs.TopArticles.Count > 0)
            {
                foreach (ArticleInfo info in refs.TopArticles)
                {
                    TopArticles.Add(new ArticleInfoModel(info));
                }
            }
        }

        public Article CreatePostForSave()
        {
            return new Article()
            {
                Title = Title,
                Cover = Cover,
                Author = Author,
                Index = Index != null ? Convert.ToInt32(Index) : 0,
                Content = Content,
                Editor = new User() { ObjectID = EditorID },
                Reference = !string.IsNullOrEmpty(ReferenceID) ? new Answer() { ObjectID = ReferenceID } : default(Answer)
            };
        }
    }

    public class ArticleCommentPostModel
    {
        [Required(ErrorMessage = "请您输入评论正文")]
        [DisplayName("评论内容：")]
        [StringLength(500, ErrorMessage = "超过字数上限，评论请不要超过{1}字")]
        public string PostContent { get; set; }

        public string ArticleID { get; set; }

        public string UserID { get; set; }

        public string NotifyUserID { get; set; }

        public ArticleComment CreatePostForSave()
        {
            return new ArticleComment()
            {
                Content = PostContent,
                ForArticle = new Article() { ObjectID = ArticleID },
                Creator = new User() { ObjectID = UserID }
            };
        }
    }
}