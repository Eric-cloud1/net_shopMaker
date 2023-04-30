using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using MakerShop.Messaging;
using MakerShop.Utility;

public partial class Admin_Store_EmailTemplates_Default : MakerShop.Web.UI.MakerShopAdminPage
{
    protected string GetMailFormat(object dataItem)
    {
        EmailTemplate emailTemplate = (EmailTemplate)dataItem;
        if (emailTemplate.IsHTML) return "HTML";
        else return "Text";
    }

    protected string GetCatalog(object dataItem)
    {
        EmailTemplate emailTemplate = (EmailTemplate)dataItem;
        if (emailTemplate.Triggers.Count == 0) return string.Empty;
        List<string> triggers = new List<string>();
        foreach (EmailTemplateTrigger_Catalog trigger in emailTemplate.TriggerCatalogs)
        {
            triggers.Add("<span style=\"white-space:nowrap\">" + trigger.Name + " (" + trigger.CatalogNodeTypeName + ")</span>");
        }
        return string.Join("<br />", triggers.ToArray());
    }

    protected string GetTriggers(object dataItem)
    {
        EmailTemplate emailTemplate = (EmailTemplate)dataItem;
        if (emailTemplate.Triggers.Count == 0) return string.Empty;
        List<string> triggers = new List<string>();
        foreach (EmailTemplateTrigger trigger in emailTemplate.Triggers)
        {
            triggers.Add("<span style=\"white-space:nowrap\">" + StoreDataHelper.GetFriendlyStoreEventName(trigger.StoreEvent) + "(" + trigger.PaymentType+")</span>");
        }
        return string.Join("<br />", triggers.ToArray());
    }
}