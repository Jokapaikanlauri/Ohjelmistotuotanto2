using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Diagnostics;
using OpenQA.Selenium.Support.UI;

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
	public class Tests
	{
		private IWebDriver _webDriver;
		private string _baseUrl { get; set; } = "https://localhost:7012";
		//private CustomWebApplicationFactory<Program> _factory;
		//private HttpClient _client;

		private IWebDriver driver;
		private Process _webServerProcess;


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
				Thread.Sleep(5000);

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

		[Test, Order(1), Description("Test description here")]
		public void PublicUser()
		{
			//Navigate to specifi URL
			_webDriver.Navigate().GoToUrl(_baseUrl);

			//Wait until a specific element is found(timeout defined in global ImplicitWait
			_webDriver.FindElement(By.Id("index-razor"));
			//Or wait a specific time
			//Thread.Sleep(5000);




			/*
			// Wait for an element with ID "myButton" to be clickable
			//https://www.selenium.dev/selenium/docs/api/dotnet/html/M_OpenQA_Selenium_Support_UI_ExpectedConditions_ElementToBeClickable.htm
            
			
			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			//var myButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("myButton")));

			var myButton = wait.Until(ExpectedConditions.precenseOfElementLocated(By.Id("LoginDisplay")));
			*/

			// Check that the page title contains "Home Page"
			var pageTitle = _webDriver.Title;
			string expected = "Home Page";
			Assert.True(pageTitle.Contains(expected), $"Expected page title to contain \"{expected}\", but actual title is \"{pageTitle}\"");
		}

		[Test, Order(2)]
		public void LogIn_LogOut()
		{
			//Navigate to specifi URL
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

			//Thread.Sleep(1000);

			//Get nickname
			string loginInsides = _webDriver.FindElement(By.Id("nick-display")).Text.ToString();
			string expected = "Dude";

			//Thread.Sleep(1000);

			Assert.True(loginInsides.Contains(expected), $"Expected login box to contain nickname \"{expected}\", but it wasn't found: \"{loginInsides}\"");
			Assert.False(loginInsides.Contains("login"), $"Expected login box to not \"login\", but it was found: \"{loginInsides}\"");

			//Thread.Sleep(1000);

			_webDriver.FindElement(By.Id("logout_button")).Click();
			Thread.Sleep(1000);
			//The whole bucket, logged out notfication hardcoded elsewhere
			string pageContent = _webDriver.FindElement(By.Id("app")).Text.ToString();
			expected = "You are logged out";


			Thread.Sleep(2000);
			Assert.True(pageContent.Contains(expected), $"Expected page to contain \"{expected}\", but it wasn't found: \"{loginInsides}\"");

		}


		/*
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}
		*/
	}
}