using Agrin.Helper.ComponentModel;
using Agrin.UI.Views.Lists;
using Agrin.UI.Views.Pages;
using Agrin.UI.Views.Pages.Authorization;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

namespace Agrin.UI.ViewModels.Pages
{
    public class PagesManagerViewModel : ANotifyPropertyChanged<PagesManager>
    {
        public PagesManagerViewModel()
        {
            _This = this;
            BackCommand = new RelayCommand(BackItem);
            
            AddItem(new AddLinks(), new RelayCommand(BackToList), true);
            AddItem(new AddGroup(), new RelayCommand(BackToList), false);
            AddItem(new AboutPage(), new RelayCommand(BackToList), false);
            AddItem(new SendFeedBack(), new RelayCommand(BackToList), false);
            AddItem(new LinkInfoSetting(), new RelayCommand(BackToList), false);
            AddItem(new TaskManager(), new RelayCommand(BackToList), false);
            AddItem(new Login(), new RelayCommand(BackToList), false);
            AddItem(new RapidBazGetList(), new RelayCommand(BackToList), false);//7
        }

        static PagesManagerViewModel _This;
        public static PagesManagerViewModel This
        {
            get { return PagesManagerViewModel._This; }
            set { PagesManagerViewModel._This = value; }
        }

        public RelayCommand BackCommand { get; set; }

        FrameworkElement _CurrentControl;

        public FrameworkElement CurrentControl
        {
            get { return _CurrentControl; }
            set { _CurrentControl = value; OnPropertyChanged("CurrentControl"); }
        }

        FrameworkElement _CurrentControlCollapsed;

        public FrameworkElement CurrentControlCollapsed
        {
            get { return _CurrentControlCollapsed; }
            set { _CurrentControlCollapsed = value; OnPropertyChanged("CurrentControlCollapsed"); }
        }


        Dictionary<FrameworkElement, RelayCommand> _Items;

        public Dictionary<FrameworkElement, RelayCommand> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new Dictionary<FrameworkElement, RelayCommand>();
                return _Items;
            }
            set { _Items = value; }
        }

        static bool nobati = true;
        static int lastIndex = -1;
        public static void SetIndex(int index)
        {
            if (lastIndex == index)
                return;
            lastIndex = index;
            var newControl = This.Items.Keys.ToArray()[index];
            if ((nobati && This.CurrentControl == newControl) || (!nobati && This.CurrentControlCollapsed == newControl))
                return;
            else
            {
                Storyboard story = new Storyboard();
                AnimationTimeline animation = new ThicknessAnimation(new Thickness(800, 0, -800, 0), new Duration(new TimeSpan(0, 0, 0, 0, 500))) { AccelerationRatio = 1 };

                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Margin") { Path = "Margin" });
                animation = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity") { Path = "Opacity" });

                ObjectAnimationUsingKeyFrames objKey = new ObjectAnimationUsingKeyFrames() { Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500)) };
                DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame(Visibility.Collapsed);
                objKey.KeyFrames.Add(discreteObjectKeyFrame);
                story.Children.Add(objKey);
                Storyboard.SetTargetProperty(objKey, new PropertyPath("Visibility") { Path = "Visibility" });

                if (nobati)
                {
                    This.CurrentControlCollapsed = This.Items.Keys.ToArray()[index];
                    This.ViewElement.currentControl.BeginStoryboard(story);
                }
                else
                {
                    This.CurrentControl = This.Items.Keys.ToArray()[index];
                    This.ViewElement.currentControlCollapsed.BeginStoryboard(story);
                }
                story = new Storyboard();
                animation = new ThicknessAnimation(new Thickness(-800, 0, 800, 0), new Thickness(0), new Duration(new TimeSpan(0, 0, 0, 0, 500))) { DecelerationRatio = 1 };
                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Margin") { Path = "Margin" });
                animation = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity") { Path = "Opacity" });
                objKey = new ObjectAnimationUsingKeyFrames() { Duration = new Duration(new TimeSpan(0)) };
                discreteObjectKeyFrame = new DiscreteObjectKeyFrame(Visibility.Visible);
                objKey.KeyFrames.Add(discreteObjectKeyFrame);
                story.Children.Add(objKey);
                Storyboard.SetTargetProperty(objKey, new PropertyPath("Visibility") { Path = "Visibility" });

                if (!nobati)
                    This.ViewElement.currentControl.BeginStoryboard(story);
                else
                    This.ViewElement.currentControlCollapsed.BeginStoryboard(story);

                nobati = !nobati;
            }
        }


        public void BackItem()
        {
            Items[CurrentControl].Execute();
        }

        public void AddItem(FrameworkElement element, RelayCommand command, bool current = false)
        {
            Items.Add(element, command);
            if (current)
                CurrentControl = element;
        }

        void BackToList()
        {
            MainWindow.This.IsShowPage = false;
        }
    }
}
