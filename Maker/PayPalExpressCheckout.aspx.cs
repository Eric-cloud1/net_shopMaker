using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.paypal.soap.api;
using MakerShop.Payments;
using MakerShop.Payments.Providers.PayPal;
using MakerShop.Orders;
using MakerShop.Shipping;
using MakerShop.Utility;

partial class PayPalExpressCheckout : MakerShop.Web.UI.MakerShopPage
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        string action = AlwaysConvert.ToString(Request.QueryString["Action"]).Trim().ToUpperInvariant();
        switch (action)
        {
            case "GET":
                GetExpressCheckout();
                break;
            case "RETRY":
            case "SET":
                ExpressCheckoutSession.Delete(MakerShop.Common.Token.Instance.User);
                SetExpressCheckout();
                break;
            case "DO":
                DoExpressCheckout();
                break;
            case "CANCEL":
                CancelExpressCheckout();
                break;
            case "ERROR":
                DisplayExpressCheckoutErrors();
                break;
            default:
                RedirectWithDefaultError();
                break;
        }
    }

    private void CancelExpressCheckout()
    {
        ExpressCheckoutSession.Delete(MakerShop.Common.Token.Instance.User);
        string sReturnURL = Request.QueryString["ReturnURL"];
        if (!string.IsNullOrEmpty(sReturnURL))
        {
            switch (sReturnURL)
            {
                case "REF":
                    Response.Redirect(Request.UrlReferrer.ToString());
                    break;
                case "PAY":
                    Response.Redirect(NavigationHelper.GetPaymentUrl());
                    break;
                case "SHIP":
                    Response.Redirect(NavigationHelper.GetShipMethodUrl());
                    break;
            }
        }
        Response.Redirect(NavigationHelper.GetBasketUrl());
    }

    private void SetExpressCheckout()
    {
        PayPalProvider client = StoreDataHelper.GetPayPalProvider();
        PayPalProvider.ExpressCheckoutResult result = client.SetExpressCheckout();

        // IF ERRORS ARE PRESENT, REDIRECT TO DISPLAY THEM
        if (result.Errors != null) RedirectWithErrors(result.Errors);

        // NO ERRORS FOUND, SEND TO SPECIFIED REDIRECT URL
        Response.Redirect(result.RedirectUrl);
    }

    private void GetExpressCheckout()
    {
        PayPalProvider client = StoreDataHelper.GetPayPalProvider();
        GetExpressCheckoutResult result = client.GetExpressCheckout();

        // IF ERRORS ARE PRESENT, REDIRECT TO DISPLAY THEM
        if (result.Errors != null) RedirectWithErrors(result.Errors);

        // NO ERRORS FOUND, IF THERE ARE SHIPPABLE ITEMS REDIRECT TO SELECTSHIPPING METHOD
        Basket paypalBasket = result.PayPalUser.Basket;
        if (paypalBasket.Items.HasShippableProducts) Response.Redirect(NavigationHelper.GetShipMethodUrl());

        // NO SHIPPABLE ITEMS, SHOW THE PAYMENT / CONFIRMATION PAGE
        Response.Redirect(NavigationHelper.GetPaymentUrl());
    }

    private void DoExpressCheckout()
    {
        PayPalProvider client = StoreDataHelper.GetPayPalProvider();
        PayPalProvider.ExpressCheckoutResult result = client.DoExpressCheckout();

        // LOOK FOR AN ORDERID TO INDICATE THE CHECKOUT SUCCEEDED
        if (result.OrderId != 0)
        {
            // ORDER ID LOCATED, SEND TO THE RECEIPT PAGE
            Response.Redirect(NavigationHelper.GetReceiptUrl(result.OrderId));
        }

        // THE CHECKOUT FAILED, SEE IF WE HAVE ERRORS AVAIALBLE
        if (result.Errors != null && result.Errors.Length > 0)
            RedirectWithErrors(result.Errors);

        // NO ERRORS AVAILABLE, USE DEFAULT
        RedirectWithDefaultError();
    }

    /// <summary>
    /// Redirects to error page to prevent refresh from causing a retry attempt
    /// </summary>
    /// <param name="errors">The error(s) that occurred during the operation.</param>
    private void RedirectWithErrors(ErrorType[] errors)
    {
        Session["PayPalExpressCheckoutErrors"] = errors;
        Response.Redirect("PayPalExpressCheckout.aspx?Action=ERROR");
    }

    /// <summary>
    /// Redirects to error page with a default error message
    /// </summary>
    private void RedirectWithDefaultError()
    {
        // CHECKOUT FAILED AND NO ERROR MESSAGE IS INDICATED
        // SET A DEFAULT MESSAGE AND REDIRECT
        ErrorType defaultError = new ErrorType();
        defaultError.ErrorCode = "ORDER";
        defaultError.SeverityCode = SeverityCodeType.Error;
        defaultError.ShortMessage = "The checkout could not be completed and your payment was not processed.";
        defaultError.LongMessage = "The checkout could not be completed and your payment was not processed.";
        ErrorType[] errors = { defaultError };
        RedirectWithErrors(errors);
    }

    private void DisplayExpressCheckoutErrors()
    {
        // LOOK FOR ERRORS IN SESSION TO DISPLAY
        ErrorType[] errors = Session["PayPalExpressCheckoutErrors"] as ErrorType[];

        // TRACK WHETHER WE HAVE AN ORDER ERROR - ORDER ERRORS SHOULD NOT PROVIDE A RETRY OPTION
        bool orderErrorFound = false;

        // DISPLAY ADDITIONAL ERROR MESSAGES (IF AVAILABLE)
        if (errors != null && errors.Length > 0)
        {
            foreach (ErrorType error in errors)
            {
                if (error.ErrorCode == "ORDER") orderErrorFound = true;
                ErrorList.Items.Add(error.LongMessage);
            }
        }

        // UPDATE VISIBLE BUTTONS BASED ON PRESENCE OF ORDER ERROR
        BasketLink.Visible = orderErrorFound;
        RetryLink.Visible = !orderErrorFound;
        CancelLink.Visible = !orderErrorFound;
    }
}