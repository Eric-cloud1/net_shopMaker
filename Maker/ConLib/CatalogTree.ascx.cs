using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ComponentArt.Web.UI;
using MakerShop.Catalog;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Products;

public partial class ConLib_CatalogTree : System.Web.UI.UserControl
{
    

    String _RootNodeText = Token.Instance.Store.Name;
    [Personalizable,WebBrowsable]
    public String RootNodeText
    {
        get { return _RootNodeText; }
        set { _RootNodeText = value; }
    }
    
    bool _ShowRootNode = false;
    [Personalizable,WebBrowsable]
    public bool ShowRootNode
    {
        get { return _ShowRootNode; }
        set { _ShowRootNode = value; }
    }

    bool _ShowAllNodes = false;
    [Personalizable, WebBrowsable]
    public bool ShowAllNodes
    {
        get { return _ShowAllNodes; }
        set { _ShowAllNodes = value; }
    }

    int _PreExpandLevel = 1;
    [Personalizable, WebBrowsable]
    public int PreExpandLevel
    {
        get { return _PreExpandLevel; }
        set {
            if (value < 0) value = 0;
            _PreExpandLevel = value; 
        }
    }   
   

    protected void Page_Init(object sender, EventArgs e) 
    {       
        InitializeCategoryTree();
    } 

    private void InitializeCategoryTree() 
    {
        GetCategoryTreeNodes();
        
        // Specify the current category 
        int categoryId = PageHelper.GetCategoryId();
        TreeViewNode node = TreeView1.FindNodeById(categoryId.ToString());
        if (node != null)
        {
            node.ServerTemplateId = "SelectedCategoryTemplate";
            node.Expanded = true;
            EnsureTreeNodeVisible(node);
        }
    } 

    private void GetCategoryTreeNodes() 
    { 
        string categoryNodes = null; 

       
        if (ShowRootNode)
        {
            if (categoryNodes == null)
            {
                // Start with the application root folder         
                ComponentArt.Web.UI.TreeViewNode rootNode = new ComponentArt.Web.UI.TreeViewNode();
                rootNode.Text = RootNodeText;
                rootNode.ImageUrl = "category.gif";
                rootNode.Expanded = true;
                rootNode.ID = "CatalogRoot";
                AddSubNodes(rootNode, 0);
                
                TreeView1.Nodes.Add(rootNode);
                TreeView1.SelectedNode = TreeView1.Nodes[0];
              
            }
        }
        else
        {

            InitRootCategories(TreeView1.Nodes);
        }
    }

    private void EnsureTreeNodeVisible(TreeViewNode node)
    {
        TreeViewNode currentNode = node.ParentNode;

        while (currentNode != null)
        {
            currentNode.Expanded = true;
            currentNode = currentNode.ParentNode;
        }
    }
    
    // Builds a TreeView level containing the list of sub-categories of the root level   
    public void InitRootCategories(TreeViewNodeCollection nodes)
    {
        ComponentArt.Web.UI.TreeViewNode node;
        CategoryCollection categoreis = CategoryDataSource.LoadForParent(0,true);
        int currentLevel = ShowRootNode ? 1 : 0;
        foreach (Category category in categoreis)
        {           
            node = new ComponentArt.Web.UI.TreeViewNode();
            node.ID = "Category_" + category.CategoryId.ToString();
            node.Text = category.Name;
            node.NavigateUrl = category.NavigateUrl;
            node.ImageUrl = "category.gif";
            if (currentLevel < PreExpandLevel)
            {
                node.Expanded = true;
                AddSubNodes(node, 1);
            }
            else
            {
                node.Expanded = false;
                node.ContentCallbackUrl = "~/CatalogTreeXmlGenerator.ashx?CategoryId=" + category.CategoryId.ToString() +  "&ShowAllNodes=" + ShowAllNodes;
            }            
            nodes.Add(node);            
        }
    }


    /// <summary>
    /// Will add sub-nodes to the parent node recursivly upto PreExpandLevel
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="currentLevel"></param>
    protected void AddSubNodes(TreeViewNode parentNode, int currentLevel)
    {
        ComponentArt.Web.UI.TreeViewNode node;
        int rootCategoryId = AlwaysConvert.ToInt(parentNode.ID.Substring(9));
        
        CatalogNodeCollection catalogNodes = null;
        if (!_ShowAllNodes)
        {
            catalogNodes = CatalogNodeDataSource.LoadForCriteria("CategoryId = " + rootCategoryId + " And CatalogNodeTypeId = " + (int)CatalogNodeType.Category);
        }
        else
        {
            catalogNodes = CatalogNodeDataSource.LoadForCategory(rootCategoryId);
        }

        foreach (CatalogNode catalogNode in catalogNodes)
        {
            // SKIP NON-PUBLIC NODES
            if (catalogNode.Visibility != CatalogVisibility.Public) continue;

            switch (catalogNode.CatalogNodeType)
            {
                case CatalogNodeType.Category:
                    Category category = ((Category)catalogNode.ChildObject);
                    if (category != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Category_" + category.CategoryId.ToString();
                        node.Text = category.Name;
                        node.NavigateUrl = category.NavigateUrl;
                        node.ImageUrl = "category.gif";

                        if (currentLevel < PreExpandLevel)
                        {
                            node.Expanded = true;
                            AddSubNodes(node, currentLevel + 1);
                        }
                        else
                        {
                            node.Expanded = false;
                            node.ContentCallbackUrl = "~/CatalogTreeXmlGenerator.ashx?CategoryId=" + category.CategoryId.ToString() + "&ShowAllNodes=" + ShowAllNodes;
                        }                        
                        parentNode.Nodes.Add(node);
                    }
                    break;
                case CatalogNodeType.Product:
                    Product product = ((Product)catalogNode.ChildObject);
                    if (product != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Product_" + product.ProductId.ToString();
                        node.Text = product.Name;
                        node.NavigateUrl = product.NavigateUrl;
                        node.ImageUrl = "product.gif";
                        parentNode.Nodes.Add(node);
                    }
                    break;
                case CatalogNodeType.Webpage:
                    Webpage webpage = ((Webpage)catalogNode.ChildObject);
                    if (webpage != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Webpage_" + webpage.WebpageId.ToString();
                        node.Text = webpage.Name;
                        node.NavigateUrl = webpage.NavigateUrl;
                        node.ImageUrl = "webpage.gif";
                        parentNode.Nodes.Add(node);
                    }
                    break;

                case CatalogNodeType.Link:
                    Link link = ((Link)catalogNode.ChildObject);
                    if (link != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Link_" + link.LinkId.ToString();
                        node.Text = link.Name;
                        node.NavigateUrl = link.NavigateUrl;
                        node.ImageUrl = "link.gif";
                        parentNode.Nodes.Add(node);
                    }
                    break;
                default: break;
            }
        }
    }
}
