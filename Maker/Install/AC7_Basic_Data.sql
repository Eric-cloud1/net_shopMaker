-- <[ac_OrderStatuses]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_OrderStatuses] ON;

INSERT INTO [ac_OrderStatuses] ([OrderStatusId], [StoreId], [Name], [DisplayName], [InventoryActionId], [IsActive], [IsValid], [OrderBy]) 
	VALUES ( 1, 1, 'Payment Pending', 'Payment Pending', 0, 0, 1, 1 );

INSERT INTO [ac_OrderStatuses] ([OrderStatusId], [StoreId], [Name], [DisplayName], [InventoryActionId], [IsActive], [IsValid], [OrderBy]) 
	VALUES ( 2, 1, 'Shipment Pending', 'Shipment Pending', 1, 1, 1, 2 );

INSERT INTO [ac_OrderStatuses] ([OrderStatusId], [StoreId], [Name], [DisplayName], [InventoryActionId], [IsActive], [IsValid], [OrderBy]) 
	VALUES ( 3, 1, 'Completed', 'Completed', 1, 1, 1, 3 );

INSERT INTO [ac_OrderStatuses] ([OrderStatusId], [StoreId], [Name], [DisplayName], [InventoryActionId], [IsActive], [IsValid], [OrderBy]) 
	VALUES ( 4, 1, 'Problem', 'Payment Pending', 2, 0, 1, 4 );

INSERT INTO [ac_OrderStatuses] ([OrderStatusId], [StoreId], [Name], [DisplayName], [InventoryActionId], [IsActive], [IsValid], [OrderBy]) 
	VALUES ( 5, 1, 'Cancelled', 'Cancelled', 2, 0, 0, 5 );

INSERT INTO [ac_OrderStatuses] ([OrderStatusId], [StoreId], [Name], [DisplayName], [InventoryActionId], [IsActive], [IsValid], [OrderBy]) 
	VALUES ( 6, 1, 'Fraud', 'Cancelled', 0, 0, 0, 6 );

SET IDENTITY_INSERT [ac_OrderStatuses] OFF;

-- </[ac_OrderStatuses]>

GO

-- <[ac_OrderStatusTriggers]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

INSERT INTO [ac_OrderStatusTriggers] ([StoreEventId], [OrderStatusId]) 
	VALUES ( 100, 1 );

INSERT INTO [ac_OrderStatusTriggers] ([StoreEventId], [OrderStatusId]) 
	VALUES ( 102, 4 );

INSERT INTO [ac_OrderStatusTriggers] ([StoreEventId], [OrderStatusId]) 
	VALUES ( 105, 4 );

INSERT INTO [ac_OrderStatusTriggers] ([StoreEventId], [OrderStatusId]) 
	VALUES ( 106, 2 );

INSERT INTO [ac_OrderStatusTriggers] ([StoreEventId], [OrderStatusId]) 
	VALUES ( 109, 3 );

INSERT INTO [ac_OrderStatusTriggers] ([StoreEventId], [OrderStatusId]) 
	VALUES ( 117, 3 );

INSERT INTO [ac_OrderStatusTriggers] ([StoreEventId], [OrderStatusId]) 
	VALUES ( 115, 5 );


-- </[ac_OrderStatusTriggers]>

GO

-- <[ac_PaymentMethods]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_PaymentMethods] ON;

INSERT INTO [ac_PaymentMethods] ([PaymentMethodId], [StoreId], [Name], [PaymentInstrumentId], [PaymentGatewayId], [OrderBy]) 
	VALUES ( 1, 1, 'Visa', 1, NULL, 1 );

INSERT INTO [ac_PaymentMethods] ([PaymentMethodId], [StoreId], [Name], [PaymentInstrumentId], [PaymentGatewayId], [OrderBy]) 
	VALUES ( 2, 1, 'MasterCard', 2, NULL, 2 );

INSERT INTO [ac_PaymentMethods] ([PaymentMethodId], [StoreId], [Name], [PaymentInstrumentId], [PaymentGatewayId], [OrderBy]) 
	VALUES ( 3, 1, 'American Express', 4, NULL, 3 );

INSERT INTO [ac_PaymentMethods] ([PaymentMethodId], [StoreId], [Name], [PaymentInstrumentId], [PaymentGatewayId], [OrderBy]) 
	VALUES ( 4, 1, 'Discover', 3, NULL, 4 );

INSERT INTO [ac_PaymentMethods] ([PaymentMethodId], [StoreId], [Name], [PaymentInstrumentId], [PaymentGatewayId], [OrderBy]) 
	VALUES ( 5, 1, 'PayPal', 7, NULL, 5 );

SET IDENTITY_INSERT [ac_PaymentMethods] OFF;

