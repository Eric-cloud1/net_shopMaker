namespace MakerShop.Utility
{
    using System;
    using MakerShop.Common;

    public partial class LanguageTranslationCollection : PersistentCollection<LanguageTranslation>
    {
        public int IndexOf(string pCulture, string pFieldName)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pCulture == this[i].Culture) && (pFieldName == this[i].FieldName)) return i;
            }
            return -1;
        }
    }
}
