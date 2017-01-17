using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using RTCareerAsk.Models;

namespace RTCareerAsk.App_DLL
{
    public static class UpperHtmlHelpers
    {
        private static string _defaultPortrait = "/Images/defaultPortrait.png";

        #region Helper
        public static string AssignClassWithCondition(this HtmlHelper html, bool condition, string className)
        {
            return condition ? className : string.Empty;
        }

        public static string IsSelected(this HtmlHelper html, string controllers = "", string actions = "", string cssClass = "active")
        {
            ViewContext viewContext = html.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
                viewContext = html.ViewContext.ParentActionViewContext;

            RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            if (String.IsNullOrEmpty(actions))
                actions = currentAction;

            if (String.IsNullOrEmpty(controllers))
                controllers = currentController;

            string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }

        public static string IsActive(this HtmlHelper html, bool isLikeBtn, QuestionModel model, string cssClass = "not-active")
        {
            string newClass = "new";
            bool result = false;

            if (model.IsEditAllowed)
            {
                return cssClass;
            }
            else if (model.IsLike == null)
            {
                return newClass;
            }

            result = Convert.ToBoolean(model.IsLike) == isLikeBtn;

            return result ? cssClass : string.Empty;
        }

        public static string IsActive(this HtmlHelper html, bool isLikeBtn, AnswerModel model, string cssClass = "not-active")
        {
            string newClass = "new";
            bool result = false;

            if (model.IsEditAllowed)
            {
                return cssClass;
            }
            else if (model.IsLike == null)
            {
                return newClass;
            }

            result = Convert.ToBoolean(model.IsLike) == isLikeBtn;

            return result ? cssClass : string.Empty;
        }

        public static IHtmlString UpperPortrait(this HtmlHelper html, string portraitUrl, PortraitSize size)
        {
            return UpperPortrait(html, portraitUrl, size, null);
        }

        public static IHtmlString UpperPortrait(this HtmlHelper html, string portraitUrl, PortraitSize size, object htmlAttributes)
        {
            TagBuilder img = new TagBuilder("img");
            img.MergeAttribute("src", string.IsNullOrEmpty(portraitUrl) ? _defaultPortrait : portraitUrl);
            img.AddCssClass(size == PortraitSize.Small ? "portrait-sm" : size == PortraitSize.Medium ? "portrait-md" : size == PortraitSize.Large ? "portrait-lg" : "");
            img.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { }));