-- </[ac_PaymentMethods]>

GO

-- <[ac_TaxCodes]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_TaxCodes] ON;

INSERT INTO [ac_TaxCodes] ([TaxCodeId], [StoreId], [Name]) 
	VALUES ( 1, 1, 'Taxable' );

SET IDENTITY_INSERT [ac_TaxCodes] OFF;

-- </[ac_TaxCodes]>

GO

-- <[ac_ShipZones]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_ShipZones] ON;

INSERT INTO [ac_ShipZones] ([ShipZoneId], [StoreId], [Name], [CountryRuleId], [ProvinceRuleId], [PostalCodeFilter]) 
	VALUES ( 1, 1, 'Contiguous US', 1, 1, NULL );

INSERT INTO [ac_ShipZones] ([ShipZoneId], [StoreId], [Name], [CountryRuleId], [ProvinceRuleId], [PostalCodeFilter]) 
	VALUES ( 2, 1, 'Alaska and Hawaii', 1, 1, NULL );

SET IDENTITY_INSERT [ac_ShipZones] OFF;

-- </[ac_ShipZones]>

GO

-- <[ac_ShipZoneCountries]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

INSERT INTO [ac_ShipZoneCountries] ([ShipZoneId], [CountryCode]) 
	VALUES ( 1, 'US' );

INSERT INTO [ac_ShipZoneCountries] ([ShipZoneId], [CountryCode]) 
	VALUES ( 2, 'US' );


-- </[ac_ShipZoneCountries]>

GO

-- <[ac_ShipZoneProvinces]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 46, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 47, 2 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 48, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 49, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 50, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 51, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 52, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 53, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 54, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 55, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 56, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 57, 2 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 58, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 59, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 60, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 61, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 62, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 63, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 64, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 65, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 66, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 67, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 68, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 69, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 70, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 71, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 72, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 73, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 74, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 75, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 76, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 77, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 78, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 79, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 80, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 81, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 82, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 83, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 84, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 85, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 86, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 87, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 88, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 89, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 90, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 91, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 92, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 93, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 94, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 95, 1 );

INSERT INTO [ac_ShipZoneProvinces] ([ProvinceId], [ShipZoneId]) 
	VALUES ( 96, 1 );


-- </[ac_ShipZoneProvinces]>

GO

-- <[ac_EmailTemplates]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_EmailTemplates] ON;

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 1, 1, 'Customer Order Notification', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Confirmation - Order Number $order.OrderNumber', 'Customer Order Notification.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 3, 1, 'Lost Password', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Password Reset Request at $store.Name', 'Lost Password.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 4, 1, 'Low Inventory', 'info@yourdomain.xyz', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Low Inventory Warning for $store.Name', 'Low Inventory.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 5, 1, 'Vendor Notification', 'vendor', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Packing Slip - Shipment Notification for $store.Name', 'Vendor Notification.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 6, 1, 'Gift Certificate Validated', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Gift Certificate Validated for Order Number $order.OrderNumber', 'Gift Certificate Validated.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 7, 1, 'Order Shipped Partial', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Part of your order has shipped from $store.Name', 'Order Shipped Partial.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 8, 1, 'Order Shipped', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Your order has shipped from $store.Name', 'Order Shipped.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 9, 1, 'Product Review Verification', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Product Review Verfication', 'Product Review Verification.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 10, 1, 'Email List Signup With Verification', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Activation required to join $list.Name', 'Email List Signup With Verification', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 11, 1, 'Email List Signup Notification Only', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Thank you for joining $list.Name', 'Email List Signup Notification Only.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 12, 1, 'ESD File Activated', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Your digital good is available for download', 'ESD File Activated.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 13, 1, 'License Key Fulfilled', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Your License key for $orderItemDigitalGood.DigitalGood.Name is fulfilled', 'License Key Fulfilled.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 14, 1, 'Send Product To Friend', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, '${fromName} wants you to see this item at $store.Name', 'Send Product To Friend.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 15, 1, 'Note Added By Customer', 'merchant', 'info@yourdomain.xyz', NULL, NULL, NULL, 'Customer added a note to Order Number $order.OrderNumber', 'Note Added By Customer.html', 1 );

INSERT INTO [ac_EmailTemplates] ([EmailTemplateId], [StoreId], [Name], [ToAddress], [FromAddress], [ReplyToAddress], [CCList], [BCCList], [Subject], [ContentFileName], [IsHTML]) 
	VALUES ( 16, 1, 'Note Added By Merchant', 'customer', 'info@yourdomain.xyz', NULL, NULL, NULL, '$store.Name has added a message to your Order Number $order.OrderNumber', 'Note Added By Merchant.html', 1 );

SET IDENTITY_INSERT [ac_EmailTemplates] OFF;

