using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.Models.Filters
{
    /// <summary>
    /// کلاس بیس فیلتر که برای همه فیلتر ها مشترک است
    /// </summary>
    public class FilterBaseInfo
    {
        /// <summary>
        /// شروع صفحه
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// تاریخ شروع
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// تاریخ پایان
        /// </summary>
        public DateTime? EndDateTime { get; set; }
    }
}
