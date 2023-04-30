using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;
using MakerShop.Exceptions;

namespace MakerShop.Products
{
    /// <summary>
    /// DataSource class for ProductVariant type objects
    /// </summary>
    [DataObject(true)]
    public partial class ProductVariantDataSource
    {
        /// <summary>
        /// Gets the product variant Id for the given product with given option choices
        /// </summary>
        /// <param name="productId">Id of the product for which to get the variant</param>
        /// <param name="optionChoices">Selected option choices of the variant</param>
        /// <param name="ignoreInvalidOptions">If true invalid options are ignored</param>
        /// <returns>Id of the ProductVariant</returns>
        public static int GetProductVariantId(int productId, Dictionary<int,int> optionChoices, bool ignoreInvalidOptions)
        {
            if ((optionChoices == null) || (optionChoices.Count == 0)) return 0;
            int[] optionChoiceArray;
            try
            {
                optionChoiceArray = ProductVariantManager.GetSortedOptionChoices(productId, optionChoices);
            }
            catch (InvalidOptionsException ipaex)
            {
                if (ignoreInvalidOptions) return 0;
                throw ipaex;
            }
            return ProductVariantDataSource.GetProductVariantId(productId, optionChoiceArray);
        }

        /// <summary>
        /// Gets the product variant ID for the given choices
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="optionChoices">An array of option choices for the product</param>
        /// <returns>The id of the variant for the given choices</returns>
        public static int GetProductVariantId(int productId, int[] optionChoices)
        {
            if ((optionChoices == null) || (optionChoices.Length == 0)) return 0;
            //CHECK THE DATABASE FOR A PRODUCT VARIANT WITH THE GIVEN OPTION COMBINATION
            //IF FOUND RETURN THE GUID.  OTHERWISE GUID.EMPTY
            //GET RECORDS STARTING AT FIRST ROW
            List<string> criteria = new List<string>();
            int optionCount = optionChoices.Length;
            for (int i = 1; i <= ProductVariant.MAXIMUM_ATTRIBUTES; i++)
            {
                if (i <= optionCount) criteria.Add("Option" + i.ToString() + " = @opt" + i.ToString());
                else criteria.Add("Option" + i.ToString() + " = 0");
            }
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT ProductVariantId FROM ac_ProductVariants");
            selectQuery.Append(" WHERE ProductId = @productId AND ");
            selectQuery.Append(string.Join(" AND ", criteria.ToArray()));
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            for (int i = 1; i <= optionCount; i++)
            {
                database.AddInParameter(selectCommand, "@opt" + i.ToString(), System.Data.DbType.Int32, optionChoices[i - 1]);
            }
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets the option list for the given product with given option choices
        /// </summary>
        /// <param name="productId">Id of the product for which to get the variant</param>
        /// <param name="optionChoices">Selected option choices of the variant</param>
        /// <param name="ignoreInvalidOptions">If true invalid options are ignored</param>
        /// <returns>Comma delimited option list string for the variant</returns>
        public static string GetOptionList(int productId, Dictionary<int, int> optionChoices, bool ignoreInvalidOptions)
        {
            if ((optionChoices == null) || (optionChoices.Count == 0)) return string.Empty;
            int[] optionChoiceArray;
            try
            {
                optionChoiceArray = ProductVariantManager.GetSortedOptionChoices(productId, optionChoices);
            }
            catch (InvalidOptionsException ipaex)
            {
                if (ignoreInvalidOptions) return string.Empty;
                throw ipaex;
            }
            return ProductVariantDataSource.GetOptionList(productId, optionChoiceArray);
        }

        /// <summary>
        /// Gets the option list for the given product with given option choices
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="optionChoices">An array of option choices for the product</param>
        /// <returns>Comma delimited option list string for the variant</returns>
        public static string GetOptionList(int productId, int[] optionChoices)
        {
            if ((optionChoices == null) || (optionChoices.Length == 0)) return string.Empty;
            int optionCount = optionChoices.Length;
            //THE LIST SHOULD INCLUDE AT LEAST THE MAXIMUM NUMBER OF OPTIONS
            //BUT INCLUDE MORE IF THEY ARE PROVIDED
            int endCount = Math.Max(ProductVariant.MAXIMUM_ATTRIBUTES, optionCount);
            List<string> tempChoices = new List<string>();
            for (int i = 0; i < endCount; i++)
            {
                if (i < optionCount) tempChoices.Add(optionChoices[i].ToString());
                else tempChoices.Add("0");
            }
            return string.Join(",", tempChoices.ToArray());
        }

        /// <summary>
        /// Gets the product variant name(s) for the given option choices 
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="optionChoices">An array of option choices for the product</param>
        /// <param name="separator">If there are more than one variants matching the given criteria, this is the separator used to separate the names</param>
        /// <returns>Name or names of variants separated by the separator</returns>
        public static string GetVariantName(int productId, int[] optionChoices, string separator)
        {
            //GET RECORDS STARTING AT FIRST ROW
            List<string> criteria = new List<string>();
            int choiceCount = optionChoices.Length;
            for (int i = 0; i < choiceCount; i++)
            {
                criteria.Add("OC.OptionChoiceId = @choice" + i.ToString());
            }
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT OC.Name FROM ac_Options O, ac_OptionChoices OC");
            selectQuery.Append(" WHERE O.OptionId = OC.OptionId AND (");
            selectQuery.Append(string.Join(" OR ", criteria.ToArray()));
            selectQuery.Append(")");
            //SORT THE OPTION NAMES BY MERCHANT DEFINED ORDER OF ATTRIBUTES
            selectQuery.Append(" ORDER BY O.OrderBy");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            for (int i = 0; i < choiceCount; i++)
            {
                database.AddInParameter(selectCommand, "@choice" + i.ToString(), System.Data.DbType.Int32, optionChoices[i]);
            }
            //EXECUTE THE COMMAND
            List<string> name = new List<string>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while(dr.Read())
                {
                    name.Add(dr.GetString(0));
                }
                dr.Close();
            }
            return string.Join(separator, name.ToArray());
        }

