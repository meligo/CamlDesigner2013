// ----------------------------------------------------------------------
// <copyright file="MainScreen.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml;
    using CamlDesigner.ViewModels;
    using WinInterop = System.Windows.Interop;
    using CamlDesigner.Helpers;

    public enum TreeViewState
    {
        ShowWebs,
        ShowLists,
        ShowFields,
        ShowSiteColumns,
        ShowContentTypes
    }

    public enum QueryType
    {
        ViewFields,
        OrderBy,
        Where,
        QueryOptions
    }

    /// <summary>
    /// POINT aka POINTAPI
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// x coordinate of point.
        /// </summary>
        public int x;
        /// <summary>
        /// y coordinate of point.
        /// </summary>
        public int y;

        /// <summary>
        /// Initializes a new instance of the <see cref="POINT" /> struct.
        /// </summary>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    /// <summary> Win32 </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct RECT
    {
        /// <summary> Win32 </summary>
        public int left;
        /// <summary> Win32 </summary>
        public int top;
        /// <summary> Win32 </summary>
        public int right;
        /// <summary> Win32 </summary>
        public int bottom;

        /// <summary> Win32 </summary>
        public static readonly RECT Empty = new RECT();

        /// <summary> Win32 </summary>
        public int Width
        {
            get
            {
                return Math.Abs(this.right - this.left);
            }
            // Abs needed for BIDI OS
        }
        /// <summary> Win32 </summary>
        public int Height
        {
            get
            {
                return this.bottom - this.top;
            }
        }

        /// <summary> Initializes a new instance of the RECT struct </summary>
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }


        /// <summary> Initializes a new instance of the <see cref="RECT" /> struct.</summary>
        public RECT(RECT rcSrc)
        {
            this.left = rcSrc.left;
            this.top = rcSrc.top;
            this.right = rcSrc.right;
            this.bottom = rcSrc.bottom;
        }

        /// <summary> Win32 </summary>
        public bool IsEmpty
        {
            get
            {
                // BUGBUG : On Bidi OS (hebrew arabic) left > right
                return this.left >= this.right || this.top >= this.bottom;
            }
        }
        /// <summary> Return a user friendly representation of this struct </summary>
        public override string ToString()
        {
            if (this == RECT.Empty)
            {
                return "RECT {Empty}";
            }
            return "RECT { left : " + this.left + " / top : " + this.top + " / right : " + this.right + " / bottom : " + this.bottom + " }";
        }

        /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Rect))
            {
                return false;
            }
            return this == (RECT)obj;
        }

        /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
        public override int GetHashCode()
        {
            return this.left.GetHashCode() + this.top.GetHashCode() + this.right.GetHashCode() + this.bottom.GetHashCode();
        }


        /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
        public static bool operator ==(RECT rect1, RECT rect2)
        {
            return rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom;
        }

        /// <summary> Determine if 2 RECT are different(deep compare)</summary>
        public static bool operator !=(RECT rect1, RECT rect2)
        {
            return !(rect1 == rect2);
        }
    }

    /// <summary>
    /// Interaction logic for GeneralInfo.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        private System.ComponentModel.BackgroundWorker mWorker;

        private TreeViewState treeViewState;
        private QueryType? queryType;
        // controls
        // private QueryControl queryControl = null;

        private SiteColumnControl siteColumnControl = null;

        // treeview models
        private RootViewModel rootViewModel;

        // other variables
        private ListViewModel selectedList = null;

        public MainScreen()
        {
            this.InitializeComponent();
            this.tree.PreviewKeyDown += delegate(object obj, KeyEventArgs args) { args.Handled = true; };
            this.tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(this.Tree_SelectedItemChanged);
            // this.PopulateTreeView();
            this.MainRibbon.QuickAccessToolBar = new Microsoft.Windows.Controls.Ribbon.RibbonQuickAccessToolBar() { Visibility = System.Windows.Visibility.Hidden };
            if (App.CamlWrapper != null)
            {
                Connect.IsEnabled = false;
                itemConnect.IsEnabled = false;
            }
            else
            {
                itemConnect.IsEnabled = true;
                Connect.IsEnabled = true;
            }
            this.Loaded += new RoutedEventHandler(this.MainScreen_Loaded);
            this.SourceInitialized += new EventHandler(this.MainScreen_SourceInitialized);
            this.AddHandler(Keyboard.KeyUpEvent, (KeyEventHandler)this.HandleKeyUpEvent);

            //var userPrefs = new UserPreferences();

            //this.Height = userPrefs.WindowHeight;
            //this.Width = userPrefs.WindowWidth;
            //this.Top = userPrefs.WindowTop;
            //this.Left = userPrefs.WindowLeft;
            //this.WindowState = userPrefs.WindowState;
            // set buttons for default language type and connection type
        }

        public event CAMLEventHandler CAMLEvent;

        bool isExecuting = false;

        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!isExecuting)
            {
                CAMLTextBlock.Text = string.Empty;
                CAMLGridView.ItemsSource = null;
                QueryControl.Clear(true);
                this.InitializeQueryControl(QueryType.Where);
            }
            else
                isExecuting = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var userPrefs = new UserPreferences();

            userPrefs.WindowHeight = this.Height;
            userPrefs.WindowWidth = this.Width;
            userPrefs.WindowTop = this.Top;
            userPrefs.WindowLeft = this.Left;
            userPrefs.WindowState = this.WindowState;

            userPrefs.Save();
        }

        private void HandleKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                // ctrl + C is trapped 
                Clipboard.SetText(CAMLTextBlock.Text.Trim());
            }
        }

        #region SetScreenHeightWidth

        public void MainScreen_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
        }

        public void MainScreen_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            ChildControlsPanel.Visibility = System.Windows.Visibility.Hidden;
        }

        private static System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {

            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                default:
                    break;
            }

            return (System.IntPtr)0;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.Monitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// internal class for getting the monitor settings
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion

        private CAMLDesigner.BusinessObjects.Wrapper CamlWrapper
        {
            get
            {
                if (App.CamlWrapper == null)
                {
                    App.CamlWrapper = new CAMLDesigner.BusinessObjects.Wrapper(
                        App.GeneralInformation.SharePointUrl,
                        App.GeneralInformation.ConnectionType);
                }
                return App.CamlWrapper;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            // store selected tab in the app.config
            if (CamlQueryTab.IsSelected)
                UtilityFunctions.SetAppSetting("Tab", "CamlQueryTab", true);

            Application dummyapp = Application.Current;
            dummyapp.Shutdown();
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.WindowState != System.Windows.WindowState.Maximized)
                {
                    this.WindowState = System.Windows.WindowState.Maximized;
                }
                else
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile("An error has occured in ButtonMaximize_Click", ex);
            }

        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Minimized)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void ContentTypeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButtonConnect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Connect.IsChecked = true;
            Disconnect.IsChecked = false;
            var loginscreen = new LoginScreen();
            loginscreen.ShowDialog();

            // navigate to the last accessed tab. If app is accessed for the first time, activate the CAML Designer tab
            bool isFound = false;
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            if (appSettings != null)
            {
                for (int i = 0; i < appSettings.Count; i++)
                {
                    if (appSettings.GetKey(i) == "Tab")
                    {
                        string selectedTab = appSettings[i];
                        switch (selectedTab)
                        {
                            case "CamlQueryTab":
                                isFound = true;
                                CamlQueryTab.IsSelected = true;
                                break;
                        }
                    }
                }
            }

            if (!isFound)
                CamlQueryTab.IsSelected = true;
        }

        private void RadioButtonDisconnect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Connect.IsChecked = false;
            Disconnect.IsChecked = true;

            // reinitialize the ribbon buttons
            ViewFieldsButton.IsChecked = false;
            QueryWhereButton.IsChecked = false;
            QueryOrderByButton.IsChecked = false;
            QueryOptionsButton.IsChecked = false;
            ViewFieldsButton.IsChecked = false;

            CamlButton.IsChecked = true;
            ServerOMButton.IsChecked = false;
            ClientOMForDotnetButton.IsChecked = false;
            ClientOMForEcmaScriptButton.IsChecked = false;
            ClientOMForSilverlightButton.IsChecked = false;
            WebServicesButton.IsChecked = false;

            this.ReInit();

        }

        private void ReInit()
        {
            // reinitialize the other controls
            CAMLTextBlock.Text = string.Empty;
            CAMLGridView.ItemsSource = null;
            QueryControl.Clear(true);
            QueryControl.Visibility = Visibility.Collapsed;
            this.tree.DataContext = null;
            this.rootViewModel = null;
            App.CamlDocument = null;
            App.GeneralInformation = null;
        }

        private void OnCAMLEvent(CamlEventArgs args)
        {
            if (args == null || args.CamlDocument == null)
            {
                CAMLTextBlock.Text = string.Empty;
                CAMLGridView.ItemsSource = null;
            }
            else
            {
                // get the CAML
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(args.ListName, args.CamlDocument, args.WhereFieldsList, args.QueryOptions, args.ConnectionType, args.LanguageType, args.QueryType);

                // execute the query
                if (App.GeneralInformation.ExecuteQuery)
                {
                    DataTable resultTable = this.CamlWrapper.ExecuteQuery(App.SelectedListViewModel.Title, App.CamlDocument, App.QueryOptions, args.QueryType);
                    if (resultTable != null)
                        CAMLGridView.ItemsSource = resultTable.DefaultView;
                    else
                        CAMLGridView.ItemsSource = null;
                }
                else
                {
                    CAMLGridView.ItemsSource = null;
                }
            }
        }

        private void CamlQueryTab_Selected(object sender, RoutedEventArgs e)
        {
            if (App.ClearControls)
            {
                App.CamlDocument = null;
                tree.DataContext = null;
                QueryControl.Clear(true);
                App.ClearControls = false;

                this.mWorker = new BackgroundWorker();
                this.mWorker.DoWork += new DoWorkEventHandler(this.Worker_DoWork);
                this.mWorker.ProgressChanged += new ProgressChangedEventHandler(this.Worker_ProgressChanged);
                this.mWorker.WorkerReportsProgress = true;
                this.mWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
                _busyIndicator.IsBusy = true;
                this.mWorker.RunWorkerAsync();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _busyIndicator.IsBusy = false;

            // set the execute button
            SiteDataQueryButton.IsChecked = false;
            ExecuteQueryButton.IsChecked = true;
            App.GeneralInformation.ExecuteQuery = true;

            // set the SiteDataQuery button
            if (App.GeneralInformation.ConnectionType == BPoint.SharePoint.Common.Enumerations.ConnectionType.ServerObjectModel)
                SiteDataQueryButton.IsEnabled = true;
            else
                SiteDataQueryButton.IsEnabled = false;

            // check if the correct web is loaded
            RootViewModel rootViewModel = tree.DataContext as RootViewModel;
            if (rootViewModel != null && rootViewModel.SharePointViewModels != null && rootViewModel.SharePointViewModels.Count > 0)
            {
                SharePointViewModel sharePointViewModel = rootViewModel.SharePointViewModels[0];
                if (sharePointViewModel != null && sharePointViewModel.ListViewModels != null && sharePointViewModel.ListViewModels.Count > 0)
                {
                    ListViewModel listViewModel = sharePointViewModel.ListViewModels[0];
                    if (listViewModel.WebUrl != App.GeneralInformation.SharePointUrl)
                    {
                        // change the url of the rootmodel
                        App.GeneralInformation.SharePointUrl = listViewModel.WebUrl;

                        // show a message box that another web is loaded
                        MessageBox.Show("Please, check the URL of the SharePoint site you want to work on: the CAML Designer was not able to connect using this URL but connects to the parent site instead.");
                    }
                }
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (App.GeneralInformation == null)
            {
                this.CallThreadedLoginScreen();
            }

            _busyIndicator.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate()
                {
                    _busyIndicator.BusyContent = "Retrieve SharePoint Information";
                }));

            try
            {
                this.rootViewModel = new RootViewModel("list");

                // Let the UI bind to the view-model.
                if (this.rootViewModel != null)
                {
                    this.treeViewState = TreeViewState.ShowLists;
                    // rootViewModel.TreeViewItemViewModelSelected += new TreeViewItemViewModelDelegate(rootViewModel_TreeViewItemViewModelSelected);
                    this.tree.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                this.tree.DataContext = this.rootViewModel;
                            }));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occurred");
            }
        }

        private bool calledFromThread;

        private void CallThreadedLoginScreen()
        {

            Thread _backgroundThread = new Thread(new ThreadStart(this.CallLogin));
            _backgroundThread.SetApartmentState(ApartmentState.STA);
            _backgroundThread.IsBackground = true;
            _backgroundThread.Start();
            _backgroundThread.Join();
            this.calledFromThread = true;
        }

        private void CallLogin()
        {
            this.CallLoginScreen();
        }

        private void CallLoginScreen()
        {
            var loginscreen = new LoginScreen();
            loginscreen.Topmost = true;

            loginscreen.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() { loginscreen.ShowDialog(); }));

            // if (calledFromThread)
            // {
            // loginscreen.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();
            // System.Windows.Threading.Dispatcher.Run();
            // }
        }

        private void AmnitemConnect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.CallLoginScreen();

            // set the execute button
            ExecuteQueryButton.IsChecked = true;
            App.GeneralInformation.ExecuteQuery = true;
            App.CamlDocument = null;
            QueryControl.Clear(true);
        }

        private void AmnitemDisconnect_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AmnitemSaveQuery_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".caml";
            dlg.Filter = "Caml query files (.caml)|*.caml";
            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                bool succesful = Helpers.SaveFile.SaveContentToCamlFile(filename, CAMLTextBlock.Text);
                if (succesful)
                {
                    MessageBox.Show("Caml query saved succesfully to path: " + filename, "Success", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Caml query not saved to path: " + filename, "Failed", MessageBoxButton.OK);
                }
                // Save file
            }
        }

        private void AmnitemClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application dummyapp = Application.Current;
            dummyapp.Shutdown();
        }

        private void RadioButtonCopyQuery_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(CAMLTextBlock.Text);
            CopyQueryButton.IsChecked = false;
        }

        private void RadioButtonExecuteQuery_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.GeneralInformation.ExecuteQuery = (bool)ExecuteQueryButton.IsChecked;

            if (App.GeneralInformation.ExecuteQuery)
            {
                // execute the query
                DataTable resultTable = this.CamlWrapper.ExecuteQuery(App.SelectedListViewModel.Title, App.CamlDocument, App.QueryOptions, App.QueryType);
                if (resultTable != null)
                    CAMLGridView.ItemsSource = resultTable.DefaultView;
                else
                    CAMLGridView.ItemsSource = null;
            }
        }

        private void RadioButtonClearQuery_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Clear the CAML Query and the controls
            QueryControl.Clear(false);
            CAMLTextBlock.Text = string.Empty;
            CAMLGridView.ItemsSource = null;
            App.CamlDocument = null;
            ClearQueryButton.IsChecked = false;
        }

        private void SiteColumnDesignerTab_Selected(object sender, RoutedEventArgs e)
        {
            // Show the designer
            if (this.siteColumnControl == null)
            {
                this.siteColumnControl = new SiteColumnControl();
                this.siteColumnControl.CAMLEvent += new CAMLEventHandler(this.OnCAMLEvent);
            }

            ChildControlsBorder.Visibility = System.Windows.Visibility.Visible;
            ChildControlsPanel.Children.Clear();
            ChildControlsPanel.Children.Add(this.siteColumnControl);
            CAMLTextBlock.Visibility = System.Windows.Visibility.Visible;

            // TODO: Populate the treeview with existing site columns
        }


        #region CAML Query type buttons
        private void CamlQueryCommand_Executed(object sender, RoutedEventArgs e)
        {
            SiteDataQueryButton.IsChecked = false;
            App.QueryType = BPoint.SharePoint.Common.Enumerations.QueryType.CAMLQuery;
            QueryControl.InitializeQueryOptions(false);
        }


        private void SiteDataQueryCommand_Executed(object sender, RoutedEventArgs e)
        {
            CamlQueryButton.IsChecked = false;
            App.QueryType = BPoint.SharePoint.Common.Enumerations.QueryType.SiteDataQuery;
            if (selectedList != null)
                QueryControl.InitializeQueryOptions(true);
            else
                MessageBox.Show("Please, select a list first", "No list selected", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        #region CAML Query buttons
        private void ViewFieldsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            QueryWhereButton.IsChecked = false;
            QueryOrderByButton.IsChecked = false;
            QueryOptionsButton.IsChecked = false;
            this.InitializeQueryControl(QueryType.ViewFields);
            // BackgroundWorker _backgroundWorker = new BackgroundWorker();
            // _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            // _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
            // _backgroundWorker.RunWorkerAsync();   
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // cancelled
            }
            else if (e.Error != null)
            {
                // exception thrown
            }
            else
            {
                // completed
            }
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QueryWhereButton.IsChecked = false;
            QueryOrderByButton.IsChecked = false;
            QueryOptionsButton.IsChecked = false;
            this.InitializeQueryControl(QueryType.ViewFields);
        }

        private void QueryWhereCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewFieldsButton.IsChecked = false;
            QueryOrderByButton.IsChecked = false;
            QueryOptionsButton.IsChecked = false;
            this.InitializeQueryControl(QueryType.Where);
        }

        private void QueryOrderByCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewFieldsButton.IsChecked = false;
            QueryWhereButton.IsChecked = false;
            QueryOptionsButton.IsChecked = false;
            this.InitializeQueryControl(QueryType.OrderBy);
        }

        private void QueryOptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewFieldsButton.IsChecked = false;
            QueryOrderByButton.IsChecked = false;
            QueryWhereButton.IsChecked = false;
            this.InitializeQueryControl(QueryType.QueryOptions);
        }

        private void InitializeQueryControl(QueryType? qType)
        {
            if (this.CAMLEvent == null)
                QueryControl.CAMLEvent += new CAMLEventHandler(this.OnCAMLEvent);

            if (QueryControl.Visibility == Visibility.Collapsed)
            {
                QueryControl.Visibility = Visibility.Visible;
                QueryControl.Width = ChildControlsPanel.ActualWidth - 10.0;
                QueryControl.Height = ChildControlsPanel.ActualHeight - 10.0;
                ChildControlsPanel.Visibility = System.Windows.Visibility.Visible;
            }

            if (this.selectedList != App.SelectedListViewModel || (qType != null && qType != this.queryType))
            {
                if (this.selectedList != App.SelectedListViewModel)
                {
                    if (qType != null)
                        this.queryType = (QueryType)qType;
                    QueryControl.InitializeFieldsComboBox(QueryControl.Height);
                }
                else if (qType != null && qType != this.queryType)
                {
                    this.queryType = (QueryType)qType;
                }

                if (qType != null)
                {
                    if (queryType != QueryType.QueryOptions)
                    {
                        QueryControl.InitializeTargetComboBox(queryType, QueryControl.Width, QueryControl.Height);
                        switch (App.QueryType)
                        {
                            case BPoint.SharePoint.Common.Enumerations.QueryType.CAMLQuery:
                                QueryControl.InitializeQueryOptions(false);
                                break;
                            case BPoint.SharePoint.Common.Enumerations.QueryType.SiteDataQuery:
                                QueryControl.InitializeQueryOptions(true);
                                break;
                        }
                    }
                    else
                    {
                        QueryControl.InitializeQueryOptions(true);
                    }

                    // uncheck all buttons
                    ViewFieldsButton.IsChecked = false;
                    QueryOrderByButton.IsChecked = false;
                    QueryWhereButton.IsChecked = false;
                    QueryOptionsButton.IsChecked = false;

                    // select the button
                    switch (this.queryType)
                    {
                        case QueryType.ViewFields:
                            ViewFieldsButton.IsChecked = true;
                            break;
                        case QueryType.OrderBy:
                            QueryOrderByButton.IsChecked = true;
                            break;
                        case QueryType.Where:
                            QueryWhereButton.IsChecked = true;
                            break;
                        case QueryType.QueryOptions:
                            QueryOptionsButton.IsChecked = true;
                            break;
                    }
                }
                else
                {
                    QueryControl.Clear(false);
                }
            }

            CAMLTextBlock.Visibility = System.Windows.Visibility.Visible;
            this.selectedList = App.SelectedListViewModel;
        }

        #endregion

        #region Language buttons
        private void CSharpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.LanguageType = BPoint.SharePoint.Common.Enumerations.LanguageType.CSharp;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
            {
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            }
        }

        #endregion

        #region Code Snippet buttons
        private void CamlCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteButton.Visibility = Visibility.Visible;
            ServerOMButton.IsChecked = false;
            ClientOMForDotnetButton.IsChecked = false;
            ClientOMForEcmaScriptButton.IsChecked = false;
            ClientOMForRestJsonButton.IsChecked = false;
            ClientOMForSilverlightButton.IsChecked = false;
            WebServicesButton.IsChecked = false;

            App.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.CAML;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
            {
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            }
        }

        private void ServerOMCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CamlButton.IsChecked = false;
            ClientOMForDotnetButton.IsChecked = false;
            ClientOMForEcmaScriptButton.IsChecked = false;
            ClientOMForRestJsonButton.IsChecked = false;
            ClientOMForSilverlightButton.IsChecked = false;
            WebServicesButton.IsChecked = false;

            App.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.ServerObjectModel;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            ExecuteButton.Visibility = Visibility.Collapsed;
        }

        private void ClientOMForDotnetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CamlButton.IsChecked = false;
            ServerOMButton.IsChecked = false;
            ClientOMForEcmaScriptButton.IsChecked = false;
            ClientOMForRestJsonButton.IsChecked = false;
            ClientOMForSilverlightButton.IsChecked = false;
            WebServicesButton.IsChecked = false;

            App.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.ClientObjectModelForDotnet;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            ExecuteButton.Visibility = Visibility.Collapsed;
        }

        private void ClientOMForEcmaScriptCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CamlButton.IsChecked = false;
            ServerOMButton.IsChecked = false;
            ClientOMForDotnetButton.IsChecked = false;
            ClientOMForRestJsonButton.IsChecked = false;
            ClientOMForSilverlightButton.IsChecked = false;
            WebServicesButton.IsChecked = false;

            App.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.ClientObjectModelForJavaScript;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            ExecuteButton.Visibility = Visibility.Collapsed;
        }

        private void ClientOMForRestJsonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CamlButton.IsChecked = false;
            ServerOMButton.IsChecked = false;
            ClientOMForDotnetButton.IsChecked = false;
            ClientOMForSilverlightButton.IsChecked = false;
            WebServicesButton.IsChecked = false;

            App.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.ClientObjectModelForRestWithJson;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            ExecuteButton.Visibility = Visibility.Collapsed;
        }

        private void ClientOMForSilverlightCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CamlButton.IsChecked = false;
            ServerOMButton.IsChecked = false;
            ClientOMForDotnetButton.IsChecked = false;
            ClientOMForEcmaScriptButton.IsChecked = false;
            ClientOMForRestJsonButton.IsChecked = false;
            WebServicesButton.IsChecked = false;

            App.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.ClientObjectModelForSilverlight;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            ExecuteButton.Visibility = Visibility.Collapsed;
        }

        private void WebServicesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CamlButton.IsChecked = false;
            ServerOMButton.IsChecked = false;
            ClientOMForDotnetButton.IsChecked = false;
            ClientOMForEcmaScriptButton.IsChecked = false;
            ClientOMForRestJsonButton.IsChecked = false;
            ClientOMForSilverlightButton.IsChecked = false;

            App.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.WebServices;
            if (App.CamlDocument == null)
                CAMLTextBlock.Text = string.Empty;
            else
                CAMLTextBlock.Text = this.CamlWrapper.FormatCamlString(App.SelectedListViewModel.Title, App.CamlDocument, QueryControl.WhereFieldsList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
            ExecuteButton.Visibility = Visibility.Collapsed;
        }

        #endregion

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // take the CAML in the textbox
                if (CAMLTextBlock.Text.Length > 0)
                {
                    XmlDocument doc = new XmlDocument();
                    XmlNode queryNode = doc.CreateElement("CAML");
                    doc.AppendChild(queryNode);
                    queryNode.InnerXml = CAMLTextBlock.Text;
                    DataTable resultTable = this.CamlWrapper.ExecuteQuery(App.SelectedListViewModel.Title, doc, App.QueryOptions, App.QueryType);
                    if (resultTable != null)
                        CAMLGridView.ItemsSource = resultTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("CAML seems not to be well-formed: {0}", ex.Message));
            }
        }

        private void AboutUs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CamlDesigner.Controls.About test = new CamlDesigner.Controls.About(this);
            test.ShowDialog();
            About.IsChecked = false;
        }

        private void AboutTab_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }

    /// <summary>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        /// <summary>
        /// </summary>            
        public int RectangleSize = Marshal.SizeOf(typeof(MONITORINFO));

        /// <summary>
        /// </summary>            
        public RECT Monitor = new RECT();

        /// <summary>
        /// </summary>            
        public RECT rcWork = new RECT();

        /// <summary>
        /// </summary>            
        public int dwFlags = 0;
    }
}