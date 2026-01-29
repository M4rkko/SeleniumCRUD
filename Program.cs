using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumCRUD_Testing
{
    internal class Program
    {
        private const string BASE_URL = "https://localhost:44380";
        private const string KG_INDEX = "/Kindergarten";
        private const string RE_INDEX = "/realestates";

        static void Main(string[] args)
        {
            var options = new FirefoxOptions();
            options.AddArgument("--width=1280");
            options.AddArgument("--height=800");

            IWebDriver driver = new FirefoxDriver(options);

            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(12));

                driver.Navigate().GoToUrl(BASE_URL);
                wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("body")));
                Console.WriteLine("Successfully Home loaded");

                RunKinderGartenCrud(driver, wait);
                RunRealEstatesCrud(driver, wait);

                Console.WriteLine("ALL TESTS PASSED");
            }
            catch (Exception ex)
            {
                Console.WriteLine("TEST FAILED:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }

        private static void RunKinderGartenCrud(IWebDriver driver, WebDriverWait wait)
        {
            Console.WriteLine("\n--- KINDERGARTEN CRUD ---");

            Navigate(driver, wait, BASE_URL + KG_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.h1")));
            wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Create")));
            Console.WriteLine("Successfully KG Navigate");

            string createdName = "Stars";
            string createdGroup = "Group A";
            string createdTeacher = "Teacher Test";
            string createdCount = "25";

            CreateKinderGarten(driver, wait, createdName, createdGroup, createdTeacher, createdCount, expectSuccess: true);


            Navigate(driver, wait, BASE_URL + KG_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            AssertContains(driver, createdName, "KG Create (valid) failed");
            Console.WriteLine("Successfully KG Create valid");

            CreateKinderGarten(driver, wait, "KG_BAD_" + Guid.NewGuid().ToString("N")[..4], "Group B", "Teacher Bad", "abc", expectSuccess: false);
            Console.WriteLine("Successfully KG Create invalid (wrong type blocked)");

            Navigate(driver, wait, BASE_URL + KG_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            ClickRowButton(driver, createdName, "Details");

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h1[contains(.,'Details')]")));
            AssertContains(driver, createdGroup, "KG Details failed");
            AssertContains(driver, createdTeacher, "KG Details failed");
            AssertContains(driver, createdCount, "KG Details failed");

            Console.WriteLine("Successfully KG Details");

            driver.FindElement(By.LinkText("Back")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));

            ClickRowButton(driver, createdName, "Update");
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));

            string updatedName = createdName + "_U";
            string updatedGroup = "Group U";
            string updatedTeacher = "Teacher U";
            string updatedCount = "30";

            ClearAndType(driver, By.Id("KindergartenName"), updatedName);
            ClearAndType(driver, By.Id("GroupName"), updatedGroup);
            ClearAndType(driver, By.Id("Teacher"), updatedTeacher);
            ClearAndType(driver, By.Id("ChildrenCount"), updatedCount);
            ClickSubmit(driver, "Update");

            Navigate(driver, wait, BASE_URL + KG_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            ClickRowButton(driver, updatedName, "Details");

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h1[contains(.,'Details')]")));
            AssertContains(driver, updatedName, "KG Update (valid) failed");
            AssertContains(driver, updatedGroup, "KG Update (valid) failed");
            AssertContains(driver, updatedTeacher, "KG Update (valid) failed");
            AssertContains(driver, updatedCount, "KG Update (valid) failed");
            Console.WriteLine("Successfully KG Update valid");

            driver.FindElement(By.LinkText("Back")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));


            ClickRowButton(driver, updatedName, "Update");
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));
            ClearAndType(driver, By.Id("ChildrenCount"), "xyz");
            ClickSubmit(driver, "Update");

            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));
            if (!driver.FindElement(By.TagName("h1")).Text.Contains("Update", StringComparison.OrdinalIgnoreCase))
                throw new Exception("KG Update (invalid) expected to stay on Update page");
            Console.WriteLine("Successfully KG Update invalid (wrong type blocked)");

            Navigate(driver, wait, BASE_URL + KG_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            ClickRowButton(driver, updatedName, "Delete");

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h1[contains(.,'Delete')]")));
            ClickSubmit(driver, "Delete");

            Navigate(driver, wait, BASE_URL + KG_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            if (driver.PageSource.Contains(updatedName, StringComparison.OrdinalIgnoreCase))
                throw new Exception("KG Delete failed");
            Console.WriteLine("Successfully KG Delete");
        }

        private static void CreateKinderGarten(
            IWebDriver driver, WebDriverWait wait,
            string name, string group, string teacher, string childrenCount,
            bool expectSuccess)
        {
            Navigate(driver, wait, BASE_URL + KG_INDEX);
            driver.FindElement(By.LinkText("Create")).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));

            ClearAndType(driver, By.Id("KindergartenName"), name);
            ClearAndType(driver, By.Id("GroupName"), group);
            ClearAndType(driver, By.Id("Teacher"), teacher);
            ClearAndType(driver, By.Id("ChildrenCount"), childrenCount);

            ClickSubmit(driver, "Create");

            if (!expectSuccess)
                wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));
            else
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
        }

        private static void RunRealEstatesCrud(IWebDriver driver, WebDriverWait wait)
        {
            Console.WriteLine("\n--- REALESTATES CRUD ---");

            Navigate(driver, wait, BASE_URL + RE_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.h1")));
            wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Create")));
            Console.WriteLine("Successfully RE Navigate");

            string createdSize = "22";
            string createdLocation = "Tallinn";
            string createdBuildingType = "Apartment";
            string room = new Random().Next(1000, 9999).ToString();


            CreateRealEstate(driver, wait, createdSize, createdLocation, room, createdBuildingType, expectSuccess: true);


            Navigate(driver, wait, BASE_URL + RE_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            ClickRealEstateRowButtonByRoom(driver, room, "Details");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h1[contains(.,'Details')]")));
            AssertContains(driver, createdSize, "RE Create (valid) failed");
            AssertContains(driver, createdLocation, "RE Create (valid) failed");
            AssertContains(driver, room, "RE Create (valid) failed");
            AssertContains(driver, createdBuildingType, "RE Create (valid) failed");
            Console.WriteLine("Successfully RE Create valid");
            driver.FindElement(By.LinkText("Back")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));

            CreateRealEstate(driver, wait, "Size_BAD_" + Guid.NewGuid().ToString("N")[..4], "Tartu", "abc", "Apartment", expectSuccess: false);
            Console.WriteLine("Successfully RE Create invalid (wrong type blocked)");

            Navigate(driver, wait, BASE_URL + RE_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            ClickRealEstateRowButtonByRoom(driver, room, "Details");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h1[contains(.,'Details')]")));
            AssertContains(driver, "Tallinn", "RE Details failed");
            AssertContains(driver, room, "RE Details failed");
            AssertContains(driver, "Apartment", "RE Details failed");
            Console.WriteLine("Successfully RE Details");
            driver.FindElement(By.LinkText("Back")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));


            ClickRealEstateRowButtonByRoom(driver, room, "Update");
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));

            string updatedSize = "33";
            string updatedLocation = "Tartu";
            string updatedBuildingType = "House";

            ClearAndType(driver, By.Id("Size"), updatedSize);
            ClearAndType(driver, By.Id("Location"), updatedLocation);
            ClearAndType(driver, By.Id("BuildingType"), updatedBuildingType);
            ClickSubmit(driver, "Update");


            Navigate(driver, wait, BASE_URL + RE_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            ClickRealEstateRowButtonByRoom(driver, room, "Details");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h1[contains(.,'Details')]")));
            AssertContains(driver, updatedSize, "RE Update (valid) failed");
            AssertContains(driver, updatedLocation, "RE Update (valid) failed");
            AssertContains(driver, room, "RE Update (valid) failed");
            AssertContains(driver, updatedBuildingType, "RE Update (valid) failed");
            Console.WriteLine("Successfully RE Update valid");
            driver.FindElement(By.LinkText("Back")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));


            ClickRealEstateRowButtonByRoom(driver, room, "Update");
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));
            ClearAndType(driver, By.Id("RoomNumber"), "3zzz");
            ClickSubmit(driver, "Update");

            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));
            if (!driver.FindElement(By.TagName("h1")).Text.Contains("Update", StringComparison.OrdinalIgnoreCase))
                throw new Exception("RE Update (invalid) expected to stay on Update page");
            Console.WriteLine("Successfully RE Update invalid (wrong type blocked)");

            Navigate(driver, wait, BASE_URL + RE_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            ClickRealEstateRowButtonByRoom(driver, room, "Delete");

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h1[contains(.,'Delete')]")));
            ClickSubmit(driver, "Delete");

            Navigate(driver, wait, BASE_URL + RE_INDEX);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
            if (driver.PageSource.Contains(room, StringComparison.OrdinalIgnoreCase))
                throw new Exception("RE Delete failed");
            Console.WriteLine("Successfully RE Delete");
        }

        private static void CreateRealEstate(
            IWebDriver driver, WebDriverWait wait,
            string size, string location, string roomNumber, string buildingType,
            bool expectSuccess)
        {
            Navigate(driver, wait, BASE_URL + RE_INDEX);
            driver.FindElement(By.LinkText("Create")).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));

            ClearAndType(driver, By.Id("Size"), size);
            ClearAndType(driver, By.Id("Location"), location);
            ClearAndType(driver, By.Id("RoomNumber"), roomNumber);
            ClearAndType(driver, By.Id("BuildingType"), buildingType);

            ClickSubmit(driver, "Create");

            if (!expectSuccess)
                wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));
            else
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table")));
        }

        private static void Navigate(IWebDriver driver, WebDriverWait wait, string url)
        {
            driver.Navigate().GoToUrl(url);
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("body")));
        }

        private static void ClearAndType(IWebDriver driver, By by, string text)
        {
            var el = driver.FindElement(by);
            el.Clear();
            el.SendKeys(text);
        }

        private static void ClickRowButton(IWebDriver driver, string rowText, string buttonText)
        {
            var row = driver.FindElement(By.XPath($"//table//tr[td[contains(.,'{rowText}')]]"));
            row.FindElement(By.XPath($".//a[normalize-space(text())='{buttonText}']")).Click();
        }

        private static void ClickRealEstateRowButtonByRoom(IWebDriver driver, string roomNumber, string buttonText)
        {
            var row = driver.FindElement(By.XPath($"//table//tr[td[contains(normalize-space(.), '{roomNumber}')]]"));
            row.FindElement(By.XPath($".//a[normalize-space(text())='{buttonText}']")).Click();
        }

        private static void AssertContains(IWebDriver driver, string text, string error)
        {
            if (!driver.PageSource.Contains(text, StringComparison.OrdinalIgnoreCase))
                throw new Exception(error);
        }

        private static void ClickSubmit(IWebDriver driver, string text)
        {
            var inputs = driver.FindElements(By.CssSelector($"input[type='submit'][value='{text}']"));
            if (inputs.Count > 0) { inputs[0].Click(); return; }

            var buttons = driver.FindElements(By.XPath($"//button[@type='submit' and normalize-space(.)='{text}']"));
            if (buttons.Count > 0) { buttons[0].Click(); return; }

            var anySubmit = driver.FindElements(By.CssSelector("form button[type='submit'], form input[type='submit']"));
            if (anySubmit.Count > 0) { anySubmit[0].Click(); return; }

            throw new Exception($"Submit button '{text}' not found.");
        }
    }
}
