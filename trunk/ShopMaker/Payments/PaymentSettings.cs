using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Stores;
using System.Web;
using MakerShop.Utility;
using MakerShop.Users;
using System.Text.RegularExpressions;

namespace MakerShop.Payments
{
    public partial class PaymentSettingscollection
    {
        //Hashtable settingMap = new Hashtable();

        //public new void Add(StoreSetting item)
        //{
        //    settingMap[item.FieldName] = item;
        //    base.Add(item);
        //}

        //public override bool Save()
        //{
        //    bool result = base.Save();
        //    Store.ClearCachedSettings();
        //    return result;
        //}

        //public bool Save(int storeId)
        //{
        //    foreach (StoreSetting setting in this)
        //    {
        //        setting.StoreId = storeId;
        //    }
        //    return this.Save();
        //}
        //public string GetValueByKey(string key)
        //{
        //    return GetValueByKey(key, string.Empty);
        //}

        //public string GetValueByKey(string key, string defaultValue)
        //{
        //    if (settingMap.ContainsKey(key)) return ((StoreSetting)settingMap[key]).FieldValue;
        //    return defaultValue;
        //}

        //public void SetValueByKey(string key, string value)
        //{
        //    StoreSetting setting = (StoreSetting)settingMap[key];
        //    if (setting == null) setting = new StoreSetting();
        //    setting.FieldName = key;
        //    setting.FieldValue = value;
        //    this.Add(setting);
        //}


        
    }
}
