namespace MakerShop.Common
{
    using System;
    using System.Runtime.CompilerServices;
    /// <summary>Represents the method that handles the <see cref="E:MakerShop.IPersistable.Saving"></see> and <see cref="E:MakerShop.IPersistable.Saved"></see> events of a <see cref="T:MakerShop.Common.IPersistable"></see> control.</summary>
    public delegate void PersistableEventHandler(object sender, PersistableEventArgs e);

}
