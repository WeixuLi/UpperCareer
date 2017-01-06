using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    /// <summary>
    /// 用于保存用户登录后返回的信息，并保存于Session中。
    /// </summary>
    public class UserInfoModel
    {
        public UserInfoModel()
        {
            RoleNames = new List<string>();
        }

        public UserInfoModel(User u)
        {
            RoleNames = new List<string>();
            ConvertUserObjectToUserInfoModel(u);
        }

        public string UserID { get; set; }

        public string Name { get; set; }

        public string Portrait { get; set; }

        public bool EmailVerified { get; set; }

        public int NewMessageCount { get; set; }

        public List<string> RoleNames { get; set; }

        private void ConvertUserObjectToUserInfoModel(User u)
        {
            UserID = u.ObjectID;
            Name = u.Name;
            Portrait = u.Portrait;
            EmailVerified = u.EmailVerified;

            if (u.Roles.Count > 0)
            {
                foreach (Role r in u.Roles)
                {
                    RoleNames.Add(r.RoleName);
                }
            }
        }

        public void SetNewMessageCount(int count)
        {
            NewMessageCount = count;
        }
    }

    /// <summary>
    /// 用于生成用户的名片信息。
    /// </summary>
    public class UserTagModel
    {
        public UserTagModel() { }

        public UserTagModel(User u)
        {
            ConvertUserObjectToUserTagModel(u);
        }

        public UserTagModel(UserTag ut)
        {
            ConvertUserObjectToUserTagModel(ut);
        }

        private void ConvertUserObjectToUserTagModel(UserTag ut)
        {
            UserID = ut.ObjectID;
            Name = ut.Name;
            Gender = ut.Gender;
            Portrait = ut.Portrait;
            Title = ut.Title;
            Company = ut.Company;
            HasFollowed = ut.HasFollowed;
            FollowerCount = ut.FollowerCount;
            AnswerCount = ut.AnswerCount;
        }

        public string UserID { get; set; }

        public string Name { get; set; }

        public int Gender { get; set; }

        public string Portrait { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public bool? HasFollowed { get; set; }

        public int FollowerCount { get; set; }

        public int AnswerCount { get; set; }

        private void ConvertUserObjectToUserTagModel(User u)
        {
            UserID = u.ObjectID;
            Name = u.Name;
            Gender = u.Gender;
            Portrait = u.Portrait;
            Title = u.Title;
            Company = u.Company;
        }

        public UserTagModel SetFollowerAndAnswerCount(bool? hasFollowed, int followerCnt, int answerCnt)
        {
            HasFollowed = hasFollowed;
            FollowerCount = followerCnt;
            AnswerCount = answerCnt;

            return this;
        }
    }

    /// <summary>
    /// 用于获取用户信息页面的具体信息。
    /// </summary>
    public class UserDetailModel
    {
        public UserDetailModel()
        {
            RecentQuestions = new List<QuestionInfoModel>();
            RecentAnswers = new List<AnswerModel>();
        }

        public UserDetailModel(UserDetail ud)
        {
            RecentQuestions = new List<QuestionInfoModel>();
            RecentAnswers = new List<AnswerModel>();

            ConvertUserDetailObjectToModel(ud);
        }

        public string UserDetailID { get; set; }

        public string UserID { get; set; }

        public string Name { get; set; }

        public int Gender { get; set; }

        public string Portrait { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public string SelfDescription { get; set; }

        public int FieldIndex { get; set; }

        public DateTime DateCreate { get; set; }

        public bool? HasFollowed { get; set; }

        public int FollowerCount { get; set; }

        public int FolloweeCount { get; set; }

        public List<QuestionInfoModel> RecentQuestions { get; set; }

        public List<AnswerModel> RecentAnswers { get; set; }

        private void ConvertUserDetailObjectToModel(UserDetail ud)
        {
            if (ud != null)
            {
                UserDetailID = ud.ObjectId;
                UserID = ud.ForUser.ObjectID;
                Name = ud.ForUser.Name;
                Gender = ud.ForUser.Gender;
                Portrait = ud.ForUser.Portrait;
                Title = ud.ForUser.Title;
                Company = ud.ForUser.Company;
                SelfDescription = ud.SelfDescription;
                FieldIndex = ud.ForUser.FieldIndex;
                DateCreate = ud.ForUser.DateCreate;
                FollowerCount = ud.FollowerCount;
                FolloweeCount = ud.FolloweeCount;
            }
        }
    }
}