        /// <summary>
        /// Counts the number of ProductVariants for the given product
        /// </summary>
        /// <param name="productId">Id of the product for which to count the number of variants</param>
        /// <returns>The number of ProductVariants for the given product</returns>
        public virtual int CountAllForProduct(int productId)
        {
            //calculate the maximum number of options
            Product product = new Product();
            if (!product.Load(productId)) return 0;
            return (ProductVariantDataSource.CountVariantsForProduct(product));
        }

        /// <summary>
        /// Counts the number of ProductVariants for the given product
        /// </summary>
        /// <param name="product">The product for which to count the number of variants</param>
        /// <returns>The number of ProductVariants for the given product</returns>
        public static int CountVariantsForProduct(Product product)
        {
            int rowCount = 1;
            foreach (ProductOption item in product.ProductOptions)
            {
                rowCount = (rowCount * item.Option.Choices.Count);
            }
            return rowCount;
        }

        /// <summary>
        /// Gets a comma delimited list of option choices for the given row
        /// </summary>
        /// <param name="optionCollection">The collection of options that apply to the product</param>
        /// <param name="optionMultiplier">The option multiplier for this product</param>
        /// <param name="row">The row of the option matrix to build choice list for</param>
        /// <returns>An int array of option choices for the given row</returns>
        private int[] GetOptionChoices(OptionCollection optionCollection, int[] optionMultiplier, int row)
        {
            int rowRemainder = row;
            int[] choices = new int[ProductVariant.MAXIMUM_ATTRIBUTES];
            int optionCount = optionCollection.Count;
            for (int i = ProductVariant.MAXIMUM_ATTRIBUTES - 1; i >= 0; i--)
            {
                if (i >= optionCount)
                {
                    choices[i] = 0;
                }
                else
                {
                    Option option = optionCollection[i];
                    int optionIndex = (int)Decimal.Truncate(rowRemainder / optionMultiplier[i]);
                    choices[i] = option.Choices[optionIndex].OptionChoiceId;
                    rowRemainder = (rowRemainder % optionMultiplier[i]);
                }
            }
            return choices;
        }

        /// <summary>
        /// Gets a collection of options for the given product.
        /// </summary>
        /// <param name="product">The product to get options for</param>
        /// <returns>A collection of options</returns>
        private OptionCollection GetOptions(Product product)
        {
            OptionCollection options = new OptionCollection();
            foreach (ProductOption item in product.ProductOptions)
            {
                options.Add(item.Option);
            }
            return options;
        }

