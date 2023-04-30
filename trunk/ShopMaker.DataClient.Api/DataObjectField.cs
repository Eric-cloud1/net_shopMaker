using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api
{
    public abstract class DataObjectField
    {

        protected string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        protected Type aC6Type;
        protected Type clientType;

        public Type ClientType
        {
            get { return clientType; }
            set { clientType = value; }
        }

        public Type AC6Type
        {
            get { return aC6Type; }
            set { aC6Type = value; }
        }
    }
}
