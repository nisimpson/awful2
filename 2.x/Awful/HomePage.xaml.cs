﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Awful.ViewModels;

namespace Awful
{
    public partial class HomePage : PhoneApplicationPage
    {
        public HomePage()
        {
            InitializeComponent();
            Datasource = new HomePageViewModel();
            MainPivot.DataContext = Datasource;
        }

        private HomePageViewModel Datasource
        {
            get;
            set;
        }

        private void LoadPivotContent(object sender, SelectionChangedEventArgs e)
        {
            var index = (sender as Pivot).SelectedIndex;
            var item = Datasource.Items[index];
            if (!item.IsDataLoaded)
                item.LoadData();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // if we got here from the login page...
            if (e.IsNavigationInitiator && e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}