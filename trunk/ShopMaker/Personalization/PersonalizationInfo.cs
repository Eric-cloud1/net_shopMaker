namespace MakerShop.Personalization
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI;

    /// <summary>
    /// Provides access to elements of a personalization blob
    /// </summary>
    public class PersonalizationInfo
    {
        private string _controlID;
        private Type _controlType;
        private string _controlVPath;
        private bool _isStatic;
        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        private PersonalizationDictionary _customProperties = new PersonalizationDictionary();

        /// <summary>
        /// Gets or sets the ID of the control
        /// </summary>
        public string ControlID
        {
            get { return _controlID; }
            set { _controlID = value; }
        }

        /// <summary>
        /// Gets or sets the type of the control
        /// </summary>
        public Type ControlType
        {
            get { return _controlType; }
            set { _controlType = value; }
        }

        /// <summary>
        /// Gets or sets the virtual path of the control
        /// </summary>
        public string ControlVPath
        {
            get { return _controlVPath; }
            set { _controlVPath = value; }
        }

        /// <summary>
        /// Gets or sets the static indicator
        /// </summary>
        public bool IsStatic
        {
            get { return _isStatic; }
            set { _isStatic = value; }
        }

        /// <summary>
        /// Gets or sets the properties for the control
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        /// <summary>
        /// Gets or sets the custom properties of the control
        /// </summary>
        public PersonalizationDictionary CustomProperties
        {
            get { return _customProperties; }
            set { _customProperties = value; }
        }

        /// <summary>
        /// Initializes this instance from an object queue
        /// </summary>
        /// <param name="q">The object queue</param>
        /// <returns>A personaliation info object initialized from the queue</returns>
        internal static PersonalizationInfo FromObjectQueue(Queue<object> q)
        {
            PersonalizationInfo info = new PersonalizationInfo();
            object firstEntry = q.Dequeue();
            if (firstEntry is string)
            {
                // This is a static web part, it will be present in the page through declaration
                info.ControlID = (string)firstEntry;
                info.IsStatic = true;
            }
            else
            {
                info.ControlType = (Type)firstEntry;
                if (firstEntry == typeof(UserControl))
                {
                    // For usercontrols, the next entry holds the VPath
                    info.ControlVPath = (string)q.Dequeue();
                }
                info.ControlID = (string)q.Dequeue();
                info.IsStatic = false;
            }
            ExtractProperties(q, info);
            ExtractCustomProperties(q, info);
            return info;
        }

        private static void ExtractCustomProperties(Queue<object> q, PersonalizationInfo info)
        {
            info.CustomProperties = new PersonalizationDictionary();
            int PropertyNumber = (int)q.Dequeue();
            for (int PropertyCount = 0; PropertyCount < PropertyNumber; PropertyCount++)
            {
                String Key = ((IndexedString)q.Dequeue()).Value;
                object Value = q.Dequeue();
                PersonalizationScope Scope = ((bool)q.Dequeue()) ?
                    PersonalizationScope.Shared : PersonalizationScope.User;
                bool Sensitive = false;
                Sensitive = (bool)q.Dequeue();

                info.CustomProperties.Add(Key, new PersonalizationEntry(Value, Scope, Sensitive));
            }
        }

        private static void ExtractProperties(Queue<object> q, PersonalizationInfo info)
        {
            info.Properties = new Dictionary<string, object>();
            int PropertyNumber = (int)q.Dequeue();
            for (int PropertyCount = 0; PropertyCount < PropertyNumber; PropertyCount++)
            {
                String Key = ((IndexedString)q.Dequeue()).Value;
                object Value = q.Dequeue();
                info.Properties.Add(Key, Value);
            }
        }

        /// <summary>
        /// Gets a string representation of the PersonalizationInfo object
        /// </summary>
        /// <returns>A string representation of the PersonalizationInfo object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Control: {0}\n", this.ControlID);
            sb.AppendFormat("Properties: {0}, Custom properties: {1}", Properties.Count, CustomProperties.Count);
            return sb.ToString();
        }
    }
}