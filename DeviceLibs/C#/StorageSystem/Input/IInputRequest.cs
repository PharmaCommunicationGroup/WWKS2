
namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Interface which defines the methods and properties of a pack input request.
    /// </summary>
    public interface IInputRequest
    {
        #region Properties

        /// <summary>
        /// Gets the identifier of the input request.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the source identifier of the input request.
        /// </summary>
        int Source { get; }

        /// <summary>
        /// If the pack input is part of a new stock delivery, this property
        /// specifies the delivery number which is usually entered by the user
        /// at the storage system UI.
        /// If this property is null, this pack input is NOT part of a new delivery.
        /// </summary>
        string DeliveryNumber { get; }

        /// <summary>
        /// Flag whether a so-called picking indicator was set by the 
        /// storage system to enforce a redefinition of the requested 
        /// pack articles as storage system capable articles.
        /// This flag is usually used when the new storage system 
        /// gets filled for the very first time to realize a kind
        /// of first-time synchronization between the IT system and 
        /// the storage system according to which articles are "storage
        /// system articles".
        /// </summary>
        bool PickingIndicator { get; }

        /// <summary>
        /// Gets the list of packs, the storage system wants to input.
        /// The IT system iterates this pack list, defines further article details
        /// for each pack and decides wether to allow the pack input or not.
        /// Afterwards the method "Finish" is called to complete the input process.
        /// </summary>
        IInputPack[] Packs { get; }

        #endregion

        /// <summary>
        /// Finishes the input request by sending the according answer to
        /// the storage system. This method has to be called after the list of
        /// requested packs was updated to allow or reject pack input.
        /// </summary>
        void Finish();
    }
}
