using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Orders
{
    //TODO ??? why extend from System.Exception? 
    //Why not CommmerceBuilder.Exceptions.MakerShopException
    /// <summary>
    /// Invalid order exception
    /// </summary>
    public class InvalidOrderException : System.Exception
    {
    }
}
