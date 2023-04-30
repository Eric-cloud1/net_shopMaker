using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Payments.Providers.PayPal;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Utility;

/// <summary>
/// Classes that assist in retrieving frequently requested data.
/// </summary>
public static class StoreDataHelper
{
    /// <summary>
    /// Get the payment methods available to a user.
    /// </summary>
    /// <param name="userId">The user to load payment methods for.</param>
    /// <returns>A collection of payment methods for the given user.</returns>
    public static PaymentMethodCollection GetPaymentMethods(int userId)
    {
        string cacheKey = "PaymentMethods_" + userId.ToString();
        HttpContext context = HttpContext.Current;
        if ((context != null) && (context.Items.Contains(cacheKey))) return (PaymentMethodCollection)context.Items[cacheKey];
        PaymentMethodCollection methods = MakerShop.Payments.PaymentMethodDataSource.LoadForUser(userId);
        if (context != null) context.Items[cacheKey] = methods;
        return methods;
    }

    public static PayPalProvider GetPayPalProvider()
    {
        int gatewayId = PaymentGatewayDataSource.GetPaymentGatewayIdByClassId(Misc.GetClassId(typeof(PayPalProvider)));
        if (gatewayId != 0)
        {
            PaymentGateway gateway = new PaymentGateway();
            gateway.Load(gatewayId);
            return (PayPalProvider)gateway.GetInstance();
        }
        return null;
    }

    public static string GetFriendlyPaymentStatus(Payment payment)
    {
        switch (payment.PaymentStatus)
        {
            case PaymentStatus.Unprocessed:
            case PaymentStatus.Authorized:
            case PaymentStatus.AuthorizationFailed:
            case PaymentStatus.CaptureFailed:
                return "Pending";
            case PaymentStatus.AuthorizationPending:
            case PaymentStatus.CapturePending:
                if (!string.IsNullOrEmpty(payment.PaymentStatusReason))
                    return "Pending (" + payment.PaymentStatusReason + ")";
                return "Pending";
            default:
                return payment.PaymentStatus.ToString();
        }
    }

    public static string GetFriendlyShipMethodType(ShipMethod method)
    {
        if (method.ShipGateway != null)
        {
            return method.ShipGateway.Name;
        } else {
            switch(method.ShipMethodType)
            {
                case ShipMethodType.FlatRate: return "Flat Rate";
                case ShipMethodType.WeightBased: return "Vary by Weight";
                case ShipMethodType.CostBased: return "Vary by Cost";
                case ShipMethodType.QuantityBased: return "Vary by Quantity";
                default: return string.Empty;
            }
        }
    }

    public static string GetFriendlyStoreEventName(StoreEvent storeEvent)
    {
        switch (storeEvent)
        {
            case StoreEvent.OrderPlaced: return "Order was placed";
            case StoreEvent.OrderPaid: return "Order with shippable items was fully paid";
            case StoreEvent.OrderPaidNoShipments: return "Order without shippable items was fully paid";
            case StoreEvent.OrderPaidPartial: return "Order was partially paid (balance remains)";
            case StoreEvent.OrderPaidCreditBalance: return "Order was overpaid (credit remains)";
            case StoreEvent.OrderShipped: return "Order was fully shipped";
            case StoreEvent.OrderShippedPartial: return "Order was partially shipped";
            case StoreEvent.ShipmentShipped: return "Shipment was shipped";
            case StoreEvent.OrderNoteAddedByMerchant: return "Order note was added by merchant";
            case StoreEvent.OrderNoteAddedByCustomer: return "Order note was added by customer";
            case StoreEvent.OrderStatusUpdated: return "Order status was updated";
            case StoreEvent.OrderCancelled: return "Order was cancelled";
            case StoreEvent.GiftCertificateValidated: return "Pending gift certificate validated";
            case StoreEvent.PaymentAuthorized: return "Payment authorization succeeded";
            case StoreEvent.PaymentAuthorizationFailed: return "Payment authorization failed";
            case StoreEvent.PaymentCaptured: return "Payment was fully captured";
            case StoreEvent.PaymentCapturedPartial: return "Payment was partially captured";
            case StoreEvent.PaymentCaptureFailed: return "Payment capture failed";
            case StoreEvent.PaymentRefunded: return "Payment refunded";
            case StoreEvent.PaymentVoided: return "Payment voided";
            case StoreEvent.CustomerPasswordRequest: return "Customer requested password assistance";
            case StoreEvent.LowInventoryItemPurchased: return "Product with low inventory was purchased";
            default: return string.Empty;
        }
    }

    public static string TranslateCVVCode(string cvvCode)
    {
        switch (cvvCode)
        {
            case "M": return "Match";
            case "N": return "No Match";
            case "P": return "Not Processed";
            case "S": return "Not Provided";
            case "U": return "Issuer Not Certified";
            case "X": return "No Response";
        }
        return "Unknown";
    }

