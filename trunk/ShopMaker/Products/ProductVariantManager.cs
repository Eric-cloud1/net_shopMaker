using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Data.Common;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;
using MakerShop.DigitalDelivery;
using MakerShop.Orders;
using MakerShop.Users;

namespace MakerShop.Products
{
    /// <summary>
    /// Class for product variant management
    /// </summary>
    [DataObject(true)]
    public class ProductVariantManager
    {

        private int _ProductId;
        private OptionCollection _Options;
        private int[] _OptionMultiplier;
        private int _Count;
        
        //THIS TRACKS DEFAULT OPTION CHOICES PAST THE FIRST EIGHT
        //THAT SHOULD BE INCLUDED WITH VARIANTS RETRIEVED FROM THE MANAGER
        List<int> _AdditionalOptions;

        /// <summary>
        /// Product Id
        /// </summary>
        public int ProductId { get { return _ProductId; } }
        
        /// <summary>
        /// Number of variants
        /// </summary>
        public int Count { get { return _Count; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productId">Product Id</param>
        public ProductVariantManager(int productId)
        {
            //OBTAIN AND VALIDATE THE OPTIONS
            _ProductId = productId;
            _Options = OptionDataSource.LoadForProduct(productId, ProductVariant.VARIANT_SORT_EXPRESSION);
            if (_Options.Count > 0)
            {
                //VARIANT MANAGER ONLY SUPPORTS A FIXED NUMBER OF OPTIONS
                if (_Options.Count > ProductVariant.MAXIMUM_ATTRIBUTES)
                {
                    //WE NEED TO KEEP TRACK OF OUR DEFAULT VALUES FOR THE ADDITIONAL OPTIONS
                    //SO THAT WE CAN PRODUCE VALID VARIANT RECORDS FOR THIS PRODUCT
                    _AdditionalOptions = new List<int>();
                    while (_Options.Count > ProductVariant.MAXIMUM_ATTRIBUTES)
                    {
                        Option o = _Options[ProductVariant.MAXIMUM_ATTRIBUTES];
                        //SAVE THE DEFAULT CHOICE
                        if (o.Choices.Count > 0) _AdditionalOptions.Add(o.Choices[0].OptionChoiceId);
                        else
                        {
                            //THIS IS AN ERROR CONDITION
                            try
                            {
                                Product p = ProductDataSource.Load(productId);
                                if (p != null)
                                    Logger.Error("The product " + p.Name + " has an option named " + o.Name + " that has no choices.  You must delete this option or add at least one choice.");
                            }
                            catch { }
                            _AdditionalOptions.Add(0);
                        }
                        _Options.RemoveAt(ProductVariant.MAXIMUM_ATTRIBUTES);
                    }
                }
                //CALCULATE THE VARIANT COUNT
                this.CalculateVariantCount();
                //INITIALIZE THE ATTRIBUTE MULTIPLIER
                this.CalculateOptionMultiplier();
            }
            else
            {
                _Count = 0;
                _OptionMultiplier = null;
            }
        }

        /// <summary>
        /// Determines the index of the variant within the grid
        /// </summary>
        /// <param name="variant">The variant to find in the grid</param>
        /// <returns>The index of the variant, or -1 if not found</returns>
        public int IndexOf(ProductVariant variant)
        {
            if (variant == null) return -1;
            return GetIndexFromOptions(variant.GetOptionChoices(ProductVariant.OptionCountBehavior.ActualCount));
        }

        /// <summary>
        /// Check if the given product has any variant data specified
        /// </summary>
        /// <param name="productId">Id of the product to check variant data for</param>
        /// <returns><b>true</b> if product has variant data, <b>false</b> otherwise</returns>
        public static bool HasVariantData(int productId)
        {
            Database database = Token.Instance.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) FROM ac_ProductVariants WHERE ProductId = @productId");
            sql.Append(" AND (");
            sql.Append("VariantName IS NOT NULL");
            sql.Append(" OR Sku IS NOT NULL");
            sql.Append(" OR Price IS NOT NULL");
            sql.Append(" OR PriceModeId <> 0");
            sql.Append(" OR Weight IS NOT NULL");
            sql.Append(" OR WeightModeId <> 0");
            sql.Append(" OR CostOfGoods IS NOT NULL");
            sql.Append(" OR InStock <> 0");
            sql.Append(" OR InStockWarningLevel <> 0");
            sql.Append(" OR Available = 0)");
            using (DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString()))
            {
                database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                int varCount = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
                return (varCount > 0);
            }
        }

