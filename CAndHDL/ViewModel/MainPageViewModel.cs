using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CAndHDL.Model;

namespace CAndHDL.ViewModel
{
    /// <summary>
    /// ViewModel for the MainPage
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        // TODO: try to access/update the model directly
        #region DataModel properties
        /// <summary>Data model</summary>
        private DataModel DataModel { get; set; }

        private DateTimeOffset? _StartDate;
        /// <summary>Start date</summary>
        public DateTimeOffset? StartDate
        {
            get { return this._StartDate; }
            set
            {
                SetProperty(ref this._StartDate, value);
            }
        }

        private DateTimeOffset? _EndDate;
        /// <summary>End date</summary>
        public DateTimeOffset? EndDate
        {
            get { return this._EndDate; }
            set
            {
                SetProperty(ref this._EndDate, value);
            }
        }

        private bool _DLAll;
        /// <summary>Download all the comics</summary>
        public bool DLAll
        {
            get { return this._DLAll; }
            set
            {
                SetProperty(ref this._DLAll, value);
            }
        }

        private bool _DLSinceLastTime;
        /// <summary>Download all the comics since last download</summary>
        public bool DLSinceLastTime
        {
            get { return this._DLSinceLastTime; }
            set
            {
                SetProperty(ref this._DLSinceLastTime, value);
            }
        }

        private string _Path;
        /// <summary>Path to the comic folder</summary>
        public string Path
        {
            get { return this._Path; }
            set
            {
                SetProperty(ref this._Path, value);
            }
        }
        #endregion DataModel properties

        #region commands
        public Command SetTodayAsEndDate { get; private set; }
        #endregion commands

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPageViewModel()
        {
            this.SetTodayAsEndDate = new Command(this.TodayAsEndDate, () => { return (!this.DLAll && !this.DLSinceLastTime); } );

            this.DataModel = new DataModel();

            // TODO: retrieve the data from the precedent execution

            if (this.DataModel.StartDate == null)
            {
                this.DataModel.StartDate = new DateTime(2000, 1, 1);
            }

            if (this.DataModel.EndDate == null)
            {
                this.DataModel.EndDate = DateTime.Now;
            }

            // TODO: probably not necessary when the model is directly accessed
            this.StartDate = this.DataModel.StartDate;
            this.EndDate = this.DataModel.EndDate;
            this.DLAll = this.DataModel.DLAll;
            this.DLSinceLastTime = this.DataModel.DLSinceLastTime;
            this.Path = this.DataModel.Path;

            // TODO: manage commands
        }

        /// <summary>
        /// Put the current date as the end date
        /// </summary>
        public void TodayAsEndDate()
        {
            this.EndDate = DateTime.Now;
        }

        // TODO: retrieve comics

        #region event handling
        /// <summary>Event fired when a property is changed in the UI</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Changing property event
        /// </summary>
        /// <param name="propertyName">Changing property</param>
        protected void RaisedPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Field change notification only if the field has really changed
        /// </summary>
        /// <typeparam name="T">Field type</typeparam>
        /// <param name="storage">Initial value</param>
        /// <param name="value">Updated value</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>true if the field value changed, false otherwise</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                this.RaisedPropertyChanged(propertyName);
                return true;
            }
        }
        #endregion event handling
    }
}
