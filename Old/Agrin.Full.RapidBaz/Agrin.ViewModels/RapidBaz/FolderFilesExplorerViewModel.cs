using Agrin.BaseViewModels.RapidBaz;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.RapidBaz
{
    public class FolderFilesExplorerViewModel : FolderFilesExplorerBaseViewModel
    {
        public FolderFilesExplorerViewModel()
        {
            RefreshCommand = new RelayCommand(RefreshFolderListManual);
            DeleteFolderCommand = new RelayCommand(DeleteFolderMessage, CanDeleteFolderMessage);
            MessageCommand = new RelayCommand(DeleteSelectionFolder);
            AddFolderCommand = new RelayCommand(AddFolder);
            AddFolderOKCommand = new RelayCommand(AddFolderOK,CanAddFolder);
        }

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand AddFolderCommand { get; set; }
        public RelayCommand AddFolderOKCommand { get; set; }
        public RelayCommand DeleteFolderCommand { get; set; }
        public RelayCommand MessageCommand { get; set; }
    }
}
