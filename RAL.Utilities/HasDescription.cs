using System;
using System.Collections.Generic;
using System.Text;

namespace TheColonel2688.Utilities
{
    public abstract class HasDescription : IHasDescription
    {
        /// <summary
        /// Name Or Description of class instance. This is used to provide context for logging, this is meant to report the Description of the last Descendant
        /// </summary>
        public virtual string Description { get; protected set; }
    }

    public interface IHasDescription
    {
        string Description
        {
            get;
        }
    }
}
