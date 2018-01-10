using System;
using System.Collections.Generic;
using Agrin.Download.Web;

namespace Agrin.Mono.UI
{
	public class LinksBaseViewModel : Agrin.BaseViewModels.Lists.LinksBaseViewModel
	{
		Func<IEnumerable<LinkInfo>> _getSelections = null;

		public LinksBaseViewModel (Func<IEnumerable<LinkInfo>> getSelections)
		{
			_getSelections = getSelections;
		}

		public override IEnumerable<LinkInfo> GetSelectedItems ()
		{
			return _getSelections();
		}
	}
}

