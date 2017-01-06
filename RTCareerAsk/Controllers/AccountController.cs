using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using RTCareerAsk.Filters;
using RTCareerAsk.Models;
using RTCareerAsk.BL;
using RTCareerAsk.PLtoDA;

namespace RTCareerAsk.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership] //Placeholder: Comment this area to disable WebSecurity functions.
    public class AccountController : UpperBaseController
    {
        #region Upper Included

        //
        // GET: /Account/Login

        [UpperResult]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, bool fromRegister = false)
        {
            ViewBag.Title = GeneralTitle;

            if (fromRegister)
            {
                ViewBag.FromRegister = true;
                ViewBag.ReturnUrl = returnUrl;
                bool hasTempData = (TempData["errorMsg"] != null) && (TempData["regiModel"] != null);

                if (hasTempData)
                {
                    ModelState.AddModelError("", TempData["errorMsg"].ToString());
                }

                return View(new LoginRegiModel() { Register = hasTempData ? TempData["regiModel"] as RegisterModel : new RegisterModel() });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginRegiModel());
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {

            #region Login Method Predefined
            //if (ModelState.IsValid && WebSecurity.Login(model.Login.UserName, model.Login.Password, persistCookie: model.Login.RememberMe))
            //{
            //    return RedirectToLocal(returnUrl);
            //}
            #endregion

            #region Login Method Original
            //if (ModelState.IsValid && await LeanCloudSecurity.Login(model.Login.UserName, model.Login.Password))
            //{
            //    if (model.Login.RememberMe)
            //    {
            //        FormsAuthentication.SetAuthCookie(model.Login.UserName, model.Login.RememberMe);
            //    }

            //    await AccountDa.GetUserModel(model.Login.UserName).ContinueWith(t => StoreUserToSession(t.Result));

            //    return RedirectToLocal(returnUrl);
            //}

            //// If we got this far, something failed, redisplay form
            //ModelState.AddModelError("", "用户名或密码不正确");
            //return View(model);
            #endregion

            #region Login Method Updated
            if (ModelState.IsValid)
            {
                try
                {
                    await AccountLogin(model);

                    return RedirectToLocal(returnUrl);
                }
                catch (Exception e)
                {
                    while (e.InnerException != null) e = e.InnerException;
                    ModelState.AddModelError("", TranslateExceptionMessage(e.Message));
                    return View(new LoginRegiModel() { Login = model });
                }
            }

            ModelState.AddModelError("", "用户名或密码不正确");
            return View(new LoginRegiModel() { Login = model });
            #endregion
        }

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous] //Placeholder: Self added code.
        public ActionResult LogOff()
        {
            //WebSecurity.Logout();

            ClearUserFromSession();
            RemoveLoginCookie();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model, string returnUrl)
        {
            string eMsg = "注册出现了问题: ";

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    //WebSecurity.Login(model.UserName, model.Password);
                    if (await LeanCloudSecurity.ValidateInviteCode(model.InviteCode))
                    {
                        if (await LeanCloudSecurity.CreateUserAndAccount(model.RestoreRegisterModelToUserObject(), model.InviteCode))
                        {
                            //await AccountLogin(model.UserName, model.Password);//.ContinueWith(t => StoreUserToSession(t.Result));

                            return RedirectToAction("RegisterSuccess", new { returnUrl = returnUrl, email = model.Email });
                        }
                    }
                    else
                    {
                        throw new NullReferenceException("您输入的邀请码无效，请查看");
                    }
                    //if (await LeanCloudSecurity.Login(model.Register.UserName, model.Register.Password))
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //    return RedirectToLocal(returnUrl);
                    //};
                }
                catch (Exception e)
                {
                    //ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    while (e.InnerException != null) e = e.InnerException;
                    eMsg += TranslateExceptionMessage(e.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            TempData["regiModel"] = model;
            TempData["errorMsg"] = eMsg;

            return RedirectToAction("Login", new { returnUrl = returnUrl, fromRegister = true });
            //return View(model);
        }

        #endregion

        #region Upper Exclusive

        [UpperResult]
        [AllowAnonymous]
        public async Task<ActionResult> UserManage()
        {
            try
            {
                await AutoLogin();

                if (!IsUserAuthorized("User,Admin"))
                {
                    throw new InvalidOperationException("未登录不能访问此页面");
                }

                await UpdateNewMessageCount();

                UserManageModel model = await AccountDa.LoadUserManageInfo(GetUserID());
                model.SelfDescription = ModifyTextareaData(model.SelfDescription, false);

                ViewBag.Title = GenerateTitle("个人设置");
                ViewBag.FieldList = GetFieldList();

                return View(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        public ActionResult RegisterSuccess(string returnUrl, string email)
        {
            ViewBag.IsAdmin = IsUserAuthorized("Admin");
            ViewBag.Title = GenerateTitle("注册成功");
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Email = email;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> UserManage(UserManageModel model)
        {
            try
            {
                model.SelfDescription = ModifyTextareaData(model.SelfDescription, true);

                if (await AccountDa.UpdateProfile(model))
                {
                    UpdateUserInfo(new Dictionary<string, object>() { { "Name", model.Name } });

                    return PartialView("_NavBar");
                }

                throw new InvalidOperationException("未能成功保存信息");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult QuickLoginForm(string returnUrl)
        {
            try
            {
                ViewBag.returnUrl = returnUrl;

                return PartialView("_QuickLoginModal", new LoginModel());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult ForgetPasswordForm()
        {
            try
            {
                return PartialView("_ForgetPassword", new ForgetPasswordModel());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ForgetPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AccountDa.ResetPasswordByEmail(model.Email);
                    ViewBag.Email = model.Email;

                    return View("ResetPasswordSuccess");
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> ChangePortrait(HttpPostedFileBase portrait)
        {
            try
            {
                string url = await UploadImageFile(portrait, string.Format("portrait{0}", GetUserID()));

                if (await AccountDa.ChangeUserPortrait(GetUserID(), url))
                {
                    UpdateUserInfo(new Dictionary<string, object>() { { "Portrait", url } });
                }
                else
                {
                    await HomeDa.DeleteFileWithUrl(url);
                }

                return PartialView("_NavBar");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        #endregion

        #region Upper Excluded

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #endregion

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
