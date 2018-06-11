using CareFusion.Lib.StorageSystem.Stock;

namespace CareFusion.Lib.StorageSystem.Output
{
    /// <summary>
    /// Interface which defines the methods and properties of a pack which was dispensed by a stroage system.
    /// </summary>
    public interface IDispensedPack : IPack
    {
        #region Properties

        /// <summary>
        /// Gets the unique article identifier (e.g. PZN or EAN) of the dispensed pack.
        /// </summary>
        string ArticleId { get; }

        /// <summary>
        /// If the pack was dispensed to a box within a box system scenario,
        /// this property identifies the box the pack was dispensed to.
        /// </summary>
        string BoxNumber { get; }

        /// <summary>
        /// Gets the destination that was used to dispense this pack. 
        /// In some error scenarios it might happen that the pack is dispensed to another 
        /// output destination than the requested one (e.g. the output destination was not 
        /// operational when the output process was running).
        /// </summary>
        int OutputDestination { get; }

        /// <summary>
        /// Gets the number which may define a more detailed  part of the destination 
        /// that was used to dispense the pack (e.g. a specific conveyor belt or slide). 
        /// </summary>
        int OutputPoint { get; }

        /// <summary>
        /// Gets the labelling status of the dispensed pack. 
        /// Only relevant when pack labelling was requested.
        /// </summary>
        LabelState LabelState { get; }

        #endregion
    }
}
