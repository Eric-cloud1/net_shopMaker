using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
using MakerShop.Common;
//using MakerShop.Data;

namespace MakerShop.DataClient.Api.ObjectHandlers
{
    public class WarehouseHandler
    {
        public static string ImportWarehouses(MakerShop.DataClient.Api.Schema.Warehouse[] arrWarehouse, string ImportStatus)
        {
            StringBuilder strLog = new StringBuilder("");
            DataObject objDataObject = new DataObject("WAREHOUSES", typeof(MakerShop.Shipping.Warehouse), typeof(Api.Schema.Warehouse));
            
            MakerShop.Shipping.Warehouse objWarehouse = null;
            foreach (Api.Schema.Warehouse objClientApiWarehouse in arrWarehouse)
            {
                objWarehouse = (MakerShop.Shipping.Warehouse)objDataObject.ConvertToAC6Object(objClientApiWarehouse);
                objWarehouse.WarehouseId = 0;
                if (objWarehouse.Save().Equals(SaveResult.Failed))
                {
                    strLog.Append("Warehouse Not Saved\tWarehouse Name=" + objWarehouse.Name + "\n");
                }
            }
            return strLog.ToString();
        }
    }
}
