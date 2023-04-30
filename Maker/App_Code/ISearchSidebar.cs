using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Category search side bar implements this interface
/// </summary>
public interface ISearchSidebar
{
    int CategoryId {get; set;}
	int ManufacturerId {get; set;}
	string Keyword {get; set;}

	void SubscribeSidebarUpdated(EventHandler eventHandler);
	void UnsubscribeSidebarUpdated(EventHandler eventHandler);

}
