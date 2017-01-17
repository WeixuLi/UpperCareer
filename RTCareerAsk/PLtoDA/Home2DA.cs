using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RTCareerAsk.App_DLL;
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
        public async Task<List<FileInfoModel>> GetFileInfoModels()
        {
            return await LCDal.FindAllFiles().ContinueWith(t => ConvertFileInfoObjectsToModels(t.Result));
        }

        public async Task<string> LoadAlertInfo(string key)
        {
            return await LCDal.LoadAlertInfo(key);
        }

        public async Task<FileModel> DownloadImageFiles(string fileId)
        {
            return await LCDal.DownloadFileByID(fileId).ContinueWith(t => new FileModel(t.Result));
        }

        public async Task<SearchResultModel> SearchStupid(string userId, string keyword)
        {
            SearchResult result = await LCDal.SearchByKeywordStupid(keyword);

            result.UserResults = await UpdateUserSearchResults(userId, result.UserResults);

            return new SearchResultModel(result);
        }

        public async Task<SearchResultModel> ExtendSearchResult(string userId, string keyword, SearchModelType type, int pageIndex)
        {
            SearchResult result = await LCDal.ExtendedSearchByKeywordStupid(keyword, (SearchType)type, pageIndex);

            result.UserResults = await UpdateUserSearchResults(userId, result.UserResults);

            return new SearchResultModel(result);
        }

        public async Task<List<UserTag>> UpdateUserSearchResults(string userId, IEnumerable<UserTag> userSearchResults)
        {
            if (userSearchResults.Count() > 0)
            {
                List<Task<UserTag>> tasks = new List<Task<UserTag>>();

                tasks.AddRange(userSearchResults.Select(x => LCDal.BuildUserTag(userId, x)));

                await Task.WhenAll(tasks.ToArray());

                return tasks.Select(x => x.Result).OrderByDescending(x => x.AnswerCount).ThenByDescending(x => x.FollowerCount).ToList();
            }

            return new List<UserTag>();
        }

        public async Task<IEnumerable<FeedModel>> LoadFeedsForUser(string userId, int pageIndex)
        {
            IEnumerable<History> feeds = await LCDal.LoadNewFeeds(userId, pageIndex);

            List<Task<FeedModel>> tasks = feeds.Select(x => FetchFeedContent(x)).ToList();

            await Task.WhenAll(tasks);

            return tasks.Select(x => x.Result);
        }

        public async Task<FeedModel> FetchFeedContent(History hsty)
        {
            FeedModel result = new FeedModel(hsty);

            switch (hsty.Type)
            {
                case 1:
                    result.Content = await LCDal.LoadQuestionForFeed(hsty.ReadInfoStringByIndex(0)).ContinueWith(t => new QuestionInfoModel(t.Result));
                    break;
                case 2:
                    result.Content = await LCDal.LoadAnswerForFeed(hsty.ReadInfoStringByIndex(0)).ContinueWith(t => new AnswerInfoModel(t.Result));
                    break;
                case 5:
                    result.Content = await LCDal.LoadAnswerForFeed(hsty.ReadInfoStringByIndex(0)).ContinueWith(t => new AnswerInfoModel(t.Result));
                    break;
                case 8:
                    result.Content = await LCDal.LoadQuestionForFeed(hsty.ReadInfoStringByIndex(0)).ContinueWith(t => new QuestionInfoModel(t.Result));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("输入数据非动态类型，输入类型：" + hsty.Type.ToString());
            }

            return result;
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