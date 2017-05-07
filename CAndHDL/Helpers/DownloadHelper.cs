using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CAndHDL.Exceptions;
using CAndHDL.Model;
using Windows.Storage;
using Windows.Storage.Search;

namespace CAndHDL.Helpers
{
    /// <summary>
    /// Class where all the download methods are
    /// </summary>
    public static class DownloadHelper
    {
        /// <summary>Root folder where the comic will be downloaded</summary>
        private static StorageFolder rootFolder = null;

        #region URLs
        /// <summary>Root URL for the comic</summary>
        private static readonly string URL_ROOT = "http://explosm.net/comics/";

        /// <summary>Root URL for the comic images</summary>
        private static readonly string URL_DL_ROOT = "http://files.explosm.net/comics/";

        /// <summary>Root URL for the archived comics</summary>
        private static readonly string URL_ROOT_ARCHIVE = "http://explosm.net/comics/archive/";
        #endregion URLs

        #region regex patterns
        /// <summary>Pattern to extract the comic page URL on a page</summary>
        private static readonly string PATTERN_URL_CUR = @"id=""permalink"" .* value=""(.*)"" .*";

        /// <summary>Pattern to extract the comic relative image URL on a page (path + number)</summary>
        private static readonly string PATTERN_URL_REL_IMG = @"id=""main-comic"" src=""\/\/files\.explosm\.net\/comics\/(.*)""";

        /// <summary>Pattern to extract the comic number in the comic page URL</summary>
        private static readonly string PATTERN_NUM_IMG = @"comics/(.*)/";

        /// <summary>Pattern to extract the comic name in a relative URL</summary>
        private static readonly string PATTERN_NAME_IMG = @"(.*\/)?(?<name>[^?]*)(\?.*)?";

        /// <summary>Pattern to extract the raw date from a comic page</summary>
        private static readonly string PATTERN_RAW_DATE = @"<h3 .*><a .*>(.*)<\/a><\/h3>";
        #endregion regex patterns

        #region init
        /// <summary>
        /// Init the storage location with a desired folder. If the desired folder is not available,
        /// the root folder is set to AppData\Local\Packages\[AppPackageName]\LocalState\CAndHDL\
        /// </summary>
        /// <param name="desiredFolder">Desired folder where the user wants to store comics</param>
        public static async Task<StorageFolder> InitRootFolder(StorageFolder desiredFolder)
        {
            if (desiredFolder != null)
            {
                rootFolder = desiredFolder;
            }
            else
            {
                rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("CAndHDL", CreationCollisionOption.OpenIfExists);
            }

            return rootFolder;
        }
        #endregion init

        #region download
        /// <summary>
        /// Download a comic page
        /// </summary>
        /// <param name="URL">Comic page URL</param>
        /// <param name="progress">Provider for progress update</param>
        /// <returns>The page body</returns>
        private static async Task<string> DownloadPage(Uri URL, IProgress<string> progress)
        {
            using (var client = new HttpClient())
            {
                progress.Report("Downloading comic page...");

                var httpResponse = await client.GetAsync(URL);

                progress.Report("Processing response...");
                if (!httpResponse.IsSuccessStatusCode)
                {
                    progress.Report($"Unable to get comic at {URL}");
                    throw new HttpRequestException($"Unable to get comic at {URL}. Status code: {httpResponse.StatusCode}");
                }

                progress.Report("Parsing data...");
                var httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

                progress.Report("Finished downloading comic page");

                return httpResponseBody;
            }
        }

        /// <summary>
        /// Download a comic based on its URL
        /// </summary>
        /// <param name="comic">Comic to download</param>
        /// <param name="locationPath">Location to store the downloaded comic</param>
        /// <param name="progress">Provider for progress update</param>
        private static async Task DownloadComic(Comic comic, string locationPath, IProgress<string> progress)
        {
            // Init the file location
            var comicFile = await rootFolder.CreateFileAsync(locationPath, CreationCollisionOption.ReplaceExisting);

            // Download the comic
            using (var client = new HttpClient())
            {
                progress.Report($"Downloading comic image number {comic.Number}, date {comic.Date.ToString("d")}...");

                var comicAsByte = await client.GetByteArrayAsync(comic.DlURL);

                using (Stream stream = await comicFile.OpenStreamForWriteAsync())
                {
                    stream.Write(comicAsByte, 0, comicAsByte.Length);
                }

                progress.Report("Finished downloading comic image");
            }
        }
        #endregion download

