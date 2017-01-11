using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using RTCareerAsk.PLtoDA;
using RTCareerAsk.Models;
using RTCareerAsk.App_DLL;

namespace RTCareerAsk.Controllers
{
    /// <summary>
    /// 基础控制器，用于提供通用方法。
    /// </summary>
    public class UpperBaseController : Controller
    {
        #region Private Field

        private string _roles;
        private string[] _rolesSplit = new string[0];

        #endregion

        #region Property

        private string TitleMark { get { return "UpperCareer尚职"; } }
        protected string GeneralTitle { get { return "UpperCareer尚职 - 真实的职场知识、经验、见解分享社区"; } }
        protected string MasterSearchKey { get { return "axiaolaimao1017"; } }

        protected Home2DA HomeDa { get { return new Home2DA(); } }
        protected Account2DA AccountDa { get { return new Account2DA(); } }
        protected Question2DA QuestionDa { get { return new Question2DA(); } }
        protected Article2DA ArticleDa { get { return new Article2DA(); } }
        protected Message2DA MessageDa { get { return new Message2DA(); } }
        protected User2DA UserDa { get { return new User2DA(); } }
        protected Test2DA TestDa { get { return new Test2DA(); } }

        public string Roles
        {
            get { return _roles ?? String.Empty; }
            set
            {
                _roles = value;
                _rolesSplit = SplitString(value);
            }
        }

        public bool HasUserInfo
        {
            get { return Session["UserInfo"] != null; }
        }

        public bool HasLoginInfo
        {
            get { return this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("LoginInfo"); }
        }

        public UserInfoModel UserInfo
        {
            get { return Session["UserInfo"] != null ? Session["UserInfo"] as UserInfoModel : null; }
        }

        #endregion

        #region Upper Helper

        protected async Task AutoLogin()
        {
            if (HasUserInfo)
            {
                return;
            }
            else if (HasLoginInfo)
            {
                await AccountLogin(LoadLoginInfo());
            }
        }

        protected async Task AccountLogin(LoginModel model)
        {
            await AccountDa.LoginWithEmail(model.Email, model.Password).ContinueWith(t =>
            {
                StoreUserToSession(t.Result);

                if (model.RememberMe)
                {
                    SaveLoginAsCookie(model.Email, model.Password);
                }
                else
                {
                    RemoveLoginCookie();
                }
            });
        }

        protected string GenerateTitle(string prefix)
        {
            return string.Format("{0} - {1}", prefix, TitleMark);
        }

        protected string GetUserID()
        {
            if (!HasUserInfo)
            {
                throw new InvalidOperationException("错误：未能找到用户信息");
            }

            return UserInfo.UserID;
        }

        protected string GetUserName()
        {
            if (!HasUserInfo)
            {
                throw new InvalidOperationException("错误：未能找到用户信息");
            }

            return UserInfo.Name;
        }

        protected void StoreUserToSession(UserInfoModel um)
        {
            Session["UserInfo"] = um;
        }

        protected void UpdateUserInfo(IDictionary<string, object> newInfo)
        {
            foreach (string key in newInfo.Keys)
            {
                ModifyUserInfo(key, newInfo[key]);
            }
        }

        protected async Task<int> LoadNewMessageCount(string userId)
        {
            return await HomeDa.LoadMessageCount(userId);
        }

        protected async Task UpdateNewMessageCount()
        {
            if (HasUserInfo)
            {
                UpdateUserInfo(new Dictionary<string, object>() { { "NewMessageCount", await LoadNewMessageCount(GetUserID()) } });
            }
        }

        protected void ClearUserFromSession()
        {
            Session["UserInfo"] = null;
        }

        protected string TranslateExceptionMessage(string msg)
        {
            switch (msg)
            {
                case "Could not find user":
                    return "找不到对应的用户";
                case "The username and password mismatch.":
                    return "用户名与密码不匹配";
                case "Email address isn't verified.":
                    return "邮箱地址未验证，请验证后登录";
                case "Username has already been taken":
                    return "用户名已经被占用";
                default:
                    return msg;
            }
        }

