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
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(
			UserManager<Traveller> userManager,
			SignInManager<Traveller> signInManager,
			ILogger<IndexModel> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_logger.LogInformation("IndexModel constructor called.");
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
			[Display(Name = "Etunimi")]
			public string Forename { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Sukunimi")]
			public string Surname { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Nimimerkki")]
			public string Nickname { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Paikkakunta")]
			public string Municipality { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Kuvaus")]
			public string Description { get; set; }

			//[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Kuva")]
			public string Image { get; set; }

			[Phone]
			[Display(Name = "Puhelinnumero")]
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
				// Add required translations to get an image object that can be refferred in the .cshtml, 
				// Also remember to add required image => binary translation in OnPostAsync()
				//Image = "I'm a Teapot",
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
			//_logger.LogInformation("OnPostAsync method was called.");
			var user = await _userManager.GetUserAsync(User);
			//_logger.LogInformation($"OnPostAsync User: {user}");
			if (user == null)
			{
				//_logger.LogWarning("OnPostAsync user was null");
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				//_logger.LogWarning("OnPostAsync model state was not valid.");
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					//_logger.LogError(error.ErrorMessage);
				}
				await LoadAsync(user);
				return Page();
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			//_logger.LogInformation($"OnPostAsync phoneNumber: {phoneNumber}");
			if (Input.PhoneNumber != phoneNumber)
			{
				//_logger.LogInformation("OnPostAsync phone number was different");
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded)
				{
					//_logger.LogWarning("OnPostAsync phone number change failed");
					StatusMessage = "Odottamaton virhe päivittäessä puhelinnumeroasi";
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
				user.Nickname = Input.Nickname;
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
			//_logger.LogInformation("OnPostAsync all properties were set according to changes");
			await _userManager.UpdateAsync(user);

			//_logger.LogInformation("OnPostAsync changes were updated to user");
			await _signInManager.RefreshSignInAsync(user);

			//_logger.LogInformation("OnPostAsync user was refreshed");
			StatusMessage = "Profiilisi on päivitetty";

			//_logger.LogInformation("OnPostAsync profile was refreshed and message delivered, returning to page");
			return RedirectToPage();
		}
	}
}
