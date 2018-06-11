using System;

namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Interface which defines the methods and properties of a pack that is requested for input by a storage system.
    /// </summary>
    public interface IInputPack
    {
        #region Properties

        /// <summary>
        /// Gets the raw scan code of the pack.
        /// </summary>
        string ScanCode { get; }

        /// <summary>
        /// Gets the batch number of the pack if one was specified by the storage system.
        /// </summary>
        string BatchNumber { get; }

        /// <summary>
        /// Gets the external identifier of the pack.
        /// </summary>
        string ExternalId { get; }

        /// <summary>
        /// Gets the expiry date of the pack if one was specified by the storage system.
        /// If no expiry date was specified, this property is null.
        /// </summary>
        Nullable<DateTime> ExpiryDate { get; }

        /// <summary>
        /// Gets the optional number of elements (e.g. pills or ampoules) which are currently in this pack.
        /// This value is used to identify opened packs which does not contain the full amount of pills or ampoules anymore.
        /// A value of 0 means that the pack is completely filled.
        /// </summary>
        uint SubItemQuantity { get; }

        /// <summary>
        /// Gets the stock location to use for the pack which was specified by the storage system.
        /// </summary>
        string StockLocationId { get; }

        /// <summary>
        /// Gets the machine location to use for the pack.
        /// Is only relevant for multi machine environments.
        /// </summary>
        string MachineLocation { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the article related information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="articleId">Unique identifier of the article.</param>
        /// <param name="articleName">Name of the article.</param>
        /// <param name="dosageForm">Dosage form of the article.</param>
        /// <param name="packagingUnit">Packaging unit of the article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        void SetArticleInformation(string articleId, 
                                   string articleName, 
                                   string dosageForm, 
                                   string packagingUnit, 
                                   uint maxSubItemQuantity = 0);

        /// <summary>
        /// Sets the virtual article related information for this article.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="articleId">Unique identifier of the virtual article.</param>
        /// <param name="articleName">Name of the virtual article.</param>
        /// <param name="dosageForm">Dosage form of the virtual article.</param>
        /// <param name="packagingUnit">Packaging unit of the virtual article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        void SetVirtualArticleInformation(string articleId,
                                   string articleName,
                                   string dosageForm,
                                   string packagingUnit,
                                   uint maxSubItemQuantity = 0);
        
        /// <summary>
        /// Adds other article information related to the same virtual article.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="articleId">Unique identifier of the virtual article.</param>
        /// <param name="articleName">Name of the virtual article.</param>
        /// <param name="dosageForm">Dosage form of the virtual article.</param>
        /// <param name="packagingUnit">Packaging unit of the virtual article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        void AddOtherRobotArticleInformation(string articleId,
                                   string articleName,
                                   string dosageForm,
                                   string packagingUnit,
                                   uint maxSubItemQuantity = 0);

        /// <summary>
        /// Sets the pack specific information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="batchNumber">
        /// Optional batch number of this pack or empty string.
        /// This paramter will overwrite the originally suggested one of the storage system.
        /// </param>
        /// <param name="externalId">Optional external identifier of this pack or empty string.</param>
        void SetPackInformation(string batchNumber, string externalId);

        /// <summary>
        /// Sets the pack specific information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="batchNumber">
        /// Optional batch number of this pack or empty string.
        /// This paramter will overwrite the originally suggested one of the storage system.
        /// </param>
        /// <param name="externalId">
        /// Optional external identifier of this pack or empty string.
        /// </param>
        /// <param name="expiryDate">
        /// Expiry date of this pack. If this parameter is null, the storage system will automatically choose an expiry date.
        /// This paramter will overwrite the originally suggested one of the storage system.
        /// </param>
        /// <param name="subItemQuantity">
        /// Optional number of elements (e.g. pills or ampoules) which are currently in this pack.
        /// This value is used to identify opened packs which does not contain the full amount of pills or ampoules anymore.
        /// A value of 0 means that the pack is completely filled.
        /// This paramter will overwrite the originally suggested one of the storage system.
        /// </param>
        /// <param name="stockLocationId">
        /// Optional stock location to use for this pack.
        /// If this parameter is not null, it will overwrite the originally suggested one of the storage system.
        /// </param>
        void SetPackInformation(string batchNumber, 
                                string externalId, 
                                Nullable<DateTime> expiryDate, 
                                uint subItemQuantity,
                                string stockLocationId = null);

        /// <summary>
        /// Defines the input handling for this pack.
        /// The input handling instructs the storage system how to handle the scanned pack (e.g. input the pack).
        /// </summary>
        /// <param name="handling">The handling which describes how to handle the pack.</param>
        void SetHandling(InputHandling handling);

        /// <summary>
        /// Defines the input handling for this pack.
        /// The input handling for a pack instructs the storage system how to handle the scanned pack (e.g. input the pack).
        /// </summary>
        /// <param name="handling">
        /// The handling which describes how to handle the pack.
        /// </param>
        /// <param name="message">
        /// Additional message to show in the UI of the storage system.
        /// This message is typically used in case of a pack rejection to show a more detailed rejection reason.
        /// </param>
        void SetHandling(InputHandling handling, string message);

        #endregion
    }
}
