using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class QuestionInfoModel : UpperInfoBaseModel
    {
        public QuestionInfoModel() { }

        public QuestionInfoModel(QuestionInfo qio)
        {
            ConvertQuestionInfoObjectToQuestionInfoModel(qio);
        }

        private void ConvertQuestionInfoObjectToQuestionInfoModel(QuestionInfo qio)
        {
            ConvertInfoObjectToModel(qio);
        }
    }

    public class AnswerInfoModel : UpperInfoBaseModel
    {
        public AnswerInfoModel() { }

        public AnswerInfoModel(AnswerInfo aio)
        {
            ConvertAnswerInfoObjectToAnswerInfoModel(aio);
        }

        public string RecommandationID { get; set; }

        private void ConvertAnswerInfoObjectToAnswerInfoModel(AnswerInfo aio)
        {
            ConvertInfoObjectToModel(aio);
            
            RecommandationID = aio.RecommandationID;
        }
    }

    public class QuestionModel : UpperQACBaseModel
    {
        public QuestionModel()
        {
            Answers = new List<AnswerModel>();
        }

        public QuestionModel(Question po)
        {
            Answers = new List<AnswerModel>();

            ConvertQuestionObjectToModel(po);
        }

        public string Title { get; set; }

        public int VoteDiff { get; set; }

        public bool? IsLike { get; set; }

        public string VotePositive { get; set; }

        public string VoteNegative { get; set; }

        public bool IsEditAllowed { get; set; }

        public bool IsAnswerAllowed { get; set; }

        public List<AnswerModel> Answers { get; set; }

        private void ConvertQuestionObjectToModel(Question po)
        {
            ConvertQACObjectToModel(po);
            Title = po.Title;
            VoteDiff = po.VoteDiff;
            IsLike = po.IsLike;
            VotePositive = ProcessLargeNumDisplay(po.VotePositive);
            VoteNegative = ProcessLargeNumDisplay(po.VoteNegative);

            if (po.Answers != null)
            {
                foreach (Answer a in po.Answers)
                {
                    Answers.Add(new AnswerModel(a));
                }
            }
        }
    }

    public class AnswerModel : UpperQACBaseModel
    {
        public AnswerModel()
        {
            Comments = new List<CommentModel>();
        }

        public AnswerModel(Answer ao)
        {
            Comments = new List<CommentModel>();

            ConvertAnswerObjectToModel(ao);
        }

        public int VoteDiff { get; set; }

        public bool? IsLike { get; set; }

        public string VotePositive { get; set; }

        public string VoteNegative { get; set; }

        public QuestionModel ForQuestion { get; set; }

        public List<CommentModel> Comments { get; set; }

        public bool IsEditAllowed { get; set; }

        private void ConvertAnswerObjectToModel(Answer ao)
        {
            ConvertQACObjectToModel(ao);
            ForQuestion = ao.ForQuestion != null ? new QuestionModel(ao.ForQuestion) : null;
            VoteDiff = ao.VoteDiff;
            IsLike = ao.IsLike;
            VotePositive = ProcessLargeNumDisplay(ao.VotePositive);
            VoteNegative = ProcessLargeNumDisplay(ao.VoteNegative);

            foreach (Comment c in ao.Comments)
            {
                Comments.Add(new CommentModel(c));
            }
        }
    }

    public class CommentModel : UpperQACBaseModel
    {
        public CommentModel() { }

        public CommentModel(Comment co)
        {
            ConvertCommentObjectToModel(co);
        }

        public bool IsDeleteAllowed { get; set; }

        private void ConvertCommentObjectToModel(Comment co)
        {
            ConvertQACObjectToModel(co);
        }
    }
}