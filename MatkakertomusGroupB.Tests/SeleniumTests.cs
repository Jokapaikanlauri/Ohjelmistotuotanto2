using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Diagnostics;
using OpenQA.Selenium.Support.UI;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using NUnit.Framework;
using MatkakertomusGroupB.Client.Pages;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Xml.Linq;

namespace MatkakertomusGroupB.Tests
{
	//https://docs.nunit.org/articles/nunit/writing-tests/attributes/testfixture.html
	/*
	 Beginning with NUnit 2.5, the TestFixture attribute is optional 
	for non-parameterized, non-generic fixtures. 
	So long as the class contains at least one method marked with the 
	Test, TestCase or TestCaseSource attribute, it will be treated as a test fixture.
	 */
	[TestFixture]

	public class SeleniumTests
	{
		private IWebDriver _webDriver;
		private Process _webServerProcess;
		private string _baseUrl { get; set; } = "https://localhost:7012";

		private bool debuggingMessagesEnabled = false;
		private bool extraDelayEnabled = true;
		private int extraDelayInMilliSeconds = 500;
		private int waitB4FailSeconds = 5;

		//Data for images
		private string destImagePath { get; set; }
		private string destImagePathEdited { get; set; }
		private string pictureImagePath { get; set; }
		private string pictureImagePath2 { get; set; }
		private string userImagePath { get; set; }


		//Data for User out data
		private string userForename { get; set; }
		private string userSurname { get; set; }
		private string userNickname { get; set; }
		private string userEmail { get; set; }
		private string userPassword { get; set; }
		private string userMunicipality { get; set; }
		private string userDescription { get; set; }
		private string userPhoneNumber { get; set; }

		//Data for Destination
		private string destName { get; set; }
		private string destCountry { get; set; }
		private string destMunicipality { get; set; }
		private string destDescription { get; set; }

		//Data for trip
		private string tripPubStartDate { get; set; }
		private string tripPubEndDate { get; set; }
		private string tripPrivStartDate { get; set; }
		private string tripPrivEndDate { get; set; }

		//Data for Story
		private string storyDate { get; set; }
		private string storyDescription { get; set; }


