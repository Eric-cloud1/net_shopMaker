using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
using MakerShop.Products;
using MakerShop.Utility;
using MakerShop.Common;
using MakerShop.Catalog;

namespace MakerShop.DataClient.Api.ObjectHandlers
{
    class VariantsCsvImporter
    {
        internal static byte[] Import(MakerShop.DataClient.Api.Schema.ProductVariantsImportRequest request)
        {
            ProductVariantsImportResponse response = new ProductVariantsImportResponse();
            response.ResponseId = request.RequestId;
            response.Log = new Log();
            StringBuilder logBuilder = new StringBuilder();
            StringBuilder errorLogBuilder = new StringBuilder();
            StringBuilder warnLogBuilder = new StringBuilder();
            if (request.Products != null && request.Products.Length > 0)
            {
                logBuilder.AppendLine("Starting import, found " + request.Products.Length + " products to import.");
                foreach (CsvProduct csvProduct in request.Products)
                {
                    try
                    {
                        MakerShop.Products.Product product = null;
                        if (csvProduct.ProductId <= 0)
                        {
                            // ZERO OR NEGITIVE ID's INDICATE A NEW PRODUCT
                            product = new MakerShop.Products.Product();
                            csvProduct.ProductId = 0;
                        }
                        else
                        {
                            // Try load existing product
                            product = ProductDataSource.Load(csvProduct.ProductId);
                        }
                        if (product != null)
                        {
                            if (request.CsvFields.Contains("ProductName")) product.Name = csvProduct.Name;
                            if (request.CsvFields.Contains("ProductSku")) product.Sku = csvProduct.Sku;
                            if (request.CsvFields.Contains("ProductPrice")) product.Price = csvProduct.Price;
                            if (request.CsvFields.Contains("ProductMSRP")) product.MSRP = csvProduct.MSRP;
                            if (request.CsvFields.Contains("ProductCostOfGoods")) product.CostOfGoods = csvProduct.CostOfGoods;
                            if (request.CsvFields.Contains("ProductInventoryMode")) product.InventoryMode = (InventoryMode)AlwaysConvert.ToEnum(typeof(InventoryMode), csvProduct.InventoryMode, InventoryMode.None);
                            if (request.CsvFields.Contains("ProductInStock")) product.InStock = csvProduct.InStock;
                            if (request.CsvFields.Contains("ProductInStockWarningLevel")) product.InStockWarningLevel = csvProduct.InStockWarningLevel;
                            if (request.CsvFields.Contains("ProductAllowBackorder")) product.AllowBackorder = csvProduct.AllowBackorder;

                            product.Save();

                            // CATEGORIES
                            if (request.CsvFields.Contains("ProductCategories"))
                            {
                                if (csvProduct.Categories != null && csvProduct.Categories.Length > 0)
                                {
                                    foreach (String prodCat in csvProduct.Categories)
                                    {
                                        if (!String.IsNullOrEmpty(prodCat))
                                        {
                                            MakerShop.Catalog.Category category = CategoryDataSource.LoadForPath(prodCat, ":", true);
                                            if (category != null)
                                            {
                                                if(!product.Categories.Contains(category.CategoryId)) product.Categories.Add(category.CategoryId);
                                            }
                                        }                                        
                                    }
                                    product.Categories.Save();
                                }
                            }

                            // IMPORT OPTIONS
                            if (!String.IsNullOrEmpty(csvProduct.Options))
                            {
                                String[] csvOptions = csvProduct.Options.Split('|');
                                OptionCollection options = OptionDataSource.LoadForProduct(product.ProductId);
                                List<MakerShop.Products.Option> matchingOptions = new List<MakerShop.Products.Option>();
                                List<MakerShop.Products.Option> existingUnmatchedOptions = new List<MakerShop.Products.Option>();
                                List<String> csvNewOptions = new List<string>();
                                foreach (MakerShop.Products.Option option in options)
                                {
                                    // INITIALLY ADD ALL EXISTING OPTIONS AS UNMATCHED
                                    existingUnmatchedOptions.Add(option);
                                }
                                foreach (String option in csvOptions)
                                {
                                    // INITIALLY ADD ALL CSV OPTIONS AS NEW OPTIONS
                                    csvNewOptions.Add(option);
                                }

                                bool choicesAltered = false; // WILL INDICATE IF SOME CHOICES ARE ADDED/REMOVED
                                bool optionsAltered = false; // WILL INDICATE IF SOME OPTIONS ARE ADDED/REMOVED
                                foreach (String csvOption in csvOptions)
                                {
                                    string[] optionParts = csvOption.Split(':');
                                    String csvOptionName = optionParts[0];
                                    String[] csvChoices = optionParts[1].Split(',');

                                    foreach (MakerShop.Products.Option option in options)
                                    {
                                        if (option.Name == csvOptionName)
                                        {
                                            matchingOptions.Add(option);
                                            if (existingUnmatchedOptions.Contains(option)) existingUnmatchedOptions.Remove(option);
                                            if (csvNewOptions.Contains(csvOption)) csvNewOptions.Remove(csvOption);

                                            // FOR MATCHING OPTIONS MATCH THE CHOICES
                                            List<MakerShop.Products.OptionChoice> existingUnmatchedChoices = new List<MakerShop.Products.OptionChoice>();
                                            List<String> csvNewChoices = new List<string>();
                                            foreach (MakerShop.Products.OptionChoice choice in option.Choices)
                                            {
                                                // INITIALLY ADD ALL EXISTING CHOICES AS UNMATCHED
                                                existingUnmatchedChoices.Add(choice);
                                            }
                                            foreach (String csvChoice in csvChoices)
                                            {
                                                // INITIALLY ADD ALL CHOICES AS NEW CHOICES
                                                csvNewChoices.Add(csvChoice);
                                                foreach (MakerShop.Products.OptionChoice choice in option.Choices)
                                                {
                                                    if (csvChoice == choice.Name)
                                                    {
                                                        if (existingUnmatchedChoices.Contains(choice)) existingUnmatchedChoices.Remove(choice);
                                                        if (csvNewChoices.Contains(csvChoice)) csvNewChoices.Remove(csvChoice);
                                                    }
                                                }
                                            }

                                            choicesAltered = false;
                                            // DELETE UN-MATCHED EXISTING CHOICES
                                            foreach (MakerShop.Products.OptionChoice choice in existingUnmatchedChoices)
                                            {
                                                option.Choices.Remove(choice);
                                                choice.Delete();
                                                choicesAltered = true;
                                                logBuilder.AppendLine("Choice deleted, Choice:" + choice.Name + ", Option: " + option.Name + ", ProductId:" + product.ProductId + ", Product Name:" + product.Name);
                                            }

                                            // ADD NEW CHOICES SPECIFIED IN CSV
                                            foreach (String csvChoice in csvNewChoices)
                                            {
                                                MakerShop.Products.OptionChoice newChoice = new MakerShop.Products.OptionChoice();
                                                newChoice.Name = csvChoice;
                                                newChoice.OptionId = option.OptionId;
                                                newChoice.Save();
                                                option.Choices.Add(newChoice);
                                                choicesAltered = true;
                                                logBuilder.AppendLine("New choice added, Choice:" + csvChoice + ", Option: " + option.Name + ", ProductId:" + product.ProductId + ", Product Name:" + product.Name);
                                            }

                                            if (choicesAltered)
                                            {
                                                option.Choices.Save();
                                                ProductVariantManager.ScrubVariantGrid(product.ProductId, option.OptionId);
                                            }
                                        }
                                    }
                                }

                                optionsAltered = false;
                                // REMOVE UN-MATCHED EXISTING OPTIONS
                                foreach (MakerShop.Products.Option option in existingUnmatchedOptions)
                                {
                                    options.Remove(option);
                                    option.Delete();
                                    optionsAltered = true;
                                    logBuilder.AppendLine("Option deleted, Option:" + option.Name + ", ProductId:" + product.ProductId + ", Product Name:" + product.Name);
                                }

                                // ADD NEW OPTIONS AS SPECIFIED IN CSV
                                foreach (String csvOption in csvNewOptions)
                                {
                                    string[] optionParts = csvOption.Split(':');
                                    String csvOptionName = optionParts[0];
                                    String[] csvChoices = optionParts[1].Split(',');

                                    MakerShop.Products.Option option = new MakerShop.Products.Option();
                                    option.Name = csvOptionName.Trim();
                                    foreach (string item in csvChoices)
                                    {
                                        string choiceName = item.Trim();
                                        if (choiceName != String.Empty)
                                        {
                                            MakerShop.Products.OptionChoice choice = new MakerShop.Products.OptionChoice();
                                            choice.Name = StringHelper.Truncate(choiceName, 50);
                                            choice.OrderBy = -1;
                                            option.Choices.Add(choice);
                                        }
                                    }
                                    option.ProductOptions.Add(new MakerShop.Products.ProductOption(product.ProductId, 0, -1));
                                    option.Save();
                                    options.Add(option);
                                    optionsAltered = true;
                                    logBuilder.AppendLine("New Option added, Option:(" + csvOption + "), ProductId:" + product.ProductId + ", Product Name:" + product.Name);
                                }

                                if (optionsAltered)
                                {
                                    options.Save();
                                    if (csvProduct.ProductId > 0)
                                    {
                                        ProductVariantManager.ResetVariantGrid(product.ProductId);
                                        warnLogBuilder.AppendLine("One or more options added/deleted, variants grid will reset to default, ProductId:" + product.ProductId + ", Product Name:" + product.Name);
                                    }
                                }
                            }

                            // IMPORT VARIANTS DATA
                            if (csvProduct.Variants != null && csvProduct.Variants.Length > 0)
                            {
                                ProductVariantManager variantManager = new ProductVariantManager(product.ProductId);
                                PersistentCollection<MakerShop.Products.ProductVariant> variants = variantManager.LoadVariantGrid();

                                // ADD THE VARIANTS IN A DIC, SO THAT THESE CAN BE EASILY ACCESSIBLE BY NAME
                                SortedDictionary<string, MakerShop.Products.ProductVariant> variantsDic = new SortedDictionary<string, MakerShop.Products.ProductVariant>();
                                foreach (MakerShop.Products.ProductVariant variant in variants)
                                {
                                    variantsDic.Add(variant.VariantName, variant);
                                }

                                foreach (CsvVariant csvVariant in csvProduct.Variants)
                                {
                                    if (variantsDic.ContainsKey(csvVariant.VariantName))
                                    {
                                        MakerShop.Products.ProductVariant variant = variantsDic[csvVariant.VariantName];
                                        if (request.CsvFields.Contains("VariantSku")) variant.Sku = csvVariant.Sku;
                                        if (request.CsvFields.Contains("VariantPrice")) variant.Price = csvVariant.Price;
                                        if (request.CsvFields.Contains("VariantPriceMode")) variant.PriceMode = (ModifierMode)AlwaysConvert.ToEnum(typeof(ModifierMode), csvVariant.PriceMode, ModifierMode.Modify);
                                        if (request.CsvFields.Contains("VariantWeight")) variant.Weight = csvVariant.Weight;
                                        if (request.CsvFields.Contains("VariantWeightMode")) variant.WeightMode = (ModifierMode)AlwaysConvert.ToEnum(typeof(ModifierMode), csvVariant.WeightMode, ModifierMode.Modify);
                                        if (request.CsvFields.Contains("VariantCostOfGoods")) variant.CostOfGoods = csvVariant.CostOfGoods;
                                        if (request.CsvFields.Contains("VariantInStock")) variant.InStock = csvVariant.InStock;
                                        if (request.CsvFields.Contains("VariantInStockWarningLevel")) variant.InStockWarningLevel = csvVariant.InStockWarningLevel;
                                        if (request.CsvFields.Contains("VariantAvailable")) variant.Available = csvVariant.Available;

                                        variant.Save();
                                    }
                                    else
                                    {
                                        //Log the warning, error that variant not found
                                        errorLogBuilder.AppendLine("Variant not found, invalid variant name specified, VariantName:" + csvVariant.VariantName + " Options:" + csvProduct.Options + " , ProductId:" + product.ProductId + ", Product Name:" + product.Name);
                                    }
                                }
                            }
                            product.Save();
                        }
                        else
                        {
                            errorLogBuilder.AppendLine("Error: no matching product found. Id: " + csvProduct.ProductId + ", Name:" + csvProduct.Name + ", Options:" + csvProduct.Options);
                            if (!String.IsNullOrEmpty(csvProduct.Name)) logBuilder.Append(", Product Name:" + csvProduct.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogBuilder.AppendLine("An error occurred while trying to import data for ProductId: " + csvProduct.ProductId + ", Name" + csvProduct.Name + " Options:" + csvProduct.Options);
                        if (!String.IsNullOrEmpty(csvProduct.Name)) logBuilder.Append(", Product Name:" + csvProduct.Name);
                        errorLogBuilder.AppendLine("Error details: " + ex.Message);
                        Logger.Error("An error occurred while trying to import data for ProductId: " + csvProduct.ProductId + ", Name" + csvProduct.Name + " Options:" + csvProduct.Options,ex);
                    }
                }
                logBuilder.AppendLine("Import completed.");
            }
            else
            {
                warnLogBuilder.AppendLine("No products available to import.");
            }

            response.Log.Message = logBuilder.ToString();
            response.Log.ErrorMessages = errorLogBuilder.ToString();
            response.Log.WarningMessages = warnLogBuilder.ToString();
            byte[] byteRequestXml = EncodeHelper.Serialize(response);
            return byteRequestXml;
        }
    }
}
