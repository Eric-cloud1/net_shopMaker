<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CatalogTree.ascx.cs" Inherits="ConLib_CatalogTree" EnableViewState="false"%>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%-- 
<conlib> 
<summary>Displays the catalog of your store in a treeview.</summary> 
<param name="ShowStoreNode" default="true">If 'true' then will show the name of the store as root node.</param> 
<param name="RootNodeText" default="Store Name">Text shown for root node, usualy the name of the store.</param> 
<param name="ShowAllNodes" default="false">If true will also display products, webpages and links.</param> 
<param name="PreExpandLevel" default="0">Represents up to which level the tree will be pre-expanded on first load.</param> 
</conlib> 
--%>
<componentart:treeview id="TreeView1" runat="server" enableviewstate="true">    
</componentart:treeview>