            return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
        }

        public static IHtmlString UpperTab(this AjaxHelper ajax, TabModel model, object htmlAttributes)
        {
            TagBuilder tabWrap = new TagBuilder("div");
            tabWrap.AddCssClass("upper-tab");

            TagBuilder tabContainer = new TagBuilder("div");
            tabContainer.GenerateId("nav-container");
            tabContainer.AddCssClass("tab-container");

            TagBuilder tabItems = new TagBuilder("ul");

            TagBuilder line = new TagBuilder("div");
            line.GenerateId("line");
            line.AddCssClass("line");

            foreach (string key in model.TabItems.Keys)
            {
                TagBuilder li = new TagBuilder("li");

                li.AddCssClass(key == model.ActiveItem ? "active-nav" : "");
                li.AddCssClass("tab-li");

                TagBuilder a = new TagBuilder("a");

                a.SetInnerText(key);
                a.MergeAttribute("href", UrlHelper.GenerateUrl(null, model.TabItems[key].ActionName, model.TabItems[key].ControllerName, new RouteValueDictionary(model.TabItems[key].RouteValues), ajax.RouteCollection, ajax.ViewContext.RequestContext, true));
                a.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { }));
                a.MergeAttributes((model.AjaxOptns ?? new AjaxOptions()).ToUnobtrusiveHtmlAttributes());
                foreach (string name in model.HtmlAttrs.Keys)
                {
                    a.MergeAttribute(name, model.HtmlAttrs[name]);
                }

                li.InnerHtml = a.ToString();
                tabItems.InnerHtml += li.ToString();
            }

            tabContainer.InnerHtml = tabItems.ToString();
            tabContainer.InnerHtml += line.ToString();
            tabWrap.InnerHtml = tabContainer.ToString();

            return MvcHtmlString.Create(tabWrap.ToString());
        }

        public static IHtmlString UpperNameTag(this HtmlHelper html, UserModel model)
        {
            return UpperNameTag(html, model, null);
        }

        public static IHtmlString UpperNameTag(this HtmlHelper html, UserModel model, object htmlAttributes)
        {
            string defaultTitle = "[未提供个人信息]";
            string defaultDivider = "|";

            TagBuilder nameTag = new TagBuilder("div");
            nameTag.AddCssClass("nametag");
            nameTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { }));

            TagBuilder portrait = new TagBuilder("img");
            portrait.AddCssClass("portrait-sm");
            portrait.AddCssClass("photo");
            portrait.MergeAttribute("src", string.IsNullOrEmpty(model.Portrait) ? _defaultPortrait : model.Portrait);
            portrait.MergeAttribute("alt", model.Name);

            TagBuilder link = new TagBuilder("a");
            link.MergeAttribute("href", UrlHelper.GenerateUrl(null, "Index", "User", new RouteValueDictionary(new { id = model.UserID }), html.RouteCollection, html.ViewContext.RequestContext, true));

            TagBuilder name = new TagBuilder("p");
            name.AddCssClass("name");
            name.SetInnerText(model.Name);

            TagBuilder title = new TagBuilder("p");
            title.AddCssClass("info");

            if (string.IsNullOrEmpty(model.Company) && string.IsNullOrEmpty(model.Title))
            {
                title.SetInnerText(defaultTitle);
            }
            else if (string.IsNullOrEmpty(model.Company) || string.IsNullOrEmpty(model.Title))
            {
                title.SetInnerText(string.IsNullOrEmpty(model.Company) ? model.Title : model.Company);
            }
            else
            {
                TagBuilder company = new TagBuilder("span");
                company.AddCssClass("company");
                company.SetInnerText(model.Company);

                TagBuilder position = new TagBuilder("span");
                position.AddCssClass("position");
                position.SetInnerText(model.Title);

                title.InnerHtml = company.ToString();
                title.InnerHtml += defaultDivider;
                title.InnerHtml += position.ToString();
            }

            link.InnerHtml = name.ToString();
            link.InnerHtml += title.ToString();

            nameTag.InnerHtml = portrait.ToString(TagRenderMode.SelfClosing);
            nameTag.InnerHtml += link.ToString();

            return MvcHtmlString.Create(nameTag.ToString());
        }

        public static IHtmlString UpperNameTag(this HtmlHelper html, UserTagModel model)
        {
            return UpperNameTag(html, model, null);
        }

        public static IHtmlString UpperNameTag(this HtmlHelper html, UserTagModel model, object htmlAttributes)
        {
            string defaultTitle = "[未提供个人信息]";
            string defaultDivider = "|";

            TagBuilder nameTag = new TagBuilder("div");
            nameTag.AddCssClass("nametag");
            nameTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { }));

            TagBuilder portrait = new TagBuilder("img");
            portrait.AddCssClass("portrait-sm");
            portrait.AddCssClass("photo");
            portrait.MergeAttribute("src", string.IsNullOrEmpty(model.Portrait) ? _defaultPortrait : model.Portrait);
            portrait.MergeAttribute("alt", model.Name);

            TagBuilder link = new TagBuilder("a");
            link.MergeAttribute("href", UrlHelper.GenerateUrl(null, "Index", "User", new RouteValueDictionary(new { id = model.UserID }), html.RouteCollection, html.ViewContext.RequestContext, true));

            TagBuilder name = new TagBuilder("p");
            name.AddCssClass("name");
            name.SetInnerText(model.Name);

            TagBuilder title = new TagBuilder("p");
            title.AddCssClass("info");

            if (string.IsNullOrEmpty(model.Company) && string.IsNullOrEmpty(model.Title))
            {
                title.SetInnerText(defaultTitle);
            }
            else if (string.IsNullOrEmpty(model.Company) || string.IsNullOrEmpty(model.Title))
            {
                title.SetInnerText(string.IsNullOrEmpty(model.Company) ? model.Title : model.Company);
            }
            else
            {
                TagBuilder company = new TagBuilder("span");
                company.AddCssClass("company");
                company.SetInnerText(model.Company);

                TagBuilder position = new TagBuilder("span");
                position.AddCssClass("position");
                position.SetInnerText(model.Title);

                title.InnerHtml = company.ToString();
                title.InnerHtml += defaultDivider;
                title.InnerHtml += position.ToString();
            }

            link.InnerHtml = name.ToString();
            link.InnerHtml += title.ToString();

            nameTag.InnerHtml = portrait.ToString(TagRenderMode.SelfClosing);
            nameTag.InnerHtml += link.ToString();

            return MvcHtmlString.Create(nameTag.ToString());
        }

        public static IHtmlString UpperAuthor(this HtmlHelper html, string author, string time)
        {
            return UpperAuthor(html, author, time, null);
        }

        public static IHtmlString UpperAuthor(this HtmlHelper html, string author, string time, string targetUrl)
        {
            TagBuilder authorTag = new TagBuilder("small");
            authorTag.AddCssClass("author");

            if (string.IsNullOrEmpty(targetUrl))
            {
                TagBuilder name = new TagBuilder("span");
                name.AddCssClass("left");
                name.SetInnerText(author);
                authorTag.InnerHtml = name.ToString();
            }
            else
            {
                TagBuilder name = new TagBuilder("a");
                name.MergeAttribute("href", targetUrl);
                name.AddCssClass("left");
                name.SetInnerText(author);
                authorTag.InnerHtml = name.ToString();
            }

            TagBuilder span = new TagBuilder("span");
            span.AddCssClass("right");
            span.SetInnerText(time);

            authorTag.InnerHtml += "·";
            authorTag.InnerHtml += span.ToString();

            return MvcHtmlString.Create(authorTag.ToString());
        }

        public static IHtmlString UpperNotification(this HtmlHelper html, NotificationModel model)
        {
            TagBuilder alertText = new TagBuilder("p");
            List<TagBuilder> links = new List<TagBuilder>();

            if (model.From != null && !string.IsNullOrEmpty(model.From.UserID))
            {
                links.Add(new TagBuilder("a"));
                links[0].InnerHtml = model.From.Name;
                links[0].MergeAttribute("href", UrlHelper.GenerateUrl(null, "Index", "User", new RouteValueDictionary(new { id = model.From.UserID }), html.RouteCollection, html.ViewContext.RequestContext, true));
            }

            foreach (string name in model.NameStrings)
            {
                TagBuilder link = new TagBuilder("a");
                link.InnerHtml = name;
                links.Add(link);
            }

            switch (model.Type)
            {
                case NotificationType.LikedQstn:
                    links[1].MergeAttribute("href", UrlHelper.GenerateUrl(null, "QuestionDetail", "Question", new RouteValueDictionary(new { id = model.InfoStrings[0] }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    alertText.InnerHtml = string.Format("{0} 推荐了您提出的问题 {1} 。", links[0].ToString(), links[1].ToString());
                    break;
                case NotificationType.LikedAns:
                    links[1].MergeAttribute("href", UrlHelper.GenerateUrl(null, "AnswerDetail", "Question", new RouteValueDictionary(new { id = model.InfoStrings[0] }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    alertText.InnerHtml = string.Format("{0} 推荐了您在问题 {1} 下的回答。", links[0].ToString(), links[1].ToString());
                    break;
                case NotificationType.CommentAns:
                    links[1].MergeAttribute("href", UrlHelper.GenerateUrl(null, "AnswerDetail", "Question", new RouteValueDictionary(new { id = model.InfoStrings[0] }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    alertText.InnerHtml = string.Format("{0} 评论了您在问题 {1} 下发表的回答。", links[0].ToString(), links[1].ToString());
                    break;
                case NotificationType.RepliedCmt:
                    links[1].MergeAttribute("href", UrlHelper.GenerateUrl(null, "AnswerDetail", "Question", new RouteValueDictionary(new { id = model.InfoStrings[0] }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    alertText.InnerHtml = string.Format("{0} 回复了您在问题 {1} 下发表的评论。", links[0].ToString(), links[1].ToString());
                    break;
                case NotificationType.Answered:
                    links[1].MergeAttribute("href", UrlHelper.GenerateUrl(null, "AnswerDetail", "Question", new RouteValueDictionary(new { id = model.InfoStrings[0] }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    alertText.InnerHtml = string.Format("{0} 在您的问题 {1} 下发表了新回答。", links[0].ToString(), links[1].ToString());
                    break;
                case NotificationType.Published:
                    links[0].MergeAttribute("href", UrlHelper.GenerateUrl(null, "Detail", "Article", new RouteValueDictionary(new { id = model.InfoStrings[0] }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    alertText.InnerHtml = string.Format("祝贺！您在问题 {0} 下的回答已被UpperCareer官方选送推荐至首页。", links[0].ToString());
                    break;
                case NotificationType.Followed:
                    alertText.InnerHtml = string.Format("{0} 关注了你。", links[0].ToString());
                    break;
                default:
                    throw new IndexOutOfRangeException("所给与的提醒类型错误。");
            }

            return MvcHtmlString.Create(alertText.ToString());
        }

        public static IHtmlString UpperFeed(this HtmlHelper html, FeedModel model)
        {
            TagBuilder wrap = new TagBuilder("li");
            wrap.AddCssClass("list-group-item");

            TagBuilder header = new TagBuilder("div");
            header.AddCssClass("feed-header");

            TagBuilder p = new TagBuilder("p");
            TagBuilder link = new TagBuilder("a");
            link.MergeAttribute("href", UrlHelper.GenerateUrl(null, "Index", "User", new RouteValueDictionary(new { id = model.From.UserID }), html.RouteCollection, html.ViewContext.RequestContext, true));
            link.InnerHtml = model.From.Name;
            TagBuilder title = new TagBuilder("a");
            title.AddCssClass("title");
            title.InnerHtml = model.Content.Title;

            TagBuilder body = new TagBuilder("div");
            body.AddCssClass("feed-body");

            switch (model.Type)
            {
                case FeedType.LikedQstn:
                    p.InnerHtml += string.Format("问题被推荐 · {0} · {1}", link.ToString(), model.DateCreate);
                    title.MergeAttribute("href", UrlHelper.GenerateUrl(null, "QuestionDetail", "Question", new RouteValueDictionary(new { id = model.Content.ID}), html.RouteCollection, html.ViewContext.RequestContext, true));
                    body.InnerHtml = html.Partial("_QuestionFeed", model.Content as QuestionInfoModel).ToString();
                    break;
                case FeedType.LikedAns:
                    p.InnerHtml += string.Format("答案被推荐 · {0} · {1}", link.ToString(), model.DateCreate);
                    title.MergeAttribute("href", UrlHelper.GenerateUrl(null, "AnswerDetail", "Question", new RouteValueDictionary(new { id = model.Content.ID }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    body.InnerHtml = html.Partial("_AnswerFeed", model.Content as AnswerInfoModel).ToString();
                    break;
                case FeedType.Answered:
                    p.InnerHtml += string.Format("回答 · {0} · {1}", link.ToString(), model.DateCreate);
                    title.MergeAttribute("href", UrlHelper.GenerateUrl(null, "AnswerDetail", "Question", new RouteValueDictionary(new { id = model.Content.ID }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    body.InnerHtml = html.Partial("_AnswerFeed", model.Content as AnswerInfoModel).ToString();
                    break;
                case FeedType.QuestionPosted:
                    p.InnerHtml += string.Format("提问 · {0} · {1}", link.ToString(), model.DateCreate);
                    title.MergeAttribute("href", UrlHelper.GenerateUrl(null, "QuestionDetail", "Question", new RouteValueDictionary(new { id = model.Content.ID }), html.RouteCollection, html.ViewContext.RequestContext, true));
                    body.InnerHtml = html.Partial("_QuestionFeed", model.Content as QuestionInfoModel).ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("输入数据非动态类型，输入类型：" + model.Type.ToString());
            }

            header.InnerHtml += p.ToString();
            header.InnerHtml += title.ToString();
            wrap.InnerHtml += header.ToString();
            wrap.InnerHtml += body.ToString();

            return MvcHtmlString.Create(wrap.ToString());
        }
        #endregion

        #region Support
        #endregion
    }
}