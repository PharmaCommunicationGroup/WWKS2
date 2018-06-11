using System;

namespace CareFusion.Lib.StorageSystem.Stock
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system pack.
    /// </summary>
    public interface IPack
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier of the pack.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// Gets the delivery number that was used during the input of the pack.
        /// </summary>
        string DeliveryNumber { get; }

        /// <summary>
        /// Gets the batchnumber of the pack.
        /// </summary>
        string BatchNumber { get; }

        /// <summary>
        /// Gets the external identifier of the pack.
        /// </summary>
        string ExternalId { get; }

        /// <summary>
        /// Gets the expiry date of the pack.
        /// </summary>
        DateTime ExpiryDate { get; }

        /// <summary>
        /// Gets the stock in date of the pack.
        /// </summary>
        DateTime StockInDate { get; }

        /// <summary>
        /// Gets the full scan code of the pack.
        /// </summary>
        string ScanCode { get; }

        /// <summary>
        /// Gets the optional number of elements (e.g. pills or ampoules) which are currently in this pack.
        /// This value is used to identify opened packs which does not contain the full amount of pills or ampoules anymore.
        /// A value of 0 means that the pack is completely filled.
        /// </summary>
        uint SubItemQuantity { get; }

        /// <summary>
        /// Gets the depth of the pack in millimeter.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Gets the width of the pack in millimeter.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the pack in millimeter.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the shape of the pack.
        /// </summary>
        PackShape Shape { get; }

        /// <summary>
        /// Gets the current state of the pack. It might happen that even if a pack
        /// is stored in a storage system, it is not available for dispensing.
        /// For example if there is currently a maintenance processed at the storage system.
        /// </summary>
        PackState State { get; }

        /// <summary>
        /// Gets a flag whether the pack is stored in a refrigerator.
        /// </summary>
        bool IsInFridge { get; }

        /// <summary>
        /// Gets the stock location of the pack.
        /// </summary>
        string StockLocationId { get; }

        /// <summary>
        /// Gets the machine location of the pack.
        /// Is only relevant for multi machine environments.
        /// </summary>
        string MachineLocation { get; }

        #endregion
    }
}
