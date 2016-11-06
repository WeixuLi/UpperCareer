﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        #endregion

        #region Support
        #endregion
    }

    public enum PortraitSize
    {
        Small,
        Medium,
        Large
    }

}