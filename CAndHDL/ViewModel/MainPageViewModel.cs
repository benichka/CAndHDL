﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CAndHDL.Helpers;
using CAndHDL.Model;

namespace CAndHDL.ViewModel
{
    /// <summary>
    /// ViewModel for the MainPage
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region general information
        /// <summary>Oldest comic number</summary>
        public uint NumOldestComic { get; set; }

        /// <summary>Oldest comic date</summary>
        public DateTime DateOldestComic { get; set; }

        /// <summary>Latest comic number</summary>
        public uint NumLatestComic { get; set; }

        /// <summary>Latest comic date</summary>
        public DateTime DateLatestComic { get; set; }
        #endregion general information

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
                this.SetProperty(ref this._StartDate, value);
                this.CheckDates();
            }
        }

        private DateTimeOffset? _EndDate;
        /// <summary>End date</summary>
        public DateTimeOffset? EndDate
        {
            get { return this._EndDate; }
            set
            {
                this.SetProperty(ref this._EndDate, value);
                this.CheckDates();
            }
        }

        private bool? _DLAll;
        /// <summary>Download all the comics</summary>
        public bool? DLAll
        {
            get { return this._DLAll; }
            set
            {
                SetProperty(ref this._DLAll, value);
                this.SetTodayAsEndDate.RaiseCanExecuteChanged();
                this.DatesAreChoosable = this.CheckIfDateAreChoosable();
            }
        }

        private bool? _DLSinceLastTime;
        /// <summary>Download all the comics since last download</summary>
        public bool? DLSinceLastTime
        {
            get { return this._DLSinceLastTime; }
            set
            {
                SetProperty(ref this._DLSinceLastTime, value);
                this.SetTodayAsEndDate.RaiseCanExecuteChanged();
                this.DatesAreChoosable = this.CheckIfDateAreChoosable();
            }
        }

        private bool _DatesAreChoosable;
        /// <summary>Indicate if the dates are choosable or not</summary>
        public bool DatesAreChoosable
        {
            get { return this._DatesAreChoosable; }
            set
            {
                SetProperty(ref this._DatesAreChoosable, value);
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

        private string _ErrorMessage;
        /// <summary>Error message</summary>
        public string ErrorMessage
        {
            get { return this._ErrorMessage; }
            set
            {
                SetProperty(ref this._ErrorMessage, value);
            }

        }

        private string _InfoMessage;
        /// <summary>Information message</summary>
        public string InfoMessage
        {
            get { return this._InfoMessage; }
            set
            {
                SetProperty(ref this._InfoMessage, value);
            }
        }
        #endregion DataModel properties

        #region commands
        /// <summary>
        /// Command associated with the button that set the end date
        /// with today
        /// </summary>
        public Command SetTodayAsEndDate { get; private set; }

        /// <summary>
        /// Command associated with the button that download the comics
        /// </summary>
        public Command StartDL { get; set; }
        #endregion commands

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPageViewModel()
        {
            this.SetTodayAsEndDate = new Command(this.TodayAsEndDate, this.CheckIfDateAreChoosable);

            this.StartDL = new Command(this.DL, this.CheckIfDLIsPossible);

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
        }

        /// <summary>
        /// Put the current date as the end date
        /// </summary>
        private void TodayAsEndDate()
        {
            this.EndDate = DateTime.Now;
        }

        /// <summary>
        /// Check if the dates (start and end) are choosable
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CheckIfDateAreChoosable()
        {
            return ((this.DLAll.HasValue && !this.DLAll.Value) && (this.DLSinceLastTime.HasValue && !this.DLSinceLastTime.Value));
        }

        /// <summary>
        /// Choose a folder to store the comics
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        public async void ChooseFolder(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                this.Path = folder.Path;

                await DownloadHelper.InitRootFolder(folder);
            }
        }

        /// <summary>
        /// Download the comics in the interval
        /// </summary>
        private void DL()
        {
            // TODO: make it async and DL the comics in the interval
            //DownloadHelper.GetComic(DateTime.Now);
        }

        /// <summary>
        /// Check if the start date is before (or the same day as) the end date
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CheckDates()
        {
            if ((!this.StartDate.HasValue || !this.EndDate.HasValue) || DateTimeOffset.Compare(this.StartDate.Value.Date, this.EndDate.Value.Date) <= 0)
            {
                this.ErrorMessage = string.Empty;
                return true;
            }
            else
            {
                this.ErrorMessage = "La date de début doit être inférieure ou égale à la date de fin";
                return false;
            }
        }

        /// <summary>
        /// Check if the download is possible
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CheckIfDLIsPossible()
        {
            return true;
        }
        // TODO: retrieve comics

        /// <summary>
        /// Initialisation: get the first and last comic date and number, etc.
        /// </summary>
        public async void Init()
        {
            IProgress<string> progress = new Progress<string>(s => this.InfoMessage = s);

            progress.Report("Initialising...");

            // Initialise the root folder
            var selectedFolder = await DownloadHelper.InitRootFolder(null);
            this.Path = selectedFolder.Path;

            // Init: extract number and date for the oldest comic
            progress.Report("Processing oldest comic");
            try
            {
                var oldestComic = await DownloadHelper.GetOldestComic(progress);
                this.NumOldestComic = oldestComic.Number;
                this.DateOldestComic = oldestComic.Date;
                this.StartDate = oldestComic.Date;
            }
            catch (Exception getOldestPageEx)
            {
                progress.Report(getOldestPageEx.Message);
            }

            // Init: extract number and date for the latest comic
            progress.Report("Processing latest comic");
            try
            {
                var latestComic = await DownloadHelper.GetLatestComic(progress);
                this.NumLatestComic = latestComic.Number;
                this.DateLatestComic = latestComic.Date;
                this.EndDate = latestComic.Date;
            }
            catch (Exception getLatestPageEx)
            {
                progress.Report(getLatestPageEx.Message);
            }

            // TODO: delete this
            //try
            //{
            //    await DownloadHelper.GetComics(1000, 1010, progress);
            //}
            //catch (Exception getComicExc)
            //{
            //    progress.Report(getComicExc.Message);
            //}

            try
            {
                await DownloadHelper.GetComics(new DateTime(1998, 1, 1), new DateTime(1998, 1, 10), progress);
            }
            catch (Exception getComicExc)
            {
                progress.Report(getComicExc.Message);
            }

            progress.Report("Initialised");
        }

        #region event handling
        /// <summary>Event handler</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the changing property event
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
