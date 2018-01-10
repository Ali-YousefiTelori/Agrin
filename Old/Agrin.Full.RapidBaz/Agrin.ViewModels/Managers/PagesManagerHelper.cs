using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

namespace Agrin.ViewModels.Managers
{
    public static class PagesManagerHelper
    {
        static Dictionary<FrameworkElement, bool> _NobatiItems;

        public static Dictionary<FrameworkElement, bool> NobatiItems
        {
            get
            {
                if (_NobatiItems == null)
                    _NobatiItems = new Dictionary<FrameworkElement, bool>();
                return PagesManagerHelper._NobatiItems;
            }
            set { PagesManagerHelper._NobatiItems = value; }
        }

        /// <summary>
        /// برای تب ها
        /// </summary>
        static Dictionary<PageItem, RelayCommand> _BasePageItems;
        public static Dictionary<PageItem, RelayCommand> BasePageItems
        {
            get
            {
                if (_BasePageItems == null)
                    _BasePageItems = new Dictionary<PageItem, RelayCommand>();
                return _BasePageItems;
            }
            set { _BasePageItems = value; }
        }


        /// <summary>
        /// تب انتخاب شده
        /// </summary>
        public static FrameworkElement SelectedPage { get; set; }


        static PageItem FindPageItem(FrameworkElement basePage)
        {
            foreach (var item in BasePageItems)
            {
                if (item.Key.Page == basePage)
                {
                    return item.Key;
                }
            }
            return null;
        }

        public static FrameworkElement FindBasePageItem<T>()
        {
            foreach (var pageItem in BasePageItems)
            {
                foreach (var item in pageItem.Key.InnerPageItems)
                {
                    if (item.Key.GetType() == typeof(T))
                        return pageItem.Key.Page;
                }
            }
            return null;
        }

        public static FrameworkElement FindPageItem<T>()
        {
            foreach (var pageItem in BasePageItems)
            {
                foreach (var item in pageItem.Key.InnerPageItems)
                {
                    if (item.Key.GetType() == typeof(T))
                        return item.Key;
                }
            }
            return null;
        }

        public static void AddPageToTabPageItem(FrameworkElement basePage, FrameworkElement element, RelayCommand command)
        {
            PageItem page = FindPageItem(basePage);
            page.InnerPageItems.Add(element, command);
        }

        public static void AddBaseTabPageItem(FrameworkElement element, FrameworkElement defualtItem, RelayCommand command)
        {
            BasePageItems.Add(new PageItem() { Page = element, SelectedInnerPage = defualtItem }, command);
        }

        public static void SelectInnerPage(FrameworkElement basePage, FrameworkElement element)
        {

        }

        public static void SelectPage(FrameworkElement page)
        {

        }

        public static void SetIndex(FrameworkElement pageManagerElement,FrameworkElement newControl, FrameworkElement currentContentControl, FrameworkElement currentContentControlCollapsed, FrameworkElement currentControl, FrameworkElement currentControlCollapsed,
              Action<FrameworkElement> setCurrentContentControl, Action<FrameworkElement> setCurrentContentControlCollapsed)
        {
            //var pageItem = FindPageItem(basePage);
            //FrameworkElement newControl = null;
            //foreach (var item in pageItem.InnerPageItems)
            //{
            //    if (item.Key.GetType() == typeof(T))
            //    {
            //        newControl = item.Key;
            //    }
            //}
            if (!NobatiItems.ContainsKey(pageManagerElement))
            {
                NobatiItems.Add(pageManagerElement, true);
            }
            bool nobati = NobatiItems[pageManagerElement];
            if ((nobati && currentContentControl == newControl) || (!nobati && currentContentControlCollapsed == newControl))
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
                    setCurrentContentControlCollapsed(newControl);
                    currentControl.BeginStoryboard(story);
                }
                else
                {
                    setCurrentContentControl(newControl);
                    currentControlCollapsed.BeginStoryboard(story);
                }
                story = new Storyboard();
                animation = new ThicknessAnimation(new Thickness(-800, 0, 800, 0), new Thickness(0), new Duration(new TimeSpan(0, 0, 0, 0, 500))) { DecelerationRatio = 1 };
                story.BeginTime = new TimeSpan(0, 0, 0, 0, 200);
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
                    currentControl.BeginStoryboard(story);
                else
                    currentControlCollapsed.BeginStoryboard(story);

                NobatiItems[pageManagerElement] = !nobati;
            }
        }
    }

    public class PageItem
    {
        /// <summary>
        /// صفحات موجود در داخل یک تب
        /// </summary>
        Dictionary<FrameworkElement, RelayCommand> _InnerPageItems;
        public Dictionary<FrameworkElement, RelayCommand> InnerPageItems
        {
            get
            {
                if (_InnerPageItems == null)
                    _InnerPageItems = new Dictionary<FrameworkElement, RelayCommand>();
                return _InnerPageItems;
            }
            set { _InnerPageItems = value; }
        }
        /// <summary>
        /// خود تب
        /// </summary>
        public FrameworkElement Page { get; set; }
        /// <summary>
        /// دکمه ی انتخاب شده از داخل یک تب
        /// </summary>
        public FrameworkElement SelectedInnerPage { get; set; }
    }
}
