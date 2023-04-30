using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
using MakerShop.Orders;
using MakerShop.Shipping.Providers.UPS;
using MakerShop.Utility;

namespace MakerShop.DataClient.Api
{
    class UpsWsImporter
    {
        internal static byte[] ImportData(MakerShop.DataClient.Api.Schema.UpsWsImportRequest upsWsImportRequest)
        {           
            ACImportResponse objACImportResponse = new ACImportResponse();
            objACImportResponse.ImportResponse = new ImportResponse();
            objACImportResponse.ImportResponse.Log = new Log();

            String upsProviderClassId = Misc.GetClassId(typeof(MakerShop.Shipping.Providers.UPS.UPS));

            StringBuilder logMessage = new StringBuilder();
            ShipmentTrackingRecord[] trackingRecords =  upsWsImportRequest.ShipmentTrackingRecords;

            int updated = 0, deleted = 0,  added = 0;

            if (trackingRecords != null && trackingRecords.Length > 0)
            {
                logMessage.AppendLine("Found a total of " + trackingRecords.Length + " Records...");
                foreach (ShipmentTrackingRecord record in trackingRecords)
                {                    
				    //validate that the shipment belongs to this store
				    Orders.OrderShipment  shipment = OrderShipmentDataSource.Load(record.ShipmentId);
                    if (shipment != null)
                    {

                        //the shipment is valid, check for existing tracking record

                        int intIndex = -1;
                        int j = 0;
                        while (intIndex < 0 && j < shipment.TrackingNumbers.Count)
                        {
                            Orders.TrackingNumber objTracking = shipment.TrackingNumbers[j];
                            if (objTracking.TrackingNumberData == record.TrackingNumber)
                            {
                                if (objTracking.ShipGateway.ClassId == upsProviderClassId)
                                {
                                    intIndex = j;
                                }
                            }
                            j++;
                        }

                        if (record.Void)
                        {
                            if (intIndex > -1)
                            {
                                shipment.TrackingNumbers.DeleteAt(intIndex);
                                deleted++;
                            }
                        }
                        else
                        {
                            //add tracking number if not found
                            if (intIndex < 0)
                            {
                                Orders.TrackingNumber trackingNumber = new Orders.TrackingNumber();
                                trackingNumber.TrackingNumberData = record.TrackingNumber;
                                trackingNumber.OrderShipmentId = record.ShipmentId;
                                trackingNumber.ShipGatewayId = shipment.ShipMethod.ShipGatewayId;

                                shipment.TrackingNumbers.Add(trackingNumber);
                                added++;
                            }
                            //mark shipped if not already 
                            if (!shipment.IsShipped) shipment.Ship();                            
                        }
                    }
                }
            }
            else
            {
                objACImportResponse.ImportResponse.Log.Error = new Error();
                objACImportResponse.ImportResponse.Log.Error.Message = "UPS Worldship data was empty";
            }

            logMessage.AppendLine(updated + " records updated. ").AppendLine(deleted + " records deleted.");
            objACImportResponse.ImportResponse.Log.Message = logMessage.ToString();
            objACImportResponse.ImportResponse.ResponseId = upsWsImportRequest.RequestId;
            byte[] byteRequestXml = EncodeHelper.Serialize(objACImportResponse);
            return byteRequestXml;
        }
    }
}
