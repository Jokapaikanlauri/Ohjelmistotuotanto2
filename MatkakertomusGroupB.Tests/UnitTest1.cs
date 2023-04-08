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
				Console.WriteLine("dotnet");
				Console.WriteLine($"run --project {webProjectPath}\\MatkakertomusGroupB.Server.csproj");

				// Wait for the server to start up
				Thread.Sleep(5000);

				// Create a new Selenium WebDriver manager instance
				new DriverManager().SetUpDriver(new ChromeConfig());

				// Create Chrome options
				var chromeOptions = new ChromeOptions();

				// Set the "ignore-certificate-errors" flag
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

			//Optional delay, waits, for application get loaded before performing tests
			_webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
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

		[Test]
		public void Test()
		{
			_webDriver.Navigate().GoToUrl(_baseUrl);

			// Wait for an element with ID "myButton" to be clickable
			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			var myButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("myButton")));

			// Check that the page title contains "Home Page"
			var pageTitle = _webDriver.Title;
			Assert.True(pageTitle.Contains("Home Page"), $"Expected page title to contain \"Home Page\", but actual title is \"{pageTitle}\"");
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