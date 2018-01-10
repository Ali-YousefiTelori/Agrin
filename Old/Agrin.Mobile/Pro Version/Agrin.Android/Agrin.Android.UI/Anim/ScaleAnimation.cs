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
using SlidingMenuSharp;

namespace Agrin.Android.UI.Anim
{
    [Activity(Label = "Scale Animation", Theme = "@style/Theme.AppCompat")]
    public class ScaleAnimation : CustomAnimation
    {
        public ScaleAnimation()
            : base(Resource.String.anim_scale)
        {
            Transformer = new ScaleTransformer();
        }
    }
}