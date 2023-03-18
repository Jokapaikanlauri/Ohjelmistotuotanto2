// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MatkakertomusGroupB.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace MatkakertomusGroupB.Server.Areas.Identity.Pages.Account.Manage
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<Traveller> _userManager;
		private readonly SignInManager<Traveller> _signInManager;

		public IndexModel(
			UserManager<Traveller> userManager,
			SignInManager<Traveller> signInManager)
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

			// Added in accordance with https://learn.microsoft.com/en-us/aspnet/core/security/authentication/add-user-data?view=aspnetcore-7.0&tabs=netcore-cli
			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Forename")]
			public string Forename { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Surname")]
			public string Surname { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Nickname")]
			public string Nickname { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Municipality")]
			public string Municipality { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Description")]
			public string Description { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Image")]
			public string Image { get; set; }

			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }
		}

		private async Task LoadAsync(Traveller user)
		{
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			Username = userName;

			Input = new InputModel
			{
				// Added in accordance with https://learn.microsoft.com/en-us/aspnet/core/security/authentication/add-user-data?view=aspnetcore-7.0&tabs=netcore-cli
				Forename = user.Forename,
				Surname = user.Surname,
				Nickname = user.Nickname,
				Municipality = user.Municipality,
				Description = user.Description,
				Image = user.Image,
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

			// Added in accordance with https://learn.microsoft.com/en-us/aspnet/core/security/authentication/add-user-data?view=aspnetcore-7.0&tabs=netcore-cli
			if (Input.Forename != user.Forename)
			{
				user.Forename = Input.Forename;
			}
			if (Input.Surname != user.Surname)
			{
				user.Surname = Input.Surname;
			}
			if (Input.Nickname != user.Nickname)
			{
				user.Nickname = Input.Surname;
			}
			if (Input.Municipality != user.Municipality)
			{
				user.Municipality = Input.Municipality;
			}
			if (Input.Description != user.Description)
			{
				user.Description = Input.Description;
			}
			if (Input.Image != user.Image)
			{
				user.Image = Input.Image;
			}
			await _userManager.UpdateAsync(user);


			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}