        #region comic download
        /// <summary>
        /// Download the oldest comic
        /// </summary>
        /// <param name="progress">Provider for progress update</param>
        public static async Task<Comic> GetOldestComic(IProgress<string> progress)
        {
            progress.Report("Get oldest comic information...");

            var oldestComic = await GetComicInfo(new Uri(URL_ROOT + "oldest", UriKind.Absolute), progress);

            progress.Report("Oldest comic information retrieved");
            return oldestComic;
        }

        /// <summary>
        /// Download the latest comic
        /// </summary>
        /// <param name="progress">Provider for progress update</param>
        public static async Task<Comic> GetLatestComic(IProgress<string> progress)
        {
            progress.Report("Get latest comic information...");

            var latestComic = await GetComicInfo(new Uri(URL_ROOT + "latest", UriKind.Absolute), progress);

            progress.Report("Latest comic information retrieved");

            return latestComic;
        }

        /// <summary>
        /// Get a comic by its date
        /// </summary>
        /// <param name="comicDate">Comic date</param>
        /// <param name="path">Destination path</param>
        /// <param name="progress">Provider for progress update</param>
        private static async Task<Comic> GetComic(DateTime comicDate, string path, IProgress<string> progress)
        {
            progress.Report($"Getting comic {comicDate.ToString("d")}...");

            var comicNumber = await GetComicNumber(comicDate, progress);

            var comic = await GetComic(comicNumber, progress);

            return comic;
        }

        /// <summary>
        /// Get a comic by its number
        /// </summary>
        /// <param name="comicNumber">Comic number</param>
        /// <param name="progress">Provider for progress update</param>
        /// <returns>The downloaded comic information</returns>
        private static async Task<Comic> GetComic(uint comicNumber, IProgress<string> progress)
        {
            progress.Report($"Getting comic {comicNumber}...");

            // Retrieve comic information
            var comic = await GetComicInfo(comicNumber, progress);

            // Set the target path
            var comicTargetName = comicNumber + "-" + comic.Date.ToString("yyyyMMdd") + "-" + comic.Name;

            // Write the comic to disk
            await DownloadComic(comic, comicTargetName, progress);

            progress.Report($"Done Getting comic {comicNumber}");

            return comic;
        }

        /// <summary>
        /// Get a range of comics within a date range
        /// </summary>
        /// <param name="dateMin">Minimum date (included)</param>
        /// <param name="dateMax">Maximum date (included)</param>
        /// <param name="progress">Provider for progress update</param>
        public static async Task GetComics(DateTime dateMin, DateTime dateMax, IProgress<string> progress)
        {
            // It's not directly possible to download an image based on its date; therefore
            // the interval is calculated with the first and last date and then downloaded
            // based on the numbers.
            // Because of that, some comics could not be downloaded because in the early years of C&H some
            // comics in the date d+1 got a number inferior to the comic in the date d
            progress.Report($"Getting comics coming from {dateMin.ToString("d")} to {dateMax.ToString("d")}");

            // TODO: if there is no comic for first, take the first next number that exist (with a loop until?)
            // TODO: if there is no comic for last, take the first previous number that exist
            var first = await GetComicNumber(dateMin, progress);
            var last = await GetComicNumber(dateMax, progress);

            await GetComics(first, last, progress);
        }

