using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiceApi.Pages
{
    public class BrowseModel : PageModel
    {
        public bool? Create { get; set; }
        public void OnGet(bool? create)
        {
            Create = create;
        }
    }
}