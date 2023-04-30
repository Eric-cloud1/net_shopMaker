using System;
using System.Data;
using System.Data.Common;
using System.Web;
using MakerShop.Configuration;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Marketing;
using MakerShop.Catalog;

namespace MakerShop.Common
{
    /// <summary>
    /// MakerShop Token object
    /// </summary>
    public sealed class Token : TokenBase
    {
        /// <summary>
        /// Initializes store context using the given Store object
        /// </summary>
        /// <param name="store">The store for initializing the context</param>
        public void InitStoreContext(Store store)
        {
            _Store = store;
            if (store != null) base.StoreId = store.StoreId;
            else base.StoreId = 0;
        }

        /// <summary>
        /// Initializes the user context for this token.
        /// </summary>
        /// <param name="context">The HttpContext to obtain the user data from</param>
        public void InitUserContext(HttpContext context)
        {
            //INITIALIZE THE TOKEN FOR THIS REQUEST
            if (context != null)
            {
                //GET THE USER CONTEXT
                HttpRequest request = context.Request;
                if (request.IsAuthenticated)
                {
                    //FOR MULTISTORE, IF AUTHENTICATED MAKE SURE THE USER IS VALID FOR THE STORE
                    _User = UserDataSource.LoadForUserName(context.User.Identity.Name);
                    if ((_User == null) || (_User.StoreId != this.StoreId))
                    {
                        //store mismatch, expire the forms ticket
                        User.Logout();
                        //redirect to this page to start over
                        context.Response.Redirect(request.RawUrl, true);
                    }
                }
                else
                {
                    _User = UserDataSource.LoadForUserName(request.AnonymousID, true);
                }
                //GET LOCAL TIME
                DateTime localNow = LocaleHelper.LocalNow;
                if (!string.IsNullOrEmpty(context.Request.QueryString["afid"]))
                {
                    int affiliateId = AlwaysConvert.ToInt(context.Request.QueryString["afid"]);
                    if (AffiliateDataSource.Load(affiliateId) != null)
                    {
                        _User.AffiliateId = affiliateId;
                        _User.AffiliateReferralDate = localNow;
                    }
                }
                this.UserId = _User.UserId;
                //UPDATE LAST ACTIVITY DATE, SAVE IF USER IS NOT ANONYMOUS
                _User.LastActivityDate = localNow;
                if (_User.UserId != 0) _User.Save();
            }
        }

        /// <summary>
        /// Initialize the user context for this token using the given user object
        /// </summary>
        /// <param name="user">User object to use for initializing the user context</param>
        public void InitUserContext(User user)
        {
            if (user != null)
            {
                if (user.UserId == 0)
                    user.Save();
                _User = user;
                this.UserId = _User.UserId;
            }
            else this.UserId = 0;
        }

        //CREATE AN INSTANCE OF THE TOKEN FOR THE CURRENT THREAD
        //FOR USE WHEN OUTSIDE OF HTTP CONTEXT
        [ThreadStatic]
        private static Token _ThreadInstance;

        // public property that can only get the single instance of this class.
        /// <summary>
        /// Returns the current instance of the token for the request
        /// </summary>
        /// <remarks>When run within an HttpContext, Token maintains the same instance for each 
        /// HTTP request.  When run outside of HttpContext, Token maintains a single instance 
        /// for each thread.</remarks>
        static public Token Instance
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    Token contextInstance = (Token)context.Items["MakerShopToken"];
                    if (contextInstance == null)
                    {
                        contextInstance = new Token();
                        context.Items["MakerShopToken"] = contextInstance;
                    }
                    return contextInstance;
                }
                if (_ThreadInstance == null) _ThreadInstance = new Token();
                return _ThreadInstance;
            }
        }

        /// <summary>
        /// Used to reset the context/thread instance so it can be reinitialized.
        /// </summary>
        public static void ResetInstance()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                if (context.Items.Contains("MakerShopToken")) context.Items.Remove("MakerShopToken");
            }
            _ThreadInstance = null;
        }

        private Store _Store;
        /// <summary>
        /// Store object associated with this token
        /// </summary>
        public Store Store
        {
            get
            {
                if (_Store == null)
                {
                    _Store = StoreDataSource.Load(base.StoreId);
                }
                return _Store;
            }
            set { _Store = value; }
        }

        private User _User;
        /// <summary>
        /// User object associated with this token
        /// </summary>
        public User User
        {
            get { return _User; }
            set
            {
                _User = value;
                if (value != null)
                    this.UserId = value.UserId;
                else
                    this.UserId = 0;
            }
        }

     

        private bool _EnableRequestCaching = true;
        /// <summary>
        /// Whether to enable request caching?
        /// </summary>
        public bool EnableRequestCaching
        {
            get { return _EnableRequestCaching; }
            set { _EnableRequestCaching = value; }
        }

        private IUrlGenerator _UrlGenerator = null;
        /// <summary>
        /// Instance of active URL generator
        /// </summary>
        public IUrlGenerator UrlGenerator
        {
            get
            {
                if (_UrlGenerator == null)
                {
                    _UrlGenerator = GetUrlGenerator();
                }
                return _UrlGenerator;
            }
        }
        
        private IUrlRewriter _UrlRewriter = null;
        /// <summary>
        /// Instance of active URL rewriter
        /// </summary>
        public IUrlRewriter UrlRewriter
        {
            get
            {
                if (_UrlRewriter == null)
                {
                    _UrlRewriter = GetUrlRewriter();
                }
                return _UrlRewriter;
            }
        }

        private IUrlGenerator GetUrlGenerator()
        {
            IUrlGenerator instance;
            try
            {
                instance = Activator.CreateInstance(Type.GetType(this.UrlGeneratorSettings.Provider)) as IUrlGenerator;
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to create instance for URL generator. " + this.UrlGeneratorSettings.Provider, ex);
                instance = null;
            }
            return instance;
        }

        private IUrlRewriter GetUrlRewriter()
        {
            IUrlRewriter instance;
            try
            {
                instance = Activator.CreateInstance(Type.GetType(this.UrlRewriterSettings.Provider)) as IUrlRewriter;
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to create instance for URL rewriter. " + this.UrlRewriterSettings.Provider, ex);
                instance = null;
            }
            return instance;
        }
    }
}
