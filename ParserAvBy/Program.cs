using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Support.UI;

Console.Title = "Parser by C4ke";
Console.Write("Specify the index of the page from which to start parsing (or leave it blank):");

var value = Console.ReadLine();
var index = (string.IsNullOrEmpty(value)) ? 1 : int.Parse(value);
var fileName = $"phones_{new Random().Next(1_000, 9_999)}.txt";

var options = new ChromeOptions();
var service = ChromeDriverService.CreateDefaultService();
service.HideCommandPromptWindow = true;
options.AddArgument("no-sandbox");
options.AddArgument("remote-debugging-port=0");
options.AddArgument("disable-extensions");

var chrome = new ChromeDriver(service, options);
while (true)
{
    chrome.Navigate().GoToUrl($"https://cars.av.by/filter?page={index}");

    for (var i = 1; i < 29; i++)
    {
        if (!IsElementExist(chrome, By.XPath($"/html/body/div[1]/div[2]/main/div/div/div[1]/div[4]/div[3]/div/div[3]/div/div[{i}]/div/div[2]/h3/a/span")))
            continue;
        
        chrome.FindElement(
            By.XPath($"/html/body/div[1]/div[2]/main/div/div/div[1]/div[4]/div[3]/div/div[3]/div/div[{i}]/div/div[2]/h3/a/span")).Click();

        await Task.Delay(1_000);

        again:
        if (IsElementExist(chrome, By.XPath("/html/body/div[1]/div[2]/div[2]/button")))
            chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/button")).Click();
        
        var xPathToBtn = string.Empty;
        if (IsElementExist(chrome,
                By.XPath("/html/body/div[1]/div[2]/main/div/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[5]/button")))
            xPathToBtn = "/html/body/div[1]/div[2]/main/div/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[5]/button";
        else if (IsElementExist(chrome,
                By.XPath("/html/body/div[1]/div[3]/main/div/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[5]/button[2]")))
            xPathToBtn = "/html/body/div[1]/div[3]/main/div/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[5]/button[2]";
        
        if (string.IsNullOrEmpty(xPathToBtn))
            goto again; 
        
        chrome.FindElement(By.XPath(xPathToBtn)).Click();
        await Task.Delay(1_000);

        var xPathToPhone = string.Empty;
        if (IsElementExist(chrome,
                By.XPath("/html/body/div[1]/div[2]/main/div/div/div[1]/div[1]/div[2]/div/div/div/div[2]/ul/li")))
            xPathToPhone = "/html/body/div[1]/div[2]/main/div/div/div[1]/div[1]/div[2]/div/div/div/div[2]/ul/li";
        else if (IsElementExist(chrome,
                     By.XPath("/html/body/div[1]/div[3]/main/div/div/div[1]/div[1]/div[2]/div/div/div/div[2]/ul/li")))
            xPathToPhone = "/html/body/div[1]/div[3]/main/div/div/div[1]/div[1]/div[2]/div/div/div/div[2]/ul/li";
        
        if (string.IsNullOrEmpty(xPathToPhone))
            goto again; 
        
        await File.AppendAllTextAsync(fileName, $"{chrome.FindElement(By.XPath(xPathToPhone)).Text}\n");
        
        chrome.Navigate().GoToUrl($"https://cars.av.by/filter?page={index}");
        await Task.Delay(1_500);
    }
    
    index++;
}

bool IsElementExist(ISearchContext driver, By by)
{
    try
    {
        var element = driver.FindElement(by);
        return element.Displayed;
    }
    catch (NoSuchElementException)
    {
        return false;
    }
}