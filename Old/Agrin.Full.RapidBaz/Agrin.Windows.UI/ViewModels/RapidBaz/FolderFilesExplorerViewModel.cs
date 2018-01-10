using Agrin.RapidBaz.Models;
using Agrin.Windows.UI.Views.RapidBaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Windows.UI.ViewModels.RapidBaz
{
    public class FolderFilesExplorerViewModel : Agrin.ViewModels.RapidBaz.FolderFilesExplorerViewModel
    {
        public QueueListRapidBazViewModel MainQueueListRapidBazViewModel { get; set; }
        public override IEnumerable<DocumantInfo> GetSelectedLinks()
        {
            if (FolderFilesExplorer.This == null || FolderFilesExplorer.This.MainList == null)
                return new List<DocumantInfo>();

            return FolderFilesExplorer.This.MainList.SelectedItems.Cast<DocumantInfo>();
        }
    }
}
