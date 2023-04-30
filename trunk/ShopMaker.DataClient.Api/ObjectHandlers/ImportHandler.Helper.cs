using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
//using MakerShop.Data;
using System.Collections.Specialized;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Users;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.DataClient.Api.ObjectHandlers;
using MakerShop.Shipping;
using System.Windows.Forms;
using MakerShop.Stores;
using System.Xml;
using System.Web;
using System.IO;
using System.Collections;
using System.Reflection;
using MakerShop.Taxes;
using MakerShop.Messaging;
using MakerShop.Payments;
using MakerShop.Common;


namespace MakerShop.DataClient.Api.ObjectHandlers
{    
    public partial class ImportHandler
    {
        // PRIVATE IMPORT HELPER METHODS
        private delegate object TryLoadExistingObject(Object schemaObject);
        private delegate void TranslateObjectAssociatedIds(ref Object acObject, Object schemaObject);
        private delegate SaveResult SaveACObject(ref Object acObject);
        private delegate void ImportObjectChildsAndAssociations(ref Object acObject, Object schemaObject);
        private delegate void ImportDynamicAssociationObjectData(ref Object acAssociationObject, Object schemaAssociationObject);

        // DEFAULT DEFINITION FOR CALLING SAVE METHOD
        SaveACObject defaultSaveACObject = delegate(ref object acObject)
        {
            Type acType = acObject.GetType();
            MethodInfo saveInfo = acType.GetMethod("Save", new Type[0]);
            SaveResult saveResult = SaveResult.Failed;
            if (saveInfo != null)
                saveResult = (SaveResult)saveInfo.Invoke(acObject, null);
            else
                Logger.Debug("DataPort Import error: In Default Save method delegate, the Save() method not found for " + acType.ToString());
            return saveResult;
        };        

        private void ImportObjectsArray(Array schemaObjectsArray, Type acType, String objName, String idFieldName, String displayNamePropertyName,  TryLoadExistingObject tryLoadExistingObject, TranslateObjectAssociatedIds translateObjectAssociatedIds, ImportObjectChildsAndAssociations importObjectChildsAndAssociations)
        {

            ImportObjectsArray(schemaObjectsArray, acType, objName, idFieldName, displayNamePropertyName, tryLoadExistingObject, translateObjectAssociatedIds,defaultSaveACObject, importObjectChildsAndAssociations, string.Empty);
        }

        private void ImportObjectsArray(Array schemaObjectsArray, Type acType, String objName, String idFieldName, String displayNamePropertyName, TryLoadExistingObject tryLoadExistingObject, TranslateObjectAssociatedIds translateObjectAssociatedIds, SaveACObject saveACObject, ImportObjectChildsAndAssociations importObjectChildsAndAssociations, string csvFields)
        {
            ImportChildObjectsArray(schemaObjectsArray, acType, objName, idFieldName, displayNamePropertyName, 0, tryLoadExistingObject, translateObjectAssociatedIds, saveACObject, importObjectChildsAndAssociations, csvFields);
        }

        private void ImportObjectsArray(Array schemaObjectsArray, Type acType, String objName, String idFieldName, String displayNamePropertyName, TryLoadExistingObject tryLoadExistingObject, TranslateObjectAssociatedIds translateObjectAssociatedIds,  ImportObjectChildsAndAssociations importObjectChildsAndAssociations, string csvFields)
        {
            ImportChildObjectsArray(schemaObjectsArray, acType, objName, idFieldName, displayNamePropertyName, 0, tryLoadExistingObject, translateObjectAssociatedIds, defaultSaveACObject, importObjectChildsAndAssociations, csvFields);
        }

        private void ImportChildObjectsArray(Array schemaObjectsArray, Type acType, String objName, String idFieldName, String displayNamePropertyName, object parentId, TryLoadExistingObject tryLoadExistingObject, TranslateObjectAssociatedIds translateObjectAssociatedIds, ImportObjectChildsAndAssociations importObjectChildsAndAssociations)
        {
            ImportChildObjectsArray(schemaObjectsArray, acType, objName, idFieldName, displayNamePropertyName, parentId, tryLoadExistingObject, translateObjectAssociatedIds, defaultSaveACObject, importObjectChildsAndAssociations);
        }

