﻿using Microsoft.Win32;
using Nanoleaf_Api.Endpoints;
using Nanoleaf_Models.Models;
using Nanoleaf_wpf.ViewModels;
using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Windows;

namespace Nanoleaf_wpf.Views.Options
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private static readonly string _appName = "NanoleafWPF";

        public OptionsViewModel OptionsViewModel { get; set; }

        private RegistryKey _startupKey;

        public OptionsWindow()
        {
            InitializeComponent();

            OptionsViewModel = new OptionsViewModel
            {
                StartAtWindowsStartUp = UserSettings.Settings.StartAtWindowsStartup,
                Latitude = UserSettings.Settings.Latitude?.ToString("N7", CultureInfo.InvariantCulture),
                Longitude = UserSettings.Settings.Longitude?.ToString("N7", CultureInfo.InvariantCulture)
            };

            _startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            DataContext = OptionsViewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            double latitude = 0;
            double longitude = 0;
            try
            {
                latitude = Convert.ToDouble(OptionsViewModel.Latitude, CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for latitude");
                return;
            }

            try
            {
                longitude = Convert.ToDouble(OptionsViewModel.Longitude, CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for longitude");
                return;
            }

            if (latitude != UserSettings.Settings.Latitude || longitude != UserSettings.Settings.Longitude)
            {
                var endpoint = new SunsetEndpoint();

                try
                {
                    var sunTimes = endpoint.GetSunsetSunrise(latitude, longitude).GetAwaiter().GetResult();

                    UserSettings.Settings.UpdateSunriseSunset(sunTimes.SunriseHour, sunTimes.SunriseMinute, sunTimes.SunsetHour, sunTimes.SunsetMinute);
                }
                catch
                {
                    MessageBox.Show("Something went wrong when updating the sunrise and sunset times");
                    return;
                }

                UserSettings.Settings.Latitude = latitude;
                UserSettings.Settings.Longitude = longitude;
            }

            if (UserSettings.Settings.StartAtWindowsStartup != OptionsViewModel.StartAtWindowsStartUp)
            {
                if (OptionsViewModel.StartAtWindowsStartUp)
                {
                    _startupKey.SetValue(_appName, $"{System.Reflection.Assembly.GetExecutingAssembly().Location} -s");
                }
                else
                {
                    _startupKey.DeleteValue(_appName, false);
                }

                _startupKey.Close();

                UserSettings.Settings.StartAtWindowsStartup = OptionsViewModel.StartAtWindowsStartUp;
            }

            UserSettings.Settings.SaveSettings();
            Close();
        }
    }
}
