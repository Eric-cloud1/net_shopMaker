<%@ WebHandler Language="C#" Class="Download" %>

using System;
using System.Web;
using MakerShop.Exceptions;
using MakerShop.Common;
using MakerShop.Stores;
using MakerShop.Utility;

public class Download : IHttpHandler {
    
    public void ProcessRequest (HttpContext context)
    {
        //WHAT PART NUMBER TO GET?
        int partNumber = AlwaysConvert.ToInt(context.Request.QueryString["part"]);
        //TODO: REMOVE THE ABILITY TO PASS 0 TO GET FULL KEY!!!
        if (partNumber != 0)
        {
            //GET THE BACKUP DATA
            byte[] backupKeyData = EncryptionHelper.GetBackupKey(partNumber);
            //SEND THE BACKUP PART TO BROWSER
            SendKeyDataToClient("keypart" + partNumber.ToString() + ".bin", backupKeyData);
        }
        else SendKeyDataToClient("key.bin", EncryptionHelper.GetEncryptionKey());
    }
    
    private void SendKeyDataToClient(string fileName, byte[] keyData)
    {
        HttpResponse Response = HttpContext.Current.Response;
        Response.Clear();
        Response.ContentType = "application/octet-stream";
        Response.AddHeader("Content-Length", keyData.Length.ToString());
        Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
        Response.OutputStream.Write(keyData, 0, keyData.Length);
        Response.Flush();
        Response.Close();
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }

}