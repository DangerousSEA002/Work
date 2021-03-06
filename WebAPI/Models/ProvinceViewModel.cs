using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class ProvinceViewModel
    {
        public long province_id { get; set; }
        public long country_id { get; set; }
        public string seo_alias { get; set; }
        public string name { get; set; }
        public string seo_title { get; set; }
        public string seo_description { get; set; }
        public virtual IEnumerable<ApplicationUserViewModel> employees { get; set; }
        public virtual IEnumerable<JobViewModel> jobs { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool status { get; set; }
    }
}