using Agrin.BaseViewModels.Group;
using Agrin.IO.Helper;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Group
{
    public class AddGroupViewModel : AddGroupBaseViewModel
    {
        public AddGroupViewModel()
        {
            AddGroupCommand = new RelayCommand(AddGroup, CanAddGroup);
            //EditGroupCommand = new RelayCommand(EditGroup, CanEditGroup);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            BackCommand = new RelayCommand(BackItem);
        }

        public RelayCommand AddGroupCommand { get; set; }
        //public RelayCommand EditGroupCommand { get; set; }
        public RelayCommand BrowseFileCommand { get; set; }
        public RelayCommand BackCommand { get; set; }

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
