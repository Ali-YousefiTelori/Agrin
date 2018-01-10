using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Lists
{
    public class GroupsBaseViewModel : ListBaseViewModel<GroupInfo>
    {
        public FastCollection<GroupInfo> Items
        {
            get
            {
                if (ApplicationGroupManager.Current == null)
                    return null;
                return ApplicationGroupManager.Current.GroupInfoes;
            }
        }

        public virtual void AddGroup()
        {

        }

        public virtual void DeleteGroups()
        {
            ApplicationGroupManager.Current.DeleteRangeGroupInfo(GetSelectedItems().ToList());
        }

        public bool CanDeleteGroups()
        {
            return GetSelectedItems().Count() > 0;
        }
    }
}
