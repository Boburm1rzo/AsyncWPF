using AsyncWPF.Models;
using AsyncWPF.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace AsyncWPF
{
    /// <summary>                            
    /// Interaction logic for MainWindow.xaml
    /// </summary>               
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cts;
        private readonly UniversityService _universityService;
        private readonly BitcoinService _bitcoinService;
        private readonly Stopwatch _stopwatch;
        private readonly List<University> universities;
        private readonly ObservableCollection<Bitcoin> bitcoins;
        private static object locker = new object();
        private static object locker1 = new object();

        int counter1 = 0;
        int counter2 = 0;

        public MainWindow()
        {
            InitializeComponent();

            _universityService = new();
            _bitcoinService = new();

            universities = [];
            bitcoins = [];

            _stopwatch = Stopwatch.StartNew();

            SearchInput.KeyDown += ((sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    Search_Click(sender, e);
                }
            });

            BitcoinList.ItemsSource = bitcoins;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelSave_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (universities.Count < 1)
            {
                MessageBox.Show(
                    "No data to save!",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (!TryGetFileName(out string? fileName))
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(_ => SaveToFile(fileName!));
        }

        private void SaveToFile(string fileName)
        {
            Dispatcher.Invoke(() => BeforeLoading());
            string json = string.Empty;

            lock (locker)
            {
                foreach (var item in Enumerable.Range(0, 1_000_000))
                {
                    var index = Random.Shared.Next(0, universities.Count);
                    universities.Add(universities[index]);
                }
                json = JsonConvert.SerializeObject(universities);
            }

            FileStream? stream = null;
            try
            {
                if (File.Exists(fileName))
                {
                    stream = File.OpenWrite(fileName);
                }
                else
                {
                    stream = File.Create(fileName);
                }

                var writer = new StreamWriter(stream);
                writer.WriteLine(json);

                MessageBox.Show(
                   $"Data was saved to file successfully.",
                   "Success",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                   $"Error occurred while saving data to file. Please, try again. Details: {ex.Message}",
                   "Error",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);
            }
            finally
            {
                stream?.Dispose();
                Dispatcher.Invoke(() => AfterLoading());
            }
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            BeforeLoading();
            Search.Content = "Cancel";

            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                    return;
                }

                cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                var country = SearchInput.Text;
                var data = _universityService.GetByCountryName(country, cts.Token);
            }
            catch (Exception ex)
            {
                Notes.Text += "\n" + ex.Message;
            }
            finally
            {
                Search.Content = "Search";
                Dispatcher.Invoke(AfterLoading);
                cts.Dispose();
            }
        }

        private async void RefreshBitcoin_Click(object sender, RoutedEventArgs e)
        {
            BeforeLoading();

            try
            {
                //var loadTask = _bitcoinService.GetLatest();
                //var saveTask = loadTask.ContinueWith(async task => await SaveToFile(task.Result));
                //var resultTask = saveTask.ContinueWith(async task =>
                //{
                //    await task;
                //    MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
                //});

                //await resultTask;

                var loadTask = await _bitcoinService.GetLatest(cts.Token); // thread 1

                await SaveToFile(loadTask); // throw

                bitcoins.Add(loadTask);
            }
            catch (Exception ex)
            {
                Notes.Text = $"Error fetching bitcoin data. Details: {ex.Message}";
            }
            finally
            {
                AfterLoading();
            }
        }

        private void BeforeLoading()
        {
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.IsIndeterminate = true;
            _stopwatch.Restart();
        }

        private void AfterLoading()
        {
            ProgressBar.Visibility = Visibility.Hidden;
            _stopwatch.Stop();
            LoaderStatus.Text = $"Data loaded in {_stopwatch.ElapsedMilliseconds}ms.";
        }

        private void UpdateUniversities(List<University> data)
        {
            lock (locker)
            {
                universities.Clear();
                universities.AddRange(data);
            }
        }

        private async Task SaveToFile(Bitcoin bitcoin)
        {
            ArgumentNullException.ThrowIfNull(bitcoin);

            string fileName = "bitcoins.txt";
            using var writer = new StreamWriter(fileName, true);

            var task = writer.WriteLineAsync(bitcoin.ToString()).ConfigureAwait(false);
            await task;
        }

        private static bool TryGetFileName(out string? fileName)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "data",
                DefaultExt = ".json",
                Filter = "Text documents (.json)|*.json"
            };

            var result = dialog.ShowDialog();

            if (result is not true)
            {
                fileName = null;
                return false;
            }

            fileName = dialog.FileName;
            return true;
        }
    }
}