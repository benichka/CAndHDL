using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAndHDL.Helpers
{
    /// <summary>
    /// Class where all the controls on the page are
    /// </summary>
    public static class ControlsHelper
    {
        /// <summary>
        /// Check the validity of the chosen date
        /// </summary>
        /// <param name="dateFirstComic">Date of the first comic</param>
        /// <param name="dateLastComic">Date of the last comic</param>
        /// <param name="chosenDate">Date chosen by the user</param>
        public static void CheckDate(DateTime dateFirstComic, DateTime dateLastComic, DateTime chosenDate)
        {
            // TODO: implement CheckDate
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check the validity of the chosen number
        /// </summary>
        /// <param name="numFirstComic">First comic number</param>
        /// <param name="numLastComic">Last comic number</param>
        /// <param name="chosenNumber"></param>
        public static void CheckNumber(uint numFirstComic, uint numLastComic, uint chosenNumber)
        {
            // TODO: implement CheckNumber
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check the validity of the chosen range (as dates)
        /// </summary>
        /// <param name="dateFirstComic">First comic date</param>
        /// <param name="dateLastComic">Last comic date</param>
        /// <param name="rangeMin">Minimum date range chosen by the user</param>
        /// <param name="rangeMax">Maximum date range chosen by the user</param>
        public static void CheckRange(DateTime dateFirstComic, DateTime dateLastComic, DateTime rangeMin, DateTime rangeMax)
        {
            // TODO: implement CheckRange (dates)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check the validity of the chosen range (as numbers)
        /// </summary>
        /// <param name="numFirstComic">First comic number</param>
        /// <param name="numLastComic">Last comic number</param>
        /// <param name="rangeMin">Minimum number range chosen by the user</param>
        /// <param name="rangeMax">Maximum number range chosen by the user</param>
        public static void CheckRange(uint numFirstComic, uint numLastComic, uint rangeMin, uint rangeMax)
        {
            // TODO: implement CheckRange (numbers)
            throw new NotImplementedException();
        }
    }
}
