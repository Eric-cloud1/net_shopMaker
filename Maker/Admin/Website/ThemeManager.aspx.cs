using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Utility;
using System.Text;

public partial class Admin_Website_ThemeManager : MakerShop.Web.UI.MakerShopAdminPage
{
    private const string ParentFolderText = "<< Parent Folder";
    private const string SafeDirectoryPattern = "[^a-zA-Z0-9\\-_]";

    private string _CurrentPath = string.Empty;
    private string _FullCurrentPath = string.Empty;
    private string _CurrentFileName = string.Empty;
    private string _FullCurrentFileName = string.Empty;
    //SAVES THE LIST OF FILE ITEMS TO PROCESS DELETES
    private List<string> _FileNameList = new List<string>();
    //SAVES THE PATH FROM VIEWSTATE TO PROCESS DELETES
    private string _VSCurrentPath = string.Empty;
    private List<string> _VSFileNameList = new List<string>();

    //STORE IN CUSTOM VIEWSTATE
    protected string CurrentPath
    {
        get { return _CurrentPath; }
        set
        {
            _CurrentPath = value;
            _FullCurrentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "App_Themes\\", value);
        }
    }

    protected string FullCurrentPath
    {
        get
        {
            if (string.IsNullOrEmpty(_FullCurrentPath))
            {
                _FullCurrentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "App_Themes\\", this.CurrentPath);
            }
            return _FullCurrentPath;
        }
    }

    //STORE IN CUSTOM VIEWSTATE
    protected string CurrentFileName
    {
        get { return _CurrentFileName; }
        set
        {
            _CurrentFileName = value;
            if (!string.IsNullOrEmpty(value)) _FullCurrentFileName = Path.Combine(this.FullCurrentPath, value);
            else _FullCurrentFileName = string.Empty;
        }
    }

    protected string FullCurrentFileName
    {
        get
        {
            if (string.IsNullOrEmpty(_FullCurrentFileName) && !string.IsNullOrEmpty(this.CurrentFileName))
                _FullCurrentFileName = Path.Combine(this.FullCurrentPath, this.CurrentFileName);
            return _FullCurrentFileName;
        }
    }

    protected string GetRelativeImagePath()
    {
        if (string.IsNullOrEmpty(CurrentPath))
            return "~/App_Themes/" + CurrentFileName;
        return "~/App_Themes/" + CurrentPath.Replace("\\", "/") + "/" + CurrentFileName;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        // EXTRA MEASURE TO PROTECT AGAINST MISCONFIGURED SECURITY POLICY
        if (!Token.Instance.User.IsAdmin) NavigationHelper.Trigger403(Response, "Admin user rights required.");
        LoadCustomViewState();
        PageHelper.DisableValidationScrolling(this.Page);
        BindFileListRepeater(false);
        BindCurrentFile();
        //DETERMINE WHICH RADIO IS SELECTED
        NewFolderButton.OnClientClick = "fnSetFocus('" + NewFolderName.ClientID + "');";

        // PREVENT FIREFOX DOUBLE POSTING, FIREFOX POSTS TWICE WHEN WE ADD CLIENT SIDE MANUAL POSTING
        // SO, ADD IT ONLY FOR BROWSERS OTHER THEN FIREFOX
        if(Request.Browser.Browser != "Firefox") NewFolderOKButton.OnClientClick = String.Format("if(IsValidFileName('" + NewFolderName.ClientID + "'))__doPostBack('{0}','{1}');", NewFolderOKButton.UniqueID, "");
        else NewFolderOKButton.OnClientClick = "if(!IsValidFileName('" + NewFolderName.ClientID + "'))  return false;";

        RenameButton.OnClientClick = "fnSetFocus('" + RenameName.ClientID + "');";
        RenameOKButton.OnClientClick = String.Format("__doPostBack('{0}','{1}')", RenameOKButton.UniqueID, "");
        CopyButton.OnClientClick = "fnSetFocus('" + CopyName.ClientID + "');";
        CopyOKButton.OnClientClick = String.Format("__doPostBack('{0}','{1}')", CopyOKButton.UniqueID, "");
        ValidFiles.Text = Store.GetCachedSettings().FileExt_Themes;
        if (string.IsNullOrEmpty(ValidFiles.Text)) ValidFiles.Text = "any";
        RenameValidFiles.Text = ValidFiles.Text;
        CopyValidFiles.Text = ValidFiles.Text;

        PageHelper.SetDefaultButton(NewFolderName, NewFolderOKButton);
    }

    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(EncryptionHelper.DecryptAES(Request.Form[VS_CustomState.UniqueID]));
            this.CurrentPath = customViewState.TryGetValue("CurrentPath");
            this.CurrentFileName = customViewState.TryGetValue("CurrentFileName");
            //THIS LIST OF NAMES IS USED TO PROCESS DELETES
            string[] fileNames = customViewState.TryGetValue("FileNameList").Split("|".ToCharArray());
            _VSFileNameList.AddRange(fileNames);
        }
        //THIS IS TO DOUBLE CHECK PATH WAS NOT MODIFIED BEFORE DELETE IS PROCESSED
        _VSCurrentPath = this.CurrentPath;
    }

    protected void BindFileListRepeater(bool updateAjax)
    {
        List<MyFileItem> fileItems = new List<MyFileItem>();
        if (!string.IsNullOrEmpty(this.CurrentPath))
        {
            //ADD IN THE PARENT DIRECTORY
            fileItems.Add(new MyFileItem(ParentFolderText, FileItemType.Directory));
        }
        //GET DIRECTORIES
        string[] directories = System.IO.Directory.GetDirectories(FullCurrentPath);
        foreach (string dir in directories)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.Name != ".svn")
            {
                fileItems.Add(new MyFileItem(dirInfo.Name, FileItemType.Directory));
                _FileNameList.Add(dirInfo.Name);
            }
        }
        //GET FILES
        string[] files = System.IO.Directory.GetFiles(FullCurrentPath);
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(FullCurrentPath, file));
            if (fileInfo.Exists)
            {
                FileItemType thisType = FileHelper.IsImageFile(Path.Combine(FullCurrentPath, file)) ? FileItemType.Image : FileItemType.Other;
                fileItems.Add(new MyFileItem(fileInfo.Name, thisType));
                _FileNameList.Add(fileInfo.Name);
            }
        }
        FileListRepeater.DataSource = fileItems;
        FileListRepeater.DataBind();
        //UPDATE ASSOCIATED PATH CONTROLS
        if (CurrentPath.Length > 0)
        {
            CurrentFolder.Text = "\\App_Themes\\" + CurrentPath.Replace("\\", "<wbr>\\");
            phUpload.Attributes.Clear();
            phUpload.Attributes.Add("style", "display:block");
            phCopyTheme.Visible = false;
        }
        else
        {
            phUpload.Attributes.Clear();
            phUpload.Attributes.Add("style", "display:none");
            phCopyTheme.Visible = true;
            BindThemeList();
            CurrentFolder.Text = "\\App_Themes";
        }
        if (updateAjax) FileListAjax.Update();
    }

    private void BindThemeList()
    {
        ThemeList.Items.Clear();
        ThemeList.Items.Add(new ListItem(string.Empty));
        List<MakerShop.UI.Styles.Theme> themes = MakerShop.UI.Styles.ThemeDataSource.Load();
        foreach (MakerShop.UI.Styles.Theme theme in themes)
        {
            ThemeList.Items.Add(new ListItem(theme.Name));
        }
        string selectedTheme = Request.Form[ThemeList.UniqueID];
        if (!string.IsNullOrEmpty(selectedTheme))
        {
            ListItem selected = ThemeList.Items.FindByValue(selectedTheme);
            if (selected != null) selected.Selected = true;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        RenameName.Text = this.CurrentFileName;
        SaveCustomViewState();
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState["CurrentPath"] = this.CurrentPath;
        customViewState["CurrentFileName"] = this.CurrentFileName;
        //WE WILL PARSE THE FILE NAME LIST WHEN DELETE BUTTON IS CLICKED
        customViewState["FileNameList"] = string.Join("|", _FileNameList.ToArray());
        VS_CustomState.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    public class MyFileItem
    {
        private string _Name;
        private FileItemType _FileItemType;
        public string Name
        {
            get { return _Name; }
        }
        public FileItemType FileItemType
        {
            get { return _FileItemType; }
        }
        public MyFileItem(string name, FileItemType fileItemType)
        {
            _Name = name;
            _FileItemType = fileItemType;
        }
    }

    public enum FileItemType
    {
        Directory, Image, Other
    }

    protected bool ShowFileIcon(object dataItem, FileItemType iconType)
    {
        FileItemType itemType = ((MyFileItem)dataItem).FileItemType;
        return (itemType == iconType);
    }

    protected string IsDeleteDisabled(object container)
    {
        RepeaterItem item = (RepeaterItem)container;
        if ((item.ItemIndex > 0) || (string.IsNullOrEmpty(this.CurrentPath)))
        {
            MyFileItem fileItem = (MyFileItem)item.DataItem;
            if (fileItem.FileItemType == FileItemType.Directory)
            {
                //PREVENT DELETE OF MakerShop THEMES
                if (fileItem.Name == "MakerShop" || fileItem.Name == "MakerShopAdmin") return "disabled";
                else return string.Empty;
            }
            //IT'S A FILE, ALWAYS ALLOW DELETES
            else return string.Empty;
        }
        //this item should not allow delete
        return "disabled";
    }

    protected bool IsBrowseableItem(object dataItem)
    {
        if (this.CurrentPath.Length == 0)
        {
            MyFileItem fileitem = (MyFileItem)dataItem;
            if (fileitem.FileItemType == FileItemType.Directory)
            {
                if (fileitem.Name == "MakerShop" || fileitem.Name == "MakerShopAdmin")
                    return false;
            }
        }
        return true;
    }

    protected void CopyThemeButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            bool success = false;
            string safeFolderName = Regex.Replace(NewThemeName.Text, SafeDirectoryPattern, string.Empty);
            if (!string.IsNullOrEmpty(safeFolderName))
            {
                string newFullCurrentPath = Path.Combine(FullCurrentPath, safeFolderName);
                if (!Directory.Exists(newFullCurrentPath))
                {
                    FileHelper.CopyDirectory(Path.Combine(this.FullCurrentPath, ThemeList.SelectedValue), newFullCurrentPath);
                    success = true;
                }
                else
                {
                    CustomValidator themeExists = new CustomValidator();
                    themeExists.ValidationGroup = "CopyTheme";
                    themeExists.ControlToValidate = "NewThemeName";
                    themeExists.Text = "*";
                    themeExists.ErrorMessage = "This theme already exists.";
                    themeExists.IsValid = false;
                    NewThemeNameValidators.Controls.Add(themeExists);
                }
            }
            BindFileListRepeater(true);
            if (success)
            {
                NewThemeName.Text = string.Empty;
                ThemeList.SelectedIndex = 0;
            }
        }
    }

    protected void NewFolderOKButton_Click(object sender, EventArgs e)
    {
        string safeFolderName = Regex.Replace(NewFolderName.Text, SafeDirectoryPattern, string.Empty);
        if (!string.IsNullOrEmpty(safeFolderName))
        {
            string newFullCurrentPath = Path.Combine(FullCurrentPath, safeFolderName);
            if (!Directory.Exists(newFullCurrentPath))
            {
                Directory.CreateDirectory(newFullCurrentPath);
                if (safeFolderName != NewFolderName.Text)
                {
                    ErrorMessage2.Text = "<br/> Folder created, invalid characters removed from name.";
                    ErrorMessage2.Visible = true;
                }
                else
                {
                    ErrorMessage2.Visible = false;
                }
            }
            else
            {
                ErrorMessage2.Text = "<br/>Folder not created, another folder with same name already exists.";
                ErrorMessage2.Visible = true;
            }
        }
        else
        {
            ErrorMessage2.Visible = true;
        }
        
        NewFolderName.Text = string.Empty;
        BindFileListRepeater(true);
    }

    private string FormatSize(long bytes)
    {
        if (bytes < 1000) return string.Format("{0} bytes", bytes);
        int kb = (int)Math.Round((double)(bytes / 1000), 0);
        if (kb < 1000) return string.Format("{0}kb", kb);
        int mb = (int)Math.Round((double)(kb / 1000), 0);
        return string.Format("{0}mb", mb);
    }

    protected string GetParentPath()
    {
        if (string.IsNullOrEmpty(this.CurrentPath)) return string.Empty;
        //FIND THE LAST INSTANCE OF \
        int lastIndex = CurrentPath.LastIndexOf("\\");
        if (lastIndex < 0) return string.Empty;
        return CurrentPath.Substring(0, lastIndex);
    }

    protected void FileListRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Directory":
                if (e.CommandArgument.Equals(ParentFolderText))
                {
                    CurrentPath = GetParentPath();
                }
                else
                {
                    string safeFolderName = Regex.Replace(e.CommandArgument.ToString(), SafeDirectoryPattern, string.Empty);
                    string newFullCurrentPath = Path.Combine(FullCurrentPath, safeFolderName);
                    if (Directory.Exists(newFullCurrentPath))
                        CurrentPath = Path.Combine(CurrentPath, safeFolderName);
                }
                BindFileListRepeater(true);
                this.CurrentFileName = string.Empty;
                BindCurrentFile();
                break;
            case "Image":
            case "Other":
                this.CurrentFileName = e.CommandArgument.ToString();
                this.BindCurrentFile();
                break;
        }
    }

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        if (UploadedFile.HasFile)
        {
            string safeFileName = FileHelper.GetSafeBaseImageName(UploadedFile.FileName, true);
            if (!string.IsNullOrEmpty(safeFileName) && FileHelper.IsExtensionValid(safeFileName, Store.GetCachedSettings().FileExt_Themes))
            {
                string safeFilePath = Path.Combine(FullCurrentPath, safeFileName);
                try
                {
                    UploadedFile.SaveAs(safeFilePath);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Warn("Exception while saving uploaded file: " + safeFilePath, ex);
                    ErrorMessage.Text = "Unable to upload, access to the path '" + safeFilePath + "' is denied.";
                    ErrorMessage.Visible = true;
                    return;
                }
                BindFileListRepeater(true);
                this.CurrentFileName = safeFileName;
                BindCurrentFile();
            }
            else
            {
                CustomValidator filetype = new CustomValidator();
                filetype.IsValid = false;
                filetype.ControlToValidate = "UploadedFile";
                filetype.ErrorMessage = "'" + safeFileName + "' is not a valid file name.";
                filetype.Text = "*";
                filetype.ValidationGroup = "Upload";
                phValidFiles.Controls.Add(filetype);
            }
        }
    }

    protected void BindCurrentFile()
    {
        //HIDE FILE DETAILS BY DEFAULT
        FileDetails.Visible = false;
        //DETERMINE IF WE HAVE DETAILS TO DISPLAY
        if (!string.IsNullOrEmpty(this.CurrentFileName))
        {
            FileInfo fileInfo = new FileInfo(this.FullCurrentFileName);
            if (fileInfo.Exists)
            {
                //UPDATE FILE VIEWING PANEL
                FileDetails.Visible = true;
                phImagePreview.Visible = false;
                phTextEditor.Visible = false;
                SaveButton.Visible = false;
                FileName.Text = fileInfo.Name;
                FileSize.Text = FormatSize(fileInfo.Length);
                if (FileHelper.IsTextFile(fileInfo.FullName))
                {
                    SaveButton.Visible = true;
                    phTextEditor.Visible = true;
                    TextEditor.Text = File.ReadAllText(fileInfo.FullName);
                }
                else if (FileHelper.IsImageFile(fileInfo.FullName))
                {
                    System.Drawing.Image thisImage = null;
                    try
                    {
                        thisImage = System.Drawing.Image.FromFile(fileInfo.FullName);
                        phImagePreview.Visible = true;
                        ImagePreview.ImageUrl = "~/App_Themes/" + CurrentPath.Replace("\\", "/") + "/" + this.CurrentFileName + "?ts=" + DateTime.Now.ToString("hhmmss");
                        Dimensions.Visible = true;
                        Dimensions.Text = string.Format("({0}w X {1}h)", thisImage.Width, thisImage.Height);
                    }
                    catch
                    {
                        phImagePreview.Visible = false;
                        Dimensions.Visible = false;
                    }
                    finally
                    {
                        if (thisImage != null)
                        {
                            thisImage.Dispose();
                            thisImage = null;
                        }
                    }
                }
            }
        }
        NoFileSelectedPanel.Visible = !FileDetails.Visible;
        FileDetailsAjax.Update();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        File.WriteAllText(this.FullCurrentFileName, TextEditor.Text);
        phSavedMessage.Visible = true;
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }

    protected void RenameOKButton_Click(object sender, EventArgs e)
    {
        string targetFileName = FileHelper.GetSafeBaseImageName(RenameName.Text, true);
        if (!string.IsNullOrEmpty(targetFileName) && FileHelper.IsExtensionValid(targetFileName, Store.GetCachedSettings().FileExt_Themes))
        {
            string fullTargetFileName = Path.Combine(this.FullCurrentPath, targetFileName);
            if (fullTargetFileName != this.FullCurrentFileName)
            {
                if (!File.Exists(fullTargetFileName))
                {
                    File.Move(this.FullCurrentFileName, fullTargetFileName);
                    this.CurrentFileName = targetFileName;
                    BindCurrentFile();
                    BindFileListRepeater(true);
                }
                else
                {
                    //TODO: DISPLAY ERROR TARGET FILE EXISTS
                }
            }
            else
            {
                //TODO: DISPLAY ERROR SAME SOURCE AND TARGET FILE NAME
            }
        }
        else
        {
            CustomValidator filetype = new CustomValidator();
            filetype.IsValid = false;
            filetype.ControlToValidate = "RenameName";
            filetype.ErrorMessage = "'" + targetFileName + "' is not a valid file name.";
            filetype.Text = "*";
            filetype.ValidationGroup = "Rename";
            phRenameValidFiles.Controls.Add(filetype);
            RenamePopup.Show();
        }
        RenameName.Text = string.Empty;
    }

    protected void CopyOKButton_Click(object sender, EventArgs e)
    {
        string targetFileName = FileHelper.GetSafeBaseImageName(CopyName.Text, true);
        if (!string.IsNullOrEmpty(targetFileName) && FileHelper.IsExtensionValid(targetFileName, Store.GetCachedSettings().FileExt_Themes))
        {
            string fullTargetFileName = Path.Combine(this.FullCurrentPath, targetFileName);
            if (fullTargetFileName != this.FullCurrentFileName)
            {
                if (!File.Exists(fullTargetFileName))
                {
                    File.Copy(this.FullCurrentFileName, fullTargetFileName);
                    this.CurrentFileName = targetFileName;
                    BindCurrentFile();
                    BindFileListRepeater(true);
                }
                else
                {
                    //TODO: DISPLAY ERROR TARGET FILE EXISTS
                }
            }
            else
            {
                //TODO: DISPLAY ERROR SAME SOURCE AND TARGET FILE NAME
            }
        }
        else
        {
            CustomValidator filetype = new CustomValidator();
            filetype.IsValid = false;
            filetype.ControlToValidate = "CopyName";
            filetype.ErrorMessage = "'" + targetFileName + "' is not a valid file name.";
            filetype.Text = "*";
            filetype.ValidationGroup = "Copy";
            phCopyValidFiles.Controls.Add(filetype);
            CopyPopup.Show();
        }
        CopyName.Text = string.Empty;
    }

    protected void DeleteSelectedButton_Click(object sender, EventArgs e)
    {
        string selectedItems = Request.Form["selected"];
        if (!string.IsNullOrEmpty(selectedItems))
        {
            //VERIFY THE CURRENT PATH IS THE SAME AS IT WAS AT VIEWSTATE LOAD TIME
            if (_VSCurrentPath == this.CurrentPath)
            {
                //GET THE NAMES OF THE ITEMS TO DELETE
                string[] deleteNames = selectedItems.Split(",".ToCharArray());
                foreach (string deleteName in deleteNames)
                {
                    //MAKE SURE THE DELETE NAME APPEARED IN THE ORIGINAL LIST
                    if (_VSFileNameList.IndexOf(deleteName) > -1)
                    {
                        //OK TO DELETE
                        string fullDeletePath = Path.Combine(this.FullCurrentPath, deleteName);
                        try
                        {
                            if (Directory.Exists(fullDeletePath))
                                //TRY TO DELETE CATEGORY
                                Directory.Delete(fullDeletePath, true);
                            else if (File.Exists(fullDeletePath))
                                //TRY TO DELETE FILE
                                File.Delete(fullDeletePath);
                        }
                        catch { }
                    }
                }
                //REBIND THE PAGE
                BindFileListRepeater(true);
                this.CurrentFileName = string.Empty;
                BindCurrentFile();
            }
        }
    }
}