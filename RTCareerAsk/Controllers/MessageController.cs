using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using RTCareerAsk.BL;
using RTCareerAsk.Models;
using RTCareerAsk.Filters;

namespace RTCareerAsk.Controllers
{
    public class MessageController : UpperBaseController
    {
        public string SessionCopyName { get { return "MessageCopy"; } }

        [UpperResult]
        public async Task<ActionResult> Index(string Id = "")
        {
            try
            {
                await AutoLogin();

                if (IsUserAuthorized("User,Admin"))
                {
                    ViewBag.Title = GenerateTitle("消息");

                    Task tUpdateCount = UpdateNewMessageCount();
                    Task<List<NotificationModel>> tNtfnModel = MessageDa.LoadNotificationsByPage(GetUserID(), new int[] { 0 }, 0);
                    Task<List<MessageModel>> tMsgModel = MessageDa.LoadMessagesByUserID(GetUserID(), 0);

                    await Task.WhenAll(tUpdateCount, tNtfnModel, tMsgModel);

                    ViewBag.Notifications = tNtfnModel.Result;
                    ViewBag.UserId = GetUserID();

                    if (!string.IsNullOrEmpty(Id))
                    {
                        if (await UpperMessageService.MarkMessageAsOpened(GetUserID(), Id))
                        {
                            ViewBag.Message = Id;
                        }
                    }

                    return View(tMsgModel.Result);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> Notifications()
        {
            try
            {
                if (IsUserAuthorized("Admin"))
                {
                    List<NotificationModel> model = await MessageDa.LoadNotificationsByPage(new int[] { 0 }, 0);
                    ViewBag.IsForAdmin = true;

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
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
        public async Task<PartialViewResult> LoadNotificationsByType(int contentType, int pageIndex = 0)
        {
            try
            {
                List<int> types = new List<int>();

                switch (contentType)
                {
                    case 0:
                        types.Add(0);
                        break;
                    case 1:
                        types.AddRange(new int[] { 1, 2, 6 });
                        break;
                    case 2:
                        types.AddRange(new int[] { 3, 4, 5 });
                        break;
                    case 3:
                        types.Add(7);
                        break;
                    default:
                        break;
                }

                List<NotificationModel> model = await MessageDa.LoadNotificationsByPage(GetUserID(), types.ToArray(), pageIndex);

                return PartialView("_NotificationList", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadAllNotificationsByType(int contentType, int pageIndex = 0)
        {
            try
            {
                List<int> types = new List<int>();

                switch (contentType)
                {
                    case 0:
                        types.Add(0);
                        break;
                    case 1:
                        types.AddRange(new int[] { 1, 2, 6 });
                        break;
                    case 2:
                        types.AddRange(new int[] { 3, 4, 5 });
                        break;
                    case 3:
                        types.Add(7);
                        break;
                    default:
                        break;
                }

                List<NotificationModel> model = await MessageDa.LoadNotificationsByPage(types.ToArray(), pageIndex);
                ViewBag.IsForAdmin = true;

                return PartialView("_NotificationList", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadMessagesByPage(int pageIndex)
        {
            if (!HasUserInfo)
            {
                throw new ArgumentNullException("您还没有登录");
            }

            List<MessageModel> results = await MessageDa.LoadMessagesByUserID(GetUserID(), pageIndex);
            ViewBag.UserId = GetUserID();

            return PartialView("_MessagePartial", results);
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task MarkNotificationAsRead(string id)
        {
            try
            {
                await MessageDa.MarkNotificationAsRead(id);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task MarkMessageAsRead(string id)
        {
            try
            {
                await UpperMessageService.MarkMessageAsOpened(GetUserID(), id);
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
        public async Task<PartialViewResult> UpdateMsgCount()
        {
            await UpdateNewMessageCount();

            return PartialView("_NavBar");
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult CreateLetterForm(string targetId)
        {
            try
            {
                LetterModel model;

                if (HasSessionCopy(SessionCopyName))
                {
                    model = RestoreCopy<LetterModel>(SessionCopyName);
                    model.To = targetId;
                    model.Content = ModifyTextareaData(model.Content, false);
                }
                else
                {
                    model = new LetterModel() { To = targetId };
                }

                return PartialView("_LetterModal", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task WritePersonalLetter(LetterModel l)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    l.From = GetUserID();
                    l.Content = ModifyTextareaData(l.Content, true);
                    CopyToSave(SessionCopyName, l);

                    await MessageDa.WriteNewMessage(l);
                    ClearCopy(SessionCopyName);
                }
                else
                {
                    throw new InvalidOperationException("邮件格式不符合要求");
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
        public async Task DeleteMessage(string id)
        {
            try
            {
                await UpperMessageService.DeleteMessage(GetUserID(), id);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }
    }
}
