using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class SearchResult
    {
        public SearchResult()
        {
            QuestionResults = new List<QuestionInfo>();
            UserResults = new List<UserTag>();
        }

        public SearchResult(IEnumerable<AVObject> results)
        {
            QuestionResults = new List<QuestionInfo>();
            UserResults = new List<UserTag>();
            GenerateSearchResultObject(results);
        }

        public SearchResult(IEnumerable<AVObject> results, SearchType type)
        {
            QuestionResults = new List<QuestionInfo>();
            UserResults = new List<UserTag>();

            switch (type)
            {
                case SearchType.All:
                    GenerateSearchResultObject(results);
                    break;
                case SearchType.Question:
                case SearchType.User:
                    GenerateSearchResultObject(results, type);
                    break;
                default:
                    throw new IndexOutOfRangeException("错误：不能识别的搜索类型。");
            }
        }

        private const string _questionClassName = "Post";
        private const string _userClassName = "_User";

        public SearchType ResultType { get; set; }

        public int ResultCount { get; set; }

        public List<QuestionInfo> QuestionResults { get; set; }

        public List<UserTag> UserResults { get; set; }

        private void GenerateSearchResultObject(IEnumerable<AVObject> results)
        {
            ResultType = SearchType.All;
            ResultCount = results.Count();

            QuestionResults.AddRange(results.Where(x => x.ClassName == _questionClassName).Select(x => new QuestionInfo(x)));
            UserResults.AddRange(results.Where(x => x.ClassName == _userClassName).Select(x => new UserTag(x as AVUser)));
        }

        private void GenerateSearchResultObject(IEnumerable<AVObject> results, SearchType type)
        {
            ResultType = type;
            ResultCount = results.Count();

            switch (type)
            {
                case SearchType.Question:
                    QuestionResults.AddRange(results.Select(x => new QuestionInfo(x)));
                    break;
                case SearchType.User:
                    UserResults.AddRange(results.Select(x=>new UserTag(x as AVUser)));
                    break;
                default:
                    throw new IndexOutOfRangeException("错误：不能识别的搜索类型。");
            }
        }
    }
}