        /// <summary>
        /// Get a range of comics within a number range
        /// </summary>
        /// <param name="numMin">Minimum number (included)</param>
        /// <param name="numMax">Maximum number (included)</param>
        /// <param name="progress">Provider for progress update</param>
        public static async Task GetComics(uint numMin, uint numMax, IProgress<string> progress)
        {
            progress.Report($"Getting comics coming from {numMin} to {numMax}");

            Comic latestDLedComic = null;

            for (uint i = numMin; i <= numMax; i++)
            {
                try
                {
                   latestDLedComic = await GetComic(i, progress);
                }
                // Exceptions regarding an unknow number in the serie, a date that cannot be parsed, etc. -> we only log it
                catch (HttpRequestException httpRequestEx) { progress.Report(httpRequestEx.Message); }
                catch (ComicURLNotFoundException comicURLNotFoundEx) { progress.Report(comicURLNotFoundEx.Message); }
                catch (ComicNumberNotFoundException comicNumberNotFoundEx) { progress.Report(comicNumberNotFoundEx.Message); }
                catch (ComicNameNotFoundException comicNameNotFoundEx) { progress.Report(comicNameNotFoundEx.Message); }
                catch (ComicDateNotFoundException comicDateNotFoundEx) { progress.Report(comicDateNotFoundEx.Message); }
                catch (Exception)
                {
                    throw;
                }
            }
            progress.Report("Comics downloaded");

            if (latestDLedComic != null)
            {
                await StoreLatestDownload(latestDLedComic);
            }
        }
        #endregion comic download

        #region retrieve comic information
        /// <summary>
        /// Get comic information based on its number
        /// </summary>
        /// <param name="comicNumber">Comic number</param>
        /// <param name="progress">Provider for progress update</param>
        /// <returns>The comic information</returns>
        public static async Task<Comic> GetComicInfo(uint comicNumber, IProgress<string> progress)
        {
            var comic = new Comic()
            {
                PageURL = new Uri(URL_ROOT + comicNumber, UriKind.Absolute),
                Number = comicNumber,
                Name = string.Empty,
                Date = DateTime.MinValue
            };

            // Page download
            var responsePage = await DownloadPage(comic.PageURL, progress);

            // Comic relative URL and absolute URL extraction
            GetComicURLs(responsePage, out var comicPageURL, out var comicRelURL, out var comicSourcePath);

            comic.DlURL = comicSourcePath;

            // Comic name extraction
            comic.Name = GetComicName(comicRelURL);

            // Comic date extraction
            comic.Date = GetComicDate(responsePage);

            return comic;
        }

        /// <summary>
        /// Get comic information based on its date
        /// </summary>
        /// <param name="date">Comic date</param>
        /// <param name="progress">Provider for progress update</param>
        /// <returns>The comic information</returns>
        public static async Task<Comic> GetComicInfo(DateTime date, IProgress<string> progress)
        {
            var comicNumber = await GetComicNumber(date, progress);

            var comic = await GetComicInfo(comicNumber, progress);

            return comic;
        }

        /// <summary>
        /// Get comic information based on its page URL
        /// </summary>
        /// <param name="comicURL">Comic page URL</param>
        /// <param name="progress">Provider for progress update</param>
        /// <returns>The comic information</returns>
        public static async Task<Comic> GetComicInfo(Uri comicURL, IProgress<string> progress)
        {
            var comic = new Comic();

            // Page download
            var responsePage = await DownloadPage(comicURL, progress);

            // Comic relative URL and absolute URL extraction
            GetComicURLs(responsePage, out var comicPageURL, out var comicRelURL, out var comicSourcePath);

            // Comic page URL extraction
            comic.PageURL = comicPageURL;

            // Comic number extraction
            comic.Number = GetComicNumber(comic.PageURL);

            // Comic download URL extraction
            comic.DlURL = comicSourcePath;

            // Comic name extraction
            comic.Name = GetComicName(comicRelURL);

            // Comic date extraction
            comic.Date = GetComicDate(responsePage);

            return comic;
        }
        #endregion retrieve comic information

