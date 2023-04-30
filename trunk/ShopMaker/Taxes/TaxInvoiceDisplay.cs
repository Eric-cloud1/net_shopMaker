namespace MakerShop.Taxes
{
    /// <summary>
    /// Enumeration that represents the invoice display options for taxes
    /// </summary>
    public enum TaxInvoiceDisplay
    {
        /// <summary>
        /// Show summary tax information only
        /// </summary>
        Summary, 

        /// <summary>
        /// Show taxes included in the line item prices
        /// </summary>
        Included,

        /// <summary>
        /// Show taxes as separate line items
        /// </summary>
        LineItem,

        /// <summary>
        /// Show taxes included in the line item prices for registered customers
        /// </summary>
        IncludedRegistered,

        /// <summary>
        /// Show taxes as separate line items
        /// </summary>
        LineItemRegistered
    }
}
