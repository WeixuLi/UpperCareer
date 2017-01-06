using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.PLtoDA;
using RTCareerAsk.Filters;
using RTCareerAsk.App_DLL;

namespace RTCareerAsk.Controllers
{
    public class QuestionController : UpperBaseController
    {
        [UpperResult]
        public async Task<ActionResult> Index()
        {
            try
            {
                await Task.WhenAll(AutoLogin(), UpdateNewMessageCount());

                ViewBag.Title = GeneralTitle;

                return View(await QuestionDa.LoadAnswerListByPage(0, 4));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> QuestionDetail(string id)
        {
            try
            {
                await Task.WhenAll(AutoLogin(), UpdateNewMessageCount());

                QuestionModel model = await QuestionDa.GetQuestionModel(HasUserInfo ? GetUserID() : string.Empty, id);
                ViewBag.Title = GenerateTitle(model.Title);

                return View(SetFlagsForActions(model));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> AnswerDetail(string id)
        {
            try
            {
                await Task.WhenAll(AutoLogin(), UpdateNewMessageCount());

                AnswerModel model = await QuestionDa.GetAnswerModel(HasUserInfo ? GetUserID() : string.Empty, id);
                ViewBag.Title = GenerateTitle(string.Format("{0} - {1}的回答", model.ForQuestion.Title, model.Creator.Name));

                return View(SetFlagsForActions(model));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadContentInfo(int id)
        {
            try
            {
                switch (id)
                {
                    case 1:
                    case 2:
                        return PartialView("_QuestionList", await QuestionDa.LoadQuestionListByPage(0, id));
                    case 3:
                    case 4:
                        return PartialView("_AnswerList", await QuestionDa.LoadAnswerListByPage(0, id));
                    default:
                        throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", id));
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadContentUpdate(int contentType, int pageIndex)
        {
            try
            {
                switch (contentType)
                {
                    case 1:
                    case 2:
                        return PartialView("_QuestionList", await QuestionDa.LoadQuestionListByPage(pageIndex, contentType));
                    case 3:
                    case 4:
                        return PartialView("_AnswerList", await QuestionDa.LoadAnswerListByPage(pageIndex, contentType));
                    default:
                        throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", contentType));
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult CreateQuestionForm()
        {
            try
            {
                return PartialView("_PostModal", new QuestionPostModel());
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
        public async Task<PartialViewResult> PostQuestion(QuestionPostModel p)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                p.UserID = GetUserID();

                if (await QuestionDa.PostNewQuestion(p))
                {
                    return PartialView("_QuestionList", await QuestionDa.LoadNewQuestions());
                }

                throw new InvalidOperationException("保存问题失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        [HttpPost]
        [ValidateInput(false)]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> PostAnswer(AnswerPostModel a)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                a.UserID = GetUserID();
                a.UserName = GetUserName();

                if (await QuestionDa.PostNewAnswer(a))
                {
                    return PartialView("_AnswersDetail", SetFlagsForActions(await QuestionDa.GetAnswerModels(GetUserID(), a.QuestionID, 0, true)));
                }

                throw new InvalidOperationException("保存答案失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> PostComment(CommentPostModel c)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                c.UserID = GetUserID();
                c.UserName = GetUserName();
                c.PostContent = ModifyTextareaData(c.PostContent, true);

                if (await QuestionDa.PostNewComment(c))
                {
                    List<CommentModel> model = await QuestionDa.GetCommentModels(c.AnswerID);

                    return PartialView("_CommentList", SetFlagsForActions(model));
                }

                throw new InvalidOperationException("保存评论失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task UpdateContent(bool isQuestion, string id, string content)
        {
            try
            {
                await QuestionDa.UpdateContent(isQuestion, id, content);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task Delete(string id, QACType type)
        {
            try
            {
                switch (type)
                {
                    case QACType.Answer:
                        await QuestionDa.DeleteAnswerWithComments(id);
                        break;
                    case QACType.Comment:
                        await QuestionDa.DeleteComment(id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("请求类型超出范围。收到的请求：{0}", type));
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task SaveVote(VoteModel model)
        {
            try
            {
                model.VoterID = GetUserID();
                model.VoterName = GetUserName();

                await QuestionDa.SaveOrUpdateVote(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadAnswersDetails(string targetId, int pageIndex, int contentType)
        {
            try
            {
                string userId = HasUserInfo ? GetUserID() : string.Empty;

                List<AnswerModel> results = await QuestionDa.GetAnswerModels(userId, targetId, pageIndex, contentType == 1 ? true : false);

                return PartialView("_AnswersDetail", SetFlagsForActions(results));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        #region Helpers

        private QuestionModel SetFlagsForActions(QuestionModel model)
        {
            if (HasUserInfo)
            {
                bool createdByUser = model.Creator.UserID == GetUserID();
                IEnumerable<AnswerModel> answerByUser = model.Answers.Where(x => x.Creator.UserID == GetUserID());

                model.IsEditAllowed = createdByUser;
                model.IsAnswerAllowed = !createdByUser && answerByUser.Count() == 0;

                if (answerByUser.Count() > 0)
                {
                    foreach (AnswerModel ans in answerByUser)
                    {
                        model.Answers.Where(x => x.ID == ans.ID).First().IsEditAllowed = true;
                    }
                }

                foreach (AnswerModel ans in model.Answers)
                {
                    if (ans.Comments.Count > 0)
                    {
                        ans.Comments = SetFlagsForActions(ans.Comments);
                    }
                }
            }

            return model;
        }

        private AnswerModel SetFlagsForActions(AnswerModel model)
        {
            if (!HasUserInfo)
            {
                return model;
            }

            model.IsEditAllowed = model.Creator.UserID == GetUserID();

            if (model.Comments.Count > 0)
            {
                model.Comments = SetFlagsForActions(model.Comments);
            }

            return model;
        }

        private List<AnswerModel> SetFlagsForActions(List<AnswerModel> models)
        {
            if (HasUserInfo)
            {
                foreach (AnswerModel ans in models)
                {
                    ans.IsEditAllowed = ans.Creator.UserID == GetUserID();

                    if (ans.Comments.Count > 0)
                    {
                        ans.Comments = SetFlagsForActions(ans.Comments);
                    }
                }
            }

            return models;
        }

        private List<CommentModel> SetFlagsForActions(List<CommentModel> models)
        {
            foreach (CommentModel cmt in models)
            {
                cmt.IsDeleteAllowed = cmt.Creator.UserID == GetUserID();
            }

            return models;
        }

        #endregion

        #region Trunk

        //[HttpPost]
        //public async Task DeleteAnswer(string ansId)
        //{
        //    await QuestionDa.DeleteAnswerWithComments(ansId);
        //}

        //[HttpPost]
        //public async Task DeleteComment(string cmtId)
        //{
        //    await QuestionDa.DeleteComment(cmtId);
        //}

        //[HttpPost]
        //[JsonExceptionFilter]
        //public PartialViewResult CreateCommentForm(string ansId, string ntfyId, string prefixText = "")
        //{
        //    try
        //    {
        //        return PartialView("_CommentForm", new CommentPostModel() { AnswerID = ansId, NotifyUserID = ntfyId, PostContent = prefixText });
        //    }
        //    catch (Exception e)
        //    {
        //        while (e.InnerException != null) e = e.InnerException;
        //        throw e;
        //    }
        //}

        #endregion
    }
}
