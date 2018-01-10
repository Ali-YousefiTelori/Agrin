using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Agrin.Download.Manager;
using Agrin.Helpers;
using System.Threading.Tasks;
using Agrin.Download.Data.Settings;
using Agrin.Framesoft.Messages;

namespace Agrin.Views.List
{
    public class NoticesManager : IDisposable
    {
        Activity CurrentActivity { get; set; }

        LinearLayout _mainLayoutListView = null;
        LinearLayout _mainLayout;
        LinearLayout _actionToolBoxLayout;
        LinearLayout _actionTopToolBoxLayout;
        int _templateResourceId;
        View _CustomListView;
        View _noticesToolBox;
        public NoticesManager(Activity context, int templateResourceId, LinearLayout mainLayout, LinearLayout actionToolBoxLayout, LinearLayout actionTopToolBoxLayout)
        {
            CurrentActivity = context;
            _mainLayout = mainLayout;
            _templateResourceId = templateResourceId;
            _CustomListView = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.CustomListView, mainLayout, false);
            _mainLayoutListView = _CustomListView.FindViewById<LinearLayout>(Resource.CustomListView.mainLayoutListView);
            _actionToolBoxLayout = actionToolBoxLayout;
            _actionTopToolBoxLayout = actionTopToolBoxLayout;

            _noticesToolBox = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.NoticesToolBox, mainLayout, false);


            var btnRefresh = _noticesToolBox.FindViewById<Button>(Resource.NoticesToolBox.btnRefresh);
            var btnSendMessage = _noticesToolBox.FindViewById<Button>(Resource.NoticesToolBox.btnSendMessage);
            var linearLayoutReverse = _noticesToolBox.FindViewById<View>(Resource.NoticesToolBox.linearLayoutReverse);

            btnRefresh.Click += btnRefresh_Click;
            btnSendMessage.Click += btnSendMessage_Click;

            ViewsUtility.SetRightToLeftLayout(_noticesToolBox, new List<int>() { Resource.NoticesToolBox.linearLayoutReverse });
            ViewsUtility.SetTextLanguage(CurrentActivity, _noticesToolBox, new List<int>() { Resource.NoticesToolBox.btnRefresh, Resource.NoticesToolBox.btnSendMessage });

            InitializeView();

            ApplicationNoticeManager.Current.NoticeAddedAction = (notice) =>
            {
                if (_isDispose)
                    return;
                lock (lockOBJ)
                {
                    CurrentActivity.RunOnUI(() =>
                    {
                        if (items.ContainsKey(notice))
                            return;
                        var viewItem = DrawOneView(notice);
                        _mainLayoutListView.AddView(viewItem, 0);
                        items.Add(notice, viewItem);
                        if (MainActivity.This is MainActivity)
                        {
                            ((MainActivity)MainActivity.This).GenerateAlertCountData();
                        }
                    });
                }
            };

