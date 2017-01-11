using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTCareerAsk.DAL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.BL
{
    public static class UpperMessageService
    {
        //用于进行对站内消息的操作

        public static LeanCloudAccess LCDal { get { return new LeanCloudAccess(); } }

        public static async Task<int> CountNewMessages(string userId)
        {
            return await LCDal.CountNewMessageForUser(userId);
        }

        public static async Task<bool> MarkMessageAsOpened(string userId, string messageId)
        {
            return await LCDal.MarkMessageAsRead(userId,messageId);
        }

        public static async Task<bool> DeleteMessage(string userId, string messageId)
        {
            return await LCDal.DeleteMessage(userId, messageId);
        }

        public static async Task<bool> SendSystemMessageAsAdmin()
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> SendSystemMessageAsSelf()
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> SendPersonalMessageAsAdmin()
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> SendPersonalMessageAsSelf()
        {
            throw new NotImplementedException();
        }
    }
}
