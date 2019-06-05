using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiceApi.Dtos;
using DiceApi.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiceApi.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}