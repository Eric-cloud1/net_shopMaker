<%@ Page Language="C#" MasterPageFile="../../Admin.master"
CodeFile="Default.aspx.cs" Inherits="Admin_Taxes_Providers_Default" Title="Third
Party Providers" %> <%@ Import Namespace="MakerShop.Taxes.Providers" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Third Party Providers"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <div class="section">
    <div class="content">
      <asp:Localize ID="ThirdPartyIntroText" runat="server">
        MakerShop provides a built in tax calculator that uses a combination of
        tax codes and tax rules to calculate tax liability. However, alternate
        providers can be developed to use calculation services like Avalara.
      </asp:Localize>
      <asp:Localize ID="ThirdPartyHelpText" runat="server">
        You can configure third party providers in addition to or in lieu of the
        built-in tax rules. The installed third party tax providers are listed
        below:
      </asp:Localize>
      <asp:PlaceHolder ID="ProviderPanel" runat="server">
        <br /><br />
        <asp:UpdatePanel ID="ProviderAjax" runat="server">
          <ContentTemplate>
            <asp:GridView
              ID="ProviderGrid"
              runat="server"
              AutoGenerateColumns="false"
              Width="100%"
              SkinID="PagedList"
              OnRowCommand="ProviderGrid_RowCommand"
            >
              <Columns>
                <asp:TemplateField>
                  <ItemStyle
                    HorizontalAlign="Center"
                    Width="80px"
                    Wrap="false"
                  />
                  <ItemTemplate>
                    <asp:ImageButton
                      ID="AddButton"
                      runat="server"
                      SkinID="AddIcon"
                      AlternateText="Configure Provider"
                      CommandName="AddProvider"
                      CommandArgument="<%#GetClassId(Container.DataItem)%>"
                      Visible="<%#!IsConfigured(Container.DataItem)%>"
                    />
                    <asp:PlaceHolder
                      ID="ConfiguredButtons"
                      runat="server"
                      Visible="<%#IsConfigured(Container.DataItem)%>"
                    >
                      <asp:Image
                        ID="AlreadyAdded"
                        runat="server"
                        SkinID="CodeGreen"
                        ToolTip="Already Added"
                      />
                      <asp:HyperLink
                        ID="EditButton"
                        runat="server"
                        NavigateUrl="<%#GetConfigUrl(Container.DataItem)%>"
                        Visible="<%#HasConfigUrl(Container.DataItem)%>"
                        ><asp:Image
                          ID="EditIcon"
                          runat="server"
                          SkinID="EditIcon"
                          ToolTip="Edit"
                      /></asp:HyperLink>
                      <asp:ImageButton
                        ID="DeleteButton"
                        runat="server"
                        CausesValidation="False"
                        CommandName="DeleteProvider"
                        CommandArgument="<%#GetClassId(Container.DataItem)%>"
                        SkinID="DeleteIcon"
                        OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete the configuration data for {0}?\")") %>'
                        AlternateText="Delete"
                      />
                    </asp:PlaceHolder>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Provider">
                  <HeaderStyle HorizontalAlign="left" />
                  <ItemStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <asp:Label
                      ID="Name"
                      runat="server"
                      Text='<%#Eval("Name")%>'
                      SkinID="FieldHeader"
                    ></asp:Label
                    ><br />
                    <asp:Label
                      ID="ProviderDescription"
                      runat="server"
                      Text='<%#Eval("Description")%>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:PlaceHolder
                      ID="LogoPanel"
                      runat="server"
                      Visible="<%#HasLogo(Container.DataItem)%>"
                    >
                      <asp:HyperLink
                        ID="ProviderLink"
                        runat="server"
                        NavigateUrl="<%#GetConfigUrl(Container.DataItem)%>"
                      >
                        <asp:Image
                          ID="ProviderLogo"
                          runat="server"
                          ImageUrl="<%#GetLogoUrl(Container.DataItem)%>"
                        /> </asp:HyperLink
                      ><br />
                    </asp:PlaceHolder>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
            </asp:GridView>
          </ContentTemplate>
        </asp:UpdatePanel>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="NoProvidersPanel" runat="server">
        <br /><br />
        There are no third party tax providers currently installed.
        <br /><br />
      </asp:PlaceHolder>
      <asp:Localize ID="Localize1" runat="server">
        For more information on alternate tax providers, check our
        <a href="http://forums.MakerShop.com" target="_blank"
          >community forum</a
        >
        or our
        <a
          href="http://wiki.MakerShop.com/index.php/Integrating_A_Tax_Provider"
          target="_blank"
          >developer resources</a
        >.<br /><br />
      </asp:Localize>
    </div>
  </div>
</asp:Content>
