﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 
namespace MakerShop.Web.SiteMap
{
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.sitemaps.org/schemas/sitemap/0.9", IsNullable=false)]
    public partial class sitemapindex {
        
        private sitemap[] sitemapField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sitemap")]
        public sitemap[] sitemap {
            get {
                return this.sitemapField;
            }
            set {
                this.sitemapField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.sitemaps.org/schemas/sitemap/0.9", IsNullable=false)]
    public partial class sitemap {
        
        private string locField;
        
        private string lastmodField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI")]
        public string loc {
            get {
                return this.locField;
            }
            set {
                this.locField = value;
            }
        }
        
        /// <remarks/>
        public string lastmod {
            get {
                return this.lastmodField;
            }
            set {
                this.lastmodField = value;
            }
        }
    }
}
