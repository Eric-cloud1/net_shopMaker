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
        // PRIVATE IMPORT HELPER METHODS FOR USERS IMPORT

        #region USERS IMPORT

        private void ImportUsers(MakerShop.DataClient.Api.Schema.User[] arrImportObjects, string csvFields, string matchFields) // UPDATE
        {
            List<String> numaricFields = MakerShop.DataClient.Api.Schema.User.GetNumaricFields();

            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.User user = (Schema.User)schemaObject;
                if (String.IsNullOrEmpty(matchFields)) return MakerShop.Users.UserDataSource.LoadForUserName(user.UserName);
                else
                {
                    String strCriteria = CalculateCriteria(schemaObject, matchFields, numaricFields);
                    UserCollection users = UserDataSource.LoadForCriteria(strCriteria);
                    if (users.Count == 0) return null;
                    else if (users.Count == 1) return users[0];
                    else return "-1"; // INIDCATE THAT THERE ARE MORE THEN ONE MATCHING OBJECTS, SO WE CAN NOT UPDATE
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Users.User acUser = (Users.User)acObject;
                Schema.User schemaUser = (Schema.User)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acUser.StoreId = Token.Instance.StoreId; //VERIFY

                //AffiliateId
                if (CanTranslate("Affiliate", schemaUser.AffiliateId)) acUser.AffiliateId = Translate("Affiliate", schemaUser.AffiliateId);
                else if (!String.IsNullOrEmpty(schemaUser.Affiliate))
                {
                    int newId = GetAffiliateId(schemaUser.Affiliate);
                    if (newId > 0)
                    {
                        acUser.AffiliateId = newId;
                        AddToTranslationDic("Affiliate" + schemaUser.AffiliateId, newId);
                    }
                }
                
                if (CanTranslate("Affiliate", schemaUser.ReferringAffiliateId)) acUser.ReferringAffiliateId = Translate("Affiliate", schemaUser.ReferringAffiliateId);
                if (CanTranslate("Address", schemaUser.PrimaryAddressId)) acUser.PrimaryAddressId = Translate("Address", schemaUser.PrimaryAddressId);
                if (CanTranslate("Wishlist", schemaUser.PrimaryWishlistId)) acUser.PrimaryWishlistId = Translate("Wishlist", schemaUser.PrimaryWishlistId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Users.User acUser = (Users.User)acObject;
                Schema.User schemaUser = (Schema.User)schemaObject;

                //CHILDS
                Schema.Profile[] tempProfiles = null;
                if (schemaUser.Profile != null)
                {
                    tempProfiles = new Profile[] { schemaUser.Profile };
                    ImportProfiles(tempProfiles, acUser.UserId);
                }

                Schema.ReviewerProfile[] tempReviewerProfiles = null;
                if (schemaUser.ReviewerProfile != null)
                {
                    tempReviewerProfiles = new Schema.ReviewerProfile[] { schemaUser.ReviewerProfile };
                    ImportReviewerProfiles(tempReviewerProfiles, acUser.UserId);
                }                
                
                ImportWishlists(schemaUser.Wishlists, acUser.UserId);
                ImportUserPeronalizations(schemaUser.UserPersonalizations, acUser.UserId);                
                ImportAddresses(schemaUser.Addresses, acUser.UserId);

                // UPDATE USER ADDRESS INFORMATION
                if (!string.IsNullOrEmpty(schemaUser.FirstName))
                    acUser.PrimaryAddress.FirstName = schemaUser.FirstName;
                if (!string.IsNullOrEmpty(schemaUser.LastName))
                    acUser.PrimaryAddress.LastName = schemaUser.LastName;

                if (!string.IsNullOrEmpty(schemaUser.Nickname))
                    acUser.PrimaryAddress.Nickname = schemaUser.Nickname;
                if (!string.IsNullOrEmpty(schemaUser.Company))
                    acUser.PrimaryAddress.Company = schemaUser.Company;
                if (!string.IsNullOrEmpty(schemaUser.Address1))
                    acUser.PrimaryAddress.Address1 = schemaUser.Address1;
                if (!string.IsNullOrEmpty(schemaUser.Address2))
                    acUser.PrimaryAddress.Address2 = schemaUser.Address2;
                if (!string.IsNullOrEmpty(schemaUser.City))
                    acUser.PrimaryAddress.City = schemaUser.City;
                if (!string.IsNullOrEmpty(schemaUser.Province))
                    acUser.PrimaryAddress.Province = schemaUser.Province;
                if (!string.IsNullOrEmpty(schemaUser.PostalCode))
                    acUser.PrimaryAddress.PostalCode = schemaUser.PostalCode;
                if (!string.IsNullOrEmpty(schemaUser.CountryCode))
                    acUser.PrimaryAddress.CountryCode = schemaUser.CountryCode;
                if (!string.IsNullOrEmpty(schemaUser.Phone))
                    acUser.PrimaryAddress.Phone = schemaUser.Phone;
                if (!string.IsNullOrEmpty(schemaUser.Fax))
                    acUser.PrimaryAddress.Fax = schemaUser.Fax;
                
                acUser.PrimaryAddress.Residence = schemaUser.AddressIsResidence;
                
                acUser.PrimaryAddress.Save();


                // IMPORT PASSWORD INFORMATION
                if (!string.IsNullOrEmpty(schemaUser.PasswordPlainText))
                    acUser.SetPassword(schemaUser.PasswordPlainText);
                else if (!string.IsNullOrEmpty(schemaUser.PasswordEncrypted))
                {
                    // UPDATE THE USER PRIMARY PASSWORD
                    MakerShop.Users.UserPassword userPassword = UserPasswordDataSource.Load(acUser.UserId, 1);
                    if (userPassword != null)
                    {
                        if (userPassword.Password != schemaUser.PasswordEncrypted)
                        {
                            string oldUserPassword = userPassword.Password;
                            userPassword.Password = schemaUser.PasswordEncrypted;
                            userPassword.Save();
                            // LOG THE WARNING MESSAGE
                            log("Warning: User '" + acUser.UserName + "' encrypted password has be overwritten from: '" + oldUserPassword + "' to: '" + schemaUser.PasswordEncrypted + "'");
                        }
                    }
                    else
                    {
                        acUser.SetPassword("TempPassword");                        
                        userPassword = UserPasswordDataSource.Load(acUser.UserId, 1);
                        userPassword.Password = schemaUser.PasswordEncrypted;
                        userPassword.Save();
                    }
                }
                  

                ImportUserSettings(schemaUser.Settings, acUser.UserId);
                ImportUserPasswords(schemaUser.Passwords, acUser.UserId);

                // USER GROUPS
                ImportDynamicAssociationObjectData importExtraData = delegate(ref Object acAssociationObject, Object schemaAssociationObject)
                {
                    Users.UserGroup acUserGroup = (Users.UserGroup)acAssociationObject;
                    Schema.UserGroup schemaUserGroup = (Schema.UserGroup)schemaAssociationObject;

                    // UPDATE ORDERBY PROPERTY DATA
                    if (CanTranslate("Subscription", schemaUserGroup.SubscriptionId)) acUserGroup.SubscriptionId = Translate("Subscription", schemaUserGroup.SubscriptionId);
                };

                ImportDynamicAssociations(schemaUser.UserGroups,
                    acUser.UserGroups, acUser.UserId,
                    typeof(MakerShop.Users.UserGroup),
                    "GroupId",
                    "Group",
                    importExtraData); 
            };
            ImportObjectsArray(arrImportObjects, typeof(MakerShop.Users.User), "User", "UserId", "UserName",
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations, csvFields);
        }

        
        #endregion

        #region Import Profile

        private void ImportProfiles(MakerShop.DataClient.Api.Schema.Profile[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Profile profile = (Schema.Profile)schemaObject;
                return MakerShop.Personalization.ProfileDataSource.Load(profile.UserId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Personalization.Profile acProfile = (Personalization.Profile)acObject;
                Schema.Profile schemaProfile = (Schema.Profile)schemaObject;
                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acProfile.UserId = (int)parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Personalization.Profile acProfile = (Personalization.Profile)acObject;
                Schema.Profile schemaProfile = (Schema.Profile)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Personalization.Profile), "Profile", "UserId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import ReviewerProfile

        private void ImportReviewerProfiles(MakerShop.DataClient.Api.Schema.ReviewerProfile[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ReviewerProfile reviewerProfile = (Schema.ReviewerProfile)schemaObject;
                return MakerShop.Personalization.ProfileDataSource.Load(reviewerProfile.ReviewerProfileId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ReviewerProfile acReviewerProfile = (Products.ReviewerProfile)acObject;
                Schema.ReviewerProfile schemaReviewerProfile = (Schema.ReviewerProfile)schemaObject;
                // TRANSLATE ID's AND ASSOCIATE STORE ID
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ReviewerProfile acReviewerProfile = (Products.ReviewerProfile)acObject;
                Schema.ReviewerProfile schemaReviewerProfile = (Schema.ReviewerProfile)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ReviewerProfile), "ReviewerProfile", "ReviewerProfileId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import Wishlists

        private void ImportWishlists(MakerShop.DataClient.Api.Schema.Wishlist[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Wishlist wishlist = (Schema.Wishlist)schemaObject;
                return MakerShop.Users.WishlistDataSource.Load(wishlist.WishlistId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Users.Wishlist acWishlist = (Users.Wishlist)acObject;
                Schema.Wishlist schemaWishlist = (Schema.Wishlist)schemaObject;
                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acWishlist.UserId = (int)parentId; 
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Users.Wishlist acWishlist = (Users.Wishlist)acObject;
                Schema.Wishlist schemaWishlist = (Schema.Wishlist)schemaObject;

                //CHILDS
                ImportWishlistsItems(schemaWishlist.Items, acWishlist.WishlistId);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Users.Wishlist), "Wishlist", "WishlistId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import WishlistItem

        private void ImportWishlistsItems(MakerShop.DataClient.Api.Schema.WishlistItem[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.WishlistItem wishlistItem = (Schema.WishlistItem)schemaObject;
                return MakerShop.Users.WishlistItemDataSource.Load(wishlistItem.WishlistItemId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Users.WishlistItem acWishlistItem = (Users.WishlistItem)acObject;
                Schema.WishlistItem schemaWishlistItem = (Schema.WishlistItem)schemaObject;
                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acWishlistItem.WishlistId = (int)parentId;
                acWishlistItem.ProductId = Translate("Product", schemaWishlistItem.ProductId);                
                                    
                //if (CanTranslate("ProductVariant", schemaKitProduct.ProductVariantId)) acKitProduct.ProductVariantId = Translate("ProductVariant", schemaKitProduct.ProductVariantId);
                //ProductVariant
                if (!String.IsNullOrEmpty(schemaWishlistItem.OptionList))
                {
                    int[] schemaOptionChoiceIds = AlwaysConvert.ToIntArray(schemaWishlistItem.OptionList);
                    string[] acOptionChoiceIds = new string[schemaOptionChoiceIds.Length];
                    for (int i = 0; i < schemaOptionChoiceIds.Length; i++)
                    {
                        acOptionChoiceIds[i] = Translate("OptionChoice", schemaOptionChoiceIds[i]).ToString();
                    }

                    acWishlistItem.OptionList = String.Join(",", acOptionChoiceIds);
                }

                // KitList
                if (!String.IsNullOrEmpty(schemaWishlistItem.KitList))
                {
                    int[] schemaKitProductIds = AlwaysConvert.ToIntArray(schemaWishlistItem.KitList);
                    string[] acKitProductIds = new string[schemaKitProductIds.Length];
                    for (int i = 0; i < schemaKitProductIds.Length; i++)
                    {
                        acKitProductIds[i] = Translate("KitProduct", schemaKitProductIds[i]).ToString();
                    }

                    acWishlistItem.KitList = String.Join(",", acKitProductIds);
                }
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Users.WishlistItem acWishlistItem = (Users.WishlistItem)acObject;
                Schema.WishlistItem schemaWishlistItem = (Schema.WishlistItem)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Users.WishlistItem), "WishlistItem", "WishlistItemId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import UserPeronalizations

        private void ImportUserPeronalizations(MakerShop.DataClient.Api.Schema.UserPersonalization[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.UserPersonalization userPersonalization = (Schema.UserPersonalization)schemaObject;
                return MakerShop.Personalization.UserPersonalizationDataSource.Load(userPersonalization.PersonalizationPathId, userPersonalization.UserId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Personalization.UserPersonalization acUserPersonalization = (Personalization.UserPersonalization)acObject;
                Schema.UserPersonalization schemaUserPersonalization = (Schema.UserPersonalization)schemaObject;
                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acUserPersonalization.UserId = (int)parentId;
            };


            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Personalization.UserPersonalization acUserPersonalization = (Personalization.UserPersonalization)acObject;
                Schema.UserPersonalization schemaUserPersonalization = (Schema.UserPersonalization)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Personalization.UserPersonalization), "UserPersonalization", "UserId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import Addresses

        private void ImportAddresses(MakerShop.DataClient.Api.Schema.Address[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Address address = (Schema.Address)schemaObject;
                return Users.AddressDataSource.Load(address.AddressId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Users.Address acAddress = (Users.Address)acObject;
                Schema.Address schemaAddress = (Schema.Address)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acAddress.UserId = (int)parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Users.Address acAddress = (Users.Address)acObject;
                Schema.Address schemaAddress = (Schema.Address)schemaObject;
            };
  
            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Users.Address), "Address", "AddressId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);

        }

        #endregion

        #region Import UserSetting

        private void ImportUserSettings(MakerShop.DataClient.Api.Schema.UserSetting[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.UserSetting userSetting = (Schema.UserSetting)schemaObject;
                return Users.UserSettingDataSource.Load(userSetting.UserSettingId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Users.UserSetting acUserSetting = (Users.UserSetting)acObject;
                Schema.UserSetting schemaUserSetting = (Schema.UserSetting)schemaObject;
                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acUserSetting.UserId = (int)parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Users.UserSetting acUserSetting = (Users.UserSetting)acObject;
                Schema.UserSetting schemaUserSetting = (Schema.UserSetting)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Users.UserSetting), "UserSetting", "UserSettingId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import UserPasswords

        //private void ImportUserPasswords(MakerShop.DataClient.Api.Schema.UserPassword[] arrImportObjects, int parentId)
        //{
        //    TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
        //    {
        //        Schema.UserPassword userPassword = (Schema.UserPassword)schemaObject;
        //        int userNewId = Translate("User", userPassword.UserId);
        //        return Users.UserPasswordDataSource.Load(userNewId, (byte)userPassword.PasswordNumber);
        //    };

        //    TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
        //    {
        //        Users.UserPassword acUserPassword = (Users.UserPassword)acObject;
        //        Schema.UserPassword schemaUserPassword = (Schema.UserPassword)schemaObject;

        //        // TRANSLATE ID's AND ASSOCIATE STORE ID
        //        acUserPassword.UserId = (int)parentId;
        //    };

        //    ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
        //    {
        //        Users.UserPassword acUserPassword = (Users.UserPassword)acObject;
        //        Schema.UserPassword schemaUserPassword = (Schema.UserPassword)schemaObject;                
        //    };

        //    ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Users.UserPassword), "UserPassword", "PasswordNumber", String.Empty, parentId,
        //        tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);

        //}

        private void ImportUserPasswords(MakerShop.DataClient.Api.Schema.UserPassword[] arrImportObjects, int parentId)
        {
            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                String objName = "UserPassword";   //UPDATE             
                Type acType = typeof(MakerShop.Users.UserPassword); //UPDATE
                Type schemaType = typeof(Schema.UserPassword); //UPDATE
                int imported = 0;
                int updated = 0;

                DataObject objDataObject = new DataObject(acType, schemaType);

                log("Importing " + arrImportObjects.Length + " " + objName + "s");
                foreach (Schema.UserPassword schemaObject in arrImportObjects) //UPDATE
                {
                    MakerShop.Users.UserPassword acObject = null; //UPDATE                    
                    
                    try
                    {

                        if (ImportOption == ImportOptions.Import)
                        {
                            // IF IMPORT ONLY OPTION IS SELECTED
                            acObject = (MakerShop.Users.UserPassword)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE
                            if (acObject == null)
                            {
                                log("Error:" + objName + " with passwordNumber (" + schemaObject.PasswordNumber + ") can not be imported.");
                                continue;
                            }                            

                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            // IF UPDATE ONLY OPTION IS SELECTED
                            acObject = Users.UserPasswordDataSource.Load(parentId, (byte)schemaObject.PasswordNumber);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Users.UserPassword)objDataObject.UpdateToAC6Object(schemaObject, acObject);
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
                            acObject = Users.UserPasswordDataSource.Load(parentId, (byte)schemaObject.PasswordNumber);
                            if (acObject != null)
                            {
                                acObject = (MakerShop.Users.UserPassword)objDataObject.UpdateToAC6Object(schemaObject, acObject); // UPDATE
                                updated++;
                            }
                            else
                            {
                                // CAN NOT BE UPDATED, SO IMPORT IT
                                acObject = (MakerShop.Users.UserPassword)objDataObject.ConvertToAC6Object(schemaObject); // UPDATE                                

                                imported++;
                            }

                        }

                        acObject.UserId = parentId; // VERIFY
                        if (!acObject.Save().Equals(SaveResult.Failed))
                        {
                            
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
                            strLog.AppendLine(objName + " Password Number = " + schemaObject.PasswordNumber);  //UPDATE
                        }
                        strLog.AppendLine("Error message: " + ex.Message);
                        log(strLog.ToString());
                    }
                }
                log(imported + objName + "s imported, " + updated + objName + "s updated.");
                log(objName + "s Import Complete...");
            }
        }

        #endregion

        #region Import Baskets

        private void ImportBaskets(MakerShop.DataClient.Api.Schema.Basket[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Basket basket = (Schema.Basket)schemaObject;
                return MakerShop.Orders.BasketDataSource.Load(basket.BasketId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.Basket acBasket = (MakerShop.Orders.Basket)acObject;
                Schema.Basket schemaBasket = (Schema.Basket)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acBasket.UserId = (int)parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.Basket acBasket = (MakerShop.Orders.Basket)acObject;
                Schema.Basket schemaBasket = (Schema.Basket)schemaObject;
                ImportBasketCoupons(schemaBasket.BasketCoupons, acBasket.BasketId);
                ImportBasketItems(schemaBasket.Items, acBasket.BasketId);
                ImportBasketShipments(schemaBasket.Shipments, acBasket.BasketId);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.Basket), "Basket", "BasketId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import BasketCoupon

        private void ImportBasketCoupons(MakerShop.DataClient.Api.Schema.BasketCoupon[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.BasketCoupon basketCoupon = (Schema.BasketCoupon)schemaObject;
                int basketNewId = Translate("Basket", basketCoupon.BasketId);
                return MakerShop.Orders.BasketCouponDataSource.Load(basketNewId, basketCoupon.CouponId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketCoupon acBasketCoupon = (MakerShop.Orders.BasketCoupon)acObject;
                Schema.BasketCoupon schemaBasketCoupon = (Schema.BasketCoupon)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acBasketCoupon.BasketId = (int)parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketCoupon acBasketCoupon = (MakerShop.Orders.BasketCoupon)acObject;
                Schema.BasketCoupon schemaBasketCoupon = (Schema.BasketCoupon)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.BasketCoupon), "BasketCoupon", "BasketId,CouponId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import BasketItem

        private void ImportBasketItems(MakerShop.DataClient.Api.Schema.BasketItem[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.BasketItem basketItem = (Schema.BasketItem)schemaObject;
                return MakerShop.Orders.BasketItemDataSource.Load(basketItem.BasketItemId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketItem acBasketItem = (MakerShop.Orders.BasketItem)acObject;
                Schema.BasketItem schemaBasketItem = (Schema.BasketItem)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acBasketItem.BasketId = (int)parentId;                
                if (CanTranslate("Basket", schemaBasketItem.BasketId)) acBasketItem.BasketId = Translate("Basket", schemaBasketItem.BasketId);
                if (CanTranslate("BasketShipment", schemaBasketItem.BasketShipmentId)) acBasketItem.BasketShipmentId = Translate("BasketShipment", schemaBasketItem.BasketShipmentId);
                if (CanTranslate("Product", schemaBasketItem.ProductId)) acBasketItem.ProductId = Translate("Product", schemaBasketItem.ProductId);

                //ProductVariant
                if (!String.IsNullOrEmpty(schemaBasketItem.OptionList))
                {
                    int[] schemaOptionChoiceIds = AlwaysConvert.ToIntArray(schemaBasketItem.OptionList);
                    string[] acOptionChoiceIds = new string[schemaOptionChoiceIds.Length];
                    for (int i = 0; i < schemaOptionChoiceIds.Length; i++)
                    {
                        acOptionChoiceIds[i] = Translate("OptionChoice", schemaOptionChoiceIds[i]).ToString();
                    }

                    acBasketItem.OptionList = String.Join(",", acOptionChoiceIds);
                }

                // KitList
                if (!String.IsNullOrEmpty(schemaBasketItem.KitList))
                {
                    int[] schemaKitProductIds = AlwaysConvert.ToIntArray(schemaBasketItem.KitList);
                    string[] acKitProductIds = new string[schemaKitProductIds.Length];
                    for (int i = 0; i < schemaKitProductIds.Length; i++)
                    {
                        acKitProductIds[i] = Translate("KitProduct", schemaKitProductIds[i]).ToString();
                    }

                    acBasketItem.KitList = String.Join(",", acKitProductIds);
                }
                
                if (CanTranslate("TaxCode", schemaBasketItem.TaxCodeId)) acBasketItem.TaxCodeId = Translate("TaxCode", schemaBasketItem.TaxCodeId);
                if (CanTranslate("WrapStyle", schemaBasketItem.WrapStyleId)) acBasketItem.WrapStyleId = Translate("WrapStyle", schemaBasketItem.WrapStyleId);
                if (CanTranslate("WishlistItem", schemaBasketItem.WishlistItemId)) acBasketItem.WishlistItemId = Translate("WishlistItem", schemaBasketItem.WishlistItemId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketItem acBasketItem = (MakerShop.Orders.BasketItem)acObject;
                Schema.BasketItem schemaBasketItem = (Schema.BasketItem)schemaObject;
                ImportBasketItemInputs(schemaBasketItem.Inputs,schemaBasketItem.BasketItemId);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.BasketCoupon), "BasketItem", "BasketItemId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region Import BasketItemInputs

        private void ImportBasketItemInputs(MakerShop.DataClient.Api.Schema.BasketItemInput[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.BasketItemInput basketItemInput = (Schema.BasketItemInput)schemaObject;
                return MakerShop.Orders.BasketItemDataSource.Load(basketItemInput.BasketItemInputId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketItemInput acBasketItemInput = (MakerShop.Orders.BasketItemInput)acObject;
                Schema.BasketItemInput schemaBasketItemInput = (Schema.BasketItemInput)schemaObject;
                acBasketItemInput.BasketItemId = (int)parentId;
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketItemInput acBasketItemInput = (MakerShop.Orders.BasketItemInput)acObject;
                Schema.BasketItemInput schemaBasketItemInput = (Schema.BasketItemInput)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.BasketItemInput), "BasketItemInput", "BasketItemInputId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
      
        }
        #endregion

        #region Import BasketShipment

        private void ImportBasketShipments(MakerShop.DataClient.Api.Schema.BasketShipment[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.BasketShipment basketShipment = (Schema.BasketShipment)schemaObject;
                return MakerShop.Orders.BasketShipmentDataSource.Load(basketShipment.BasketShipmentId);
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketShipment acBasketShipment = (MakerShop.Orders.BasketShipment)acObject;
                Schema.BasketShipment schemaBasketShipment = (Schema.BasketShipment)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acBasketShipment.BasketId = (int)parentId;
                if (CanTranslate("Basket", schemaBasketShipment.BasketId)) acBasketShipment.BasketId = Translate("Basket", schemaBasketShipment.BasketId);
                if (CanTranslate("Warehouse", schemaBasketShipment.WarehouseId)) acBasketShipment.WarehouseId = Translate("Warehouse", schemaBasketShipment.WarehouseId);
                if (CanTranslate("ShipMethod", schemaBasketShipment.ShipMethodId)) acBasketShipment.ShipMethodId = Translate("ShipMethod", schemaBasketShipment.ShipMethodId);
                if (CanTranslate("AddressId", schemaBasketShipment.AddressId)) acBasketShipment.AddressId = Translate("ProductVariant", schemaBasketShipment.AddressId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                MakerShop.Orders.BasketShipment acBasketShipment = (MakerShop.Orders.BasketShipment)acObject;
                Schema.BasketShipment schemaBasketShipment = (Schema.BasketShipment)schemaObject;
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Orders.BasketCoupon), "BasketShipment", "BasketShipmentId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion
    }
}
