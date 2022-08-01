using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BPoint.SharePoint.DataAccess.SP2013
{
    public class UtilityFunctions
    {
        // How to use it:
        //Func<SPWeb, string> GetTitleFromWeb2 = (SPWeb web) => { return web.Title};
        //Func<SPWeb, string> GetTitleFromWeb2 = web => web.Title;
        //string title = DoInSPWeb(url, url, web => web.Title);

        //public static object DoInSPWeb(string siteUrl, string webRelativeUrl, Func<SPWeb, object> funcInSP)
        //{
        //    object result = null;
        //    try
        //    {
        //        using (SPSite site = new SPSite(siteUrl))
        //        {
        //            if (string.IsNullOrEmpty(webRelativeUrl))
        //            {
        //                using (SPWeb web = site.OpenWeb())
        //                {
        //                    result = funcInSP(web);
        //                }
        //            }
        //            else
        //            {
        //                using (SPWeb web = site.OpenWeb(webRelativeUrl))
        //                {
        //                    result = funcInSP(web);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw new Exception(string.Format("Cannot create a web object based on the incoming site url {0} and web relative url {1}", siteUrl, webRelativeUrl));
        //        throw ex;
        //    }

        //    return result;
        //}

        //public static object DoInSPWeb(string siteUrl, Guid webId, Func<SPWeb, object> funcInSP)
        //{
        //    object result = null;
        //    try
        //    {
        //        using (SPSite spSite = new SPSite(siteUrl))
        //        {
        //            using (SPWeb spWeb = spSite.OpenWeb(webId))
        //            {
        //                result = funcInSP(spWeb);
        //            }
        //        }
        //    }
        //    catch 
        //    {
        //        throw new Exception(string.Format("Cannot create a web object based on the incoming site url {0} and web ID {1}", siteUrl, webId.ToString()));
        //    }

        //    return result;
        //}

        //public static string DoInSPSPSite(string siteUrl, Func<SPSite, string> funcInSP)
        //{
        //    string result = string.Empty;
        //    try
        //    {
        //        using (SPSite spSite = new SPSite(siteUrl))
        //        {
        //            result = funcInSP(spSite);
        //        }
        //    }
        //    catch 
        //    {
        //        throw new Exception(string.Format("Cannot create a web object based on the incoming site url {0} ", siteUrl));
        //    }

        //    return result;
        //}
    }
}
