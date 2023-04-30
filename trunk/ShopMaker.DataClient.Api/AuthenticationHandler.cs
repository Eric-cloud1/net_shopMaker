using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Common;
using MakerShop.DataClient.Api.Schema;

//using MakerShop.Security;

namespace MakerShop.DataClient.Api
{
    public class AuthenticationHandler : IHttpHandler
    {
        
        #region IHttpHandler Members

        bool IHttpHandler.IsReusable
        {
            get
            {
                return false;
            }
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse httpResponse = context.Response;
            httpResponse.ContentType = "text/xml";
            AuthenticationError error = AuthenticationError.None;
            String errorMessage = String.Empty;

            byte[] response = null;
            try
            {

                string userName = request.Params["login"];
                string password = request.Params["password"];
                string requestXml = request.Params["xmlData"];


                if (String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(requestXml) && String.IsNullOrEmpty(password))
                {
                    error = AuthenticationError.InvalidRequest;
                }
                else
                {
                    // VALIDATE USER 
                    MakerShop.Users.User user = UserDataSource.LoadForUserName(userName);
                    if (user != null)
                    {
                        if (user.IsSystemAdmin)
                        {
                            {
                                if (!user.IsLockedOut && user.IsApproved)
                                {
                                    UserPasswordCollection passwordCollection = user.Passwords;
                                    if (passwordCollection.Count > 0)
                                    {
                                        MakerShop.Users.UserPassword storedPassword = passwordCollection[0];
                                        if (storedPassword.VerifyPassword(password))
                                        {
                                            // UPDATE THE CURRENT USER ID
                                            Token.Instance.UserId = user.UserId;
                                            response = ACRequestHandler.ProcessRequest(requestXml);
                                        }
                                        else error = AuthenticationError.InvalidUser;
                                    }
                                    else error = AuthenticationError.UnknownError;
                                }
                                else error = AuthenticationError.AccountLocked;
                            }
                        }
                        else error = AuthenticationError.NotSuperUser;
                    }
                    else error = AuthenticationError.InvalidUser;
                }
            }
            catch (Exception ex)
            {
                error = AuthenticationError.UnknownError;
                errorMessage = " Exception Message:" + ex.Message + "\n" + ex.StackTrace;
            }

            // IF NOT AUTHENTICATED
            if (response == null)
            {
                ACAuthenticationResponse authResponse = new ACAuthenticationResponse();
                authResponse.InitVersionInfo();
                authResponse.Log = new Log();
                authResponse.Log.Message = GetMessage(error);
                authResponse.Log.Error = new Error();
                authResponse.Log.Error.Message = GetMessage(error) + errorMessage;
                authResponse.Log.Error.Code = AlwaysConvert.ToInt(String.Format("{0:0#}",(int)error));
                response = EncodeHelper.Serialize(authResponse);
            }

            //write response back 
            httpResponse.BinaryWrite(response);
        }

        #endregion

        private String GetMessage(AuthenticationError error)
        {
            switch (error)
            {
                case AuthenticationError.InvalidRequest:
                    return "Invalid or empty request.";
                case AuthenticationError.InvalidUser:
                    return "Invalid user name or password.";
                case AuthenticationError.UnknownError:
                    return "An unknown error occured while processing request.";
                case AuthenticationError.AccountLocked:
                    return "User account is locked.";
                case AuthenticationError.NotSuperUser:
                    return "User does not have super user privileges.";
                default:
                    return String.Empty;
            }

        }
    }

    /// <summary>
    /// Enum used to indicate authentication errors
    /// </summary>
    public enum AuthenticationError
    {
        None,
        InvalidRequest,
        InvalidUser,
        UnknownError,
        AccountLocked,
        NotSuperUser
    }
}