        #region get comic properties
        /// <summary>
        /// Get a comic page URL, relative URL and absolute URL based on the page where the comic is
        /// </summary>
        /// <param name="responsePage">Page as string for the comic that we want to download</param>
        /// <param name="comicPageURL">Comic page URL</param>
        /// <param name="comicRelURL">Comic relative URL</param>
        /// <param name="comicSourcePath">Comic absolute URL</param>
        /// <exception cref="ComicURLNotFoundException">Thrown when the URLs cannot be extracted</exception>
        private static void GetComicURLs(string responsePage, out Uri comicPageURL, out Uri comicRelURL, out Uri comicSourcePath)
        {
            // Extracting comic page URL
            var comicPageURLMatch = Regex.Match(responsePage, PATTERN_URL_CUR);
            if (comicPageURLMatch.Success)
            {
                comicPageURL = new Uri(comicPageURLMatch.Groups[1].Value, UriKind.Absolute);
            }
            else
            {
                throw new ComicURLNotFoundException($"there was a problem extracting the page URL of comic: the URL was not found in the downloaded page: {responsePage}");
            }

            // Extracting comic relative URL and download URL
            var comicRelURLMatch = Regex.Match(responsePage, PATTERN_URL_REL_IMG);
            if (comicRelURLMatch.Success)
            {
                comicRelURL = new Uri(comicRelURLMatch.Groups[1].Value, UriKind.Relative);
                comicSourcePath = new Uri(URL_DL_ROOT + comicRelURL, UriKind.Absolute);
            }
            else
            {
                throw new ComicURLNotFoundException($"there was a problem extracting the URL of comic: the URL was not found in the downloaded page: {responsePage}");
            }
        }

        /// <summary>
        /// Get a comic number based on its relative URL
        /// </summary>
        /// <param name="comicPageURL">Comic page URL</param>
        /// <returns>The comic number</returns>
        /// <exception cref="ComicNumberNotFoundException">Thrown when the number cannot be extracted</exception>
        private static uint GetComicNumber(Uri comicPageURL)
        {
            var comicNumber = default(uint);

            var comicNumberMatch = Regex.Match(comicPageURL.ToString(), PATTERN_NUM_IMG);
            if (comicNumberMatch.Success)
            {
                comicNumber = Convert.ToUInt32(comicNumberMatch.Groups[1].Value);
            }
            else
            {
                throw new ComicNumberNotFoundException($"there was a problem extracting the comic number; the number was not found in the URL provided ({comicPageURL})");
            }

            return comicNumber;
        }

        /// <summary>
        /// Get a comic number based on its date.<br />
        /// This method is very costly because it download a page at every call.
        /// </summary>
        /// <param name="date">Comic date</param>
        /// <param name="progress">Provider for progress update</param>
        /// <returns>The comic number</returns>
        /// <exception cref="ComicDateNotFoundException">Thrown when there is no date for the given comic</exception>
        private static async Task<uint> GetComicNumber(DateTime date, IProgress<string> progress)
        {
            var comicNumber = default(uint);

            var year = date.ToString("yyyy");
            var month = date.ToString("MM");
            var day = date.ToString("dd");

            var pattern_url_cur = @"<a href=""(.*)\/(?<number>[0-9]*)\/"">" + year + "." + month + "." + day;

            // Page download
            // for some reason, the URL to the archive for each month that is the same month as the current date
            // is URL_ROOT_ARCHIVE/yyyy. Otherwise, it's URL_ROOT_ARCHIVE/yyyy/MM
            var URLArchive = string.Empty;
            if (month == DateTime.Now.ToString("MM"))
            {
                URLArchive = URL_ROOT_ARCHIVE + year;
            }
            else
            {
                URLArchive = URL_ROOT_ARCHIVE + year + "/" + month;
            }
            var responsePage = await DownloadPage(new Uri(URLArchive, UriKind.Absolute), progress);

            // Extracting comic page URL
            var comicNumberMatch = Regex.Match(responsePage, pattern_url_cur);
            if (comicNumberMatch.Success)
            {
                comicNumber = Convert.ToUInt32(comicNumberMatch.Groups["number"].Value);
            }
            else
            {
                throw new ComicDateNotFoundException($"there is no comic for the date {date.ToString("d")}");
            }

            return comicNumber;
        }

