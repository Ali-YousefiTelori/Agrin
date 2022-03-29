using Agrin.BaseViewModels.Lists;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.IO.Helper;
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
            ChangeLinkSaveLocationCommand = new RelayCommand(ChangeLinkSaveLocation, CanChangeSelectedLinksSaveLocationCommand);
            PlayLinkInfoCommand = new RelayCommand(PlayLinkInfo, CanPlayLinkInfo);
            PlayForceLinkInfoCommand = new RelayCommand(PlayForceLinkInfo, CanPlayLinkInfo);
            StopLinkInfoCommand = new RelayCommand(StopLinkInfo, CanStopLinkInfo);

            CopyHeadersCommand = new RelayCommand(CopyHeaders, CanCopyHeaders);
            PasteHeadersCommand = new RelayCommand(PasteHeaders, CanPasteHeaders);

            PlayLinksCommand = new RelayCommand(PlayLinks, CanPlayLinks);
            StopLinksCommand = new RelayCommand(StopLinks, CanStopLinks);
            DeleteLinksCommand = new RelayCommand(DeleteLinks, CanDeleteLinks);
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
            PasteLinkAddressCommand = new RelayCommand(PasteLinkAddress);
            CreateReportLinkCommand = new RelayCommand(CreateReportLink);
            RepairLinkCommand = new RelayCommand(RepairLink);
            ReconnectCommand = new RelayCommand(ReconnectSelectedLinks, CanReconnectSelectedLinks);

            AddGroupInfoCommand = new RelayCommand(AddGroupInfo);
            //EditGroupInfoCommand = new RelayCommand<GroupInfo>(EditGroupInfo, CanEditGroupInfo);
            RenameGroupInfoCommand = new RelayCommand<object>(RenameGroupInfo, CanRenameGroupInfo);
            DeleteGroupInfoCommand = new RelayCommand<object>(DeleteGroupInfo, CanDeleteGroupInfo);
        }

        private bool CanPasteHeaders()
        {
            try
            {
                if (GetSelectedItems().Count() == 0)
                    return false;
                string text = Clipboard.GetText(TextDataFormat.UnicodeText);
                return text != null && text.Contains("AgrinHeaders");
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool CanCopyHeaders()
        {
            return GetSelectedItems().Count() == 1 && SelectedItem?.DownloadingProperty?.CustomHeaders != null && SelectedItem?.DownloadingProperty?.CustomHeaders?.Count > 0;
        }

        private void PasteHeaders()
        {
            try
            {
                string text = Clipboard.GetText(TextDataFormat.UnicodeText);
                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach (string header in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (header == "AgrinHeaders")
                        continue;
                    string[] keyValue = header.Split(':');
                    headers.Add(keyValue[0], keyValue[1]);
                }

                foreach (LinkInfo link in GetSelectedItems())
                {
                    if (link.DownloadingProperty.CustomHeaders == null)
                        link.DownloadingProperty.CustomHeaders = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        link.DownloadingProperty.CustomHeaders.TryAdd(header.Key, header.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void CopyHeaders()
        {
            StringBuilder headers = new StringBuilder();
            headers.AppendLine("AgrinHeaders");
            foreach (KeyValuePair<string, string> item in SelectedItem?.DownloadingProperty?.CustomHeaders)
            {
                headers.AppendLine(item.Key + ":" + item.Value);
            }
            Clipboard.SetText(headers.ToString(), TextDataFormat.UnicodeText);
        }

        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand OpenFileLocationCommand { get; set; }
        public RelayCommand ChangeLinkSaveLocationCommand { get; set; }

        public RelayCommand PlayLinkInfoCommand { get; set; }
        public RelayCommand PlayForceLinkInfoCommand { get; set; }
        public RelayCommand StopLinkInfoCommand { get; set; }
        public RelayCommand CopyHeadersCommand { get; set; }
        public RelayCommand PasteHeadersCommand { get; set; }
        public RelayCommand AddLinkCommand { get; set; }
        public RelayCommand PlayLinksCommand { get; set; }
        public RelayCommand StopLinksCommand { get; set; }
        public RelayCommand DeleteLinksCommand { get; set; }
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
        public RelayCommand PasteLinkAddressCommand { get; set; }
        public RelayCommand CreateReportLinkCommand { get; set; }
        public RelayCommand RepairLinkCommand { get; set; }

        public RelayCommand ReconnectCommand { get; set; }

        public RelayCommand AddGroupInfoCommand { get; set; }
        //public RelayCommand<GroupInfo> EditGroupInfoCommand { get; set; }
        public RelayCommand<object> RenameGroupInfoCommand { get; set; }
        public RelayCommand<object> DeleteGroupInfoCommand { get; set; }

        private RelayCommand _MessageCommand;
        public RelayCommand MessageCommand
        {
            get { return _MessageCommand; }
            set { _MessageCommand = value; OnPropertyChanged("MessageCommand"); }
        }

        private bool _IsCancelButton = false;

        public bool IsCancelButton
        {
            get { return _IsCancelButton; }
            set { _IsCancelButton = value; OnPropertyChanged("IsCancelButton"); }
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

        private bool _IsShowMessage;

        public bool IsShowMessage
        {
            get { return _IsShowMessage; }
            set { _IsShowMessage = value; OnPropertyChanged("IsShowMessage"); }
        }

        public override void DeleteLinks()
        {
            IsCancelButton = true;
            MessageCommand = new RelayCommand(() =>
            {
                base.DeleteLinks();
                IsShowMessage = false;
            });
            MessageTitle = "حذف لینک ها";
            Message = "به تعداد (" + GetSelectedItems().Count() + ") لینک انتخاب شده است.آیا میخواهید لینک های انتخاب شده را حذف کنید؟";
            IsShowMessage = true;
        }

        public virtual void AddLink()
        {

        }

        public void PlayForceLinkInfo()
        {
            if (!LinksBaseViewModel.CanPlayForce(SelectedItem))
            {
                IsCancelButton = false;
                MessageCommand = new RelayCommand(() =>
                {
                    IsShowMessage = false;
                });
                MessageTitle = "شروع لینک دارای مشکل...";
                Message = "برای شروع به این روش باید حداقل 95 درصد فایل شما دانلود شده باشد.";
                IsShowMessage = true;
                return;
            }
            PlayForceLinkInfo(SelectedItem);
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
            StackPanel stack = obj as StackPanel;
            Initialize(stack);
        }

        public void PasteLinkAddress()
        {
            string link = Clipboard.GetText();
            if (Uri.TryCreate(link, UriKind.Absolute, out Uri uri))
            {
                foreach (LinkInfo item in GetSelectedItems())
                {
                    item.PathInfo.Address = link;
                    item.Management.MultiLinks.Clear();
                    item.Management.MultiLinks.Add(new Download.Web.Link.MultiLinkAddress() { Address = link, IsApplicationAdded = true, IsSelected = true });
                }
            }
        }

        private List<StackPanel> stacks = new List<StackPanel>();

        private void Initialize(StackPanel stackPanel)
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
                //System.Diagnostics.Process.Start("explorer.exe", "/select, " + linkInfo.PathInfo.FullAddressFileName);
                System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + linkInfo.PathInfo.FullAddressFileName + "\"");

            }
            catch
            {

            }
        }

        public void ChangeLinkSaveLocation()
        {
            IEnumerable<LinkInfo> items = GetSelectedItems();

            if (items == null || items.Count() == 1)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { CheckPathExists = true, InitialDirectory = SelectedItem.PathInfo.SavePath, FileName = SelectedItem.PathInfo.FileName };
                if (saveFileDialog.ShowDialog().Value)
                {
                    string path = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                    string fileName = System.IO.Path.GetFileName(saveFileDialog.FileName);
                    if (!MPath.EqualPath(path, SelectedItem.PathInfo.SavePath))
                        SelectedItem.PathInfo.UserSavePath = path;

                    if (fileName != SelectedItem.PathInfo.FileName)
                        SelectedItem.PathInfo.UserFileName = fileName;
                    SelectedItem.SaveThisLink();
                }
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog saveFileDialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = SelectedItem.PathInfo.SavePath };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (LinkInfo item in items)
                    {
                        item.PathInfo.UserSavePath = saveFileDialog.SelectedPath;
                        item.SaveThisLink();
                    }
                }
            }
        }
    }
}
