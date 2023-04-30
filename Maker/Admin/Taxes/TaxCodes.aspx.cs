using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers.MakerShop;
using MakerShop.Utility;

public partial class Admin_Taxes_TaxCodes : MakerShop.Web.UI.MakerShopAdminPage
{
    protected void AddButton_Click(object sender, EventArgs e)
    {
        // DUPLICATE TAX CODE NAMES SHOULD NOT BE ALLOWED                        
        TaxCodeCollection tempCollection = TaxCodeDataSource.LoadForCriteria("Name = '" + StringHelper.SafeSqlString(AddTaxCodeName.Text) + "'");
        if (tempCollection.Count > 0)
        {   
            // TAX RULE(S) WITH SAME NAME ALREADY EXIST
            CustomValidator customNameValidator = new CustomValidator();
            customNameValidator.ValidationGroup = "Add";
            customNameValidator.ControlToValidate = "AddTaxCodeName";
            customNameValidator.Text = "*";
            customNameValidator.ErrorMessage = "A Tax Code with the same name <strong>" + AddTaxCodeName.Text + "</strong> already exists.";
            customNameValidator.IsValid = false;
            phNameValidator.Controls.Add(customNameValidator);

            return;
        }


        TaxCode t = new TaxCode();
        t.Name = AddTaxCodeName.Text;
        t.Save();
        TaxCodeGrid.DataBind();
        AddTaxCodeName.Text = string.Empty;
    }

    protected void TaxCodeGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int taxCodeId = AlwaysConvert.ToInt(TaxCodeGrid.DataKeys[e.RowIndex].Value);
        TaxCode t = TaxCodeDataSource.Load(taxCodeId);
        TextBox Name = (TextBox)TaxCodeGrid.Rows[e.RowIndex].FindControl("Name");
        if (t != null && Name != null)
        {

            // IF NAME IS CHANGED DUPLICATE TAX CODE NAMES SHOULD NOT BE ALLOWED                        
            if (t.Name != Name.Text)
            {
                TaxCodeCollection tempCollection = TaxCodeDataSource.LoadForCriteria("Name = '" + StringHelper.SafeSqlString(Name.Text) + "'");
                if (tempCollection.Count > 0)
                {
                    PlaceHolder phGridNameValidator = (PlaceHolder)TaxCodeGrid.Rows[e.RowIndex].FindControl("phGridNameValidator");
                    // TAX RULE(S) WITH SAME NAME ALREADY EXIST
                    CustomValidator customNameValidator = new CustomValidator();
                    customNameValidator.ValidationGroup = "EditTaxRule";
                    customNameValidator.ControlToValidate = "Name";
                    customNameValidator.Text = "A Tax Code with the same name already exists.";
                    customNameValidator.ErrorMessage = "A Tax Code with the same name already exists.";
                    customNameValidator.IsValid = false;
                    phGridNameValidator.Controls.Add(customNameValidator);

                    e.Cancel = true;
                    return;
                }
            }

            t.Name = Name.Text;
            t.Save();
        }
        e.Cancel = true;
        TaxCodeGrid.EditIndex = -1;
        TaxCodeGrid.DataBind();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetDefaultButton(AddTaxCodeName, AddButton.ClientID);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // DISPLAY THE WARNING MESSAGE WHEN TAXES ARE DISABLED
        TaxGateway taxGateway = null;
        MakerShopTax taxProvider = null;
        int taxGatewayId = TaxGatewayDataSource.GetTaxGatewayIdByClassId(Misc.GetClassId(typeof(MakerShopTax)));
        if (taxGatewayId > 0) taxGateway = TaxGatewayDataSource.Load(taxGatewayId);
        if (taxGateway != null) taxProvider = taxGateway.GetProviderInstance() as MakerShopTax;
        //CREATE NEW INSTANCES IF EXISTING RECORDS NOT FOUND        
        if (taxProvider == null) taxProvider = new MakerShopTax();
        TaxSettingsWarning.Visible = !taxProvider.Activated;
    }
}
