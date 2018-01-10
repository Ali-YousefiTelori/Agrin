using Agrin.ViewModels.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Agrin.OldViews.Link
{
    public partial class AddLink : ContentPage
    {
        public AddLink()
        {
            InitializeComponent();

            //var a = new Animation();
            //a.Add(0, 0.5, new Animation(f => view.Scale = f, 1, 1.2, Easing.Linear, null));
            //a.Add(0.5, 1, new Animation(f => view.Scale = f, 1.2, 1, Easing.Linear, null));
            //a.Commit(view, "ScaleTo", 16, 300, null, null, () => true);

            //Grid grid = null;
            //grid.Animate<double>("Opacity", (a) => a + 0.1, (callback) =>
            //{

            //}, repeat: () => true);
            //var animate = new Animation(d => grid.Opacity = d, 0, 1);
            //animate.Commit(grid, "Opacity", 16, 3500, repeat: () => true);

            var vm = this.BindingContext as AddLinksViewModel;
            vm.UriAddress = "http://ali.com/";
            groupsCombo.Items.Add(Agrin.Download.Manager.ApplicationGroupManager.Current.NoGroup.Name);
            foreach (var item in vm.Groups)
            {
                groupsCombo.Items.Add(item.Name);
            }
            groupsCombo.SelectedIndex = 0;
        }
    }
}
