using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IntelligentFitness.Models
{

    public static class HtmlHelperExtensions
    {

      public static MvcHtmlString LinkToRemoveNestedForm(this HtmlHelper htmlHelper, string linkText, string container, string deleteElement) {
          var js = string.Format("javascript:removeNestedForm(this,'{0}','{1}');return false;", container, deleteElement);
          TagBuilder tb = new TagBuilder("a");
          tb.Attributes.Add("href", "#");
          tb.Attributes.Add("onclick", js);
          tb.InnerHtml = linkText;
          var tag = tb.ToString(TagRenderMode.Normal);
          return MvcHtmlString.Create(tag);
      }

      public static MvcHtmlString LinkToAddNestedForm<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string linkText, string containerElement, string counterElement, string cssClass = null) where TProperty : IEnumerable<object>
      {
          string fakeindex = Guid.NewGuid().ToString();

          // pull the name and type from the passed in expression
          string collectionProperty = ExpressionHelper.GetExpressionText(expression);
          var nestedObject = Activator.CreateInstance(typeof(TProperty).GetGenericArguments()[0]);

          // save the field prefix name so we can reset it when we're done
          string oldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
          // if the prefix isn't empty, then prepare to append to it by appending another delimiter
          if (!string.IsNullOrEmpty(oldPrefix))
          {
              htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix += ".";
          }
          // append the collection name and our fake index to the prefix name before rendering
          htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix += string.Format("{0}[{1}]", collectionProperty, fakeindex);
          string partial = System.Web.Mvc.Html.EditorExtensions.EditorFor(htmlHelper, x => nestedObject).ToHtmlString();


          // done rendering, reset prefix to old name
          htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;


          // strip out the fake name injected in (our name was all in the prefix)
          partial = Regex.Replace(partial, @"[\._]?nestedObject", "");


          // encode the output for javascript since we're dumping it in a JS string
          partial = HttpUtility.JavaScriptStringEncode(partial);


          // create the link to render
          var js = string.Format("javascript:addNestedForm(this,'{0}','{1}','{2}');return false;", counterElement, fakeindex, partial);
          TagBuilder a = new TagBuilder("a");
          a.Attributes.Add("href", "javascript:void(0)");
          a.Attributes.Add("onclick", js);
          if (cssClass != null)
          {
              a.AddCssClass(cssClass);
          }
          a.InnerHtml = linkText;

          return MvcHtmlString.Create(a.ToString(TagRenderMode.Normal));
      }
    }
}