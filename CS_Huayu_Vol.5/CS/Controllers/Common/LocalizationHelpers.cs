using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    public static class LocalizationHelpers
    {
        /// <summary>
        /// 在外边的 Html 中直接使用
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Lang(this HtmlHelper htmlhelper, string key)
        {
            string FilePath = htmlhelper.ViewContext.HttpContext.Server.MapPath("~/") + "Resource\\";
            return GetLangString(htmlhelper.ViewContext.HttpContext, key, FilePath);
        }
        public static string LangOutJsVar(this HtmlHelper htmlhelper, string key)
        {
            string FilePath = htmlhelper.ViewContext.HttpContext.Server.MapPath("~/") + "Resource\\";
            string langstr = GetLangString(htmlhelper.ViewContext.HttpContext, key, FilePath);
            return string.Format("var {0} = \"{1}\";", key, langstr);
        }
        /// <summary>
        /// 在 C# 中使用
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string InnerLang(HttpContextBase httpContext, string key)
        {
            string FilePath = httpContext.Server.MapPath("~/") + "Resource\\";
            return GetLangString(httpContext, key, FilePath);
        }

        private static string GetLangString(HttpContextBase httpContext, string key, string FilePath)
        {
            LangType langtype = LangType.cn;
            if (httpContext.Session["Lang"] != null)
            {
                langtype = (LangType)httpContext.Session["Lang"];
            }
            return LangResourceFileProvider.GetLangString(key, langtype, FilePath);
        }
    }

    public static class LangResourceFileProvider
    {
        public static string GetLangString(string Key, LangType langtype, string FilePath)
        {
            string filename;
            switch (langtype)
            {
                case LangType.cn: filename = "zh-cn.resources"; break;
                case LangType.en: filename = "en-us.resources"; break;
                default: filename = "zh-cn.resources"; break;
            }

            System.Resources.ResourceReader reader = new System.Resources.ResourceReader(FilePath + filename);

            string resourcetype;
            byte[] resourcedata;
            string result = string.Empty;

            try
            {
                reader.GetResourceData(Key, out resourcetype, out resourcedata);
                //去掉第一个字节，无用
                byte[] arr = new byte[resourcedata.Length - 1];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = resourcedata[i + 1];
                }
                result = System.Text.Encoding.UTF8.GetString(arr);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                reader.Close();
            }

            return result;
        }
    }

    public enum LangType
    {
        cn,
        en
    }
}