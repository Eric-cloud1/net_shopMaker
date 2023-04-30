using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
//using MakerShop.Data;
using MakerShop.Utility;
using System.Text.RegularExpressions;
using System.IO;
using MakerShop.DataClient.Api.Schema;
using MakerShop.DataClient.Api.Schema.Utils;
using MakerShop.DataClient.Csv;
using System.Collections;
using System.Collections.Specialized;
using MakerShop.Common;

namespace MakerShop.DataClient.Api
{
    /// <summary>
    /// Mostly used for CSV import and export
    /// This class will be a composite and will act as datafield as well as the dataobject
    /// </summary>
    public class DataObject:DataObjectField
    {
        public DataObject(String name, Type aC6Type, Type clientType)
        {
            this.name = name;
            this.AC6Type = aC6Type;
            this.clientType = clientType;
        }
                
        public DataObject(Type aC6Type, Type clientType)
        {
            this.name = String.Empty;
            this.AC6Type = aC6Type;
            this.clientType = clientType;
        }

        private List<DataObjectField> fields;

        private SortedDictionary<String, DataField> fieldsDic = null;

        /// <summary>
        /// List of DataField's based on AC Object Properties
        /// </summary>
        public List<DataObjectField> Fields
        {
            get {
                if (fields == null)
                {                    
                    fields = GetFields(AC6Type);                    
                }
                return fields; 
            }            
        }

        /// <summary>
        /// Find a field by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>DataField object if found, other wise null</returns>
        public DataField GetFieldByName(String name)
        {
            // init fieldsDic if not yet initialized
            if (fieldsDic == null)
            {
                fieldsDic = new SortedDictionary<string,DataField>();
                foreach(DataField field in Fields){
                    fieldsDic.Add(field.Name, field);
                }
            }

            if(fieldsDic.ContainsKey(name)){
                return fieldsDic[name];
            }else{
                return null;
            }

        }
        /// <summary>
        /// List of DataField's based on AC Object Properties
        /// </summary>
        /// <param name="AC6Type"></param>
        /// <returns></returns>
        public static List<DataObjectField> GetFields(Type AC6Type)
        {
            // init field based on object and the property mapping files
            List<DataObjectField> fields = new List<DataObjectField>();
            MethodInfo GetColumnNamesInfo = AC6Type.GetMethod("GetColumnNames");
            String[] cNames = GetColumnNamesInfo.Invoke(null, new object[]{String.Empty}).ToString().Split(',');

            BindingFlags flags = ( BindingFlags.Public | BindingFlags.Instance);
            //Get Property Information
            PropertyInfo[] myPropertyInfo = AC6Type.GetProperties(flags);

            foreach (PropertyInfo propertyInfo in myPropertyInfo)
            {
                DataObjectField objField = new DataField();
                String propertyName = String.Empty;
                //Loop for Fields Name
                foreach (String cName in cNames)
                {
                    //if field name and property name are same get it
                    if (cName.ToUpper().Equals(propertyInfo.Name.ToUpper()))
                    {
                        propertyName = propertyInfo.Name;
                        break;
                    }
                }
                //if we found the property then assign it to DataObject
                if (propertyName != string.Empty)
                {
                    //counter++;
                    objField.Name = propertyName;
                    objField.AC6Type = propertyInfo.PropertyType;
                    objField.ClientType = parseClientType(propertyInfo.PropertyType);
                    fields.Add(objField);
                }
                else
                {
                    // property name does not match any column name
                    //Serious: it should not happen                                        
                    //Logger.Error("Object: " + AC6Type.ToString() + " Property Name \'" + propertyInfo.Name + "\' does not match any column name");
                    //throw new Exception("Property Name \'" + propertyName + "\' does not match any column name");
                }
            }
            return fields;
        }

        /// <summary>
        /// Checks wether the specified type is a AC Collection type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>        
        private static bool IsAC6Collection(Type type)
        {
            Type[] baseInterfaces = type.GetInterfaces();
            
            bool isPersistable = false;
            bool isCollection = false;
            foreach (Type t in baseInterfaces)
            {
                if (t == typeof(IPersistable))
                {
                    isPersistable = true;
                }
                if (t == typeof(ICollection))
                {
                    isCollection = true;
                    Type t2 = type.GetGenericTypeDefinition();
                }
            }

            return (isPersistable && isCollection);
        }


