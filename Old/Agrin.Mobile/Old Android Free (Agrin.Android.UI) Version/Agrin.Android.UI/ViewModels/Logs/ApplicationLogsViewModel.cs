using System;
using System.Text;
using Android.Widget;
using Android.App;

namespace Agrin.MonoAndroid.UI
{
	public class ApplicationLogsViewModel:IBaseViewModel
	{
		#region IBaseViewModel implementation

		public Activity CurrentActivity { get; set; }

		#endregion
		public void Initialize (Activity currentActivity)
		{
			CurrentActivity = currentActivity;
			//LogAll ();
			Button button = CurrentActivity.FindViewById<Button> (Resource.ApplicationLog.btn_All);
			button.Click += btnLogAllClick;
			button = CurrentActivity.FindViewById<Button> (Resource.ApplicationLog.btn_ErrorLogs);
			button.Click += btnErrorLogClick;
			button = CurrentActivity.FindViewById<Button> (Resource.ApplicationLog.btn_ClearLogs);
			button.Click += btnClearClick;
			EditText txtLog = CurrentActivity.FindViewById<EditText> (Resource.ApplicationLog.TxTmessageBox);
            LogAll();
		}

		public ApplicationLogsViewModel (Activity currentActivity)
		{
			Initialize (currentActivity);
		}

		void btnLogAllClick (object sender, EventArgs e)
		{
			LogAll ();
		}

		void btnErrorLogClick (object sender, EventArgs e)
		{
			LogAll (true);
		}

		void btnClearClick (object sender, EventArgs e)
		{
			//Agrin.Log.AutoLogger.ClearLogs ();
			LogAll ();
		}

		public void LogAll (bool isError = false)
		{
			EditText txtLog = CurrentActivity.FindViewById<EditText> (Resource.ApplicationLog.TxTmessageBox);
            //StringBuilder builder = new StringBuilder ();
            //foreach (var item in Agrin.Log.AutoLogger.GetLogsForSave().Logs) {
            //    if (isError) {
            //        if (item.Values != null) {
            //            bool finded = false;
            //            foreach (var valItem in item.Values) {
            //                if (valItem != null && valItem.ToLower ().Contains ("exception")) {
            //                    finded = true;
            //                    break;
            //                }
            //            }
            //            if (!finded)
            //                continue;
            //        } else
            //            continue;
            //    }
            //    builder.Append (item.FullClassName);
            //    builder.AppendLine (item.Id.ToString ());
            //    builder.AppendLine (item.MethodName);
            //    builder.AppendLine (item.Mode.ToString ());
            //    builder.AppendLine ("Values");
            //    foreach (var value in item.Values) {
            //        builder.AppendLine (value);
            //    }
            //    builder.AppendLine ("-------END OF Item-------");
            //}
            string path = System.IO.Path.Combine(Agrin.Log.AutoLogger.ApplicationDirectory, "Error Logs.log");
            if (System.IO.File.Exists(path))
                txtLog.Text = System.IO.File.ReadAllText(path);
            else
                txtLog.Text = "No Log Found";
		}
	}
}

