
namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Defines the different possible input handlings for a pack.
    /// </summary>
    public enum InputHandling
    {
        /// <summary>
        /// The pack input is allowed.
        /// </summary>
        Allowed,

        /// <summary>
        /// The pack input is only allowed when the pack is stored in a fridge.
        /// </summary>
        AllowedForFridge,

        /// <summary>
        /// The pack input is forbidden.
        /// </summary>
        Rejected,

        /// <summary>
        /// The pack input is forbidden because no expiry date has been defined by the storage system.
        /// </summary>
        RejectedNoExpiryDate,

        /// <summary>
        /// The pack input is rejected because no picking indicator has been defined by the storage system.
        /// </summary>
        RejectedNoPickingIndicator,

        /// <summary>
        /// Pack input is rejected because no batch number has been defined by the storage system.
        /// </summary>
        RejectedNoBatchNumber,

        /// <summary>
        /// Pack input is rejected because no stock location has been defined by the storage system.
        /// </summary>
        RejectedNoStockLocation,

        /// <summary>
        /// Pack input is rejected because an invalid stock location has been defined by the storage system.
        /// </summary>
        RejectedInvalidStockLocation
    }
}
