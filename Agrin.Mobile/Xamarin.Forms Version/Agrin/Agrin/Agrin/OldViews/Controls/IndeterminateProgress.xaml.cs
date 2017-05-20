using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Agrin.OldViews.Controls
{
    public partial class IndeterminateProgress : ContentView
    {
        public IndeterminateProgress()
        {
            InitializeComponent();

            try
            {
                Action playAnimation = null;
                playAnimation = () =>
                {
                    var parentAnimation = new Animation();
                    var scaleUpAnimation = new Animation(v =>
                    {
                        grid1.Opacity = v;
                    }, 0.0, 1.0, Easing.Linear);
                    var scaleDownAnimation = new Animation(v =>
                    {
                        grid1.Opacity = v;
                    }, 1.0, 0.0, Easing.Linear);

                    parentAnimation.Add(0.0, 0.5, scaleUpAnimation);
                    parentAnimation.Add(0.5, 1.0, scaleDownAnimation);

                    parentAnimation.Commit(grid1, "Opacity", 10, 2000, null, finished: (v, v2) =>
                    {
                        playAnimation();
                    });
                };
                playAnimation();
            }
            catch (Exception ex)
            {

            }
            //var animate = new Animation();
            //animate.Add(0, 1, new Animation(d => grid1.Opacity = d, 0, 1));
            //animate.Add(1, 0, new Animation(d => grid1.Opacity = d, 1, 0));
            //animate.Commit(grid1, "Opacity", 16, 3500, repeat: () => true);

            //grid1.Animate<double>("Opacity", (a) => 0.1, (callback) =>
            //{

            //}, repeat: () => true);
        }
    }
}
