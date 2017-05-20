using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Agrin.UI.ViewModels.Pages.Settings
{
    public class GeneralSettingViewModel : ANotifyPropertyChanged
    {
        public GeneralSettingViewModel()
        {
            This = this;
            ChangeAddressCommand = new RelayCommand(ChangeAddress);
        }

        public RelayCommand ChangeAddressCommand { get; set; }

        public static GeneralSettingViewModel This;

        string _FileType;
        public string FileType
        {
            get { return _FileType; }
            set { _FileType = value; OnPropertyChanged("FileType"); }
        }

        string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged("FileName"); }
        }

        double _Size;
        public double Size
        {
            get { return _Size; }
            set { _Size = value; OnPropertyChanged("Size"); }
        }

        string _saveAddress;

        public string SaveAddress
        {
            get { return _saveAddress; }
            set { _saveAddress = value; OnPropertyChanged("SaveAddress"); }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }


        private void ChangeAddress()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = SaveAddress;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveAddress = dialog.SelectedPath;
            }
        }
    }
}
