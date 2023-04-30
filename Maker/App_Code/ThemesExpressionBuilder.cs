using System.CodeDom;
using System.ComponentModel;
using System.Configuration;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.UI;
using System.Text.RegularExpressions;
// '' <summary>
// '' Class that enables the creation of expressions containing the current theme or stylesheet theme as set in 
// '' the <c>pages</c> section of web.config.
// '' </summary>
// '' <remarks>
// '' <para>The following two types of expressions are supported:
// ''    1. <c><%$ Themes:StylesheetTheme %></c> or
// '' 2. <c><%$ Themes:StylesheetTheme(<some format string>) %></c>, where <c><some format 
// '' string></c> can contain one placeholder, such as '~/App_Themes/{0}/Images/someimage.gif'.</para>''' 
// '' <para>You can query both the <c>StyleSheetTheme</c> and <c>Theme</c> properties. The respective expressions
// '' are <c><%$ Themes:StylesheetTheme %></c> and <c><%$ Themes:Theme %></c>.</para>
// '' </remarks>
[ExpressionPrefix("Themes")]
public class ThemesExpressionBuilder : System.Web.Compilation.ExpressionBuilder
{

    private const string STYLESHEET_THEME = "stylesheettheme";

    private const string THEME = "theme";

    public override bool SupportsEvaluate
    {
        get
        {
            return true;
        }
    }

    public override System.CodeDom.CodeExpression GetCodeExpression(System.Web.UI.BoundPropertyEntry entry, object parsedData, System.Web.Compilation.ExpressionBuilderContext context)
    {
        CodeExpression[] expressionArray = new CodeExpression[0];
        expressionArray[0] = new CodePrimitiveExpression(entry.Expression.Trim());
        return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(base.GetType()), "GetEvalData", expressionArray);
    }

    public override object EvaluateExpression(object target, System.Web.UI.BoundPropertyEntry entry, object parsedData, System.Web.Compilation.ExpressionBuilderContext context)
    {
        return GetEvalData(entry.Expression);
    }

    public static object GetEvalData(string expression)
    {
        string result = string.Empty;
        //  Get the Web application configuration.
        Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
        //  Get the 'pages' section.
        PagesSection pagSection = ((PagesSection)(config.GetSection("system.web/pages")));
        bool hasArgs = System.Text.RegularExpressions.Regex.IsMatch(expression, ".+\\(.+\\)", (RegexOptions.IgnorePatternWhitespace
                        | (RegexOptions.Compiled | RegexOptions.IgnoreCase)));
        // Determine the expression result
        if (expression.ToLowerInvariant().StartsWith(STYLESHEET_THEME))
        {
            if (hasArgs)
            {
                expression = expression.Substring((STYLESHEET_THEME.Length + 1), (expression.Length
                                - (STYLESHEET_THEME.Length - 2)));
                System.Web.UI.Control helper = new Control();
                result = helper.ResolveUrl(string.Format(expression, pagSection.StyleSheetTheme));
            }
            else
            {
                result = pagSection.StyleSheetTheme;
            }
        }
        else if (expression.ToLowerInvariant().StartsWith(THEME))
        {
            if (hasArgs)
            {
                expression = expression.Substring((THEME.Length + 1), (expression.Length
                                - (THEME.Length - 2)));
                System.Web.UI.Control helper = new Control();
                result = helper.ResolveUrl(string.Format(expression, pagSection.Theme));
            }
            else
            {
                result = pagSection.Theme;
            }
        }
        return result;
    }
}