        /// <summary>
        /// Get a comic name based on its relative URL
        /// </summary>
        /// <param name="comicRelURL">Comic relative URL</param>
        /// <returns>The comic name</returns>
        /// <exception cref="ComicNameNotFoundException">Thrown when the comic name cannot be extracted</exception>
        private static string GetComicName(Uri comicRelURL)
        {
            var comicName = string.Empty;

            var comicNameMatch = Regex.Match(comicRelURL.ToString(), PATTERN_NAME_IMG);
            if (comicNameMatch.Success)
            {
                comicName = comicNameMatch.Groups["name"].Value;
            }
            else
            {
                throw new ComicNameNotFoundException($"there was a problem extracting the comic name; the name was not found in the URL provided ({comicRelURL})");
            }

            return comicName;
        }

        /// <summary>
        /// Get a comic date based on the page where the comic is
        /// </summary>
        /// <param name="responsePage">Page as string for the comic that we want to download</param>
        /// <returns>The comic date</returns>
        /// <exception cref="ComicDateNotFoundException">Thrown when the comic date cannot be extracted</exception>
        private static DateTime GetComicDate(string responsePage)
        {
            var comicRawDate = string.Empty;
            var comicDate = DateTime.MinValue;

            var comicRawDateMatch = Regex.Match(responsePage, PATTERN_RAW_DATE);
            if (comicRawDateMatch.Success)
            {
                comicRawDate = comicRawDateMatch.Groups[1].Value;
                comicDate = DateTime.ParseExact(comicRawDate, "yyyy.MM.dd", CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ComicDateNotFoundException($"there was a problem extracting the comic date; the date was not found in the downloaded page: {responsePage}");
            }

            return comicDate;
        }
        #endregion get comic properties

        #region latest download information
        /// <summary>
        /// Write the latest comic downloaded information as a file in the root folder.
        /// The file is named this way : latest_[comic number]_[comic date]
        /// </summary>
        /// <param name="latestComic">Latest comic information</param>
        private static async Task StoreLatestDownload(Comic latestComic)
        {
            // delete the previous file if there is one
            var previousInfo = await GetLatestInfoFile();
            if (previousInfo != null)
            {
                await previousInfo.DeleteAsync();
            }

            // Create an empty file with the number and the date
            await rootFolder.CreateFileAsync($"latest_{latestComic.Number}_{latestComic.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}", CreationCollisionOption.ReplaceExisting);
        }

        /// <summary>
        /// Retrieve the latest comic downloaded information
        /// </summary>
        /// <returns>The latest comic downloaded information</returns>
        public static async Task<Comic> GetLatestDownload()
        {
            Comic latestComic = null;

            var latest = await GetLatestInfoFile();
            if (latest != null)
            {
                Regex regExtractInfo = new Regex(@"latest_(?<number>[0-9]*)_(?<date>[0-9]*)");
                var match = regExtractInfo.Match(latest.Name);
                latestComic = new Comic()
                {
                    Number = Convert.ToUInt32(match.Groups["number"].Value),
                    Date = DateTime.ParseExact(match.Groups["date"].Value, "yyyyMMdd", CultureInfo.InvariantCulture)
                };
            }

            return latestComic;
        }

        /// <summary>
        /// Get the file containing the latest download information
        /// </summary>
        /// <returns>The file if there is one; null otherwise</returns>
        private static async Task<StorageFile> GetLatestInfoFile()
        {
            // "Quick and dirty"
            // var infoAsFile = (await rootFolder.GetFilesAsync()).Where(file => file.DisplayName.StartsWith("latest_")).FirstOrDefault();

            StorageFile result = null;

            var queryOptions = new QueryOptions();
            queryOptions.ApplicationSearchFilter = $"System.FileName:latest_*";

            StorageFileQueryResult queryResult = rootFolder.CreateFileQueryWithOptions(queryOptions);

            var files = await queryResult.GetFilesAsync();
            if (files.Count > 0)
            {
                // In case there are multiple files, only the first one is returned
                result = files.First();
            }

            return result;
        }
        #endregion latest download information
    }
}
