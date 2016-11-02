﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.IO;
using RTCareerAsk.Models;
using RTCareerAsk.PLtoDA;
using RTCareerAsk.Filters;

namespace RTCareerAsk.Controllers
{
    public class UserController : UpperBaseController
    {
        [UpperResult]
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                ViewBag.Title = "浏览用户信息";
                //ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
                //ViewBag.IsAdmin = IsUserAuthorized("Admin");
                ViewBag.IsSelf = HasUserInfo ? GetUserID() == id : false;

                UserDetailModel model = await UserDa.LoadUserDetail(id, HasUserInfo ? GetUserID() : "");

                return View(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        public async Task FollowUser(string id)
        {
            try
            {
                if (IsUserAuthorized("User,Admin"))
                {
                    await UserDa.Follow(GetUserID(), id);
                }
                else
                {
                    throw new OperationCanceledException("您还未登录，不能关注");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        public async Task UnfollowUser(string id)
        {
            try
            {
                if (IsUserAuthorized("User,Admin"))
                {
                    await UserDa.Unfollow(GetUserID(), id);
                }
                else
                {
                    throw new OperationCanceledException("您还未登录，不能关注");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        public async Task<PartialViewResult> RecentRecord(int id, string targetId)
        {
            if (id == 1)
            {
                return PartialView("_RecentQuestions", await UserDa.GetRecentQuestions(targetId));
            }
            else if (id == 2)
            {
                return PartialView("_RecentAnswers", await UserDa.GetRecentAnswers(targetId));
            }
            else
            {
                throw new ArgumentOutOfRangeException("所提供的操作代码不符合要求。");
            }
        }
    }
}
