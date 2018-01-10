
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

namespace Agrin.MonoAndroid.UI
{
	[Activity (Label = "ExceptionActivity")]			
	public class ExceptionActivity : Activity
	{
		public static Exception exception;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.MessageBox);
			EditText textBox = FindViewById<EditText> (Resource.Id.TxTmessageBox);
			string stack = "";
			if (exception == null) {
				textBox.Text = "Error!";
				return;
			}
			if (exception.StackTrace != null)
				stack = exception.StackTrace;

			textBox.Text = "لطفاً این پیغام خطا را به ما گزارش دهید: " + System.Environment.NewLine + exception.Message + " Stack: " + stack;

		}
	}
}

