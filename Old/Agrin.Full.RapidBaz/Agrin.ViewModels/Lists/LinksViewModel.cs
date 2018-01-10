using Agrin.BaseViewModels.Lists;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.ViewModels.Helper.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Agrin.ViewModels.Lists
{
    public class LinksViewModel : LinksBaseViewModel
    {
        public LinksViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            OpenFileLocationCommand = new RelayCommand(OpenFileLocation, CanOpenFile);
            OpenFileItemCommand = new RelayCommand<LinkInfo>(OpenFileItem, CanOpenFileItem);
            OpenFileLocationItemCommand = new RelayCommand<LinkInfo>(OpenFileLocationItem, CanOpenFileLocationItem);
            PlayLinkInfoCommand = new RelayCommand(PlayLinkInfo, CanPlayLinkInfo);
            StopLinkInfoCommand = new RelayCommand(StopLinkInfo, CanStopLinkInfo);
            PlayLinksCommand = new RelayCommand(PlayLinks, CanPlayLinks);
            StopLinksCommand = new RelayCommand(StopLinks, CanStopLinks);
            DeleteLinksCommand = new RelayCommand(DeleteLinks, CanDeleteLinks);
            DeleteLinkCommand = new RelayCommand<LinkInfo>(DeleteLink,CanDeleteLink);
            SettingCommand = new RelayCommand(SettingLinks);
            AddLinkCommand = new RelayCommand(AddLink);
            AddTimeTaskCommand = new RelayCommand(AddTimeTask);
            AddStopTimeTaskCommand = new RelayCommand(AddStopTimeTask);
            RemoveAllStartTimeTaskCommand = new RelayCommand<LinkInfo>(RemoveAllStartTimeTask);
            RemoveAllStopTimeTaskCommand = new RelayCommand<LinkInfo>(RemoveAllStopTimeTask);
            DeleteTimesCommand = new RelayCommand(DeleteSelectedLinksTimes, CanDeleteSelectedLinksTimes);
            MoveDownFromTaskCommand = new RelayCommand(MoveDownFromTask, CanDeleteSelectedLinksTimes);
            MoveUpFromTaskCommand = new RelayCommand(MoveUpFromTask, CanDeleteSelectedLinksTimes);
            PlayTaskCommand = new RelayCommand(PlaySelectionTask, CanPlaySelectionTask);
            StopTaskCommand = new RelayCommand(StopSelectionTask, CanStopSelectionTask);
            CopyLinkLocationCommand = new RelayCommand(CopyLinkLocationOK);
            CreateReportLinkCommand = new RelayCommand(CreateReportLink);
            ReconnectCommand = new RelayCommand(ReconnectSelectedLinks, CanReconnectSelectedLinks);

            AddGroupInfoCommand = new RelayCommand(AddGroupInfo);
            //EditGroupInfoCommand = new RelayCommand<GroupInfo>(EditGroupInfo, CanEditGroupInfo);
            RenameGroupInfoCommand = new RelayCommand<object>(RenameGroupInfo, CanRenameGroupInfo);
            DeleteGroupInfoCommand = new RelayCommand<object>(DeleteGroupInfo, CanDeleteGroupInfo);
        }

        

        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand OpenFileLocationCommand { get; set; }
        public RelayCommand<LinkInfo> OpenFileItemCommand { get; set; }
        public RelayCommand<LinkInfo> OpenFileLocationItemCommand { get; set; }
        public RelayCommand PlayLinkInfoCommand { get; set; }
        public RelayCommand StopLinkInfoCommand { get; set; }
        public RelayCommand AddLinkCommand { get; set; }
        public RelayCommand PlayLinksCommand { get; set; }
        public RelayCommand StopLinksCommand { get; set; }
        public RelayCommand DeleteLinksCommand { get; set; }
        public RelayCommand<LinkInfo> DeleteLinkCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }
        public RelayCommand AddTimeTaskCommand { get; set; }
        public RelayCommand<LinkInfo> RemoveAllStartTimeTaskCommand { get; set; }
        public RelayCommand<LinkInfo> RemoveAllStopTimeTaskCommand { get; set; }
        public RelayCommand DeleteTimesCommand { get; set; }
        public RelayCommand AddStopTimeTaskCommand { get; set; }
        public RelayCommand MoveDownFromTaskCommand { get; set; }
        public RelayCommand MoveUpFromTaskCommand { get; set; }
        public RelayCommand PlayTaskCommand { get; set; }
        public RelayCommand StopTaskCommand { get; set; }
        public RelayCommand CopyLinkLocationCommand { get; set; }
        public RelayCommand CreateReportLinkCommand { get; set; }
        public RelayCommand ReconnectCommand { get; set; }

        public RelayCommand AddGroupInfoCommand { get; set; }
        //public RelayCommand<GroupInfo> EditGroupInfoCommand { get; set; }
        public RelayCommand<object> RenameGroupInfoCommand { get; set; }
        public RelayCommand<object> DeleteGroupInfoCommand { get; set; }
        
        RelayCommand _MessageCommand;
        public RelayCommand MessageCommand
        {
            get { return _MessageCommand; }
            set { _MessageCommand = value; OnPropertyChanged("MessageCommand"); }
        }

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; OnPropertyChanged("Message"); }
        }

        private string _MessageTitle;

        public string MessageTitle
        {
            get { return _MessageTitle; }
            set { _MessageTitle = value; OnPropertyChanged("MessageTitle"); }
        }

        bool _IsShowMessage;

        public bool IsShowMessage
        {
            get { return _IsShowMessage; }
            set { _IsShowMessage = value; OnPropertyChanged("IsShowMessage"); }
        }

        public override void DeleteLinks()
        {
            MessageCommand = new RelayCommand(() =>
            {
                base.DeleteLinks();
                IsShowMessage = false;
                IsDeleteFromDisk = false;
            });
            MessageTitle = "حذف لینک ها";
            Message = "به تعداد (" + GetSelectedItems().Count() + ") لینک انتخاب شده است.آیا میخواهید لینک های انتخاب شده را حذف کنید؟";
            IsShowMessage = true;
        }


        public override void DeleteLink(LinkInfo link)
        {
            MessageCommand = new RelayCommand(() =>
            {
                base.DeleteLink(link);
                IsShowMessage = false;
                IsDeleteFromDisk = false;
            });
            MessageTitle = "حذف لینک";
            Message = "آیا میخواهید لینک مورد نظر را حذف کنید؟";
            IsShowMessage = true;
        }

        public virtual void AddLink()
        {

        }

        public bool CanRenameGroupInfo(object obj)
        {
            if (obj == null)
                return false;
            obj = (obj as System.Windows.FrameworkElement).Tag;
            return obj != ApplicationGroupManager.Current.NoGroup;
        }

        private void RenameGroupInfo(object obj)
        {
            var stack = obj as StackPanel;
            Initialize(stack);
        }

        List<StackPanel> stacks = new List<StackPanel>();
        void Initialize(StackPanel stackPanel)
        {
            Action<TextBlock, TextBox, bool> visibleData = (txt, tBox, visible) =>
            {
                txt.Visibility = visible ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                tBox.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            };
            TextBlock textBlock = stackPanel.Children[0] as TextBlock;
            TextBox textBox = stackPanel.Children[1] as TextBox;
            GroupInfo group = stackPanel.Tag as GroupInfo;
            textBox.Text = group.Name;
            if (!stacks.Contains(stackPanel))
            {
                stacks.Add(stackPanel);
                Action enterAction = () =>
                {
                    if (!String.IsNullOrWhiteSpace(textBox.Text))
                    {
                        group.Name = textBox.Text;
                        visibleData(textBlock, textBox, false);
                        ApplicationGroupManager.Current.SaveEditGroup(group);
                    }
                };
                textBox.KeyDown += (s, e) =>
                {
                    if (e.Key == System.Windows.Input.Key.Escape)
                        visibleData(textBlock, textBox, false);
                    else if (e.Key == System.Windows.Input.Key.Enter)
                    {
                        enterAction();
                    }
                };

                textBox.LostFocus += (s, e) =>
                {
                    if (textBox.Visibility != Visibility.Collapsed)
                        enterAction();
                };
            }

            visibleData(textBlock, textBox, true);
            textBox.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            textBox.SelectAll();
        }
        private void CreateReportLink()
        {
            SaveFileDialog save = new SaveFileDialog() { FileName = "ReportLink.agn" };
            if (save.ShowDialog().Value)
            {
                ReportLink(save.FileName);
            }
        }

        public void CopyLinkLocationOK()
        {
            string links = base.CopyLinkLocation();
            Clipboard.SetText(links);
        }

        public virtual void ShowGroupLinks(bool value)
        {

        }

        public bool CanOpenFileLocationItem(LinkInfo item)
        {
            return CanOpenFile(item);
        }

        public void OpenFileLocationItem(LinkInfo item)
        {
            OpenFileLocationStatic(item);
        }

        public bool CanOpenFileItem(LinkInfo item)
        {
            return CanOpenFile(item);
        }

        public void OpenFileItem(LinkInfo item)
        {
            OpenFileStatic(item);
        }

        public override void OpenFile()
        {
            OpenFileStatic(SelectedItem);
        }

        public override void OpenFileLocation()
        {
            OpenFileLocationStatic(SelectedItem);
        }

        public static void OpenFileStatic(LinkInfo linkInfo)
        {
            try
            {
                System.Diagnostics.Process.Start(linkInfo.PathInfo.FullAddressFileName);
            }
            catch
            {

            }
        }

        public static void OpenFileLocationStatic(LinkInfo linkInfo)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + linkInfo.PathInfo.FullAddressFileName);

            }
            catch
            {

            }
        }
    }
}
