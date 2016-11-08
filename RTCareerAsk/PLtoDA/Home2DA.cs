﻿using System;
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
    /// 此页方法仅限于与Home Controller的沟通。
    /// </summary>
    public class Home2DA : DABase
    {
        public async Task<string> UploadImageFile(FileModel f)
        {
            return await LCDal.SaveNewStreamFile(f.RestoreFileModelToObject());
        }

        public async Task<List<FileInfoModel>> GetFileInfoModels()
        {
            return await LCDal.FindAllFiles().ContinueWith(t => ConvertFileInfoObjectsToModels(t.Result));
        }

        public async Task<FileModel> DownloadImageFiles(string fileId)
        {
            return await LCDal.DownloadFileByID(fileId).ContinueWith(t => new FileModel(t.Result));
        }

        #region Trunk

        //public async Task<List<QuestionInfoModel>> GetQuestionInfoModels(int id = 1)
        //{
        //    return await LCDal.FindQuestionList().ContinueWith(t =>
        //    {
        //        IEnumerable<QuestionInfo> qis = new List<QuestionInfo>();

        //        switch (id)
        //        {
        //            case 1:
        //                qis = t.Result;
        //                break;
        //            case 2:
        //                qis = t.Result.OrderByDescending(x => x.DateCreate);
        //                break;
        //            default:
        //                throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", id));
        //        }

        //        List<QuestionInfoModel> qiList = new List<QuestionInfoModel>();

        //        foreach (QuestionInfo q in qis)
        //        {
        //            qiList.Add(new QuestionInfoModel(q));
        //        }

        //        return qiList;
        //    });
        //}

        //public async Task<List<AnswerInfoModel>> GetAnswerInfoModels(int id = 3)
        //{
        //    return await LCDal.FindAnswerList().ContinueWith(t =>
        //        {
        //            IEnumerable<AnswerInfo> ais = new List<AnswerInfo>();

        //            switch (id)
        //            {
        //                case 3:
        //                    ais = t.Result;
        //                    break;
        //                case 4:
        //                    ais = t.Result.OrderByDescending(x => x.DateCreate);
        //                    break;
        //                default:
        //                    throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", id));
        //            }

        //            List<AnswerInfoModel> aiList = new List<AnswerInfoModel>();

        //            foreach (AnswerInfo a in ais)
        //            {
        //                aiList.Add(new AnswerInfoModel(a));
        //            }

        //            return aiList;
        //        });
        //}

        //public async Task<List<QuestionInfoModel>> LoadQuestionListByPage(int pageIndex, int id = 1)
        //{
        //    bool isHottestFirst = true;

        //    switch (id)
        //    {
        //        case 1:
        //            break;
        //        case 2:
        //            isHottestFirst = false;
        //            break;
        //        default:
        //            throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", id));
        //    }

        //    List<QuestionInfoModel> qiList = new List<QuestionInfoModel>();

        //    foreach (QuestionInfo q in await LCDal.LoadQuestionList(pageIndex, isHottestFirst))
        //    {
        //        qiList.Add(new QuestionInfoModel(q));
        //    }

        //    return qiList;
        //}

        //public async Task<List<AnswerInfoModel>> LoadAnswerListByPage(int pageIndex, int id = 3)
        //{
        //    bool isHottestFirst = true;

        //    switch (id)
        //    {
        //        case 3:
        //            break;
        //        case 4:
        //            isHottestFirst = false;
        //            break;
        //        default:
        //            throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", id));
        //    }

        //    List<AnswerInfoModel> aiList = new List<AnswerInfoModel>();

        //    foreach (AnswerInfo a in await LCDal.LoadAnswerList(pageIndex, isHottestFirst))
        //    {
        //        aiList.Add(new AnswerInfoModel(a));
        //    }

        //    return aiList;

        //}

        //public async Task<UserInfoModel> LoadUserInfo(string userId)
        //{
        //    return await LCDal.LoadUserInfo(userId).ContinueWith(t =>
        //        {
        //            return new UserInfoModel(t.Result);
        //        });
        //}

        //public async Task<bool> ChangeUserPortrait(string userId, string portraitUrl)
        //{
        //    try
        //    {
        //        return await LCDal.ChangeUserPortrait(userId, portraitUrl);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        #endregion
    }
}