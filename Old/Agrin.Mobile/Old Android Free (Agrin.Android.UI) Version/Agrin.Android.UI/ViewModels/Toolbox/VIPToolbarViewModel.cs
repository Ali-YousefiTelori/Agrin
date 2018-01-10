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
using Agrin.Framesoft.Helper;
using Agrin.Download.Data.Settings;
using Agrin.Helper.ComponentModel;
using System.Threading;
using Agrin.Framesoft;
using Agrin.Download.Data;
using Agrin.MonoAndroid.UI.Helper;
using Xamarin.InAppBilling;

namespace Agrin.MonoAndroid.UI.ViewModels.Toolbox
{
    public class VIPToolbarViewModel : IBaseViewModel
    {
        public Activity CurrentActivity { get; set; }
        public BuyProductHelper buyProductHelper = null;
        bool isConnect = false;
        public VIPToolbarViewModel(Activity currentActivity)
        {
            CurrentActivity = currentActivity;
            CreateBuyHelper();
        }

        public void CreateBuyHelper()
        {
            buyProductHelper = new BuyProductHelper(CurrentActivity);
            buyProductHelper.ConnectComeplete = () =>
            {
                if (isConnect)
                {
                    try
                    {
                        var purchases = buyProductHelper.GetPurchases().ToList();
                        BuyPurchases(purchases);
                    }
                    catch (Exception e)
                    {

                    }
                }
                isConnect = true;
            };
            buyProductHelper.StartConnetion();

            buyProductHelper.BuyItemAction = (item) =>
            {
                ApplicationSetting.Current.FramesoftSetting.PurchaseProductIds.Add(item.PurchaseToken);
                SerializeData.SaveApplicationSettingToFile();
                BuyPurchases(new List<Purchase>() { item });
            };
        }

