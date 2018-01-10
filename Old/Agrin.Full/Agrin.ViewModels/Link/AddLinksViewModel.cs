using Agrin.BaseViewModels.Link;
using Agrin.IO.Helper;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agrin.ViewModels.Link
{
    public class AddLinksViewModel : AddLinksBaseViewModel
    {
        public AddLinksViewModel()
        {
            AddLinkAndPlayCommand = new RelayCommand(AddLinkAndPlay, CanAddLink);
            AddLinkCommand = new RelayCommand(AddLink, CanAddLink);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            RefreshSharingLinksCommand = new RelayCommand(RefreshSharingLinks);
            BackCommand = new RelayCommand(BackItem);
            AddFromClipboardCommand = new RelayCommand(AddFromClipboard);
            ShowGroupLinksCommand = new RelayCommand(ShowGroupList);
            AddGroupLinksCommand = new RelayCommand(AddGroupLinks);
            RemoveLinkFromGroupListCommand = new RelayCommand<string>(RemoveLinkFromGroupList);
            SelectQualityCommand = new RelayCommand(SelectQuality, CanSelectQuality);
        }


        public RelayCommand AddLinkCommand { get; set; }
        public RelayCommand AddLinkAndPlayCommand { get; set; }
        public RelayCommand BrowseFileCommand { get; set; }
        public RelayCommand RefreshSharingLinksCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand AddFromClipboardCommand { get; set; }
        public RelayCommand AddGroupLinksCommand { get; set; }
        public RelayCommand ShowGroupLinksCommand { get; set; }
        public RelayCommand SelectQualityCommand { get; set; }
        public RelayCommand<string> RemoveLinkFromGroupListCommand { get; set; }

        public void AddFromClipboard()
        {
            AddFromList(Clipboard.GetText(TextDataFormat.UnicodeText));
        }

        public override void BrowseFile()
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = SaveToPath, ShowNewFolderButton = true };
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!MPath.EqualPath(folderDialog.SelectedPath, SaveToPath))
                {
                    SaveToPath = folderDialog.SelectedPath;
                }
            }
        }
    }
}
