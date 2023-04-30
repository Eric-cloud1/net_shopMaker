using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MakerShop;
using MakerShop.Catalog;
//using MakerShop.Data;
using MakerShop.Shipping;
using MakerShop.Products;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Payments;
using MakerShop.Taxes;
using MakerShop.DataClient.Api.Schema;
using MakerShop.Utility;
using System.Collections;

namespace MakerShop.DataClient.Api
{
    public class PreLoadRequestHandler
    {
        public byte[] HandleProductsPreload()
        {
            ACResponseForProductsCriteria objResponse = new ACResponseForProductsCriteria();
            objResponse.ResponseForProductsCriteria = getDataForProductsCriteria();
            byte[] byteRequestXml = EncodeHelper.Serialize(objResponse);
            //ResponseXml = EncodeHelper.Utf8BytesToString(byteRequestXml);
            return byteRequestXml;
        }

        public byte[] HandleOrdersPreload()
        {
            OrdersPreloadResponse objResponse = new OrdersPreloadResponse();
            objResponse.OrderStatuses = getPreLoadOrderStatuses();

            byte[] byteRequestXml = EncodeHelper.Serialize(objResponse);                        
            return byteRequestXml;
        }

        public byte[] HandleUsersPreLoad() 
        {
            UsersPreLoadResponse objResponse = new UsersPreLoadResponse();
            objResponse.PreLoadGroups = getPreLoadGroups();

            byte[] byteRequestXml = EncodeHelper.Serialize(objResponse);
            return byteRequestXml;
        }

        private PreLoadGroup[] getPreLoadGroups()
        {
            MakerShop.Users.RoleCollection groups = RoleDataSource.LoadAll();//.LoadForStore();
            PreLoadGroup[] preLoadGroups = null;
            ArrayList groupList = new ArrayList();
            foreach (MakerShop.Users.Role group in groups)
            {
                groupList.Add(group);
            }
            if(groupList.Count > 0)
            {
                preLoadGroups = new PreLoadGroup[groupList.Count];
                PreLoadGroup group = null;
                for (int i = 0; i < groupList.Count;i++ )
                {
                    group = new PreLoadGroup();
                    group.Id = ((MakerShop.Users.Role)groupList[i]).RoleId;
                    group.Name = ((MakerShop.Users.Role)groupList[i]).Name;//.RoleName;
                    preLoadGroups[i] = group;
                }
            }
            return preLoadGroups; 
        }

        private PreLoadOrderStatus[] getPreLoadOrderStatuses()
        {
            MakerShop.Orders.OrderStatusCollection orderStatusCollection = MakerShop.Orders.OrderStatusDataSource.LoadForStore();
            PreLoadOrderStatus[] arrPreLoadOrderStatus = new PreLoadOrderStatus[orderStatusCollection.Count];            
            for(int i = 0; i < orderStatusCollection.Count; i++)
            {
                arrPreLoadOrderStatus[i] = new PreLoadOrderStatus();
                arrPreLoadOrderStatus[i].OrderStatusId = orderStatusCollection[i].OrderStatusId;                                
                arrPreLoadOrderStatus[i].Name = orderStatusCollection[i].Name;
                
            }
            return arrPreLoadOrderStatus;
        }

        private ResponseForProductsCriteria getDataForProductsCriteria()
        {
            ResponseForProductsCriteria obj = new ResponseForProductsCriteria();
            obj.Categories = getCategoriesData();
            obj.TaxCodes = getTaxCodesData();
            obj.Vendors = getVendorsData();
            obj.Manufacturers = getManufacturersData();
            obj.Warehouses = getWarehousesdata();
            obj.ShippableStatus = getShippableStatus();
            return obj;
        }

        private PreLoadShippableStatus[] getShippableStatus()
        {
            int i = 0;
            String[] status = Enum.GetNames(typeof(Shippable));
            PreLoadShippableStatus[] arrPreLoadShippableStatus = new PreLoadShippableStatus[status.Length];
            foreach (String Option in status)
            {
                PreLoadShippableStatus obj = new PreLoadShippableStatus();
                obj.Name=Option;
                obj.ShippableId=AlwaysConvert.ToInt(Enum.Parse(typeof(Shippable),Option,true));
                arrPreLoadShippableStatus[i++] = obj;
            }
            return arrPreLoadShippableStatus;
            
        }