-- </[ac_EmailTemplates]>

GO

-- <[ac_StoreSettings]>

INSERT INTO [ac_StoreSettings] ([FieldName], [FieldValue], [StoreId])
	VALUES ('Email_DefaultAddress', 'info@yourdomain.xyz', 1);

INSERT INTO [ac_StoreSettings] ([FieldName], [FieldValue], [StoreId])
	VALUES ('Email_SubscriptionAddress', 'noreply@yourdomain.xyz', 1);

INSERT INTO [ac_StoreSettings] ([FieldName], [FieldValue], [StoreId])
	VALUES ('ProductTellAFriendEmailTemplateId', 14, 1);

-- </[ac_StoreSettings]>

GO

-- <[ac_EmailTemplateTriggers]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 1, 100 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 3, 200 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 4, 300 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 5, 106 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 5, 117 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 6, 116 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 7, 110 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 8, 109 );
	
INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 15, 113 );

INSERT INTO [ac_EmailTemplateTriggers] ([EmailTemplateId], [StoreEventId]) 
	VALUES ( 16, 112 );

-- </[ac_EmailTemplateTriggers]>

GO


-- <[ac_ProductTemplates]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_ProductTemplates] ON;

INSERT INTO [ac_ProductTemplates] ([ProductTemplateId], [StoreId], [Name]) 
	VALUES ( 1, 1, 'Book' );

INSERT INTO [ac_ProductTemplates] ([ProductTemplateId], [StoreId], [Name]) 
	VALUES ( 2, 1, 'Movie' );

INSERT INTO [ac_ProductTemplates] ([ProductTemplateId], [StoreId], [Name]) 
	VALUES ( 3, 1, 'TV' );

INSERT INTO [ac_ProductTemplates] ([ProductTemplateId], [StoreId], [Name]) 
	VALUES ( 4, 1, 'Personalized' );

SET IDENTITY_INSERT [ac_ProductTemplates] OFF;

-- </[ac_ProductTemplates]>

GO

-- <[ac_InputFields]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_InputFields] ON;

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 1, 1, 'Author', 'Author:', 2, 0, 0, 0, 0, NULL, 1, 1, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 2, 1, 'ISBN', 'ISBN:', 2, 0, 0, 0, 0, NULL, 1, 2, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 3, 2, 'Director', 'Director:', 2, 0, 0, 0, 0, NULL, 1, 1, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 4, 2, 'Actors', 'Actors:', 2, 0, 0, 0, 0, NULL, 1, 2, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 5, 2, 'Rating', 'Rating:', 4, 0, 0, 0, 0, NULL, 1, 3, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 13, 3, 'Model', 'Model', 2, 0, 0, 0, 0, NULL, 1, 1, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 14, 3, 'Description', 'Description', 2, 0, 0, 0, 0, NULL, 1, 2, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 15, 3, 'Resolution', 'Resolution', 2, 0, 0, 0, 0, NULL, 1, 3, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 16, 3, 'Includes', 'Includes', 8, 0, 0, 0, 0, NULL, 1, 4, 0 );

INSERT INTO [ac_InputFields] ([InputFieldId], [ProductTemplateId], [Name], [UserPrompt], [InputTypeId], [Rows], [Columns], [MaxLength], [IsRequired], [RequiredMessage], [IsMerchantField], [OrderBy], [PersistWithOrder]) 
	VALUES ( 17, 4, 'Personalization', 'Enter the recipients name below: (30 char max)', 2, 0, 30, 30, 0, NULL, 0, 1, 0 );

SET IDENTITY_INSERT [ac_InputFields] OFF;

-- </[ac_InputFields]>

GO

-- <[ac_InputChoices]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_InputChoices] ON;

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 1, 5, 'G', NULL, 0, 1 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 2, 5, 'PG', NULL, 0, 2 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 3, 5, 'PG-13', NULL, 0, 3 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 4, 5, 'R', NULL, 0, 4 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 16, 16, 'Integrated pedestal stand and 20W integrated speaker system', NULL, 0, 1 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 17, 16, 'NTSC and ATSC (HDTV) tuners', NULL, 0, 2 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 18, 16, '2 HDMI inputs', NULL, 0, 3 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 19, 16, '2 component video', NULL, 0, 4 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 20, 16, '3 composite video', NULL, 0, 5 );

INSERT INTO [ac_InputChoices] ([InputChoiceId], [InputFieldId], [ChoiceText], [ChoiceValue], [IsSelected], [OrderBy]) 
	VALUES ( 21, 16, '3 S-Video connection', NULL, 0, 6 );

SET IDENTITY_INSERT [ac_InputChoices] OFF;

-- </[ac_InputChoices]>