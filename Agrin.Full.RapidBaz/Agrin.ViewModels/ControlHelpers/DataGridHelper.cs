using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Agrin.ViewModels.ControlHelpers
{
    public class DataGridHelper
    {
        public static bool GetIsSoringManager(DependencyObject d)
        {
            return (bool)d.GetValue(DataGridHelper.IsSoringManagerProperty);
        }

        public static void SetIsSoringManager(DependencyObject d, bool val)
        {
            d.SetValue(DataGridHelper.IsSoringManagerProperty, val);
        }

        public static readonly DependencyProperty IsSoringManagerProperty =
            DependencyProperty.RegisterAttached("IsSoringManager",
                typeof(bool),
                typeof(DataGridHelper),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if ((bool)e.NewValue)
                        {
                            if (d is DataGrid)
                            {
                                ((DataGrid)d).Sorting += DataGridHelper_Sorting;
                            }
                        }
                    }));

        static void DataGridHelper_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            string sortPropertyName = e.Column.SortMemberPath;
            if (!string.IsNullOrEmpty(sortPropertyName))
            {
                // sorting is cleared when the previous state is Descending
                if (e.Column.SortDirection.HasValue && e.Column.SortDirection.Value == ListSortDirection.Descending)
                {
                    e.Column.SortDirection = null;
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                    {
                        foreach (var sd in dataGrid.Items.SortDescriptions
                            .Where(sd => sd.PropertyName == sortPropertyName).ToList())
                        {
                            dataGrid.Items.SortDescriptions.Remove(sd);
                        }
                    }
                    else
                    {
                        dataGrid.Items.SortDescriptions.Clear();
                    }

                    dataGrid.Items.Refresh();
                }
                else
                {
                    ICollectionView collection = CollectionViewSource.GetDefaultView(dataGrid.Items);
                    collection.SortDescriptions.Clear();
                    if (e.Column.SortDirection.HasValue && e.Column.SortDirection.Value == ListSortDirection.Ascending)
                        e.Column.SortDirection = ListSortDirection.Descending;
                    else
                        e.Column.SortDirection = ListSortDirection.Ascending;
                   
                    collection.SortDescriptions.Add(new SortDescription(e.Column.SortMemberPath, e.Column.SortDirection.GetValueOrDefault()));
                }

                e.Handled = true;
                dataGrid.Dispatcher.BeginInvoke((Action)delegate()
                {
                    if (dataGrid.SelectedItem != null)
                    {
                        dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                        dataGrid.UpdateLayout();
                        //int index = dataGrid.Items.IndexOf(dataGrid.SelectedItem);
                        //var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
                    }
                }, null);

            }
        }
    }

}
