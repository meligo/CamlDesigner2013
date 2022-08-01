// ----------------------------------------------------------------------
// <copyright file="WhereFieldControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using BPoint.SharePoint.Objects;
    using Microsoft.Windows.Controls;


    /// <summary>
    /// Interaction logic for WhereFieldControl.xaml
    /// </summary>
    public partial class WhereFieldControl : UserControl
    {
        public event WhereFieldEventHandler WhereFieldSelectedEvent;
        public event WhereFieldEventHandler WhereFieldDeletedEvent;

        private QueryControl parentControl;
        private WhereField whereField;
        private bool isTimeControlsAdded;
        private double dateTimeControlHeight = 65.0;
        private string selectedOperator;

        // these controls must be class level variables because they need to be made visible later on
        private CheckBox offsetCheckBox;
        private StackPanel offsetStackPanel;
        private Control valueControl;
        private TextBlock messageControl;

        public WhereField WhereField
        {
            get { return this.whereField; }
            set { this.whereField = value; }
        }

        public WhereFieldControl(WhereField whereField, QueryControl parentControl)
        {
            this.InitializeComponent();
            this.parentControl = parentControl;
            this.whereField = whereField;
            DisplayNameTextBlock.Text = whereField.Field.DisplayName;
            this.DisplayValueControl();
            // adapt the operator control to the selected datatype
            OperatorControl.AdaptContent(whereField.Field.DataType);
        }

        private void DeleteCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WhereFieldDeletedEvent != null)
                WhereFieldDeletedEvent(this.whereField);
        }

        private void DisplayValueControl()
        {
            ValueStackPanel.Children.Clear();
            switch (this.whereField.Field.DataType)
            {
                case "Boolean":
                    CreateControlsForBooleanField();
                    break;
                case "DateTime":
                    CreateControlsForDateTimeField();
                    break;
                case "Lookup":
                    CreateControlsForLookupField();
                    break;
                case "LookupMulti":
                    CreateControlsForLookupField();
                    break;
                case "Choice":
                    CreateControlsForChoiceField(false);
                    break;
                case "MultiChoice":
                    CreateControlsForChoiceField(true);
                    break;
                case "User":
                case "UserMulti":
                    CreateControlsForUserField();
                    break;
                case "TaxonomyFieldType":
                case "TaxonomyFieldTypeMulti":
                    CreateControlsForTaxonomyField();
                    break;
                default:
                    CreateControlsForDefaultBehavior();
                    break;
            }

            // also set the operator
            OperatorControl.SetOperator(whereField.WhereOperator);
        }

        #region Text field methods
        private void CreateControlsForDefaultBehavior()
        {
            TextBox textbox = new TextBox();
            textbox.Width = 180.0;
            textbox.Margin = new Thickness(0, 10, 0, 0);
            textbox.TextChanged += new TextChangedEventHandler(this.textbox_TextChanged);
            // set the value
            if (whereField.Values != null && whereField.Values[0] != null)
                textbox.Text = (string)whereField.Values[0];
            ValueStackPanel.Children.Add(textbox);
            TextBlock messageTextBlock = new TextBlock();
            messageTextBlock.Margin = new Thickness(0, 10, 0, 0);
            messageTextBlock.Text = "Separate values with a + sign, ex: titel 1+title 2+titel 3";
            messageTextBlock.TextWrapping = TextWrapping.Wrap;
            messageTextBlock.Foreground = new SolidColorBrush(UtilityFunctions.GetColorFromHexString("#FF293955"));
            messageTextBlock.Width = 180.0;
            messageTextBlock.Visibility = Visibility.Collapsed;
            ValueStackPanel.Children.Add(messageTextBlock);
            this.valueControl = textbox;
            this.messageControl = messageTextBlock;
        }
        #endregion

        #region Boolean field methods
        private void CreateControlsForBooleanField()
        {
            CheckBox checkbox = new CheckBox();
            checkbox.Margin = new Thickness(0, 10, 0, 0);
            checkbox.Content = "True";
            checkbox.Foreground = new SolidColorBrush(Colors.Ivory);
            checkbox.Background = new SolidColorBrush(Colors.SlateGray);
            checkbox.Click += new RoutedEventHandler(this.checkbox_Click);
            ValueStackPanel.Children.Add(checkbox);
            this.valueControl = checkbox;

            // set the value
            if (whereField.Values != null)
                checkbox.IsChecked = (bool)whereField.Values[0];
            else
                this.AddWhereFieldValue("False");
            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(this.whereField);
        }
        #endregion

        #region DateTime field methods
        private void CreateControlsForDateTimeField()
        {
            // Today checkbox
            RadioButton todayRadioButton = new RadioButton();
            todayRadioButton.Margin = new Thickness(0, 10, 0, 0);
            todayRadioButton.Content = "Today";
            todayRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            todayRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            todayRadioButton.Click += new RoutedEventHandler(this.todayRadioButton_Click);
            ValueStackPanel.Children.Add(todayRadioButton);
            // Generate offset panel
            this.GenerateOffsetPanel();
            // Specific date RadioButton
            RadioButton dateRadioButton = new RadioButton();
            dateRadioButton.Margin = new Thickness(0, 10, 0, 0);
            dateRadioButton.Content = "Specific date";
            dateRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            dateRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            dateRadioButton.Click += new RoutedEventHandler(this.dateRadioButton_Click);
            ValueStackPanel.Children.Add(dateRadioButton);
            //resize control
            this.ResizeControl(this.dateTimeControlHeight);
            // set the value
            if (whereField.Values != null)
            {
                if (whereField.OptionalDateParameter == BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.Today)
                    todayRadioButton.IsChecked = true;
                else if (whereField.OptionalDateParameter == BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.SpecificDate)
                {
                    dateRadioButton.IsChecked = true;
                    CreateCalendarControl();
                }
            }
        }

        private void GenerateOffsetPanel()
        {
            this.offsetCheckBox = new CheckBox();
            this.offsetCheckBox.Margin = new Thickness(20, 10, 0, 0);
            this.offsetCheckBox.Content = " Add an offset to today";
            this.offsetCheckBox.Foreground = new SolidColorBrush(Colors.Ivory);
            this.offsetCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            this.offsetCheckBox.Click += new RoutedEventHandler(this.offsetCheckBox_Click);
            this.offsetCheckBox.Visibility = System.Windows.Visibility.Collapsed;
            ValueStackPanel.Children.Add(this.offsetCheckBox);

            this.offsetStackPanel = new StackPanel();
            this.offsetStackPanel.Margin = new Thickness(30, 10, 0, 0);
            this.offsetStackPanel.Orientation = Orientation.Horizontal;
            this.offsetStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            ValueStackPanel.Children.Add(this.offsetStackPanel);

            ComboBox offsetComboBox = new ComboBox();
            //Populate the combobox
            ComboBoxItem item1 = new ComboBoxItem();
            item1.Content = "+";
            offsetComboBox.Items.Add(item1);
            ComboBoxItem item2 = new ComboBoxItem();
            item2.Content = "-";
            offsetComboBox.Items.Add(item2);
            offsetComboBox.SelectionChanged += new SelectionChangedEventHandler(this.offsetComboBox_SelectionChanged);
            this.offsetStackPanel.Children.Add(offsetComboBox);

            TextBox offsetTextBox = new TextBox();
            offsetTextBox.Margin = new Thickness(10, 10, 0, 0);
            offsetTextBox.Width = 50.0;
            offsetTextBox.TextChanged += new TextChangedEventHandler(this.offsetTextBox_TextChanged);
            this.offsetStackPanel.Children.Add(offsetTextBox);

            TextBlock offsetTextBlock = new TextBlock();
            offsetTextBlock.Margin = new Thickness(0, 10, 0, 0);
            offsetTextBlock.Text = " days";
            offsetTextBlock.Foreground = new SolidColorBrush(Colors.Ivory);
            this.offsetStackPanel.Children.Add(offsetTextBlock);

            if (whereField.IncludeOffset)
            {
                offsetCheckBox.IsChecked = true;
                offsetStackPanel.Visibility = System.Windows.Visibility.Visible;
                this.dateTimeControlHeight += 50.0;
                this.ResizeControl(this.dateTimeControlHeight);

                if (whereField.OffsetValue.Sign == null || whereField.OffsetValue.Sign == "+")
                    offsetComboBox.SelectedIndex = 0;
                else
                    offsetComboBox.SelectedIndex = 1;
                offsetTextBox.Text = whereField.OffsetValue.Value.ToString();
            }
        }

        void CreateCalendarControl()
        {
            Microsoft.Windows.Controls.Calendar calendar = new Microsoft.Windows.Controls.Calendar();
            calendar.Margin = new Thickness(0, 5, 0, 0);
            calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(this.calendar_SelectedDatesChanged);
            ValueStackPanel.Children.Add(calendar);
            this.valueControl = calendar;

            if (whereField.Values != null && whereField.Values[0] is DateTime)
                calendar.SelectedDate = whereField.Values[0] as DateTime?;
            else
                calendar.SelectedDate = DateTime.Today;

            CheckBox includeTimeCheckBox = new CheckBox();
            includeTimeCheckBox.Margin = new Thickness(0, 10, 0, 0);
            includeTimeCheckBox.Content = "Include Time Value";
            includeTimeCheckBox.Foreground = new SolidColorBrush(Colors.SlateGray);
            includeTimeCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            includeTimeCheckBox.Click += new RoutedEventHandler(this.includeTimeCheckBox_Click);
            ValueStackPanel.Children.Add(includeTimeCheckBox);
            this.dateTimeControlHeight += 190.0;
            this.ResizeControl(this.dateTimeControlHeight);

            if (whereField.IncludeTimeValue)
            {
                includeTimeCheckBox.IsChecked = true;
                CreateIncludeTimeControls();
            }
        }

        void CreateIncludeTimeControls()
        {
            this.isTimeControlsAdded = true;
            TextBox timeTextBox = new TextBox();
            timeTextBox.Width = 100.0;
            timeTextBox.Margin = new Thickness(15, 5, 0, 0);
            timeTextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            timeTextBox.TextChanged += new TextChangedEventHandler(this.timeTextBox_TextChanged);
            ValueStackPanel.Children.Add(timeTextBox);

            TextBlock timeLabel = new TextBlock();
            timeLabel.Text = "Format: xx:xx:xx";
            timeLabel.Foreground = new SolidColorBrush(Colors.SlateGray);
            timeLabel.Margin = new Thickness(15, 0, 0, 0);
            ValueStackPanel.Children.Add(timeLabel);
            this.dateTimeControlHeight += 40.0;
            this.ResizeControl(this.dateTimeControlHeight);

            if (whereField.IncludeTimeValue && !string.IsNullOrEmpty(whereField.TimeValue))
                timeTextBox.Text = whereField.TimeValue;
        }

        void todayRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                whereField.AddValue("Today");
                whereField.OptionalDateParameter = BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.Today;

                // display Offset controls
                this.offsetCheckBox.Visibility = System.Windows.Visibility.Visible;
                this.dateTimeControlHeight += 25.0;
                this.ResizeControl(this.dateTimeControlHeight);

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(whereField);
            }
        }

        void offsetCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                whereField.IncludeOffset = (bool)((CheckBox)sender).IsChecked;

                // display Offset panel
                this.offsetStackPanel.Visibility = System.Windows.Visibility.Visible;

                this.dateTimeControlHeight += 50.0;
                this.ResizeControl(this.dateTimeControlHeight);

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(whereField);
            }
        }

        void offsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                if (WhereField.OffsetValue == null)
                    WhereField.OffsetValue = new OffsetValue();
                string sign = ((ComboBoxItem)((ComboBox)sender).SelectedValue).Content.ToString();
                if (!string.IsNullOrEmpty(sign))
                    WhereField.OffsetValue.Sign = sign;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(whereField);
            }
        }

        void offsetTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                string offsetString = ((TextBox)sender).Text;
                int offsetValue = 0;
                int.TryParse(offsetString, out offsetValue);
                if (offsetValue > 0)
                {
                    if (WhereField.OffsetValue == null)
                        WhereField.OffsetValue = new OffsetValue();
                    whereField.OffsetValue.Value = offsetValue;

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent(whereField);
                }
            }
        }

        void dateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Calendar control
            CreateCalendarControl();
        }


        void includeTimeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                whereField.IncludeTimeValue = (bool)((CheckBox)sender).IsChecked;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(whereField);

                if ((bool)((CheckBox)sender).IsChecked && !this.isTimeControlsAdded)
                {
                    CreateIncludeTimeControls();
                }
            }
        }

        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Microsoft.Windows.Controls.Calendar)
            {
                this.AddWhereFieldValue(((Microsoft.Windows.Controls.Calendar)sender).SelectedDate);
                whereField.OptionalDateParameter = BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.SpecificDate;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(whereField);
            }
        }
        void timeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
                whereField.TimeValue = ((TextBox)sender).Text;

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(whereField);
        }

        #endregion

        #region Lookup field methods
        private void CreateControlsForLookupField()
        {
            ListBox lookupListBox = new ListBox();
            if (WhereField.WhereOperator == "In")
                lookupListBox.SelectionMode = SelectionMode.Multiple;
            lookupListBox.Margin = new Thickness(0, 5, 0, 0);
            lookupListBox.Height = 60.0;
            this.PopulateLookupListControl(lookupListBox);
            lookupListBox.SelectionChanged += new SelectionChangedEventHandler(this.listbox_SelectionChanged);
            ValueStackPanel.Children.Add(lookupListBox);
            this.valueControl = lookupListBox;

            CheckBox lookupCheckBox = new CheckBox();
            lookupCheckBox.Margin = new Thickness(0, 10, 0, 0);
            lookupCheckBox.Content = "Query by ID";
            lookupCheckBox.Foreground = new SolidColorBrush(Colors.Ivory);
            lookupCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            lookupCheckBox.Click += new RoutedEventHandler(this.lookupCheckBox_Click);
            if (whereField.Values != null)
                lookupCheckBox.IsChecked = whereField.ByLookupId;
            ValueStackPanel.Children.Add(lookupCheckBox);
            this.ResizeControl(100.0);
        }

        void lookupCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
                this.whereField.ByLookupId = (bool)((CheckBox)sender).IsChecked;

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(whereField);
        }

        private void PopulateLookupListControl(Control listcontrol)
        {
            //WhereField.Field
            if (this.parentControl != null)
            {
                List<LookupValue> lookupValues = this.parentControl.GetLookupValues(((LookupField)whereField.Field).LookupList, ((LookupField)whereField.Field).ShowField, ((LookupField)whereField.Field).LookupWebId);
                if (lookupValues != null)
                {
                    if (listcontrol is ComboBox)
                    {
                        ComboBox combobox = listcontrol as ComboBox;
                        combobox.Items.Clear();
                        combobox.ItemsSource = lookupValues;
                        combobox.DisplayMemberPath = "Value";
                        combobox.SelectedValuePath = "ID";

                    }
                    else if (listcontrol is ListBox)
                    {
                        ListBox listbox = listcontrol as ListBox;
                        listbox.Items.Clear();
                        listbox.ItemsSource = lookupValues;
                        listbox.DisplayMemberPath = "Value";
                        listbox.SelectedValuePath = "ID";
                    }

                    SetValuesForLookupListControl(listcontrol);
                }
            }

        }

        private void SetValuesForLookupListControl(Control listcontrol)
        {
            // set the value
            // if operator differs from in, it will contain only one value
            if (whereField.Values != null)
            {
                foreach (object value in whereField.Values)
                {
                    string valuestring = null;
                    if (value is LookupValue)
                        valuestring = ((LookupValue)value).Value;
                    else
                        valuestring = value.ToString();

                    foreach (var item in ((ListBox)listcontrol).Items)
                    {
                        if (((LookupValue)item).Value == valuestring)
                        {
                            if (listcontrol is ListBox)
                            {
                                if (((ListBox)listcontrol).SelectionMode == SelectionMode.Multiple)
                                    ((ListBox)listcontrol).SelectedItems.Add(item);
                                else
                                    ((ListBox)listcontrol).SelectedItem = item;
                            }
                            else if (listcontrol is ComboBox)
                            {
                                ((ComboBox)listcontrol).SelectedItem = item;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Choice field methods
        private void CreateControlsForChoiceField(bool isMultiSelect)
        {
            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(0, 5, 0, 0);
            listbox.Height = 60.0;
            if (isMultiSelect && WhereField.WhereOperator == "In")
                listbox.SelectionMode = SelectionMode.Multiple;
            this.PopulateChoiceListControl(listbox);
            this.ResizeControl(75.0);
            listbox.SelectionChanged += new SelectionChangedEventHandler(this.multiChoiceListBox_SelectionChanged);
            ValueStackPanel.Children.Add(listbox);
            this.valueControl = listbox;
        }

        void choiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                if (((ListBox)sender).SelectionMode == SelectionMode.Single || ((ListBox)sender).SelectedItems == null)
                {
                    this.AddWhereFieldValue(((ListBox)sender).SelectedItem.ToString());
                }
                else
                {
                    WhereField.Values = null;
                    foreach (object value in ((ListBox)sender).SelectedItems)
                    {
                        this.AddWhereFieldValue(value.ToString());
                    }
                }
            }

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent(this.whereField);
        }

        void multiChoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                if (((ListBox)sender).SelectionMode == SelectionMode.Single || ((ListBox)sender).SelectedItems == null)
                {
                    this.AddWhereFieldValue(((ListBox)sender).SelectedItem.ToString());
                }
                else
                {
                    WhereField.Values = null;
                    foreach (object value in ((ListBox)sender).SelectedItems)
                    {
                        this.AddWhereFieldValue(value.ToString());
                    }
                }
            }

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent(this.whereField);
        }

        private void PopulateChoiceListControl(Control listcontrol)
        {
            if (WhereField.Field is ChoiceField)
            {
                if (listcontrol is ComboBox)
                {
                    ComboBox combobox = listcontrol as ComboBox;
                    combobox.Items.Clear();
                    foreach (string choice in ((ChoiceField)WhereField.Field).Choices)
                    {
                        combobox.Items.Add(choice);
                    }
                }
                else if (listcontrol is ListBox)
                {
                    ListBox listbox = listcontrol as ListBox;
                    listbox.Items.Clear();
                    foreach (string choice in ((ChoiceField)WhereField.Field).Choices)
                    {
                        listbox.Items.Add(choice);
                    }
                }

                SetValuesForChoiceListControl(listcontrol);
            }
        }

        private void SetValuesForChoiceListControl(Control listcontrol)
        {
            // set the value
            // if operator differs from in, it will contain only one value
            if (whereField.Values != null)
            {
                foreach (string value in whereField.Values)
                {
                    foreach (var item in ((ListBox)listcontrol).Items)
                    {
                        if (item == value)
                        {
                            if (listcontrol is ListBox)
                            {
                                if (((ListBox)listcontrol).SelectionMode == SelectionMode.Multiple)
                                    ((ListBox)listcontrol).SelectedItems.Add(item);
                                else
                                    ((ListBox)listcontrol).SelectedItem = item;
                            }
                            else if (listcontrol is ComboBox)
                            {
                                ((ComboBox)listcontrol).SelectedItem = item;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region User field methods
        private void CreateControlsForUserField()
        {
            // Radio button for current user
            RadioButton currentUserRadioButton = new RadioButton();
            currentUserRadioButton.Margin = new Thickness(0, 10, 0, 0);
            currentUserRadioButton.Name = "CurrentUserRadioButton";
            currentUserRadioButton.Content = "Current user";
            currentUserRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            currentUserRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            currentUserRadioButton.Click += new RoutedEventHandler(this.userRadioButton_Click);
            ValueStackPanel.Children.Add(currentUserRadioButton);
            // Radio button for specific user
            RadioButton userRadioButton = new RadioButton();
            userRadioButton.Margin = new Thickness(0, 10, 0, 0);
            userRadioButton.Content = "Specific user";
            userRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            userRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            userRadioButton.Click += new RoutedEventHandler(this.userRadioButton_Click);
            ValueStackPanel.Children.Add(userRadioButton);
            this.ResizeControl(60.0);

            if (whereField.Values != null && whereField.Values[0] != null)
            {
                if (whereField.Values[0].ToString() == "UserID")
                    currentUserRadioButton.IsChecked = true;
                else
                {
                    userRadioButton.IsChecked = true;
                    CreateUserTextBox();
                }
            }
        }

        void userRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                RadioButton radio = (RadioButton)sender;
                if (radio.Name == "CurrentUserRadioButton")
                {
                    this.AddWhereFieldValue("UserID");

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent(whereField);
                }
                else
                {
                    CreateUserTextBox();
                }
            }
        }

        private void CreateUserTextBox()
        {
            TextBox userTextBox = new TextBox();
            userTextBox.Width = 180.0;
            userTextBox.Margin = new Thickness(0, 10, 0, 0);
            userTextBox.TextChanged += new TextChangedEventHandler(this.userTextBox_TextChanged);
            ValueStackPanel.Children.Add(userTextBox);
            this.ResizeControl(90.0);

            if (whereField.Values != null && whereField.Values[0] != null)
            {
                userTextBox.Text = whereField.Values[0].ToString();
            }
        }

        void userTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                if (WhereField.Values == null)
                    WhereField.AddValue(((TextBox)sender).Text);
                else
                    WhereField.Values[0] = ((TextBox)sender).Text;
            }

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(whereField);
        }

        #endregion

        #region Taxonomy field methods
        private void CreateControlsForTaxonomyField()
        {
            StackPanel taxonomyStackPanel = new StackPanel();
            taxonomyStackPanel.Orientation = Orientation.Horizontal;
            taxonomyStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            TextBox taxonomyTextBox = new TextBox();
            taxonomyTextBox.Width = 140.0;
            taxonomyTextBox.Height = 24.0;
            taxonomyTextBox.Margin = new Thickness(0, 10, 0, 0);
            taxonomyTextBox.TextChanged += new TextChangedEventHandler(textbox_TextChanged);
            taxonomyStackPanel.Children.Add(taxonomyTextBox);
            if (App.GeneralInformation.ConnectionType != BPoint.SharePoint.Common.Enumerations.ConnectionType.WebServices)
            {
                Button taxonomyButton = new Button();
                taxonomyButton.Width = 30.0;
                taxonomyButton.Height = 24.0;
                taxonomyButton.Content = "...";
                taxonomyButton.Background = new SolidColorBrush(UtilityFunctions.GetColorFromHexString("#FF4D6082"));
                taxonomyButton.Foreground = new SolidColorBrush(Colors.Ivory);
                taxonomyButton.Background = new SolidColorBrush(Colors.SlateGray);
                taxonomyButton.Margin = new Thickness(5, 10, 0, 0);
                taxonomyButton.Click += new RoutedEventHandler(taxonomyButton_Click);
                taxonomyStackPanel.Children.Add(taxonomyButton);
            }
            ValueStackPanel.Children.Add(taxonomyStackPanel);
            TextBlock tmessageTextBlock = new TextBlock();
            tmessageTextBlock.Margin = new Thickness(0, 10, 0, 0);
            tmessageTextBlock.Text = "Separate values with a + sign, ex: titel 1+title 2+titel 3";
            tmessageTextBlock.TextWrapping = TextWrapping.Wrap;
            tmessageTextBlock.Foreground = new SolidColorBrush(UtilityFunctions.GetColorFromHexString("#FF293955"));
            tmessageTextBlock.Width = 180.0;
            tmessageTextBlock.Visibility = Visibility.Collapsed;
            ValueStackPanel.Children.Add(tmessageTextBlock);
            valueControl = taxonomyTextBox;
            messageControl = tmessageTextBlock;
        }

        void taxonomyButton_Click(object sender, RoutedEventArgs e)
        {
            // get the taxonomy
            List<TaxonomyValue> values = this.parentControl.GetTaxonomyValues(((TaxonomyField)whereField.Field).TermStoreId, ((TaxonomyField)whereField.Field).TermSetId);
            if (values != null)
            {
                bool multiSelect = false;
                if (whereField.Field.DataType == "TaxonomyFieldTypeMulti")
                    multiSelect = true;
                TaxonomyWindow taxonomyWindow = new TaxonomyWindow(values, multiSelect, whereField.WhereOperator);
                Nullable<bool> dialogResult = taxonomyWindow.ShowDialog();
                if (!string.IsNullOrEmpty(taxonomyWindow.SelectedValues))
                {
                    TextBox textbox = valueControl as TextBox;

                    if (((TaxonomyField)whereField.Field).MultiSelect)
                    {
                        if (textbox != null)
                            textbox.Text = taxonomyWindow.SelectedValues;
                        AddWhereFieldValue(taxonomyWindow.SelectedValues);
                    }
                    else
                    {
                        if (taxonomyWindow.SelectedValues.Contains(";"))
                        {
                            // in that case only take the first selected value
                            AddWhereFieldValue(taxonomyWindow.SelectedValues.Substring(0, taxonomyWindow.SelectedValues.IndexOf(';')));
                            if (textbox != null)
                                textbox.Text = taxonomyWindow.SelectedValues.Substring(0, taxonomyWindow.SelectedValues.IndexOf(';'));
                        }
                        else
                        {
                            if (textbox != null)
                                textbox.Text = taxonomyWindow.SelectedValues;
                            AddWhereFieldValue(taxonomyWindow.SelectedValues);
                        }
                    }

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent(whereField);
                }
            }
            else
            {
                System.Windows.MessageBox.Show(string.Format("No terms found for managed metadata field {0} with group {1} and term set id {2}",
                    whereField.Field.DisplayName, ((TaxonomyField)whereField.Field).TermStoreId, ((TaxonomyField)whereField.Field).TermSetId));
            }
        }
        #endregion


        #region Events that cause the CAML Query to fire
        void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                if (WhereField.Values == null)
                    WhereField.AddValue(((TextBox)sender).Text);
                else
                    WhereField.Values[0] = ((TextBox)sender).Text;
            }

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent(this.whereField);
        }

        void checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                this.AddWhereFieldValue(((CheckBox)sender).IsChecked);

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent(this.whereField);
        }

        void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // first check which operator is used
            object selectedValue = null;
            if (sender is ListBox)
            {
                if (((ListBox)sender).SelectionMode == SelectionMode.Single || ((ListBox)sender).SelectedItems == null)
                {
                    selectedValue = (LookupValue)((ListBox)sender).SelectedItem;
                    this.AddWhereFieldValue(selectedValue);
                }
                else
                {
                    WhereField.Values = null;
                    foreach (object value in ((ListBox)sender).SelectedItems)
                    {
                        this.AddWhereFieldValue(value);
                    }
                }
            }
            else if (sender is ComboBox)
            {
                selectedValue = (LookupValue)((ComboBox)sender).SelectedItem;
                this.AddWhereFieldValue(selectedValue);
            }

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(whereField);
        }

        #endregion

        private void AddWhereFieldValue(object selectedValue)
        {
            if (selectedValue != null)
            {
                if (WhereField.WhereOperator == "In")
                    WhereField.AddValue(selectedValue);
                else
                {
                    if (WhereField.Values == null)
                        WhereField.AddValue(selectedValue);
                    else
                        WhereField.Values[0] = selectedValue;
                }
            }
        }

        private void ResizeControl(double height)
        {
            if (height >= 20)
            {
                this.Height = height;
                OverlayRectangle.Height = height;
            }
        }


        private void OperatorControl_FieldOperatorEvent(string fieldOperator)
        {
            if (!string.IsNullOrEmpty(fieldOperator))
            {
                // set the Visibility of the Value StackPanel
                if (fieldOperator == "IsNull" || fieldOperator == "IsNotNull")
                    ValueStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                else
                    ValueStackPanel.Visibility = System.Windows.Visibility.Visible;

                // set the SelectionMode for list boxes
                if (fieldOperator == "In")
                {
                    // TODO: what with other controls?
                    if (valueControl != null)
                    {
                        if (valueControl is ListBox)
                            ((ListBox)valueControl).SelectionMode = SelectionMode.Multiple;

                        // if a ; separated string can be found in the value control, change it into a + sign
                        if (valueControl is TextBox)
                        {
                            ((TextBox)valueControl).Text = ((TextBox)valueControl).Text.Replace(';', '+');
                        }
                    }

                    if (this.messageControl != null)
                    {
                        this.messageControl.Visibility = Visibility.Visible;
                        this.ResizeControl(this.Height + 20.0);
                    }
                }
                else
                {
                    // TODO: what with other controls?
                    if (this.valueControl is ListBox)
                        ((ListBox)this.valueControl).SelectionMode = SelectionMode.Single;
                    if (this.messageControl != null && whereField.WhereOperator == "In")
                    {
                        // otherwise the box is made less heigh, even if the previous where operator was not "In"
                        (this.messageControl).Visibility = Visibility.Collapsed;
                        this.ResizeControl(this.Height - 20.0);
                    }
                }

                // Set the And or Or operator
                if (fieldOperator == "And" || fieldOperator == "Or")
                    whereField.AndOrOperator = fieldOperator;
                else
                    whereField.WhereOperator = fieldOperator;

                if (WhereField.Values != null || fieldOperator == "IsNull" || fieldOperator == "IsNotNull")
                {
                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent(whereField);
                }
            }
        }
    }
}
