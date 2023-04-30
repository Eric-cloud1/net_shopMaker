namespace MakerShop.Products
{
    /// <summary>
    /// Enumeration that represents inheritance mode of kit items
    /// </summary>
    public enum InheritanceMode
    {
        /// <summary>
        /// Inherit from base product
        /// </summary>
        Inherit, 

        /// <summary>
        /// Modify the base product value
        /// </summary>
        Modify, 

        /// <summary>
        /// Override the base product value
        /// </summary>
        Override
    }
}
