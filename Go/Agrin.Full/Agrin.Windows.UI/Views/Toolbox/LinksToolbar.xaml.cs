using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.Toolbox
{
    /// <summary>
    /// Interaction logic for LinksToolbar.xaml
    /// </summary>
    public partial class LinksToolbar : UserControl
    {
        public LinksToolbar()
        {
            InitializeComponent();
        }
        
        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty DownloadCommandProperty = DependencyProperty.Register("DownloadCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty PlayCommandProperty = DependencyProperty.Register("PlayCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty StopCommandProperty = DependencyProperty.Register("StopCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty SettingCommandProperty = DependencyProperty.Register("SettingCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty AddTimeTaskCommandProperty = DependencyProperty.Register("AddTimeTaskCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty AddStopTimeTaskCommandProperty = DependencyProperty.Register("AddStopTimeTaskCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty DeleteTimesCommandProperty = DependencyProperty.Register("DeleteTimesCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty MoveDownCommandProperty = DependencyProperty.Register("MoveDownCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty MoveUpCommandProperty = DependencyProperty.Register("MoveUpCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty PopupShowSettingCommandProperty = DependencyProperty.Register("PopupShowSettingCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty PopupSettingSaveCommandProperty = DependencyProperty.Register("PopupSettingSaveCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty PlayTaskCommandProperty = DependencyProperty.Register("PlayTaskCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty StopTaskCommandProperty = DependencyProperty.Register("StopTaskCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());
        public static readonly DependencyProperty ShowListCommandProperty = DependencyProperty.Register("ShowListCommand", typeof(ICommand), typeof(LinksToolbar), new PropertyMetadata());

        public static readonly DependencyProperty AddTimeMinutesProperty = DependencyProperty.Register("AddTimeMinutes", typeof(int), typeof(LinksToolbar), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AddTimeHoursProperty = DependencyProperty.Register("AddTimeHours", typeof(int), typeof(LinksToolbar), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty StopTimeMinutesProperty = DependencyProperty.Register("StopTimeMinutes", typeof(int), typeof(LinksToolbar), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty StopTimeHoursProperty = DependencyProperty.Register("StopTimeHours", typeof(int), typeof(LinksToolbar), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty StopTimeForMinutesProperty = DependencyProperty.Register("StopTimeForMinutes", typeof(int), typeof(LinksToolbar), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty StartNowProperty = DependencyProperty.Register("StartNow", typeof(bool), typeof(LinksToolbar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsStopForMinutesProperty = DependencyProperty.Register("IsStopForMinutes", typeof(bool), typeof(LinksToolbar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty PopupSettingContentProperty = DependencyProperty.Register("PopupSettingContent", typeof(object), typeof(LinksToolbar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty GripVisibilityProperty = DependencyProperty.Register("GripVisibility", typeof(Visibility), typeof(LinksToolbar), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ICommand TestCommand { get { return new RelayCommand((() => { })); } set { } }
        
        public ICommand AddCommand { get { return (ICommand)GetValue(AddCommandProperty); } set { SetValue(AddCommandProperty, value); } }
        public ICommand DownloadCommand { get { return (ICommand)GetValue(DownloadCommandProperty); } set { SetValue(DownloadCommandProperty, value); } }
        public ICommand PlayCommand { get { return (ICommand)GetValue(PlayCommandProperty); } set { SetValue(PlayCommandProperty, value); } }
        public ICommand StopCommand { get { return (ICommand)GetValue(StopCommandProperty); } set { SetValue(StopCommandProperty, value); } }
        public ICommand DeleteCommand { get { return (ICommand)GetValue(DeleteCommandProperty); } set { SetValue(DeleteCommandProperty, value); } }
        public ICommand SettingCommand { get { return (ICommand)GetValue(SettingCommandProperty); } set { SetValue(SettingCommandProperty, value); } }
        public ICommand RefreshCommand { get { return (ICommand)GetValue(RefreshCommandProperty); } set { SetValue(RefreshCommandProperty, value); } }
        public ICommand AddTimeTaskCommand { get { return (ICommand)GetValue(AddTimeTaskCommandProperty); } set { SetValue(AddTimeTaskCommandProperty, value); } }
        public ICommand AddStopTimeTaskCommand { get { return (ICommand)GetValue(AddStopTimeTaskCommandProperty); } set { SetValue(AddStopTimeTaskCommandProperty, value); } }
        public ICommand DeleteTimesCommand { get { return (ICommand)GetValue(DeleteTimesCommandProperty); } set { SetValue(DeleteTimesCommandProperty, value); } }
        public ICommand MoveDownCommand { get { return (ICommand)GetValue(MoveDownCommandProperty); } set { SetValue(MoveDownCommandProperty, value); } }
        public ICommand MoveUpCommand { get { return (ICommand)GetValue(MoveUpCommandProperty); } set { SetValue(MoveUpCommandProperty, value); } }
        public ICommand PopupShowSettingCommand { get { return (ICommand)GetValue(PopupShowSettingCommandProperty); } set { SetValue(PopupShowSettingCommandProperty, value); } }
        public ICommand PopupSettingSaveCommand { get { return (ICommand)GetValue(PopupSettingSaveCommandProperty); } set { SetValue(PopupSettingSaveCommandProperty, value); } }
        public ICommand PlayTaskCommand { get { return (ICommand)GetValue(PlayTaskCommandProperty); } set { SetValue(PlayTaskCommandProperty, value); } }
        public ICommand StopTaskCommand { get { return (ICommand)GetValue(StopTaskCommandProperty); } set { SetValue(StopTaskCommandProperty, value); } }
        public ICommand ShowListCommand { get { return (ICommand)GetValue(ShowListCommandProperty); } set { SetValue(ShowListCommandProperty, value); } }


        public int AddTimeMinutes { get { return (int)GetValue(AddTimeMinutesProperty); } set { SetValue(AddTimeMinutesProperty, value); } }
        public int AddTimeHours { get { return (int)GetValue(AddTimeHoursProperty); } set { SetValue(AddTimeHoursProperty, value); } }
        public int StopTimeMinutes { get { return (int)GetValue(StopTimeMinutesProperty); } set { SetValue(StopTimeMinutesProperty, value); } }
        public int StopTimeHours { get { return (int)GetValue(StopTimeHoursProperty); } set { SetValue(StopTimeHoursProperty, value); } }
        public int StopTimeForMinutes { get { return (int)GetValue(StopTimeForMinutesProperty); } set { SetValue(StopTimeForMinutesProperty, value); } }
        public bool StartNow { get { return (bool)GetValue(StartNowProperty); } set { SetValue(StartNowProperty, value); } }
        public bool IsStopForMinutes { get { return (bool)GetValue(IsStopForMinutesProperty); } set { SetValue(IsStopForMinutesProperty, value); } }
        public object PopupSettingContent { get { return GetValue(PopupSettingContentProperty); } set { SetValue(PopupSettingContentProperty, value); } }
        public Visibility GripVisibility { get { return (Visibility)GetValue(GripVisibilityProperty); } set { SetValue(GripVisibilityProperty, value); } }



        bool _isTimeTaskVisibility = false;

        public bool IsTimeTaskVisibility
        {
            get { return _isTimeTaskVisibility; }
            set
            {
                _isTimeTaskVisibility = value;
                btnTimeStopTask.Visibility = btnTimeTask.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
