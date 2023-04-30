<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" Title="Merchant
Login" %> <%@ Register Src="~/Admin/UserControls/LoginDialog.ascx"
TagName="LoginDialog" TagPrefix="uc" %>
<script runat="server">
  private void FixDates()
  {
      Random rand = new Random();
      List<DateTime> orderDates = new List<DateTime>();
      OrderCollection fixOrders = OrderDataSource.LoadForStore();
      for (int i = 0; i < fixOrders.Count; i++)
      {
          int daysAgo = rand.Next(0, 90);
          int minutesAgo = rand.Next(0, (24 * 60));
          orderDates.Add(DateTime.UtcNow.AddDays((-1 * daysAgo)).AddMinutes((-1 * minutesAgo)));
      }
      List<int> orderNumbers = new List<int>();
      foreach (Order fixOrder in fixOrders)
      {
          orderNumbers.Add(fixOrder.OrderId);
      }
      orderDates.Sort();
      orderNumbers.Sort();
      int dateIndex = 0;
      DateTime[] dateArray = orderDates.ToArray();
      foreach (int orderId in orderNumbers)
      {
          Order fixOrder = fixOrders[fixOrders.IndexOf(orderId)];
          fixOrder.OrderDate = dateArray[dateIndex];
          fixOrder.Save(false);
          dateIndex++;
      }
      //UPDATE DATES IN PAGEVIEWS
      PageViewCollection views = PageViewDataSource.LoadForStore();
      foreach(PageView pageView in views)
      {
          int daysAgo = rand.Next(0, 14);
          int minutesAgo = rand.Next(0, (24 * 60));
          pageView.ActivityDate = DateTime.UtcNow.AddDays((-1 * daysAgo)).AddMinutes((-1 * minutesAgo));
          pageView.Save();
      }
      //CLEAR THE ERROR LOG
      ErrorMessageDataSource.DeleteForStore();
  }

  protected void Page_Init(object sender, EventArgs e)
  {
      string demoFile = Server.MapPath("~/demo.dat");
      if (System.IO.File.Exists(demoFile))
      {
          string demoData = System.IO.File.ReadAllText(demoFile);
          string[] tokens = demoData.Split("|".ToCharArray());
          Token.Instance.Store.Name = tokens[0];
          System.Data.Common.DbCommand command = Token.Instance.Database.GetSqlStringCommand("UPDATE ac_Users SET UserName = @username, LoweredUserName = @username, Email = @username, LoweredEmail = @username WHERE UserName = 'admin@MakerShop.com'");
          Token.Instance.Database.AddInParameter(command, "@username", System.Data.DbType.String, tokens[1]);
          Token.Instance.Database.ExecuteNonQuery(command);
          Token.Instance.Store.StoreUrl = GetStoreUrl();
          Token.Instance.Store.Save();
          // UPDATE LAST PASSWORD DATE
          command = Token.Instance.Database.GetSqlStringCommand("UPDATE ac_UserPasswords SET CreateDate = @today");
          Token.Instance.Database.AddInParameter(command, "@today", System.Data.DbType.DateTime, DateTime.UtcNow);
          Token.Instance.Database.ExecuteNonQuery(command);
          System.IO.File.Delete(demoFile);
          FixDates();
          FixPassword();
      }
  }

  private string GetStoreUrl()
  {
      string tempUrl = Request.Url.ToString();
      int qIndex = tempUrl.IndexOf("?");
      if (qIndex > -1)
      {
          tempUrl = tempUrl.Substring(0, qIndex);
      }
      tempUrl = tempUrl.ToLowerInvariant().Replace("admin/login.aspx", string.Empty);
      return tempUrl;
  }

  private void FixPassword()
  {
      MerchantPasswordPolicy merchantPolicy = new MerchantPasswordPolicy();
      merchantPolicy.MinLength = 6;
      merchantPolicy.RequireUpper = false;
      merchantPolicy.RequireLower = false;
      merchantPolicy.RequireNumber = false;
      merchantPolicy.RequireSymbol = false;
      merchantPolicy.RequireNonAlpha = false;
      merchantPolicy.MaxAge = 90;
      merchantPolicy.HistoryDays = 0;
      merchantPolicy.HistoryCount = 0;
      merchantPolicy.MaxAttempts = 20;
      merchantPolicy.LockoutPeriod = 1;
      merchantPolicy.InactivePeriod = 24;
      merchantPolicy.ImageCaptcha = false;
      merchantPolicy.Save();
  }
</script>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div
    class="loginWrapper"
    style="width: 400px; margin: 0 auto; padding: 20px 0"
  >
    <uc:LoginDialog ID="LoginDialog1" runat="server" />
  </div>
</asp:Content>
