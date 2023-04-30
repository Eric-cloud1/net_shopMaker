using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MakerShop;
//using MakerShop.Data;
using MakerShop.Shipping;
using MakerShop.Products;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Catalog;
using MakerShop.Users;
using MakerShop.Payments;
using MakerShop.DataClient.Api.Schema;
using System.Text.RegularExpressions;
using MakerShop.DataClient.Api.ObjectHandlers;
using MakerShop.Utility;
using MakerShop.Common;
using System.Xml;
using System.Reflection;


namespace MakerShop.DataClient.Api
{
    public class ACRequestHandler
    {
        static ExportHandler exportHandler = new ExportHandler();

        public static Byte[] ProcessRequest(String requestXml)
        {
            //Disable the Caching
            Token.Instance.EnableRequestCaching = false;
            
            Utility.LogDebug(EncodeHelper.GetTopElement(requestXml) + " Starting");            

            byte[] response = null;
            try
            {
                //Validate User
                // Process the incoming XML.
                // check the type of request
                switch (EncodeHelper.GetTopElement(requestXml))
                {
                    //invoke appropriate method(s)                    
                    case "ACExportRequest":
                        ACExportRequest objACExportRequest = ((ACExportRequest)EncodeHelper.Deserialize(requestXml, typeof(ACExportRequest)));
                        exportHandler.RequestId = objACExportRequest.StandardExportRequest.RequestId;
                        int chunkSize = objACExportRequest.StandardExportRequest.ExportOptions.ChunkSize;
                        //if (objACExportRequest.StandardExportRequest.ExportOptions.Data.Equals("ExportAll"))
                        //{
                            response = exportHandler.DoStandardExport(objACExportRequest.StandardExportRequest.ExportOptions,chunkSize);
                        //}
                        //else
                        //{
                        //    response = exportHandler.DoSelectedStandardExport(objACExportRequest.StandardExportRequest.ExportOptions.Data, objACExportRequest.StandardExportRequest.ExportOptions.ChunkSize);                            
                        //}                       
                        break;
                    case "ACCustomizedProductRequest":
                        ACCustomizedProductRequest objProductRequest = ((ACCustomizedProductRequest)EncodeHelper.Deserialize(requestXml, typeof(ACCustomizedProductRequest)));
                        exportHandler.RequestId = objProductRequest.RequestId;
                        response = exportHandler.DoCustomizedProductsExport(objProductRequest);
                        break;
                    case "ACCustomizedOrderRequest":
                        ACCustomizedOrderRequest objOrdersRequest = ((ACCustomizedOrderRequest)EncodeHelper.Deserialize(requestXml, typeof(ACCustomizedOrderRequest)));
                        exportHandler.RequestId = objOrdersRequest.RequestId;
                        response = exportHandler.DoCustomizedOrdersExport(objOrdersRequest);
                        break;
                    case "ACCustomizedUserRequest":
                        ACCustomizedUserRequest objUsersRequest = ((ACCustomizedUserRequest)EncodeHelper.Deserialize(requestXml, typeof(ACCustomizedUserRequest)));
                        exportHandler.RequestId = objUsersRequest.RequestId;
                        response = exportHandler.DoCustomizedUsersExport(objUsersRequest);
                        break;
                    case "ACRequestForProductsCriteria":
                        PreLoadRequestHandler H2 = new PreLoadRequestHandler();
                        response = H2.HandleProductsPreload();
                        break;
                    case "ACRequestForOrdersCriteria":

                        break;
                    case "ACRequestForUsersCriteria":
                        break;
                    case "ACImportRequest":
                        ACImportRequest ImpRequest = ((ACImportRequest)EncodeHelper.Deserialize(requestXml, typeof(ACImportRequest)));
                        ImportHandler objImportHandler = new ImportHandler(ImpRequest.ImportRequest);
                        //Utility.LogDebug("Going to start import...");
                        response = objImportHandler.ImportStandardData();
                        break;
                    case "AC5xImportRequest":
                        AC5xImportRequest aC5xImportRequest = ((AC5xImportRequest)EncodeHelper.Deserialize(requestXml, typeof(AC5xImportRequest)));                        
                        //Utility.LogDebug("Starting AC5x Import...");
                        response = ImportHandler.ImportAC5xData(aC5xImportRequest);
                        break;                        
                    case "OrdersPreLoadRequest":
                        PreLoadRequestHandler H3 = new PreLoadRequestHandler();
                        response = H3.HandleOrdersPreload();
                        break;
                    case "UsersPreLoadRequest":
                        PreLoadRequestHandler H4 = new PreLoadRequestHandler();
                        response = H4.HandleUsersPreLoad();
                        break;
                    case "ACAuthenticationRequest":
                        ACAuthenticationResponse H5 = new ACAuthenticationResponse();
                        H5.InitVersionInfo();
                        H5.Log = new Log();
                        H5.Log.Message = "Request authorized.";
                        String fieldLengthWarnings = ACDataSource.CheckFieldLengths(false);
                        if (!String.IsNullOrEmpty(fieldLengthWarnings)) H5.Log.WarningMessages = fieldLengthWarnings;

                        
                        response = EncodeHelper.Serialize(H5);
                        break;
                    case "UpsWsImportRequest":
                        UpsWsImportRequest upsWsImportRequest = ((UpsWsImportRequest)EncodeHelper.Deserialize(requestXml, typeof(UpsWsImportRequest)));                        
                        response = UpsWsImporter.ImportData(upsWsImportRequest);
                        break;
                    case "ProductVariantsExportRequest":
                        ProductVariantsExportRequest productVariantsExportRequest = (ProductVariantsExportRequest)EncodeHelper.Deserialize(requestXml, typeof(ProductVariantsExportRequest));
                        exportHandler.RequestId = productVariantsExportRequest.RequestId;
                        response = ExportHandler.ExportProductVariants(productVariantsExportRequest);
                        break;
                    case "ProductVariantsImportRequest":
                        ProductVariantsImportRequest productVariantsImportRequest = (ProductVariantsImportRequest)EncodeHelper.Deserialize(requestXml, typeof(ProductVariantsImportRequest));
                        response = VariantsCsvImporter.Import(productVariantsImportRequest);
                        break;
                    case "UpdateDatabaseFieldsRequest":
                        UpdateDatabaseFieldsRequest updateDatabaseFieldsRequest = (UpdateDatabaseFieldsRequest)EncodeHelper.Deserialize(requestXml, typeof(UpdateDatabaseFieldsRequest));
                        UpdateDatabaseFieldsResponse updateDatabaseFieldsResponse = new UpdateDatabaseFieldsResponse();
                        updateDatabaseFieldsResponse.RequestId = updateDatabaseFieldsRequest.RequestId;
                        updateDatabaseFieldsResponse.LogMessage = ACDataSource.CheckFieldLengths(true);
                        response = EncodeHelper.Serialize(updateDatabaseFieldsResponse);
                        break;
                    default:
                        break;
                }
                Utility.LogDebug(EncodeHelper.GetTopElement(requestXml) + " Success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message,ex);
                throw ex;
            }
            return response;
        }
    }
}
