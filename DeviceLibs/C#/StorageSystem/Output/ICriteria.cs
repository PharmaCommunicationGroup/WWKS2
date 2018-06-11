using System;

namespace CareFusion.Lib.StorageSystem.Output
{
    /// <summary>
    /// Interface which defines the methods and properties of an output process criteria.
    /// </summary>
    public interface ICriteria
    {
        #region Properties

        /// <summary>
        /// Gets the unique article identifier (e.g. PZN or EAN) filter criterion of the requested packs.
        /// </summary>
        string ArticleId { get; }

        /// <summary>
        /// Gets the amount of full packs to dispense.
        /// </summary>
        uint Quantity { get; }

        /// <summary>
        /// Gets the number of elements (e.g. pills or ampoules) to dispense. 
        /// If this property is set to a value greater than 0, the Quantity property is ignored and should be 0.
        /// </summary>
        uint SubItemQuantity { get; }

        /// <summary>
        /// Gets the optional additional filter criterion to request only packs that have at least the specified expiry date.
        /// A value of null indicates that the expiry date is not relevant for the pack choice.
        /// </summary>
        Nullable<DateTime> MinimumExpiryDate { get; }

        /// <summary>
        /// Gets the optional additional batch number filter criterion for the requested packs. 
        /// </summary>
        string BatchNumber { get; }

        /// <summary>
        /// Gets the optional additional flag whether all requested articles have to belong to one batch number.
        /// </summary>
        bool SingleBatchNumber { get; }

        /// <summary>
        /// Gets the optional additional external identifier filter criterion for the requested packs.
        /// </summary>
        string ExternalId { get; }

        /// <summary>
        /// Gets the optional additional filter criterion which refers to the storage system internal pack identifier.
        /// </summary>
        ulong PackId { get; }

        /// <summary>
        /// Gets the optional stock location filter criterion for the requested packs.
        /// </summary>
        string StockLocationId { get; }

        /// <summary>
        /// Gets the optional machine location filter criterion for the requested packs.
        /// </summary>
        string MachineLocation { get; }

        /// <summary>
        /// Gets the list of labels which are currently assigned to this output process criteria.
        /// </summary>
        ILabel[] Labels { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new label definition to this output process criteria.
        /// </summary>
        /// <param name="templateId">
        /// The identifier of the label template which has to be used by 
        /// the label printer to process the label content correctly.
        /// </param>
        /// <param name="content">
        /// The arbitrary label content to use when printing labels on the packs.
        /// </param>
        void AddLabel(string templateId, string content);

        #endregion
    }
}
