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
    public class Article2DA : DABase
    {
        public async Task<ArticlePostModel> CreatePostModelWithReference(string id)
        {
            return await LCDal.LoadReference(id).ContinueWith(t =>
                {
                    return new ArticlePostModel(t.Result);
                });
        }

        public async Task<List<ArticleInfoModel>> LoadArticleList(int pageIndex = 0)
        {
            return await LCDal.LoadArticleList(pageIndex).ContinueWith(t =>
                {
                    return t.Result != null && t.Result.Count() > 0 ? t.Result.Select(x => new ArticleInfoModel(x)).ToList() : new List<ArticleInfoModel>();
                });
        }

        public async Task<bool> PostNewArticle(ArticlePostModel model)
        {
            return await LCDal.SaveNewArticle(model.CreatePostForSave());
        }

        public async Task<ArticleCommentModel> PostNewArticleComment(ArticleCommentPostModel model)
        {
            return await LCDal.SaveNewArticleComment(model.CreatePostForSave()).ContinueWith(t =>
                {
                    return new ArticleCommentModel(t.Result);
                });
        }

        public async Task<ArticleModel> LoadArticleDetail(string id)
        {
            return await LCDal.LoadArticle(id).ContinueWith(t =>
            {
                return new ArticleModel(t.Result);
            });
        }

        public async Task<List<ArticleCommentModel>> LoadArticleCommentList(string atclId, int pageIndex)
        {
            return await LCDal.LoadArticleComments(atclId, pageIndex).ContinueWith(t =>
                {
                    return t.Result != null && t.Result.Count() > 0 ? t.Result.Select(x => new ArticleCommentModel(x)).ToList() : new List<ArticleCommentModel>();
                });
        }

        public async Task<ArticleCommentModel> DeleteArticleComment(string acmtId, string atclId, int replaceIndex)
        {
            return await LCDal.DeleteArticleComment(acmtId, atclId, replaceIndex).ContinueWith(t =>
                {
                    return replaceIndex > 0 && t.Result != null ? new ArticleCommentModel(t.Result) : default(ArticleCommentModel);
                });
        }
    }
}