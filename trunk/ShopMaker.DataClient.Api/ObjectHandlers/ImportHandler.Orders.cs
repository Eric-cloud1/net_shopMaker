using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
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
        // PRIVATE IMPORT HELPER METHODS FOR ORDERS IMPORT
        
        #region ImportOrders
        private void ImportOrders(MakerShop.DataClient.Api.Schema.Order[] arrImportObjects, String csvFields, String matchFields)
        {
            List<String> numaricFields = MakerShop.DataClient.Api.Schema.Order.GetNumaricFields();
            
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                if (String.IsNullOrEmpty(matchFields)) return OrderDataSource.Load(((Schema.Order)schemaObject).OrderId);
                else
                {
                    String strCriteria = CalculateCriteria(schemaObject, matchFields, numaricFields);
                    OrderCollection orders = OrderDataSource.LoadForCriteria(strCriteria);
                    if (orders.Count == 0) return null;
                    else if (orders.Count == 1) return orders[0];
                    else return "-1"; // INIDCATE THAT THERE ARE MORE THEN ONE MATCHING OBJECTS, SO WE CAN NOT UPDATE
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.Order acOrder = (Orders.Order)acObject;
                Schema.Order schemaOrder = (Schema.Order)schemaObject;

                // ORDER NUMBER SHOULD NEVER BE ZERO, BUT SHOULD BE NEXT AVAILABLE NUMBER
                if (acOrder.OrderNumber == 0)
                {
                    // NEW ORDER
                    acOrder.OrderNumber = StoreDataSource.GetNextOrderNumber(true);
                }

                acOrder.StoreId = Token.Instance.StoreId;
                
                // USERID
                if(CanTranslate("User",schemaOrder.UserId)) acOrder.UserId =  Translate("User",schemaOrder.UserId);
                else if (!String.IsNullOrEmpty(schemaOrder.UserName))
                {
                    int newId = GetUserId(schemaOrder.UserName);
                    if(newId > 0){
                        acOrder.UserId = newId ;
                        if(schemaOrder.UserId > 0) AddToTranslationDic("User"+schemaOrder.UserId,newId );
                    }
                }

                //AffiliateId
                if(CanTranslate("Affiliate",schemaOrder.AffiliateId)) acOrder.AffiliateId =  Translate("Affiliate",schemaOrder.AffiliateId);
                else if (!String.IsNullOrEmpty(schemaOrder.Affiliate))
                {
                    int newId = GetAffiliateId(schemaOrder.Affiliate);
                    if(newId > 0){
                        acOrder.AffiliateId = newId ;
                        AddToTranslationDic("Affiliate"+schemaOrder.AffiliateId,newId );
                    }
                }

                //OrderStatusId
                if(CanTranslate("OrderStatus",schemaOrder.OrderStatusId)) acOrder.OrderStatusId =  Translate("OrderStatus",schemaOrder.OrderStatusId);
                else if (!String.IsNullOrEmpty(schemaOrder.OrderStatus))
                {
                    int newId = GetOrderStatusId(schemaOrder.OrderStatus);
                    if(newId > 0){
                        acOrder.OrderStatusId = newId ;
                        AddToTranslationDic("OrderStatus"+schemaOrder.OrderStatusId,newId );
                    }
                }

            };

            // SAVE ORDER WITHOUT RE-CALCULATING THE PAYMENT AND SHIPPING STATUS
            SaveACObject saveACObject = delegate(ref object acObject)
            {
                Orders.Order acOrder = (Orders.Order)acObject;
                return acOrder.Save(false);
            }; 

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.Order acOrder = (Orders.Order)acObject;
                Schema.Order schemaOrder = (Schema.Order)schemaObject;

                ImportOrderShipments(schemaOrder.Shipments, acOrder);
                ImportOrderItems(schemaOrder.OrderItems,acOrder);
                ImportOrderCoupons(schemaOrder.Coupons,acOrder);
                ImportOrderPayments(schemaOrder.Payments,acOrder);
                ImportOrderNotes(schemaOrder.Notes, acOrder);
            };

            ImportObjectsArray(arrImportObjects,typeof(MakerShop.Orders.Order),"Order","OrderId",String.Empty,
                tryLoadExistingObject, translateObjectAssociatedIds,saveACObject , importObjectChildsAndAssociations, csvFields);
        }        

        private void ImportOrderNotes(MakerShop.DataClient.Api.Schema.OrderNote[] arrImportObjects, MakerShop.Orders.Order acOrder)
        {
            int parentId = acOrder.OrderId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.OrderNote schemaNote = (Schema.OrderNote)schemaObject;
                if (acOrder.Notes.IndexOf(schemaNote.OrderNoteId) < 0) return null;
                else
                {
                    int index = acOrder.Notes.IndexOf(schemaNote.OrderNoteId);
                    return acOrder.Notes[index];
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.OrderNote acNote = (Orders.OrderNote)acObject;
                Schema.OrderNote schemaNote = (Schema.OrderNote)schemaObject;

                //acNote.StoreId = Token.Instance.StoreId;
                acNote.OrderId = parentId;
                if (CanTranslate("User", schemaNote.UserId)) acNote.UserId = Translate("User", schemaNote.UserId);                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.OrderNote acNote = (Orders.OrderNote)acObject;
                Schema.OrderNote schemaNote = (Schema.OrderNote)schemaObject;
                
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.OrderNote), "OrderNote", "OrderNoteId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

       #endregion

        #region ImportOrderCoupons
        private void ImportOrderCoupons(MakerShop.DataClient.Api.Schema.OrderCoupon[] schemaOrderCoupons, MakerShop.Orders.Order acOrder)
        {
            if (IsNonEmptyArray(schemaOrderCoupons))
            {
                OrderCouponCollection acOrderCouponCollection = acOrder.Coupons;
                MakerShop.Orders.OrderCoupon acOrderCoupon = null;
                MakerShop.Marketing.Coupon acCoupon = null;
                foreach (Schema.OrderCoupon schemaOrderCoupon in schemaOrderCoupons)
                {
                    acCoupon = CouponDataSource.LoadForCouponCode(schemaOrderCoupon.CouponCode);
                    if (acCoupon != null)
                    {
                        //int index = objShipMethodGroupCollection.IndexOf(objClientApiShipMethod.ShipMethodId, AlwaysConvert.ToInt(strGroup));
                        int index = acOrderCouponCollection.IndexOf(acOrder.OrderId, schemaOrderCoupon.CouponCode);
                        if (index > -1)
                        {
                            //Already exist no need to add
                        }
                        else
                        {
                            //if exist in database load and add
                            acOrderCoupon = OrderCouponDataSource.Load(acOrder.OrderId, schemaOrderCoupon.CouponCode);
                            if (acOrderCoupon != null)
                            {
                                acOrderCouponCollection.Add(acOrderCoupon);
                            }
                            else
                            {
                                acOrderCoupon = new MakerShop.Orders.OrderCoupon(acOrder.OrderId, schemaOrderCoupon.CouponCode);
                                acOrderCoupon.Save();
                                acOrderCouponCollection.Add(acOrderCoupon);
                            }
                        }
                    }
                    //if coupon does not exits
                    else
                    {
                        //Create a new Coupon
                        acCoupon = new MakerShop.Marketing.Coupon();
                        acCoupon.CouponCode = schemaOrderCoupon.CouponCode;
                        if (!acCoupon.Save().Equals(SaveResult.Failed))
                        {
                            acOrderCoupon = new MakerShop.Orders.OrderCoupon(acOrder.OrderId, schemaOrderCoupon.CouponCode);
                            acOrderCoupon.Save();
                            acOrderCouponCollection.Add(acOrderCoupon);
                            acOrderCouponCollection.Save();
                        }
                    }
                }
            }
        }

        #endregion

        #region ImportOrderPayments

        private void ImportOrderPayments(MakerShop.DataClient.Api.Schema.Payment[] arrImportObjects, MakerShop.Orders.Order acOrder)
        {
            int parentId = acOrder.OrderId;
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "Payment";   //UPDATE             
                Type acType = typeof(MakerShop.Payments.Payment); //UPDATE
                Type schemaType = typeof(Schema.Payment); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.Payment schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Payments.Payment acObject = null; //UPDATE
                    //String displayName = String.Empty; // VERIFY

                    int oldId = schemaObject.PaymentId; // UPDATE
                    int newId = 0;
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            if (acOrder.Payments.IndexOf(schemaObject.PaymentId) < 0) acObject = null;
                            else
                            {
                                int index = acOrder.Payments.IndexOf(schemaObject.PaymentId);
                                acObject = acOrder.Payments[index];
                            }

                            if (acObject == null)
                            {
                                log("Error:" + objName + " with Id (" + oldId + ") can not be imported.");
                                continue;
                            }
                            schemaObject.PaymentId = 0; // UDPATE

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            if (acOrder.Payments.IndexOf(schemaObject.PaymentId) < 0) acObject = null;
                            else
                            {
                                int index = acOrder.Payments.IndexOf(schemaObject.PaymentId);
                                acObject = acOrder.Payments[index];
                            }

                            if (acObject != null)
                            {
                                String encryptedAccountData = acObject.EncryptedAccountData;
                                acObject = (MakerShop.Payments.Payment)objDataObject.UpdateToAC6Object(schemaObject, acObject);
                                //RESTORE THE "EncryptedAccountData" data
                                if (String.IsNullOrEmpty(schemaObject.EncryptedAccountData)) acObject.EncryptedAccountData = encryptedAccountData;
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
                            if (acOrder.Payments.IndexOf(schemaObject.PaymentId) < 0) acObject = null;
                            else
                            {
                                int index = acOrder.Payments.IndexOf(schemaObject.PaymentId);
                                acObject = acOrder.Payments[index];
                            }

                            if (acObject != null)
                            {
                                String encryptedAccountData = acObject.EncryptedAccountData;
                                acObject = (MakerShop.Payments.Payment)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                //RESTORE THE "EncryptedAccountData" data
                                if (String.IsNullOrEmpty(schemaObject.EncryptedAccountData)) acObject.EncryptedAccountData = encryptedAccountData;
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Payments.Payment)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                                acObject.PaymentId = 0;  // UPDATE

                                imported++;
                            }

                        }

                        acObject.OrderId = parentId;

                        if (CanTranslate("Subscription", schemaObject.SubscriptionId)) acObject.SubscriptionId = Translate("Subscription", schemaObject.SubscriptionId);
                        if (CanTranslate("PaymentMethod", schemaObject.PaymentMethodId)) acObject.PaymentMethodId = Translate("PaymentMethod", schemaObject.PaymentMethodId);

                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            newId = acObject.PaymentId; // UPDATE
                            AddToTranslationDic(objName.ToUpperInvariant() + oldId.ToString(), newId);

                            //ASSOCIATIONS AND CHILD
                            ImportTransactions(schemaObject.Transactions, acObject);   
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


        private void ImportTransactions(MakerShop.DataClient.Api.Schema.Transaction[] arrImportObjects, Payments.Payment acPayment)
        {
            int parentId = acPayment.PaymentId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Transaction schemaTransaction = (Schema.Transaction)schemaObject;
                if (acPayment.Transactions.IndexOf(schemaTransaction.TransactionId) < 0) return null;
                else
                {
                    int index = acPayment.Transactions.IndexOf(schemaTransaction.TransactionId);
                    return acPayment.Transactions[index];
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Payments.Transaction acTransaction = (Payments.Transaction)acObject;
                Schema.Transaction schemaTransaction = (Schema.Transaction)schemaObject;

                //acTransaction.StoreId = Token.Instance.StoreId;
                acTransaction.PaymentId = parentId;

                if (CanTranslate("PaymentGateway", schemaTransaction.PaymentGatewayId)) acTransaction.PaymentGatewayId = Translate("PaymentGateway", schemaTransaction.PaymentGatewayId);                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Payments.Transaction acTransaction = (Payments.Transaction)acObject;
                Schema.Transaction schemaTransaction = (Schema.Transaction)schemaObject;
                
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Payments.Transaction), "Transaction", "TransactionId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region ImportOrderShipments

        private void ImportOrderShipments(MakerShop.DataClient.Api.Schema.OrderShipment[] arrImportObjects, MakerShop.Orders.Order acOrder)
        {
            int parentId = acOrder.OrderId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.OrderShipment schemaOrderShipment = (Schema.OrderShipment)schemaObject;
                if (acOrder.Shipments.IndexOf(schemaOrderShipment.OrderShipmentId) < 0) return null;
                else
                {
                    int index = acOrder.Shipments.IndexOf(schemaOrderShipment.OrderShipmentId);
                    return acOrder.Shipments[index];
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.OrderShipment acOrderShipment = (Orders.OrderShipment)acObject;
                Schema.OrderShipment schemaOrderShipment= (Schema.OrderShipment)schemaObject;

                acOrderShipment.OrderId = parentId;

                if (CanTranslate("WarehouseId", schemaOrderShipment.WarehouseId)) acOrderShipment.WarehouseId = Translate("Warehouse", schemaOrderShipment.WarehouseId);
                if (CanTranslate("ShipMethod", schemaOrderShipment.ShipMethodId)) acOrderShipment.ShipMethodId = Translate("ShipMethod", schemaOrderShipment.ShipMethodId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.OrderShipment acOrderShipment = (Orders.OrderShipment)acObject;
                Schema.OrderShipment schemaOrderShipment = (Schema.OrderShipment)schemaObject;

                //ASSOCIATIONS AND CHILD
                ImportTrackingNumbers(schemaOrderShipment.TrackingNumbers, acOrderShipment);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.OrderShipment), "OrderShipment", "OrderShipmentId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);    
        }

        private void ImportTrackingNumbers(MakerShop.DataClient.Api.Schema.TrackingNumber[] arrImportObjects, MakerShop.Orders.OrderShipment acOrderShipment)
        {
            int parentId = acOrderShipment.OrderShipmentId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.TrackingNumber schemaTrackingNumber = (Schema.TrackingNumber)schemaObject;
                if (acOrderShipment.TrackingNumbers.IndexOf(schemaTrackingNumber.TrackingNumberId) < 0) return null;
                else
                {
                    int index = acOrderShipment.TrackingNumbers.IndexOf(schemaTrackingNumber.TrackingNumberId);
                    return acOrderShipment.TrackingNumbers[index];
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.TrackingNumber acTrackingNumber = (Orders.TrackingNumber)acObject;
                Schema.TrackingNumber schemaTrackingNumber = (Schema.TrackingNumber)schemaObject;
                acTrackingNumber.OrderShipmentId = parentId;
                if (CanTranslate("ShipGateway", schemaTrackingNumber.ShipGatewayId)) acTrackingNumber.ShipGatewayId = Translate("ShipGateway", schemaTrackingNumber.ShipGatewayId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.TrackingNumber acTrackingNumber = (Orders.TrackingNumber)acObject;
                Schema.TrackingNumber schemaTrackingNumber = (Schema.TrackingNumber)schemaObject;

                //ASSOCIATIONS AND CHILD
                //ImportTransactions(schemaPayment.Transactions, acPayment);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.TrackingNumber), "TrackingNumber", "TrackingNumberId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }
        #endregion

        #region ImportOrderItems


        private void ImportOrderItems(MakerShop.DataClient.Api.Schema.OrderItem[] arrImportObjects, MakerShop.Orders.Order acOrder)
        {
            int parentId = acOrder.OrderId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.OrderItem schemaOrderItem = (Schema.OrderItem)schemaObject;
                if (acOrder.Items.IndexOf(schemaOrderItem.OrderItemId) < 0) return null;
                else
                {
                    int index =  acOrder.Items.IndexOf(schemaOrderItem.OrderItemId);
                    return acOrder.Items[index];
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.OrderItem acOrderItem = (Orders.OrderItem)acObject;
                Schema.OrderItem schemaOrderItem = (Schema.OrderItem)schemaObject;

                //acOrderItem.StoreId = Token.Instance.StoreId;
                acOrderItem.OrderId = parentId;

                if (CanTranslate("OrderShipment", schemaOrderItem.OrderShipmentId)) acOrderItem.OrderShipmentId = Translate("OrderShipment", schemaOrderItem.OrderShipmentId);
                if (CanTranslate("Product", schemaOrderItem.ProductId)) acOrderItem.ProductId = Translate("Product", schemaOrderItem.ProductId);

                //if (CanTranslate("ProductVariant", schemaOrderItem.ProductVariantId)) acOrderItem.ProductVariantId = Translate("ProductVariant", schemaOrderItem.ProductVariantId);
                //else if (acOrderItem.ProductVariantId != 0)
                //{
                //    // CHECK IN DATA BASE FOR VARIANT ID IF NOT FOUND SET IT TO ZERO
                //    MakerShop.Products.ProductVariant variant = ProductVariantDataSource.Load(acOrderItem.ProductVariantId);
                //    if (variant == null) acOrderItem.ProductVariantId = 0;
                //}
                if (!String.IsNullOrEmpty(schemaOrderItem.OptionList))
                {
                    int[] schemaOptionChoiceIds = AlwaysConvert.ToIntArray(schemaOrderItem.OptionList);
                    string[] acOptionChoiceIds = new string[schemaOptionChoiceIds.Length];
                    for (int i = 0; i < schemaOptionChoiceIds.Length; i++)
                    {
                        acOptionChoiceIds[i] = Translate("OptionChoice", schemaOptionChoiceIds[i]).ToString();
                    }

                    acOrderItem.OptionList = String.Join(",", acOptionChoiceIds);
                }

                // KitList
                if (!String.IsNullOrEmpty(schemaOrderItem.KitList))
                {
                    int[] schemaKitProductIds = AlwaysConvert.ToIntArray(schemaOrderItem.KitList);
                    string[] acKitProductIds = new string[schemaKitProductIds.Length];
                    for (int i = 0; i < schemaKitProductIds.Length; i++)
                    {
                        acKitProductIds[i] = Translate("KitProduct", schemaKitProductIds[i]).ToString();
                    }

                    acOrderItem.KitList = String.Join(",", acKitProductIds);
                }

                //CustomFields: Let them import automatically, no manual conversion is needed
                
                if (CanTranslate("TaxCode", schemaOrderItem.TaxCodeId)) acOrderItem.TaxCodeId = Translate("TaxCode", schemaOrderItem.TaxCodeId);
                if (CanTranslate("WrapStyle", schemaOrderItem.WrapStyleId)) acOrderItem.WrapStyleId = Translate("WrapStyle", schemaOrderItem.WrapStyleId);
                if (CanTranslate("WishlistItem", schemaOrderItem.WishlistItemId)) acOrderItem.WishlistItemId = Translate("WishlistItem", schemaOrderItem.WishlistItemId);                
                
                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject) {
                Orders.OrderItem acOrderItem = (Orders.OrderItem)acObject;
                Schema.OrderItem schemaOrderItem = (Schema.OrderItem)schemaObject;

                //ASSOCIATIONS AND CHILD
                ImportOrderItemInputs(schemaOrderItem.Inputs, acOrderItem);
                ImportOrderItemGiftCertificates(schemaOrderItem.GiftCertificates, acOrderItem);
                ImportOrderItemDigitalGoods(schemaOrderItem.OrderItemDigitalGoods, acOrderItem);
                ImportSubscriptions(schemaOrderItem.Subscriptions, acOrderItem);

                //ParentItemId
                if (CanTranslate("OrderItem", schemaOrderItem.ParentItemId)) acOrderItem.ParentItemId = Translate("OrderItem", schemaOrderItem.ParentItemId);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.OrderItem), "OrderItem", "OrderItemId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region ImportOrderItemInputs

        private void ImportOrderItemInputs(MakerShop.DataClient.Api.Schema.OrderItemInput[] arrImportObjects, MakerShop.Orders.OrderItem acOrderItem)
        {

            int parentId = acOrderItem.OrderItemId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.OrderItemInput schemaOrderItemInput = (Schema.OrderItemInput)schemaObject;
                if (acOrderItem.Inputs.IndexOf(schemaOrderItemInput.OrderItemInputId) < 0) return null;
                else
                {
                    int index = acOrderItem.Inputs.IndexOf(schemaOrderItemInput.OrderItemInputId);
                    return acOrderItem.Inputs[index];
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.OrderItemInput acOrderItemInput = (Orders.OrderItemInput)acObject;
                Schema.OrderItemInput schemaOrderItemInput = (Schema.OrderItemInput)schemaObject;

                acOrderItemInput.OrderItemId = parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {                
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.OrderItemInput), "OrderItemInput", "OrderItemInputId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region ImportOrderItemGiftCertificates

        private void ImportOrderItemGiftCertificates(MakerShop.DataClient.Api.Schema.GiftCertificate[] arrImportObjects, MakerShop.Orders.OrderItem acOrderItem)
        {
            int parentId = acOrderItem.OrderItemId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.GiftCertificate schemaGiftCertificate = (Schema.GiftCertificate)schemaObject;

                if (acOrderItem.GiftCertificates.IndexOf(schemaGiftCertificate.GiftCertificateId) < 0) return null;
                else
                {
                    int index = acOrderItem.GiftCertificates.IndexOf(schemaGiftCertificate.GiftCertificateId);
                    return acOrderItem.GiftCertificates[index];
                }
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Payments.GiftCertificate acGiftCertificate = (Payments.GiftCertificate)acObject;

                acGiftCertificate.StoreId = Token.Instance.StoreId;
                acGiftCertificate.OrderItemId = parentId;                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Schema.GiftCertificate schemaGiftCertificate = (Schema.GiftCertificate)schemaObject;
                Payments.GiftCertificate acGiftCertificate = (Payments.GiftCertificate)acObject;

                ImportGiftCertificateTransactions(schemaGiftCertificate.Transactions,acGiftCertificate);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Payments.GiftCertificate), "GiftCertificate", "GiftCertificateId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion


        #region ImportGiftCertificateTransactions
        private void ImportGiftCertificateTransactions(MakerShop.DataClient.Api.Schema.GiftCertificateTransaction[] arrImportObjects, MakerShop.Payments.GiftCertificate acGiftCertificate)
        {
            int parentId = acGiftCertificate.GiftCertificateId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.GiftCertificateTransaction schemaGiftCertificateTransaction = (Schema.GiftCertificateTransaction)schemaObject;

                if (acGiftCertificate.Transactions.IndexOf(schemaGiftCertificateTransaction.GiftCertificateTransactionId) < 0) return null;
                else
                {
                    int index = acGiftCertificate.Transactions.IndexOf(schemaGiftCertificateTransaction.GiftCertificateTransactionId);
                    return acGiftCertificate.Transactions[index];
                }
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Payments.GiftCertificateTransaction acGiftCertificateTransaction = (Payments.GiftCertificateTransaction)acObject;

                //acGiftCertificateTransaction.StoreId = Token.Instance.StoreId;
                acGiftCertificateTransaction.GiftCertificateId = parentId;
                acGiftCertificateTransaction.OrderId = acGiftCertificate.OrderItem.OrderId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {                
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Payments.GiftCertificateTransaction), "GiftCertificateTransaction", "GiftCertificateTransactionId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }
        #endregion

        #region ImportOrderItemDigitalGoods

        private void ImportOrderItemDigitalGoods(MakerShop.DataClient.Api.Schema.OrderItemDigitalGood[] arrImportObjects, MakerShop.Orders.OrderItem acOrderItem)
        {
            int parentId = acOrderItem.OrderItemId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.OrderItemDigitalGood schemaOrderItemDigitalGood = (Schema.OrderItemDigitalGood)schemaObject;

                if (acOrderItem.DigitalGoods.IndexOf(schemaOrderItemDigitalGood.OrderItemDigitalGoodId) < 0) return null;
                else
                {
                    int index = acOrderItem.DigitalGoods.IndexOf(schemaOrderItemDigitalGood.OrderItemDigitalGoodId);
                    return acOrderItem.DigitalGoods[index];
                }
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.OrderItemDigitalGood acOrderItemDigitalGood = (Orders.OrderItemDigitalGood)acObject;
                Schema.OrderItemDigitalGood schemaOrderItemDigitalGood = (Schema.OrderItemDigitalGood)schemaObject;

                //acOrderItemDigitalGood.StoreId = Token.Instance.StoreId;
                acOrderItemDigitalGood.OrderItemId = parentId;
                if (CanTranslate("DigitalGood", schemaOrderItemDigitalGood.DigitalGoodId)) acOrderItemDigitalGood.DigitalGoodId = Translate("DigitalGood", schemaOrderItemDigitalGood.DigitalGoodId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Schema.OrderItemDigitalGood schemaOrderItemDigitalGood = (Schema.OrderItemDigitalGood)schemaObject;
                Orders.OrderItemDigitalGood acOrderItemDigitalGood = (Orders.OrderItemDigitalGood)acObject;

                ImportDownloads(schemaOrderItemDigitalGood.Downloads, acOrderItemDigitalGood);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.OrderItemDigitalGood), "OrderItemDigitalGood", "OrderItemDigitalGoodId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        private void ImportDownloads(Schema.Download[] arrImportObjects, MakerShop.Orders.OrderItemDigitalGood acOrderItemDigitalGood)
        {
            int parentId = acOrderItemDigitalGood.OrderItemDigitalGoodId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Download schemaDownload = (Schema.Download)schemaObject;

                if (acOrderItemDigitalGood.Downloads.IndexOf(schemaDownload.DownloadId) < 0) return null;
                else
                {
                    int index = acOrderItemDigitalGood.Downloads.IndexOf(schemaDownload.DownloadId);
                    return acOrderItemDigitalGood.Downloads[index];
                }
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                DigitalDelivery.Download acDownload = (DigitalDelivery.Download)acObject;
                Schema.Download schemaDownload = (Schema.Download)schemaObject;

                //acDownload.StoreId = Token.Instance.StoreId;
                acDownload.OrderItemDigitalGoodId = parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Schema.Download schemaDownload = (Schema.Download)schemaObject;
                DigitalDelivery.Download acDownload = (DigitalDelivery.Download)acObject;                
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.DigitalDelivery.Download), "Download", "DownloadId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region ImportSubscriptions

        private void ImportSubscriptions(MakerShop.DataClient.Api.Schema.Subscription[] arrImportObjects, MakerShop.Orders.OrderItem acOrderItem)
        {
            int parentId = acOrderItem.OrderItemId;

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Subscription schemaSubscription = (Schema.Subscription)schemaObject;

                if (acOrderItem.Subscriptions.IndexOf(schemaSubscription.SubscriptionId) < 0) return null;
                else
                {
                    int index = acOrderItem.Subscriptions.IndexOf(schemaSubscription.SubscriptionId);
                    return acOrderItem.Subscriptions[index];
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Orders.Subscription acSubscription = (Orders.Subscription)acObject;
                Schema.Subscription schemaSubscription = (Schema.Subscription)schemaObject;

                //acSubscription.StoreId = Token.Instance.StoreId;
                acSubscription.OrderItemId = parentId;
                if (CanTranslate("Product", schemaSubscription.ProductId)) acSubscription.ProductId= Translate("Product", schemaSubscription.ProductId);
                if (CanTranslate("User", schemaSubscription.UserId)) acSubscription.UserId= Translate("User", schemaSubscription.UserId);
                if (CanTranslate("Transaction", schemaSubscription.TransactionId)) acSubscription.TransactionId = Translate("Transaction", schemaSubscription.TransactionId);                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Schema.Subscription schemaSubscription = (Schema.Subscription)schemaObject;
                Orders.Subscription acSubscription = (Orders.Subscription)acObject;

            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.Subscription), "Subscription", "SubscriptionId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion
               
    }
}