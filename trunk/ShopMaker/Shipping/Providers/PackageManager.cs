using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Orders;
using MakerShop.Stores;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Shipping.Providers
{
    /// <summary>
    /// Utility class for managing shipping packages
    /// </summary>
    public static class PackageManager
    {
        /// <summary>
        /// Gets a list of packages for the given basket items.
        /// </summary>
        /// <param name="contents">A colletion of basket items in the order</param>
        /// <returns>An array of Package objects for the given items; or null if no packages are available.</returns>        
        public static Package[] GetPackages(BasketItemCollection contents)
        {
            Store store = Token.Instance.Store;
            WeightUnit storeWeightUnit = store.WeightUnit;
            MeasurementUnit storeMeasurementUnit = store.MeasurementUnit;
            Package sharedPackage = new Package(storeWeightUnit, storeMeasurementUnit);
            List<Package> packages = new List<Package>();
            int sharedItems = 0;
            LSDecimal[] dimensions = new LSDecimal[3];
            foreach (BasketItem item in contents)
            {
                if (item.Shippable == Shippable.Yes)
                {
                    LSDecimal extWeight = item.ExtendedWeight;
                    //WE DO NOT WANT TO SET MINIMUM WEIGHT (BUG 5426)
                    //if (extWeight < 0.1M) extWeight = 0.1M;
                    sharedPackage.Weight += extWeight;
                    sharedPackage.RetailValue += item.ExtendedPrice;
                    sharedPackage.Name = item.Name;
                    if (item.Product != null)
                    {
                        //set the properties of shared package to current item
                        dimensions[2] = item.Product.Length;
                        dimensions[1] = item.Product.Width;
                        dimensions[0] = item.Product.Height;
                        Array.Sort(dimensions);
                        sharedPackage.Length = dimensions[2];
                        sharedPackage.Width = dimensions[1];
                        sharedPackage.Height = dimensions[0];
                    }
                    sharedItems++;
                }
                else if (item.Shippable == Shippable.Separately)
                {
                    if (item.Weight > 0)
                    {
                        Package package = new Package(storeWeightUnit, storeMeasurementUnit);
                        package.Weight = item.Weight;
                        //WE DO NOT WANT TO SET MINIMUM WEIGHT (BUG 5426)
                        //if (package.Weight < 0.1M) package.Weight = 0.1M;
                        //package.RetailValue = item.ExtendedPrice;
                        package.RetailValue = item.Price;
                        package.Multiplier = item.Quantity;
                        //ensure that length is the longest package dimension
                        dimensions[2] = item.Product.Length;
                        dimensions[1] = item.Product.Width;
                        dimensions[0] = item.Product.Height;
                        Array.Sort(dimensions);
                        //set dimensions
                        package.Length = dimensions[2];
                        package.Width = dimensions[1];
                        package.Height = dimensions[0];       
                        package.Name = item.Name;
                        packages.Add(package);
                    }
                }
            }
            if (sharedPackage.Weight > 0)
            {
                sharedPackage.Multiplier = 1;
                if (sharedItems > 1)
                {
                    sharedPackage.Name = "Shared Package";
                    sharedPackage.Length = 0;
                    sharedPackage.Width = 0;
                    sharedPackage.Height = 0;
                }
                packages.Add(sharedPackage);
            }

            if (packages.Count == 0) return null;
            return packages.ToArray();
        }

        /// <summary>
        /// Gets a list of packages for the given basket items.
        /// </summary>
        /// <param name="contents">A colletion of basket items in the order</param>
        /// <returns>A list of packages for the items; or null if no packages are available.</returns>
        public static PackageList GetPackageList(BasketItemCollection contents)
        {            
            PackageList packages = new PackageList();
            Package[] pkgs = GetPackages(contents);
            if (pkgs == null) return null;
            packages.AddRange(pkgs);
            return packages;
        }
    }
}
