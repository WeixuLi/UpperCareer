using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RTCareerAsk.Models;
using RTCareerAsk.Filters;

namespace RTCareerAsk.Controllers
{
    public class ArticleController : UpperBaseController
    {
        [UpperResult]
        public async Task<ActionResult> Index()
        {
            try
            {
                await Task.WhenAll(AutoLogin(), UpdateNewMessageCount());

                ViewBag.Title = GeneralTitle;

                return View(await ArticleDa.LoadArticleList());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> Detail(string id)
        {
            try
            {
                await Task.WhenAll(AutoLogin(), UpdateNewMessageCount());

                ArticleModel model = await ArticleDa.LoadArticleDetail(id);
                model.Comments = SetFlagsForActions(model.Comments);
                ViewBag.Title = GenerateTitle(model.Title);

                return View(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> Compose(string id = null)
        {
            try
            {
                if (!IsUserAuthorized("Admin"))
                {
                    return RedirectToAction("Login", "Account");
                }

                ArticlePostModel model = await ArticleDa.CreatePostModelWithReference(id);
                ViewBag.Title = "撰写新文章";

                return View(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperResult]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> PostComment(ArticleCommentPostModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                model.UserID = GetUserID();
                model.PostContent = ModifyTextareaData(model.PostContent, true);

                return PartialView("_ArticleCommentList", SetFlagsForActions(new List<ArticleCommentModel>() { await ArticleDa.PostNewArticleComment(model) }));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperResult]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> DeleteComment(string acmtId, string atclId, int replaceIndex)
        {
            try
            {
                ArticleCommentModel result = await ArticleDa.DeleteArticleComment(acmtId, atclId, replaceIndex);
                List<ArticleCommentModel> model = new List<ArticleCommentModel>();

                if (result != null)
                {
                    model.Add(result);
                }

                return PartialView("_ArticleCommentList", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadArticlesByPage(int pageIndex)
        {
            try
            {
                return PartialView("_ArticleList", await ArticleDa.LoadArticleList(pageIndex));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadArticleCommentsByPage(string targetId, int pageIndex)
        {
            try
            {
                List<ArticleCommentModel> results = string.IsNullOrEmpty(targetId) ? new List<ArticleCommentModel>() : await ArticleDa.LoadArticleCommentList(targetId, pageIndex);

                return PartialView("_ArticleCommentList", SetFlagsForActions(results));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [UpperJsonExceptionFilter]
        public async Task<ActionResult> Compose(ArticlePostModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new ArgumentException("您输入的内容不符合格式");
                }
                else if (!HasUserInfo)
                {
                    throw new InvalidOperationException("请您先登录进行操作");
                }

                if (model.HasReference)
                {
                    model.Content = ModifyTextareaData(model.Content, true);
                }

                model.EditorID = GetUserID();

                await ArticleDa.PostNewArticle(model);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<JsonResult> UploadCoverPic(HttpPostedFileBase cover)
        {
            try
            {
                string url = await UploadImageFile(cover, string.Format("ArticleCover{0}", GetUserID()));

                return Json(new
                {
                    url = url
                });
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        public List<ArticleCommentModel> SetFlagsForActions(List<ArticleCommentModel> models)
        {
            if (!HasUserInfo)
            {
                return models;
            }

            foreach (ArticleCommentModel acmt in models)
            {
                acmt.IsDeleteAllowed = acmt.Creator.UserID == GetUserID();
            }

            return models;
        }
    }
}
