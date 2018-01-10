﻿using Agrin.BaseViewModels.RapidBaz;
using Agrin.RapidBaz.Models;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agrin.ViewModels.RapidBaz
{
    public class QueueListRapidBazViewModel : QueueListRapidBazBaseViewModel
    {
        public QueueListRapidBazViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            DeleteCommand = new RelayCommand(ShowDeleteMessageBox, CanDeleteSelectedItem);
            DeleteOkCommand = new RelayCommand(DeleteSelectedItem);
            RetryCommand = new RelayCommand(RetrySelectedItems, CanRetrySelectedItems);
            PopupShowSettingCommand = new RelayCommand(PopupShowSetting);
            PopupSettingSaveCommand = new RelayCommand(PopupSettingSave);
            CopyLinkLocationCommand = new RelayCommand(CopyLinkLocation);
            AddCommand = new RelayCommand(AddRapidBazLink);
            AddLinkCommand = new RelayCommand(AddLinkSelectedItem, CanAddLinkSelectedItem);
            AddToListCommand = new RelayCommand(AddToList, CanAddLinkSelectedItem);
            AddToListAndDownloadCommand = new RelayCommand(AddToListAndDownload, CanAddLinkSelectedItem);
        }

        public RelayCommand AddCommand { get; set; }

        public RelayCommand AddToListCommand { get; set; }
        public RelayCommand AddToListAndDownloadCommand { get; set; }
        public RelayCommand AddLinkCommand { get; set; }

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteOkCommand { get; set; }
        public RelayCommand RetryCommand { get; set; }
        public RelayCommand PopupShowSettingCommand { get; set; }
        public RelayCommand PopupSettingSaveCommand { get; set; }
        public RelayCommand CopyLinkLocationCommand { get; set; }
        
        public void CopyLinkLocation()
        {
            string links = base.CopyLinkLocationBase();
            Clipboard.SetText(links);
        }
    }
}
