-- <[ac_StoreSettings]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_StoreSettings] ON;

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 1, 1, 'ProductReviewEnabled', '3' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 2, 1, 'ProductReviewApproval', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 3, 1, 'ProductReviewImageVerification', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 4, 1, 'ProductReviewEmailVerification', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 5, 1, 'PageViewTrackingEnabled', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 6, 1, 'PageViewTrackingSaveArchive', 'False' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 7, 1, 'PageViewTrackingDays', '7' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 8, 1, 'StoreTheme', 'MakerShop' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 9, 1, 'AdminTheme', 'MakerShopAdmin' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 11, 1, 'ProductPurchasingDisabled', 'False' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 12, 1, 'MerchantPasswordMinLength', '8' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 13, 1, 'MerchantPasswordRequireUpper', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 14, 1, 'MerchantPasswordRequireLower', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 15, 1, 'MerchantPasswordRequireNonAlpha', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 16, 1, 'MerchantPasswordMaxAge', '30' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 17, 1, 'MerchantPasswordHistoryCount', '3' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 18, 1, 'MerchantPasswordHistoryDays', '10' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 19, 1, 'MerchantPasswordMaxAttempts', '3' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 20, 1, 'MerchantPasswordLockoutPeriod', '10' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 21, 1, 'MerchantPasswordInactivePeriod', '6' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 22, 1, 'CustomerPasswordMinLength', '6' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 23, 1, 'CustomerPasswordRequireUpper', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 24, 1, 'CustomerPasswordRequireLower', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 25, 1, 'CustomerPasswordRequireNonAlpha', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 26, 1, 'CustomerPasswordMaxAge', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 27, 1, 'CustomerPasswordHistoryCount', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 28, 1, 'CustomerPasswordMaxAttempts', '5' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 29, 1, 'CustomerPasswordLockoutPeriod', '10' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 30, 1, 'BaseCurrencyId', '1' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 31, 1, 'IconImageSize', '50' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 32, 1, 'ThumbnailImageSize', '120' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 33, 1, 'StandardImageSize', '500' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 34, 1, 'ImageSkuLookupEnabled', 'False' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 35, 1, 'OptionThumbnailHeight', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 36, 1, 'OptionThumbnailWidth', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 37, 1, 'OptionThumbnailColumns', '2' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 38, 1, 'ProductReviewEmailVerificationTemplate', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 39, 1, 'ProductReviewTermsAndConditions', '' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 40, 1, 'VolumeDiscountMode', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 41, 1, 'Store_TimeZoneCode', '' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 42, 1, 'Store_TimeZoneOffset', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 43, 1, 'SiteDisclaimerMessage', '' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 44, 1, 'EnableInventory', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 45, 1, 'InventoryDisplayDetails', 'True' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 46, 1, 'InventoryInStockMessage', '{0} units available' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 47, 1, 'InventoryOutOfStockMessage', 'The product is currently out of stock, current quantity is {0}' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 48, 1, 'CheckoutTermsAndConditions', '' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 49, 1, 'OrderMinimumAmount', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 50, 1, 'OrderMaximumAmount', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 51, 1, 'AnonymousUserLifespan', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 52, 1, 'AnonymousAffiliateUserLifespan', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 53, 1, 'GiftCertificateDaysToExpire', '0' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 54, 1, 'PostalCodeCountries', 'US,CA' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 55, 1, 'FileExt_Assets', 'gif, jpg' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 56, 1, 'FileExt_Themes', 'gif, jpg, skin, css, htm, html' );

INSERT INTO [ac_StoreSettings] ([StoreSettingId], [StoreId], [FieldName], [FieldValue]) 
	VALUES ( 57, 1, 'FileExt_DigitalGoods', 'zip' );

SET IDENTITY_INSERT [ac_StoreSettings] OFF;

-- </[ac_StoreSettings]>

GO

-- <[ac_Currencies]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_Currencies] ON;

INSERT INTO [ac_Currencies] ([CurrencyId], [StoreId], [Name], [CurrencySymbol], [DecimalDigits], [DecimalSeparator], [GroupSeparator], [GroupSizes], [NegativePattern], [NegativeSign], [PositivePattern], [ISOCode], [ISOCodePattern], [ExchangeRate], [AutoUpdate], [LastUpdate]) 
	VALUES ( 1, 1, 'US Dollar', '$', 2, '.', ',', '3', 1, '-', 0, 'USD', 0, 1.0000, 0, '2007-09-09 19:21:19.360' );

SET IDENTITY_INSERT [ac_Currencies] OFF;

-- </[ac_Currencies]>

GO

-- <[ac_Provinces]>

SET NOCOUNT ON;
SET DATEFORMAT ymd;

