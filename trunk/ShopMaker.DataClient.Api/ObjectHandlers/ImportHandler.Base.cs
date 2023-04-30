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

        #region Import AuditEvents

        private void ImportAuditEvents(MakerShop.DataClient.Api.Schema.AuditEvent[] arrImportObjects) // UPDATE
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.AuditEvent auditEvent = (Schema.AuditEvent)schemaObject;
                return MakerShop.Stores.AuditEventDataSource.Load(auditEvent.AuditEventId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Stores.AuditEvent acAuditEvent = (Stores.AuditEvent)acObject;
                Schema.AuditEvent schemaAuditEvent = (Schema.AuditEvent)schemaObject;
                acAuditEvent.StoreId = Token.Instance.StoreId; // VERIFY
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Stores.AuditEvent acAuditEvent = (Stores.AuditEvent)acObject;
                Schema.AuditEvent schemaAuditEvent = (Schema.AuditEvent)schemaObject;
            };

            ImportObjectsArray(arrImportObjects, typeof(MakerShop.Stores.AuditEvent), "AuditEvent", "AuditEventId", String.Empty,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import IPLocations

        //private void ImportIPLocations(MakerShop.DataClient.Api.Schema.IPLocation[] arrImportObjects) // UPDATE
        //{
        //    if (arrImportObjects != null && arrImportObjects.Length > 0)
        //    {
        //        String objName = "AuditEvent";   //UPDATE             
        //        Type acType = typeof(MakerShop.Stores.AuditEvent); //UPDATE
        //        Type schemaType = typeof(Schema.AuditEvent); //UPDATE
        //        int imported = 0;
        //        int updated = 0;

        //        DataObject objDataObject = new DataObject(acType, schemaType);

        //        log("Importing " + arrImportObjects.Length + " " + objName + "s");
        //        foreach (Schema.AuditEvent schemaObject in arrImportObjects) //UPDATE
        //        {
        //            MakerShop.Stores.AuditEvent acObject = null; //UPDATE

        //            int oldId = schemaObject.AuditEventId; // UPDATE
        //            int newId = 0;
        //            try
        //            {

        //                if (ImportOption == ImportOptions.Import)
        //                {
        //                    // IF IMPORT ONLY OPTION IS SELECTED
        //                    acObject = (MakerShop.Stores.AuditEvent)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
        //                    if (acObject == null)
        //                    {
        //                        log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
        //                        continue;
        //                    }
        //                    schemaObject.AuditEventId = 0; // UDPATE

        //                    imported++;
        //                }
        //                else if (ImportOption == ImportOptions.Update)
        //                {
        //                    // IF UPDATE ONLY OPTION IS SELECTED
        //                    acObject = AuditEventDataSource.Load(oldId); // UPDATE
        //                    if (acObject != null)
        //                    {
        //                        acObject = (MakerShop.Stores.AuditEvent)objDataObject.UpdateToAC6Object(schemaObject, acObject);
        //                        updated++;
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                }
        //                else if (ImportOption == ImportOptions.ImportOrUpdate)
        //                {
        //                    // IF UPDATE OR IMPORT IMPORT OPTION SELECTED
        //                    // TRY TO UPDATE FIRST
        //                    acObject = AuditEventDataSource.Load(oldId);
        //                    if (acObject != null)
        //                    {
        //                        acObject = (MakerShop.Stores.AuditEvent)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
        //                        updated++;
        //                    }
        //                    else
        //                    {
        //                        // CAN NOT BE UPDATED, SO IMPORT IT
        //                        acObject = (MakerShop.Stores.AuditEvent)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
        //                        acObject.AuditEventId = 0;  // UPDATE

        //                        imported++;
        //                    }

        //                }

        //                acObject.StoreId = Token.Instance.StoreId; // VERIFY
        //                if (!acObject.Save().Equals(SaveResult.Failed))
        //                {
        //                    newId = acObject.AuditEventId; // UPDATE
        //                    AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
        //                }
        //                else
        //                {
        //                    log(objName + " Not Saved.\n");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                StringBuilder strLog = new StringBuilder();
        //                strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
        //                if (schemaObject != null)
        //                {
        //                    strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
        //                }
        //                strLog.AppendLine("Error message: " + ex.Message);
        //                log(strLog.ToString());
        //            }
        //        }
        //        log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
        //        log(objName + "s Import Complete...");
        //    }
        //}

        #endregion

        #region Import ErrorMessages

        private void ImportErrorMessages(MakerShop.DataClient.Api.Schema.ErrorMessage[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "ErrorMessage";   //UPDATE             
                Type acType = typeof(MakerShop.Utility.ErrorMessage); //UPDATE
                Type schemaType = typeof(Schema.ErrorMessage); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.ErrorMessage schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Utility.ErrorMessage acObject = null; //UPDATE
                    int oldId = schemaObject.ErrorMessageId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            acObject = (MakerShop.Utility.ErrorMessage)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.ErrorMessageId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = MakerShop.Utility.ErrorMessageDataSource.Load(oldId); // UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Utility.ErrorMessage)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            acObject = MakerShop.Utility.ErrorMessageDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Utility.ErrorMessage)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Utility.ErrorMessage)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.ErrorMessageId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.ErrorMessageId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        #endregion

        #region Import PersonalizationPaths

        private void ImportPersonalizationPaths(MakerShop.DataClient.Api.Schema.PersonalizationPath[] arrImportObjects) // UPDATE
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.PersonalizationPath personalizationPath = (Schema.PersonalizationPath)schemaObject;
                return MakerShop.Personalization.PersonalizationPathDataSource.Load(personalizationPath.PersonalizationPathId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Personalization.PersonalizationPath acPersonalizationPath = (Personalization.PersonalizationPath)acObject;
                Schema.PersonalizationPath schemaPersonalizationPath = (Schema.PersonalizationPath)schemaObject;
                acPersonalizationPath.StoreId = Token.Instance.StoreId; // VERIFY
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Personalization.PersonalizationPath acPersonalizationPath = (Personalization.PersonalizationPath)acObject;
                Schema.PersonalizationPath schemaPersonalizationPath = (Schema.PersonalizationPath)schemaObject;

            };

            ImportObjectsArray(arrImportObjects, typeof(MakerShop.Personalization.PersonalizationPath), "PersonalizationPath", "PersonalizationPathId", String.Empty,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region ImportReadmes

        private void ImportReadmes(MakerShop.DataClient.Api.Schema.Readme[] arrImportObjects) // UPDATE
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Readme raedMe = (Schema.Readme)schemaObject;
                MakerShop.DigitalDelivery.ReadmeCollection tempCollection = MakerShop.DigitalDelivery.ReadmeDataSource.LoadForCriteria("DisplayName = '" + raedMe.DisplayName + "'"); // Update
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                DigitalDelivery.Readme acReadme = (DigitalDelivery.Readme)acObject;
                Schema.Readme schemaReadme = (Schema.Readme)schemaObject;
                acReadme.StoreId = Token.Instance.StoreId; // VERIFY
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                DigitalDelivery.Readme acReadme = (DigitalDelivery.Readme)acObject;
                Schema.Readme schemaReadme = (Schema.Readme)schemaObject;

            };

            ImportObjectsArray(arrImportObjects, typeof(MakerShop.DigitalDelivery.Readme), "Readme", "ReadmeId", String.Empty,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import Roles

        private void ImportRoles(MakerShop.DataClient.Api.Schema.Role[] arrImportObjects) // UPDATE
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Role role = (Schema.Role)schemaObject;
                return MakerShop.Users.RoleDataSource.LoadForRolename(role.Name); // Update
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Users.Role acRole = (Users.Role)acObject;
                Schema.Role schemaRole = (Schema.Role)schemaObject;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Users.Role acRole = (Users.Role)acObject;
                Schema.Role schemaRole = (Schema.Role)schemaObject;
            };
            ImportObjectsArray(arrImportObjects, typeof(MakerShop.Users.Role), "Role", "RoleId", String.Empty,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        private void ImportShipGateways(MakerShop.DataClient.Api.Schema.ShipGateway[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "ShipGateway";   //UPDATE             
                Type acType = typeof(MakerShop.Shipping.ShipGateway); //UPDATE
                Type schemaType = typeof(Schema.ShipGateway); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.ShipGateway schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Shipping.ShipGateway acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.ShipGatewayId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED

                            MakerShop.Shipping.ShipGatewayCollection tempCollection = MakerShop.Shipping.ShipGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.ShipGatewayId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Shipping.ShipGatewayCollection tempCollection = MakerShop.Shipping.ShipGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipGateway)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Shipping.ShipGatewayCollection tempCollection = MakerShop.Shipping.ShipGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipGateway)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Shipping.ShipGateway)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.ShipGatewayId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.ShipGatewayId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportTaxGateways(MakerShop.DataClient.Api.Schema.TaxGateway[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "TaxGateway";   //UPDATE             
                Type acType = typeof(MakerShop.Taxes.TaxGateway); //UPDATE
                Type schemaType = typeof(Schema.TaxGateway); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.TaxGateway schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Taxes.TaxGateway acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.TaxGatewayId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            MakerShop.Taxes.TaxGatewayCollection tempCollection = MakerShop.Taxes.TaxGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.TaxGatewayId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Taxes.TaxGatewayCollection tempCollection = MakerShop.Taxes.TaxGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Taxes.TaxGateway)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Taxes.TaxGatewayCollection tempCollection = MakerShop.Taxes.TaxGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Taxes.TaxGateway)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Taxes.TaxGateway)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.TaxGatewayId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.TaxGatewayId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportAffiliates(MakerShop.DataClient.Api.Schema.Affiliate[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Affiliate";   //UPDATE             
                Type acType = typeof(MakerShop.Marketing.Affiliate); //UPDATE
                Type schemaType = typeof(Schema.Affiliate); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Affiliate schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Marketing.Affiliate acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.AffiliateId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED

                            MakerShop.Marketing.AffiliateCollection tempCollection = MakerShop.Marketing.AffiliateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.AffiliateId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Marketing.AffiliateCollection tempCollection = MakerShop.Marketing.AffiliateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Marketing.Affiliate)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Marketing.AffiliateCollection tempCollection = MakerShop.Marketing.AffiliateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Marketing.Affiliate)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Marketing.Affiliate)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.AffiliateId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.AffiliateId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportBannedIPs(MakerShop.DataClient.Api.Schema.BannedIP[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "BannedIP";   //UPDATE             
                Type acType = typeof(MakerShop.Stores.BannedIP); //UPDATE
                Type schemaType = typeof(Schema.BannedIP); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.BannedIP schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Stores.BannedIP acObject = null; //UPDATE

                    int oldId = schemaObject.BannedIPId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            acObject = (MakerShop.Stores.BannedIP)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.BannedIPId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = BannedIPDataSource.Load(oldId); // UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Stores.BannedIP)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            acObject = BannedIPDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Stores.BannedIP)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Stores.BannedIP)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.BannedIPId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.BannedIPId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportCurrencies(MakerShop.DataClient.Api.Schema.Currency[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Currency";   //UPDATE             
                Type acType = typeof(MakerShop.Stores.Currency); //UPDATE
                Type schemaType = typeof(Schema.Currency); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Currency schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Stores.Currency acObject = null; //UPDATE
                    String displayName = schemaObject.ISOCode; // VERIFY

                    int oldId = schemaObject.CurrencyId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            CurrencyCollection tempCollection = CurrencyDataSource.LoadForCriteria("ISOCode = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.CurrencyId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            CurrencyCollection tempCollection = CurrencyDataSource.LoadForCriteria("ISOCode = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                int objectId = acObject.CurrencyId;
                                acObject = (MakerShop.Stores.Currency)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                acObject.CurrencyId = objectId;
                                updated++;
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
                            CurrencyCollection tempCollection = CurrencyDataSource.LoadForCriteria("ISOCode = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                int objectId = acObject.CurrencyId;
                                acObject = (MakerShop.Stores.Currency)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                acObject.CurrencyId = objectId;
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Stores.Currency)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.CurrencyId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.CurrencyId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportGroups(MakerShop.DataClient.Api.Schema.Group[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Group";   //UPDATE             
                Type acType = typeof(MakerShop.Users.Group); //UPDATE
                Type schemaType = typeof(Schema.Group); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Group schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Users.Group acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.GroupId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED

                            acObject = MakerShop.Users.GroupDataSource.LoadForName(displayName); // Update
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.GroupId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = MakerShop.Users.GroupDataSource.LoadForName(displayName); // Update
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Users.Group)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            acObject = MakerShop.Users.GroupDataSource.LoadForName(displayName); // Update
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Users.Group)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Users.Group)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.GroupId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.GroupId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION
                            ImportDynamicAssociations(schemaObject.GroupRoles,                             // Update
                                                acObject.GroupRoles,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Users.GroupRole),                 // Update  
                                                "RoleId", "Role");                                        // Update  
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportTaxCodes(MakerShop.DataClient.Api.Schema.TaxCode[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "TaxCode";   //UPDATE             
                Type acType = typeof(MakerShop.Taxes.TaxCode); //UPDATE
                Type schemaType = typeof(Schema.TaxCode); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.TaxCode schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Taxes.TaxCode acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.TaxCodeId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED

                            MakerShop.Taxes.TaxCodeCollection tempCollection = MakerShop.Taxes.TaxCodeDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.TaxCodeId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Taxes.TaxCodeCollection tempCollection = MakerShop.Taxes.TaxCodeDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Taxes.TaxCode)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Taxes.TaxCodeCollection tempCollection = MakerShop.Taxes.TaxCodeDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Taxes.TaxCode)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Taxes.TaxCode)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.TaxCodeId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.TaxCodeId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportTaxRules(MakerShop.DataClient.Api.Schema.TaxRule[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "TaxRule";   //UPDATE             
                Type acType = typeof(MakerShop.Taxes.TaxRule); //UPDATE
                Type schemaType = typeof(Schema.TaxRule); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.TaxRule schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Taxes.TaxRule acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.TaxRuleId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED

                            MakerShop.Taxes.TaxRuleCollection tempCollection = MakerShop.Taxes.TaxRuleDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.TaxRuleId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Taxes.TaxRuleCollection tempCollection = MakerShop.Taxes.TaxRuleDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Taxes.TaxRule)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Taxes.TaxRuleCollection tempCollection = MakerShop.Taxes.TaxRuleDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Taxes.TaxRule)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Taxes.TaxRule)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.TaxRuleId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (CanTranslate("TaxCode",schemaObject.TaxCodeId)) acObject.TaxCodeId =  Translate("TaxCode",schemaObject.TaxCodeId);
                        if (CanTranslate("Province", schemaObject.ProvinceId)) acObject.ProvinceId = Translate("Province", schemaObject.ProvinceId);
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.TaxRuleId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION
                            ImportDynamicAssociations(schemaObject.TaxRuleTaxCodes,                             // Update
                                                acObject.TaxRuleTaxCodes,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Taxes.TaxRuleTaxCode),                 // Update  
                                                "TaxCodeId", "TaxCode");                                        // Update  

                            ImportDynamicAssociations(schemaObject.TaxRuleGroups,
                                                acObject.TaxRuleGroups,
                                                newId,
                                                typeof(MakerShop.Taxes.TaxRuleGroup),
                                                "GroupId", "Group");
                            ImportDynamicAssociations(schemaObject.TaxRuleShipZones,
                                                acObject.TaxRuleShipZones,
                                                newId,
                                                typeof(MakerShop.Taxes.TaxRuleShipZone),
                                                "ShipZoneId", "ShipZone"); 
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportVolumeDiscounts(MakerShop.DataClient.Api.Schema.VolumeDiscount[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "VolumeDiscount";   //UPDATE             
                Type acType = typeof(MakerShop.Marketing.VolumeDiscount); //UPDATE
                Type schemaType = typeof(Schema.VolumeDiscount); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.VolumeDiscount schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Marketing.VolumeDiscount acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.VolumeDiscountId;// UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            MakerShop.Marketing.VolumeDiscountCollection tempCollection = MakerShop.Marketing.VolumeDiscountDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.VolumeDiscountId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Marketing.VolumeDiscountCollection tempCollection = MakerShop.Marketing.VolumeDiscountDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Marketing.VolumeDiscount)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Marketing.VolumeDiscountCollection tempCollection = MakerShop.Marketing.VolumeDiscountDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Marketing.VolumeDiscount)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Marketing.VolumeDiscount)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.VolumeDiscountId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.VolumeDiscountId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS
                            //VolumeDiscountLevels
                            ImportVolumeDiscountLevels(schemaObject.Levels, acObject.Levels, newId);

                            // IMPORTING ASSOCIATION
                            ImportDynamicAssociations(schemaObject.VolumeDiscountGroups,                             // Update
                                                acObject.VolumeDiscountGroups,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Marketing.VolumeDiscountGroup),                 // Update  
                                                "GroupId", "Group");                                        // Update  
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportVolumeDiscountLevels(MakerShop.DataClient.Api.Schema.VolumeDiscountLevel[] arrImportObjects, VolumeDiscountLevelCollection volumeDiscountLevelCollection, int volumeDiscountId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.VolumeDiscountLevel schemaVolumeDiscountLevel = (Schema.VolumeDiscountLevel)schemaObject;                
                Marketing.VolumeDiscountLevelCollection tempCollection = VolumeDiscountLevelDataSource.LoadForCriteria(" VolumeDiscountId = " + volumeDiscountId + " AND VolumeDiscountLevelId = " + schemaVolumeDiscountLevel.VolumeDiscountLevelId);
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Marketing.VolumeDiscountLevel acVolumeDiscountLevel = (Marketing.VolumeDiscountLevel)acObject;
                Schema.VolumeDiscountLevel schemaVolumeDiscountLevel = (Schema.VolumeDiscountLevel)schemaObject;

                acVolumeDiscountLevel.VolumeDiscountId = volumeDiscountId;                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject) { };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Marketing.VolumeDiscountLevel), "VolumeDiscountLevel", "VolumeDiscountLevelId", String.Empty, volumeDiscountId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        private void ImportLicenseAgreements(MakerShop.DataClient.Api.Schema.LicenseAgreement[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "LicenseAgreement";   //UPDATE             
                Type acType = typeof(MakerShop.DigitalDelivery.LicenseAgreement); //UPDATE
                Type schemaType = typeof(Schema.LicenseAgreement); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.LicenseAgreement schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.DigitalDelivery.LicenseAgreement acObject = null; //UPDATE
                    String displayName = schemaObject.DisplayName; // VERIFY

                    int oldId = schemaObject.LicenseAgreementId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            MakerShop.DigitalDelivery.LicenseAgreementCollection tempCollection = MakerShop.DigitalDelivery.LicenseAgreementDataSource.LoadForCriteria("DisplayName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.LicenseAgreementId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.DigitalDelivery.LicenseAgreementCollection tempCollection = MakerShop.DigitalDelivery.LicenseAgreementDataSource.LoadForCriteria("DisplayName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.DigitalDelivery.LicenseAgreement)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.DigitalDelivery.LicenseAgreementCollection tempCollection = MakerShop.DigitalDelivery.LicenseAgreementDataSource.LoadForCriteria("DisplayName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.DigitalDelivery.LicenseAgreement)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.DigitalDelivery.LicenseAgreement)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.LicenseAgreementId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.LicenseAgreementId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportStoreSettings(MakerShop.DataClient.Api.Schema.StoreSetting[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "StoreSetting";   //UPDATE             
                Type acType = typeof(MakerShop.Stores.StoreSetting); //UPDATE
                Type schemaType = typeof(Schema.StoreSetting); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.StoreSetting schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Stores.StoreSetting acObject = null; //UPDATE
                    String displayName = schemaObject.FieldName; // VERIFY

                    int oldId = schemaObject.StoreSettingId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            MakerShop.Stores.StoreSettingCollection tempCollection = MakerShop.Stores.StoreSettingDataSource.LoadForCriteria("FieldName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.StoreSettingId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Stores.StoreSettingCollection tempCollection = MakerShop.Stores.StoreSettingDataSource.LoadForCriteria("FieldName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Stores.StoreSetting)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Stores.StoreSettingCollection tempCollection = MakerShop.Stores.StoreSettingDataSource.LoadForCriteria("FieldName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Stores.StoreSetting)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Stores.StoreSetting)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.StoreSettingId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.StoreSettingId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        
        private void ImportManufacturers(MakerShop.DataClient.Api.Schema.Manufacturer[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Manufacturer";   //UPDATE             
                Type acType = typeof(MakerShop.Products.Manufacturer); //UPDATE
                Type schemaType = typeof(Schema.Manufacturer); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Manufacturer schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Products.Manufacturer acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.ManufacturerId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            acObject = MakerShop.Products.ManufacturerDataSource.LoadForName(displayName, false); // Update
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.ManufacturerId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = MakerShop.Products.ManufacturerDataSource.LoadForName(displayName, false); // Update
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.Manufacturer)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            acObject = MakerShop.Products.ManufacturerDataSource.LoadForName(displayName, false); // Update
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.Manufacturer)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Products.Manufacturer)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.ManufacturerId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.ManufacturerId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportPaymentGateways(MakerShop.DataClient.Api.Schema.PaymentGateway[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "PaymentGateway";   //UPDATE             
                Type acType = typeof(MakerShop.Payments.PaymentGateway); //UPDATE
                Type schemaType = typeof(Schema.Payment); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.PaymentGateway schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Payments.PaymentGateway acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.PaymentGatewayId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            MakerShop.Payments.PaymentGatewayCollection tempCollection = MakerShop.Payments.PaymentGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.PaymentGatewayId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Payments.PaymentGatewayCollection tempCollection = MakerShop.Payments.PaymentGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Payments.PaymentGateway)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Payments.PaymentGatewayCollection tempCollection = MakerShop.Payments.PaymentGatewayDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Payments.PaymentGateway)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Payments.PaymentGateway)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.PaymentGatewayId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.PaymentGatewayId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        /// <summary>
        /// Imports EmailTemplats
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportEmailTemplates(MakerShop.DataClient.Api.Schema.EmailTemplate[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "EmailTemplate";   //UPDATE             
                Type acType = typeof(MakerShop.Messaging.EmailTemplate); //UPDATE
                Type schemaType = typeof(Schema.EmailTemplate); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.EmailTemplate schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Messaging.EmailTemplate acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.EmailTemplateId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            EmailTemplateCollection tempCollection = EmailTemplateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Messaging.EmailTemplate)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.EmailTemplateId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            EmailTemplateCollection tempCollection = EmailTemplateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Messaging.EmailTemplate)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            EmailTemplateCollection tempCollection = EmailTemplateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Messaging.EmailTemplate)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Messaging.EmailTemplate)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.EmailTemplateId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.EmailTemplateId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION
                            ImportStaticAssociations(schemaObject.Triggers,                                   // Update
                                                acObject.Triggers,                                      // Update  
                                                newId,
                                                typeof(Messaging.EmailTemplateTrigger),                 // Update  
                                                "StoreEventId");                                        // Update  

                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportVendors(MakerShop.DataClient.Api.Schema.Vendor[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "EmailTemplate";   //UPDATE             
                Type acType = typeof(MakerShop.Products.Vendor); //UPDATE
                Type schemaType = typeof(Schema.Vendor); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Vendor schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Products.Vendor acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.VendorId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS

                            MakerShop.Products.VendorCollection tempCollection = MakerShop.Products.VendorDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Products.Vendor)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.VendorId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Products.VendorCollection tempCollection = MakerShop.Products.VendorDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.Vendor)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            MakerShop.Products.VendorCollection tempCollection = MakerShop.Products.VendorDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.Vendor)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Products.Vendor)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.VendorId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.VendorId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION
                            ImportDynamicAssociations(schemaObject.VendorGroups,                                   // Update
                                                acObject.VendorGroups,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Products.VendorGroup),                 // Update  
                                                "GroupId", "Group");                                        // Update  

                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        /// <summary>
        /// Imports Countries
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportCountries(MakerShop.DataClient.Api.Schema.Country[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Country";   //UPDATE             
                Type acType = typeof(MakerShop.Shipping.Country); //UPDATE
                Type schemaType = typeof(Schema.Country); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Country schemaObject in arrImportObjects) //UPDATE
                {
                    if (String.IsNullOrEmpty(schemaObject.CountryCode))
                    {
                        log(schemaObject.Name + ",CountryCode not provided. Country can not be imported/updated.");
                        continue;
                    }
                    MakerShop.Shipping.Country acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY                    
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            acObject = CountryDataSource.Load(schemaObject.CountryCode); // Update                            
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Shipping.Country)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with CountryCode (" + displayName + ") can not be imported.");
                                continue;
                            }

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = CountryDataSource.Load(schemaObject.CountryCode); // Update
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.Country)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            acObject = CountryDataSource.Load(schemaObject.CountryCode); // Update                            
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.Country)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Shipping.Country)objDataObject.ConvertToAC6Object(schemaObject);
                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            // IMPORT CHILD OBJECTS
                            ImportProvinces(schemaObject.Provinces);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + " = " + displayName);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        /// <summary>
        /// Imports Provinces
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportProvinces(MakerShop.DataClient.Api.Schema.Province[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Province";   //UPDATE             
                Type acType = typeof(MakerShop.Shipping.Province); //UPDATE
                Type schemaType = typeof(Schema.Province); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Province schemaObject in arrImportObjects) //UPDATE
                {                    
                    MakerShop.Shipping.Province acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY                    
                    int oldId = schemaObject.ProvinceId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS 
                            String strCriteria = "CountryCode='" + schemaObject.CountryCode + "'";
                            if (!String.IsNullOrEmpty(schemaObject.Name)) strCriteria += " AND Name = '" + schemaObject.Name + "'";
                            if (oldId != 0) strCriteria += " AND ProvinceId = " + oldId;
                            ProvinceCollection tempCollection = ProvinceDataSource.LoadForCriteria(strCriteria);
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Shipping.Province)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with ProvinceCode (" + displayName + ") can not be imported.");
                                continue;
                            }

                            acObject.ProvinceId = 0;
                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            ProvinceCollection tempCollection = ProvinceDataSource.LoadForCriteria("ProvinceCode = '" + schemaObject.ProvinceCode + "' AND CountryCode='" + schemaObject.CountryCode + "'");
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.Province)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            String strCriteria = "CountryCode='" + schemaObject.CountryCode + "'";
                            if (!String.IsNullOrEmpty(schemaObject.Name)) strCriteria += " AND Name = '" + schemaObject.Name + "'";
                            if (oldId != 0) strCriteria += " AND ProvinceId = " + oldId;
                            ProvinceCollection tempCollection = ProvinceDataSource.LoadForCriteria(strCriteria);
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.Province)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE                            
                                if (acObject != null)
                                {
                                    acObject = (MakerShop.Shipping.Province)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Shipping.Province)objDataObject.ConvertToAC6Object(schemaObject);
                                acObject.ProvinceId = 0;
                                imported++;
                            }

                        }

                        if (acObject.Save().Equals(SaveResult.Failed))
                        {
                            log(objName + " Not Saved.\n");
                        }
                        else
                        {
                            newId = acObject.ProvinceId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + " = " + displayName);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }



        /// <summary>
        /// Imports WrapGroups
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportWrapGroups(MakerShop.DataClient.Api.Schema.WrapGroup[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "WrapGroup";   //UPDATE             
                Type acType = typeof(MakerShop.Products.WrapGroup); //UPDATE
                Type schemaType = typeof(Schema.WrapGroup); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.WrapGroup schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Products.WrapGroup acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY
                    int oldId = schemaObject.WrapGroupId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            WrapGroupCollection tempCollection = WrapGroupDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Products.WrapGroup)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.WrapGroupId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            WrapGroupCollection tempCollection = WrapGroupDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WrapGroupDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.WrapGroup)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            WrapGroupCollection tempCollection = WrapGroupDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WrapGroupDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.WrapGroup)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                //acObject = WrapGroupDataSource.Load(oldId);// UPDATE
                                if (acObject != null)
                                {
                                    acObject = (MakerShop.Products.WrapGroup)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Products.WrapGroup)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.WrapGroupId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.WrapGroupId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS
                            ImportWrapStyles(schemaObject.WrapStyles, newId);

                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportProductTemplates(MakerShop.DataClient.Api.Schema.ProductTemplate[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "ProductTemplate";   //UPDATE             
                Type acType = typeof(MakerShop.Products.ProductTemplate); //UPDATE
                Type schemaType = typeof(Schema.ProductTemplate); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.ProductTemplate schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Products.ProductTemplate acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.ProductTemplateId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            MakerShop.Products.ProductTemplateCollection tempCollection = MakerShop.Products.ProductTemplateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Products.ProductTemplate)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.ProductTemplateId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Products.ProductTemplateCollection tempCollection = MakerShop.Products.ProductTemplateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.ProductTemplate)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            MakerShop.Products.ProductTemplateCollection tempCollection = MakerShop.Products.ProductTemplateDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.ProductTemplate)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Products.ProductTemplate)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.ProductTemplateId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.ProductTemplateId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS
                            ImportInputFields(schemaObject.InputFields, newId);
                            // IMPORTING ASSOCIATION

                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportInputFields(MakerShop.DataClient.Api.Schema.InputField[] arrImportObjects, int productTemplateId) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "InputField";   //UPDATE             
                Type acType = typeof(MakerShop.Products.InputField); //UPDATE
                Type schemaType = typeof(Schema.InputField); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.InputField schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Products.InputField acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.InputFieldId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS

                            MakerShop.Products.InputFieldCollection tempCollection = MakerShop.Products.InputFieldDataSource.LoadForCriteria("Name = '" + displayName + "' AND ProductTemplateId = " + productTemplateId); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Products.InputField)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.InputFieldId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Products.InputFieldCollection tempCollection = MakerShop.Products.InputFieldDataSource.LoadForCriteria("Name = '" + displayName + "' AND ProductTemplateId = " + productTemplateId); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.InputField)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            MakerShop.Products.InputFieldCollection tempCollection = MakerShop.Products.InputFieldDataSource.LoadForCriteria("Name = '" + displayName + "' AND ProductTemplateId = " + productTemplateId); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.InputField)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Products.InputField)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.InputFieldId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.ProductTemplateId = productTemplateId;
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.InputFieldId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS
                            ImportInputChoices(schemaObject.InputChoices, newId);
                            // IMPORTING ASSOCIATION

                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportInputChoices(MakerShop.DataClient.Api.Schema.InputChoice[] arrImportObjects, int inputFieldId) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "InputChoice";   //UPDATE             
                Type acType = typeof(MakerShop.Products.InputChoice); //UPDATE
                Type schemaType = typeof(Schema.InputChoice); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.InputChoice schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Products.InputChoice acObject = null; //UPDATE
                    int oldId = schemaObject.InputChoiceId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            acObject = (MakerShop.Products.InputChoice)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.InputChoiceId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = MakerShop.Products.InputChoiceDataSource.Load(oldId); // UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.InputChoice)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            acObject = MakerShop.Products.InputChoiceDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.InputChoice)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Products.InputChoice)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.InputChoiceId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.InputFieldId = inputFieldId;
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.InputChoiceId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        /// <summary>
        /// Imports WrapStyles
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportWrapStyles(MakerShop.DataClient.Api.Schema.WrapStyle[] arrImportObjects, int parentId) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "WrapStyle";   //UPDATE             
                Type acType = typeof(MakerShop.Products.WrapStyle); //UPDATE
                Type schemaType = typeof(Schema.WrapStyle); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.WrapStyle schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Products.WrapStyle acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY
                    int oldId = schemaObject.WrapStyleId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            WrapStyleCollection tempCollection = WrapStyleDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Products.WrapStyle)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.WrapStyleId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            WrapStyleCollection tempCollection = WrapStyleDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WrapStyleDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.WrapStyle)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            WrapStyleCollection tempCollection = WrapStyleDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WrapStyleDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Products.WrapStyle)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                //acObject = WrapStyleDataSource.Load(oldId);// UPDATE
                                if (acObject != null)
                                {
                                    acObject = (MakerShop.Products.WrapStyle)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Products.WrapStyle)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.WrapStyleId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        // TRANSLATE ID's AND ASSOCIATE STORE ID
                        //acObject.StoreId = Token.Instance.StoreId; //VERIFY
                        acObject.WrapGroupId = parentId; // PARENT ID FOR CHILD OBJECTS
                        acObject.TaxCodeId = Translate("TaxCode", schemaObject.TaxCodeId);

                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.WrapStyleId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS


                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }


        /// <summary>
        /// Imports Categories
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportCategories(MakerShop.DataClient.Api.Schema.Category[] arrImportObjects, string csvFields, string matchFields) // UPDATE
        {
            List<String> numaricFields = MakerShop.DataClient.Api.Schema.Category.GetNumaricFields();
            
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Category category = (Schema.Category)schemaObject;
                // TRANSLATE THE CATEGORY PARENT ID IF ITS VALUE IS NON ZERO
                int parentId = 0;
                if (category.ParentId != 0) parentId = Translate("Category", category.ParentId);

                if (String.IsNullOrEmpty(matchFields)) return MakerShop.Catalog.CategoryDataSource.LoadForName(category.Name, category.ParentId);
                else
                {
                    String strCriteria = CalculateCriteria(schemaObject, matchFields, numaricFields);
                    CategoryCollection categories = CategoryDataSource.LoadForCriteria(strCriteria);
                    if (categories.Count == 0) return null;
                    else if (categories.Count == 1) return categories[0];
                    else return "-1"; // INIDCATE THAT THERE ARE MORE THEN ONE MATCHING OBJECTS, SO WE CAN NOT UPDATE
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Catalog.Category acCategory = (Catalog.Category)acObject;
                Schema.Category schemaCategory = (Schema.Category)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acCategory.StoreId = Token.Instance.StoreId; //VERIFY
                // TRANSLATE THE CATEGORY PARENT ID IF ITS VALUE IS NON ZERO
                if(schemaCategory.ParentId != 0) acCategory.ParentId = Translate("Category", schemaCategory.ParentId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Catalog.Category acCategory = (Catalog.Category)acObject;
                Schema.Category schemaCategory = (Schema.Category)schemaObject;

                // IMPORTING ASSOCIATION
                ImportDynamicAssociations(schemaCategory.CategoryVolumeDiscounts,     // Update
                                    acCategory.CategoryVolumeDiscounts,               // Update  
                                    acCategory.CategoryId,
                                    typeof(Marketing.CategoryVolumeDiscount),       // Update  
                                    "VolumeDiscountId",
                                    "VolumeDiscount");

                //TODO: CatalogNode need more clarification

                // CategoryParents

                if (schemaCategory.Parents != null)
                {
                    CategoryParentCollection acCategoryParents = CategoryParentDataSource.LoadForCategory(acCategory.CategoryId);
                    foreach (Schema.CategoryParent catParent in schemaCategory.Parents)
                    {
                        // IF PARENT ALREADY EXISTS
                        int catId = Translate("Category", catParent.CategoryId);
                        int prntId = Translate("Category", catParent.ParentId);
                        if (catId == 0 || prntId == 0) continue;

                        if (acCategoryParents.IndexOf(catId, prntId) < 0)
                        {

                            Catalog.CategoryParent parent = new Catalog.CategoryParent(catId, acCategory.ParentId);
                            parent.ParentNumber = catParent.ParentNumber;
                            parent.ParentLevel = catParent.ParentLevel;
                            acCategoryParents.Add(parent);
                        }
                    }
                    acCategoryParents.Save();
                }
            };

            ImportObjectsArray(arrImportObjects, typeof(MakerShop.Catalog.Category), "Category", "CategoryId", "Name",
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations, csvFields);
        }
        

        /// <summary>
        /// Imports Links
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportLinks(MakerShop.DataClient.Api.Schema.Link[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Link";   //UPDATE             
                Type acType = typeof(MakerShop.Catalog.Link); //UPDATE
                Type schemaType = typeof(Schema.Link); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Link schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Catalog.Link acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY
                    int oldId = schemaObject.LinkId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            LinkCollection tempCollection = LinkDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Catalog.Link)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.LinkId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            LinkCollection tempCollection = LinkDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = LinkDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Catalog.Link)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            LinkCollection tempCollection = LinkDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = LinkDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Catalog.Link)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                //acObject = LinkDataSource.Load(oldId);// UPDATE
                                if (acObject != null)
                                {
                                    acObject = (MakerShop.Catalog.Link)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Catalog.Link)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.LinkId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.LinkId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT ASSOCIATIONs
                            if (!String.IsNullOrEmpty(schemaObject.Categories))
                            {
                                LinkCategoryCollection linkCats = acObject.Categories;
                                String[] idList = schemaObject.Categories.Split(',');
                                foreach (String id in idList)
                                {
                                    int newCatId = Translate("CATEGORY" , AlwaysConvert.ToInt(id));
                                    if (newCatId != 0 && linkCats.IndexOf(newCatId) < 0)
                                    {
                                        linkCats.Add(newCatId);
                                    }
                                }
                                linkCats.Save();
                            }
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }
        /// <summary>
        /// Imports Webpages
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportWebpages(MakerShop.DataClient.Api.Schema.Webpage[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Webpage";   //UPDATE             
                Type acType = typeof(MakerShop.Catalog.Webpage); //UPDATE
                Type schemaType = typeof(Schema.Webpage); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Webpage schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Catalog.Webpage acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY
                    int oldId = schemaObject.WebpageId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            WebpageCollection tempCollection = WebpageDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null && acObject.Summary.Equals(schemaObject.Summary))
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Catalog.Webpage)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.WebpageId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            WebpageCollection tempCollection = WebpageDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WebpageDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Catalog.Webpage)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            WebpageCollection tempCollection = WebpageDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WebpageDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Catalog.Webpage)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                //acObject = WebpageDataSource.Load(oldId);// UPDATE
                                if (acObject != null)
                                {
                                    acObject = (MakerShop.Catalog.Webpage)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Catalog.Webpage)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.WebpageId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.WebpageId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT ASSOCIATIONs
                            if (!String.IsNullOrEmpty(schemaObject.Categories))
                            {
                                WebpageCategoryCollection WebpageCats = acObject.Categories;
                                String[] idList = schemaObject.Categories.Split(',');
                                foreach (String id in idList)
                                {
                                    int newCatId = Translate("CATEGORY" , AlwaysConvert.ToInt(id));
                                    if (newCatId != 0 && WebpageCats.IndexOf(newCatId) < 0)
                                    {
                                        WebpageCats.Add(newCatId);
                                    }
                                }
                                WebpageCats.Save();
                            }
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        ///// <summary>
        ///// Imports EmailLists
        ///// </summary>
        ///// <param name="arrImportObjects"></param>
        //private void ImportEmailLists(MakerShop.DataClient.Api.Schema.EmailList[] arrImportObjects) // UPDATE
        //{
        //    if (arrImportObjects != null && arrImportObjects.Length > 0)
        //    {
        //        String objName = "EmailList";   //UPDATE             
        //        Type acType = typeof(MakerShop.Products.EmailList); //UPDATE
        //        Type schemaType = typeof(Schema.EmailList); //UPDATE
        //        int imported = 0;
        //        int updated = 0;

        //        DataObject objDataObject = new DataObject(acType, schemaType);

        //        log("Importing " + arrImportObjects.Length + " " + objName + "s");
        //        foreach (Schema.EmailList schemaObject in arrImportObjects) //UPDATE
        //        {
        //            MakerShop.Products.EmailList acObject = null; //UPDATE
        //            String displayName = schemaObject.Name; // VERIFY
        //            int oldId = schemaObject.EmailListId; // UPDATE
        //            int newId = 0;
        //            try
        //            {
        //                if (ImportOption == ImportOptions.Import)
        //                {
        //                    // IF IMPORT ONLY OPTION IS SELECTED
        //                    // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
        //                    EmailListCollection tempCollection = EmailListDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
        //                    acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
        //                    if (acObject != null)
        //                    {
        //                        log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
        //                        continue;
        //                    }

        //                    // IMPORT THE OBJECT
        //                    acObject = (MakerShop.Products.EmailList)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
        //                    if (acObject == null)
        //                    {
        //                        log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
        //                        continue;
        //                    }
        //                    schemaObject.EmailListId = 0; // UDPATE

        //                    imported++;
        //                }
        //                else if (ImportOption == ImportOptions.Update)
        //                {
        //                    // IF UPDATE ONLY OPTION IS SELECTED
        //                    EmailListCollection tempCollection = EmailListDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
        //                    acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
        //                    //acObject = EmailListDataSource.Load(oldId);
        //                    if (acObject != null)
        //                    {
        //                        acObject = (MakerShop.Products.EmailList)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
        //                        updated++;
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                }
        //                else if (ImportOption == ImportOptions.ImportOrUpdate)
        //                {
        //                    // IF UPDATE OR IMPORT IMPORT OPTION SELECTED                            
        //                    // TRY TO UPDATE FIRST
        //                    EmailListCollection tempCollection = EmailListDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
        //                    acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
        //                    //acObject = EmailListDataSource.Load(oldId);
        //                    if (acObject != null)
        //                    {
        //                        acObject = (MakerShop.Products.EmailList)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
        //                        //acObject = EmailListDataSource.Load(oldId);// UPDATE
        //                        if (acObject != null)
        //                        {
        //                            acObject = (MakerShop.Products.EmailList)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
        //                            updated++;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // CAN NOT BE UPDATED, SO IMPORT IT
        //                        acObject = (MakerShop.Products.EmailList)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
        //                        acObject.EmailListId = 0;  // UPDATE

        //                        imported++;
        //                    }

        //                }

        //                acObject.StoreId = Token.Instance.StoreId; // VERIFY
        //                if (!acObject.Save().Equals(SaveResult.Failed))
        //                {
        //                    newId = acObject.EmailListId; // UPDATE
        //                    AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

        //                   // IMPORT CHILD OBJECTS
        //                    ImportWrapStyles(schemaObject.WrapStyles, newId);

        //                }
        //                else
        //                {
        //                    log(objName + " Not Saved.\n");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                StringBuilder strLog = new StringBuilder();
        //                strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
        //                if (schemaObject != null)
        //                {
        //                    strLog.AppendLine(objName + "Id= " + oldId);
        //                }
        //                strLog.AppendLine("Error message: " + ex.Message);
        //                log(strLog.ToString());
        //            }
        //        }
        //        log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
        //        log(objName + "s Import Complete...");
        //    }
        //}


        /// <summary>
        /// Imports ShipZones
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportShipZones(MakerShop.DataClient.Api.Schema.ShipZone[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "ShipZone";   //UPDATE             
                Type acType = typeof(MakerShop.Shipping.ShipZone); //UPDATE
                Type schemaType = typeof(Schema.ShipZone); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.ShipZone schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Shipping.ShipZone acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY
                    int oldId = schemaObject.ShipZoneId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            ShipZoneCollection tempCollection = ShipZoneDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Shipping.ShipZone)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.ShipZoneId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            ShipZoneCollection tempCollection = ShipZoneDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = ShipZoneDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipZone)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            ShipZoneCollection tempCollection = ShipZoneDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = ShipZoneDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipZone)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                //acObject = ShipZoneDataSource.Load(oldId);// UPDATE
                                if (acObject != null)
                                {
                                    acObject = (MakerShop.Shipping.ShipZone)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Shipping.ShipZone)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.ShipZoneId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.ShipZoneId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT ASSOCIATIONS
                            //ShipZoneCountries
                            ShipZoneCountryCollection acShipZoneCountries = acObject.ShipZoneCountries;
                            foreach (Schema.ShipZoneCountry schemaShipZoneCountry in schemaObject.ShipZoneCountries)
                            {
                                if (acShipZoneCountries.IndexOf(newId, schemaShipZoneCountry.CountryCode) < 0)
                                {
                                    acShipZoneCountries.Add(new Shipping.ShipZoneCountry(newId, schemaShipZoneCountry.CountryCode));
                                }
                            }
                            acShipZoneCountries.Save();

                            //ShipZoneProvinces
                            ShipZoneProvinceCollection acShipZoneProvinces = acObject.ShipZoneProvinces;
                            foreach (Schema.ShipZoneProvince schemaShipZoneProvince in schemaObject.ShipZoneProvinces)
                            {
                                if (acShipZoneProvinces.IndexOf(newId, schemaShipZoneProvince.ProvinceId) < 0)
                                {
                                    acShipZoneProvinces.Add(new Shipping.ShipZoneProvince(newId, schemaShipZoneProvince.ProvinceId));
                                }
                            }
                            acShipZoneProvinces.Save();
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        strLog.Append(ex.StackTrace);
                        if (ex.GetBaseException() != null)
                        {
                            strLog.Append(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);
                        }
                        logError(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }


        /// <summary>
        /// Imports OrderStatuses
        /// </summary>
        /// <param name="arrImportObjects"></param>
        private void ImportOrderStatuses(MakerShop.DataClient.Api.Schema.OrderStatus[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "OrderStatus";   //UPDATE             
                Type acType = typeof(MakerShop.Orders.OrderStatus); //UPDATE
                Type schemaType = typeof(Schema.OrderStatus); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.OrderStatus schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Orders.OrderStatus acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.OrderStatusId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS

                            OrderStatusCollection tempCollection = MakerShop.Orders.OrderStatusDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Orders.OrderStatus)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.OrderStatusId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            OrderStatusCollection tempCollection = MakerShop.Orders.OrderStatusDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Orders.OrderStatus)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            OrderStatusCollection tempCollection = MakerShop.Orders.OrderStatusDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Orders.OrderStatus)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Orders.OrderStatus)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.OrderStatusId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.OrderStatusId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION
                            ImportDynamicAssociations(schemaObject.OrderStatusEmails,                                   // Update
                                                acObject.OrderStatusEmails,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Orders.OrderStatusEmail),                 // Update  
                                                "EmailTemplateId", "EmailTemplate");                                        // Update  

                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportPaymentMethods(MakerShop.DataClient.Api.Schema.PaymentMethod[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "PaymentMethod";   //UPDATE             
                Type acType = typeof(MakerShop.Payments.PaymentMethod); //UPDATE
                Type schemaType = typeof(Schema.PaymentMethod); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.PaymentMethod schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Payments.PaymentMethod acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.PaymentMethodId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS

                            PaymentMethodCollection tempCollection = MakerShop.Payments.PaymentMethodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Payments.PaymentMethod)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.PaymentMethodId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            PaymentMethodCollection tempCollection = MakerShop.Payments.PaymentMethodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Payments.PaymentMethod)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            PaymentMethodCollection tempCollection = MakerShop.Payments.PaymentMethodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Payments.PaymentMethod)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Payments.PaymentMethod)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.PaymentMethodId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (CanTranslate("PaymentGateway", schemaObject.PaymentGatewayId)) acObject.PaymentGatewayId = Translate("PaymentGateway", schemaObject.PaymentGatewayId);                        
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.PaymentMethodId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION
                            ImportDynamicAssociations(schemaObject.PaymentMethodGroups,                                   // Update
                                                acObject.PaymentMethodGroups,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Payments.PaymentMethodGroup),                 // Update  
                                                "GroupId", "Group");                                        // Update  

                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportShipMethods(MakerShop.DataClient.Api.Schema.ShipMethod[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "ShipMethod";   //UPDATE             
                Type acType = typeof(MakerShop.Shipping.ShipMethod); //UPDATE
                Type schemaType = typeof(Schema.ShipMethod); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.ShipMethod schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Shipping.ShipMethod acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY

                    int oldId = schemaObject.ShipMethodId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS

                            MakerShop.Shipping.ShipMethodCollection tempCollection = MakerShop.Shipping.ShipMethodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Shipping.ShipMethod)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.ShipMethodId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Shipping.ShipMethodCollection tempCollection = MakerShop.Shipping.ShipMethodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipMethod)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            MakerShop.Shipping.ShipMethodCollection tempCollection = MakerShop.Shipping.ShipMethodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipMethod)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Shipping.ShipMethod)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.ShipMethodId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.StoreId = Token.Instance.StoreId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.ShipMethodId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION

                            ImportDynamicAssociations(schemaObject.ShipMethodWarehouses,                                   // Update
                                                acObject.ShipMethodWarehouses,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Shipping.ShipMethodWarehouse),                 // Update  
                                                "WarehouseId", "Warehouse");                                        // Update  

                            ImportDynamicAssociations(schemaObject.ShipMethodGroups,                                   // Update
                                                acObject.ShipMethodGroups,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Shipping.ShipMethodGroup),                 // Update  
                                                "GroupId", "Group");                                        // Update  

                            ImportDynamicAssociations(schemaObject.ShipMethodShipZones,                                   // Update
                                                acObject.ShipMethodShipZones,                                      // Update  
                                                newId,
                                                typeof(MakerShop.Shipping.ShipMethodShipZone),                 // Update  
                                                "ShipZoneId", "ShipZone");                                        // Update  

                            ImportShipRateMatrices(schemaObject.ShipRateMatrices, newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportShipRateMatrices(MakerShop.DataClient.Api.Schema.ShipRateMatrix[] arrImportObjects, int shipMethodId) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "ShipRateMatrix";   //UPDATE             
                Type acType = typeof(MakerShop.Shipping.ShipRateMatrix); //UPDATE
                Type schemaType = typeof(Schema.ShipRateMatrix); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.ShipRateMatrix schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Shipping.ShipRateMatrix acObject = null; //UPDATE

                    int oldId = schemaObject.ShipMethodId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS

                            acObject = MakerShop.Shipping.ShipRateMatrixDataSource.Load(schemaObject.ShipRateMatrixId);
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same id already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.Shipping.ShipRateMatrix)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.ShipRateMatrixId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = MakerShop.Shipping.ShipRateMatrixDataSource.Load(schemaObject.ShipRateMatrixId);
                            //acObject = EmailTemplateDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipRateMatrix)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            acObject = MakerShop.Shipping.ShipRateMatrixDataSource.Load(schemaObject.ShipRateMatrixId);
                            //acObject = EmailTemplateDataSource.Load(oldId);// UPDATE
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Shipping.ShipRateMatrix)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Shipping.ShipRateMatrix)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.ShipMethodId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.ShipMethodId = shipMethodId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.ShipRateMatrixId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS

                            // IMPORTING ASSOCIATION
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportDigitalGoods(MakerShop.DataClient.Api.Schema.DigitalGood[] arrImportObjects) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "DigitalGood";   //UPDATE             
                Type acType = typeof(MakerShop.DigitalDelivery.DigitalGood); //UPDATE
                Type schemaType = typeof(Schema.DigitalGood); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.DigitalGood schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.DigitalDelivery.DigitalGood acObject = null; //UPDATE
                    String displayName = schemaObject.Name; // VERIFY
                    int oldId = schemaObject.DigitalGoodId; // UPDATE
                    int newId = 0;
                    try
                    {
                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            // CHECK IF AN OBJECT WITH SAME NAME ALREADY EXISTS
                            MakerShop.DigitalDelivery.DigitalGoodCollection tempCollection = MakerShop.DigitalDelivery.DigitalGoodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                log("Skipping import, " + objName + " with same name (" + displayName + ") already exists.");
                                continue;
                            }

                            // IMPORT THE OBJECT
                            acObject = (MakerShop.DigitalDelivery.DigitalGood)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.DigitalGoodId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.DigitalDelivery.DigitalGoodCollection tempCollection = MakerShop.DigitalDelivery.DigitalGoodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WrapStyleDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.DigitalDelivery.DigitalGood)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
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
                            MakerShop.DigitalDelivery.DigitalGoodCollection tempCollection = MakerShop.DigitalDelivery.DigitalGoodDataSource.LoadForCriteria("Name = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            //acObject = WrapStyleDataSource.Load(oldId);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.DigitalDelivery.DigitalGood)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                //acObject = WrapStyleDataSource.Load(oldId);// UPDATE
                                if (acObject != null)
                                {
                                    acObject = (MakerShop.DigitalDelivery.DigitalGood)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                    updated++;
                                }
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.DigitalDelivery.DigitalGood)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.DigitalGoodId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        // TRANSLATE ID's AND ASSOCIATE STORE ID
                        acObject.StoreId = Token.Instance.StoreId; //VERIFY
                        if (CanTranslate("EmailTemplate", schemaObject.ActivationEmailId)) acObject.ActivationEmailId = Translate("EmailTemplate", schemaObject.ActivationEmailId);
                        if (CanTranslate("EmailTemplate", schemaObject.FulfillmentEmailId)) acObject.FulfillmentEmailId = Translate("EmailTemplate", schemaObject.FulfillmentEmailId);
                        if (CanTranslate("Readme", schemaObject.ReadmeId)) acObject.ReadmeId = Translate("Readme", schemaObject.ReadmeId);
                        if (CanTranslate("LicenseAgreement", schemaObject.LicenseAgreementId)) acObject.LicenseAgreementId = Translate("LicenseAgreement", schemaObject.LicenseAgreementId);

                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.DigitalGoodId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            // IMPORT CHILD OBJECTS


                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportCustomFields(MakerShop.DataClient.Api.Schema.CustomField[] arrImportObjects, string tableName) // UPDATE
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "CustomField";   //UPDATE             
                Type acType = typeof(MakerShop.Stores.CustomField); //UPDATE
                Type schemaType = typeof(Schema.CustomField); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.CustomField schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Stores.CustomField acObject = null; //UPDATE
                    String displayName = schemaObject.FieldName; // VERIFY

                    int oldId = schemaObject.CustomFieldId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED

                            MakerShop.Stores.CustomFieldCollection tempCollection = MakerShop.Stores.CustomFieldDataSource.LoadForCriteria("FieldName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.CustomFieldId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            MakerShop.Stores.CustomFieldCollection tempCollection = MakerShop.Stores.CustomFieldDataSource.LoadForCriteria("FieldName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Stores.CustomField)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                updated++;
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
                            MakerShop.Stores.CustomFieldCollection tempCollection = MakerShop.Stores.CustomFieldDataSource.LoadForCriteria("FieldName = '" + displayName + "'"); // Update
                            acObject = (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Stores.CustomField)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Stores.CustomField)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.CustomFieldId = 0;  // UPDATE

                                imported++;
                            }

                        }
                        acObject.StoreId = Token.Instance.Store.StoreId;

                        switch (tableName)
                        {
                            case "User":
                                if (CanTranslate("User", schemaObject.ForeignKeyId)) acObject.ForeignKeyId = Translate("User", schemaObject.ForeignKeyId);
                                break;
                            case "Product":
                                if (CanTranslate("Product", schemaObject.ForeignKeyId)) acObject.ForeignKeyId = Translate("Product", schemaObject.ForeignKeyId);
                                break;
                        }

                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.CustomFieldId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);
                        }
                        else
                        {
                            log(objName + " Not Saved.\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for " + objName);
                        if (schemaObject != null)
                        {
                            strLog.AppendLine(objName + "Id= " + oldId);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + " " + objName + "(s) imported, " + updated + " " + objName + "(s) updated.");
                log(objName + "s Import Complete...");
            }
        }

        private void ImportComponents(MakerShop.DataClient.Api.Schema.KitComponent[] arrImportObjects)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.KitComponent kitComponent = (Schema.KitComponent)schemaObject;
                return KitComponentDataSource.Load(kitComponent.KitComponentId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.KitComponent acKitComponent = (Products.KitComponent)acObject;
                Schema.KitComponent schemaKitComponent = (Schema.KitComponent)schemaObject;
                acKitComponent.StoreId = Token.Instance.StoreId; // VERIFY
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Products.KitComponent acKitComponent = (Products.KitComponent)acObject;
                Schema.KitComponent schemaKitComponent = (Schema.KitComponent)schemaObject;
                ImportKitProducts(schemaKitComponent.KitProducts, acKitComponent.KitComponentId);
            };

            ImportObjectsArray(arrImportObjects, typeof(MakerShop.Products.KitComponent), "KitComponent", "KitComponentId", String.Empty,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);            
        }

        private void ImportKitProducts(MakerShop.DataClient.Api.Schema.KitProduct[] arrImportObjects,int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.KitProduct kitProduct = (Schema.KitProduct)schemaObject;
                return KitProductDataSource.Load(kitProduct.KitProductId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.KitProduct acKitProduct = (Products.KitProduct)acObject;
                Schema.KitProduct schemaKitProduct = (Schema.KitProduct)schemaObject;
                acKitProduct.KitComponentId = parentId;
                if (CanTranslate("Product", schemaKitProduct.ProductId)) acKitProduct.ProductId = Translate("ProductId", schemaKitProduct.ProductId);
                
                //if (CanTranslate("ProductVariant", schemaKitProduct.ProductVariantId)) acKitProduct.ProductVariantId = Translate("ProductVariant", schemaKitProduct.ProductVariantId);
                //ProductVariant
                if (!String.IsNullOrEmpty(schemaKitProduct.OptionList))
                {
                    int[] schemaOptionChoiceIds = AlwaysConvert.ToIntArray(schemaKitProduct.OptionList);
                    string[] acOptionChoiceIds = new string[schemaOptionChoiceIds.Length];
                    for (int i = 0; i < schemaOptionChoiceIds.Length; i++)
                    {
                        acOptionChoiceIds[i] = Translate("OptionChoice", schemaOptionChoiceIds[i]).ToString();
                    }

                    acKitProduct.OptionList = String.Join(",", acOptionChoiceIds);
                }
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Products.KitProduct acKitProduct = (Products.KitProduct)acObject;
                Schema.KitProduct schemaKitProduct = (Schema.KitProduct)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.KitProduct), "KitProduct", "KitProductId", String.Empty,parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        private void ImportPageViews(PageView[] arrImportObjects)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.PageView pageView = (Schema.PageView)schemaObject;
                return MakerShop.Reporting.PageViewDataSource.Load(pageView.PageViewId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Reporting.PageView acPageView = (Reporting.PageView)acObject;
                Schema.PageView schemaPageView = (Schema.PageView)schemaObject;
                acPageView.StoreId = Token.Instance.StoreId; // VERIFY
                if (CanTranslate("User", schemaPageView.UserId)) acPageView.UserId = Translate("User", schemaPageView.UserId);
                if (CanTranslate("CatalogNode", schemaPageView.CatalogNodeId)) acPageView.CatalogNodeId = Translate("CatalogNode", schemaPageView.CatalogNodeId);
                if (CanTranslate("Affiliate", schemaPageView.AffiliateId)) acPageView.AffiliateId = Translate("Affiliate", schemaPageView.AffiliateId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Reporting.PageView acPageView = (Reporting.PageView)acObject;
                Schema.PageView schemaPageView = (Schema.PageView)schemaObject;
            };

            ImportObjectsArray(arrImportObjects, typeof(MakerShop.Reporting.PageView), "PageView", "PageViewId", String.Empty,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);            
        }

    }
}
