using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RTCareerAsk.Models;

namespace RTCareerAsk.Filters
{
    public class UpperResultAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            HttpContextBase httpContext = filterContext.HttpContext;

            UserInfoModel userInfo = httpContext.Session["UserInfo"] != null ? httpContext.Session["UserInfo"] as UserInfoModel : null;

            filterContext.Controller.ViewBag.IsAuthorized = IsUserAuthorized(userInfo, "User,Admin");
            filterContext.Controller.ViewBag.IsAdmin = IsUserAuthorized(userInfo, "Admin");
        }

        protected bool IsUserAuthorized(UserInfoModel user, string roles)
        {
            if (user != null)
            {
                string[] rolesSplit = SplitString(roles);

                if (user.RoleNames.Contains("Block"))
                {
                    return false;
                }

                if (rolesSplit.Length > 0 && rolesSplit.Intersect(user.RoleNames).Count() == 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }

    public class UpperJsonExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        // obviously here you could include whatever information you want about the exception
                        // for example if you have some custom exceptions you could test
                        // the type of the actual exception and extract additional data
                        // For the sake of simplicity let's suppose that we want to
                        // send only the exception message to the client
                        errorMessage = filterContext.Exception.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}