        /// <summary>
        /// Loads all ProductVariants for the given product
        /// </summary>
        /// <param name="productId">Id of the product for which to load the ProductVariants</param>
        /// <returns>Collection of ProductVariant objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public virtual PersistentCollection<ProductVariant> LoadAllForProduct(int productId)
        {
            return LoadAllForProduct(productId, 0, 0);
        }

        /// <summary>
        /// Loads all ProductVariants for the given product
        /// </summary>
        /// <param name="productId">Id of the product for which to load the ProductVariants</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>Collection of ProductVariant objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public virtual PersistentCollection<ProductVariant> LoadAllForProduct(int productId, int maximumRows, int startRowIndex)
        {
            //LOAD THE PRODUCT
            Product product = new Product();
            if (!product.Load(productId)) throw new InvalidProductException();

            //COUNT ROWS TO MAKE SURE AT LEAST ONE ATTRIBUTE IS DEFINED AND ALL ATTRIBUTES HAVE AT LEAST ONE OPTION
            int rowCount = this.CountAllForProduct(productId);
            if (rowCount == 0) throw new InvalidOptionsException();

            //CREATE COLLECTION TO HOLD GENERATED VARIANTS
            PersistentCollection<ProductVariant> variantCollection = new PersistentCollection<ProductVariant>();

            //BUILD THE COLLECTION OF OPTIONS FOR THIS PRODUCT
            OptionCollection options = GetOptions(product);

            //GET THE MULTIPLIER VALUES FOR MATHING THE ROW INDEXES
            int[] optionMultiplier = new int[product.ProductOptions.Count];
            optionMultiplier[0] = 1;
            for (int i = 1; i < options.Count; i++)
            {
                //THE MULTIPLIER FOR CURRENT OPTION IS THE MULTIPLIER OF PREVIOUS OPTION TIMES NUMBER OF PREVIOUS OPTION CHOICES
                Option previousOption = options[i - 1];
                optionMultiplier[i] = previousOption.Choices.Count * optionMultiplier[i - 1];
            }

            //CALCULATE START/END POSITION AND LOOP
            if (startRowIndex > maximumRows) throw new ArgumentOutOfRangeException("startRowIndex");
            int endCount = startRowIndex + maximumRows;
            if ((maximumRows < 1) || (endCount > rowCount)) endCount = rowCount;
            for (int i = startRowIndex; i < endCount; i++)
            {
                //GET THE OPTIONS FOR THIS ROW INDEX
                int[] choices = GetOptionChoices(options, optionMultiplier, i);
                //CREATE THE PRODUCT VARIANT
                ProductVariant productVariant = new ProductVariant();
                productVariant.ProductVariantId = 0;
                productVariant.ProductId = productId;
                productVariant.Option1 = choices[0];
                productVariant.Option2 = choices[1];
                productVariant.Option3 = choices[2];
                productVariant.Option4 = choices[3];
                productVariant.Option5 = choices[4];
                productVariant.Option6 = choices[5];
                productVariant.Option7 = choices[6];
                productVariant.Option8 = choices[7]; 
                productVariant.Sku = string.Empty;
                productVariant.Price = 0;
                productVariant.Weight = 0;
                productVariant.CostOfGoods = 0;
                productVariant.InStock = 0;
                productVariant.IsDirty = false;
                variantCollection.Add(productVariant);
            }

            //RETURN THE CALCULATED VARIANT GRID
            return variantCollection;
        }

        /// <summary>
        /// Gets a variant for the given options
        /// </summary>
        /// <param name="productId">ID of the product</param>
        /// <param name="optionList">Comma delimited list of option choices</param>
        /// <returns>The variant for the given options, or null if the options are invalid</returns>
        public static ProductVariant LoadForOptionList(int productId, string optionList)
        {
            ProductVariant v = new ProductVariant();
            if (v.Load(productId, optionList)) return v;
            return null;
        }

        /// <summary>
        /// Gets a variant for the given options
        /// </summary>
        /// <param name="productId">ID of the product</param>
        /// <param name="optionChoiceIds">An array of option choices</param>
        /// <returns>The variant for the given options, or null if the options are invalid</returns>
        public static ProductVariant LoadForOptionList(int productId, int[] optionChoiceIds)
        {
            ProductVariant v = new ProductVariant();
            if (v.Load(productId, optionChoiceIds)) return v;
            return null;
        }
    }
}
