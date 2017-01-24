using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.DAL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PLtoDA
{
    /// <summary>
    /// 此目录下所有方法都可以直接读取数据库，不需经过逻辑层。所有方法仅为了模型转换用。
    /// 
    /// 此页方法仅限于与Message Controller的沟通。
    /// </summary>
    public class Message2DA : DABase
    {
        public async Task<List<NotificationModel>> LoadNotificationsByPage(string userId, int[] types, int pageIndex = 0)
        {
            return await LCDal.LoadNotifications(userId, types, pageIndex).ContinueWith(t =>
                {
                    return t.Result != null && t.Result.Count() > 0 ? t.Result.Select(x => new NotificationModel(x)).ToList() : new List<NotificationModel>();
                });
        }

        public async Task<List<NotificationModel>> LoadNotificationsByPage(int[] types, int pageIndex = 0)
        {
            return await LCDal.LoadNotifications(types, pageIndex).ContinueWith(t =>
            {
                return t.Result != null && t.Result.Count() > 0 ? t.Result.Select(x => new NotificationModel(x)).ToList() : new List<NotificationModel>();
            });
        }

        public async Task<bool> MarkNotificationAsRead(string id)
        {
            return await LCDal.MarkNotificationAsRead(id);
        }

        public async Task<List<MessageModel>> LoadMessagesByUserID(string userId, int pageIndex)
        {
            return await LCDal.LoadMessagesForUser(userId, pageIndex).ContinueWith(t =>
                {
                    return t.Result != null && t.Result.Count() > 0 ? t.Result.Select(x => new MessageModel(x)).ToList() : new List<MessageModel>();
                });
        }

        public async Task WriteNewMessage(LetterModel l)
        {
            await LCDal.WriteNewMessage(l.CreateMessageForSave());
        }
    }
}