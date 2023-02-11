// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Matkakertomus_webapp_GroupB.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Matkakertomus_webapp_GroupB.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Matkaaja> _userManager;
        private readonly SignInManager<Matkaaja> _signInManager;

        public IndexModel(
            UserManager<Matkaaja> userManager,
            SignInManager<Matkaaja> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 

            //Required modifications:
            //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/add-user-data?view=aspnetcore-7.0&tabs=visual-studio
            [Required]
            [Display(Name = "Etunimi")]
            [DataType(DataType.Text)]
            public string Etunimi { get; set; }

            [Required]
            [Display(Name = "Sukunimi")]
            [DataType(DataType.Text)]
            public string Sukunimi { get; set; }

            [Required]
            [Display(Name = "Paikkakunta")]
            [DataType(DataType.Text)]
            public string Paikkakunta { get; set; }

            [Required]
            [Display(Name = "Esittely")]
            [DataType(DataType.Text)]
            public string Esittely { get; set; }

            [Required]
            [Display(Name = "Kuva")]
            [DataType(DataType.Text)]
            public string Kuva { get; set; }


            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(Matkaaja user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Etunimi = user.Etunimi,
                Sukunimi = user.Sukunimi,
                Paikkakunta = user.Paikkakunta,
                Esittely = user.Esittely,
                Kuva = user.Kuva,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.Etunimi != user.Etunimi)
            {
                user.Etunimi = Input.Etunimi;
            }
            if (Input.Sukunimi != user.Sukunimi)
            {
                user.Sukunimi = Input.Sukunimi;
            }
            if (Input.Paikkakunta != user.Paikkakunta)
            {
                user.Paikkakunta = Input.Paikkakunta;
            }
            if (Input.Esittely != user.Esittely)
            {
                user.Esittely = Input.Esittely;
            }
            if (Input.Kuva != user.Kuva)
            {
                user.Kuva = Input.Kuva;
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