    public static string TranslateAVSCode(string avsCode)
    {
        switch (avsCode)
        {
            //US DOMESTIC CODES
            case "A": return "Partial Match";
            case "E": return "Invalid";
            case "N": return "No Match";
            case "R": return "Unavailable";
            case "S": return "Not Supported";
            case "U": return "Unavailable";
            case "W": return "Partial Match";
            case "X": return "Match";
            case "Y": return "Match";
            case "Z": return "Partial Match";
            //INTERNATIONAL CODES
            case "B": return "Partial Match";
            case "C": return "No Match";
            case "D": return "Match";
            case "G": return "Not Supported";
            case "I": return "No Match";
            case "M": return "Match";
            case "P": return "Partial Match";
        }
        return "Unknown";
    }

    public static void SetDateFilter(string filter, out DateTime startDate, out DateTime endDate)
    {
        DateTime localNow = LocaleHelper.LocalNow;
        DateTime fromDate;
        switch (filter.ToUpperInvariant())
        {
            case "TODAY":
                startDate = new DateTime(localNow.Year, localNow.Month, localNow.Day);
                endDate = DateTime.MinValue;
                break;
            case "THISWEEK":
                //this week
                DateTime firstDayOfWeek = localNow.AddDays(-1 * (int)localNow.DayOfWeek);
                startDate = new DateTime(firstDayOfWeek.Year, firstDayOfWeek.Month, firstDayOfWeek.Day);
                endDate = DateTime.MinValue;
                break;
            case "LASTWEEK":
                //last week
                DateTime firstDayOfLastWeek = localNow.AddDays((-1 * (int)localNow.DayOfWeek) - 7);
                DateTime lastDayOfLastWeek = firstDayOfLastWeek.AddDays(6);
                startDate = new DateTime(firstDayOfLastWeek.Year, firstDayOfLastWeek.Month, firstDayOfLastWeek.Day);
                endDate = new DateTime(lastDayOfLastWeek.Year, lastDayOfLastWeek.Month, lastDayOfLastWeek.Day, 23, 59, 59);
                break;
            case "THISMONTH":
                //this month
                startDate = new DateTime(localNow.Year, localNow.Month, 1);
                endDate = DateTime.MinValue;
                break;
            case "LASTMONTH":
                //last month
                DateTime lastMonth = localNow.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                DateTime lastDayOfLastMonth = new DateTime(localNow.Year, localNow.Month, 1).AddDays(-1);
                endDate = lastDayOfLastMonth;
                break;
            case "LAST15":
                //LAST 15 DAYS
                fromDate = localNow.AddDays(-15);
                startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                endDate = DateTime.MinValue;
                break;
            case "LAST30":
                //LAST 30 DAYS
                fromDate = localNow.AddDays(-30);
                startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                endDate = DateTime.MinValue;
                break;
            case "LAST60":
                //LAST 60 DAYS
                fromDate = localNow.AddDays(-60);
                startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                endDate = DateTime.MinValue;
                break;
            case "LAST90":
                //LAST 90 DAYS
                fromDate = localNow.AddDays(-90);
                startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                endDate = DateTime.MinValue;
                break;
            case "LAST120":
                //LAST 120 DAYS
                fromDate = localNow.AddDays(-120);
                startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                endDate = DateTime.MinValue;
                break;
            case "THISYEAR":
                //This Year
                startDate = new DateTime(localNow.Year, 1, 1);
                endDate = DateTime.MinValue;
                break;
            default:
                //DEFAULT TO ALL DATES
                startDate = DateTime.MinValue;
                endDate = DateTime.MinValue;
                break;
        }
    }

    public static MakerShop.UI.Styles.Theme[] GetStoreThemes()
    {
        List<MakerShop.UI.Styles.Theme> allThemes = MakerShop.UI.Styles.ThemeDataSource.Load();
        List<MakerShop.UI.Styles.Theme> storeThemes = new List<MakerShop.UI.Styles.Theme>();
        foreach (MakerShop.UI.Styles.Theme theme in allThemes)
        {
            if (!theme.IsAdminTheme) storeThemes.Add(theme);
        }
        if (storeThemes.Count == 0) return null;
        return storeThemes.ToArray();
    }

    public static bool HasGiftCertificates() {
        return MakerShop.Common.Token.Instance.Store.GiftCertificates != null &&
                MakerShop.Common.Token.Instance.Store.GiftCertificates.Count > 0;
    }

    /// <summary>
    /// Gets a list of countries for the store, sorted by name.
    /// </summary>
    /// <returns>A list of countries for the store, sorted by name.</returns>
    public static CountryCollection GetCountryList()
    {
        CountryCollection countries = CountryDataSource.LoadForStore("Name");
        //FIND STORE COUNTRY AND COPY TO FIRST POSITION
        string storeCountry = Token.Instance.Store.DefaultWarehouse.CountryCode;
        if (storeCountry.Length == 0) storeCountry = "US";
        int index = countries.IndexOf(storeCountry);
        if (index > -1)
        {
            Country breakItem = new Country(storeCountry);
            breakItem.Name = "----------";
            countries.Insert(0, breakItem);
            countries.Insert(0, countries[index + 1]);
            if (storeCountry == "US")
            {
                index = countries.IndexOf("CA");
                if (index > -1) countries.Insert(1, countries[index]);
            }
            else if (storeCountry == "CA")
            {
                index = countries.IndexOf("US");
                if (index > -1) countries.Insert(1, countries[index]);
            }
        }
        return countries;
    }

}
