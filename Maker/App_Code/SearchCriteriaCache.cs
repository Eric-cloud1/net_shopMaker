using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for SearchCriteriaCache
/// </summary>
public static class SearchCache
{
    public static SearchCriteria GetCriteria()
    {
        HttpContext context = HttpContext.Current;
        string page = context.Request.AppRelativeCurrentExecutionFilePath;
        return (context.Session["SearchCache_Criteria_" + page] as SearchCriteria);
    }

    public static void SetCriteria(SearchCriteria criteria)
    {
        HttpContext context = HttpContext.Current;
        string page = context.Request.AppRelativeCurrentExecutionFilePath;
        context.Session["SearchCache_Criteria_" + page] = criteria;
    }

    [Serializable]
    public class SearchCriteria
    {
        public Dictionary<string, object> Arguments;
        public int PageIndex;
        public string SortExpression;
        public SortDirection SortDirection;

        public SearchCriteria()
        {
            Arguments = new Dictionary<string, object>();
            PageIndex = 0;
            SortExpression = string.Empty;
            SortDirection = SortDirection.Ascending;
        }
    }
}

