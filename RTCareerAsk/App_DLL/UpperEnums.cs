using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTCareerAsk.App_DLL
{
    public enum QACType
    {
        Question,
        Answer,
        Comment
    }

    public enum NotificationType
    {
        LikedQstn = 1,
        LikedAns,
        CommentAns,
        RepliedCmt,
        Answered,
        Published,
        Followed
    }

    public enum SearchModelType
    {
        All,
        Question,
        User
    }

    public enum PortraitSize
    {
        Small,
        Medium,
        Large
    }
}