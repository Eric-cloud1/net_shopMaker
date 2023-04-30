namespace MakerShop.DigitalDelivery
{
    using System;
    using MakerShop.Common;
    
    /// <summary>
    /// This class implements a PersistentCollection of ProductDigitalGood objects.
    /// </summary>
    public partial class ProductDigitalGoodCollection
    {
        /// <summary>
        /// Gets the index of a specified digital good in this collection
        /// </summary>
        /// <param name="digitalGoodId">Id of a DigitalGood object to find in this collection</param>
        /// <returns>Index of a specified digital good in this collection</returns>
        public int IndexOfValue(Int32 digitalGoodId)
        {
            return IndexOfValue(digitalGoodId, string.Empty);
        }

        /// <summary>
        /// Gets the index of a specified digital good in this collection
        /// </summary>
        /// <param name="digitalGoodId">Id of a DigitalGood object to find in this collection</param>
        /// <param name="optionList">Option list in case the product has options</param>
        /// <returns></returns>
        public int IndexOfValue(Int32 digitalGoodId, string optionList)
        {
            ProductDigitalGood pdg;
            for (int i = 0; i < this.Count; i++)
            {
                pdg = this[i];
                if (pdg.DigitalGoodId == digitalGoodId && (string.IsNullOrEmpty(pdg.OptionList) || pdg.OptionList == optionList))
                    return i;
            }
            return -1;
        }
    }
}
