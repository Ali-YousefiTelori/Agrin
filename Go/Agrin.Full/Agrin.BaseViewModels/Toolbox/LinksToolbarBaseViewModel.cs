using Agrin.BaseViewModels.Lists;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Toolbox
{
    public class LinksToolbarBaseViewModel : ANotifyPropertyChanged
    {
        public virtual IEnumerable<LinkInfo> GetSelectedLinks()
        {
            return null;
        }

        public bool CanPlayLinks()
        {
            foreach (var item in GetSelectedLinks())
            {
                if (item.CanPlay)
                    return true;
            }

            return false;
        }

        public void PlayLinks()
        {
            foreach (var item in GetSelectedLinks())
            {
                if (item.CanPlay)
                    LinksBaseViewModel.PlayLinkInfo(item);
            }
        }

        public bool CanStopLinks()
        {
            foreach (var item in GetSelectedLinks())
            {
                if (item.CanStop)
                    return true;
            }

            return false;
        }

        public void StopLinks()
        {
            foreach (var item in GetSelectedLinks())
            {
                if (item.CanStop)
                    LinksBaseViewModel.StopLinkInfo(item);
            }
        }

        public bool CanDeleteLinks()
        {
            foreach (var item in GetSelectedLinks())
            {
                if (item.CanDelete)
                    return true;
            }

            return false;
        }

        public virtual void DeleteLinks()
        {
            List<LinkInfo> deleteItems = new List<LinkInfo>();
            foreach (var item in GetSelectedLinks())
            {
                if (item.CanDelete)
                    deleteItems.Add(item);
            }
            ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(deleteItems);
        }

        public virtual void SettingLinks()
        {

        }
    }
}
