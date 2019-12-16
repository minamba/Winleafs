﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for TaskbarIcon.xaml
    /// </summary>
    public partial class TaskbarIcon : UserControl
    {
        private static readonly int _amountOfEffects = 5;

        private MainWindow _parent;

        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    SelectedDeviceChanged();
                    DevicesDropdown.SelectedItem = _selectedDevice;
                }
            }
        }

        private int _brightness;

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }

        private string _selectedEffect;

        public ObservableCollection<string> DeviceNames { get; set; }

        public TaskbarIcon()
        {
            InitializeComponent();

            DataContext = this;
        }

        //Called after the MainWindow is intialized
        public void Initialize(MainWindow mainWindow)
        {
            _parent = mainWindow;

            DeviceNames = _parent.DeviceNames;
            //TODO: Brightness = _parent.OverrideScheduleUserControl.Brightness;

            BuildMostUsedEfectList();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SelectedDeviceChanged()
        {
            BuildMostUsedEfectList();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _selectedEffect = null;

            BuildMostUsedEfectList();
        }

        public void BuildMostUsedEfectList()
        {
            MostUsedEffectList.Children.Clear();

            if (UserSettings.Settings.ActiveDevice == null)
            {
                return;
            }

            if (UserSettings.Settings.ActiveDevice.EffectCounter == null)
            {
                UserSettings.Settings.ActiveDevice.EffectCounter = new Dictionary<string, ulong>();
                return;
            }

            var mostUsedEffects = UserSettings.Settings.ActiveDevice.EffectCounter.Take(_amountOfEffects).ToList();

            foreach (var mostUsedEffect in mostUsedEffects)
            {
                MostUsedEffectList.Children.Add(new MostUsedEffectUserControl(this, mostUsedEffect.Key, mostUsedEffect.Key == _selectedEffect));
            }
        }

        public void EffectSelected(string effectName)
        {
            _selectedEffect = effectName;

            BuildMostUsedEfectList();

            SetManualControl();
        }

        private void SetManualControl()
        {
            //TODO: make this work, also update the correct user control in the main window
        }

        private void BrightnessSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            SetManualControl();
        }
    }
}