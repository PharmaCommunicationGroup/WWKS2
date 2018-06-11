using CareFusion.Lib.StorageSystem.State;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Component datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Component : IComponent
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string State { get; set; }

        [XmlAttribute]
        public string StateText { get; set; }

        #endregion

        #region IComponent Specific Properties

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        [XmlIgnore]
        ComponentType IComponent.Type
        {
            get { return TypeConverter.ConvertEnum<ComponentType>(this.Type, ComponentType.StorageSystem); }
        }

        /// <summary>
        /// Gets the description of the component.
        /// </summary>
        [XmlIgnore]
        string IComponent.Description
        {
            get { return this.Description != null ? TextConverter.EscapeInvalidXmlChars(this.Description) : string.Empty; }
        }

        /// <summary>
        /// Gets the state of the component.
        /// </summary>
        [XmlIgnore]
        ComponentState IComponent.State
        {
            get { return TypeConverter.ConvertEnum<ComponentState>(this.State, ComponentState.NotReady); }
        }

        /// <summary>
        /// Gets the optional additional state description of the component.
        /// </summary>
        [XmlIgnore]
        string IComponent.StateText
        {
            get { return this.StateText != null ? TextConverter.EscapeInvalidXmlChars(this.StateText) : string.Empty; }
        }

        #endregion        
    }
}
