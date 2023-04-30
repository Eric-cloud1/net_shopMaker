
namespace MakerShop.Users
{
    using System;
    using MakerShop.Common;

    	public partial class OptInCollection : PersistentCollection<OptIn>
		{
			public int IndexOf(Int32 pUserId)
			{
				for (int i = 0; i < this.Count; i++)
				{
					if( ( pUserId == this[i].UserId)  ) return i;
				}
				return -1;
			}
		}
	
}