        /// <summary>
        /// Checks if the given product has digital good data associated with one of its variants
        /// </summary>
        /// <param name="productId">Id of the product to check digital good data for</param>
        /// <returns><b>true</b> if product has digital good data associated to a variant, <b>false</b> otherwise</returns>
        public static bool HasDigitalGoodData(int productId)
        {
            Database database = Token.Instance.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) FROM ac_ProductDigitalGoods WHERE ProductId = @productId");
            sql.Append(" AND (OptionList IS NOT NULL) AND (LEN(OptionList) > 0)");
            using (DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString()))
            {
                database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                int dgCount = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
                return (dgCount > 0);
            }
        }

        /// <summary>
        /// Calculates number of variants
        /// </summary>
        private void CalculateVariantCount()
        {
            //THIS METHOD ASSUMES ATTRIBUTE COUNT HAS BEEN VERIFIED
            _Count = 1;
            foreach (Option option in _Options)
            {
                _Count = (_Count * option.Choices.Count);
            }
        }

        private void CalculateOptionMultiplier()
        {
            //THIS METHOD ASSUMES ATTRIBUTE COUNT HAS BEEN VERIFIED
            int optionCount = _Options.Count;
            _OptionMultiplier = new int[optionCount];
            _OptionMultiplier[0] = 1;
            for (int i = 1; i < optionCount; i++)
            {
                //THE MULTIPLIER FOR CURRENT OPTION IS THE MULTIPLIER OF PREVIOUS OPTION TIMES NUMBER OF PREVIOUS OPTION CHOICES
                Option option = _Options[i - 1];
                _OptionMultiplier[i] = option.Choices.Count * _OptionMultiplier[i - 1];
            }
        }

        /// <summary>
        /// Gets a ProductVariant at the given index
        /// </summary>
        /// <param name="index">Index to get the ProductVariant for</param>
        /// <returns>ProductVariant at the given index</returns>
        public ProductVariant GetVariantByIndex(int index)
        {
            //VALIDATE INDEX IS IN RANGE
            if ((index < 0) || (index > _Count - 1)) throw new ArgumentOutOfRangeException("index");
            ProductVariant variant = new ProductVariant();
            //ATTEMPT TO LOAD
            //EVEN IF LOAD IS NOT SUCCESSFUL, VARIANT SHOULD STILL HAVE PRODUCT AND OPTION IDS SET
            variant.Load(this._ProductId, this.GetOptionChoicesByIndex(index));
            return variant;
        }

        /// <summary>
        /// Gets the option choices given the index in the variant matrix
        /// </summary>
        /// <param name="index">Index in the variant matrix to retrive choices for</param>
        /// <returns>Option choices for the given index in the variant matrix</returns>
        /// <remarks>This will return at least eight choices as supported by the database, more
        /// if available for the product.  If the product has less than eight choices, the 
        /// array is padded with zeros.</remarks>
        public int[] GetOptionChoicesByIndex(int index)
        {
            int rowRemainder = index;
            List<int> choices = new List<int>();
            int optionCount = _Options.Count;
            //CALCULATE THE CHOICES IN REVERSE ORDER BASED ON INDEX PROVIDED
            for (int i = ProductVariant.MAXIMUM_ATTRIBUTES - 1; i >= 0; i--)
            {
                if (i >= optionCount)
                {
                    choices.Add(0);
                }
                else
                {
                    Option option = _Options[i];
                    int choiceIndex = (int)Decimal.Truncate(rowRemainder / this._OptionMultiplier[i]);
                    choices.Add(option.Choices[choiceIndex].OptionChoiceId);
                    rowRemainder = (rowRemainder % this._OptionMultiplier[i]);
                }
            }
            //PUT THE CHOICES IN THE CORRECT ORDER
            choices.Reverse();
            //ADD DEFAULT CHOICES FOR OPTIONS PAST THE FIRST EIGHT, IF PRESENT
            if ((_AdditionalOptions != null) && (_AdditionalOptions.Count > 0))
                choices.AddRange(_AdditionalOptions);
            //RETURN THE CHOICE ARRAY
            return choices.ToArray();
        }

        /// <summary>
        /// Gets the index of a variant in the grid given the option choices
        /// </summary>
        /// <param name="optionList">Comma delimited list of option choices</param>
        /// <returns>The index of the variant in the grid, or -1 if option choices are invalid</returns>
        public int GetIndexFromOptions(string optionList)
        {
            int[] optionChoices = AlwaysConvert.ToIntArray(optionList);
            return GetIndexFromOptions(optionChoices);
        }

        /// <summary>
        /// Gets the index of a variant in the grid given the option choices
        /// </summary>
        /// <param name="optionChoiceIds">Array of option choices</param>
        /// <returns>The index of the variant in the grid, or -1 if option choices are invalid</returns>
        /// <remarks>This method only uses the first eight option choices.  Additional choices
        /// are ignored.</remarks>
        public int GetIndexFromOptions(int[] optionChoiceIds)
        {
            if (optionChoiceIds == null || optionChoiceIds.Length == 0) return -1;
            int calculatedIndex = 0;
            int endIndex = Math.Min(ProductVariant.MAXIMUM_ATTRIBUTES, optionChoiceIds.Length);
            for (int i = 0; i < endIndex; i++)
            {
                int optionChoiceId = optionChoiceIds[i];
                if (optionChoiceId > 0)
                {
                    Option option = _Options[i];
                    int choiceIndex = option.Choices.IndexOf(optionChoiceId);
                    if (choiceIndex < 0) return -1;
                    calculatedIndex += (this._OptionMultiplier[i] * choiceIndex);
                }
            }
            return calculatedIndex;
        }

        /// <summary>
        /// Gets a variant given the option choices
        /// </summary>
        /// <param name="optionList">Comma delimited list of option choices</param>
        /// <returns>The variant for the given options, or null if not found</returns>
        public ProductVariant GetVariantFromOptions(string optionList)
        {
            int variantIndex = GetIndexFromOptions(optionList);
            if (variantIndex < 0) return null;
            return GetVariantByIndex(variantIndex);
        }

        /// <summary>
        /// Gets a variant given the option choices
        /// </summary>
        /// <param name="optionChoiceIds">Array of option choices</param>
        /// <returns>The variant for the given options, or null if not found</returns>
        public ProductVariant GetVariantFromOptions(int[] optionChoiceIds)
        {
            int variantIndex = GetIndexFromOptions(optionChoiceIds);
            if (variantIndex < 0) return null;
            return GetVariantByIndex(variantIndex);
        }

        //THIS IS A METHOD RATHER THAN PROPERTY SO IT CAN BE USED FOR PAGING BY OBJECTDATASOURCE
        /// <summary>
        /// Gets the number of variants
        /// </summary>
        /// <returns>Number of variants</returns>
        public int CountVariantGrid()
        {
            return _Count;
        }

        /// <summary>
        /// Loads the product variant collection for associated product
        /// </summary>
        /// <returns>ProductVariant collection for product associated with this product variant manager</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public virtual PersistentCollection<ProductVariant> LoadVariantGrid()
        {
            return this.LoadVariantGrid(0, 0);
        }

        /// <summary>
        /// Loads the product variant collection for associated product
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>ProductVariant collection for product associated with this product variant manager</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public virtual PersistentCollection<ProductVariant> LoadVariantGrid(int maximumRows, int startRowIndex)
        {
            //CREATE COLLECTION TO HOLD GENERATED VARIANTS
            PersistentCollection<ProductVariant> variantCollection = new PersistentCollection<ProductVariant>();
            //CALCULATE START/END POSITION AND LOOP
            if (startRowIndex > _Count) throw new ArgumentOutOfRangeException("startRowIndex");
            int endCount = startRowIndex + maximumRows;
            if ((maximumRows < 1) || (endCount > _Count)) endCount = _Count;
            for (int i = startRowIndex; i < endCount; i++)
            {
                //CREATE/LOAD THE VARIANT OBJECT FOR THIS INDEX
                variantCollection.Add(this.GetVariantByIndex(i));
            }
            //RETURN THE CALCULATED VARIANT GRID
            return variantCollection;
        }

        /// <summary>
        /// Loads the available product variants for product associated with this product variant manager
        /// </summary>
        /// <returns>Collection of available product variants for product associated with this product variant manager</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public virtual PersistentCollection<ProductVariant> LoadAvailableVariantGrid()
        {
            return this.LoadAvailableVariantGrid(0, 0);
        }

        /// <summary>
        /// Loads the available product variants for product associated with this product variant manager
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>Collection of available product variants for product associated with this product variant manager</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public virtual PersistentCollection<ProductVariant> LoadAvailableVariantGrid(int maximumRows, int startRowIndex)
        {
            //CREATE COLLECTION TO HOLD GENERATED VARIANTS
            PersistentCollection<ProductVariant> variantCollection = new PersistentCollection<ProductVariant>();
            //CALCULATE START/END POSITION AND LOOP
            if (startRowIndex > _Count) throw new ArgumentOutOfRangeException("startRowIndex");
            int endCount = startRowIndex + maximumRows;
            if ((maximumRows < 1) || (endCount > _Count)) endCount = _Count;
            for (int i = startRowIndex; i < endCount; i++)
            {
                //CREATE/LOAD THE VARIANT OBJECT FOR THIS INDEX
                ProductVariant varnt = this.GetVariantByIndex(i);
                if (varnt.Available)
                {
                    variantCollection.Add(varnt);
                }
            }
            //RETURN THE CALCULATED VARIANT GRID
            return variantCollection;
        }


        //STATIC METHOD TO WIPE THE VARIANT GRID FOR A PRODUCT
        /// <summary>
        /// Resets the variant grid for a product
        /// </summary>
        /// <param name="productId">Id of the product to reset the variant grid for</param>
        public static void ResetVariantGrid(int productId)
        {
            Database database = Token.Instance.Database;
            //DELETE ANY KIT PRODUCTS ASSOCIATED WITH THIS VARIANT
            database.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM ac_KitProducts WHERE ProductId = " + productId.ToString());
            //DELETE BASKET ITEM ASSOCIATED WITH THE VARIANT
            database.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM ac_BasketItems WHERE ProductId = " + productId.ToString());
            //DELETE WISHLIST ITEM ASSOCIATED WITH THE VARIANT
            database.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM ac_WishlistItems WHERE ProductId = " + productId.ToString());
            //DELETE THE PRODUCT VARIANT

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_ProductVariants WHERE ProductId = @productId"))
            {
                database.AddInParameter(deleteCommand, "@productId", System.Data.DbType.Int32, productId);
                database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
        }

        /// <summary>
        /// Checks records currently saved to the variant grid to determine if they are valid
        /// </summary>
        /// <param name="productId"></param>
        public static void ScrubVariantGrid(int productId)
        {
            Product product = ProductDataSource.Load(productId);
            if (product != null)
            {
                Database database = Token.Instance.Database;
                //GET ALL THE VARIANTS FROM THE GRID
                System.Data.DataSet variants = database.ExecuteDataSet(System.Data.CommandType.Text, "SELECT ProductVariantId,Option1,Option2,Option3,Option4,Option5,Option6,Option7,Option8 FROM ac_ProductVariants WHERE ProductId = " + productId);
                foreach (System.Data.DataRow variantRow in variants.Tables[0].Rows)
                {
                    if (!IsVariantValid(product, (int)variantRow[1], (int)variantRow[2], (int)variantRow[3], (int)variantRow[4], (int)variantRow[5], (int)variantRow[6], (int)variantRow[7], (int)variantRow[8]))
                    {
                        database.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM ac_ProductVariants WHERE ProductId = " + productId + " AND ProductVariantId = " + variantRow[0].ToString());
                    }
                }
            }
            else throw new InvalidProductException("Invalid product id specified.");
        }

        /// <summary>
        /// Removes records from the variant grid linked to the given invalid choice ID
        /// </summary>
        /// <param name="productId">Product for which the variant grid is being scrubbed</param>
        /// <param name="invalidOptionChoiceId">The ID of a known invalid choice, for instance due to a delete operation</param>
        public static void ScrubVariantGrid(int productId, int invalidOptionChoiceId)
        {
            Product product = ProductDataSource.Load(productId);
            if (product != null)
            {
                Database database = Token.Instance.Database;
                string sql = "DELETE FROM ac_ProductVariants WHERE ProductId=@productId AND (Option1=@invalidChoice OR Option2=@invalidChoice OR Option3=@invalidChoice OR Option4=@invalidChoice OR Option5=@invalidChoice OR Option6=@invalidChoice OR Option7=@invalidChoice OR Option8=@invalidChoice)";

                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                using (DbCommand deleteCommand = database.GetSqlStringCommand(sql))
                {
                    database.AddInParameter(deleteCommand, "@productId", System.Data.DbType.Int32, productId);
                    database.AddInParameter(deleteCommand, "@invalidChoice", System.Data.DbType.Int32, invalidOptionChoiceId);
                    database.ExecuteNonQuery(deleteCommand);
                }

                MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
                // DELETE INVALID PRODUCT DIGITAL GOOD ASSOCIATIONS
                sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT ProductDigitalGoodId, OptionList FROM ac_ProductDigitalGoods WHERE ProductId = @productId AND (OptionList IS NOT NULL) AND (LEN(OptionList) > 0)";
                using (DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString()))
                {
                    database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                    using (IDataReader dr = database.ExecuteReader(selectCommand))
                    {
                        while (dr.Read())
                        {
                            
                            String optionList = dr.GetString(1);
                            if (OptionListContainsChoice(optionList, invalidOptionChoiceId))
                            {
                                int productDigitalGoodId = dr.GetInt32(0);
                                ProductDigitalGoodDataSource.Delete(productDigitalGoodId);
                            }
                        }
                        dr.Close();
                    }
                }

                // DELETE INVALID BASKET ITEMS ASSOCIATIONS                
                sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT BasketItemId, OptionList FROM ac_BasketItems WHERE ProductId = @productId AND (OptionList IS NOT NULL) AND (LEN(OptionList) > 0)";
                using (DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString()))
                {
                    database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                    using (IDataReader dr = database.ExecuteReader(selectCommand))
                    {
                        while (dr.Read())
                        {

                            String optionList = dr.GetString(1);
                            if (OptionListContainsChoice(optionList, invalidOptionChoiceId))
                            {
                                int basketItemId = dr.GetInt32(0);
                                BasketItemDataSource.Delete(basketItemId);
                            }
                        }
                        dr.Close();
                    }
                }

                // DELETE INVALID WISHLIST ITEMS ASSOCIATIONS                
                sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT WishlistItemId, OptionList FROM ac_WishlistItems WHERE ProductId = @productId AND (OptionList IS NOT NULL) AND (LEN(OptionList) > 0)";
                using (DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString()))
                {
                    database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                    using (IDataReader dr = database.ExecuteReader(selectCommand))
                    {
                        while (dr.Read())
                        {

                            String optionList = dr.GetString(1);
                            if (OptionListContainsChoice(optionList, invalidOptionChoiceId))
                            {
                                int ishlistItemId = dr.GetInt32(0);
                                WishlistItemDataSource.Delete(ishlistItemId);
                            }
                        }
                        dr.Close();
                    }
                }

                // DELETE INVALID PRODUCT KIT ASSOCIATIONS
                if (product.KitStatus == KitStatus.Member)
                {
                    sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT KitProductId, OptionList FROM ac_KitProducts WHERE ProductId = @productId AND (OptionList IS NOT NULL) AND (LEN(OptionList) > 0)";
                    using (DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                        using (IDataReader dr = database.ExecuteReader(selectCommand))
                        {
                            while (dr.Read())
                            {

                                String optionList = dr.GetString(1);
                                if (OptionListContainsChoice(optionList, invalidOptionChoiceId))
                                {
                                    int kitProductId = dr.GetInt32(0);
                                    KitProductDataSource.Delete(kitProductId);
                                }
                            }
                            dr.Close();
                        }
                    }
                }
            }
            else throw new InvalidProductException("Invalid product id specified.");
        }
        
        private static bool OptionListContainsChoice(String optionList, int optionChoiceId)
        {
            if (!String.IsNullOrEmpty(optionList))
            {
                String[] choices = optionList.Split(',');
                return Array.IndexOf(choices, optionChoiceId.ToString()) > -1;
            }
            return false;
        }

        private static bool IsVariantValid(Product product, int option1, int option2, int option3, int option4, int option5, int option6, int option7, int option8)
        {
            //TODO: IMPROVE THE PERFORMANCE OF THIS METHOD
            //CREATE A QUERY THAT CAN DETERMINE IF AN OPTION CHOICE IS VALID
            //WITHOUT HAVING TO LOAD THE WHOLE OPION CHOICE COLLECTION
            int optionCount = product.ProductOptions.Count;

            if ((optionCount > 0) && (option1 == 0)) return false;
            else if (optionCount == 0) return true;
            if (product.ProductOptions[0].Option.Choices.IndexOf(option1) < 0) return false;

            if ((optionCount > 1) && (option2 == 0)) return false;
            else if (optionCount == 1) return true;
            if (product.ProductOptions[1].Option.Choices.IndexOf(option2) < 0) return false;

            if ((optionCount > 2) && (option3 == 0)) return false;
            else if (optionCount == 2) return true;
            if (product.ProductOptions[2].Option.Choices.IndexOf(option3) < 0) return false;

            if ((optionCount > 3) && (option4 == 0)) return false;
            else if (optionCount == 3) return true;
            if (product.ProductOptions[3].Option.Choices.IndexOf(option4) < 0) return false;

            if ((optionCount > 4) && (option5 == 0)) return false;
            else if (optionCount == 4) return true;
            if (product.ProductOptions[4].Option.Choices.IndexOf(option5) < 0) return false;

            if ((optionCount > 5) && (option6 == 0)) return false;
            else if (optionCount == 5) return true;
            if (product.ProductOptions[5].Option.Choices.IndexOf(option6) < 0) return false;

            if ((optionCount > 6) && (option7 == 0)) return false;
            else if (optionCount == 6) return true;
            if (product.ProductOptions[6].Option.Choices.IndexOf(option7) < 0) return false;

            if ((optionCount > 7) && (option8 == 0)) return false;
            else if (optionCount == 7) return true;
            if (product.ProductOptions[7].Option.Choices.IndexOf(option8) < 0) return false;

            return true;
        }


        /// <summary>
        /// Converts a dictionary of attribute / option pairs into an array of int with the option choices in matrix order.
        /// </summary>
        /// <param name="productId">ID of product</param>
        /// <param name="optionChoices">Dictionary of option, option choice ID relationships</param>
        /// <returns>An array of int with the option choices in matrix order</returns>
        public static int[] GetSortedOptionChoices(int productId, Dictionary<int, int> optionChoices)
        {
            if ((optionChoices == null) || (optionChoices.Count == 0)) return null;
            //FIRST, GET ATTRIBUTES IN ORDER OF CREATED DATE
            OptionCollection options = OptionDataSource.LoadForProduct(productId, ProductVariant.VARIANT_SORT_EXPRESSION);
            //YOU HAVE TO PASS IN THE SAME NUMBER OF ATTRIBUTES AS DEFINED
            if (optionChoices.Count != options.Count) throw new InvalidOptionsException();
            //ADD THE PASSED OPTIONS INTO THE ARRAY IN MATRIX ORDER
            int order = 0;
            int[] sortedOptions = new int[options.Count];
            foreach (Option option in options)
            {
                sortedOptions[order] = optionChoices[option.OptionId];
                order++;
            }
            //CONVERT THE SORTED ARRAY TO LIST AND RETURN
            return sortedOptions;
        }
    }
}