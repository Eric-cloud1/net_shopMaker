<%@ Page Language="C#" %> <%@ Import Namespace="MakerShop.Data" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

  protected void Page_Load(object sender, EventArgs e)
  {
      Response.Clear();
      Response.Write("Checking Variant Data<br/>");
      bool deleteMode = (AlwaysConvert.ToInt(Request.QueryString["Delete"]) == 1);
      if (deleteMode) Response.Write("Delete mode enabled<br/>");
      Response.Flush();
      Database database = Token.Instance.Database;
      System.Data.DataSet products = database.ExecuteDataSet(System.Data.CommandType.Text, "SELECT DISTINCT ProductId FROM ac_ProductVariants");
      int count = products.Tables[0].Rows.Count;
      Response.Write("Found variant data for " + count.ToString() + "products<BR/>");
      Response.Flush();
      if (count > 0)
      {
          foreach (System.Data.DataRow row in products.Tables[0].Rows)
          {
              int productId = (int)row[0];
              Product product = ProductDataSource.Load(productId);
              if (product != null)
              {
                  if (product.ProductOptions.Count > 0)
                  {
                      //HAVE TO VALIDATE EACH VARIANT TO SEE IF IT IS VALID
                      System.Data.DataSet variants = database.ExecuteDataSet(System.Data.CommandType.Text, "SELECT ProductVariantId,Option1,Option2,Option3,Option4,Option5,Option6,Option7,Option8 FROM ac_ProductVariants WHERE ProductId = " + productId);
                      foreach (System.Data.DataRow variantRow in variants.Tables[0].Rows)
                      {
                          if (!IsVariantValid(product, (int)variantRow[1], (int)variantRow[2], (int)variantRow[3], (int)variantRow[4], (int)variantRow[5], (int)variantRow[6], (int)variantRow[7], (int)variantRow[8]))
                          {
                              Response.Write("Invalid variant detected for product #" + productId + "<br/>");
                              if (deleteMode) database.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM ac_ProductVariants WHERE ProductId = " + productId + " AND ProductVariantId = " + variantRow[0].ToString());
                          }
                          else
                          {
                              Response.Write("Valid variant data detected for product #" + productId + "<br/>");
                          }
                      }
                  }
                  else
                  {
                      Response.Write("Variant data detected for product #" + productId + " which has no variants<br/>");
                      if (deleteMode) database.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM ac_ProductVariants WHERE ProductId = " + productId.ToString());
                  }
              }
              else
              {
                  Response.Write("Variant data detected for invalid product #" + productId + "<br/>");
                  if (deleteMode) database.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM ac_ProductVariants WHERE ProductId = " + productId.ToString());
              }
              Response.Flush();
          }

      }
      Response.Write("Variant data validated.");
      Response.End();
  }

  private bool IsVariantValid(Product product, int option1, int option2, int option3, int option4, int option5, int option6, int option7, int option8)
  {
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
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Untitled Page</title>
  </head>
  <body>
    <form id="form1" runat="server">
      <div></div>
    </form>
  </body>
</html>
