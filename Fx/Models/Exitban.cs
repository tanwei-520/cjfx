using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Fx.Models
{
    public class Exitban
    {
        public int id { get; set; }
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "名称在2-20字符间")]
        public string name { get; set; }
        [StringLength(20, MinimumLength = 0, ErrorMessage = "备注在0-20字符间")]
        public string text { get; set; }
    }
}