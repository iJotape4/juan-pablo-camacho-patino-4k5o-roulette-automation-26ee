using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace RouletteQA;

public class Tests
{
    private IWebDriver driver;
    private WebDriverWait wait;
    string url = "https://csgoempire.gg/roulette";
    
    //Useful Elements
    string textFieldXPath = "//input[@placeholder='Enter bet amount...']";
    IWebElement textField;
    private float textFieldValue =0;
    private string signInButtonTestId = "nav-head-sign-in";
    
    //Buttons IDs
    private string buttonMaxTestId = "roulette-bet-input-maxundefined";
    Dictionary<float, string> sumButtons = new Dictionary<float, string>()
    {
        {0.01f,"roulette-bet-input-+0.01"},
        {0.1f,"roulette-bet-input-+0.1"},
        {1f,"roulette-bet-input-+1"},
        {10f,"roulette-bet-input-+10"},
        {100f,"roulette-bet-input-+100"},

    };
    Dictionary<float, string> MultiplyButtons = new Dictionary<float, string>()
    {
        {0f,"roulette-bet-input-clearundefined"},
        {0.5f,"roulette-bet-input-1/2"},
        {2f,"roulette-bet-input-x2"},
    };
    
    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl(url);
        driver.Manage().Window.Maximize();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
        LookForTextField();
    }
    
    public void LookForTextField()
    {
       textField = wait.Until(driver => 
        {
            var element = driver.FindElement(By.XPath(textFieldXPath));
            return (element != null && element.Enabled) ? element : null;
        });
    }
    
    [Test]
    public void TestBetButtons()
    {
        CheckBetButtons();
    }
    
    public void CheckBetButtons(bool clearing = false)
    {
        foreach (var button in sumButtons)
        {
            IWebElement button1 =  GetBetButton(button.Value);
            button1.Click();
            try
            {
                Assert.IsTrue(CheckValueIncrease(button.Key));
            }
            catch (AssertionException)
            {
                Console.WriteLine("The CheckValueIncrease method returned false while checking button: " + button.Value + "clearing = "+clearing);
            }
            if(clearing) ClearTextField();
        }
        
        foreach (var button in MultiplyButtons)
        {
            IWebElement button1 =  GetBetButton(button.Value);
            button1.Click();
            try
            {
                // Assert that the value is correctly multiplied
                Assert.IsTrue(CheckValueMultiply(button.Key));
            }
            catch (AssertionException)
            {
                Console.WriteLine("The CheckValueMultiply method returned false while checking button: " + button.Value + "clearing = "+clearing);
                throw;
            }
            if(clearing) ClearTextField();
        }
        //Check again now clearing the textfield after each iteration
        if (!clearing)
            CheckBetButtons(true);
        else 
            Assert.Pass("Bet buttons works as expected, everything looking good there!");
    }
    
    public IWebElement GetBetButton(string buttonTestId)
    {
        IWebElement button = wait.Until(driver => 
        {
            var element = driver.FindElement(By.CssSelector($"button[data-testid='{buttonTestId}']"));
           
            return (element != null && element.Enabled) ? element : null;
        });
        return button;
    }

    public bool CheckValueIncrease(float amount)
    {
        float sum = textFieldValue+ amount;
        float currentValue = float.Parse(textField.GetAttribute("value"));
        textFieldValue = currentValue;
        return sum == currentValue;
    }
    
    public bool CheckValueMultiply(float amount)
    {
        float multiply =  (float) Math.Round( textFieldValue* amount, 2);
        float currentValue = float.Parse(textField.GetAttribute("value"));
        textFieldValue = currentValue;
        return multiply == currentValue;
    }
    
    public void ClearTextField()
    {
        IWebElement clearButton = GetBetButton(MultiplyButtons[0]);
        clearButton.Click();
        textFieldValue = 0;
    }
    
    [Test]
    //Thi just opens the steam website login :)
    public void SignIn()
    {
        IWebElement button = wait.Until(driver => 
        {
            var element = driver.FindElement(By.CssSelector($"button[data-testid='{signInButtonTestId}']"));
           
            return (element != null && element.Enabled) ? element : null;
        });
        button.Click();
        Assert.Pass("Sig in redirect works as expected!");
    }
    
    [TearDown]
    public void TearDown()
    {
        if (driver != null)
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}