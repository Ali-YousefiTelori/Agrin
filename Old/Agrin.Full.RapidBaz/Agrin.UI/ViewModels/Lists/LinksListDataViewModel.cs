using Agrin.Download.Engine;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.UI.ViewModels.Downloads;
using Agrin.UI.ViewModels.Pages;
using Agrin.UI.ViewModels.Toolbox;
using Agrin.UI.Views.Lists;
using Agrin.UI.Views.Toolbox;
using Agrin.ViewModels.Helper.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Agrin.UI.ViewModels.Lists
{
    public class LinksListDataViewModel : ANotifyPropertyChanged<LinksListData>
    {
        public LinksListDataViewModel()
        {
            This = this;
            if (MainWindow.This == null)
                return;
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            OpenFileLocationCommand = new RelayCommand(OpenFileLocation, CanOpenFile);
            DeleteLinkInfoCommand = new RelayCommand(Toolbox.ToolbarViewModel.DeleteLinks, Toolbox.ToolbarViewModel.CanDeleteLinks);
            SendToGroupInfoCommand = new RelayCommand<GroupInfo>(SendToGroupInfo, CanSendToGroupInfo);
            ChangeUserSavePathCommand = new RelayCommand<LinkInfo>(ChangeUserSavePath, CanChangeUserSavePath);
            LinkInfoSettingCommand = new RelayCommand<LinkInfo>(LinkInfoSetting);
            AddLinkInfoCommand = new RelayCommand(AddLinkInfo, CanAddLinkInfo);
            AddAndPlayLinkInfoCommand = new RelayCommand(AddAndPlayLinkInfo, CanAddAndPlayLinkInfo);
            AddGroupInfoCommand = new RelayCommand(AddGroupInfo);
            EditGroupInfoCommand = new RelayCommand<GroupInfo>(EditGroupInfo, CanEditGroupInfo);
            RenameGroupInfoCommand = new RelayCommand<object>(RenameGroupInfo, CanRenameGroupInfo);
            DeleteGroupInfoCommand = new RelayCommand<object>(DeleteGroupInfo, CanDeleteGroupInfo);

            SearchCommand = new RelayCommand(Search);

            ViewElementInited = () =>
            {
                ResetGridViewGroups(SelectedToolboxGroupGridEnum.Groups);
            };

            ApplicationGroupManager.Current.ChangedGroups = (item) =>
            {
                RefreshGridGrouping(item);
            };


        }

        private bool CanDeleteGroupInfo(object obj)
        {
            return obj != ApplicationGroupManager.Current.NoGroup;
        }

        private void DeleteGroupInfo(object obj)
        {
            if (obj is GroupInfo)
            {
                ApplicationGroupManager.Current.DeleteGroupInfo(obj as GroupInfo);
                foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToArray())
                {
                    if (item.PathInfo.CurrentGroupInfo == obj)
                    {
                        ApplicationGroupManager.Current.SetGroupByLinkInfo(item, null);
                    }
                }
            }
        }

        private bool CanRenameGroupInfo(object obj)
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
                textBox.SelectAll();
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
                        enterAction();
                    };
            }

            visibleData(textBlock, textBox, true);
            textBox.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
        }

        private bool CanEditGroupInfo(GroupInfo obj)
        {
            return obj != ApplicationGroupManager.Current.NoGroup;
        }

        private void EditGroupInfo(GroupInfo obj)
        {
            MenuControlViewModel.EditGroupInfo(obj);
        }

        private void AddGroupInfo()
        {
            MenuControlViewModel.AddGroup();
        }

        public static LinksListDataViewModel This;
        public RelayCommand AddLinkInfoCommand { get; set; }
        public RelayCommand AddAndPlayLinkInfoCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand OpenFileLocationCommand { get; set; }
        public RelayCommand DeleteLinkInfoCommand { get; set; }
        public RelayCommand<GroupInfo> SendToGroupInfoCommand { get; set; }
        public RelayCommand<LinkInfo> ChangeUserSavePathCommand { get; set; }
        public RelayCommand<LinkInfo> LinkInfoSettingCommand { get; set; }

        public RelayCommand AddGroupInfoCommand { get; set; }
        public RelayCommand<GroupInfo> EditGroupInfoCommand { get; set; }
        public RelayCommand<object> RenameGroupInfoCommand { get; set; }
        public RelayCommand<object> DeleteGroupInfoCommand { get; set; }
        public FastCollection<LinkInfo> Items
        {
            get
            {
                return SearchEngine.Items;
            }
        }

        public FastCollection<GroupInfo> GrouInfoes
        {
            get
            {
                return ApplicationGroupManager.Current.GroupInfoes;
            }
        }

        LinkInfo _SelectedItem;
        public LinkInfo SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        string _SeachAddress;

        public string SeachAddress
        {
            get { return _SeachAddress; }
            set { _SeachAddress = value; OnPropertyChanged("SeachAddress"); }
        }

        private void OpenFileLocation()
        {
            try
            {
                Process.Start("explorer.exe", "/select, " + SelectedItem.PathInfo.FullAddressFileName);
            }
            catch
            {

            }
        }

        private void OpenFile()
        {
            try
            {
                Process.Start(SelectedItem.PathInfo.FullAddressFileName);
            }
            catch
            {

            }
        }

        private bool CanOpenFile()
        {
            return SelectedItem != null && SelectedItem.DownloadingProperty.State == ConnectionState.Complete && System.IO.File.Exists(SelectedItem.PathInfo.FullAddressFileName);
        }

        private void SendToGroupInfo(GroupInfo obj)
        {
            foreach (var item in Toolbox.ToolbarViewModel.GetListLinks())
            {
                item.PathInfo.UserGroupInfo = obj;
            }
            Agrin.Download.Data.SerializeData.SaveLinkInfoesToFile();
        }
        private bool CanSendToGroupInfo(GroupInfo obj)
        {
            return Toolbox.ToolbarViewModel.CanDeleteLinks();
        }

        private void ChangeUserSavePath(LinkInfo obj)
        {
            var dialog = new SaveFileDialog() { InitialDirectory = obj.PathInfo.SavePath, FileName = obj.PathInfo.FileName, Filter = "*.*|All Files" };
            if (dialog.ShowDialog().Value)
            {
                obj.PathInfo.UserFileName = Path.GetFileName(dialog.FileName);
                obj.PathInfo.UserSavePath = Path.GetDirectoryName(dialog.FileName);
            }
        }

        private bool CanChangeUserSavePath(LinkInfo obj)
        {
            return obj != null && obj.CanDelete;
        }

        private void LinkInfoSetting(LinkInfo item)
        {
            Toolbox.ToolbarViewModel.ShowSetting(item);
        }

        private void Search()
        {
            SearchEngine.SearchText = SeachAddress;
            SearchEngine.Search();
        }

        private bool CanAddAndPlayLinkInfo()
        {
            Uri uri;
            return Uri.TryCreate(SeachAddress, UriKind.Absolute, out uri);
        }

        private void AddAndPlayLinkInfo()
        {
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(SeachAddress))
                return;
            LinkInfoesDownloadingManagerViewModel.This.AddLinkInfo(SeachAddress, null, null, null, null, true, -1);
            SeachAddress = "";
        }

        private bool CanAddLinkInfo()
        {
            return CanAddAndPlayLinkInfo();
        }

        private void AddLinkInfo()
        {
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(SeachAddress))
                return;
            LinkInfoesDownloadingManagerViewModel.This.AddLinkInfo(SeachAddress, null, null, null, null, false, -1);
            SeachAddress = "";
        }

        public void ResetGridViewGroups(SelectedToolboxGroupGridEnum mode)
        {
            ViewElement.mainDataGrid.Items.GroupDescriptions.Clear();
            if (mode == SelectedToolboxGroupGridEnum.Groups)
                ViewElement.mainDataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription("PathInfo.CurrentGroupInfo"));
            else if (mode == SelectedToolboxGroupGridEnum.Tasks)
                ViewElement.mainDataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription("PathInfo.CurrentQueueInfo"));
        }

        public void RefreshGridGrouping(object item)
        {
            IEditableCollectionView view = ViewElement.mainDataGrid.Items as IEditableCollectionView;
            view.EditItem(item);
            view.CommitEdit();
        }
    }
}
