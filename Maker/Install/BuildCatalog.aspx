<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    ManufacturerCollection manufacturers;
    
    protected int GetRandomManufacturerId()
    {
        Random rand = new Random();
        int index = rand.Next(manufacturers.Count);
        return manufacturers[index].ManufacturerId;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        Store store = StoreDataSource.Load();
        Category storeCatalog = CategoryDataSource.Load(0);
        //CHECK FOR MANUFACTURERS
        manufacturers = store.Manufacturers;
        if (manufacturers.Count == 0)
        {
            //ADD SOME SAMPLE MANUFACTURERS
            Manufacturer newManufacturer = new Manufacturer();
            newManufacturer.StoreId = store.StoreId;
            newManufacturer.Name = "Gadgets Ltd.";
            manufacturers.Add(newManufacturer);
            newManufacturer = new Manufacturer();
            newManufacturer.StoreId = store.StoreId;
            newManufacturer.Name = "Stuff, Inc.";
            manufacturers.Add(newManufacturer);
            newManufacturer = new Manufacturer();
            newManufacturer.StoreId = store.StoreId;
            newManufacturer.Name = "Things Intl";
            manufacturers.Add(newManufacturer);
            newManufacturer = new Manufacturer();
            newManufacturer.StoreId = store.StoreId;
            newManufacturer.Name = "Trinkets Tech";
            manufacturers.Add(newManufacturer);
            newManufacturer = new Manufacturer();
            newManufacturer.StoreId = store.StoreId;
            newManufacturer.Name = "Widget Works";
            manufacturers.Add(newManufacturer);
            manufacturers.Save();
        }
        int firstLevelCategories = 10;
        int secondLevelCategories = 10;
        int thirdLevelCategories = 100;
        int thirdLevelProducts = 100;
        string key;
        int totalProductCount = firstLevelCategories * secondLevelCategories * thirdLevelCategories * thirdLevelProducts;
        int productCount = 0;
        
        Response.Clear();
        Response.Write("Building " + totalProductCount.ToString() + " products:");
        Response.Flush();

        int defaultWarehouseId = store.DefaultWarehouse.WarehouseId;
        
        for (int i = 1; i <= firstLevelCategories; i++)
        {
            Category category1 = new Category();
            category1.Name = "Category C" + i.ToString();
            category1.Visibility = CatalogVisibility.Public;
            category1.ParentId = 0;
            category1.Save();
            for (int j = 1; j <= secondLevelCategories; j++)
            {
                key = "C" + i.ToString() + "-C" + j.ToString();
                Category category2 = new Category();
                category2.Name = "Subcategory " + key;
                category2.Visibility = CatalogVisibility.Public;
                category2.ParentId = category1.CategoryId;
                category2.Save();
                for (int k = 1; k <= thirdLevelCategories; k++)
                {
                    key = "C" + i.ToString() + "-C" + j.ToString() + "-C" + k.ToString();
                    Category category3 = new Category();
                    category3.Name = "Sub Subcategory " + k;
                    category3.Visibility = CatalogVisibility.Public;
                    category3.ParentId = category2.CategoryId;
                    category3.Save();
                    for (int l = 1; l <= thirdLevelProducts; l++)
                    {
                        if (productCount % 100 == 0)
                        {
                            Response.Write(" " + productCount.ToString());
                            Response.Flush();
                        }
                        productCount++;
                        key = "C" + i.ToString() + "-C" + j.ToString() + "-C" + k.ToString() + "-P" + l.ToString();
                        Product product = new Product();
                        product.Visibility = CatalogVisibility.Public;
                        product.WarehouseId = defaultWarehouseId;
                        product.ManufacturerId = GetRandomManufacturerId();
                        product.Name = "Product " + key;
                        product.Price = (LSDecimal)l;
                        product.Sku = "SKU" + key;
                        product.ModelNumber = "MN" + key;
                        product.Summary = "This is a sample product summary.";
                        product.Description = "This is a sample product description.";
                        product.Shippable = Shippable.Yes;
                        product.Weight = 1;
                        product.Categories.Add(category3.CategoryId);
                        product.Save();
                    }
                }
            }
        }
        Response.Write(" DONE!");
        Response.Flush();
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Catalog Built
    </div>
    </form>
</body>
</html>
