using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api
{
    public static class ApiUtility
    {
        const string AuthRequestElement = "ACAuthenticationRequest";
        const string AuthResponseElement = "ACAuthenticationResponse";

        const string ExportRequestElement = "ACExportRequest";
        const string ExportResponseElement = "ACExportResponse";

        const string ImportRequestElement = "ACImportRequest";
        const string ImportResponseElement = "ACImportResponse";

        const string OrdersPreloadRequestElement = "OrdersPreloadRequest";
        const string OrdersPreloadResponseElement = "OrdersPreloadResponse";

        const string ProductsPreLoadRequestElement = "ACRequestForProductsCriteria";
        const string ProductsPreLoadResponseElement = "ACResponseForProductsCriteria";

        const string UsersPreLoadRequestElement = "UsersPreLoadRequest";
        const string UsersPreLoadResponseElement = "UsersPreLoadResponse";

        const string ACImportResponseElement = "ACImportResponse";

        const string ProductVariantsExportRequest = "ProductVariantsExportRequest";
        const string ProductVariantsExportResponse = "ProductVariantsExportResponse";
        const string ProductVariantsImportRequest = "ProductVariantsImportRequest";
        const string ProductVariantsImportResponse = "ProductVariantsImportResponse";
        
        public static bool IsAuthenticationRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(AuthRequestElement);
        }

        public static bool IsAuthenticationResponse(string data)
        {
            if(string.IsNullOrEmpty(data)) return false;
            try
            {
                return EncodeHelper.GetTopElement(data).Equals(AuthResponseElement);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsExportRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ExportRequestElement);
        }

        public static bool IsExportResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ExportResponseElement);
        }

        public static bool IsImportRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ImportRequestElement);
        }

        public static bool IsImportResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ImportResponseElement);
        }

        public static bool IsOrdersPreloadRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(OrdersPreloadRequestElement);
        }

        public static bool IsOrdersPreloadResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(OrdersPreloadResponseElement);
        }

        public static bool IsProductsPreloadRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ProductsPreLoadRequestElement);
        }

        public static bool IsProductsPreloadResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ProductsPreLoadResponseElement);
        }

        public static bool IsUsersPreloadRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(UsersPreLoadRequestElement);
        }

        public static bool IsUsersPreloadResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(UsersPreLoadResponseElement);
        }

        public static bool IsAC5xImportResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ACImportResponseElement);
        }

        public static bool IsProductVariantsExportRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ProductVariantsExportRequest);
        }
        public static bool IsProductVariantsExportResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ProductVariantsExportResponse);
        }
        public static bool IsProductVariantsImportRequest(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ProductVariantsImportRequest);
        }
        public static bool IsProductVariantsImportResponse(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return EncodeHelper.GetTopElement(data).Equals(ProductVariantsImportResponse);
        }

    }
}
