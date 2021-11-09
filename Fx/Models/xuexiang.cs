using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Fx.Models
{
    public class xuexiang
    {
        [Required(ErrorMessage = "登录账号不能为空")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "学号在6-12字符间")]
        public string id { get; set; }
        [Required(ErrorMessage = "登录账号不能为空")]
        [StringLength(12, MinimumLength = 2, ErrorMessage = "在2-12字符间")]
        public string name { get; set; }
        public string sex { get; set; }
        public DateTime bir { get; set; }
    }
}