        /// <summary>
        /// Convert native data types to compatible xml schema types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type parseClientType(Type type)
        {
            switch (type.ToString())
            {
                case "System.Guid":
                    return typeof(System.String);
                default:
                    return type;
            }
        }

        public object ConvertToClientApiObject(Object srcObj)
        {
            String errorMessage = String.Empty;
            Object retValue = ConvertToClientApiObject(srcObj, out errorMessage);
            if(!String.IsNullOrEmpty( errorMessage)) Logger.Error(errorMessage);
            return retValue;
        }

        /// <summary>
        /// Converts AC object to Schema Object
        /// </summary>
        /// <param name="srcObj">AC6 Type object</param>
        /// <returns>respective Schema Object</returns>
        public object ConvertToClientApiObject(Object srcObj, out String errorMessage)
        {
            errorMessage = "";
            if (srcObj == null)
            {
                return null;
            }
            //Create Instence of the Object to be used to invoke method
            Object destObj = Activator.CreateInstance(ClientType);           

            //Print all data fields and their types
            foreach (DataObjectField objDataField in Fields)
            {                
                PropertyInfo srcProperty = AC6Type.GetProperty(objDataField.Name);
                PropertyInfo destProperty = ClientType.GetProperty(objDataField.Name);                
             
                if (srcProperty == null || destProperty == null)
                {                   

                    // properties should match in all cases, should never be null
                    //Serious: it should not happen         
                    errorMessage += "Error in DataObject.convertToClientApiObject() : \n" +
                        "AC Type: " + AC6Type.ToString() + "  *  " + "Client Type: " + ClientType.ToString() + "\n" +
                        (srcProperty == null ? ("the srcProperty for " + objDataField.Name + " does not exist in AC7\n") : "") +
                        (destProperty == null ? ("the destProperty for " + objDataField.Name + " does not exist in Schema\n") : "");
                    continue;
                }

                // THE VALUE SHOULD NOT BE NULL EXCEPT THE EXPECTED NULLABLE TYPES
                if (srcProperty.GetValue(srcObj, null) == null && !IsNullableType(objDataField.AC6Type))
                {
                    errorMessage += "Error in DataObject.convertToClientApiObject() : \nthe value for srcProperty of " + objDataField.Name + " is null\n";
                }
                
                Console.WriteLine("srcProperty name: " + srcProperty.Name);
                switch (objDataField.AC6Type.ToString())
                {
                    case "System.Guid":
                        destProperty.SetValue(destObj, AlwaysConvert.ToGuid(srcProperty.GetValue(srcObj, null),Guid.Empty).ToString(), null);
                        //destProperty.SetValue(destObj, Guid.Empty.ToString(), null);
                        break;
                    case "System.String":
                        destProperty.SetValue(destObj, AlwaysConvert.ToString(srcProperty.GetValue(srcObj, null)), null);
                        break;
                    case "System.Int32":
                        destProperty.SetValue(destObj, AlwaysConvert.ToInt(srcProperty.GetValue(srcObj, null)), null);
                        break;
                    case "System.Decimal":
                        destProperty.SetValue(destObj, AlwaysConvert.ToDecimal(srcProperty.GetValue(srcObj, null)), null);
                        break;
                    //New Data                        
                    case "MakerShop.Common.LSDecimal":
                        // AS THIS IS MOSTLY USED FOR CURRENCY AMOUNTS TO IT SHOULD BE ROUNDED TO 2 DECIMAL SPACES FOR XML
                        destProperty.SetValue(destObj, Decimal.Round(AlwaysConvert.ToDecimal((LSDecimal)srcProperty.GetValue(srcObj, null)),2), null);                        
                        break;
                    case "System.Boolean":
                        destProperty.SetValue(destObj, AlwaysConvert.ToBool(srcProperty.GetValue(srcObj, null), false), null);
                        break;
                    case "System.Byte":
                        destProperty.SetValue(destObj, AlwaysConvert.ToByte(srcProperty.GetValue(srcObj, null)), null);
                        break;
                    case "System.DateTime":
                        destProperty.SetValue(destObj, AlwaysConvert.ToDateTime(srcProperty.GetValue(srcObj, null), DateTime.Now), null);
                        break;
                    case "System.Int16":
                        destProperty.SetValue(destObj, AlwaysConvert.ToInt16(srcProperty.GetValue(srcObj, null)), null);
                        break;
                    case "System.Int64":
                        destProperty.SetValue(destObj, AlwaysConvert.ToLong(srcProperty.GetValue(srcObj, null)), null);
                        break;
                    case "System.Byte[]":
                        destProperty.SetValue(destObj, (Byte[])srcProperty.GetValue(srcObj, null), null);
                        break;
                    case "MakerShop.Common.UrlEncodedDictionary":
                        destProperty.SetValue(destObj, ((MakerShop.Common.UrlEncodedDictionary)srcProperty.GetValue(srcObj, null)).ToString(), null);
                        break;
                    default:
                        Logger.Error("Error: unable to find match for this, Object:"+ AC6Type.ToString() + ", Property:" + objDataField.AC6Type.ToString());
                        break;
                }
                Console.WriteLine(destProperty.GetValue(destObj, null));
            }
            return destObj;
        }

