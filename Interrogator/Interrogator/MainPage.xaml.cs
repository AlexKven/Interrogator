using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Interrogator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private int? _Reading;
        public int? Reading
        {
            get { return _Reading; }
            set
            {
                _Reading = value;
                if (value.HasValue)
                    ReadingBox.Text = value.Value.ToString() + " BPM";
                else
                    ReadingBox.Text = "--";
                if (!value.HasValue)
                {
                    StateBox.Text = "Acquiring heart rate...";
                }
                else
                {
                    StateBox.Text = "Heart rate locked.";
                }
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void InterrogateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    StateBox.Text = "This sample app requires a Microsoft Band paired to your phone. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
                }

                // Connect to Microsoft Band.
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    // Subscribe to Accelerometer data.
                    bandClient.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
                    bandClient.SensorManager.Contact.SupportedReportingIntervals.ToString();
                    await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                    await bandClient.SensorManager.Contact.StartReadingsAsync();


                    // Receive Accelerometer data for a while.
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    await bandClient.SensorManager.HeartRate.StopReadingsAsync();
                    await bandClient.SensorManager.Contact.StopReadingsAsync();
                }
            }
            catch (Exception ex)
            {
                StateBox.Text = ex.ToString();
            }
        }

        async void Contact_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandContactReading> e)
        {
            return;
            //BandReadingState state;
            //if (e.SensorReading.State == Microsoft.Band.Sensors.BandContactState.NotWorn)
            //  state = BandReadingState.NotWorn;
            //else
            //  state = Reading.ReadingState;
            //if (Reading.ReadingState == BandReadingState.NotWorn) state = BandReadingState.NotWorn;
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Reading = new HeartRateReading() { Reading = Reading.Reading, ReadingState = state }; }).AsTask();
        }

        async void HeartRate_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {
            string verdict = e.SensorReading.HeartRate.ToString() + ", quality: " + e.SensorReading.Quality.ToString();
            int? reading;
            if (e.SensorReading.Quality == Microsoft.Band.Sensors.HeartRateQuality.Acquiring)
                reading = null;
            else
                reading = e.SensorReading.HeartRate;
            //if (Reading.ReadingState == BandReadingState.NotWorn) state = BandReadingState.NotWorn;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Reading = reading; }).AsTask();
        }
    }
}