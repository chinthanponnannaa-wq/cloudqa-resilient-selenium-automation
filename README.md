
# CloudQA Resilient Selenium Automation

A C# Selenium WebDriver project that automates form testing using semantic and contextual element identification.

## Features

- Automated First Name entry
- Gender radio button selection
- State dropdown selection
- Context-based element identification
- Configurable target URL
- Automatic field verification
- PASS/FAIL execution reporting

## Tech Stack

- C#
- .NET 10
- Selenium WebDriver 4
- Google Chrome

## Installation

Clone the repository:

```bash
git clone: https://github.com/chinthanponnannaa-wq/cloudqa-resilient-selenium-automation
```

Navigate to the project:

```bash
cd CloudQAAutomation
```

Install dependencies:

```bash
dotnet restore
```

## Run

```bash
dotnet build
dotnet run
```

## Custom URL

```bash
dotnet run -- "https://your-test-site.com"
```

Alternatively:

```powershell
$env:CLOUDQA_URL="https://your-test-site.com"
dotnet run
```

## Test Fields

| Field | Test Value |
|---|---|
| First Name | Chinthan |
| Gender | Male |
| State | India |

## Limitations

- Locator reliability depends on accessible labels and surrounding semantic context.
- Completely unrelated or ambiguous page structures may require additional locator strategies.
- The original CloudQA practice form has been tested; modified page variants still require validation.

## Author

Chinthan A D