        private PreLoadWarehouse[] getWarehousesdata()
        {
            WarehouseCollection objWarehouseCollection = WarehouseDataSource.LoadForStore();
            PreLoadWarehouse[] arrPreLoadWarehouse = new PreLoadWarehouse[objWarehouseCollection.Count];
            PreLoadWarehouse objPreLoadWarehouse = null;
            int i=0;
            foreach(MakerShop.Shipping.Warehouse objWarehouse in objWarehouseCollection)
            {
                objPreLoadWarehouse = new PreLoadWarehouse();
                objPreLoadWarehouse.Name=objWarehouse.Name;
                objPreLoadWarehouse.WarehouseId=objWarehouse.WarehouseId;

                arrPreLoadWarehouse[i++] = objPreLoadWarehouse;
            }
            return arrPreLoadWarehouse;
        }

        private PreLoadVendor[] getVendorsData()
        {
            VendorCollection objVendorCollection = VendorDataSource.LoadForStore();
            PreLoadVendor[] arrPreLoadVendor = new PreLoadVendor[objVendorCollection.Count];
            PreLoadVendor objPreLoadVendor = null;
            int i = 0;
            foreach (MakerShop.Products.Vendor objVendor in objVendorCollection)
            {
                objPreLoadVendor = new PreLoadVendor();
                objPreLoadVendor.Name = objVendor.Name;
                objPreLoadVendor.VendorId = objVendor.VendorId;

                arrPreLoadVendor[i++] = objPreLoadVendor;
            }
            return arrPreLoadVendor;
        }

        private PreLoadManufacturer[] getManufacturersData()
        {
            ManufacturerCollection objManufacturerCollection = ManufacturerDataSource.LoadForStore();
            PreLoadManufacturer[] arrPreLoadManufacturer = new PreLoadManufacturer[objManufacturerCollection.Count];
            PreLoadManufacturer objPreLoadManufacturer = null;
            int i = 0;
            foreach (MakerShop.Products.Manufacturer objManufacturer in objManufacturerCollection)
            {
                objPreLoadManufacturer = new PreLoadManufacturer();
                objPreLoadManufacturer.Name = objManufacturer.Name;
                objPreLoadManufacturer.ManufacturerId = objManufacturer.ManufacturerId;

                arrPreLoadManufacturer[i++] = objPreLoadManufacturer;
            }
            return arrPreLoadManufacturer;
        }

        private PreLoadTaxCode[] getTaxCodesData()
        {
            TaxCodeCollection objTaxCodeCollection = TaxCodeDataSource.LoadForStore();
            PreLoadTaxCode[] arrPreLoadTaxCode = new PreLoadTaxCode[objTaxCodeCollection.Count];
            PreLoadTaxCode objPreLoadTaxCode = null;
            int i = 0;
            foreach (MakerShop.Taxes.TaxCode objTaxCode in objTaxCodeCollection)
            {
                objPreLoadTaxCode = new PreLoadTaxCode();
                objPreLoadTaxCode.Name = objTaxCode.Name;
                objPreLoadTaxCode.TaxCodeId = objTaxCode.TaxCodeId;

                arrPreLoadTaxCode[i++] = objPreLoadTaxCode;
            }
            return arrPreLoadTaxCode;
        }

        private PreLoadCategory[] getCategoriesData()
        {
            CategoryCollection objCategoryCollection = CategoryDataSource.LoadForStore();
            PreLoadCategory[] arrPreLoadCategory = new PreLoadCategory[objCategoryCollection.Count];
            PreLoadCategory objPreLoadCategory = null;
            int i = 0;
            foreach (MakerShop.Catalog.Category objCategory in objCategoryCollection)
            {
                objPreLoadCategory = new  PreLoadCategory();
                objPreLoadCategory.Name = objCategory.Name;
                objPreLoadCategory.CategoryId = objCategory.CategoryId;
                objPreLoadCategory.ParentId = objCategory.ParentId;

                arrPreLoadCategory[i++] = objPreLoadCategory;
            }
            return arrPreLoadCategory;
        }
    }
}
