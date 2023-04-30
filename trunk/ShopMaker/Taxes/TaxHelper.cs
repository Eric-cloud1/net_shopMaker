//-----------------------------------------------------------------------
// <copyright file="TaxHelper.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MakerShop.Taxes
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Web;
    using System.Collections.Generic;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using MakerShop.Common;
    using MakerShop.Orders;
    using MakerShop.Taxes;
    using MakerShop.Taxes.Providers;
    using MakerShop.Taxes.Providers.MakerShop;
    using MakerShop.Users;
    using MakerShop.Utility;

    /// <summary>
    /// Summary description for TaxHelper
    /// </summary>
    public static class TaxHelper
    {
        /// <summary>
        /// Gets the MakerShop tax provider
        /// </summary>
        /// <returns>The MakerShop tax provider</returns>
        public static MakerShopTax GetTaxProvider()
        {
            HttpContext context = HttpContext.Current;
            if (context.Items.Contains("MakerShopTax"))
            {
                return context.Items["MakerShopTax"] as MakerShopTax;
            }
            MakerShopTax provider = null;
            string classId = Misc.GetClassId(typeof(MakerShopTax));
            int taxGatewayId = TaxGatewayDataSource.GetTaxGatewayIdByClassId(classId);
            if (taxGatewayId > 0)
            {
                TaxGateway taxGateway = TaxGatewayDataSource.Load(taxGatewayId);
                if (taxGateway != null)
                {
                    provider = taxGateway.GetProviderInstance() as MakerShopTax;
                }
            }
            if (provider == null) provider = new MakerShopTax();
            context.Items["MakerShopTax"] = provider;
            return provider;
        }

        /// <summary>
        /// Indicates how taxes should be displayed in the invoice
        /// </summary>
        public static TaxShoppingDisplay ShoppingDisplay
        {
            get
            {
                MakerShopTax p = GetTaxProvider();
                if (!p.Enabled) return TaxShoppingDisplay.Hide;
                return p.ShoppingDisplay;
            }
        }

        /// <summary>
        /// Indicates how taxes should be displayed in the invoice
        /// </summary>
        public static TaxInvoiceDisplay InvoiceDisplay
        {
            get
            {
                MakerShopTax p = GetTaxProvider();
                if (!p.Enabled) return TaxInvoiceDisplay.Summary;
                return p.InvoiceDisplay;
            }
        }

        /// <summary>
        /// Indicates whether tax column should be shown on invoice
        /// </summary>
        public static bool ShowTaxColumn
        {
            get
            {
                MakerShopTax p = GetTaxProvider();
                return (p.Enabled && p.ShowTaxColumn);
            }
        }

        /// <summary>
        /// If tax column is displayed, contains the text for the column header
        /// </summary>
        public static string TaxColumnHeader
        {
            get
            {
                MakerShopTax p = GetTaxProvider();
                return p.TaxColumnHeader;
            }
        }

        /// <summary>
        /// Gets the price that should be displayed for shopping purposes
        /// </summary>
        /// <param name="price">The price calculated for the product before tax
        /// rules are taken into account</param>
        /// <param name="taxCodeId">The tax code associated with the price</param>
        /// <returns>The price that should be displayed to the current user</returns>
        /// <remarks>Assumes the billing/shipping addresses as the current user billing address</remarks>
        public static LSDecimal GetShopPrice(LSDecimal price, int taxCodeId)
        {
            return GetShopPrice(price, taxCodeId, null, null);
        }

        /// <summary>
        /// Gets the price that should be displayed for shopping purposes
        /// </summary>
        /// <param name="price">The price calculated for the product before tax
        /// rules are taken into account</param>
        /// <param name="taxCodeId">The tax code associated with the price</param>
        /// <param name="billingAddress">Billing address for the item, can be null for current user billing address</param>
        /// <param name="shippingAddress">Shipping address for the item, can be null to use current user billing address as shipping address</param>
        /// <returns>The price that should be displayed to the current user</returns>
        public static LSDecimal GetShopPrice(LSDecimal price, int taxCodeId, TaxAddress billingAddress, TaxAddress shippingAddress)
        {
            MakerShopTax provider = GetTaxProvider();
            if ((taxCodeId == 0) || (price == 0) || (!provider.Enabled)) return price;
            TaxShoppingDisplay taxDisplayMode = TaxHelper.GetEffectiveShoppingDisplay(billingAddress);
            if (taxDisplayMode != TaxShoppingDisplay.Included)
            {
                //SHOPPING PRICES ARE SHOWN WITHOUT TAX
                //IF MERCHANT PRICES DO NOT INCLUDE VAT, DO NOTHING
                if (!provider.PriceIncludesTax) return price;
                //MERCHANT PRICES INCLUDE VAT, WE WANT TO DISPLAY WITHOUT
                TaxInfo info = provider.GetTaxInfo(price, taxCodeId, billingAddress, shippingAddress, Token.Instance.User);
                return info.Price;
            }
            else
            {
                //SHOPPING PRICES ARE SHOWN WITH TAX
                //IF MERCHANT PRICES INCLUDE VAT, DO NOTHING
                if (provider.PriceIncludesTax) return price;
                //MERCHANT PRICE DOES NOT INCLUDE VAT, WE NEED TO GET PRICE WITH INCLUDED
                TaxInfo info = provider.GetTaxInfo(price, taxCodeId, billingAddress, shippingAddress, Token.Instance.User);
                return info.PriceWithTax;
            }
        }

        /// <summary>
        /// Gets the price that should be displayed on the basket
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <returns>The price to display on a basket/checkout page</returns>
        public static LSDecimal GetShopPrice(Basket basket, BasketItem item)
        {
            return GetInvoicePrice(basket, item, true, false);
        }

        /// <summary>
        /// Gets the price that should be displayed on the basket
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <returns>The price to display on a basket/checkout page</returns>
        public static LSDecimal GetShopExtendedPrice(Basket basket, BasketItem item)
        {
            return GetInvoiceExtendedPrice(basket, item, true, false);
        }

        /// <summary>
        /// Gets the price that should be displayed for a basket/checkout page
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <returns>The price to display on a basket/checkout page</returns>
        public static LSDecimal GetInvoicePrice(Basket basket, BasketItem item)
        {
            return GetInvoicePrice(basket, item, false, false);
        }

        /// <summary>
        /// Gets the price that should be displayed for a basket/checkout page
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <param name="forceKitBundling">If true, the invoice price for the item always uses bundle style for kitting even
        /// if the product is set to use itemized display.</param>
        /// <returns>The price to display on a basket/checkout page</returns>
        public static LSDecimal GetInvoicePrice(Basket basket, BasketItem item, bool forceKitBundling)
        {
            return GetInvoicePrice(basket, item, false, forceKitBundling);
        }

        /// <summary>
        /// Gets the price that should be displayed for a basket/checkout page
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <param name="useShoppingRule">if true, use shopping rules to determine if tax is included; if false use invoice rules</param>
        /// <param name="forceKitBundling">If true, the invoice price for the item always uses bundle style for kitting even
        /// if the product is set to use itemized display.</param>
        /// <returns>The price to display on a basket/checkout page</returns>
        private static LSDecimal GetInvoicePrice(Basket basket, BasketItem item, bool useShoppingRule, bool forceKitBundling)
        {
            // NEED THE PRICE OF THIS ITEM AND ALL CHILD ITEMS COMBINED
            LSDecimal price = 0;
            List<int> taxParentIds = new List<int>();
            bool itemizeChildProducts = (!forceKitBundling && item.Product != null && item.Product.Kit.ItemizeDisplay);
            foreach (BasketItem bi in basket.Items)
            {
                if (bi.BasketItemId == item.BasketItemId)
                {
                    price += bi.Price;
                    taxParentIds.Add(bi.BasketItemId);
                }
                else if (!itemizeChildProducts && bi.OrderItemType == OrderItemType.Product && bi.ParentItemId == item.BasketItemId)
                {
                    price += (bi.Price * bi.Quantity) / bi.GetParentItem(false).Quantity;
                    taxParentIds.Add(bi.BasketItemId);
                }
            }

            // CONDITIONAL HANDLING FOR SHOPPING OR INVOICE RULES
            if (useShoppingRule)
            {
                if (TaxHelper.GetEffectiveShoppingDisplay(basket.User) != TaxShoppingDisplay.Included) return price;
            }
            else
            {
                if (TaxHelper.GetEffectiveInvoiceDisplay(basket.User) != TaxInvoiceDisplay.Included) return price;
            }

            // ADD TAX INTO PRICES
            foreach (BasketItem bi in basket.Items)
            {
                if (bi.OrderItemType == OrderItemType.Tax && taxParentIds.Contains(bi.ParentItemId))
                {
                    price += (bi.ExtendedPrice / item.Quantity);
                    taxParentIds.Add(bi.BasketItemId);
                }
            }
            return price;
        }

        /// <summary>
        /// Gets the extended price that should be displayed for a basket/checkout page
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <returns>The extended price to display on a basket/checkout page</returns>
        public static LSDecimal GetInvoiceExtendedPrice(Basket basket, BasketItem item)
        {
            return GetInvoiceExtendedPrice(basket, item, false);
        }

        /// <summary>
        /// Gets the extended price that should be displayed for a basket/checkout page
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <param name="forceKitBundling">If true, the invoice price for the item always uses bundle style for kitting even</param>
        /// <returns>The extended price to display on a basket/checkout page</returns>
        public static LSDecimal GetInvoiceExtendedPrice(Basket basket, BasketItem item, bool forceKitBundling)
        {
            return GetInvoiceExtendedPrice(basket, item, false, forceKitBundling);
        }

        /// <summary>
        /// Gets the extended price that should be displayed for a basket/checkout page
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="item">item to calculate price for</param>
        /// <param name="useShoppingRule">if true, use shopping rules to determine if tax is included; if false use invoice rules</param>
        /// <param name="forceKitBundling">If true, the invoice price for the item always uses bundle style for kitting even</param>
        /// <returns>The extended price to display on a basket/checkout page</returns>
        private static LSDecimal GetInvoiceExtendedPrice(Basket basket, BasketItem item, bool useShoppingRule, bool forceKitBundling)
        {
            // NEED THE PRICE OF THIS ITEM AND ALL CHILD ITEMS COMBINED
            LSDecimal price = 0;
            List<int> taxParentIds = new List<int>();
            bool itemizeChildProducts = (!forceKitBundling && item.Product != null && item.Product.Kit.ItemizeDisplay);
            foreach (BasketItem bi in basket.Items)
            {
                if (bi.BasketItemId == item.BasketItemId
                    || (!itemizeChildProducts && bi.OrderItemType == OrderItemType.Product && bi.ParentItemId == item.BasketItemId))
                {
                    price += bi.ExtendedPrice;
                    taxParentIds.Add(bi.BasketItemId);
                }
            }

            // CONDITIONAL HANDLING FOR SHOPPING OR INVOICE RULES
            if (useShoppingRule)
            {
                if (TaxHelper.GetEffectiveShoppingDisplay(basket.User) != TaxShoppingDisplay.Included) return price;
            }
            else
            {
                if (TaxHelper.GetEffectiveInvoiceDisplay(basket.User) != TaxInvoiceDisplay.Included) return price;
            }

            // ADD TAX INTO PRICES
            foreach (BasketItem bi in basket.Items)
            {
                if (bi.OrderItemType == OrderItemType.Tax && taxParentIds.Contains(bi.ParentItemId))
                {
                    price += bi.ExtendedPrice;
                    taxParentIds.Add(bi.BasketItemId);
                }
            }
            return price;

        }

        /// <summary>
        /// Gets the price that should be displayed for an invoice page
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="item">item to calculate price for</param>
        /// <returns>The price to display on an invoice page</returns>
        public static LSDecimal GetInvoicePrice(Order order, OrderItem item)
        {
            // NEED THE PRICE OF THIS ITEM AND ALL CHILD ITEMS COMBINED
            LSDecimal price = item.Price;
            List<int> taxParentIds = new List<int>();
            taxParentIds.Add(item.OrderItemId);

            // IF THIS IS NOT A CHILD ITEM AND WE ARE NOT ITEMIZING CHILD PRODUCTS, 
            // ADD PRICE OF CHILD ITEMS AS WELL
            if (!item.IsChildItem && !item.ItemizeChildProducts)
            {
                foreach (OrderItem oi in order.Items)
                {
                    if (oi.ParentItemId == item.OrderItemId
                        && oi.IsChildItem
                        && oi.OrderItemType == OrderItemType.Product
                        && oi.OrderShipmentId == item.OrderShipmentId)
                    {
                        price += oi.Price;
                        taxParentIds.Add(oi.OrderItemId);
                    }
                }
            }

            // RETURN PRICE IF WE DO NOT NEED TO INCLUDE TAXES
            TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
            if (displayMode != TaxInvoiceDisplay.Included 
                && displayMode != TaxInvoiceDisplay.IncludedRegistered) return price;

            // ADD TAX INTO PRICES
            foreach (OrderItem oi in order.Items)
            {
                if (oi.OrderItemType == OrderItemType.Tax && taxParentIds.Contains(oi.ParentItemId))
                {
                    price += (oi.ExtendedPrice / item.Quantity);
                    taxParentIds.Add(oi.OrderItemId);
                }
            }
            return price;
        }

        /// <summary>
        /// Gets the extended price that should be displayed for an invoice page
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="item">item to calculate price for</param>
        /// <returns>The extended price to display on an invoice page</returns>
        public static LSDecimal GetInvoiceExtendedPrice(Order order, OrderItem item)
        {
            // NEED THE PRICE OF THIS ITEM
            LSDecimal price = item.ExtendedPrice;
            List<int> taxParentIds = new List<int>();
            taxParentIds.Add(item.OrderItemId);

            // IF THIS IS NOT A CHILD ITEM AND WE ARE NOT ITEMIZING CHILD PRODUCTS, 
            // ADD PRICE OF CHILD ITEMS AS WELL
            if (!item.IsChildItem && !item.ItemizeChildProducts)
            {
                foreach (OrderItem oi in order.Items)
                {
                    if (oi.ParentItemId == item.OrderItemId
                        && oi.IsChildItem
                        && oi.OrderItemType == OrderItemType.Product
                        && oi.OrderShipmentId == item.OrderShipmentId)
                    {
                        price += oi.ExtendedPrice;
                        taxParentIds.Add(oi.OrderItemId);
                    }
                }
            }

            // RETURN PRICE IF WE DO NOT NEED TO INCLUDE TAXES
            TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
            if (displayMode != TaxInvoiceDisplay.Included
                && displayMode != TaxInvoiceDisplay.IncludedRegistered) return price;

            // ADD TAX INTO PRICES
            foreach (OrderItem oi in order.Items)
            {
                if (oi.OrderItemType == OrderItemType.Tax && taxParentIds.Contains(oi.ParentItemId))
                {
                    price += oi.ExtendedPrice;
                    taxParentIds.Add(oi.OrderItemId);
                }
            }
            return price;
        }

        /// <summary>
        /// Gets a price inclusive of tax
        /// </summary>
        /// <param name="price">The price calculated for the product before tax
        /// rules are taken into account</param>
        /// <param name="taxCodeId">The tax code associated with the price</param>
        /// <param name="billingAddress">Billing address for the item, can be null for current user billing address</param>
        /// <param name="shippingAddress">Shipping address for the item, can be null to use current user billing address as shipping address</param>
        /// <returns>The price including tax</returns>
        public static LSDecimal GetPriceWithTax(LSDecimal price, int taxCodeId, TaxAddress billingAddress, TaxAddress shippingAddress)
        {
            MakerShopTax provider = GetTaxProvider();
            if ((taxCodeId == 0) || (price == 0) || (!provider.Enabled)) return price;
            // WE NEED TO RETURN THE PRICE INCLUDING TAX
            // IF MERCHANT PRICES INCLUDE TAX, DO NOTHING
            if (provider.PriceIncludesTax) return price;
            // MERCHANT PRICE DOES NOT INCLUDE TAX, WE NEED TO GET PRICE WITH INCLUDED
            TaxInfo info = provider.GetTaxInfo(price, taxCodeId, billingAddress, shippingAddress, Token.Instance.User);
            return info.PriceWithTax;
        }

        /// <summary>
        /// Gets the tax rate that applies to an item in the basket
        /// </summary>
        /// <param name="item">The item to find tax rate for</param>
        /// <returns>The applicable tax rate</returns>
        public static LSDecimal GetTaxRate(BasketItem item)
        {
            return item.TaxRate;
        }

        /// <summary>
        /// Gets the tax rate that was applied to an item in the order
        /// </summary>
        /// <param name="order">The order that contains the item</param>
        /// <param name="item">The item to find tax rate for</param>
        /// <returns>The tax rate</returns>
        public static LSDecimal GetTaxRate(Order order, OrderItem item)
        {
            return item.TaxRate;
        }

        /// <summary>
        /// Rounds a decimal value to a specified precision. A parameter specifies how to round the value.
        /// </summary>
        /// <param name="amount">A decimal number to be rounded.</param>
        /// <param name="decimalPlaces">The number of significant decimal places (precision) in the return value.</param>
        /// <param name="roundingRule">Specification for how to round the decimal number.</param>
        /// <returns>The number nearest amount with a precision equal to decimalPlaces.</returns>
        public static decimal Round(decimal amount, int decimalPlaces, RoundingRule roundingRule)
        {
            switch (roundingRule)
            {
                case RoundingRule.Common:
                    return Math.Round(amount, decimalPlaces, MidpointRounding.AwayFromZero);
                case RoundingRule.RoundToEven:
                    return Math.Round(amount, decimalPlaces, MidpointRounding.ToEven);
                case RoundingRule.AlwaysRoundUp:
                    return TaxHelper.AlwaysRoundUp(amount, decimalPlaces);
                default:
                    // DEFAULT ROUND TO EVEN, THIS CASE WILL NEVER BE CALLED
                    return Math.Round(amount, decimalPlaces);
            }
        }

        /// <summary>
        /// Rounds a decimal value to a specified precision. Will always round upword.
        /// </summary>
        /// <param name="amount">A decimal number to be rounded.</param>
        /// <param name="decimalPlaces">The number of significant decimal places (precision) in the return value.</param>        
        /// <returns>The up-rounded number nearest amount with a precision equal to decimalPlaces.</returns>
        public static decimal AlwaysRoundUp(Decimal amount, int decimalPlaces)
        {
            decimal dbShift = (decimal)Math.Pow(10.0, decimalPlaces);
            return decimal.Ceiling(amount * dbShift) / dbShift;
        }

        /// <summary>
        /// Determines the effective shopping display.
        /// </summary>
        /// <param name="user">The user used to determine registered status.</param>
        /// <returns>The effective shopping display.</returns>
        public static TaxShoppingDisplay GetEffectiveShoppingDisplay(User user)
        {
            TaxShoppingDisplay displayMode = TaxHelper.ShoppingDisplay;
            switch (displayMode)
            {
                case TaxShoppingDisplay.Hide:
                case TaxShoppingDisplay.Included:
                case TaxShoppingDisplay.LineItem:
                    return displayMode;
                case TaxShoppingDisplay.IncludedRegistered:
                    if (user.PrimaryAddress.IsValid) return TaxShoppingDisplay.Included;
                    break;
                case TaxShoppingDisplay.LineItemRegistered:
                    if (user.PrimaryAddress.IsValid) return TaxShoppingDisplay.LineItem;
                    break;
            }
            return TaxShoppingDisplay.Hide;
        }

        /// <summary>
        /// Determines the effective shopping display.
        /// </summary>
        /// <param name="billingAddress">The billing address specified for the user, or null to refer to the current user billing address.</param>
        /// <returns>The effective shopping display.</returns>
        private static TaxShoppingDisplay GetEffectiveShoppingDisplay(TaxAddress billingAddress)
        {
            TaxShoppingDisplay displayMode = TaxHelper.ShoppingDisplay;
            if (displayMode == TaxShoppingDisplay.IncludedRegistered
                || displayMode == TaxShoppingDisplay.LineItemRegistered)
            {
                // BILLING ADDRESS MUST BE SPECIFIED, OR THE CURRENT USER MUST HAVE A VALID BILLING ADDRESS
                if (billingAddress != null || Token.Instance.User.PrimaryAddress.IsValid)
                {
                    if (displayMode == TaxShoppingDisplay.IncludedRegistered) return TaxShoppingDisplay.Included;
                    return TaxShoppingDisplay.LineItem;
                }
                return TaxShoppingDisplay.Hide;
            }

            // ANY OTHER DISPLAY MODE, RETURN AS IS
            return displayMode;
        }

        /// <summary>
        /// Determines the effective invoice display.
        /// </summary>
        /// <param name="user">The user used to determine registered status.</param>
        /// <returns>The effective invoice display.</returns>
        public static TaxInvoiceDisplay GetEffectiveInvoiceDisplay(User user)
        {
            TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
            switch (displayMode)
            {
                case TaxInvoiceDisplay.Summary:
                case TaxInvoiceDisplay.Included:
                case TaxInvoiceDisplay.LineItem:
                    return displayMode;
                case TaxInvoiceDisplay.IncludedRegistered:
                    if (user.PrimaryAddress.IsValid) return TaxInvoiceDisplay.Included;
                    break;
                case TaxInvoiceDisplay.LineItemRegistered:
                    if (user.PrimaryAddress.IsValid) return TaxInvoiceDisplay.LineItem;
                    break;
            }
            return TaxInvoiceDisplay.Summary;
        }

        /// <summary>
        /// Determines if any tax provider is active for the current store
        /// </summary>
        /// <returns>True if at least one tax provider is active; false otherwise.</returns>
        public static bool IsATaxProviderEnabled()
        {
            TaxGatewayCollection taxGateways = Token.Instance.Store.TaxGateways;
            foreach (TaxGateway taxGateway in taxGateways)
            {
                ITaxProvider provider = taxGateway.GetProviderInstance();
                if (provider != null && provider.Activated) return true;
            }
            return false;
        }
    }
}