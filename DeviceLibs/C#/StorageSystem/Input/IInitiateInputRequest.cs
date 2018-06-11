
using CareFusion.Lib.StorageSystem.Stock;
using System;

namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Interface which defines the methods and properties of an initiate input request.
    /// </summary>
    public interface IInitiateInputRequest
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier number which is used to identify an initiated input process.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the destination identifier of the storage system that should start the input process.
        /// </summary>
        int Destination { get; }

        /// <summary>
        /// Gets the current state of the initiated input process.
        /// </summary>
        InitiateInputRequestState State { get; }

        /// <summary>
        /// If the initiated pack input is part of a new stock delivery, this property
        /// specifies the delivery number which will be used during pack input.
        /// If this property is null, this pack input is NOT part of a new delivery.
        /// </summary>
        string DeliveryNumber { get; }

        /// <summary>
        /// Flag whether the storage system will set a so-called picking 
        /// indicator during the initiated input to enforce a redefinition 
        /// of the requested pack articles as storage system capable articles.
        /// This flag is usually used when the new storage system 
        /// gets filled for the very first time to realize a kind
        /// of first-time synchronization between the IT system and 
        /// the storage system according to which articles are "storage
        /// system articles".
        /// </summary>
        bool PickingIndicator { get; }

        /// <summary>
        /// Gets the number which defines the input source that should be used 
        /// to transfer the pack into the storage system. 
        /// </summary>
        int InputSource { get; }

        /// <summary>
        /// Gets the detailed information according to the part of the input source 
        /// which is used to transfer the pack into the storage system (e.g. belt number).
        /// </summary>
        int InputPoint { get; }

        /// <summary>
        /// Gets the list of packs that are ready to be input.
        /// </summary>
        IPack[] InputPacks { get; }

        /// <summary>
        /// Gets the information of the articles and the packs that are processed during
        /// the input. This property is set after the input process started.
        /// </summary>
        IArticle[] InputArticles { get; }
                        
        #endregion

        #region Events

        /// <summary>
        /// Event which is raised when the initiated input process finished execution.
        /// </summary>
        /// <param name="sender">The initiated input process instance which raised the event.</param>
        /// <param name="e">Not used. Always null.</param>
        event EventHandler Finished;

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new pack that is ready for input.
        /// </summary>
        /// <param name="scanCode">Raw scan code of the pack.</param>
        /// <param name="batchNumber">Optional batch number of the pack.</param>
        /// <param name="externalId">Optional external identifier of the pack.</param>
        /// <param name="expiryDate">Optional expiry date of the pack.</param>
        /// <param name="subItemQuantity">Optional number of elements (e.g. pills or ampoules) which are currently in this pack.</param>
        /// <param name="depth">Optional depth of the pack in mm.</param>
        /// <param name="width">Optional width of the pack in mm.</param>
        /// <param name="height">Optional height of the pack in mm.</param>
        /// <param name="shape">Optional shape of the pack.</param>
        /// <param name="stockLocationId">Optional stock location to use for the pack.</param>
        /// <param name="machineLocation">Optional machine location to use for the pack.</param>
        void AddInputPack(string scanCode, 
                          string batchNumber = null, 
                          string externalId = null,
                          Nullable<DateTime> expiryDate = null,
                          int subItemQuantity = 0,
                          int depth = 0,
                          int width = 0,
                          int height = 0,
                          PackShape shape = PackShape.Cuboid,
                          string stockLocationId = null,
                          string machineLocation = null);

        /// <summary>
        /// Requests the error detailes for a the specified processed pack.
        /// </summary>
        /// <param name="pack">The pack to get the error detailes for.</param>
        /// <param name="errorType">On successful return the type of error that occurred during input.</param>
        /// <param name="errorText">On successful return the additional text of the error that occurred during input.</param>
        /// <returns><c>true</c> if the specified pack had an error during input;<c>false</c> otherwise.</returns>
        bool GetProcessedPackError(IPack pack, out InputErrorType errorType, out string errorText);

        /// <summary>
        /// Initiates the input process by sending the according request to the storage system.
        /// </summary>
        void Start();

        #endregion
    }
}
