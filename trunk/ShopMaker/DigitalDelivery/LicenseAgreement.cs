namespace MakerShop.DigitalDelivery
{
    public partial class LicenseAgreement
    {
        /// <summary>
        /// Deletes a agreement, reassociating any digital goods with the specified agreement.
        /// </summary>
        /// <param name="newAgreementId">The agreement that associated digital goods should be switched to</param>
        /// <returns>True if the agreement is deleted, false otherwise.</returns>
        public virtual bool Delete(int newAgreementId)
        {
            LicenseAgreementDataSource.MoveDigitalGoods(this.LicenseAgreementId, newAgreementId);
            return this.Delete();
        }
    }
}
