using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using CAndHDL.Helpers;
using CAndHDL.Model;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

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

        #region properties
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
                this.DatesAreChoosable = this.CheckIfDateAreChoosable();
                this.CheckIfCheckboxesAreChoosable();
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
                this.DatesAreChoosable = this.CheckIfDateAreChoosable();
                this.CheckIfCheckboxesAreChoosable();
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

        private bool _DLAllIsChoosable;
        /// <summary>Indicate if the "DL All" checkbox is choosable</summary>
        public bool DLAllIsChoosable
        {
            get { return this._DLAllIsChoosable; }
            set
            {
                SetProperty(ref this._DLAllIsChoosable, value);
            }
        }

        private bool _DLSinceLastTimeIsChoosable;
        /// <summary>Indicate if the "DL since last time" checkbox is choosable</summary>
        public bool DLSinceLastTimeIsChoosable
        {
            get { return this._DLSinceLastTimeIsChoosable; }
            set
            {
                SetProperty(ref this._DLSinceLastTimeIsChoosable, value);
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
        #endregion properties

        #region commands
        private bool _IsProcessing;
        /// <summary>Boolean indicating whether the comics are being downloaded</summary>
        public bool IsProcessing
        {
            get { return this._IsProcessing; }
            set
            {
                SetProperty(ref this._IsProcessing, value);
            }
        }

        /// <summary>Cancellation token source</summary>
        private CancellationTokenSource _DLTokenSource = null;

        /// <summary>
        /// Command associated with the button that set the end date
        /// with today
        /// </summary>
        public Command SetTodayAsEndDate { get; private set; }

        /// <summary>
        /// Command associated with the button that download the comics
        /// </summary>
        public Command StartDL { get; set; }

        /// <summary>Command associated with the cancel button</summary>
        public Command Cancel { get; set; }

        /// <summary>
        /// Command associated with the button that copy the path
        /// </summary>
        public Command CopyPath { get; private set; }
        #endregion commands

        #region init
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPageViewModel()
        {
            this.IsProcessing = false;

            this._DLTokenSource = new CancellationTokenSource();

            this.SetTodayAsEndDate = new Command(this.TodayAsEndDate, this.CheckIfDateAreChoosable);

            this.StartDL = new Command(this.DL, this.CheckIfDLIsPossible);

            this.Cancel = new Command(this.CancelClick, this.CanCancelClick);

            this.CopyPath = new Command(this.CopyPathAction, this.CheckIfPathCopyPossible);

            if (this.StartDate == null)
            {
                this.StartDate = new DateTime(2000, 1, 1);
            }

            if (this.EndDate == null)
            {
                this.EndDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Initialisation: get the first and last comic date and number, etc.
        /// </summary>
        public async void Init()
        {
            this.IsProcessing = true;

            IProgress<string> progress = new Progress<string>(s => this.InfoMessage = s);

            progress.Report("Initialising...");

            StorageFolder folder = null;
            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(@"C:\Users\ben\Desktop\CAndH\");
            }
            catch (Exception ex)
            {
                progress.Report($"Unable to set the desired folder: {ex.Message}");
            }

            // Initialise the root folder
            var selectedFolder = await DownloadHelper.InitRootFolder(folder);
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

            this.IsProcessing = false;

            progress.Report("Initialised");
        }
        #endregion init

        #region actions
        /// <summary>
        /// Put the current date as the end date
        /// </summary>
        private void TodayAsEndDate()
        {
            this.EndDate = DateTime.Now;
        }

        /// <summary>
        /// Copy the path where the comic will be downloaded
        /// </summary>
        private void CopyPathAction()
        {
            IProgress<string> progress = new Progress<string>(s => this.InfoMessage = s);

            var dataPackage = new DataPackage();
            dataPackage.SetText(this.Path);

            Clipboard.SetContent(dataPackage);

            progress.Report("Path copied");
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

                DownloadHelper.ChangeDLFolder(folder);
            }
        }

        /// <summary>
        /// Download the comics in the interval
        /// </summary>
        private async void DL()
        {
            this.IsProcessing = true;

            IProgress<string> progress = new Progress<string>(s => this.InfoMessage = s);

            if (this.DLAll.HasValue && this.DLAll.Value)
            {
                // The box "DL All" is checked: download all comics since the beginning to the end
                progress.Report("Downloading all comics...");
                await DownloadHelper.GetComics(this.NumOldestComic, this.NumLatestComic, this._DLTokenSource.Token, progress);
            }
            else if (this.DLSinceLastTime.HasValue && this.DLSinceLastTime.Value)
            {
                // Download all comics since last time
                Comic firstComicToDL = null;

                var latestComicInfo = await DownloadHelper.GetLatestDownload();

                // If there is no info for the latest download (first time or the file was deleted), the download starts
                // at the first comic
                if (latestComicInfo == null)
                {
                    firstComicToDL = new Comic()
                    {
                        Number = this.NumOldestComic,
                        Date = this.DateOldestComic
                    };
                }
                else
                {
                    firstComicToDL = new Comic()
                    {
                        Number = (latestComicInfo.Number + 1),
                        Date = latestComicInfo.Date.AddDays(1)
                    };
                }
                progress.Report($"Downloading all comics starting on: {firstComicToDL.Date.ToString("d")}");
                await DownloadHelper.GetComics(firstComicToDL.Number, this.NumLatestComic, this._DLTokenSource.Token, progress);
            }
            else
            {
                // Classic mode: download all comics in the interval
                progress.Report($"Downloading comics from {this.StartDate.Value.ToString("d")} to {this.EndDate.Value.ToString("d")}");
                await DownloadHelper.GetComics(this.StartDate.Value.DateTime, this.EndDate.Value.DateTime, this._DLTokenSource.Token, progress);
            }

            this.IsProcessing = false;

            if (this._DLTokenSource.Token.IsCancellationRequested)
            {
                // If a cancellation has been done, the cancellation token source is regenerated
                this._DLTokenSource.Dispose();
                this._DLTokenSource = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Method executed when the user clicks on the cancel button
        /// </summary>
        private void CancelClick()
        {
            this._DLTokenSource.Cancel();

            // The processing is stopped; this automatically disabled the cancel button,
            // so that the user can only click it once
            this.IsProcessing = false;

            this.InfoMessage = "A cancellation has been requested";
        }
        #endregion actions

        #region controls
        /// <summary>
        /// Check if the dates (start and end) are choosable
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CheckIfDateAreChoosable()
        {
            return ((this.DLAll.HasValue && !this.DLAll.Value) && (this.DLSinceLastTime.HasValue && !this.DLSinceLastTime.Value));
        }

        /// <summary>
        /// Check if the checkboxes are choosable
        /// </summary>
        private void CheckIfCheckboxesAreChoosable()
        {
            this.DLAllIsChoosable = !(this.DLSinceLastTime.HasValue && this.DLSinceLastTime.Value);
            this.DLSinceLastTimeIsChoosable = !(this.DLAll.HasValue && this.DLAll.Value);
        }

        /// <summary>
        /// Check if the start date is before (or the same day as) the end date
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CheckDates()
        {
            if ((!this.StartDate.HasValue || !this.EndDate.HasValue))
            {
                this.ErrorMessage = "La date de début et la date de fin doivent être valorisées";
                return false;
            }
            else if (DateTimeOffset.Compare(this.StartDate.Value.Date, this.EndDate.Value.Date) > 0)
            {
                this.ErrorMessage = "La date de début doit être inférieure ou égale à la date de fin";
                return false;
            }
            else
            {
                if (this.DateOldestComic != DateTime.MinValue && DateTime.Compare(this.StartDate.Value.DateTime, this.DateOldestComic) < 0)
                {
                    this.StartDate = this.DateOldestComic;
                    this.InfoMessage = "La date de début a été remplacée par la date du premier comic";
                }
                if (this.DateLatestComic != DateTime.MinValue && DateTime.Compare(this.EndDate.Value.DateTime, this.DateLatestComic) > 0)
                {
                    this.EndDate = this.DateLatestComic;
                    this.InfoMessage = "La date de fin a été remplacée par la date du dernier comic";
                }
                this.ErrorMessage = string.Empty;
                return true;
            }
        }

        /// <summary>
        /// Check if the download is possible
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CheckIfDLIsPossible()
        {
            var DLIsPossible = true;
            if (!this.CheckDates() && this.IsProcessing)
            {
                DLIsPossible = false;
            } 
            return (DLIsPossible && !this.IsProcessing);
        }

        /// <summary>
        /// Check if the path copy is possible
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CheckIfPathCopyPossible()
        {
            return !String.IsNullOrWhiteSpace(this.Path);
        }

        /// <summary>
        /// Method to determine if the user can cancel the action
        /// </summary>
        /// <returns>true if that's the case, false otherwise</returns>
        private bool CanCancelClick()
        {
            return this.IsProcessing;
        }

        /// <summary>
        /// Check if every command are executable
        /// </summary>
        private void CheckCommands()
        {
            this.CopyPath.RaiseCanExecuteChanged();
            this.StartDL.RaiseCanExecuteChanged();
            this.SetTodayAsEndDate.RaiseCanExecuteChanged();
            this.Cancel.RaiseCanExecuteChanged();
        }
        #endregion controls

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
                this.CheckCommands();
                return true;
            }
        }
        #endregion event handling
    }
}