        private bool IsNullableType(Type type)
        {
            switch (type.ToString())
            {
                case "System.Guid":
                case "System.Byte[]":
                case "MakerShop.Common.UrlEncodedDictionary":
                    return true;                    
                default: return false;
            }
        }

        public static Array ConvertToClientArray(Type acType, Type clientType, CollectionBase srcCollection)
        {
            DataObject dataObject = new DataObject(acType, clientType);
            return dataObject.ConvertAC6Collection(srcCollection);
        }

        /// <summary>
        /// Converts AC Collection to Schema Array
        /// </summary>
        /// <param name="srcCollection"></param>
        /// <returns></returns>
        public Array ConvertAC6Collection(CollectionBase srcCollection)
        {   
            Type srcCollectionType = srcCollection.GetType();
            int count = (int)srcCollectionType.InvokeMember("Count", BindingFlags.GetProperty, null, srcCollection, null);            
            Array arrReturn = Array.CreateInstance(ClientType, count);
            IEnumerator enumerator = srcCollection.GetEnumerator();
            String errorMessage = String.Empty ;
            List<String> errors = new List<string>();
            for (int index = 0; index < count; index++)
            {
                //Move to current object
                enumerator.MoveNext();
                Object srcObject = enumerator.Current;

                //srcCollectionType.InvokeMember("Count", BindingFlags.GetProperty, null, srcCollection, null);
                // MakerShop.Products.ProductReview objNestedObject = objProduct.Reviews[index];
                errorMessage = String.Empty;
                arrReturn.SetValue(ConvertToClientApiObject(srcObject,out errorMessage), index);
                if (!string.IsNullOrEmpty(errorMessage) && !errors.Contains(errorMessage))
                {
                    errors.Add(errorMessage);
                    // LOG THIS ERROR AS WELL
                    Logger.Error(errorMessage);
                }
            }

            return arrReturn;
        }


        /// <summary>
        /// Creates a list of comma saperated id, from an AC Collection type
        /// </summary>
        /// <param name="srcCollection"></param>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static String GetIdsFromCollection(CollectionBase srcCollection, Type fieldType, String fieldName)
        {
            String Ids = String.Empty;
            Type srcCollectionType = srcCollection.GetType();
            int count = (int)srcCollectionType.InvokeMember("Count", BindingFlags.GetProperty, null, srcCollection, null);
            IEnumerator enumerator = srcCollection.GetEnumerator();
            for (int index = 0; index < count; index++)
            {
                //Move to current object
                enumerator.MoveNext();
                Object srcObject = enumerator.Current;
                PropertyInfo srcProperty;

                srcProperty = fieldType.GetProperty(fieldName);
                if (srcProperty == null || srcProperty.GetValue(srcObject, null) == null)
                {

                    // properties should match in all cases, should never be null
                    //Serious: it should not happen                                        
                    Logger.Error("Error in DataObject.GetIdsFromCollection() : \n" +
                        "SrcCollectionType: " + srcCollectionType.FullName + "\n" +
                        (srcProperty == null ? ("the srcProperty for " + fieldName + " is null in " + fieldType.FullName + "\n") : "") +
                        (srcProperty.GetValue(srcObject, null) == null ? ("the value for srcProperty of" + fieldName + " is null in " + fieldType.FullName + "\n") : ""));
                    continue;
                }
                Ids = Ids + srcProperty.GetValue(srcObject, null).ToString() + ",";
                //arrReturn.SetValue(ConvertToClientApiObject(srcObject), index);
            }
            Ids = Utility.RemoveEndSeparator(Ids, ",");
            return Ids;
        }

