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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.Threading;
using Telerik.Windows.Controls;


namespace Awful
{
    public partial class App : Application
    {
        public const string THREAD_RATING_COLOR_1 = "ThreadRating1Brush";
        public const string THREAD_RATING_COLOR_2 = "ThreadRating2Brush";
        public const string THREAD_RATING_COLOR_3 = "ThreadRating3Brush";
        public const string THREAD_RATING_COLOR_4 = "ThreadRating4Brush";
        public const string THREAD_RATING_COLOR_5 = "ThreadRating5Brush";

        public const string POST_UNREAD_COLOR = "PostUnreadBrush";
        public const string POST_READ_COLOR = "PostReadBrush";

        private static AppDataModel model;
        public static AppDataModel Model
        {
            get
            {
                if (model == null)
                    model = new AppDataModel();

                return model;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
#if DEBUG
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

#endif

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }  
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {

#if DEBUG
            //IsolatedStorageExplorer.Explorer.Start("localhost");
#endif

            Model.LoadSettings();
            Model.Init();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
#if DEBUG
            //IsolatedStorageExplorer.Explorer.RestoreFromTombstone();
#endif

            Model.LoadSettings();
            Model.LoadStateFromIsoStorage();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Model.SaveSettings();
            Model.SaveStateToIsoStorage();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Model.SaveSettings();
            Model.SaveStateToIsoStorage();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            MessageBox.Show(e.Exception.StackTrace, e.Exception.Message, MessageBoxButton.OK);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            string stackTrace = e.ExceptionObject.StackTrace == null ? string.Empty : e.ExceptionObject.StackTrace.ToString();
            string message = e.ExceptionObject.Message == null ? string.Empty : e.ExceptionObject.Message;
            AwfulDebugger.AddLog(sender, AwfulDebugger.Level.Critical, e.ExceptionObject);

            MessageBox.Show(e.ExceptionObject.StackTrace, e.ExceptionObject.Message, MessageBoxButton.OK);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(stackTrace, message, MessageBoxButton.OK);
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        // An unhandled exception has occurred; break into the debugger
                        System.Diagnostics.Debugger.Break();
                    }
                });
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            //RootFrame = new PhoneApplicationFrame();

            /*
            RadTransition transition = new RadTransition()
            {
                BackwardInAnimation = this.Resources["fadeInAnimation"] as RadAnimation,
                BackwardOutAnimation = this.Resources["fadeOutAnimation"] as RadAnimation,
                ForwardInAnimation = this.Resources["fadeInAnimation"] as RadAnimation,
                ForwardOutAnimation = this.Resources["fadeOutAnimation"] as RadAnimation
            };
            */
          
            RadPhoneApplicationFrame frame = new RadPhoneApplicationFrame()
            {
                Transition = this.Resources["pageTransition"] as RadTransition
                //ClockwiseOrientationChangeAnimation = this.Resources["rotateRightAnimation"] as RadAnimation,
                //CounterClockwiseOrientationChangeAnimation = this.Resources["rotateLeftAnimation"] as RadAnimation
            };

            RootFrame = frame;
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}