        protected List<FileModel> GetUploadedFiles(HttpFileCollectionBase files)
        {
            List<FileModel> fms = new List<FileModel>();

            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload] != null && Request.Files[upload].ContentLength > 0)
                {
                    string fileName = Path.GetFileName(Request.Files[upload].FileName);
                    Stream fileStream = Request.Files[upload].InputStream;
                    IDictionary<string, object> metaData = new Dictionary<string, object>()
                    {
                        {"类型",Request.Files[upload].ContentType},
                        {"用途","Test functions"}
                    };

                    fms.Add(new FileModel()
                    {
                        FileName = fileName,
                        FileDataStream = fileStream,
                        MetaData = metaData
                    });
                }
            }

            return fms;
        }

        protected FileModel CreateFileModelForUpload(HttpPostedFileBase upload, string fileName = "")
        {
            if (upload != null && upload.ContentLength > 0)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Path.GetFileName(upload.FileName);
                }

                return new FileModel()
                {
                    FileName = fileName,
                    FileDataStream = upload.InputStream,
                    MetaData = new Dictionary<string, object>()
                    {
                        {"类型",upload.ContentType}
                    }
                };
            }

            throw new NullReferenceException("未能成功获取上传内容");
        }

        protected async Task<string> UploadImageFile(HttpPostedFileBase file, string fileName = "")
        {
            return await HomeDa.UploadImageFile(CreateFileModelForUpload(file, fileName));
        }

        protected void CopyToSave(string sessionName, object contentCopy)
        {
            Session[sessionName] = contentCopy;
        }

        protected void ClearCopy(string sessionName)
        {
            Session[sessionName] = null;
        }

        protected bool HasSessionCopy(string sessionName)
        {
            return Session[sessionName] != null ? true : false;
        }

        protected T RestoreCopy<T>(string sessionName) where T : class
        {
            return Session[sessionName] as T;
        }

        protected List<SelectListItem> GetFieldList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){Text="未选择",Value="0"},
                new SelectListItem(){Text="科技科研",Value="1"},
                new SelectListItem(){Text="金融",Value="2"},
                new SelectListItem(){Text="教育培训",Value="3"},
                new SelectListItem(){Text="服务业",Value="4"},
                new SelectListItem(){Text="信息传媒",Value="5"},
                new SelectListItem(){Text="制造业",Value="6"},
                new SelectListItem(){Text="地产开发",Value="7"},
                new SelectListItem(){Text="医疗保健",Value="8"},
                new SelectListItem(){Text="运输物流",Value="9"},
                new SelectListItem(){Text="艺术文化",Value="10"},
                new SelectListItem(){Text="体育",Value="11"},
                new SelectListItem(){Text="政府和公共事业",Value="12"},
                new SelectListItem(){Text="贸易零售",Value="13"},
                new SelectListItem(){Text="自由职业",Value="14"}
            };
        }

        protected string ModifyTextareaData(string input, bool isSave)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : isSave ? input.Replace("\r\n", "</br>") : input.Replace("</br>", "\r\n");
        }

        protected void SaveLoginAsCookie(string email, string password)
        {
            HttpCookie cookie = new HttpCookie("LoginInfo");
            cookie.Value = EncryptString(string.Format("{0}/{1}", email, password), "login");
            //cookie.Value = string.Format("{0}!{1}", email, EncryptString(password, email));
            cookie.Expires = DateTime.Now.AddDays(7);

            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
        }

        protected LoginModel LoadLoginInfo()
        {
            if (HasLoginInfo)
            {
                string[] cookieInfo = DecryptString(this.ControllerContext.HttpContext.Request.Cookies["LoginInfo"].Value, "login").Split('/');

                return new LoginModel()
                {
                    Email = cookieInfo[0],
                    Password = cookieInfo[1],
                    RememberMe = true
                };
            }

            return null;
        }

        protected void RemoveLoginCookie()
        {
            if (HasLoginInfo)
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["LoginInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);

                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }

        protected string EncryptString(string plainText, string passPhrase)
        {
            return StringCipher.Encrypt(plainText, passPhrase);
        }

        protected string DecryptString(string plainText, string passPhrase)
        {
            return StringCipher.Decrypt(plainText, passPhrase);
        }

        #endregion

        #region Authorization

        /// <summary>
        /// 验证用户身份是否符合指定的角色
        /// </summary>
        /// <param name="roles">允许通过验证的身份，多个身份请用","分开</param>
        /// <returns>代表身份验证是否通过的Boolean值</returns>
        protected bool IsUserAuthorized(string roles)
        {
            if (HasUserInfo)
            {
                Roles = roles;

                if (UserInfo.RoleNames.Contains("Block"))
                {
                    return false;
                }

                if (_rolesSplit.Length > 0 && _rolesSplit.Intersect(UserInfo.RoleNames).Count() == 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }

        #endregion

        #region Private Method

        private void ModifyUserInfo(string key, object value)
        {
            switch (key)
            {
                case "Name":
                    UserInfo.Name = value.ToString();
                    break;
                case "Portrait":
                    UserInfo.Portrait = value.ToString();
                    break;
                case "NewMessageCount":
                    UserInfo.NewMessageCount = Convert.ToInt32(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("不能识别输入的变量名");
            }
        }

        #endregion
    }
}