        /// <summary>
        /// Converts schema objects array to AC Collection
        /// </summary>
        /// <returns></returns>
        public CollectionBase ConvertToAC6Collection(Array srcArray, Type destCollectionType)
        {
            CollectionBase destCollection = (CollectionBase)Activator.CreateInstance(destCollectionType);
            for (int index = 0; index < srcArray.Length; index++)
            {
                Object[] objArgs = { ConvertToAC6Object(srcArray.GetValue(index))};
                //destCollectionType.InvokeMember("Add", BindingFlags.InvokeMethod , null, destCollection, objArgs);

                MethodInfo addInfo = destCollectionType.GetMethod("Add");
                int a = (int)addInfo.Invoke(destCollection, objArgs);
            }
            return destCollection;
        }


        /// <summary>
        /// Converts a Schema Object to corresponding AC object
        /// </summary>
        /// <param name="srcObj"></param>
        /// <returns></returns>
        public IPersistable ConvertToAC6Object(object srcObj)
        {
            //Create Instence of the AC6Object to be used to invoke method             
            Object destObj = Activator.CreateInstance(AC6Type);

            //Print all data fields and their types
            foreach (DataObjectField objDataField in Fields)
            {
                //Console.WriteLine(objDataField.Name + " -------- " + objDataField.AC6Type);                
                PropertyInfo srcProperty = ClientType.GetProperty(objDataField.Name);
                PropertyInfo destProperty = AC6Type.GetProperty(objDataField.Name);
                
                if (srcProperty == null || destProperty == null || srcProperty.GetValue(srcObj, null) == null)
                {
                    // properties should match in all cases, should never be null
                    //Serious: it should not happen     
                    if (srcProperty == null || destProperty == null) 
                        Logger.Error("Error in DataObject.ConvertToAC6Object() : \n" +
                         "AC6Type: " + AC6Type.FullName + "\n" +
                        (srcProperty == null ? ("the srcProperty for " + objDataField.Name + " is null\n") : ("the value for srcProperty of" + objDataField.Name + " is null\n") ) +
                        (destProperty == null ? ("the destProperty for " + objDataField.Name + " is null\n") : ""));
                        //(srcProperty.GetValue(srcObj, null) == null ? ("the value for srcProperty of" + objDataField.Name + " is null\n") : ""));
                    continue;
                }
                if (srcProperty.GetValue(srcObj, null) != null)
                {
                    destProperty.SetValue(destObj, ConvertType(srcProperty.GetValue(srcObj, null), objDataField.AC6Type.ToString()), null);
                }               
            }            
            return (IPersistable)destObj;
        }

        /// <summary>
        /// Updates the AC object fields according to respective Schema object fields
        /// This method will be used to Insert Update but still in confirmed
        /// </summary>
        /// <param name="srcObj">the source object</param>
        /// <param name="destObj">the object that will be updated</param>
        /// <returns></returns>
        public IPersistable UpdateToAC6Object(object srcObj, Object destObj)
        {
            return UpdateToAC6Object(srcObj, destObj, null);
        }
        
