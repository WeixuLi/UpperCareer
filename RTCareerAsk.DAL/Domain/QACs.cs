using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class QuestionInfo : UpperInfoBaseDomain
    {
        public QuestionInfo() { }

        public QuestionInfo(AVObject po)
        {
            GenerateQuestionInfoObject(po);
        }

        private void GenerateQuestionInfoObject(AVObject po)
        {
            if (po.ClassName != "Post")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是问题类object。", po.ObjectId));
            }

            GenerateInfoObject(po);
        }

        public QuestionInfo SetAnswerCount(int ansCnt)
        {
            SubPostCount = ansCnt;

            return this;
        }
    }

    public class AnswerInfo : UpperInfoBaseDomain
    {
        public AnswerInfo() { }

        public AnswerInfo(AVObject ao)
        {
            GenerateAnswerInfoObject(ao);
        }

        public string RecommandationID { get; set; }

        private void GenerateAnswerInfoObject(AVObject ao)
        {
            if (ao.ClassName != "Answer")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是问题类object。对象类型：{1}", ao.ObjectId, ao.ClassName));
            }

            GenerateInfoObject(ao);
            Title = ao.Get<AVObject>("forQuestion") != null ? ao.Get<AVObject>("forQuestion").Get<string>("title") : null;
            RecommandationID = ao.ContainsKey("recommendation") && ao.Get<AVObject>("recommendation") != null ? ao.Get<AVObject>("recommendation").ObjectId : default(string);
        }

        public AnswerInfo SetCommentCount(int cmtCnt)
        {
            SubPostCount = cmtCnt;

            return this;
        }
    }

    public class Question : UpperQACBaseDomain
    {
        public Question()
        {
            Answers = new List<Answer>();
        }

        public Question(AVObject po)
        {
            Answers = new List<Answer>();

            GenerateQuestionObject(po);
        }

        public Question(AVObject po, IEnumerable<Answer> ans)
        {
            if (ans != null)
            {
                Answers = new List<Answer>(ans);
            }

            GenerateQuestionObject(po);
        }

        public string Title { get; set; }

        public int VoteDiff { get; set; }

        public bool? IsLike { get; set; }

        public int VotePositive { get; set; }

        public int VoteNegative { get; set; }

        public List<Answer> Answers { get; set; }

        private void GenerateQuestionObject(AVObject po)
        {
            if (po.ClassName != "Post")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是问题类object。", po.ObjectId));
            }

            GenerateQACObject(po);
            Title = po.ContainsKey("title") ? po.Get<string>("title") : null;
            VoteDiff = po.ContainsKey("voteDiff") ? po.Get<int>("voteDiff") : default(int);
        }

        public Question SetUserVote(bool? isLike)
        {
            IsLike = isLike;

            return this;
        }

        public Question SetVoteCounts(Dictionary<string, int> voteCounts)
        {
            VotePositive = voteCounts["Positive"];
            VoteNegative = voteCounts["Negative"];

            return this;
        }

        public AVObject LoadQuestionObject()
        {
            if (string.IsNullOrEmpty(ObjectID))
            {
                throw new NullReferenceException("没有可用的问题ID来获取信息");
            }

            return AVObject.CreateWithoutData("Post", ObjectID);
        }

        public AVObject CreateQuestionObjectForSave()
        {
            AVObject q = new AVObject("Post");

            q.Add("title", Title);
            q.Add("content", Content);
            q.Add("createdBy", CreatedBy.LoadUserObject());

            return q;
        }
    }

    public class Answer : UpperQACBaseDomain
    {
        public Answer()
        {
            Comments = new List<Comment>();
        }

        public Answer(AVObject ao)
        {
            Comments = new List<Comment>();

            GenerateAnswerObject(ao);
        }

        public int VoteDiff { get; set; }

        public bool? IsLike { get; set; }

        public int VotePositive { get; set; }

        public int VoteNegative { get; set; }

        public Question ForQuestion { get; set; }

        public List<Comment> Comments { get; set; }

        public History Notification { get; set; }

        private void GenerateAnswerObject(AVObject ao)
        {
            if (ao.ClassName != "Answer")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是答案类object。", ao.ObjectId));
            }

            GenerateQACObject(ao);
            ForQuestion = ao.Get<AVObject>("forQuestion") != null ? new Question(ao.Get<AVObject>("forQuestion")) : null;
            VoteDiff = ao.ContainsKey("voteDiff") ? ao.Get<int>("voteDiff") : default(int);
        }

        public Answer SetComments(IEnumerable<AVObject> cmts)
        {
            if (cmts.Count() > 0)
            {
                if (cmts.First().ClassName != "Comment")
                {
                    throw new InvalidOperationException("获取的对象不是评论类object。");
                }

                foreach (AVObject cmt in cmts)
                {
                    Comments.Add(new Comment(cmt));
                }
            }

            return this;
        }

        public Answer SetUserVote(bool? isLike)
        {
            IsLike = isLike;

            return this;
        }

        public Answer SetVoteCounts(Dictionary<string, int> voteCounts)
        {
            VotePositive = voteCounts["Positive"];
            VoteNegative = voteCounts["Negative"];

            return this;
        }

        public AVObject LoadAnswerObject()
        {
            if (string.IsNullOrEmpty(ObjectID))
            {
                throw new NullReferenceException("没有可用的答案ID来获取信息");
            }

            return AVObject.CreateWithoutData("Answer", ObjectID);
        }

        public AVObject CreateAnswerObjectForSave()
        {
            AVObject ans = new AVObject("Answer");

            ans.Add("content", Content);
            ans.Add("createdBy", CreatedBy.LoadUserObject());
            ans.Add("forQuestion", ForQuestion.LoadQuestionObject());

            return ans;
        }
    }

    public class Comment : UpperQACBaseDomain
    {
        public Comment() { }

        public Comment(AVObject co)
        {
            GenerateCommentObject(co);
        }

        public Answer ForAnswer { get; set; }

        public History Notification { get; set; }

        private void GenerateCommentObject(AVObject co)
        {
            if (co.ClassName != "Comment")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是评论类object。", co.ObjectId));
            }

            GenerateQACObject(co);
        }

        public AVObject CreateCommentObjectForSave()
        {
            AVObject cmt = new AVObject("Comment");

            cmt.Add("content", Content);
            cmt.Add("createdBy", CreatedBy.LoadUserObject());
            cmt.Add("forAnswer", ForAnswer.LoadAnswerObject());

            return cmt;
        }
    }
}
