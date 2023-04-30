using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
using MakerShop.Products;
using MakerShop.Common;
using System.IO;
using MakerShop.Catalog;

namespace MakerShop.DataClient.Api.ObjectHandlers
{    
    public partial class ExportHandler
    {
        //  WILL HANDLE PRODUCT VARIANTS CSV EXPORT
        internal static byte[] ExportProductVariants(ProductVariantsExportRequest request)
        {
            ProductVariantsExportResponse response = new ProductVariantsExportResponse();
            response.ResponseId = request.RequestId;

            //Code for chunks
            // REQUEST FOR ALL PRODUCTS
            if (request.ProductCriteria == null && String.IsNullOrEmpty(request.IdList))
            {
                // if chunkSize > 0, then it means that we will export "Product Id" of products 
                // in order to export data in Chunks
                List<String> productIdList = ProductHandler.GetIdListForStore(true);
                if (request.ChunkSize > 0 && productIdList.Count > request.ChunkSize)
                {
                    response.ProductIdList = String.Join(",", productIdList.ToArray());
                    response.Products = null;
                }
                else
                {
                    ProductCollection productCollection = ProductHandler.GetProductsForIds(String.Join(",", productIdList.ToArray()));
                    response.Products = ExportHandler.GetProductVariants(productCollection);
                }
            }
            //if this is a request for customized products
            else if (request.ProductCriteria != null && String.IsNullOrEmpty(request.IdList))
            {
                // get customized products data
                //ProductCollection productCollection = ProductHandler.GetCustomizedCollection(request.ProductCriteria);
                // if chunksize is > 0 and productCollection.count > chunksize, so export Ids
                List<String> productIdList = ProductHandler.GetIdListForProductCriteria(request.ProductCriteria);
                if (request.ChunkSize > 0 && productIdList.Count > request.ChunkSize)
                {
                    response.ProductIdList = String.Join(",", productIdList.ToArray());
                    response.Products = null;
                }
                // Export products
                else if (request.ChunkSize == 0 || productIdList.Count <= request.ChunkSize)
                {
                    ProductCollection productCollection = ProductHandler.GetCustomizedCollection(request.ProductCriteria);
                    //Get products on the basis of criteria and then assign
                    response.Products = ExportHandler.GetProductVariants(productCollection);
                }
            }
            // if request is to export product for the given Ids so here Criteria will be null nad IdList will contain product Ids to be exported
            else if (request.ProductCriteria == null && !String.IsNullOrEmpty(request.IdList))
            {
                ProductCollection productCollection = ProductHandler.GetProductsForIds(request.IdList);
                response.Products = ExportHandler.GetProductVariants(productCollection);
            }            

            byte[] responseInBytes = EncodeHelper.Serialize(response);
            return responseInBytes;
        }       

        private static CsvProduct[] GetProductVariants(ProductCollection productCollection)
        {
            List<CsvProduct> products = new List<CsvProduct>();

            if (productCollection != null)
            {
                CsvProduct csvProduct = null;
                foreach (MakerShop.Products.Product product in productCollection)
                {
                    csvProduct = new CsvProduct();
                    csvProduct.ProductId = product.ProductId;
                    csvProduct.Name = product.Name;
                    csvProduct.Sku = product.Sku;
                    csvProduct.Price = (decimal)product.Price;
                    csvProduct.MSRP = (decimal)product.MSRP;
                    csvProduct.CostOfGoods = (decimal)product.CostOfGoods;
                    csvProduct.InventoryMode = product.InventoryMode.ToString();
                    csvProduct.InStock = product.InStock;
                    csvProduct.InStockWarningLevel = product.InStockWarningLevel;
                    csvProduct.AllowBackorder = product.AllowBackorder;

                    // Categories
                    String[] arrProductCategory = new String[product.Categories.Count];
                    //ProductCategory objProductCategory = null;
                    List<String> prodCategories = new List<string>();
                    for (int j = 0; j < product.Categories.Count; j++)
                    {
                        int Id = product.Categories[j];
                        //objProductCategory = new ProductCategory();
                        List<CatalogPathNode> path = CatalogDataSource.GetPath(Id, false);
                        //int orderBy = 0;
                        foreach (CatalogPathNode objNode in path)
                        {
                            prodCategories.Add(objNode.Name);
                            //orderBy = objNode.OrderBy;
                        }
                        //objProductCategory.CategoryId = Id;
                        //objProductCategory.OrderBy = orderBy;
                        //objProductCategory.Path = String.Join(":", prodCategories.ToArray());

                        arrProductCategory[j] = String.Join(":", prodCategories.ToArray());
                    }
                    csvProduct.Categories = arrProductCategory;

                    // Options 
                    // The output format will be "option1Name:choic1,choice2|Option2Name:Choice1,choice2,choice3"
                    List<string> options = new List<string>();
                    foreach(MakerShop.Products.ProductOption pOption in  product.ProductOptions)
                    {
                        MakerShop.Products.Option option = pOption.Option;
                        StringBuilder strOption = new StringBuilder();
                        strOption.Append(option.Name).Append(":");                                       
                        for(int i = 0; i < option.Choices.Count; i++)
                        {
                            if(i != 0) strOption.Append(",");
                            strOption.Append(option.Choices[i].Name);
                        }
                        options.Add(strOption.ToString());
                    }
                    csvProduct.Options = String.Join("|", options.ToArray());

                    ProductVariantManager vManager = new ProductVariantManager(product.ProductId);
                    PersistentCollection<MakerShop.Products.ProductVariant> variants = vManager.LoadVariantGrid();
                    if (variants != null)
                    {
                        List<CsvVariant> csvVariants = new List<CsvVariant>();
                        CsvVariant csvVariant = null;
                        foreach (MakerShop.Products.ProductVariant variant in variants)
                        {
                            csvVariant = new CsvVariant();
                            csvVariant.VariantName = variant.VariantName;
                            csvVariant.Sku = variant.Sku;
                            csvVariant.Price = (decimal)variant.Price;
                            csvVariant.PriceMode = variant.PriceMode.ToString();
                            csvVariant.Weight = (decimal)variant.Weight;
                            csvVariant.WeightMode = variant.WeightMode.ToString();
                            csvVariant.CostOfGoods = (decimal)variant.CostOfGoods;
                            csvVariant.InStock = variant.InStock;
                            csvVariant.InStockWarningLevel = variant.InStockWarningLevel;
                            csvVariant.Available = variant.Available;

                            csvVariants.Add(csvVariant);
                        }
                        csvProduct.Variants = csvVariants.ToArray();
                    }
                    products.Add(csvProduct);
                }
            }

            return products.ToArray();
        }
    }
}
