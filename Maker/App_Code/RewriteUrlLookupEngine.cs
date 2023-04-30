using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
class RewriteUrlLookupEngine
{

    private static string EscapeLiteral(string literalText) {
        StringBuilder escapedText = new StringBuilder();
        char[] literalChars = literalText.ToCharArray();
        string regexChars = ".$^{[(|)*+?\\";
        foreach (char thisChar in literalChars) {
            if ((regexChars.IndexOf(thisChar) > -1)) {
                escapedText.Append("\\");
            }
            escapedText.Append(thisChar);
        }
        return escapedText.ToString();
    }

    public static StringDictionary GetLookupTable(HttpApplication app)
    {
        // LOOK FOR PAGE LOOKUP IN APPLICATION CACHE
        app.Context.Trace.Write(typeof(RewriteUrlLookupEngine).ToString(), "Load lookup table from cache.");
        StringDictionary pageLookup = (StringDictionary)app.Context.Cache["AbleRewrite_PageLookup"];
        if ((pageLookup == null))
        {
            app.Context.Trace.Warn(typeof(RewriteUrlLookupEngine).ToString(), "Generate new lookup table.");
            pageLookup = RewriteUrlLookupEngine.GenerateLookupTable(app);
            app.Context.Cache["AbleRewrite_PageLookup"] = pageLookup;
        }
        return pageLookup;
    }

    private static string FindConfig(HttpApplication app)
    {
        string configFile = string.Empty;
        try
        {
            string[] segments = app.Request.Url.Segments;
            int i = (segments.Length - 1);
            while (((i > 0)
                        && (configFile.Length == 0)))
            {
                if ((segments[i].IndexOf(".aspx") < 0))
                {
                    configFile = Path.Combine(app.Server.MapPath(string.Join(string.Empty, segments)), "AbleRewrite.Config");
                    app.Context.Trace.Write(typeof(RewriteUrlLookupEngine).ToString(), ("Check for config file at " + configFile));
                    if (!File.Exists(configFile))
                    {
                        configFile = string.Empty;
                    }
                }
                segments[i] = string.Empty;
                i--;
            }
        }
        catch
        {
            configFile = string.Empty;
        }
        return configFile;
    }

    private static string LoadRulesFromFile(HttpApplication app)
    {
        // FIRST LOOK IN APPLICATIONROOT
        string rules = string.Empty;
        string configFile = FindConfig(app);
        if ((configFile.Length > 0))
        {
            app.Context.Trace.Write(typeof(RewriteUrlLookupEngine).ToString(), ("Load rules from " + configFile));
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(configFile);
                rules = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                app.Context.Trace.Write(typeof(RewriteUrlLookupEngine).ToString(), ("Error reading rules from file: " + ex.Message));
            }
            finally
            {
                if (!(sr == null))
                {
                    sr.Close();
                    sr = null;
                }
            }
        }
        else
        {
            app.Context.Trace.Write(typeof(RewriteUrlLookupEngine).ToString(), "Load rules from Configuration Settings.");
            NameValueCollection appSettings = null;
            object appObject = System.Configuration.ConfigurationManager.GetSection("AbleRewrite");
            if (!(appObject == null))
            {
                appSettings = ((NameValueCollection)(appObject));
                StringBuilder rulesBuilder = new StringBuilder();
                for (int i = 0; i < appSettings.Count; i++)
                {
                    rulesBuilder.Append("<add key=\"" + appSettings.GetKey(i) + "\" value=\"" + appSettings.Get(i) + "\" />" + "\r\n");
                }
                rules = rulesBuilder.ToString();
            }
        }
        return rules;
    }

    public static StringDictionary GenerateLookupTable(HttpApplication app) {
        StringDictionary _LookupTable = new StringDictionary();
        string rewriteRules = LoadRulesFromFile(app);
        if ((rewriteRules.Length > 0)) {
            string[] lines = rewriteRules.Split("\r\n".ToCharArray());
            Match matchIds;
            string objectType;
            string idList;
            string pageName;
            // PARSE THE REWRITE RULES INTO LOOKUP
            foreach (string currentLine in lines) {
                // <add key="-(C|P|W)\(\d+(?:\|\d+)*)\)" (?:C(\d*))?\.aspx(?:\?(.*))?" value="$1/product1.aspxproduct1.aspx?Product_ID=$2&Category_ID=$3&$4" />
                StringBuilder pattern = new StringBuilder();
                // GET THE TYPE OF CONTENT OBJECT
                pattern.Append("-(C|P|W)");
                // BEGIN THE CAPTURE FOR THE IDS
                pattern.Append("\\((");
                // CAPTURE EITHER DEFAULT RULE OR ID LIST
                pattern.Append(("(?:\\d+|" 
                                + (EscapeLiteral("\\d*") + ")")));
                // CAPTURE ADDITIONAL IDS THAT MAY APPEAR
                pattern.Append("(?:\\|\\d+)*");
                // END THE CAPTURE FOR THE IDS
                pattern.Append(")\\)");
                // THIS MAY OR MAY NOT APPEAR IN THE KEY
                pattern.Append(("(?:" 
                                + (EscapeLiteral("(?:C(\\d*))?") + ")?")));
                pattern.Append(EscapeLiteral("\\.aspx(?:\\?(.*))?\" value=\"$1/"));
                pattern.Append("(.+?)");
                pattern.Append(".aspx");
                matchIds = Regex.Match(currentLine, pattern.ToString());
                if (matchIds.Success) {
                    objectType = matchIds.Groups[1].ToString();
                    idList = matchIds.Groups[2].ToString();
                    pageName = matchIds.Groups[3].ToString();
                    if (!idList.Equals("\\d*")) {
                        string[] idArray = idList.Split("|".ToCharArray());
                        int i;
                        for (i = 0; (i 
                                    <= (idArray.Length - 1)); i++) {
                            _LookupTable[(objectType + idArray[i])] = pageName;
                        }
                    }
                    else {
                        // THIS IS THE DEFAULT RULE, KEY IS JUST THE OBJECT TYPE
                        _LookupTable[objectType] = pageName;
                    }
                }
            }
        }
        app.Context.Trace.Write(typeof(RewriteUrlLookupEngine).ToString(), ("Generated lookup table with " 
                        + (_LookupTable.Count + " entries.")));
        return _LookupTable;
    }

    public static string GetDisplayPage(HttpApplication app, string objectType, string objectId)
    {
        // LOOK FOR PAGE LOOKUP IN APPLICATION CACHE
        StringDictionary pageLookup = RewriteUrlLookupEngine.GetLookupTable(app);
        string displayPage = pageLookup[(objectType + objectId)];
        if (((displayPage == null)
                    || (displayPage.Length == 0)))
        {
            displayPage = pageLookup[objectType];
            if (((displayPage == null)
                        || (displayPage.Length == 0)))
            {
                switch (objectType)
                {
                    case "C":
                        displayPage = "category";
                        break;
                    case "P":
                        displayPage = "product1";
                        break;
                    case "W":
                        displayPage = "webpage";
                        break;
                    default:
                        displayPage = ("unknowntype_" + objectType);
                        break;
                }
            }
        }
        return displayPage;
    }
}