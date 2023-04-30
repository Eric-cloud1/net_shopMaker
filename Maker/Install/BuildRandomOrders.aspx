<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
  ShipMethodCollection shipMethods;
  PaymentMethodCollection payMethods;
  int visaIndex = -1;

  string[] lastNames = { "SMITH", "JOHNSON", "WILLIAMS", "JONES", "BROWN", "DAVIS", "MILLER", "WILSON", "MOORE", "TAYLOR", "ANDERSON", "THOMAS", "JACKSON", "WHITE", "HARRIS", "MARTIN", "THOMPSON", "GARCIA", "MARTINEZ", "ROBINSON", "CLARK", "RODRIGUEZ", "LEWIS", "LEE", "WALKER", "HALL", "ALLEN", "YOUNG", "HERNANDEZ", "KING", "WRIGHT", "LOPEZ", "HILL", "SCOTT", "GREEN", "ADAMS", "BAKER", "GONZALEZ", "NELSON", "CARTER", "MITCHELL", "PEREZ", "ROBERTS", "TURNER", "PHILLIPS", "CAMPBELL", "PARKER", "EVANS", "EDWARDS", "COLLINS", "STEWART", "SANCHEZ", "MORRIS", "ROGERS", "REED", "COOK", "MORGAN", "BELL", "MURPHY", "BAILEY", "RIVERA", "COOPER", "RICHARDSON", "COX", "HOWARD", "WARD", "TORRES", "PETERSON", "GRAY", "RAMIREZ", "JAMES", "WATSON", "BROOKS", "KELLY", "SANDERS", "PRICE", "BENNETT", "WOOD", "BARNES", "ROSS", "HENDERSON", "COLEMAN", "JENKINS", "PERRY", "POWELL", "LONG", "PATTERSON", "HUGHES", "FLORES", "WASHINGTON", "BUTLER", "SIMMONS", "FOSTER", "GONZALES", "BRYANT", "ALEXANDER", "RUSSELL", "GRIFFIN", "DIAZ", "HAYES", "MYERS", "FORD", "HAMILTON", "GRAHAM", "SULLIVAN", "WALLACE", "WOODS", "COLE", "WEST", "JORDAN", "OWENS", "REYNOLDS", "FISHER", "ELLIS", "HARRISON", "GIBSON", "MCDONALD", "CRUZ", "MARSHALL", "ORTIZ", "GOMEZ", "MURRAY", "FREEMAN", "WELLS", "WEBB", "SIMPSON", "STEVENS", "TUCKER", "PORTER", "HUNTER", "HICKS", "CRAWFORD", "HENRY", "BOYD", "MASON", "MORALES", "KENNEDY", "WARREN", "DIXON", "RAMOS", "REYES", "BURNS", "GORDON", "SHAW", "HOLMES" };
  string[] firstNames = { "JAMES", "JOHN", "ROBERT", "MICHAEL", "WILLIAM", "DAVID", "RICHARD", "CHARLES", "JOSEPH", "THOMAS", "CHRISTOPHER", "DANIEL", "PAUL", "MARK", "DONALD", "GEORGE", "KENNETH", "STEVEN", "EDWARD", "BRIAN", "RONALD", "ANTHONY", "KEVIN", "JASON", "MATTHEW", "GARY", "TIMOTHY", "JOSE", "LARRY", "JEFFREY", "FRANK", "SCOTT", "ERIC", "STEPHEN", "ANDREW", "RAYMOND", "GREGORY", "JOSHUA", "JERRY", "DENNIS", "WALTER", "PATRICK", "PETER", "HAROLD", "DOUGLAS", "HENRY", "CARL", "ARTHUR", "RYAN", "ROGER", "JOE", "JUAN", "JACK", "ALBERT", "MARY", "PATRICIA", "LINDA", "BARBARA", "ELIZABETH", "JENNIFER", "MARIA", "SUSAN", "MARGARET", "DOROTHY", "LISA", "NANCY", "KAREN", "BETTY", "HELEN", "SANDRA", "DONNA", "CAROL", "RUTH", "SHARON", "MICHELLE", "LAURA", "SARAH", "KIMBERLY", "DEBORAH", "JESSICA", "SHIRLEY", "CYNTHIA", "ANGELA", "MELISSA", "BRENDA", "AMY", "ANNA", "REBECCA", "VIRGINIA", "KATHLEEN", "PAMELA", "MARTHA", "DEBRA", "AMANDA", "STEPHANIE", "CAROLYN", "CHRISTINE", "MARIE", "JANET", "CATHERINE", "FRANCES", "ANN", "JOYCE", "DIANE", "ALICE", "JULIE", "HEATHER", "TERESA", "DORIS", "GLORIA", "EVELYN", "JEAN", "CHERYL", "MILDRED", "KATHERINE", "JOAN", "ASHLEY", "JUDITH", "ROSE", "JANICE", "KELLY", "NICOLE", "JUDY", "CHRISTINA" };
  string[] streetNames = { "Highland", "Forest", "Jefferson", "Hickory", "Wilson", "River", "Meadow", "Valley", "Smith", "East", "Chestnut", "Franklin", "Adams", "Spruce", "Laurel", "Davis", "Birch", "Williams", "Lee", "Dogwood", "Green", "Poplar", "Locust", "Woodland", "Taylor", "Ash", "Madison", "Hillcrest", "Sycamore", "Broadway", "Miller", "Lakeview", "College", "Central", "Park", "Main", "Oak", "Pine", "Maple", "Cedar", "Elm", "View", "Washington", "Lake", "Hill", "Walnut", "Spring", "North", "Ridge", "Lincoln", "Church", "Willow", "Mill", "Sunset", "Railroad", "Jackson", "Cherry", "West", "South", "Center" };
  string[] streetSuffixes = { "Ave", "St", "Ln", "Rd" };

  protected void SetRandomZip(ref Address address)
  {
      int city = (new Random()).Next(1, 10);
      switch (city)
      {
          case 1:
              address.City = "New York";
              address.Province = "NY";
              address.PostalCode = "10101";
              break;
          case 2:
              address.City = "Los Angeles";
              address.Province = "CA";
              address.PostalCode = "90001";
              break;
          case 3:
              address.City = "Chicago";
              address.Province = "IL";
              address.PostalCode = "60601";
              break;
          case 4:
              address.City = "Houston";
              address.Province = "TX";
              address.PostalCode = "77002";
              break;
          case 5:
              address.City = "Philadelphia";
              address.Province = "PA";
              address.PostalCode = "19103";
              break;
          case 6:
              address.City = "Phoenix";
              address.Province = "AZ";
              address.PostalCode = "85008";
              break;
          case 7:
              address.City = "San Antonio";
              address.Province = "TX";
              address.PostalCode = "78205";
              break;
          case 8:
              address.City = "San Diego";
              address.Province = "CA";
              address.PostalCode = "92103";
              break;
          case 9:
              address.City = "Dallas";
              address.Province = "TX";
              address.PostalCode = "75201";
              break;
          default:
              address.City = "San Jose";
              address.Province = "CA";
              address.PostalCode = "95101";
              break;
      }
  }

  protected string ToInitialCase(string input)
  {
      return input.Substring(0, 1) + input.Substring(1).ToLowerInvariant();
  }

  protected string GetRandomLastName()
  {
      return ToInitialCase(lastNames[(new Random()).Next(lastNames.Length - 1)]);
  }

  protected string GetRandomFirstName()
  {
      return ToInitialCase(firstNames[(new Random()).Next(firstNames.Length - 1)]);
  }

  protected string GetRandomStreetName()
  {
      string houseNumber = StringHelper.RandomNumber(4);
      string streetName = streetNames[(new Random()).Next(streetNames.Length - 1)];
      string streetSuffix = streetSuffixes[(new Random()).Next(streetSuffixes.Length - 1)];
      return houseNumber + " " + streetName + " " + streetSuffix;
  }

  protected string GenerateReferenceNumber(string creditCardNumber)
  {
      if (string.IsNullOrEmpty(creditCardNumber)) return string.Empty;
      int length = creditCardNumber.Length;
      if (length < 5) return creditCardNumber;
      return ("x" + creditCardNumber.Substring((length - 4)));
  }

  protected Payment GetPayment(LSDecimal amount)
  {
      PaymentMethod method = payMethods[visaIndex];
      Payment payment = new Payment();
      payment.PaymentMethodId = method.PaymentMethodId;
      payment.Amount = amount;
      payment.CurrencyCode = "USD";
      AccountDataDictionary instrumentBuilder;
      switch (method.PaymentInstrument)
      {
          case PaymentInstrument.Visa:
              instrumentBuilder = new AccountDataDictionary();
              instrumentBuilder["AccountNumber"] = MakerShop.Web.UI.WebControls.CreditCardValidator.GenerateRandomNumber(MakerShop.Web.UI.WebControls.CardType.Visa);
              instrumentBuilder["ExpirationMonth"] = "12";
              instrumentBuilder["ExpirationYear"] = "2007";
              instrumentBuilder["SecurityCode"] = StringHelper.RandomNumber(3);
              payment.AccountData = instrumentBuilder.ToString();
              payment.ReferenceNumber = GenerateReferenceNumber(instrumentBuilder["AccountNumber"]);
              break;
          case PaymentInstrument.MasterCard:
              instrumentBuilder = new AccountDataDictionary();
              instrumentBuilder["AccountNumber"] = MakerShop.Web.UI.WebControls.CreditCardValidator.GenerateRandomNumber(MakerShop.Web.UI.WebControls.CardType.MasterCard);
              instrumentBuilder["ExpirationMonth"] = "12";
              instrumentBuilder["ExpirationYear"] = "2007";
              instrumentBuilder["SecurityCode"] = "515";
              payment.AccountData = instrumentBuilder.ToString();
              payment.ReferenceNumber = GenerateReferenceNumber(instrumentBuilder["AccountNumber"]);
              break;
      }
      return payment;
  }

  protected ShipMethod GetShipMethod()
  {
      Random rand = new Random();
      int index = rand.Next(shipMethods.Count);
      return shipMethods[index];
  }

  protected User GetRandomUser()
  {
      string firstName = GetRandomFirstName();
      string lastName = GetRandomLastName();
      string email = firstName + "." + lastName + "@sample.MakerShop.xyz";
      User tempUser = UserDataSource.LoadForUserName(email, false);
      if (tempUser != null) return tempUser;
      //user not found, create new
      tempUser = UserDataSource.CreateUser(email, StringHelper.RandomString(8));
      Address billto = tempUser.PrimaryAddress;
      billto.FirstName = firstName;
      billto.LastName = lastName;
      billto.Address1 = GetRandomStreetName();
      billto.Email = email;
      SetRandomZip(ref billto);
      billto.CountryCode = "US";
      tempUser.Save();
      return tempUser;
  }

  private void BuildRandomOrders(int orders, int maxOrderItems, bool output)
  {
      System.Data.Common.DbCommand updateCommand;
      shipMethods = ShipMethodDataSource.LoadForStore();
      payMethods = PaymentMethodDataSource.LoadForStore();
      int tempIndex = 0;
      foreach (PaymentMethod method in payMethods)
      {
          if (method.PaymentInstrument == PaymentInstrument.Visa) visaIndex = tempIndex;
          tempIndex++;
      }

      ProductCollection products = ProductDataSource.LoadForCriteria("VisibilityId = 0 AND UseVariablePrice = 0", 100, 0, string.Empty);

      if (output)
      {
          Response.Clear();
          Response.Write("Building " + orders.ToString() + " orders:");
          Response.Flush();
      }

      Random rand = new Random();
      for (int i = 1; i <= orders; i++)
      {
          User tempUser = GetRandomUser();
          Basket basket = tempUser.Basket;
          int orderItems = rand.Next(1, maxOrderItems);
          for (int j = 1; j <= orderItems; j++)
          {
              int prodIndex = rand.Next(products.Count - 1);
              //ONLY USE PRODUCTS THAT ARE NOT KITS AND HAVE LESS THAN 2 OPTIONS
              Product product = products[prodIndex];
              while ((product.ProductOptions.Count > 1) || (product.KitStatus == KitStatus.Master))
              {
                  prodIndex = rand.Next(products.Count - 1);
                  product = products[prodIndex];
              }
              string optionList = string.Empty;
              if (product.ProductOptions.Count == 1)
              {
                  int randOption = rand.Next(0, product.ProductOptions[0].Option.Choices.Count);
                  optionList = product.ProductOptions[0].Option.Choices[randOption].OptionChoiceId + ",0,0,0,0,0,0,0";
              }
              BasketItem basketItem = BasketItemDataSource.CreateForProduct(products[prodIndex].ProductId, 1, optionList, string.Empty, PaymentTypes.Initial);
              basket.Items.Add(basketItem);
          }
          //DO NOT SAVE THIS ORDER IF IT HAS NO VALUE
          if (basket.Items.TotalPrice() > 0)
          {
              basket.Save();
              basket.Package();
              foreach (BasketShipment shipment in basket.Shipments)
              {
                  shipment.AddressId = tempUser.PrimaryAddressId;
                  shipment.ApplyShipMethod(GetShipMethod());
                  shipment.Save();
              }
              CheckoutRequest request = new CheckoutRequest(GetPayment(basket.Items.TotalPrice()));
              CheckoutResponse response;
              try
              {
                  response = basket.Checkout(request);
              }
              catch
              {
                  response = new CheckoutResponse(false);
              }
              if (response.Success)
              {
                  //SET THE ORDERDATE TO SOMETHING RANDOM WITHIN LAST 90 DAYS
                  int daysAgo = rand.Next(0, 90);
                  int minutesAgo = rand.Next(0, (24 * 60));
                  DateTime newOrderDate = DateTime.UtcNow.AddDays((-1 * daysAgo)).AddMinutes((-1 * minutesAgo));
                  updateCommand = Token.Instance.Database.GetSqlStringCommand("UPDATE ac_Orders SET OrderDate = @orderDate WHERE OrderId = @orderId");
                  Token.Instance.Database.AddInParameter(updateCommand, "orderDate", System.Data.DbType.DateTime, newOrderDate);
                  Token.Instance.Database.AddInParameter(updateCommand, "orderId", System.Data.DbType.Int32, response.OrderId);
                  Token.Instance.Database.ExecuteNonQuery(updateCommand);
              }
          }
          if (output)
          {
              Response.Write(" .");
              Response.Flush();
          }
      }
      //FIX ORDER DATES
      OrderCollection fixOrders = OrderDataSource.LoadForStore();
      List<DateTime> orderDates = new List<DateTime>();
      List<int> orderNumbers = new List<int>();
      foreach (Order fixOrder in fixOrders)
      {
          orderDates.Add(fixOrder.OrderDate);
          orderNumbers.Add(fixOrder.OrderId);
      }
      orderDates.Sort();
      orderNumbers.Sort();
      int dateIndex = 0;
      Response.Write("<BR><BR>Resetting dates ");
      Response.Flush();
      DateTime[] dateArray = orderDates.ToArray();
      foreach (int orderId in orderNumbers)
      {
          Order fixOrder = fixOrders[fixOrders.IndexOf(orderId)];
          fixOrder.OrderDate = dateArray[dateIndex];
          foreach (Payment payment in fixOrder.Payments)
              payment.PaymentStatus = PaymentStatus.Completed;
          fixOrder.Save();
          dateIndex++;
          Response.Write(" .");
          Response.Flush();
      }
      if (output)
      {
          Response.Write(" DONE!");
          Response.Flush();
          Response.End();
      }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      Token.Instance.InitUserContext(HttpContext.Current);
      int orders = AlwaysConvert.ToInt(Request.QueryString["orders"]);
      if (orders < 1) orders = 1;
      int maxOrderItems = AlwaysConvert.ToInt(Request.QueryString["items"]);
      if (maxOrderItems < 1) maxOrderItems = 1;
      BuildRandomOrders(orders, maxOrderItems, true);
  }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Untitled Page</title>
  </head>
  <body>
    <form id="form1" runat="server">
      <div>Orders Generated</div>
    </form>
  </body>
</html>
