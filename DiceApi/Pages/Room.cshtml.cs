using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiceApi.Pages
{
    public class RoomModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public void OnGet()
        {
            ViewData["id"] = Id;
        }
    }
}