        Button btn_BuyStorage = null, btn_Downloads = null, btn_Login = null, btn_Logout = null, btn_Register = null, btn_UserPermission = null;
        TextView txtUserName = null, txtSize = null, txtUserNameTitle = null, txtSizeTitle = null;
        public void Initialize()
        {
            btn_BuyStorage = CurrentActivity.FindViewById<Button>(Resource.VIPToolbar.btn_BuyStorage);
            btn_BuyStorage.Click += btn_BuyStorage_Click;

            btn_Downloads = CurrentActivity.FindViewById<Button>(Resource.VIPToolbar.btn_Downloads);
            btn_Downloads.Click += btn_Downloads_Click;

            btn_Login = CurrentActivity.FindViewById<Button>(Resource.VIPToolbar.btn_Login);
            btn_Login.Click += btn_Login_Click;

            btn_Logout = CurrentActivity.FindViewById<Button>(Resource.VIPToolbar.btn_Logout);
            btn_Logout.Click += btn_Logout_Click;

            btn_Register = CurrentActivity.FindViewById<Button>(Resource.VIPToolbar.btn_Register);
            btn_Register.Click += btn_Register_Click;

            btn_UserPermission = CurrentActivity.FindViewById<Button>(Resource.VIPToolbar.btn_UserPermission);
            btn_UserPermission.Click += btn_UserPermission_Click;

            txtUserName = CurrentActivity.FindViewById<TextView>(Resource.VIPToolbar.txtUserName);
            txtSize = CurrentActivity.FindViewById<TextView>(Resource.VIPToolbar.txtSize);

            txtUserNameTitle = CurrentActivity.FindViewById<TextView>(Resource.VIPToolbar.txtUserNameTitle);
            txtSizeTitle = CurrentActivity.FindViewById<TextView>(Resource.VIPToolbar.txtSizeTitle);

            SetIsLoginVisibilities();
            if (!string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.UserName) && !string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.Password))
            {
                AutoLoginNow(CurrentActivity, (user) =>
                {
                    string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(user.Size);
                    txtSize.Text = size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                    txtUserName.Text = user.UserName;
                    SetIsLoginVisibilities();
                    if (isConnect)
                    {
                        try
                        {
                            var purchases = buyProductHelper.GetPurchases().ToList();
                            BuyPurchases(purchases);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    else
                        isConnect = true;
                });
            }
        }

        void SetIsLoginVisibilities()
        {
            btn_Register.Visibility = UserManagerHelper.IsLogin ? ViewStates.Gone : ViewStates.Visible;
            btn_Login.Visibility = UserManagerHelper.IsLogin ? ViewStates.Gone : ViewStates.Visible;

            btn_BuyStorage.Visibility = UserManagerHelper.IsLogin ? ViewStates.Visible : ViewStates.Gone;
            btn_Downloads.Visibility = UserManagerHelper.IsLogin ? ViewStates.Visible : ViewStates.Gone;
            btn_Logout.Visibility = UserManagerHelper.IsLogin ? ViewStates.Visible : ViewStates.Gone;
            txtUserName.Visibility = txtUserNameTitle.Visibility = txtSizeTitle.Visibility = txtSize.Visibility = UserManagerHelper.IsLogin ? ViewStates.Visible : ViewStates.Gone;
        }

        static UserInfoData currentUserInfoData { get; set; }
        public static void AutoLoginNow(Activity activity, Action<UserInfoData> complete = null, Action errorAction = null)
        {
            if (UserManagerHelper.IsLogin)
            {
                if (complete != null)
                    complete(currentUserInfoData);
                return;
            }
            var progressDialog = ProgressDialog.Show(activity, ViewUtility.FindTextLanguage(activity, "Login_Language"), ViewUtility.FindTextLanguage(activity, "LoginIn_Language"), true, false);
            Action<string> errorLogin = (msg) =>
            {
                activity.RunOnUiThread(() =>
                {
                    progressDialog.SetMessage(ViewUtility.FindTextLanguage(activity, "LoginError_Language") + ": " + System.Environment.NewLine + msg);
                });
                System.Threading.Thread.Sleep(2000);
                activity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                    if (errorAction != null)
                        errorAction();
                });
            };
            AsyncActions.Action(() =>
            {
                if (string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.UserName) || string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.Password))
                {
                    errorLogin(ViewUtility.FindTextLanguage(activity, "EmptyUserPass_Language"));
                    return;
                }
                var login = UserManagerHelper.LoginUser(new Framesoft.UserInfoData() { UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash() });
                if (login.Data != null)
                {
                    currentUserInfoData = login.Data;
                    UserManagerHelper.IsLogin = true;
                    activity.RunOnUiThread(() =>
                    {
                        progressDialog.SetMessage(ViewUtility.FindTextLanguage(activity, "LoginSuccess_Language"));
                    });
                    Thread.Sleep(1000);
                    activity.RunOnUiThread(() =>
                    {
                        if (complete != null)
                            complete(login.Data);
                        progressDialog.Dismiss();
                        progressDialog.Hide();
                    });
                }
                else
                {
                    errorLogin(ViewUtility.FindTextLanguage(activity, UserManagerHelper.GetServerResponseMessageValue(login.Message, "")));
                }

            }, (error) =>
            {
                errorLogin(error.Message);
            });
        }

        public static void UploadLinkToServer(Activity activity, Action complete, string link, bool isYoutube = false, int formatCode = 0)
        {
            var progressDialog = ProgressDialog.Show(activity, ViewUtility.FindTextLanguage(activity, "SendFile_Language"), ViewUtility.FindTextLanguage(activity, "SendingFile_Language"), true, false);
            Action<string> errorLogin = (msg) =>
            {
                activity.RunOnUiThread(() =>
                {
                    progressDialog.SetMessage(ViewUtility.FindTextLanguage(activity, "SendingFileError_Language") + ": " + System.Environment.NewLine + msg);
                });
                System.Threading.Thread.Sleep(2000);
                activity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                    //if (errorAction != null)
                    //    errorAction();
                });
            };
            AsyncActions.Action(() =>
            {
                if (string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.UserName) || string.IsNullOrEmpty(ApplicationSetting.Current.FramesoftSetting.Password))
                {
                    errorLogin(ViewUtility.FindTextLanguage(activity, "EmptyUserPass_Language"));
                    return;
                }
                ResponseData<UserFileInfoData> file = null;
                if (isYoutube)
                    file = UserManagerHelper.DownloadYoutubeLink(new Framesoft.UserFileInfoData() { UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash(), Link = link, FormatCode = formatCode });
                else
                    file = UserManagerHelper.UploadOneLink(new Framesoft.UserFileInfoData() { UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash(), Link = link });
                if (file.Data != null)
                {
                    UserManagerHelper.IsLogin = false;
                    activity.RunOnUiThread(() =>
                    {
                        progressDialog.SetMessage(ViewUtility.FindTextLanguage(activity, "SendingFileSuccess_Language"));
                    });
                    Thread.Sleep(1000);
                    activity.RunOnUiThread(() =>
                    {
                        if (complete != null)
                            complete();
                        progressDialog.Dismiss();
                        progressDialog.Hide();
                    });
                }
                else
                {
                    errorLogin(ViewUtility.FindTextLanguage(activity, UserManagerHelper.GetServerResponseMessageValue(file.Message, "")));
                }

            }, (error) =>
            {
                errorLogin(error.Message);
            });
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void RegisterNow(Action errorAction, Action complete, string userName = "", string password = "", string email = "")
        {
            var progressDialog = ProgressDialog.Show(CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "RegisterUser_Language"), ViewUtility.FindTextLanguage(CurrentActivity, "Registering_Language"), true, false);
            Action<string> errorRegister = (msg) =>
            {
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "RegisterError_Language") + ": " + System.Environment.NewLine + msg);
                });
                System.Threading.Thread.Sleep(2000);
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                    if (errorAction != null)
                        errorAction();
                });
            };

            AsyncActions.Action(() =>
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
                {
                    errorRegister(ViewUtility.FindTextLanguage(CurrentActivity, "EmptyUserPassEmail_Language"));
                    return;
                }
                else if (!IsValidEmail(email))
                {
                    errorRegister(ViewUtility.FindTextLanguage(CurrentActivity, "InvalidEmail_Language"));
                    return;
                }

                var register = UserManagerHelper.RegisterUser(new Framesoft.UserInfoData() { UserName = userName, Password = password.Sha1Hash(), ApplicationGuid = Guid.Parse(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid), Email = email });
                if (register.Message == "OK")
                {
                    CurrentActivity.RunOnUiThread(() =>
                    {
                        progressDialog.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "RegisterSuccess_Language"));
                    });
                    Thread.Sleep(1000);
                    CurrentActivity.RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                        progressDialog.Hide();
                        if (complete != null)
                            complete();
                    });
                }
                else
                {
                    errorRegister(ViewUtility.FindTextLanguage(CurrentActivity, UserManagerHelper.GetServerResponseMessageValue(register.Message, "")));
                }

            }, (error) =>
            {
                errorRegister(error.Message);
            });
        }

        void btn_UserPermission_Click(object sender, EventArgs e)
        {
            ViewUtility.ShowPageDialog("http://framesoft.ir/UserManager/PermissionReadMe", "VIPAccount_Language", null, CurrentActivity);
        }

        void btn_Register_Click(object sender, EventArgs e)
        {
            ShowRegisterDialog();
        }

        void ShowRegisterDialog(string userName = "", string password = "", string email = "")
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(CurrentActivity);
            builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "RegisterUser_Language"));
            LinearLayout layout = new LinearLayout(CurrentActivity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;


            EditText txtUserNameE = new EditText(this.CurrentActivity);
            txtUserNameE.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "UserName_Language");
            txtUserNameE.Text = userName;

            EditText txtPassword = new EditText(this.CurrentActivity);
            txtPassword.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "Password_Language");
            txtPassword.Text = password;

            EditText txtEmail = new EditText(this.CurrentActivity);
            txtEmail.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "Email_Language");
            txtEmail.Text = email;

            layout.AddView(txtUserNameE);
            layout.AddView(txtPassword);
            layout.AddView(txtEmail);

            builder.SetView(layout);
            AlertDialog dialogW = null;

            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "RegisterUser_Language"), (EventHandler<DialogClickEventArgs>)null);
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
            dialogW = builder.Create();
            dialogW.SetCancelable(false);
            dialogW.Show();

            // Get the buttons.
            var yesBtn = dialogW.GetButton((int)DialogButtonType.Negative);
            var noBtn = dialogW.GetButton((int)DialogButtonType.Positive);

            yesBtn.Click += (s, args) =>
            {
                RegisterNow(() =>
                {
                    ShowRegisterDialog(txtUserNameE.Text, txtPassword.Text, txtEmail.Text);
                }, () =>
                {
                    ApplicationSetting.Current.FramesoftSetting.UserName = txtUserNameE.Text;
                    ApplicationSetting.Current.FramesoftSetting.Password = txtPassword.Text;
                    ApplicationSetting.Current.FramesoftSetting.Email = txtEmail.Text;
                    SerializeData.SaveApplicationSettingToFile();
                    AutoLoginNow(CurrentActivity, (user) =>
                        {
                            SerializeData.SaveApplicationSettingToFile();
                            string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(user.Size);
                            txtSize.Text = size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                            txtUserName.Text = user.UserName;
                            SetIsLoginVisibilities();
                        });
                }, txtUserNameE.Text, txtPassword.Text, txtEmail.Text);
                dialogW.Dismiss();
            };
            noBtn.Click += (s, args) =>
            {
                dialogW.Dismiss();
            };
        }

        void btn_Logout_Click(object sender, EventArgs e)
        {
            UserManagerHelper.IsLogin = false;
            SetIsLoginVisibilities();
        }

        void btn_Login_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(CurrentActivity);
            builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "Login_Language"));
            LinearLayout layout = new LinearLayout(CurrentActivity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            EditText txtUserName = new EditText(this.CurrentActivity);
            txtUserName.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "UserName_Language");

            EditText txtPassword = new EditText(this.CurrentActivity);
            txtPassword.Hint = ViewUtility.FindTextLanguage(CurrentActivity, "Password_Language");
            txtUserName.Text = ApplicationSetting.Current.FramesoftSetting.UserName;

            layout.AddView(txtUserName);
            layout.AddView(txtPassword);

            builder.SetView(layout);
            AlertDialog dialogW = null;

            builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "Login_Language"), (EventHandler<DialogClickEventArgs>)null);
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(CurrentActivity, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
            dialogW = builder.Create();
            dialogW.SetCancelable(false);
            dialogW.Show();

            // Get the buttons.
            var yesBtn = dialogW.GetButton((int)DialogButtonType.Negative);
            var noBtn = dialogW.GetButton((int)DialogButtonType.Positive);

            yesBtn.Click += (s, args) =>
            {
                ApplicationSetting.Current.FramesoftSetting.UserName = txtUserName.Text;
                ApplicationSetting.Current.FramesoftSetting.Password = txtPassword.Text;
                AutoLoginNow(CurrentActivity, (user) =>
                {
                    SerializeData.SaveApplicationSettingToFile();
                    string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(user.Size);
                    txtSize.Text = size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                    txtUserName.Text = user.UserName;
                    SetIsLoginVisibilities();
                }, () =>
                {
                    btn_Login_Click(null, null);
                });
                dialogW.Dismiss();
            };
            noBtn.Click += (s, args) =>
            {
                dialogW.Dismiss();
            };
        }

        void btn_Downloads_Click(object sender, EventArgs e)
        {
            ActivitesManager.VIPLinksActive(CurrentActivity);
        }

        public void BuyProduct(Product product)
        {
            try
            {
                buyProductHelper.BuyProduct(product);
            }
            catch (Exception e)
            {

            }
        }

        void btn_BuyStorage_Click(object sender, EventArgs e)
        {
            BuyStorageStart(true);
        }

        public void BuyStorageStart(bool isUser)
        {
            try
            {
                if (buyProductHelper.Products.Count == 0)
                {
                    ViewUtility.ShowMessageDialog(CurrentActivity, "CafeBazaarErrorConnection_Language", "Error_Language");
                    return;
                }
                AlertDialog.Builder builder = new AlertDialog.Builder(CurrentActivity);
                builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "RegisterUser_Language"));
                LinearLayout layout = new LinearLayout(CurrentActivity);
                layout.Orientation = Orientation.Vertical;
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

                layoutParams.SetMargins(5, 5, 5, 5);
                layout.LayoutParameters = layoutParams;


                //Button btnbuystorage100mb = new Button(this.CurrentActivity);
                //btnbuystorage100mb.Text = ViewUtility.FindTextLanguage(CurrentActivity, "buystorage100mb_Language");
                //btnbuystorage100mb.Click += (obj, ee) => { BuyProduct("buystorage100mb"); };

                //Button btnbuystorage500mb = new Button(this.CurrentActivity);
                //btnbuystorage500mb.Text = ViewUtility.FindTextLanguage(CurrentActivity, "buystorage500mb_Language");
                //btnbuystorage500mb.Click += (obj, ee) => { BuyProduct("buystorage500mb"); };

                //Button btnbuystorage1gb = new Button(this.CurrentActivity);
                //btnbuystorage1gb.Text = ViewUtility.FindTextLanguage(CurrentActivity, "buystorage1gb_Language");
                //btnbuystorage1gb.Click += (obj, ee) => { BuyProduct("buystorage1gb"); };

                //Button btnbuystorage2gb = new Button(this.CurrentActivity);
                //btnbuystorage2gb.Text = ViewUtility.FindTextLanguage(CurrentActivity, "buystorage2gb_Language");
                //btnbuystorage2gb.Click += (obj, ee) => { BuyProduct("buystorage2gb"); };

                //Button btnbuystorage5gb = new Button(this.CurrentActivity);
                //btnbuystorage5gb.Text = ViewUtility.FindTextLanguage(CurrentActivity, "buystorage5gb_Language");
                //btnbuystorage5gb.Click += (obj, ee) => { BuyProduct("buystorage5gb"); };

                //Button btnbuystorage10gb = new Button(this.CurrentActivity);
                //btnbuystorage10gb.Text = ViewUtility.FindTextLanguage(CurrentActivity, "buystorage10gb_Language");
                //btnbuystorage10gb.Click += (obj, ee) => { BuyProduct("buystorage10gb"); };

                //Button btnbuystorage20gb = new Button(this.CurrentActivity);
                //btnbuystorage20gb.Text = ViewUtility.FindTextLanguage(CurrentActivity, "buystorage20gb_Language");
                //btnbuystorage20gb.Click += (obj, ee) => { BuyProduct("buystorage20gb"); };

                //layout.AddView(btnbuystorage100mb);
                //layout.AddView(btnbuystorage500mb);
                //layout.AddView(btnbuystorage1gb);
                //layout.AddView(btnbuystorage2gb);
                //layout.AddView(btnbuystorage5gb);
                //layout.AddView(btnbuystorage10gb);
                //layout.AddView(btnbuystorage20gb);

                AlertDialog dialogW = null;
                foreach (var item in buyProductHelper.Products)
                {
                    Button btnBuy = new Button(this.CurrentActivity);
                    btnBuy.Text = ViewUtility.FindTextLanguage(CurrentActivity, item.ProductId + "_Language");
                    btnBuy.Click += (obj, ee) => { BuyProduct(item); dialogW.Dismiss(); };
                    layout.AddView(btnBuy);
                }

                builder.SetView(layout);

                builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "Cancel_Language"), (EventHandler<DialogClickEventArgs>)null);
                dialogW = builder.Create();
                dialogW.SetCancelable(false);
                dialogW.Show();

                // Get the buttons.
                var yesBtn = dialogW.GetButton((int)DialogButtonType.Negative);

                yesBtn.Click += (s, args) =>
                {
                    dialogW.Dismiss();
                };
            }
            catch (Exception ex)
            {
                try
                {
                    buyProductHelper.Disconnect();
                    buyProductHelper = null;
                    CreateBuyHelper();
                    if (isUser)
                        BuyStorageStart(false);
                    else
                    {
                        ViewUtility.ShowMessageDialog(CurrentActivity, "CafeBazaarErrorConnection_Language", "Error_Language");
                    }
                }
                catch
                {

                }
            }
        }

        void BuyPurchases(List<Purchase> purchases)
        {
            if (purchases.Count == 0)
                return;
            var progressDialog = ProgressDialog.Show(CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "BuyPurchase_Language"), ViewUtility.FindTextLanguage(CurrentActivity, "BuyingPurchase_Language"), true, false);
            Action<string> errorRegister = (msg) =>
            {
                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "BuyPurchaseError_Language") + ": " + System.Environment.NewLine + msg);
                });
                System.Threading.Thread.Sleep(2000);
            };

            AsyncActions.Action(() =>
            {
                bool isOK = false;
                foreach (var purchase in purchases)
                {
                    var buyStorage = UserManagerHelper.BuyStorageFromUser(new Framesoft.UserPurchaseData() { UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash(), ProductId = purchase.ProductId, PurchaseTime = purchase.PurchaseTime, PurchaseToken = purchase.PurchaseToken });
                    if (buyStorage.Message == "OK")
                    {
                        isOK = true;
                        CurrentActivity.RunOnUiThread(() =>
                        {
                            progressDialog.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "BuyStorageSuccess_Language"));
                            buyProductHelper.ConsumePurchase(purchase);
                        });
                        Thread.Sleep(1000);
                        CurrentActivity.RunOnUiThread(() =>
                        {
                            progressDialog.SetMessage(ViewUtility.FindTextLanguage(CurrentActivity, "GettingUserInfo_Language"));
                        });
                    }
                    else
                    {
                        errorRegister(ViewUtility.FindTextLanguage(CurrentActivity, UserManagerHelper.GetServerResponseMessageValue(buyStorage.Message, "")));
                    }
                }
                if (isOK)
                {
                    var user = UserManagerHelper.GetUserInfo(new UserInfoData() { UserName = ApplicationSetting.Current.FramesoftSetting.UserName, Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash() });
                    CurrentActivity.RunOnUiThread(() =>
                    {
                        if (user.Data != null)
                        {
                            string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(user.Data.Size);
                            txtSize.Text = size[0] + " " + ViewUtility.FindTextLanguage(CurrentActivity, size[1] + "_Language");
                            txtUserName.Text = user.Data.UserName;
                            currentUserInfoData = user.Data;
                            SetIsLoginVisibilities();
                        }
                    });
                }

                CurrentActivity.RunOnUiThread(() =>
                {
                    progressDialog.Dismiss();
                    progressDialog.Hide();
                });
            }, (error) =>
            {
                errorRegister(error.Message);
            });
        }
    }
}