using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MakerShop.DigitalDelivery;
using MakerShop.Products;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    /// <summary>
    /// Collection of LicenseAgreement objects for a BasketItem
    /// </summary>
    public class BasketItemLicenseAgreementCollection : Collection<LicenseAgreement>
    {
        private BasketItem _BasketItem;
        /// <summary>
        /// BasketItem this collection is for
        /// </summary>
        public BasketItem BasketItem { get { return _BasketItem; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basketItem">BasketItem object for which to create the collection</param>
        /// <param name="displayMode">Display mode of the license agreement</param>
        public BasketItemLicenseAgreementCollection(BasketItem basketItem, LicenseAgreementMode displayMode)
        {
            this._BasketItem = basketItem;
            BuildCollection(displayMode);
        }

        private void BuildCollection(LicenseAgreementMode displayMode)
        {
            List<int> agreementTracker = new List<int>();
            List<ProductAndOptionList> products = new List<ProductAndOptionList>();

            // BUILD A LIST OF PRODUCTS (WITH OPTION DATA) THAT WE MUST INCLUDE AGREEMENTS FOR
            products.Add(new ProductAndOptionList(this.BasketItem.Product, this.BasketItem.OptionList));

            // IF IT IS A KIT LOOK FOR CHILD PRODUCTS AS WELL
            if (this.BasketItem.Product.KitStatus == MakerShop.Products.KitStatus.Master)
            {
                int[] kitProductIds = AlwaysConvert.ToIntArray(this.BasketItem.KitList);
                if (kitProductIds != null && kitProductIds.Length > 0)
                {
                    foreach (int kitProductId in kitProductIds)
                    {
                        KitProduct kitProduct = KitProductDataSource.Load(kitProductId);
                        if (kitProduct != null && kitProduct.Product != null)
                        {
                            products.Add(new ProductAndOptionList(kitProduct.Product, kitProduct.OptionList));
                        }
                    }
                }
            }

            foreach (ProductAndOptionList pol in products)
            {
                foreach (ProductDigitalGood pdg in pol.Product.DigitalGoods)
                {
                    if ((string.IsNullOrEmpty(pdg.OptionList)) || (pdg.OptionList == pol.OptionList))
                    {
                        DigitalGood dg = pdg.DigitalGood;
                        if ((dg != null) && ((dg.LicenseAgreementMode & displayMode) == displayMode))
                        {
                            LicenseAgreement la = dg.LicenseAgreement;
                            if ((la != null) && (agreementTracker.IndexOf(la.LicenseAgreementId) < 0))
                            {
                                this.Add(la);
                                agreementTracker.Add(la.LicenseAgreementId);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets an array of LicenseAgreement Ids in this collection
        /// </summary>
        /// <returns>An array of LicenseAgreement Ids in this collection</returns>
        public int[] GetLicenseAgreementIds()
        {
            List<int> agreementIds = new List<int>();
            foreach (LicenseAgreement la in this)
            {
                agreementIds.Add(la.LicenseAgreementId);
            }
            return agreementIds.ToArray();
        }

        private struct ProductAndOptionList
        {
            public Product Product;
            public string OptionList;
            public ProductAndOptionList(Product product, string optionList)
            {
                this.Product = product;
                this.OptionList = optionList;
            }
        }
    }
}
