using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.App_DLL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class SearchResultModel
    {
        public SearchResultModel()
        {
            QuestionResults = new List<QuestionInfoModel>();
            UserResults = new List<UserTagModel>();
        }

        public SearchResultModel(SearchResult result)
        {
            QuestionResults = new List<QuestionInfoModel>();
            UserResults = new List<UserTagModel>();

            ConvertSearchResultObjectToModel(result);
        }

        public SearchModelType ResultType { get; set; }

        public string Keyword { get; set; }

        public int ResultCount { get; set; }

        public List<QuestionInfoModel> QuestionResults { get; set; }

        public List<UserTagModel> UserResults { get; set; }

        private void ConvertSearchResultObjectToModel(SearchResult result)
        {
            ResultType = (SearchModelType)result.ResultType;
            ResultCount = result.ResultCount;

            QuestionResults.AddRange(result.QuestionResults.Select(x => new QuestionInfoModel(x)));
            UserResults.AddRange(result.UserResults.Select(x => new UserTagModel(x)));
        }
    }
}