
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class Program
{
    private const string DefaultUrl =
        "https://app.cloudqa.io/home/AutomationPracticeForm";

    static void Main(string[] args)
    {
        string url = args.Length > 0
            ? args[0]
            : Environment.GetEnvironmentVariable("CLOUDQA_URL")
                ?? DefaultUrl;

        string firstName = "Chinthan";
        string gender = "Male";
        string state = "India";

        IWebDriver? driver = null;

        try
        {
            Console.WriteLine("CLOUDQA RESILIENT AUTOMATION");
            Console.WriteLine("Target: " + url);

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            var wait = new WebDriverWait(
                driver, TimeSpan.FromSeconds(15));

            driver.Navigate().GoToUrl(url);

            wait.Until(d =>
                ((IJavaScriptExecutor)d)
                .ExecuteScript("return document.readyState")
                ?.ToString() == "complete");

            Console.WriteLine("\nBrowser opened successfully.");

            // FIRST NAME
            Console.WriteLine("\nLocating First Name...");

            IWebElement name = FindControl(
                driver, wait, "text", "First Name", firstName);

            name.Clear();
            name.SendKeys(firstName);

            if (name.GetAttribute("value") != firstName)
                throw new Exception("First Name verification failed.");

            Console.WriteLine("[PASS] First Name: " + firstName);


            // GENDER
            Console.WriteLine("\nLocating Gender...");

            IWebElement genderElement = FindControl(
                driver, wait, "radio", "Gender", gender);

            if (!genderElement.Selected)
                genderElement.Click();

            if (!genderElement.Selected)
                throw new Exception("Gender verification failed.");

            Console.WriteLine("[PASS] Gender: " + gender);


            // STATE
            Console.WriteLine("\nLocating State...");

            IWebElement stateElement = FindControl(
                driver, wait, "select", "State", state);

            SelectElement stateDropdown =
                new SelectElement(stateElement);

            stateDropdown.SelectByValue(state);

            string selectedState =
                stateDropdown.SelectedOption.GetAttribute("value") ?? "";

            if (selectedState != state)
                throw new Exception("State verification failed.");

            Console.WriteLine("[PASS] State: " + state);


            // FINAL RESULT
            Console.WriteLine("\n================================");
            Console.WriteLine("       TEST PASSED");
            Console.WriteLine("================================");

            Console.WriteLine("First Name : " + firstName);
            Console.WriteLine("Gender     : " + gender);
            Console.WriteLine("State      : " + state);

        }
        catch (Exception ex)
        {
            Console.WriteLine("\n========== TEST FAILED ==========");
            Console.WriteLine(ex.ToString());
            Environment.ExitCode = 1;
        }
        finally
        {
            if (driver != null)
            {
                Console.WriteLine("\nClosing browser...");
                driver.Quit();
            }
        }
    }


    // LABEL-BASED AND SEMANTIC LOCATOR
    static IWebElement FindControl(
        IWebDriver driver,
        WebDriverWait wait,
        string kind,
        string label,
        string target)
    {
        string script = @"
            const kind = arguments[0].toLowerCase();
            const label = arguments[1].toLowerCase();
            const target = arguments[2].toLowerCase();

            const norm = value =>
                (value || '').replace(/\s+/g, ' ')
                             .trim().toLowerCase();

            let selector = '';

            if (kind === 'text')
                selector = 'input[type=text], input:not([type])';

            if (kind === 'radio')
                selector = 'input[type=radio]';

            if (kind === 'select')
                selector = 'select';

            const visible = el => {
                const style = getComputedStyle(el);

                return style.display !== 'none' &&
                       style.visibility !== 'hidden' &&
                       el.getClientRects().length > 0;
            };

            const matchesTarget = el => {

                if (kind === 'radio')
                    return norm(el.value) === target;

                if (kind === 'select')
                    return [...el.options].some(o =>
                        norm(o.value) === target ||
                        norm(o.textContent) === target
                    );

                return true;
            };

            const all = [...document.querySelectorAll(selector)]
                .filter(el => visible(el) && matchesTarget(el));

            // 1. Find exact visible label text
            const labelNodes = [
                ...document.querySelectorAll(
                    'label, legend, strong, b, span, p, div'
                )
            ].filter(el =>
                norm(el.innerText || el.textContent) === label
            );

            for (const node of labelNodes) {

                // Direct label-to-input association
                if (node.tagName === 'LABEL' && node.htmlFor) {
                    const associated =
                        document.getElementById(node.htmlFor);

                    if (associated &&
                        associated.matches(selector) &&
                        visible(associated) &&
                        matchesTarget(associated)) {
                        return associated;
                    }
                }

                // Search progressively larger nearby containers
                let parent = node;

                for (let depth = 0;
                     parent && depth < 6;
                     depth++) {

                    const found = [
                        ...parent.querySelectorAll(selector)
                    ].filter(el =>
                        visible(el) && matchesTarget(el)
                    );

                    if (found.length === 1)
                        return found[0];

                    parent = parent.parentElement;
                }
            }

            // 2. Semantic fallbacks
            if (kind === 'text') {
                const matches = all.filter(el =>
                    norm(el.placeholder).includes('name') ||
                    norm(el.getAttribute('aria-label')) === label
                );

                if (matches.length === 1)
                    return matches[0];
            }

            // 3. Unique radio value fallback
            if (kind === 'radio' && all.length === 1)
                return all[0];

            // 4. Unique dropdown option fallback
            if (kind === 'select' && all.length === 1)
                return all[0];

            // Never guess if ambiguous
            return null;
        ";

        return wait.Until(d =>
        {
            try
            {
                var result =
                    ((IJavaScriptExecutor)d).ExecuteScript(
                        script, kind, label, target);

                return result as IWebElement;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        }) ?? throw new NoSuchElementException(
            "Unable to uniquely locate " + label);
    }
}
