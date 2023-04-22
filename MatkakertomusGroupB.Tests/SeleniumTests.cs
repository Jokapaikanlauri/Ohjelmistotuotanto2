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
		


		[SetUp]
		public void SetUp()
		{

			// Get the solution directory path
			var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory));

			// Build the path to the web project
			var webProjectPath = solutionDir.Replace(".Tests\\bin", "\\Server");

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
		[TearDown]
		public void TearDown()
		{
			_webDriver.Quit();
			_webServerProcess.Kill();
		}

		[Test, Order(1), Description("Test public user controls")]
		public void Public_User()
		{
			//Navigate to specific URL
			_webDriver.Navigate().GoToUrl(_baseUrl);


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
			Assert.AreEqual(true, welcomepic.Displayed);

			//Test that Register and Log in exist
			//Register
			actual = _webDriver.FindElement(By.PartialLinkText("Register")).GetAttribute("href").ToString();
			expected = "authentication/register";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected registration link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");

			//Log in
			actual = _webDriver.FindElement(By.PartialLinkText("Log in")).GetAttribute("href").ToString();
			expected = "authentication/login";
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected login link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");


			//Test Nav menu contents
			//Home
			actual = _webDriver.FindElement(By.PartialLinkText("Home")).GetAttribute("href").ToString();
			expected = "";
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

			//Navigate to Destinations page
			destinationsButton.Click();

			//Wait until page title matches
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
			var title = wait.Until(drv => drv.Title.Equals("Destinations"));


			expected = "destinations-razor-public-listing";
			var publicListingElem = _webDriver.FindElement(By.Id(expected));

			Assert.AreEqual(true, publicListingElem.Enabled, $"Expected page to have element with id \"{expected}\", but it wasn't: {publicListingElem.Enabled}");

			var destinationsListHTML = _webDriver.FindElement(By.Id("Kiuruvesi-div")).GetAttribute("innerHTML");
			expected = "Kiuruvesi</h4>";
			Assert.True(destinationsListHTML.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListHTML}\"")

				;
			var destinationsListText = _webDriver.FindElement(By.Id("Kiuruvesi-div")).Text.ToString();
			expected = "Country: FINLAND";
			Assert.True(destinationsListText.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListText}\"");
			expected = "destMunicipality: Kiuruvesi";
			Assert.True(destinationsListText.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListText}\"");
			expected = "Description: Kiuruvesi tunnetaan paremmin Moravetenä tai Nistivetenä. Ota mukaan oma morasi, paikallisilta ne on jo suurimmaksi osaksi kerätty pois";
			Assert.True(destinationsListText.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{destinationsListText}\"");
			//Picture 
			//Kiuruvesi-picture
			var destinationpic = _webDriver.FindElement(By.Id("Kiuruvesi-picture"));
			Assert.AreEqual(true, destinationpic.Displayed);


			/*
			// Wait for an element with ID "myButton" to be clickable
			//https://www.selenium.dev/selenium/docs/api/dotnet/html/M_OpenQA_Selenium_Support_UI_ExpectedConditions_ElementToBeClickable.htm
            
			
			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			//var myButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("myButton")));

			var myButton = wait.Until(ExpectedConditions.precenseOfElementLocated(By.Id("LoginDisplay")));
			*/

			// Check that the page title contains "Home Page"
			/*var pageTitle = _webDriver.Title;
			string expected = "Home Page";
			Assert.True(pageTitle.Contains(expected), $"Expected page title to contain \"{expected}\", but actual title is \"{pageTitle}\"");*/
		}

		[Test, Order(2)]
		public void LogIn_Nav_LogOut()
		{
			//Navigate to specific URL
			_webDriver.Navigate().GoToUrl(_baseUrl);

			//Wait until a specific element is found(timeout defined in global ImplicitWait
			_webDriver.FindElement(By.PartialLinkText("Log in")).Click();
			//Or wait a specific time
			//Thread.Sleep(5000);

			//https://www.browserstack.com/guide/sendkeys-in-selenium
			//On the login page fill out the form and proceed
			_webDriver.FindElement(By.Id("Input_Email")).SendKeys("Chad@Chadistan.com");
			_webDriver.FindElement(By.Id("Input_Password")).SendKeys("Chad@Chadistan.com");
			_webDriver.FindElement(By.Id("login-submit")).Click();



			//User must see his NickName
			string loginDisplayHTML = _webDriver.FindElement(By.Id("nick-display")).GetAttribute("innerHTML");
			string expected = "Dude";
			Assert.True(loginDisplayHTML.Contains(expected), $"Expected login box to contain nickname \"{expected}\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");
			Assert.False(loginDisplayHTML.Contains("login"), $"Expected login box to not \"login\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");

			//Test Nav menu contents
			//Nav to pages
			//Koti-, Matkakohde-, Porukan matkat-, Omat matkat-, Omat tiedot-, Jäsenet-sivut


			//Destinations
			string linkText = "Destinations";
			//Find the correct anchor <a> item via link text
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			//Get the href of the <a>
			string actual = elem.GetAttribute("href").ToString();
			expected = "destinations";
			//Compare the previous to be as expected
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu destinations link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			//Use the found link to navigate to said page
			elem.Click();
			//Expect to find page content
			string keyElemId = "destinations-razor-auth-listing";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			keyElemId = "travellerlist-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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

			_webDriver.FindElement(By.Id("logout_button")).Click();

			//Wait for built-in auth notification
			Thread.Sleep(5000);
			//The whole bucket, logged out notfication hardcoded
			string pageContent = _webDriver.FindElement(By.Id("app")).Text.ToString();
			expected = "You are logged out.";


			Thread.Sleep(100);

			Assert.True(pageContent.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{pageContent}\"");
		}
		[Test, Order(3)]
		public void Register_nav_add_LogOut()
		{
			//Navigate to specific URL
			_webDriver.Navigate().GoToUrl(_baseUrl);

			//Wait until a specific element is found(timeout defined in global ImplicitWait
			_webDriver.FindElement(By.PartialLinkText("Register")).Click();

			//Generate the picture paths for all the tests
            //Define get the running folder of current process(TEST)
            //Should equate to (you repo)/ET21KM-Ohjelmistotuotanto2-GroupB\MatkakertomusGroupB.Tests\bin\Debug\net7.0\
            string processFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            // Build the path to the correct pictures
            string destImagePath = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testDestPicture.png");
            string pictureImagePath = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testPicture.png");
            string userImagePath = processFolder.Replace(".Tests\\bin\\Debug\\net7.0\\", ".Tests\\TestImages\\testUserPicture.png");
            

			if (debuggingMessagesEnabled)
			{
                Console.WriteLine(destImagePath);
                Console.WriteLine(pictureImagePath);
                Console.WriteLine(userImagePath);
            }

            Random random = new Random();
			int randomNumber = random.Next(100000, 999999);

			//Generate Random data for User out data
			string forename = $"Test-{nameof(forename)}-{randomNumber.ToString()}";
			string surname = $"Test-{nameof(surname)}-{randomNumber.ToString()}";
			string nickname = $"Test-{nameof(nickname)}-{randomNumber.ToString()}";
			string email = $"Test-{nameof(email)}-{randomNumber.ToString()}@Chadistan.com";
			string password = $"Test-{nameof(password)}-{randomNumber.ToString()}";

			//Generate random data for Destination
			string destName = $"Test-{nameof(destName)}-{randomNumber.ToString()}";
            string destCountry = $"Test-{nameof(destCountry)}-{randomNumber.ToString()}";
            string destMunicipality = $"Test-{nameof(destMunicipality)}-{randomNumber.ToString()}";
            string destDescription = $"Test-{nameof(destDescription)}-{randomNumber.ToString()}";
     

			//Fill out registration form
            _webDriver.FindElement(By.Id("Input_Forename")).SendKeys(forename);
			_webDriver.FindElement(By.Id("Input_Surname")).SendKeys(surname);
			_webDriver.FindElement(By.Id("Input_Nickname")).SendKeys(nickname);
			_webDriver.FindElement(By.Id("Input_Email")).SendKeys(email);
			_webDriver.FindElement(By.Id("Input_Password")).SendKeys(password);
			_webDriver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(password);
			//Proceed
			_webDriver.FindElement(By.Id("registerSubmit")).Click();
			

			//User must see his NickName
			string loginDisplayHTML = _webDriver.FindElement(By.Id("nick-display")).GetAttribute("innerHTML");
			string expected = nickname;
			Assert.True(loginDisplayHTML.Contains(expected), $"Expected login box to contain nickname \"{expected}\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");
			Assert.False(loginDisplayHTML.Contains("login"), $"Expected login box to not \"login\", but it wasn't found. Actual: \"{loginDisplayHTML}\"");

			//Navigate to Destinations, wait for add element to be enabled
			string linkText = "Destinations";
			//Find the correct anchor <a> item via link text
			var elem = _webDriver.FindElement(By.PartialLinkText(linkText));
			//Get the href of the <a>
			string actual = elem.GetAttribute("href").ToString();
			expected = "destinations";
			//Compare the previous to be as expected
			Assert.AreEqual(true, (actual.Contains(expected)), $"Expected nav menu destinations link to contain \"{expected}\", but it wasn't found. Actual: \"{actual}\"");
			//Use the found link to navigate to said page
			elem.Click();
			//Expect to find page content
			string keyElemId = "destinations-razor-add";
			//Get element
			var keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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

			//Get file element and input path
            var inputFile = _webDriver.FindElement(By.CssSelector("input[type='file']"));
            inputFile.SendKeys(destImagePath);

            //Proceed
            _webDriver.FindElement(By.Id("addSubmit")).Click();


            Thread.Sleep(10000);
			Assert.Fail();

            /*
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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
			keyElemId = "travellerlist-razor";
			//Get element
			keyElem = _webDriver.FindElement(By.Id(keyElemId));
			//Define wait time
			wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
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

			_webDriver.FindElement(By.Id("logout_button")).Click();

			//Wait for built-in auth notification
			Thread.Sleep(5000);
			//The whole bucket, logged out notfication hardcoded
			string pageContent = _webDriver.FindElement(By.Id("app")).Text.ToString();
			expected = "You are logged out.";


			Thread.Sleep(100);

			Assert.True(pageContent.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found. Actual: \"{pageContent}\"");

			*/
        }
    }
}