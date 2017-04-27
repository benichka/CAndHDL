using System;

namespace CAndHDL.Model
{
    /// <summary>
    /// Comic model
    /// </summary>
    public class Comic
    {
        /// <summary>Comic page URL</summary>
        public Uri PageURL { get; set; }

        /// <summary>Comic download URL</summary>
        public Uri DlURL { get; set; }

        /// <summary>Comic name</summary>
        public string Name { get; set; }

        /// <summary>Comic number</summary>
        public uint Number { get; set; }

        /// <summary>Comic date</summary>
        public DateTime Date { get; set; }
    }
}