SET IDENTITY_INSERT [ac_Provinces] ON;

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 1, 'CA', 'Alberta', 'alberta', 'AB' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 2, 'CA', 'British Columbia', 'british columbia', 'BC' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 3, 'CA', 'Manitoba', 'manitoba', 'MB' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 4, 'CA', 'New Brunswick', 'new brunswick', 'NB' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 5, 'CA', 'Newfoundland and Labrador', 'newfoundland and labrador', 'NL' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 6, 'CA', 'Northwest Territories', 'northwest territories', 'NT' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 7, 'CA', 'Nova Scotia', 'nova scotia', 'NS' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 8, 'CA', 'Nunavut', 'nunavut', 'NU' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 9, 'CA', 'Ontario', 'ontario', 'ON' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 10, 'CA', 'Prince Edward Island', 'prince edward island', 'PE' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 11, 'CA', 'Québec', 'québec', 'QC' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 12, 'CA', 'Saskatchewan', 'saskatchewan', 'SK' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 13, 'CA', 'Yukon', 'yukon', 'YT' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 14, 'MX', 'Aguascalientes', 'aguascalientes', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 15, 'MX', 'Baja California', 'baja california', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 16, 'MX', 'Baja California Sur', 'baja california sur', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 17, 'MX', 'Campeche', 'campeche', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 18, 'MX', 'Coahuila', 'coahuila', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 19, 'MX', 'Colima', 'colima', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 20, 'MX', 'Chiapas', 'chiapas', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 21, 'MX', 'Chihuahua', 'chihuahua', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 22, 'MX', 'Distrito Federal', 'distrito federal', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 23, 'MX', 'Durango', 'durango', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 24, 'MX', 'Guanajuato', 'guanajuato', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 25, 'MX', 'Guerrero', 'guerrero', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 26, 'MX', 'Hidalgo', 'hidalgo', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 27, 'MX', 'Jalisco', 'jalisco', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 28, 'MX', 'Mexico', 'mexico', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 29, 'MX', 'Michoacan', 'michoacan', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 30, 'MX', 'Morelos', 'morelos', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 31, 'MX', 'Nayarit', 'nayarit', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 32, 'MX', 'Nuevo Leon', 'nuevo leon', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 33, 'MX', 'Oaxaca', 'oaxaca', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 34, 'MX', 'Puebla', 'puebla', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 35, 'MX', 'Queretaro', 'queretaro', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 36, 'MX', 'Quintana Roo', 'quintana roo', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 37, 'MX', 'San Luis Potosi', 'san luis potosi', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 38, 'MX', 'Sinaloa', 'sinaloa', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 39, 'MX', 'Sonora', 'sonora', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 40, 'MX', 'Tabasco', 'tabasco', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 41, 'MX', 'Tamaulipas', 'tamaulipas', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 42, 'MX', 'Tlaxcala', 'tlaxcala', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 43, 'MX', 'Veracruz', 'veracruz', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 44, 'MX', 'Yucatan', 'yucatan', NULL );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 45, 'MX', 'Zacatecas', 'zacatecas', 'ZAC' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 46, 'US', 'Alabama', 'alabama', 'AL' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 47, 'US', 'Alaska', 'alaska', 'AK' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 48, 'US', 'Arizona', 'arizona', 'AZ' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 49, 'US', 'Arkansas', 'arkansas', 'AR' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 50, 'US', 'California', 'california', 'CA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 51, 'US', 'Colorado', 'colorado', 'CO' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 52, 'US', 'Connecticut', 'connecticut', 'CT' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 53, 'US', 'Delaware', 'delaware', 'DE' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 54, 'US', 'District of Columbia', 'district of columbia', 'DC' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 55, 'US', 'Florida', 'florida', 'FL' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 56, 'US', 'Georgia', 'georgia', 'GA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 57, 'US', 'Hawaii', 'hawaii', 'HI' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 58, 'US', 'Idaho', 'idaho', 'ID' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 59, 'US', 'Illinois', 'illinois', 'IL' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 60, 'US', 'Indiana', 'indiana', 'IN' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 61, 'US', 'Iowa', 'iowa', 'IA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 62, 'US', 'Kansas', 'kansas', 'KS' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 63, 'US', 'Kentucky', 'kentucky', 'KY' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 64, 'US', 'Louisiana', 'louisiana', 'LA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 65, 'US', 'Maine', 'maine', 'ME' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 66, 'US', 'Maryland', 'maryland', 'MD' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 67, 'US', 'Massachusetts', 'massachusetts', 'MA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 68, 'US', 'Michigan', 'michigan', 'MI' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 69, 'US', 'Minnesota', 'minnesota', 'MN' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 70, 'US', 'Mississippi', 'mississippi', 'MS' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 71, 'US', 'Missouri', 'missouri', 'MO' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 72, 'US', 'Montana', 'montana', 'MT' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 73, 'US', 'Nebraska', 'nebraska', 'NE' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 74, 'US', 'Nevada', 'nevada', 'NV' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 75, 'US', 'New Hampshire', 'new hampshire', 'NH' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 76, 'US', 'New Jersey', 'new jersey', 'NJ' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 77, 'US', 'New Mexico', 'new mexico', 'NM' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 78, 'US', 'New York', 'new york', 'NY' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 79, 'US', 'North Carolina', 'north carolina', 'NC' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 80, 'US', 'North Dakota', 'north dakota', 'ND' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 81, 'US', 'Ohio', 'ohio', 'OH' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 82, 'US', 'Oklahoma', 'oklahoma', 'OK' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 83, 'US', 'Oregon', 'oregon', 'OR' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 84, 'US', 'Pennsylvania', 'pennsylvania', 'PA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 85, 'US', 'Rhode Island', 'rhode island', 'RI' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 86, 'US', 'South Carolina', 'south carolina', 'SC' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 87, 'US', 'South Dakota', 'south dakota', 'SD' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 88, 'US', 'Tennessee', 'tennessee', 'TN' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 89, 'US', 'Texas', 'texas', 'TX' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 90, 'US', 'Utah', 'utah', 'UT' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 91, 'US', 'Vermont', 'vermont', 'VT' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 92, 'US', 'Virginia', 'virginia', 'VA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 93, 'US', 'Washington', 'washington', 'WA' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 94, 'US', 'West Virginia', 'west virginia', 'WV' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 95, 'US', 'Wisconsin', 'wisconsin', 'WI' );

INSERT INTO [ac_Provinces] ([ProvinceId], [CountryCode], [Name], [LoweredName], [ProvinceCode]) 
	VALUES ( 96, 'US', 'Wyoming', 'wyoming', 'WY' );

SET IDENTITY_INSERT [ac_Provinces] OFF;

-- </[ac_Provinces]>