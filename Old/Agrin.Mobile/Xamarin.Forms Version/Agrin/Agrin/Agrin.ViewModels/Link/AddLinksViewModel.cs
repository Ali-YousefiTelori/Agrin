using Agrin.BaseViewModels.Link;
using Agrin.Download.Data.Settings;
using Agrin.Download.Helper;
using Agrin.Download.Manager;
using Agrin.ViewModels.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agrin.ViewModels.Link
{
    public class AddLinksViewModel : AddLinksBaseViewModel
    {
        public AddLinksViewModel()
        {
            OnPropertyChangedAction = (name) =>
            {
                if (name == "UriAddress" || name == "IsGroupList" || name == "IsBusy")
                {
                    AddLinkCommand.ChangeCanExecute();
                    AddLinkAndPlayCommand.ChangeCanExecute();
                }
                else if (name == "SharingLinksSelectedIndex")
                {
                    SelectQualityCommand.ChangeCanExecute();
                }
            };

            SharingLinks.CollectionChanged += (s, e) =>
            {
                SelectQualityCommand.ChangeCanExecute();
            };

            AddLinkAndPlayCommand = new RelayCommand(AddLinkAndPlay, CanAddLink);
            AddLinkCommand = new RelayCommand(AddLink, CanAddLink);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            RefreshSharingLinksCommand = new RelayCommand(RefreshSharingLinks);
            BackCommand = new RelayCommand(BackItem);
            CancelCommand = new RelayCommand(Cancel);
            AddFromClipboardCommand = new RelayCommand(AddFromClipboard);
            ShowGroupLinksCommand = new RelayCommand(ShowGroupList);
            AddGroupLinksCommand = new RelayCommand(AddGroupLinks);
            RemoveLinkFromGroupListCommand = new RelayCommand<string>(RemoveLinkFromGroupList);
            SelectQualityCommand = new RelayCommand(SelectQuality, CanSelectQuality);
            PropertyChanged += AddLinksViewModel_PropertyChanged;
        }

        private void Cancel()
        {
            ViewsUtility.RemoveCurrentPage();
        }

        private void AddLinksViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedGroup")
            {
                if (SelectedGroup == null || SelectedGroup.Name == ApplicationGroupManager.Current.NoGroup.Name)
                {
                    SelectedGroupIndex = 0;
                }
                else
                {
                    SelectedGroupIndex = Groups.IndexOf(SelectedGroup) + 1;
                }
            }
        }

        public RelayCommand AddLinkCommand { get; set; }
        public RelayCommand AddLinkAndPlayCommand { get; set; }
        public RelayCommand BrowseFileCommand { get; set; }
        public RelayCommand RefreshSharingLinksCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AddFromClipboardCommand { get; set; }
        public RelayCommand AddGroupLinksCommand { get; set; }
        public RelayCommand ShowGroupLinksCommand { get; set; }
        public RelayCommand SelectQualityCommand { get; set; }
        public RelayCommand RemoveLinkFromGroupListCommand { get; set; }

        int _SelectedGroupIndex = 0;
        public int SelectedGroupIndex
        {
            get
            {
                return _SelectedGroupIndex;
            }
            set
            {
                _SelectedGroupIndex = value;
                if (SelectedGroupIndex == 0)
                {
                    if (SelectedGroup != ApplicationGroupManager.Current.NoGroup)
                        SelectedGroup = ApplicationGroupManager.Current.NoGroup;
                }
                else
                {
                    if (SelectedGroup != Groups[SelectedGroupIndex - 1])
                        SelectedGroup = Groups[SelectedGroupIndex - 1];
                }
                OnPropertyChanged("SelectedGroupIndex");
            }
        }

        public override void AddLink()
        {
            base.AddLink();
            ViewsUtility.RemoveCurrentPage();
        }

        public override void AddLinkAndPlay()
        {
            base.AddLinkAndPlay();
            ViewsUtility.RemoveCurrentPage();
        }

        public void AddFromClipboard()
        {
            //AddFromList(Clipboard.GetText(TextDataFormat.UnicodeText));
        }

        public override void BrowseFile()
        {
            //System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = SaveToPath, ShowNewFolderButton = true };
            //if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    if (!MPath.EqualPath(folderDialog.SelectedPath, SaveToPath))
            //    {
            //        SaveToPath = folderDialog.SelectedPath;
            //    }
            //}
        }
    }
}