        private void ImportChildObjectsArray(Array schemaObjectsArray, Type acType, String objName, String idFieldName, String displayNamePropertyName, object parentId, TryLoadExistingObject tryLoadExistingObject, TranslateObjectAssociatedIds translateObjectAssociatedIds, SaveACObject saveACObject, ImportObjectChildsAndAssociations importObjectChildsAndAssociations)
        {
            ImportChildObjectsArray(schemaObjectsArray, acType, objName, idFieldName, displayNamePropertyName, parentId, tryLoadExistingObject, translateObjectAssociatedIds, saveACObject, importObjectChildsAndAssociations, String.Empty);        
        }

        private void ImportChildObjectsArray(Array schemaObjectsArray, Type acType, String objName, String idFieldName, String displayNamePropertyName, object parentId, TryLoadExistingObject tryLoadExistingObject, TranslateObjectAssociatedIds translateObjectAssociatedIds, SaveACObject saveACObject, ImportObjectChildsAndAssociations importObjectChildsAndAssociations, String csvFields)
        {
            if (IsNonEmptyArray(schemaObjectsArray))
            {
                
                Type schemaType;
                
                PropertyInfo newIdInfo = acType.GetProperty(idFieldName);

                bool isNameBased = !String.IsNullOrEmpty(displayNamePropertyName);
                int imported = 0;
                int updated = 0;

                List<string> listCsvFields = null;                
                if (!String.IsNullOrEmpty(csvFields)) listCsvFields = new List<string>(csvFields.Split(','));               

                DataObject objDataObject;

                log("Importing " + schemaObjectsArray.Length + " " + objName + "s");
                foreach (Object schemaObject in schemaObjectsArray) //UPDATE
                {
                    Object acObject = null; //UPDATE
                    String displayName = String.Empty;

                    // SCHEMA OBJECT TYPE
                    schemaType = schemaObject.GetType();
                    objDataObject = new DataObject(acType, schemaType);

                    if (isNameBased)
                    {
                        PropertyInfo nameInfo = schemaType.GetProperty(displayNamePropertyName);
                        displayName = (String)nameInfo.GetValue(schemaObject, null);
                    }

                    // RECORD OLD ID
                    PropertyInfo oldIdInfo = schemaType.GetProperty(idFieldName);
                    int oldId = (int)oldIdInfo.GetValue(schemaObject, null);

                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            acObject = tryLoadExistingObject(schemaObject);
                            if (acObject != null)
                            {
                                if (isNameBased) log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                else log("Skipping import, " + objName + " with same Id (" + oldId + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            // SET ID TO ZERO SO THAT IT IS SAVED AS NEW OBJECT                            
                            newIdInfo.SetValue(acObject, 0, null);

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = tryLoadExistingObject(schemaObject);
                            if (acObject != null)
                            {
                                String indicator = acObject as string;
                                if ((!String.IsNullOrEmpty(indicator)) && indicator == "-1")
                                {
                                    // LOG THE MULITPLE MATCHING RECORDS WARNING AND SKIP
                                    log(String.Format("{0}({1}:{2}): Can not perform update because there are mulitple matching records.\n",objName,oldId,displayName));
                                    continue;
                                }
                                else
                                {
                                    // RECORD NEW ID                            
                                    newId = (int)newIdInfo.GetValue(acObject, null);
                                    acObject = objDataObject.UpdateToAC6Object(schemaObject, acObject, listCsvFields); // UPDATE

                                    // RESTORE THE AC OBJECT ID IF NEEDED
                                    if (oldId != newId) newIdInfo.SetValue(acObject, newId, null);

                                    updated++;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (ImportOption == ImportOptions.ImportOrUpdate)
                        {
                            // IF UPDATE OR IMPORT IMPORT OPTION SELECTED                            
                            // TRY TO UPDATE FIRST
                            acObject = tryLoadExistingObject(schemaObject);
                            if (acObject != null)
                            {
                                String indicator = acObject as string;
                                if ((!String.IsNullOrEmpty(indicator)) && indicator == "-1")
                                {
                                    // LOG THE MULITPLE MATCHING RECORDS WARNING AND SKIP
                                    log(String.Format("{0}({1}:{2})Can not perform update because there are mulitple matching records.\n", objName, oldId, displayName));
                                    continue;
                                }
                                else
                                {
                                    // RECORD NEW ID                            
                                    newId = (int)newIdInfo.GetValue(acObject, null);

                                    acObject = objDataObject.UpdateToAC6Object(schemaObject, acObject, listCsvFields);

                                    // RESTORE THE AC OBJECT ID IF NEEDED
                                    if (oldId != newId) newIdInfo.SetValue(acObject, newId, null);

                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = objDataObject.ConvertToAC6Object(schemaObject);
                                if (acObject == null)
                                {
                                    log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                    continue;
                                }
                                // SET ID TO ZERO SO THAT IT IS SAVED AS NEW OBJECT
                                newIdInfo.SetValue(acObject, 0, null);

                                imported++;
                            }

                        }

                        translateObjectAssociatedIds(ref acObject, schemaObject);
                        
                        // SAVE THE OBJECT
                        SaveResult saveResult = (SaveResult)saveACObject(ref acObject);
                        if (!saveResult.Equals(SaveResult.Failed))
                        {
                            // GET NEW ID AND ADD TO TRANSLATION DIC                            
                            newId = (int)newIdInfo.GetValue(acObject, null);
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS, AND ASSOCIATION OBJECTS
                            importObjectChildsAndAssociations(ref acObject, schemaObject);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.AppendLine(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id = " + oldId);
                            if (isNameBased) strLog.AppendLine("Name = " + displayName);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        logError(ex.Message +  "\n" + strLog.ToString() + "\n" + ex.StackTrace);
                        Exception baseException = ex.GetBaseException();
                        if(baseException != null)
                            logError("Base Exception: " + baseException.Message + "\n" + baseException.StackTrace);
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "(s) Import Complete...");
            }
        }

        /// <summary>
        /// Import static associations like with Store Events(which is a static enum type)
        /// </summary>
        /// <param name="arrSchemaObject"></param>
        /// <param name="acObjectCollection"></param>
        /// <param name="newId"></param>
        /// <param name="associationObjType"></param>
        /// <param name="associatedPropertyName"></param>
        private void ImportStaticAssociations(Array arrSchemaObject, CollectionBase acObjectCollection, int newId, Type associationObjType, String associatedPropertyName)
        {
            //String associatedPropertyName = "StoreEventId";

            //Type acObjType = typeof(MakerShop.Messaging.EmailTemplateTrigger);

            Type acCollectionType = acObjectCollection.GetType();

            if (arrSchemaObject != null && arrSchemaObject.Length > 0)
            {
                foreach (Object subSchemaObject in arrSchemaObject)
                {
                    Type schemaObjType = subSchemaObject.GetType();

                    // GET THE ASSOCIATED OBJECT ID
                    PropertyInfo associatedPropertyInfo = schemaObjType.GetProperty(associatedPropertyName);
                    int associatedObjectId = (int)associatedPropertyInfo.GetValue(subSchemaObject, null);

                    // GET THE INDEX OF ASSOCIATION OBJECT IN COLLECTION
                    Object[] objArgs = { newId, associatedObjectId };
                    Type[] paramTypes = {newId.GetType(),associatedObjectId.GetType()};
                    
                    MethodInfo indexOfInfo = acCollectionType.GetMethod("IndexOf", paramTypes);
                    int index = 0;
                    if (indexOfInfo != null)
                        index = (int)indexOfInfo.Invoke(acObjectCollection, objArgs);
                    else
                        logError("In ImportStaticAssociations method, the IndexOf() method not found for " + acCollectionType.ToString());

                    // IF SAME ASSOCIATION DOES NOT ALREADY EXISTS THEN ADD
                    if (index < 0)
                    {
                        IPersistable associationObject = (IPersistable)Activator.CreateInstance(associationObjType, objArgs);
                        
                        MethodInfo addInfo = acCollectionType.GetMethod("Add");
                        int a = 0;
                        if (addInfo != null)
                            a = (int)addInfo.Invoke(acObjectCollection, new object[] { associationObject });
                        else
                            logError("In ImportStaticAssociations method, the Add() method not found for " + acCollectionType.ToString());

                    }
                }
                // SAVE THE ASSOCIATION COLLECTION
                MethodInfo saveInfo = acCollectionType.GetMethod("Save", new Type[0]);
                if (saveInfo != null)
                    saveInfo.Invoke(acObjectCollection, null);
                else
                    logError("In ImportStaticAssociations method, the Save() method not found for " + acCollectionType.ToString());
            }
        }

        /// <summary>
        /// Imports association objects using the Translation Dictioary
        /// </summary>
        /// <param name="arrSchemaObject">Array of associated nestedObjects, For ShipMethod it can be array of ShipMethodGroups</param>
        /// <param name="acObjectCollection">Collection of associated nestedObject For ShipMethod it can be collection of ShipMethodGroup</param>
        /// <param name="newId">new Id of the object for which we are importing associations</param>
        /// <param name="associationObjType">for example for ShipMehtodGroups, it will be CommmerceBuilder.Shipping.ShipMethodGroup</param>
        /// <param name="associatedPropertyName">Example: for ShipMethodGroups it will be GroupId </param>
        /// <param name="associatedObjectName">for ShipMethodGroups it will be Group</param>
        private void ImportDynamicAssociations(Array arrSchemaObject, CollectionBase acObjectCollection, int newId, Type associationObjType, String associatedPropertyName, String associatedObjectName)
        {
            ImportDynamicAssociationObjectData importDynamicAssociationObjectData  = delegate(ref Object acAssociationObject, Object schemaAssociationObject){ /* NO DATA TO BE IMPORTED */};
            ImportDynamicAssociations(arrSchemaObject,
                acObjectCollection, newId,
                associationObjType,
                associatedPropertyName,
                associatedObjectName,
                importDynamicAssociationObjectData);   
        }

        

        /// <summary>
        /// Imports association objects using the Translation Dictioary
        /// </summary>
        /// <param name="arrSchemaObject">Array of associated nestedObjects, For ShipMethod it can be array of ShipMethodGroups</param>
        /// <param name="acObjectCollection">Collection of associated nestedObject For ShipMethod it can be collection of ShipMethodGroup</param>
        /// <param name="newId">new Id of the object for which we are importing associations</param>
        /// <param name="associationObjType">for example for ShipMehtodGroups, it will be CommmerceBuilder.Shipping.ShipMethodGroup</param>
        /// <param name="associatedPropertyName">Example: for ShipMethodGroups it will be GroupId </param>
        /// <param name="associatedObjectName">for ShipMethodGroups it will be Group</param>
        private void ImportDynamicAssociations(Array arrSchemaObject, CollectionBase acObjectCollection, int newId, Type associationObjType, String associatedPropertyName, String associatedObjectName, ImportDynamicAssociationObjectData importDynamicAssociationObjectData)
        {
            Type acCollectionType = acObjectCollection.GetType();

            if (arrSchemaObject != null && arrSchemaObject.Length > 0)
            {
                foreach (Object schemaAssociationObject in arrSchemaObject)
                {
                    try
                    {
                        Type schemaObjType = schemaAssociationObject.GetType();

                        // GET THE ASSOCIATED OBJECT ID
                        PropertyInfo associatedPropertyInfo = schemaObjType.GetProperty(associatedPropertyName);
                        int oldAssociatedObjectId = (int)associatedPropertyInfo.GetValue(schemaAssociationObject, null);

                        // TRANSLATE THE ASSOCIATED OBJECT ID USING TRANSLATION DIC
                        int associatedObjectId = Translate(associatedObjectName, oldAssociatedObjectId);

                        // AS THE ASSOCIATED OBJECT ID NOT FOUND, SKIPPING
                        if (associatedObjectId == 0)
                        {
                            log("Error importing " + associationObjType.ToString() + ", unable to translate the association. Skipping for Id = " + oldAssociatedObjectId);
                            continue;
                        }

                        // GET THE INDEX OF ASSOCIATION OBJECT IN COLLECTION
                        Object[] objArgs = { newId, associatedObjectId };
                        Type[] paramTypes = { newId.GetType(), associatedObjectId.GetType() };
                        MethodInfo indexOfInfo = acCollectionType.GetMethod("IndexOf", paramTypes);
                        int index = 0;
                        if (indexOfInfo != null)
                            index = (int)indexOfInfo.Invoke(acObjectCollection, objArgs);
                        else
                            logError("In ImportDynamicAssociations method, the IndexOf() method not found for " + acCollectionType.ToString());

                        // IF SAME ASSOCIATION DOES NOT ALREADY EXISTS THEN ADD
                        if (index < 0)
                        {
                            IPersistable associationObject = (IPersistable)Activator.CreateInstance(associationObjType, objArgs);

                            MethodInfo addInfo = acCollectionType.GetMethod("Add");
                            int a = 0;
                            if (addInfo != null)
                                a = (int)addInfo.Invoke(acObjectCollection, new object[] { associationObject });
                            else
                                logError("In ImportDynamicAssociations method, the Add() method not found for " + acCollectionType.ToString());

                            // IMPORT ANY OTHER DATA
                            Object acAssociationObject = (object)associationObject;
                            importDynamicAssociationObjectData(ref acAssociationObject, schemaAssociationObject);
                        }
                    }
                    catch (Exception exception)
                    {
                        PropertyInfo associatedPropertyInfo = schemaAssociationObject.GetType().GetProperty(associatedPropertyName);
                        String strLog = "Association import unsuccessfull for:" + acCollectionType.ToString() +
                            "\n" + associatedPropertyName + ":" + associatedPropertyInfo.GetValue(schemaAssociationObject, null).ToString() +
                            "\nIn ImportDynamicAssociations method, Excption: " + exception.Message +
                            "\n" + exception.StackTrace ;
                        if (exception.GetBaseException() != null)
                            strLog += "\nBaseExcption:" + exception.GetBaseException().Message + "\n" + exception.GetBaseException().StackTrace;

                        logError(strLog);
                    }
                }
                // SAVE THE ASSOCIATION COLLECTION
                MethodInfo saveInfo = acCollectionType.GetMethod("Save", new Type[0]);
                if (saveInfo != null) saveInfo.Invoke(acObjectCollection, null);
                else logError("In ImportDynamicAssociations method, the Save() method not found for " + acCollectionType.ToString());

            }
        }

        /// <summary>
        /// return true if the translation dictionary contains Values needed for translation
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="oldId"></param>
        /// <returns></returns>
        private bool CanTranslate(String objName, int oldId)
        {
            String key = objName.ToUpperInvariant() + oldId.ToString();
            return TranslationDic.ContainsKey(key);            
        }

        /// <summary>
        /// Translates the Old Id of the object to new Id, if tranlation not available then returns the default value specified.
        /// </summary>
        /// <param name="objName">Name of the object whose id need to be translated</param>
        /// <param name="oldId">Old Id of the object, as specifed in the import data, in association</param>
        /// <param name="defaultRetrunValue">default value to be returned,in case when no translation available</param>
        /// <returns></returns>
        private int Translate(String objName, int oldId, int defaultRetrunValue)
        {
            if (!CanTranslate(objName, oldId)) return defaultRetrunValue;

            String key = objName.ToUpperInvariant() + oldId.ToString();
            return TranslationDic[key];
        }

        /// <summary>
        /// Translates the Old Id of the object to new Id, if tranlation not available then returns the old Id without translation.
        /// </summary>
        /// <param name="objName">Name of the object whose id need to be translated</param>
        /// <param name="oldId">Old Id of the object, as specifed in the import data, in association</param>
        /// <returns></returns>
        private int Translate( String objName,int oldId)
        {
            return Translate(objName, oldId, oldId);
        }

        private String SqlEscape(String str)
        {
            // REPLACE THE SINGLE QUOTE WITH TWO SINGLE QUOTES            
            return str.Replace("'", "''");
        }

        /// <summary>
        /// Calculates object load criteria
        /// </summary>
        /// <param name="schemaObject"></param>
        /// <param name="matchFields"></param>
        /// <param name="numaricFields"></param>
        /// <returns></returns>
        private static String CalculateCriteria(Object schemaObject, String matchFields, List<String> numaricFields)
        {
            String[] fields = matchFields.Split(',');
            StringBuilder strCriteria = new StringBuilder();

            //SHOULD ONLY ALLOW TO UPDATE CURRENT STORE
            strCriteria.Append("StoreId = ").Append(Token.Instance.StoreId);

            // BUILD THE CRITERIA
            foreach (String field in fields)
            {
                bool isNumaricField = numaricFields.Contains(field);
                string fldValue = schemaObject.GetType().GetProperty(field).GetValue(schemaObject, null).ToString();

                strCriteria.Append(" AND (");
                strCriteria.Append(field).Append(" = ");
                if (!isNumaricField) strCriteria.Append("'");
                strCriteria.Append(StringHelper.SafeSqlString(fldValue));
                if (!isNumaricField) strCriteria.Append("'");
                strCriteria.Append(" ");

                //ALSO INCLUDE NULL VALUES IN SEARCH ( NON-NUMARIC FIELDS ONLY)
                if (!isNumaricField && String.IsNullOrEmpty(fldValue))
                {
                    strCriteria.Append(" OR ");
                    strCriteria.Append(field).Append(" IS NULL ");
                }

                strCriteria.Append(")");
            }
            return strCriteria.ToString();
        }
    }
}