        /// <summary>
        /// Updates the AC object fields according to respective Schema object fields
        /// This method will be used to Insert Update but still in confirmed
        /// </summary>
        /// <param name="srcObj">the source object</param>
        /// <param name="destObj">the object that will be updated</param>
        /// <param name="listUpdateFields">List of fields which will be updated, if null all fields will be updated</param>
        /// <returns></returns>
        public IPersistable UpdateToAC6Object(object srcObj, Object destObj, List<string> listUpdateFields)
        {
            //Create Instence of the AC6Object to be used to invoke method             

            //Print all data fields and their types
            foreach (DataObjectField objDataField in Fields)
            {
                // ONLY UPDATE SELECTED FIELDS IF A LIST IS AVAILABLE, OTHERWISE UPDATE ALL FIELDS
                if (listUpdateFields == null || listUpdateFields.Count == 0 || listUpdateFields.Contains(objDataField.Name))
                {
                    PropertyInfo srcProperty = ClientType.GetProperty(objDataField.Name);
                    PropertyInfo destProperty = AC6Type.GetProperty(objDataField.Name);

                    if (srcProperty == null || destProperty == null || srcProperty.GetValue(srcObj, null) == null)
                    {
                        // properties should match in all cases, should never be null
                        //Serious: it should not happen
                        if (srcProperty == null || destProperty == null) 
                            Logger.Error("Error in DataObject.UpdateToAC6Object() : \n" +
                            "AC Type: " + AC6Type.ToString() + "  *  " + "Client Type: " + ClientType.ToString() + "\n" +
                            (srcProperty == null ? ("the srcProperty for " + objDataField.Name + " is null\n") : "") +
                            (destProperty == null ? ("the destProperty for " + objDataField.Name + " is null\n") : "") /* +
                            ((srcProperty == null || srcProperty.GetValue(srcObj, null) == null) ? ("the value for srcProperty of" + objDataField.Name + " is null\n") : "")*/);
                        continue;
                    }
                    if (srcProperty.GetValue(srcObj, null) != null)
                    {
                        destProperty.SetValue(destObj, ConvertType(srcProperty.GetValue(srcObj, null), objDataField.AC6Type.ToString()), null);
                    }
                }
            }

            return (IPersistable)destObj;
        }

        /// <summary>
        /// A mapping of C# types to respective compatible schema types
        /// </summary>
        /// <returns></returns>
        public static StringDictionary  GetTypesDictionary(){
            StringDictionary TypesDictionary = new StringDictionary();
            TypesDictionary.Add("System.String","string");
            TypesDictionary.Add("System.Byte", "unsignedByte");
            TypesDictionary.Add("System.Int16", "short");
            TypesDictionary.Add("System.Int32","int");
            TypesDictionary.Add("System.Int64", "long");
            TypesDictionary.Add("System.Decimal", "decimal");
            TypesDictionary.Add("MakerShop.Common.LSDecimal", "decimal");
            TypesDictionary.Add("System.Boolean","boolean");            
            TypesDictionary.Add("System.DateTime","dateTime");
            TypesDictionary.Add("MakerShop.Common.UrlEncodedDictionary", "string");
            
            return TypesDictionary;
        }


        private static String TempGetType(StringDictionary strd , String type){
            if (strd[type] == null)
            {
                Utility.LogDebug("Unable to find type:" + type);
                return "string";
            }
            else
            {
                return strd[type];
            }
        }


        /// <summary>
        /// Generates the schema for an ac object
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static String GetObjectSchema(Type objectType, String name){
            List<DataObjectField> lstFields = GetFields(objectType);
            
            StringDictionary TypesDictionary = GetTypesDictionary();
            StringBuilder strSchemaType = new StringBuilder();
            strSchemaType.Append("<xs:complexType name=\"" + name + "\">\n");
            strSchemaType.Append("\t<xs:sequence>\n");
            foreach (DataField field in lstFields)
            {
                strSchemaType.Append("\t\t<xs:element name=\"" + field.Name + "\" type=\"xs:" + TempGetType(TypesDictionary,field.AC6Type.ToString()) + "\" />\n");
            }

            strSchemaType.Append("\t</xs:sequence>\n");
            strSchemaType.Append("</xs:complexType>\n");
            return strSchemaType.ToString();
        }

        /// <summary>
        /// Tries to Converts types according to the specified type, if fails returns default values as specified in AlwayConvert 
        /// </summary>
        /// <param name="srcObject"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object ConvertType(Object srcObject, String type)
        {
            switch (type)
            {
                case "System.String":
                    return AlwaysConvert.ToString(srcObject);
                    
                case "System.Int32":
                    return AlwaysConvert.ToInt(srcObject);
                    
                case "System.Decimal":
                    return AlwaysConvert.ToDecimal(srcObject);
                    
                case "System.Boolean":
                    return AlwaysConvert.ToBool(srcObject, false);
                    
                case "System.Byte":
                    return AlwaysConvert.ToByte(srcObject);
                    
                case "System.DateTime":
                    return AlwaysConvert.ToDateTime(srcObject, DateTime.Now);
                    
                case "System.Int16":
                    return AlwaysConvert.ToInt16(srcObject);
                case "MakerShop.Common.LSDecimal":
                    return (LSDecimal)AlwaysConvert.ToDecimal(srcObject);
                case "MakerShop.Common.UrlEncodedDictionary":
                    return Utility.ParseToUrlEncodedDictionary((string)srcObject);
                default:
                    return null;
            }
        }

