using MakerShop.Common;
using MakerShop.Stores;

namespace MakerShop.Web.UI
{
    /* This module is responsible for:
     * - applying dynamic theme to page
     */

    public class MakerShopAdminPage : MakerShopPage
    {
        /// <summary>
        /// Initialize theme.  Admin pages do not support layout theming.
        /// </summary>
        protected override void InitializeTheme()
        {
            //GET DEFAULT STORE THEME
            Store store = Token.Instance.Store;
            if (store != null)
            {
                string theme = store.Settings.AdminTheme;
                if (!string.IsNullOrEmpty(theme) && MakerShop.UI.Styles.Theme.Exists(theme))
                {
                    this.Theme = theme;
                }
            }
        }
    }
}