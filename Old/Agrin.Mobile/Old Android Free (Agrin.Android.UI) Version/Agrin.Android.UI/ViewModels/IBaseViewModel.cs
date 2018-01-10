using System;
using Android.App;

namespace Agrin.MonoAndroid.UI
{
	public interface IBaseViewModel
	{ 
		Activity CurrentActivity{ get; set; }
	}
}