            InitializeAllItems();
            if (MainActivity.This is MainActivity)
            {
                ((MainActivity)MainActivity.This).GenerateAlertCountData();
            }
        }

        string lastUserMessage = "";
        void btnSendMessage_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;

            var sendFeedback = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.SendFeedback, _mainLayout, false);
            var txtMessage = sendFeedback.FindViewById<EditText>(Resource.SendFeedback.txtMessage);
            txtMessage.Text = lastUserMessage;
            ViewsUtility.ShowCustomResultDialog(CurrentActivity, "ارسال پیام...", sendFeedback, () =>
            {
                string message = "";
                message = txtMessage.Text;
                Task task = new Task(() =>
                {
                    try
                    {
                        var send = Agrin.Framesoft.Helper.FeedBackHelper.SendFeedBack(new Guid(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid), message, ApplicationSetting.Current.FramesoftSetting.UserName, ApplicationSetting.Current.FramesoftSetting.Email);
                        CurrentActivity.RunOnUI(() =>
                        {
                            if (send)
                            {
                                lastUserMessage = "";
                                Toast.MakeText(CurrentActivity, "پیام شما با موفقیت ارسال شد.", ToastLength.Long).Show();
                                ApplicationNoticeManager.Current.AddNotice(new NoticeInfo() { IsRead = true, Mode = NoticeMode.YourMessage, Data = new PublicMessageInfoReceiveData() { Message = message, Title = "پیام شما به ما ارسال شد. با تشکر", MessageDateTime = DateTime.Now } });
                            }
                            else
                            {
                                Toast.MakeText(CurrentActivity, "پیام شما ارسال نشد.", ToastLength.Long).Show();
                                lastUserMessage = txtMessage.Text;
                            }
                            btn.Enabled = true;
                        });
                    }
                    catch
                    {
                        CurrentActivity.RunOnUI(() =>
                        {
                            lastUserMessage = txtMessage.Text;
                            Toast.MakeText(CurrentActivity, "خطا در ارسال پیام رخ داده است.", ToastLength.Long).Show();
                            btn.Enabled = true;
                        });
                    }
                });
                task.Start();
            }, () =>
            {
                btn.Enabled = true;
                lastUserMessage = txtMessage.Text;
            }, null, "ارسال پیام");
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;
            Toast.MakeText(CurrentActivity, "در حال بارگزاری...", ToastLength.Short).Show();
            Task task = new Task(() =>
            {
                try
                {
                    Agrin.Download.Engine.TimeDownloadEngine.RefrshUserMessage();

                    CurrentActivity.RunOnUI(() =>
                    {
                        Toast.MakeText(CurrentActivity, "بارگذاری انجام شد", ToastLength.Long).Show();
                        btn.Enabled = true;
                    });
                }
                catch (Exception ex)
                {
                    CurrentActivity.RunOnUI(() =>
                    {
                        btn.Enabled = true;
                        Toast.MakeText(CurrentActivity, ex.Message, ToastLength.Long).Show();
                    });
                }
            });
            task.Start();
        }

        public void InitializeView()
        {
            if (_actionTopToolBoxLayout.ChildCount > 0)
                _actionTopToolBoxLayout.RemoveAllViews();
            _actionTopToolBoxLayout.Visibility = ViewStates.Gone; 
            
            _actionToolBoxLayout.RemoveAllViews();
            _actionToolBoxLayout.AddView(_noticesToolBox);
            _actionToolBoxLayout.Visibility = ViewStates.Visible;
            _mainLayout.RemoveAllViews();
            _mainLayout.AddView(_CustomListView);
        }

        public void SetReadNoticeInfo(NoticeInfo item)
        {
            ApplicationNoticeManager.Current.SetReadNotic(item);
            if (items.ContainsKey(item))
            {
                var view = items[item];
                var layoutMain = view.FindViewById<LinearLayout>(Resource.NoticInfoView.layoutMain);
                ViewsUtility.SetAlpha(layoutMain, !item.IsRead);
            }
        }

        //List<NoticeInfo> items = new List<NoticeInfo>();
        Dictionary<NoticeInfo, View> items = new Dictionary<NoticeInfo, View>();
        object lockOBJ = new object();
        public void InitializeAllItems()
        {
            items.Clear();
            lock (lockOBJ)
            {
                foreach (var item in ApplicationNoticeManager.Current.Items.ToList())
                {
                    if (_isDispose)
                        return;
                    var view = DrawOneView(item);
                    _mainLayoutListView.AddView(view, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    items.Add(item, view);
                }
            }
        }

        bool _isDispose = false;

        public View DrawOneView(NoticeInfo item)
        {
            if (_isDispose)
                return null;

            var view = CurrentActivity.LayoutInflater.Inflate(_templateResourceId, _mainLayoutListView, false);

            var layoutMain = view.FindViewById<LinearLayout>(Resource.NoticInfoView.layoutMain);
            ViewsUtility.SetRightToLeftLayout(view, new List<int>() { Resource.NoticInfoView.layoutTop, Resource.NoticInfoView.layoutbottom });

            var txtTitle = view.FindViewById<TextView>(Resource.NoticInfoView.txtTitle);
            var txtMessage = view.FindViewById<TextView>(Resource.NoticInfoView.txtMessage);
            var txtDateTime = view.FindViewById<TextView>(Resource.NoticInfoView.txtDateTime);

            if (item.Mode == NoticeMode.PublicMessage || item.Mode == NoticeMode.YourMessage)
            {
                txtTitle.Text = ((Framesoft.Messages.PublicMessageInfoReceiveData)item.Data).Title;
                txtMessage.Text = ((Framesoft.Messages.PublicMessageInfoReceiveData)item.Data).Message;
                txtDateTime.Text = DateTimeToText(((Framesoft.Messages.PublicMessageInfoReceiveData)item.Data).MessageDateTime);
            }
            else if (item.Mode == NoticeMode.UserMessage)
            {
                txtTitle.Text = "به پیام شما پاسخ داده شد";
                txtMessage.Text = ((Framesoft.Messages.UserMessageInfoData)item.Data).Message;
                txtDateTime.Text = DateTimeToText(((Framesoft.Messages.UserMessageInfoData)item.Data).MessageDateTime);
            }
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                txtTitle.Text = txtMessage.Text;
                txtMessage.Visibility = ViewStates.Gone;
            }
            if (item.IsRead)
            {
                ViewsUtility.SetAlpha(layoutMain, !item.IsRead);
            }

            layoutMain.Tag = item.Cast();
            layoutMain.Clickable = true;
            layoutMain.Click -= layoutMain_Click;
            layoutMain.Click += layoutMain_Click;

            return view;
        }

        void layoutMain_Click(object sender, EventArgs e)
        {
            var layoutMain = sender as View;
            var info = layoutMain.Tag.Cast<NoticeInfo>();
            SetReadNoticeInfo(info);
            ((MainActivity)(MainActivity.This)).GenerateAlertCountData();
            TextView txtTitle = new TextView(CurrentActivity);
            TextView txtMessage = new TextView(CurrentActivity);

            LinearLayout layout = new LinearLayout(CurrentActivity);
            layout.Orientation = Orientation.Vertical;
            layout.SetPadding(10, 10, 10, 10);
            ScrollView scroll = new ScrollView(CurrentActivity);
            scroll.AddView(txtMessage);
            ViewsUtility.SetBackground(CurrentActivity, layout);
            ViewsUtility.SetTextViewTextColor(CurrentActivity, txtTitle, Resource.Color.foreground);
            ViewsUtility.SetTextViewTextColor(CurrentActivity, txtMessage, Resource.Color.foreground);

            string uri = "";
            Action openSite = () =>
            {
                Intent i = new Intent(Intent.ActionView);
                i.SetData(Android.Net.Uri.Parse(uri));
                CurrentActivity.StartActivity(i);
            };
            string openSiteText = null;
            if (info.Mode == NoticeMode.PublicMessage || info.Mode == NoticeMode.YourMessage)
            {
                txtTitle.Text = ((Framesoft.Messages.PublicMessageInfoReceiveData)info.Data).Title;
                txtMessage.Text = ((Framesoft.Messages.PublicMessageInfoReceiveData)info.Data).Message;
                if (((Framesoft.Messages.PublicMessageInfoReceiveData)info.Data).Link != null)
                {
                    openSiteText = "بازکردن سایت";
                    uri = ((Framesoft.Messages.PublicMessageInfoReceiveData)info.Data).Link;
                }
                else
                    openSite = null;
            }
            else if (info.Mode == NoticeMode.UserMessage)
            {
                txtTitle.Text = "پاسخی که از سوی ما ارسال شد:";
                txtMessage.Text = ((Framesoft.Messages.UserMessageInfoData)info.Data).Message;
                if (((Framesoft.Messages.UserMessageInfoData)info.Data).Link != null)
                {
                    openSiteText = "بازکردن سایت";
                    uri = ((Framesoft.Messages.UserMessageInfoData)info.Data).Link;
                }
                else
                    openSite = null;
            }

            layout.AddView(txtTitle);
            layout.AddView(scroll);

            ViewsUtility.ShowCustomResultDialog(CurrentActivity, "مشاهده ی پیام...", layout, () =>
            {

            }, () =>
            {
            }, openSite, "تایید", openSiteText);
        }

        System.Globalization.Calendar cal = new System.Globalization.PersianCalendar();
        string DateTimeToText(DateTime dt)
        {
            return cal.GetYear(dt) + "/" + cal.GetMonth(dt) + "/" + cal.GetDayOfMonth(dt) + " " + dt.Hour + ":" + dt.Minute;
        }

        public void Dispose()
        {
            try
            {
                _isDispose = true;
                _mainLayoutListView.RemoveAllViews();
                foreach (var item in items)
                {
                    item.Value.Dispose();
                }
                items.Clear();
                CurrentActivity = null;
                _mainLayoutListView.Dispose();
                _mainLayoutListView = null;

                _mainLayout.Dispose();
                _CustomListView.Dispose();
                _mainLayout = null;
                _CustomListView = null;
                _noticesToolBox.Dispose();
                _noticesToolBox = null;
                cal = null;
                ApplicationNoticeManager.Current.NoticeAddedAction = null;

                GC.Collect();
            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "NoticesManager Dispose");
            }
        }
    }
}