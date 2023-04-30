using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MakerShop.Utility;

namespace MakerShop.Products
{
    /// <summary>
    /// This class represents a InputField object in the database.
    /// </summary>
    public partial class InputField
    {
        /// <summary>
        /// Input type
        /// </summary>
        public InputType InputType
        {
            get { return (InputType)this.InputTypeId; }
            set { this.InputTypeId = (byte)value; }
        }

        /// <summary>
        /// Gets the web control for this input field
        /// </summary>
        /// <returns>Web control for this input field</returns>
        public System.Web.UI.WebControls.WebControl GetControl()
        {
            return GetControl(string.Empty);
        }

        /// <summary>
        /// Gets the web control for this input field
        /// </summary>
        /// <param name="selectedValue">The selected value in the control</param>
        /// <returns>Web control for this input field</returns>
        public System.Web.UI.WebControls.WebControl GetControl(string selectedValue)
        {
            bool setSelected = false;
            string[] selectedValues = null;
            switch (this.InputType)
            {
                case InputType.CheckBoxList:
                    CheckBoxList checkBoxList = new CheckBoxList();
                    checkBoxList.ID = this.UniqueId;
                    if (this.Columns > 1) checkBoxList.RepeatColumns = this.Columns;
                    if (!string.IsNullOrEmpty(selectedValue))
                    {
                        selectedValues = selectedValue.Replace(", ", ",").Split(",".ToCharArray());
                    }
                    foreach (InputChoice choice in this.InputChoices)
                    {
                        string choiceValue = choice.ChoiceValue;
                        if (string.IsNullOrEmpty(choiceValue)) choiceValue = choice.ChoiceText;
                        ListItem item = new ListItem(choice.ChoiceText, choiceValue);
                        if (string.IsNullOrEmpty(selectedValue))
                        {
                            if (choice.IsSelected) item.Selected = true;
                        }
                        else
                        {
                            if (System.Array.IndexOf(selectedValues, choiceValue) > -1) item.Selected = true;
                        }
                        checkBoxList.Items.Add(item);
                    }
                    return checkBoxList;
                case InputType.DropDownListBox:
                    DropDownList dropDown = new DropDownList();
                    dropDown.ID = this.UniqueId;
                    foreach (InputChoice choice in this.InputChoices)
                    {
                        string choiceValue = choice.ChoiceValue;
                        if (string.IsNullOrEmpty(choiceValue)) choiceValue = choice.ChoiceText;
                        ListItem item = new ListItem(choice.ChoiceText, choiceValue);
                        if (!setSelected)
                        {
                            if (string.IsNullOrEmpty(selectedValue))
                            {
                                if (choice.IsSelected)
                                {
                                    item.Selected = (choice.IsSelected);
                                    setSelected = true;
                                }
                            }
                            else
                            {
                                if (choiceValue == selectedValue)
                                {
                                    item.Selected = true;
                                    setSelected = true;
                                }
                            }
                        }
                        dropDown.Items.Add(item);
                    }
                    return dropDown;
                case InputType.ListBox:
                case InputType.MultipleListBox:
                    ListBox listBox = new ListBox();
                    listBox.ID = this.UniqueId;
                    listBox.SelectionMode = (this.InputType == InputType.MultipleListBox) ? ListSelectionMode.Multiple : ListSelectionMode.Single;
                    if (listBox.SelectionMode == ListSelectionMode.Multiple) listBox.SelectedIndex = -1;
                    if (this.Rows > 0) listBox.Rows = this.Rows;
                    if (!string.IsNullOrEmpty(selectedValue))
                    {
                        selectedValues = selectedValue.Replace(", ", ",").Split(",".ToCharArray());
                    }
                    foreach (InputChoice choice in this.InputChoices)
                    {
                        string choiceValue = choice.ChoiceValue;
                        if (string.IsNullOrEmpty(choiceValue)) choiceValue = choice.ChoiceText;
                        ListItem item = new ListItem(choice.ChoiceText, choiceValue);
                        if (!setSelected || (listBox.SelectionMode == ListSelectionMode.Multiple))
                        {
                            if (string.IsNullOrEmpty(selectedValue))
                            {
                                if (choice.IsSelected)
                                {
                                    item.Selected = (choice.IsSelected);
                                    setSelected = true;
                                }
                            }
                            else
                            {
                                if (System.Array.IndexOf(selectedValues, choiceValue) > -1)
                                {
                                    item.Selected = true;
                                    setSelected = true;
                                }
                            }
                        }
                        listBox.Items.Add(item);
                    }
                    if (!setSelected && (listBox.SelectionMode == ListSelectionMode.Single) && (this.InputChoices.Count > 0))
                    {
                        listBox.SelectedIndex = 0;
                    }
                    return listBox;
                case InputType.RadioButtonList:
                    RadioButtonList radioButtonList = new RadioButtonList();
                    radioButtonList.ID = this.UniqueId;
                    if (this.Columns > 1) radioButtonList.RepeatColumns = this.Columns;
                    foreach (InputChoice choice in this.InputChoices)
                    {
                        string choiceValue = choice.ChoiceValue;
                        if (string.IsNullOrEmpty(choiceValue)) choiceValue = choice.ChoiceText;
                        ListItem item = new ListItem(choice.ChoiceText, choiceValue);
                        if (string.IsNullOrEmpty(selectedValue))
                        {
                            if (choice.IsSelected)
                            {
                                item.Selected = true;
                                setSelected = true;
                            }
                        }
                        else
                        {
                            if (choiceValue == selectedValue)
                            {
                                item.Selected = true;
                                setSelected = true;
                            }
                        }
                        radioButtonList.Items.Add(item);
                    }
                    if (!setSelected && this.InputChoices.Count > 0) radioButtonList.SelectedIndex = 0;
                    return radioButtonList;
                case InputType.TextBox:
                case InputType.TextArea:
                    TextBox textBox = new TextBox();
                    textBox.ID = this.UniqueId;
                    textBox.TextMode = (this.InputType == InputType.TextArea) ? TextBoxMode.MultiLine : TextBoxMode.SingleLine;
                    if (this.Columns > 1) textBox.Columns = this.Columns;
                    if (this.MaxLength > 0) textBox.MaxLength = this.MaxLength;
                    textBox.Text = selectedValue;
                    return textBox;

            }
            return null;
        }

