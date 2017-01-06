using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RTCareerAsk.DAL;
using RTCareerAsk.Models;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PLtoDA
{
    /// <summary>
    /// 此类为所有控制器 ==> 数据层类的基类，所有方法都不应该为异步运算，只做数据转换用。
    /// </summary>
    abstract public class DABase
    {
        protected LeanCloudAccess LCDal { get { return new LeanCloudAccess(); } }

        #region Object to Model

        protected List<QuestionInfoModel> ConvertQuestionObjectsToQuestionInfoModels(IEnumerable<QuestionInfo> ps)
        {
            if (ps != null && ps.Count() > 0)
            {
                return ps.Select(x => new QuestionInfoModel(x)).ToList();
            }

            return null;
        }

        protected List<FileInfoModel> ConvertFileInfoObjectsToModels(IEnumerable<DAL.Domain.FileInfo> fis)
        {
            if (fis != null && fis.Count() > 0)
            {
                return fis.Select(x => new FileInfoModel(x)).ToList();
            }

            return null;
        }

        #endregion

        #region Public Method

        public async Task<int> LoadMessageCount(string userId)
        {
            return await LCDal.CountNewMessageForUser(userId);
        }

        public async Task<bool> DeleteFileWithUrl(string url)
        {
            return await LCDal.DeleteFileWithUrl(url);
        }

        public async Task<string> UploadImageFile(FileModel f)
        {
            return await LCDal.SaveNewStreamFile(f.RestoreFileModelToObject());
        }

        #endregion
    }
}