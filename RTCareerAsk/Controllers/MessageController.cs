﻿using System;
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
                    await UpdateNewMessageCount();

                    ViewBag.Title = GenerateTitle("消息");
                    ViewBag.Notifications = await MessageDa.LoadNotificationsByPage(GetUserID(), new int[] { 0 }, 0);

                    if (!string.IsNullOrEmpty(Id))
                    {
                        if (await UpperMessageService.MarkMessageAsOpened(GetUserID(), Id))
                        {
                            ViewBag.Message = Id;
                        }
                    }

                    return View(await MessageDa.LoadMessagesByUserID(GetUserID()));
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
                    List<HistoryModel> model = await MessageDa.LoadNotificationsByPage(new int[] { 0 }, 0);
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

                List<HistoryModel> model = await MessageDa.LoadNotificationsByPage(GetUserID(), types.ToArray(), pageIndex);

                return PartialView("_NotificationList", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
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

                List<HistoryModel> model = await MessageDa.LoadNotificationsByPage(types.ToArray(), pageIndex);
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

        [HttpGet]
        public async Task<PartialViewResult> LoadMessageContent(string Id)
        {
            try
            {
                if (await UpperMessageService.MarkMessageAsOpened(GetUserID(), Id))
                {
                    MessageModel model = await MessageDa.GetMessageByID(Id);

                    if (model.From != null)
                    {
                        ViewBag.IsFromSelf = GetUserID() == model.From.UserID;
                        ViewBag.IsFromSystem = false;
                    }
                    else
                    {
                        ViewBag.IsFromSystem = true;
                    }

                    return PartialView("_MessageContent", await MessageDa.GetMessageByID(Id));
                }

                throw new InvalidOperationException("标记消息为已读失败");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        [HttpPost]
        public async Task<PartialViewResult> UpdateMsgCount()
        {
            await UpdateNewMessageCount();

            return PartialView("_NavBar");
        }

        [HttpPost]
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
