﻿using Agrin.Download.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data
{
    [Serializable]
    public class AgrinApplicationNoticeSerialize
    {
        public List<NoticeInfo> Items { get; set; }
    }
}