		private static string GenerateRandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			var sb = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				sb.Append(chars[random.Next(chars.Length)]);
			}
			return sb.ToString();
		}

		string randomString = GenerateRandomString(300);



		//READ ME: These tests are designed to be run in a certain order and sometimes the initially assigned values
		//are changed during execution during the testing of edition of details(traveller, trip, story, dest etc)
		[OneTimeSetUp]

		public void SetUp()
		{

			// Get the solution directory path
			var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory));

			// Build the path to the web project
			var webProjectPath = solutionDir.Replace(".Tests\\bin", "\\Server");

			//Generate the picture paths for all the tests
			//Define get the running folder of current process(TEST)
			//Should equate to (you repo)/ET21KM-Ohjelmistotuotanto2-GroupB\MatkakertomusGroupB.Tests\bin\Debug\net7.0\
			string processFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
			// Build the path to the correct pictures
			this.destImagePath = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testDestPicture.png");
			this.destImagePathEdited = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testDestPicture-Edited.png");
			this.pictureImagePath = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testPicture.png");
			this.pictureImagePath2 = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testPicture_2.png");
			this.userImagePath = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testUserPicture.png");



			if (debuggingMessagesEnabled)
			{
				Console.WriteLine(destImagePath);
				Console.WriteLine(pictureImagePath);
				Console.WriteLine(userImagePath);
			}

			Random random = new Random();
			int randomNumber = random.Next(100000, 999999);

			//Generate Random data for User out data
			this.userForename = $"Test-{nameof(userForename)}-{randomNumber.ToString()}";
			this.userSurname = $"Test-{nameof(userSurname)}-{randomNumber.ToString()}";
			this.userNickname = $"Test-{nameof(userNickname)}-{randomNumber.ToString()}";
			this.userEmail = $"Test-{nameof(userEmail)}-{randomNumber.ToString()}@Bingostan.com";
			this.userPassword = $"Test-{nameof(userPassword)}-{randomNumber.ToString()}";
			this.userMunicipality = $"Test-{nameof(userMunicipality)}-{randomNumber.ToString()}";
			this.userDescription = $"Test-{nameof(userDescription)}-{randomNumber.ToString()}";
			this.userPhoneNumber = $"{randomNumber.ToString()}{randomNumber.ToString()}";

			//Generate random data for Destination
			this.destName = $"Test-{nameof(destName)}-{randomNumber.ToString()}";
			this.destCountry = $"Test-{nameof(destCountry)}-{randomNumber.ToString()}";
			this.destMunicipality = $"Test-{nameof(destMunicipality)}-{randomNumber.ToString()}";
			this.destDescription = $"Test-{nameof(destDescription)}-{randomNumber.ToString()}";

			//Generate random data for Trip
			// Start 2-400 before current, end 2-400 after current
			DateTime currentDate = DateTime.Now.Date;
			int dateRandomizer = new Random().Next(2, 400);
			this.tripPubStartDate = currentDate.AddDays(-dateRandomizer).ToString("dd/MM/yyyy");
			this.tripPubEndDate = currentDate.AddDays(dateRandomizer).ToString("dd/MM/yyyy");
			this.tripPrivStartDate = currentDate.AddDays(-dateRandomizer - 20).ToString("dd/MM/yyyy");
			this.tripPrivEndDate = currentDate.AddDays(dateRandomizer + 20).ToString("dd/MM/yyyy");


			//Generate random data for Story
			this.storyDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
			storyDescription = $"Random Story: {GenerateRandomString(60)}, " +
				$"POTATOOO, " +
				$"{GenerateRandomString(60)}, " +
				$"{GenerateRandomString(60)}.";

			try
			{
				// Start the web server
				_webServerProcess = Process.Start("dotnet", $"run --project {webProjectPath}\\MatkakertomusGroupB.Server.csproj");

				// Output for debug
				//Console.WriteLine("dotnet");
				//Console.WriteLine($"run --project {webProjectPath}\\MatkakertomusGroupB.Server.csproj");

				// Wait for the server to start up
				Thread.Sleep(7000);

				// Create a new Selenium WebDriver manager instance
				new DriverManager().SetUpDriver(new ChromeConfig());

				// Create Chrome options
				var chromeOptions = new ChromeOptions();

				// Set the "ignore-certificate-errors" flag to bypass the SSL cert failure
				chromeOptions.AddArgument("--ignore-certificate-errors");

				// Create a new Selenium WebDriver
				_webDriver = new ChromeDriver(chromeOptions);
			}
			catch (Exception ex)
			{
				// Output an error message and fail the test if the server fails to start
				Console.WriteLine($"Failed to start the server: {ex.Message}");
				Assert.Fail();
			}

			//Global Implicit wait before failing (waits for element to be found)
			_webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

			// Wait for the server to start up
			Thread.Sleep(5);
		}

		/// <summary>
		/// Cleans up after tests
		/// </summary>
		[OneTimeTearDown]
		public void TearDown()
		{
			_webDriver.Quit();
			_webDriver.Dispose();
			_webServerProcess.Kill();
		}
		[Test, Order(1)]
		public void Register_New_User()
		{
			//Navigate to specific URL
			_webDriver.Navigate().GoToUrl(_baseUrl);

			//Wait until a specific element is found(timeout defined in global ImplicitWait
			_webDriver.FindElement(By.PartialLinkText("Register")).Click();




			//Fill out registration form
			_webDriver.FindElement(By.Id("Input_Forename")).SendKeys(userForename);
			_webDriver.FindElement(By.Id("Input_Surname")).SendKeys(userSurname);
			_webDriver.FindElement(By.Id("Input_Nickname")).SendKeys(userNickname);
			_webDriver.FindElement(By.Id("Input_Email")).SendKeys(userEmail);
			_webDriver.FindElement(By.Id("Input_Password")).SendKeys(userPassword);
			_webDriver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(userPassword);
			//Proceed
			_webDriver.FindElement(By.Id("registerSubmit")).Click();

			//Should re-route to main page
			//User must see his NickName on the main page
			string loginDisplayHTML = _webDriver.FindElement(By.Id("nick-display")).GetAttribute("innerHTML");
			string expected = userNickname;
			Assert.True(loginDisplayHTML.Contains(expected), $"Expected login box to contain userNickname \"{expected}\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");
			Assert.False(loginDisplayHTML.Contains("login"), $"Expected login box to not \"login\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");

		}

		[Test, Order(2)]
		public void Edit_User_Details()
		{

			//Navigate to user details management and add the rest of the information
			//My Information
			string linkText = "My Information";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			string actual = elem.GetAttribute("href").ToString();
			var expected = "authentication/profile";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu my information link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			string keyElemId = "identitypage-index";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			//Edit Existing data in the USERDATA form
			userForename = "Edited-" + userForename;
			userSurname = "Edited-" + userSurname;
			userNickname = "Edited-" + userNickname;
			var formInput = _webDriver.FindElement(By.Id("Input_Forename"));
			formInput.Clear();
			formInput.SendKeys(userForename);
			formInput = _webDriver.FindElement(By.Id("Input_Surname"));
			formInput.Clear();
			formInput.SendKeys(userSurname);
			formInput = _webDriver.FindElement(By.Id("Input_Nickname"));
			formInput.Clear();
			formInput.SendKeys(userNickname);


			//Fill out the missing bits in the USERDATA form
			_webDriver.FindElement(By.Id("Input_Municipality")).SendKeys(userMunicipality);
			_webDriver.FindElement(By.Id("Input_Description")).SendKeys(userDescription);
			_webDriver.FindElement(By.Id("Input_PhoneNumber")).SendKeys(userPhoneNumber);

			//Get file element and input the path to set the picture
			var inputFileElement = _webDriver.FindElement(By.CssSelector("input[type='file']"));
			inputFileElement.SendKeys(userImagePath);

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Proceed
			_webDriver.FindElement(By.Id("update-profile-button")).Click();

			//The OK message should be displayed
			keyElemId = "user-update-messagebox";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			var keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "Your profile has been updated";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}


			//Navigate back to Main Page
			linkText = "Return to Main Page";
			elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			keyElemId = "index-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(3)]
		public void First_Logout()
		{
			Thread.Sleep(100);
			_webDriver.FindElement(By.Id("logout_button")).Click();

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(4)]
		public void Re_LogIn()
		{
			//Navigate to specific URL
			_webDriver.Navigate().GoToUrl(_baseUrl);

			//TODO: Add an logout if logged in
			//_webDriver.FindElement(By.Id("logout_button")).Click()

			//Wait until a specific element is found(timeout defined in global ImplicitWait
			_webDriver.FindElement(By.PartialLinkText("Log in")).Click();
			//Or wait a specific time
			//Thread.Sleep(5000);

			//https://www.browserstack.com/guide/sendkeys-in-selenium
			//On the login page fill out the form and proceed
			_webDriver.FindElement(By.Id("Input_Email")).SendKeys(userEmail);
			_webDriver.FindElement(By.Id("Input_Password")).SendKeys(userPassword);
			_webDriver.FindElement(By.Id("login-submit")).Click();



			//User must see his NickName
			string loginDisplayHTML = _webDriver.FindElement(By.Id("nick-display")).GetAttribute("innerHTML");
			string expected = userNickname;
			Assert.True(loginDisplayHTML.Contains(expected), $"Expected login box to contain userNickname \"{expected}\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");
			Assert.False(loginDisplayHTML.Contains("login"), $"Expected login box to not \"login\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(5)]
		public void Navmenu_and_Links()
		{
			//Test Nav menu contents
			//Nav to pages
			//Koti-, Matkakohde-, Porukan matkat-, Omat matkat-, Omat tiedot-, Jäsenet-sivut


			//Destinations
			string linkText = "Destinations";
			//Find the correct anchor <a> item via link text
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			//Get the href of the <a>
			string actual = elem.GetAttribute("href").ToString();
			string expected = "destinations";
			//Compare the previous to be as expected
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu destinations link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			//Use the found link to navigate to said page
			elem.Click();
			//Expect to find page content
			string keyElemId = "destinations-razor-auth-listing";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Home
			linkText = "Home";
			elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			actual = elem.GetAttribute("href").ToString();
			expected = "";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu home link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			keyElemId = "index-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Group's Trips
			linkText = "Group's Trips";
			elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			actual = elem.GetAttribute("href").ToString();
			expected = "grouptrips";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu group trips link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			keyElemId = "grouptriplist-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Own Trips
			linkText = "My Trips";
			elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			actual = elem.GetAttribute("href").ToString();
			expected = "trips";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu my trips link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			keyElemId = "owntrips-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//My Information
			linkText = "My Information";
			elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			actual = elem.GetAttribute("href").ToString();
			expected = "authentication/profile";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu my information link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			keyElemId = "identitypage-index";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");
			//Return back to previous page
			_webDriver.Navigate().Back();

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Other Travellers
			linkText = "Travellers";
			elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			actual = elem.GetAttribute("href").ToString();
			expected = "travellerlist";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu travellers link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			keyElemId = "traveller-listing-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Verify that nav menu has only 6 links
			elem = _webDriver.FindElement(By.Id("navmenu-auth"));
			var navmenuHTML = elem.GetAttribute("innerHTML");
			int regexMatches = Regex.Matches(navmenuHTML, "<a href").Count();
			Assert.True(regexMatches == 6, $"Expected nav menu to contain 6 links, but it contained {regexMatches} links.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}

		[Test, Order(6)]
		public void TravellerList_OK()
		{

			//Navigate to Travellers listing
			string linkText = "Travellers";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			string actual = elem.GetAttribute("href").ToString();
			string expected = "travellerlist";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu travellers link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content for just created user
			string keyElemId = $"traveller-listing-item-{userNickname}";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			//Store for later use
			var listingHTML = keyElem.GetAttribute("innerHTML");

			//Convert element contents to string and see if the added item exists
			actual = keyElem.Text.ToString();
			expected = userForename;
			Assert.True(actual.Contains(expected), $"Expected traveller listing to contain \"{expected}\", but it wasn't found as text. Listing was: \"{actual}\"");
			expected = userSurname;
			Assert.True(actual.Contains(expected), $"Expected traveller listing to contain \"{expected}\", but it wasn't found as text. Listing was: \"{actual}\"");
			expected = userNickname;
			Assert.True(actual.Contains(expected), $"Expected traveller listing to contain \"{expected}\", but it wasn't found as text. Listing was: \"{actual}\"");
			expected = userMunicipality;
			Assert.True(actual.Contains(expected), $"Expected traveller listing to contain \"{expected}\", but it wasn't found as text. Listing was: \"{actual}\"");
			expected = userDescription;
			Assert.True(actual.Contains(expected), $"Expected traveller listing to contain \"{expected}\", but it wasn't found as text. Listing was: \"{actual}\"");



			//Get element
			keyElem = _webDriver.FindElement(By.CssSelector("img[src^='data:image/png;base64']"));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find listing item with a one picture but it wasn't found.");

			//Verify that listing only has 1 picture
			int regexMatches = Regex.Matches(listingHTML, "<img src").Count();
			Assert.True(regexMatches == 1, $"Expected user to have 1 picture, but it contained {regexMatches} pictures.");

			//Expect to NOT to find password and email anywhere in the listing.
			expected = userPassword;
			Assert.False(listingHTML.Contains(expected), $"Expected traveller listing to NOT contain \"{expected}\", but it wasn found as. Listing was: \"{actual}\"");
			expected = userEmail;
			Assert.False(listingHTML.Contains(expected), $"Expected traveller listing to NOT contain \"{expected}\", but it wasn found as. Listing was: \"{actual}\"");



			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(7)]
		public void Add_Destination_Item()
		{
			//Navigate to Destinations, wait for add element to be enabled
			string linkText = "Destinations";
			//Find the correct anchor <a> item via link text
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			//Get the href of the <a>
			string actual = elem.GetAttribute("href").ToString();
			string expected = "destinations";
			//Compare the previous to be as expected
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu destinations link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			//Use the found link to navigate to said page
			elem.Click();
			//Expect to find page content
			string keyElemId = "destinations-razor-add";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");

			//Fill out the form
			_webDriver.FindElement(By.Id("Input_Destination_Name")).SendKeys(destName);
			_webDriver.FindElement(By.Id("Input_Destination_Country")).SendKeys(destCountry);
			_webDriver.FindElement(By.Id("Input_Destination_Municipality")).SendKeys(destMunicipality);
			_webDriver.FindElement(By.Id("Input_Destination_Description")).SendKeys(destDescription);

			//Get file element and input the path to set the picture
			var inputFileElement = _webDriver.FindElement(By.CssSelector("input[type='file']"));
			inputFileElement.SendKeys(destImagePath);

			//Proceed
			_webDriver.FindElement(By.Id("addSubmit")).Click();


			//The OK message should be displayed
			keyElemId = "destination-added-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "Destination was added successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(8)]
		public void Added_Destination_Item_Exists()
		{

			//Expect to find the added content in the list
			string keyElemId = "destinations-razor-auth-listing";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Convert element contents to string and see if the added item exists
			string actual = keyElem.Text.ToString();
			string expected = destName;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = destCountry;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = destMunicipality;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = destDescription;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");



			//test for picturelement
			string keyElemHTML = keyElem.GetAttribute("innerHTML").ToString();
			expected = $"destpic-{destName}";
			var welcomepic = _webDriver.FindElement(By.Id(expected));
			Assert.AreEqual(true, welcomepic.Displayed, $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{keyElemHTML}\"");





			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(9)]
		public void Add_publicTrip_Item()
		{
			//Navigate to Own Trips, wait for add element to be enabled
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			string actual = elem.GetAttribute("href").ToString();
			string expected = "trips";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu my trips link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			string keyElemId = "trip-razor-add";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");


			//Fill out the form
			_webDriver.FindElement(By.Id("Input_Trip_StartDate")).SendKeys(tripPubStartDate);
			_webDriver.FindElement(By.Id("Input_Trip_EndDate")).SendKeys(tripPubEndDate);
			//_webDriver.FindElement(By.Id("Input_Trip_Private"));

			//Proceed
			_webDriver.FindElement(By.Id("addSubmit")).Click();


			//The OK message should be displayed
			keyElemId = "trip-added-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "A new trip was created successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(10)]
		public void Added_publicTrip_Item_Exists()
		{
			//Expect to find the added content in the list
			string keyElemId = $"privateitem-False" +
				$"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
				$"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Convert element contents to string and see if the added item exists
			string actual = keyElem.Text.ToString();
			string expected = DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain start date \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");
			expected = DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain end date \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");

			//Expect there to be a trip with privacy status of previously declared trip
			expected = "Public";
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain privacy status of \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");


			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

		}

		[Test, Order(11)]
		public void Add_privateTrip_Item()
		{
			//Navigate to Own Trips, wait for add element to be enabled
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			string actual = elem.GetAttribute("href").ToString();
			string expected = "trips";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu my trips link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			string keyElemId = "trip-razor-add";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");


			//Fill out the form
			var inputElem = _webDriver.FindElement(By.Id("Input_Trip_StartDate"));
			inputElem.Clear();
			inputElem.SendKeys(tripPrivStartDate);
			inputElem = _webDriver.FindElement(By.Id("Input_Trip_EndDate"));
			inputElem.Clear();
			inputElem.SendKeys(tripPrivEndDate);
			_webDriver.FindElement(By.Id("Input_Trip_Private")).Click();

			//Proceed
			_webDriver.FindElement(By.Id("addSubmit")).Click();


			//The OK message should be displayed
			keyElemId = "trip-added-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "A new trip was created successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(12)]
		public void Added_privateTrip_Item_Exists()
		{
			//Expect to find the added content in the list
			string keyElemId = $"privateitem-True" +
				$"-{DateTime.Parse(tripPrivStartDate).ToString("yyyy-MM-dd")}" +
				$"-{DateTime.Parse(tripPrivEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Convert element contents to string and see if the added item exists
			string actual = keyElem.Text.ToString();
			string expected = DateTime.Parse(tripPrivStartDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain start date \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");
			expected = DateTime.Parse(tripPrivEndDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain end date \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");

			//Expect there to be a trip with privacy status of previously declared trip
			expected = "Private";
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain privacy status of \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");


			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}

		[Test, Order(13)]
		public void Added_privateTrip_Item_Not_Visible_To_All()
		{

			//Group's Trips
			string linkText = "Group's Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "grouptriplist-razor";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			string keyElemHTML = keyElem.GetAttribute("innerHTML");

			//Expect not to find the private item
			keyElemId = $"privateitem-True" +
						   $"-{DateTime.Parse(tripPrivStartDate).ToString("yyyy-MM-dd")}" +
						   $"-{DateTime.Parse(tripPrivEndDate).ToString("yyyy-MM-dd")}";

			Assert.False(keyElemHTML.Contains(keyElemId), $"Expected the page not to display the private trip with ID: {keyElemId} but it did. Element HTML was:\n {keyElemHTML}");

			//Expect to find the public item
			keyElemId = $"privateitem-False" +
			   $"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
			   $"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";

			Assert.True(keyElemHTML.Contains(keyElemId), $"Expected the page to display the public trip with ID: {keyElemId} but it didn't. Element HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

		}


		[Test, Order(14)]
		public void Add_Story_To_Public()
		{
			//Navigate back to own trips
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "owntrips-razor";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			//Find the previously added public trip element
			keyElemId = $"privateitem-False" +
			   $"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
			   $"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Click the button inside the found element to manage trip
			string expected = "tripManage";
			keyElem.FindElement(By.Id(expected)).Click();


			//Expect to find the add box content
			keyElemId = "storyadd-div";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			// find box and define new instance of select
			SelectElement selectElement = new SelectElement(keyElem.FindElement(By.CssSelector("select")));

			// Select first item in the dropdown
			selectElement.SelectByIndex(0);

			// Verify that name equals previusly added
			string expectedValue = destName;
			string actual = selectElement.Options[0].Text;
			if (selectElement.Options.Count > 0)
			{
				expectedValue = actual;
			}
			Assert.AreEqual(expectedValue, actual, $"Expected first dropdown item to be \"{expected}\", but it wasn't. Actual: \"{actual}\"");





			//Fill out the boxes on the form
			var inputElem = _webDriver.FindElement(By.Id("Input_Story_Datum"));
			inputElem.Clear();
			inputElem.SendKeys(storyDate);
			_webDriver.FindElement(By.Id("Input_Story_Description")).SendKeys(storyDescription);

			//Proceed
			_webDriver.FindElement(By.Id("addSubmit-story")).Click();

			//Expect to find the edit story content
			keyElemId = "editstory-div";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");



			//Get file element and input the path to set the picture
			var inputFileElement = _webDriver.FindElement(By.CssSelector("input[type='file']"));
			inputFileElement.SendKeys(pictureImagePath);


			//Save Changes to Story
			_webDriver.FindElement(By.Id("editSubmit-story")).Click();

			//The OK message should be displayed
			keyElemId = "story-saved-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "Changes to Story saved successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			//Navigate back to Trips:
			_webDriver.FindElement(By.Id("editReturn-story")).Click();

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}

		[Test, Order(15)]
		public void Added_story_Item_Exists()
		{
			//One reveal button to be found and clicked:
			string keyElemId = "togglevisibility-story";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Reveal stories
			keyElem.Click();


			//Get Element
			keyElem = _webDriver.FindElement(By.Id("story-ul-list"));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page that contained the Story Desciption that was added but it wasn't found.");



			//Convert element contents to string and see if the added item exists
			string actual = keyElem.Text.ToString();
			string expected = DateTime.Parse(storyDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected story listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = storyDescription;
			Assert.True(actual.Contains(expected), $"Expected story listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");

			//One reveal pictures button to be found and clicked:
			keyElemId = "togglevisibility-story-pictures";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Reveal pictures
			keyElem.Click();

			//Story must contain a picture

			//Get element
			keyElem = _webDriver.FindElement(By.CssSelector("img[src^='data:image/png;base64']"));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with a story picture but it wasn't found.");


			//Verify that listing only has 1 picture
			var listingHTML = _webDriver.FindElement(By.Id("story-ul-list")).GetAttribute("innerHTML");
			int regexMatches = Regex.Matches(listingHTML, "<img src").Count();
			Assert.True(regexMatches == 1, $"Expected story to have only 1 picture, but it contained {regexMatches} pictures.");


			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}

		[Test, Order(16)]
		public void Edit_Destination()
		{

			//Nav and wait for render
			string linkText = "Destinations";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "destinations-razor-auth-listing";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");


			// Find the parent <ul> element
			keyElem = _webDriver.FindElement(By.XPath($"//ul[li[contains(., '{destDescription}')]]"));
			//Click da button
			keyElemId = "editDestButton";
			keyElem = keyElem.FindElement(By.Id(keyElemId));
			// Scroll to the button before attempting to click
			((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
			Thread.Sleep(2000);
			keyElem.Click();



			//Expect to find page content
			keyElemId = "destinationEdit-razor-form";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");


			//Edit Existing data in the DESTINATION form
			destName = "Edited-" + destName;
			destCountry = "Edited-" + destCountry;
			destMunicipality = "Edited-" + destMunicipality;
			destDescription = "Edited-" + destDescription;


			var formInput = _webDriver.FindElement(By.Id("Input_Name"));
			formInput.Clear();
			formInput.SendKeys(destName);
			formInput = _webDriver.FindElement(By.Id("Input_Country"));
			formInput.Clear();
			formInput.SendKeys(destCountry);
			formInput = _webDriver.FindElement(By.Id("Input_Municipality"));
			formInput.Clear();
			formInput.SendKeys(destMunicipality);
			formInput = _webDriver.FindElement(By.Id("Input_Description"));
			formInput.Clear();
			formInput.SendKeys(destDescription);


			//Get file element and input the path to set the picture
			var inputFileElement = _webDriver.FindElement(By.CssSelector("input[type='file']"));
			inputFileElement.SendKeys(destImagePathEdited);

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Proceed
			_webDriver.FindElement(By.Id("editSubmitButton")).Click();


			//The OK message should be displayed
			keyElemId = "destinationd-edited-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			string actual = "Destination was edited successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Return to Destination listings
			_webDriver.FindElement(By.Id("navbackButton")).Click();


			//Expect to find page content
			keyElemId = "destinations-razor-auth-listing";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find Destinations page with element \"{keyElemId}\" via return button but it wasn't found.");
		}

		[Test, Order(17)]
		public void Edited_Destination_Updated()
		{
			//Nav and wait for render
			string linkText = "Destinations";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "destinations-razor-auth-listing";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			//Expect to find the added content in the list
			keyElemId = "destinations-razor-auth-listing";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Convert element contents to string and see if the added item exists
			string actual = keyElem.Text.ToString();
			string expected = destName;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = destCountry;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = destMunicipality;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = destDescription;
			Assert.True(actual.Contains(expected), $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");

			//test for picturelement
			string keyElemHTML = keyElem.GetAttribute("innerHTML").ToString();
			expected = $"destpic-{destName}";
			var welcomepic = _webDriver.FindElement(By.Id(expected));
			Assert.AreEqual(true, welcomepic.Displayed, $"Expected destination listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{keyElemHTML}\"");


			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

		}

		[Test, Order(18)]
		public void Edit_OwnTrip_Private()
		{
			//Navigate back to own trips
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "owntrips-razor";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			//Find the previously added private trip element
			keyElemId = $"privateitem-True" +
			   $"-{DateTime.Parse(tripPrivStartDate).ToString("yyyy-MM-dd")}" +
			   $"-{DateTime.Parse(tripPrivEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Click the button inside the found element to manage trip
			string expected = "tripManage";
			keyElem.FindElement(By.Id(expected)).Click();


			//Expect to find page content
			keyElemId = "ownTripEdit-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			tripPrivStartDate = (DateTime.Parse(tripPrivStartDate).AddDays(-365)).ToString("dd/MM/yyyy");
			tripPrivEndDate = (DateTime.Parse(tripPrivEndDate).AddDays(+1)).ToString("dd/MM/yyyy");

			var formInput = _webDriver.FindElement(By.Id("Input_Trip_Edit_StartDate"));
			formInput.Clear();
			formInput.SendKeys(tripPrivStartDate);
			formInput = _webDriver.FindElement(By.Id("Input_Trip_Edit_EndDate"));
			formInput.Clear();
			formInput.SendKeys(tripPrivEndDate);
			formInput = _webDriver.FindElement(By.Id("Input_Trip_Edit_Private"));
			formInput.Click();

			//Save Changes to Story
			_webDriver.FindElement(By.Id("editSubmit-trip")).Click();

			//The OK message should be displayed
			keyElemId = "trip-edit-saved-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			string actual = "Trip was edited successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			//Navigate back to Trips:
			_webDriver.FindElement(By.Id("navbackButton")).Click();


			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}

		[Test, Order(19)]
		public void Edited_OwnTrip_Private_Updated()
		{

			//Expect to find the added content in the list
			string keyElemId = $"privateitem-False" +
				$"-{DateTime.Parse(tripPrivStartDate).ToString("yyyy-MM-dd")}" +
				$"-{DateTime.Parse(tripPrivEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Convert element contents to string and see if the added item exists
			string actual = keyElem.Text.ToString();
			string expected = DateTime.Parse(tripPrivStartDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain start date \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");
			expected = DateTime.Parse(tripPrivEndDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain end date \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");

			//Expect there to be a trip with privacy status of previously declared trip
			expected = "Public";
			Assert.True(actual.Contains(expected), $"Expected trip listing to contain privacy status of \"{expected}\", but it wasn't found as text. Listing: \"{actual}\"");


			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}

		[Test, Order(20)]
		public void Edit_OwnStory()
		{

			//Navigate back to own trips
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "owntrips-razor";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			//Find the previously added public trip element
			keyElemId = $"privateitem-False" +
			   $"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
			   $"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Click the button inside the found element to manage trip
			string expected = "tripManage";
			keyElem.FindElement(By.Id(expected)).Click();



			//Expect to find the story LIST parent div
			keyElemId = "storylist-div";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Find all story toggle buttons
			var toggleButtons = _webDriver.FindElements(By.XPath("//button[@id='togglevisibility-story']"));

			//Click all dem buttons
			foreach (IWebElement toggleButton in toggleButtons)
			{
				// Scroll to the button before attempting to click
				((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
				Thread.Sleep(200);
				toggleButton.Click();
			}


			// Find the parent <ul> element of our story
			keyElem = _webDriver.FindElement(By.XPath($"//ul[li[contains(., '{storyDescription}')]]"));
			//Click da button
			keyElemId = "editStoryButton";
			keyElem = keyElem.FindElement(By.Id(keyElemId));
			// Scroll to the button before attempting to click
			((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
			Thread.Sleep(2000);
			keyElem.Click();








			//Expect to find the story edit div content
			keyElemId = "editstory-div";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");





			// find box and define new instance of select
			SelectElement selectElement = new SelectElement(keyElem.FindElement(By.CssSelector("select")));

			// Select first item in the dropdown
			selectElement.SelectByIndex(0);

			// Verify that name equals previusly added
			string expectedValue = destName;
			string actual = selectElement.Options[0].Text;
			if (selectElement.Options.Count > 0)
			{
				expectedValue = actual;
			}
			Assert.AreEqual(expectedValue, actual, $"Expected first dropdown item to be \"{expected}\", but it wasn't. Actual: \"{actual}\"");



			storyDate = DateTime.Parse(storyDate).AddDays(-666).ToString("dd/MM/yyyy");
			storyDescription = "EDITED SUPERIOR STORY " + storyDescription;

			//Fill out the boxes on the form
			var inputElem = _webDriver.FindElement(By.Id("Input_Story_Edit_Datum"));
			inputElem.Clear();
			inputElem.SendKeys(storyDate);
			inputElem = _webDriver.FindElement(By.Id("Input_Story_Edit_Description"));
			inputElem.Clear();
			inputElem.SendKeys(storyDescription);

			//Get file element and input the path to set the picture
			var inputFileElement = _webDriver.FindElement(By.CssSelector("input[type='file']"));
			inputFileElement.SendKeys(pictureImagePath2);

			//Save Changes to Story
			_webDriver.FindElement(By.Id("editSubmit-story")).Click();


			//The OK message should be displayed
			keyElemId = "story-saved-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "Changes to Story saved successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			//Navigate back to Trips edit:
			_webDriver.FindElement(By.Id("editReturn-story")).Click();

			//Expect to find page content
			keyElemId = "ownTripEdit-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Navigate back to Trips edit:
			_webDriver.FindElement(By.Id("navbackButton")).Click();

			//Expect to find the modified content mother in the list
			keyElemId = $"privateitem-False" +
			   $"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
			   $"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");



			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}

		[Test, Order(21)]
		public void Edited_OwnStory_Updated()
		{
			//Own Trips
			string linkText = "My Trips";
			_webDriver.FindElement(By.PartialLinkText(linkText)).Click();

			//Expect to find the modified content mother in the list
			string keyElemId = $"privateitem-False" +
				$"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
				$"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");




			//Find all story toggle buttons
			var toggleButtons = _webDriver.FindElements(By.XPath("//button[@id='togglevisibility-story']"));
			//Click all dem buttons
			foreach (IWebElement toggleButton in toggleButtons)
			{
				// Scroll to the button before attempting to click
				((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
				Thread.Sleep(200);
				toggleButton.Click();
			}

			//Find all picture toggle buttons
			toggleButtons = _webDriver.FindElements(By.XPath("//button[@id='togglevisibility-story-pictures']"));
			//Click all dem buttons
			foreach (IWebElement toggleButton in toggleButtons)
			{
				// Scroll to the button before attempting to click
				((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
				Thread.Sleep(200);
				toggleButton.Click();
			}


			//Convert mother element's contents to string and see if the added item exists
			string actual = keyElem.Text.ToString();
			string expected = DateTime.Parse(storyDate).ToString("yyyy-MM-dd");
			Assert.True(actual.Contains(expected), $"Expected story listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");
			expected = storyDescription;
			Assert.True(actual.Contains(expected), $"Expected story listing to contain \"{expected}\", but it wasn't found as text. Actual: \"{actual}\"");


		}

		[Test, Order(22)]
		public void Delete_Active_Destination_Blocked()
		{
			//Nav and wait for render
			string linkText = "Destinations";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "destinations-razor-auth-listing";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");


			// Find the parent <ul> element
			keyElem = _webDriver.FindElement(By.XPath($"//ul[li[contains(., '{destDescription}')]]"));
			//Click da button
			keyElemId = "editDestButton";
			keyElem = keyElem.FindElement(By.Id(keyElemId));
			// Scroll to the button before attempting to click
			((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
			Thread.Sleep(2000);
			keyElem.Click();


			//Try and remove the Destiantion
			_webDriver.FindElement(By.Id("deleteDestinationButton")).Click();


			//The OK message should be displayed
			keyElemId = "destinationd-edited-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			string actual = "Error while deleting, destination has stories!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			//Return to Destination listings
			_webDriver.FindElement(By.Id("navbackButton")).Click();


			//Expect to find page content
			keyElemId = "destinations-razor-auth-listing";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find Destinations page with element \"{keyElemId}\" via return button but it wasn't found.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

		}

		[Test, Order(23)]
		public void Delete_OwnStory()
		{
			//Navigate back to own trips
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			string keyElemId = "owntrips-razor";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			//Find the previously added public trip element
			keyElemId = $"privateitem-False" +
			   $"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
			   $"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Click the button inside the found element to manage trip
			string expected = "tripManage";
			keyElem.FindElement(By.Id(expected)).Click();



			//Expect to find the story LIST parent div
			keyElemId = "storylist-div";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Find all story toggle buttons
			var toggleButtons = _webDriver.FindElements(By.XPath("//button[@id='togglevisibility-story']"));

			//Click all dem buttons
			foreach (IWebElement toggleButton in toggleButtons)
			{
				// Scroll to the button before attempting to click
				((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
				Thread.Sleep(200);
				toggleButton.Click();
			}


			// Find the parent <ul> element of our story
			keyElem = _webDriver.FindElement(By.XPath($"//ul[li[contains(., '{storyDescription}')]]"));
			//Click da button
			keyElemId = "editStoryButton";
			keyElem = keyElem.FindElement(By.Id(keyElemId));
			// Scroll to the button before attempting to click
			((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", keyElem);
			Thread.Sleep(2000);
			keyElem.Click();

			keyElem = null;


			//Expect to find the delete button
			keyElemId = "editstory-div";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");


			//Try and remove the Story
			//Expect to find the delete button
			keyElemId = "delete-story";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			keyElem.Click();




			//Should reroute back to trip edit, but we doing it manually


			//Navigate back to own trips
			linkText = "My Trips";
			elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			elem.Click();
			//Expect to find page content
			keyElemId = "owntrips-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");



			//Find the previously added public trip element
			keyElemId = $"privateitem-False" +
			   $"-{DateTime.Parse(tripPubStartDate).ToString("yyyy-MM-dd")}" +
			   $"-{DateTime.Parse(tripPubEndDate).ToString("yyyy-MM-dd")}";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Click the button inside the found element to manage trip
			expected = "tripManage";
			keyElem.FindElement(By.Id(expected)).Click();



			//Expect to find the story LIST parent div
			keyElemId = "storylist-div";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" but it wasn't found.");

			//Find all story toggle buttons
			toggleButtons = _webDriver.FindElements(By.XPath("//button[@id='togglevisibility-story']"));
			int expectedInt = 0;
			int actualInt = toggleButtons.Count;

			Assert.AreEqual(0, toggleButtons.Count, $"Expected to find page without togglebuttons related to stories but found \"{actualInt}\" of the buttons on the page.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

		}

		[Test, Order(24)]
		public void Delete_OwnTrip()
		{


			//Boilerplate
			Assert.Ignore();
			/*
			//Navigate to Own Trips, wait for add element to be enabled
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			string actual = elem.GetAttribute("href").ToString();
			string expected = "trips";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu my trips link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			string keyElemId = "trip-razor-add";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");


			//Fill out the form
			_webDriver.FindElement(By.Id("Input_Trip_StartDate")).SendKeys(tripPubStartDate);
			_webDriver.FindElement(By.Id("Input_Trip_EndDate")).SendKeys(tripPubEndDate);
			//_webDriver.FindElement(By.Id("Input_Trip_Private"));

			//Proceed
			_webDriver.FindElement(By.Id("addSubmit")).Click();


			//The OK message should be displayed
			keyElemId = "trip-added-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "A new trip was created successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
			*/
		}

		[Test, Order(25)]
		public void Delete_Empty_Destination()
		{


			//Boilerplate
			Assert.Ignore();
			/*
			//Navigate to Own Trips, wait for add element to be enabled
			string linkText = "My Trips";
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			string actual = elem.GetAttribute("href").ToString();
			string expected = "trips";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu my trips link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			elem.Click();
			//Expect to find page content
			string keyElemId = "trip-razor-add";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});
			//If it was actually displayed this should resolve as "true, true"
			Assert.AreEqual(true, keyElem.Displayed, $"Expected to find page with element \"{keyElemId}\" via link with text \"{linkText}\" but it wasn't found.");


			//Fill out the form
			_webDriver.FindElement(By.Id("Input_Trip_StartDate")).SendKeys(tripPubStartDate);
			_webDriver.FindElement(By.Id("Input_Trip_EndDate")).SendKeys(tripPubEndDate);
			//_webDriver.FindElement(By.Id("Input_Trip_Private"));

			//Proceed
			_webDriver.FindElement(By.Id("addSubmit")).Click();


			//The OK message should be displayed
			keyElemId = "trip-added-alert";
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(waitB4FailSeconds));
			//Wait for the Blazor to actually display the element (it's hidden initially due to loading...)
			wait.Until(driver =>
			{
				if (keyElem.Displayed)
				{
					return true;
				}
				else
				{
					return false;
				}
			});

			string keyElemHTML = keyElem.GetAttribute("innerHTML");
			actual = "A new trip was created successfully!";
			//If it was actually displayed and contained OK TEXT this should resolve as "true, true"
			Assert.AreEqual(true, keyElemHTML.Contains(actual), $"Expected the page to display the OK message but it didn't. Messagebox HTML was:\n {keyElemHTML}");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
			*/
		}


		[Test, Order(26)]
		public void LogOut_Again()
		{

			_webDriver.FindElement(By.Id("logout_button")).Click();

			//Wait for built-in auth notification
			Thread.Sleep(1500);
			//The whole bucket, logged out notfication hardcoded
			string pageContent = _webDriver.FindElement(By.Id("app")).Text.ToString();
			string expected = "You are logged out.";


			Thread.Sleep(100);

			Assert.True(pageContent.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{pageContent}\"");
			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(27)]
		public void Welcome_Page_Checks()
		{
			//Navigate to specific URL
			_webDriver.Navigate().GoToUrl(_baseUrl);

			//TODO: Add an logout if logged in
			//_webDriver.FindElement(By.Id("logout_button")).Click();


			//Test the Welcome page contents
			//Wait until a specific element is found(timeout defined in global ImplicitWait
			_webDriver.FindElement(By.Id("index-razor-public"));
			//Or wait a specific time
			//Thread.Sleep(5000);

			//Index must contain a welcome text
			string actual = _webDriver.FindElement(By.Id("index-razor-public")).Text.ToString();
			string expected = "Welcome";
			Assert.True(actual.Contains(expected), $"Expected index to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");

			//Index must contain a welcoming picture
			var welcomepic = _webDriver.FindElement(By.Id("kuva"));
			Assert.AreEqual(true, welcomepic.Displayed, $"Expected index to display a welcoming picture, but it wasn't.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(28)]
		public void Register_and_Login_Links()
		{
			//Test that Register and Log in exist
			//Register
			var actual = _webDriver.FindElement(By.PartialLinkText("Register")).GetAttribute("href").ToString();
			string expected = "authentication/register";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected registration link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");

			//Log in
			actual = _webDriver.FindElement(By.PartialLinkText("Log in")).GetAttribute("href").ToString();
			expected = "authentication/login";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected login link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(29)]
		public void Public_Navmenu_Contents()
		{
			//Test Nav menu contents
			//Home
			var actual = _webDriver.FindElement(By.PartialLinkText("Home")).GetAttribute("href").ToString();
			string expected = "";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu home link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");

			//Destinations
			var destinationsButton = _webDriver.FindElement(By.PartialLinkText("Destinations"));

			actual = destinationsButton.GetAttribute("href").ToString();
			expected = "destinations";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu destinations link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");


			//Verify that nav menu has only 2 links
			var navmenuHTML = _webDriver.FindElement(By.Id("navmenu-public")).GetAttribute("innerHTML");
			int regexMatches = Regex.Matches(navmenuHTML, "<a href").Count();
			Assert.True(regexMatches == 2, $"Expected nav menu to contain 2 links, but it contained {regexMatches} links.");

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}
		}


		[Test, Order(30)]
		public void Public_Destinations_List()
		{
			var destinationsButton = _webDriver.FindElement(By.PartialLinkText("Destinations"));
			//Navigate to Destinations page
			destinationsButton.Click();

			//Wait until page title matches
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
			var title = wait.Until(drv => drv.Title.Equals("Destinations"));


			string expected = "destinations-razor-public-listing";
			var publicListingElem = _webDriver.FindElement(By.Id(expected));

			Assert.AreEqual(true, publicListingElem.Enabled, $"Expected page to have element with id \"{expected}\", but it wasn't: {publicListingElem.Enabled}");

			var destinationElem = _webDriver.FindElement(By.Id($"{destName}-div"));
			string destinationsListHTML = destinationElem.GetAttribute("innerHTML");
			expected = $"{destName}</h4>";
			Assert.True(destinationsListHTML.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListHTML}\"");

			string destinationsListText = destinationElem.Text.ToString();
			expected = $"Country: {destCountry}";
			Assert.True(destinationsListText.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListText}\"");
			expected = $"Municipality: {destMunicipality}";
			Assert.True(destinationsListText.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListText}\"");
			expected = $"Description: {destDescription}";
			Assert.True(destinationsListText.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListText}\"");
			//Picture 
			//Kiuruvesi-picture
			var destinationpic = _webDriver.FindElement(By.Id($"{destName}-picture"));

			if (extraDelayEnabled)
			{
				Thread.Sleep(extraDelayInMilliSeconds);
			}

			Assert.AreEqual(true, destinationpic.Displayed);

		}
	}
}