using System;

namespace Agrin.MonoAndroid.UI
{
	public class HolderHelper<T> : Java.Lang.Object {
		public readonly T Value;

		public HolderHelper (T value)
		{
			this.Value = value;
		}
	}
}

