
namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Defines the different possible input error types for a pack.
    /// </summary>
    public enum InputErrorType
    {
        /// <summary>
        /// There is no error.
        /// </summary>
        None,

        /// <summary>
        /// The IT system rejected the pack input.
        /// </summary>
        Rejected,

        /// <summary>
        /// The IT system rejected the pack input because the expiry date was missing.
        /// </summary>
        RejectedNoExpiryDate,

        /// <summary>
        /// The machine rejected the pack input because the expiry date of the pack is invalid.
        /// </summary>
        RejectedInvalidExpiryDate,

        /// <summary>
        /// The IT system rejected the pack input because no picking indicator was set.
        /// </summary>
        RejectedNoPickingIndicator,

        /// <summary>
        /// The IT system rejected the pack input because the batch number was missing.
        /// </summary>
        RejectedNoBatchNumber,

        /// <summary>
        /// The IT system rejected the pack input because the stock location was missing.
        /// </summary>
        RejectedNoStockLocation,

        /// <summary>
        /// The IT system rejected the pack input because the specified stock location was invalid.
        /// </summary>
        RejectedInvalidStockLocation,

        /// <summary>
        /// The storage system rejected the input because it is busy.
        /// </summary>
        QueueFull,

        /// <summary>
        /// The storage system rejected the input because it has no fridge for the pack.
        /// </summary>
        FridgeMissing,

        /// <summary>
        /// The storage system rejected the input because the pack dimensions could not be determined.
        /// </summary>
        UnknownPackDimensions,

        /// <summary>
        /// The storage system rejected the input because an error occurred while checking the pack dimensions.
        /// </summary>
        MeasurementError,

        /// <summary>
        /// The storage system rejected the input because a critical error occurred and the pack was acknowledged.
        /// </summary>
        PackAcknowledged,

        /// <summary>
        /// The storage system rejected the input because the input source is broken.
        /// </summary>
        InputBroken,

        /// <summary>
        /// There is not enough space left in the machine to input the pack.
        /// </summary>
        NoSpaceInMachine,

        /// <summary>
        /// No pack has been placed at the medport by the user.
        /// </summary>
        NoPackDetected
    }
}
