using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class User
    {
        public User()
        {
            Roles = new List<Role>();
        }

        public User(AVUser u)
        {
            Roles = new List<Role>();

            GenerateUserObject(u);
        }

        public string ObjectID { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public int Gender { get; set; }

        public string Portrait { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public int FieldIndex { get; set; }

        public bool EmailVerified { get; set; }

        public bool MobileVerified { get; set; }

        public DateTime DateCreate { get; set; }

        public List<Role> Roles { get; set; }

        private void GenerateUserObject(AVUser u)
        {
            if (u != null)
            {
                ObjectID = u.ObjectId;
                Name = u.ContainsKey("nickname") ? u.Get<string>("nickname") : null;
                Title = u.ContainsKey("title") ? u.Get<string>("title") : null;
                Gender = u.ContainsKey("gender") ? u.Get<int>("gender") : 0;
                Portrait = u.ContainsKey("portrait") ? u.Get<string>("portrait") : null;
                Email = u.Email;
                Company = u.ContainsKey("company") ? u.Get<string>("company") : null;
                FieldIndex = u.ContainsKey("fieldIndex") ? u.Get<int>("fieldIndex") : 0;
                EmailVerified = u.ContainsKey("emailVerified") ? u.Get<bool>("emailVerified") : false;
                MobileVerified = u.MobilePhoneVerified;
                DateCreate = Convert.ToDateTime(u.CreatedAt);
            }
        }

        public User SetRoles(IEnumerable<AVRole> ros)
        {
            if (ros.Count() > 0)
            {
                foreach (AVRole ro in ros)
                {
                    Roles.Add(new Role(ro));
                }
            }

            return this;
        }

        public AVUser LoadUserObject()
        {
            if (string.IsNullOrEmpty(ObjectID))
            {
                throw new NullReferenceException("没有可用的用户ID来获取信息");
            }

            return AVUser.CreateWithoutData("_User", ObjectID) as AVUser;
        }

        public AVUser CreateUserObjectForRegister()
        {
            AVUser user = new AVUser()
            {
                Username = Email,
                Password = Password,
                Email = Email,
            };

            user.Add("nickname", Name);

            return user;
        }
    }

    public class Role
    {
        public Role() { }

        public Role(AVRole ro)
        {
            GenerateRoleObject(ro);
        }

        public string ObjectID { get; set; }

        public string RoleName { get; set; }

        private void GenerateRoleObject(AVRole ro)
        {
            ObjectID = ro.ObjectId;
            RoleName = ro.Get<string>("name");
        }
    }

    public class UserTag
    {
        public UserTag() { }

        public UserTag(AVUser u)
        {
            GenerateUserTagObject(u);
        }

        public string ObjectID { get; set; }

        public string Name { get; set; }

        public int Gender { get; set; }

        public string Portrait { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public bool? HasFollowed { get; set; }

        public int FollowerCount { get; set; }

        public int AnswerCount { get; set; }

        private void GenerateUserTagObject(AVUser u)
        {
            if (u == null)
            {
                throw new ArgumentNullException("未能获取用户信息");
            }

            ObjectID = u.ObjectId;
            Name = u.ContainsKey("nickname") ? u.Get<string>("nickname") : null;
            Gender = u.ContainsKey("gender") ? u.Get<int>("gender") : 0;
            Portrait = u.ContainsKey("portrait") ? u.Get<string>("portrait") : null;
            Title = u.ContainsKey("title") ? u.Get<string>("title") : null;
            Company = u.ContainsKey("company") ? u.Get<string>("company") : null;

        }

        public UserTag SetFollowerAndAnswerCount(bool? hasFollowed, int followerCnt, int answerCnt)
        {
            HasFollowed = hasFollowed;
            FollowerCount = followerCnt;
            AnswerCount = answerCnt;

            return this;
        }
    }

    public class UserDetail
    {
        public UserDetail() { }

        public UserDetail(AVUser u)
        {
            CreateNewUserDetailObject(u);
        }

        public UserDetail(AVObject udo)
        {
            GenerateUserDetailObject(udo);
        }

        public string ObjectId { get; set; }

        public User ForUser { get; set; }

        public string SelfDescription { get; set; }

        public int FollowerCount { get; set; }

        public int FolloweeCount { get; set; }

        private void CreateNewUserDetailObject(AVUser u)
        {
            ForUser = new User(u);
        }

        private void GenerateUserDetailObject(AVObject ud)
        {
            if (ud.ClassName != "UserDetail")
            {
                throw new InvalidOperationException("获取的对象不是用户信息类object。");
            }

            ObjectId = ud.ObjectId;
            ForUser = new User(ud.Get<AVUser>("forUser"));
            SelfDescription = ud.Get<string>("selfDescription");
        }

        public UserDetail SetFollowCounts(int follower, int followee)
        {
            FollowerCount = follower;
            FolloweeCount = followee;

            return this;
        }

        public AVObject CreateUserDetailObjectForSave()
        {
            AVObject userDetail = new AVObject("UserDetail");

            userDetail.Add("forUser", ForUser.LoadUserObject());
            userDetail.Add("selfDescription", SelfDescription);

            return userDetail;
        }

        public AVObject UpdateUserDetailObject(AVObject udo)
        {
            if (udo.ClassName != "UserDetail")
            {
                throw new InvalidOperationException("获取的对象不是用户信息类object。");
            }

            udo["selfDescription"] = SelfDescription;

            return udo;
        }
    }
}