        //Included after fix
        public Object ConvertTypeForClient(Object srcObject, String type)
        {
            switch (type)
            {
                case "System.String":
                    return AlwaysConvert.ToString(srcObject);

                case "System.Int32":
                    return AlwaysConvert.ToInt(srcObject);

                case "System.Decimal":
                    return AlwaysConvert.ToDecimal(srcObject);

                case "System.Boolean":
                    return AlwaysConvert.ToBool(srcObject, false);

                case "System.Byte":
                    return AlwaysConvert.ToByte(srcObject);

                case "System.DateTime":
                    return AlwaysConvert.ToDateTime(srcObject, DateTime.Now);

                case "System.Int16":
                    return AlwaysConvert.ToInt16(srcObject);
                case "MakerShop.Common.LSDecimal":
                    return AlwaysConvert.ToDecimal(srcObject);
                default:
                    return null;
            }
        }



        /// <summary>
        /// Generates Default Template according to given parameters
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AC6Type"></param>
        /// <param name="ClientType"></param>
        /// <returns></returns>
        public static ImportTemplate DefaultTemplete(String Name, Type AC6Type, Type ClientType)
        {
            object srcObject = Activator.CreateInstance(ClientType);
            MethodInfo objMethodInfo = ClientType.GetMethod("GetCSVEnabledFields");
            String[] Fields = (String[])objMethodInfo.Invoke(null, new object[] { String.Empty });
            ImportTemplate objImportTemplate = new ImportTemplate();
            objImportTemplate.TemplateObject = new TemplateObject();
            objImportTemplate.TemplateObject.Name = Name;
            objImportTemplate.TemplateObject.AC6Type = AC6Type.ToString();
            objImportTemplate.TemplateObject.ClientType = ClientType.ToString();
            objImportTemplate.TemplateObject.Fields = new Field[Fields.Length];
            
            //Data Qualification and other
            objImportTemplate.TemplateObject.TextDelimiter = ",";
            objImportTemplate.TemplateObject.TextQualifier = "\"";

            Schema.Utils.Field objField = null;
            int i = 0;
            foreach (String objDataField in Fields)
            {
                objField = new Field();
                objField.Name = objDataField;
                objField.MappedName = objDataField;
                objImportTemplate.TemplateObject.Fields[i++] = objField;
            }
            return objImportTemplate;
        }
        

        /// <summary>
        /// NAMES OF AC7 PARENT OBJECTS IN IMPORT ORDER
        /// </summary>
        public enum Names
        {
            //NOTE: Changing the names order will directly effect the import process
            Affiliates,
            BannedIPs,
            Currencies,
            Readmes,
            LicenseAgreements,
            ErrorMessages,
            StoreSettings,
            PaymentGateways,
            ShipGateways,
            TaxGateways,
            Manufacturers,
            TaxCodes,
            Roles,
            EmailTemplates,
            PersonalizationPaths,
            Countries,
            Groups,
            Warehouses,
            TaxRules,
            VolumeDiscounts,
            Vendors,
            WrapGroups,
            ProductTemplates,
            Categories,
            Links,
            Webpages,
            PaymentMethods,
            OrderStatuses,            
            ShipZones,
            ShipMethods,            
            DigitalGoods,
            Users,
            EmailLists,
            AuditEvents,
            Products,
            KitComponents,            
            Coupons,
            CustomFields,
            PageViews,
            Orders,
            GiftCertificates
        }


        internal static Object ConvertToClientObject(Type acType, Type clientType, Object acObject)
        {
            DataObject dataObject = new DataObject(acType, clientType);
            return dataObject.ConvertToClientApiObject(acObject);
        }

        internal static IPersistable ConvertToAcObject(Object schemaObj, Type acType)
        {
            DataObject dataObject = new DataObject(acType, schemaObj.GetType());
            return dataObject.ConvertToAC6Object(schemaObj);
        }
    }

}