        /// <summary>
        /// Gets the value of the control for this input field
        /// </summary>
        /// <param name="control">The control to get value from</param>
        /// <returns>The value of the control</returns>
        public string GetControlValue(WebControl control)
        {
            switch (this.InputType)
            {
                case InputType.CheckBoxList:
                    CheckBoxList checkBoxList = control as CheckBoxList;
                    if (checkBoxList != null)
                    {
                        List<string> values = new List<string>();
                        foreach (ListItem item in checkBoxList.Items)
                        {
                            if (item.Selected)
                            {
                                if (string.IsNullOrEmpty(item.Value))
                                {
                                    values.Add(item.Text);
                                }
                                else
                                {
                                    values.Add(item.Value);
                                }
                            }
                        }
                        return string.Join(",", values.ToArray());
                    }
                    break;
                case InputType.DropDownListBox:
                    DropDownList dropDownList = control as DropDownList;
                    if (dropDownList != null)
                    {
                        ListItem selectedItem = dropDownList.Items[dropDownList.SelectedIndex];
                        if (selectedItem != null)
                        {
                            if (!string.IsNullOrEmpty(selectedItem.Value)) return selectedItem.Value;
                            return selectedItem.Text;
                        }
                    }
                    break;
                case InputType.ListBox:
                case InputType.MultipleListBox:
                    ListBox listBox = control as ListBox;
                    if (listBox != null)
                    {
                        List<string> values = new List<string>();
                        foreach (ListItem item in listBox.Items)
                        {
                            if (item.Selected)
                            {
                                if (string.IsNullOrEmpty(item.Value))
                                {
                                    values.Add(item.Text);
                                }
                                else
                                {
                                    values.Add(item.Value);
                                }
                            }
                        }
                        return string.Join(",", values.ToArray());
                    }
                    break;
                case InputType.RadioButtonList:
                    RadioButtonList radioButtonList = control as RadioButtonList;
                    if (radioButtonList != null)
                    {
                        ListItem selectedItem = radioButtonList.Items[radioButtonList.SelectedIndex];
                        if (selectedItem != null)
                        {
                            if (!string.IsNullOrEmpty(selectedItem.Value)) return selectedItem.Value;
                            return selectedItem.Text;
                        }
                    }
                    break;
                case InputType.TextBox:
                case InputType.TextArea:
                    TextBox textBox = control as TextBox;
                    if (textBox != null)
                    {
                        if (!this.IsMerchantField) return StringHelper.StripHtml(textBox.Text);
                        else return textBox.Text;
                    }
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates a copy of the specified input field
        /// </summary>
        /// <param name="inputFieldId">Id of the input field to create copy of</param>
        /// <param name="deepCopy">If <b>true</b>, child objects are also copied</param>
        /// <returns>A copy of the specified input field</returns>
        public static InputField Copy(int inputFieldId, bool deepCopy)
        {
            InputField copy = InputFieldDataSource.Load(inputFieldId);
            if (copy != null)
            {
                if (deepCopy)
                {
                    //LOAD THE CHILD COLLECTIONS AND RESET
                    foreach (InputChoice choice in copy.InputChoices)
                    {
                        choice.InputChoiceId = 0;
                    }
                }
                copy.InputFieldId = 0;
                return copy;
            }
            return null;
        }

        /// <summary>
        /// Gets a unique Id for this Input field
        /// </summary>
        public string UniqueId
        {
            get { return "InputField_" + this.InputFieldId.ToString(); }
        }
    }
}
