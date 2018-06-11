using System;

namespace CareFusion.Lib.StorageSystem.Output
{
	/// <summary>
	/// Interface which defines the methods and properties of a pack output process information set.
	/// </summary>
	public interface IOutputProcessInfo
	{
		#region Properties
		
		/// <summary>
        /// Gets the current state of the output process.
        /// </summary>
        OutputProcessState State { get; }
        
        /// <summary>
        /// Gets the list of packs which were dispensed by the output process.
        /// This property is set after the output process finished.
        /// </summary>
        IDispensedPack[] Packs { get; }

        /// <summary>
        /// Gets the list of boxes which were involved during pack dispensing.
        /// This property is set after the output process finished.
        /// </summary>
        string[] Boxes { get; }
        
        #endregion
	}
}
