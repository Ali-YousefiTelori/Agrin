using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Agrin
{
    public partial class App : Application
    {
        public App()
        {
            ANotifyPropertyChanged.RunCanExecuteCommand = (command) =>
            {
                ((RelayCommand)command).ChangeCanExecute();
            };

            //RelayCommand.CreateActionObj = (action) =>
            //{
            //    return new Command<T>(action);
            //};

            ViewsUtility.NavigateToPage = (page) =>
            {
                NavigateTo(page);
            };

            ViewsUtility.RemoveCurrentPage = () =>
            {
                RemoveCurrentPage();
            };
            InitializeComponent();

            // The Application ResourceDictionary is available in Xamarin.Forms 1.3 and later
            if (Application.Current.Resources == null)
            {
                Application.Current.Resources = new ResourceDictionary();
            }

            //var appStyle = new Style(typeof(Label))
            //{
            //    BaseResourceKey = Device.Styles.SubtitleStyleKey,
            //    Setters = {
            //        new Setter { Property = Label.TextColorProperty, Value = Color.Green }
            //    }
            //};
            //Application.Current.Resources.Add("AppStyle", appStyle);

            //var boxStyle = new Style(typeof(BoxView))
            //{
            //    Setters = {
            //        new Setter { Property = BoxView.ColorProperty, Value = Color.Aqua }
            //    }
            //};
            //Application.Current.Resources.Add(boxStyle); // implicit style for ALL boxviews

            //var tabs = new TabbedPage();
            //tabs.Children.Add(new StylePage { Title = "C#", Icon = "csharp.png" });
            //tabs.Children.Add(new StyleXaml { Title = "Xaml", Icon = "xaml.png" });
            //tabs.Children.Add(new DynamicResourceXaml { Title = "Dynamic", Icon = "xaml.png" });
            ////tabs.Children.Add (new OldResourceDictionary {Title = "Old", Icon = "xaml.png"});
            PageManager = new NavigationPage(new OldViews.MainMenu());
            MainPage = PageManager;
        }

        static NavigationPage PageManager { get; set; }
        static Page CurrentPage { get; set; }
        public static async void NavigateTo(object page)
        {
            CurrentPage = (Page)page;
            await PageManager.PushAsync(CurrentPage);
        }

        public static void RemoveCurrentPage()
        {
            PageManager.Navigation.RemovePage(CurrentPage);
        }
    }
}
