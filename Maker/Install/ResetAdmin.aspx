<%@ Page Language="C#" EnableTheming="false" EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

  protected void Page_Load(object sender, EventArgs e)
  {
      //BY DEFAULT THIS SCRIPT IS DISABLED
       //CHANGE THE LINE BELOW FROM "false" TO "true" TO USE THIS SCRIPT
      bool enabled = false;

      //THIS IS THE USER THAT WILL BE CREATED OR RESET
      //YOU CAN UPDATE THEM IF YOU WANT TO USE SOMETHING OTHER THAN DEFAULT
      string userName = "admin@MakerShop.com";
      string password = "password";

      //THESE ARE VARIABLES FOR THE GROUP ASSOCIATION
      //THESE SHOULD NOT BE MODIFIED EXCEPT BY MakerShop
      string groupName = "Super Users";
      string roleName = "System";
      if (enabled)
      {
          //CREATE AND/OR UPDATE THE PASSWORD FOR THE ADMIN USER
          User user = UserDataSource.LoadForUserName(userName);
          if (user == null)
          {
              Response.Write("Created specified admin user with specified password.<br />");
              user = UserDataSource.CreateUser(userName, password);
          }
          else
          {
              Response.Write("Found specified admin user and reset with specified password.<br />");
              user.SetPassword(password);
          }
          //MAKE SURE GROUP IS PRESENT
          MakerShop.Users.Group group = GroupDataSource.LoadForName(groupName);
          if (group == null)
          {
              group = new MakerShop.Users.Group();
              group.Name = groupName;
              group.IsReadOnly = true;
              group.Save();
              Response.Write("Group " + groupName + " was not found, created.<br />");
          }
          //MAKE SURE GROUP HAS SUPER USERS PERMISSION
          int roleId = RoleDataSource.GetIdByName(roleName);
          if (roleId > 0)
          {
              if (group.GroupRoles.IndexOf(group.GroupId, roleId) < 0)
              {
                  group.GroupRoles.Add(new GroupRole(group.GroupId, roleId));
                  group.Save();
                  Response.Write("Group " + groupName + " was missing the system role, it has been added.<br />");
              }
          }
          //MAKE SURE USER IS IN GROUP
          user.UserGroups.Add(new UserGroup(user.UserId, group.GroupId));
          user.IsApproved = true;
          user.IsLockedOut = false;
          user.FailedPasswordAttemptCount = 0;
          user.Save();
          Response.Write("Verifid user is a member of group " + groupName + ".<br /><br />");
          //UPDATE MESSAGES
          CompletedText.Visible = true;
          DisabledText.Visible = false;
      }
      else
      {
          CompletedText.Visible = false;
          DisabledText.Visible = true;
      }
      AdminLink.NavigateUrl = NavigationHelper.GetAdminUrl("Default.aspx");
  }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Untitled Page</title>
  </head>
  <body>
    <form id="form1" runat="server">
      <div>
        <asp:Localize
          ID="CompletedText"
          runat="server"
          Text="Admin recovery is complete."
        ></asp:Localize>
        <asp:Localize
          ID="DisabledText"
          runat="server"
          Text="Admin recovery is not enabled!"
        ></asp:Localize
        ><br /><br />
        <asp:HyperLink
          ID="AdminLink"
          runat="server"
          Text="Continue to Admin"
          NavigateUrl=""
        ></asp:HyperLink>
      </div>
    </form>
  </body>
</html>
