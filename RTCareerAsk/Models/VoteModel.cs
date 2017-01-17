using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;
using RTCareerAsk.App_DLL;

namespace RTCareerAsk.Models
{
    public class VoteModel
    {
        public string TargetID { get; set; }

        public string QuestionTitle { get; set; }

        public int Type { get; set; }

        public string VoterID { get; set; }

        public string VoterName { get; set; }

        public string NotifyUserID { get; set; }

        public bool IsLike { get; set; }

        public bool IsUpdate { get; set; }

        private HistoryModel GenerateNotification()
        {
            return new HistoryModel()
            {
                User = new UserModel() { UserID = VoterID },
                Target = new UserModel() { UserID = NotifyUserID },
                Type = (VoteType)Type == VoteType.Question ? HistoryType.LikedQstn : HistoryType.LikedAns,
                NameStrings = new string[] { QuestionTitle },
                InfoStrings = new string[] { TargetID }
            };
        }

        public Vote CreateVote()
        {
            return new Vote()
            {
                TargetID = TargetID,
                Type = (VoteType)Type,
                VoterID = VoterID,
                IsLike = IsLike,
                IsUpdate = IsUpdate,
                Notification = IsLike ? GenerateNotification().CreateHistoryForSave() : null
            };
        }
    }
}