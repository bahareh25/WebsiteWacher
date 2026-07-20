# 🌐 Website Watcher

Website Watcher is a **serverless website monitoring application** built with **Azure Functions**. It enables users to monitor specific content on websites by registering a URL and an XPath expression. The application periodically checks for changes, stores snapshots, generates PDF versions of the content, and allows users to query the latest updates.

## 🚀 Features

- Register websites for monitoring
- Track specific webpage content using XPath
- Capture and store website snapshots
- Automatically generate PDF versions of webpage content
- Detect content changes on a scheduled basis
- Store PDFs in Azure Blob Storage
- Persist monitoring data in SQL Database
- Query the latest website updates through a REST API

---

## 🏗️ Architecture

```
                  HTTP Request
                       │
                       ▼
               Register Function
                       │
                       ▼
                 Azure SQL Database
                       ▲
                       │
        ┌──────────────┼──────────────┐
        │                              │
        ▼                              ▼
 Snapshot Function             Watcher Function
        │                              │
        ▼                              ▼
 Azure SQL Database          Detect Content Changes
                                      │
                     ┌────────────────┴──────────────┐
                     ▼                               ▼
              Update Snapshot               PDF Creator Function
                     │                               │
                     ▼                               ▼
               Azure SQL Database         Azure Blob Storage
                                                     │
                                                     ▼
                                             Query Function
```

---

## ⚙️ Azure Functions

### 1. Register Function

**Trigger:** HTTP Trigger

Registers a website for monitoring by accepting:

- Website URL
- XPath expression identifying the content to monitor

The function validates the request and stores the monitoring configuration in **Azure SQL Database** using a SQL Output Binding.

---

### 2. Snapshot Function

Obtains the current content from the registered website and stores the initial snapshot in the database.

Responsibilities:

- Download webpage content
- Extract content using XPath
- Save snapshot
- Record timestamp

---

### 3. PDF Creator Function

Creates a PDF version of the monitored webpage content.

Responsibilities:

- Generate PDF
- Store PDF in Azure Blob Storage
- Associate the PDF with the corresponding website snapshot

---

### 4. Watcher Function

**Trigger:** Timer Trigger

Runs on a scheduled interval to monitor registered websites.

Responsibilities:

- Retrieve registered websites
- Download current webpage content
- Compare with the latest stored snapshot
- Detect changes
- Save updated snapshot
- Generate a new PDF
- Upload the PDF to Azure Blob Storage

---

### 5. Query Function

**Trigger:** HTTP Trigger

Provides an API for retrieving the latest monitoring results.

Users can query:

- Latest snapshot
- Last updated time
- Current monitored content
- PDF location
- Change history

---

## 🛠️ Technology Stack

### Backend

- Azure Functions (.NET)
- C#
- Azure SQL Database
- Azure Blob Storage

### Azure Services

- HTTP Trigger
- Timer Trigger
- SQL Input/Output Bindings
- Blob Storage Bindings

### Libraries

- HtmlAgilityPack (XPath parsing)
- PDF generation library (e.g., QuestPDF or iText7)

---

## 📂 Project Structure

```
WebsiteWatcher
│
├── RegisterFunction
├── SnapshotFunction
├── PdfCreatorFunction
├── WatcherFunction
├── QueryFunction
│
├── Models
├── Services
├── Helpers
└── Shared
```

---

## 🔄 Workflow

1. User registers a website URL and XPath.
2. Registration is saved in Azure SQL Database.
3. Snapshot Function captures the initial webpage content.
4. PDF Creator stores a PDF copy in Azure Blob Storage.
5. Watcher Function periodically checks for content changes.
6. If changes are detected:
   - A new snapshot is saved.
   - A new PDF is generated.
   - The PDF is uploaded to Blob Storage.
7. Users retrieve the latest monitoring results through the Query Function.

---

## ☁️ Azure Resources

- Azure Function App
- Azure SQL Database
- Azure Blob Storage
- Azure Storage Account
- Application Insights (optional)

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Azure Functions Core Tools
- Azure Storage Emulator (Azurite)
- Azure SQL Database

### Clone the repository

```bash
git clone https://github.com/yourusername/WebsiteWatcher.git

cd WebsiteWatcher
```

### Run locally

```bash
func start
```

---

## 📈 Future Enhancements

- Email notifications when changes are detected
- SMS or Microsoft Teams notifications
- Azure Service Bus integration
- Authentication with Azure AD
- Support for multiple XPath expressions
- Change history dashboard
- Unit and integration tests
- Docker deployment

---

## 📄 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

**Tahereh Boroumandnejad**

Senior Software Engineer

- C#
- .NET
- Azure Functions
- Azure SQL
- Azure Blob Storage
- Serverless Architecture
-
- # TimerTrigger - C<span>#</span>

The `TimerTrigger` makes it incredibly easy to have your functions executed on a schedule. This sample demonstrates a simple use case of calling your function every 5 minutes.

## How it works

For a `TimerTrigger` to work, you provide a schedule in the form of a [cron expression](https://en.wikipedia.org/wiki/Cron#CRON_expression)(See the link for full details). A cron expression is a string with 6 separate expressions which represent a given schedule via patterns. The pattern we use to represent every 5 minutes is `0 */5 * * * *`. This, in plain text, means: "When seconds is equal to 0, minutes is divisible by 5, for any hour, day of the month, month, day of the week, or year".

## Learn more

<TODO> Documentation
