namespace MakerShop.Marketing
{
    using MakerShop.Users;

    public partial class EmailListUser
    {
        private User _User;
        public User User
        {
            get
            {
                if (_User == null && !string.IsNullOrEmpty(this.Email))
                {
                    _User = UserDataSource.LoadMostRecentForEmail(this.Email);
                }
                return _User;
            }
        }
    }
}
