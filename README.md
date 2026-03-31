# SQL Server Script and Document Builder

**SQL Server Script and Document Builder** is a core component of the **Octofy Ecosystem**. While it functions as a robust desktop IDE for SQL Server, its primary mission is to bridge the gap between complex database schemas and AI intelligence. By transforming raw technical structures into rich, context-aware metadata, it ensures that AI agents—such as the **Octofy AI Agent** and the **Octofy Pro** built-in agent—can navigate your data with human-like understanding rather than guesswork.

A trustworthy natural-language-to-SQL workflow begins with high-quality documentation. SQL Document Builder automates the creation of "plain language" descriptions for every table, view, column, and routine, providing the essential roadmap AI models require to generate accurate, reliable code and insights.

---

## 🚀 Key Features

### 🧠 AI-Powered Development & Documentation
- **Auto-Generated Descriptions** – Leverage a built-in LLM to instantly generate or refine table and column metadata for professional, context-aware documentation.
- **Describe Missing** – Automatically fill in only the columns and tables that are missing descriptions, leaving existing documentation untouched. For views with many undescribed columns, the AI first extracts the source base tables from the view definition and inherits their column descriptions—minimizing unnecessary LLM calls.
- **Describe with…** – Generate descriptions with optional additional context or instructions to guide the AI output.
- **Batch Describe** – Process multiple selected objects sequentially in a single operation, saving pending edits between each object and reporting progress.
- **AI Code Optimization** – Automatically review and optimize SQL code for **Views, Stored Procedures, and Functions** to improve performance and readability.
- **Natural Language Refactoring** – Select a database object and enter a request (e.g., "Add a new parameter" or "Change the join logic") to have the AI modify the script for you.

> [!TIP]
> **Why this matters:** High-quality metadata and optimized code are the foundation of accurate AI insights. The more precise your scripts and descriptions, the more effective your entire data ecosystem becomes.

---

### 🔌 Database Connection Management
- **Full Connection Control** – Easily add, edit, and remove SQL Server connections.
- **Session Persistence** – Connections are saved, and the last used connection is automatically restored on startup.
- **Effortless Switching** – Instantly switch between active database connections via a menu or convenient combo box.

---

### 🗂️ Tab-Based SQL Editing
- **Multi-Tab Interface** – Work on up to 128 SQL scripts simultaneously, each in its own dedicated editor tab.
- **Tab Management** – Rename tabs with custom aliases, close tabs individually, or close all at once.
- **Quick File Access** – MRU (Most Recently Used) files list for fast retrieval of recent scripts.

---

### 🔍 Object Browsing and Selection
- **Comprehensive Object Explorer** – Browse tables, views, stored procedures, functions, triggers, synonyms, and more.
- **Advanced Filtering** – Filter by schema or search by name to quickly locate objects.
- **Batch Operations** – Select multiple objects for bulk script generation or documentation.

---

### 📝 Powerful Script Generation
- **Structure Scripts** – Generate `CREATE` scripts for tables, views, and other objects.
- **Data Scripts** – Generate `INSERT` statements with row count checks.
- **Description Export** – Generate object descriptions and export directly to Excel.
- **Automated Documentation** – Export Markdown or JSON docs for tables, views, and query results using templates.
- **Save Object Descriptions** – Persist table and column descriptions back to the database directly from the description editor.

---

### 📄 Template-Based Documentation
- **Flexible Output** – Supports Markdown, HTML, Wiki, SharePoint, and more through customizable templates.
- **Template Management** – Create, edit, and manage templates to define output formats as needed.

---

### 📋 Clipboard and File Integration
- **Data Import** – Import table data or descriptions from Excel or the clipboard.
- **Data Export** – Export object descriptions as formatted Excel files.
- **File Operations** – Open/save SQL, Markdown, HTML, and plain text files.
- **HTML-to-Markdown Paste** – When pasting into a Markdown editor tab, HTML clipboard content is automatically converted to Markdown formatting. Plain text is used as a fallback for non-HTML clipboard data.

---

### ⚡ SQL Execution and Validation
- **Precise Execution** – Run entire scripts or selected statements against active connections.
- **Syntax Checking** – Validate scripts before running to catch errors early.
- **Safety First** – Confirmation dialogs prevent accidental execution of critical operations (e.g. `DROP`).

---

### 🔎 Search & Replace
- **Find & Replace** – Full find-and-replace support with case-insensitive matching and wrap-around search.
- **Replace All (Atomic Undo)** – All replacements in a single Replace All operation are grouped into one undo action.
- **Keyboard Handling** – Pressing `Escape` closes the search panel without inserting stray characters into the editor.

---

### 💻 UI and Usability Enhancements
- **Dark Mode** – Comfortable on the eyes during extended use or in low-light environments.
- **User Feedback** – Progress bar and status updates during long operations.
- **Hotkeys** – Efficient workflows with shortcuts for search, replace, editor functions, etc.
- **Advanced Editor Tools** – Find/replace, quick search panels, text indent/outdent, zoom, case conversion.
- **Customization** – Adjust editor font and appearance to your liking.
- **Smart Change Tracking** – The description editor tracks original versus current values so that saves are skipped when nothing has actually changed, avoiding unnecessary database writes.

---

### ⚙️ Miscellaneous
- **Session Management** – Recent files are remembered for easy reopening.
- **Settings Persistence** – Save preferences for batch sizes, row limits, UI layout, and more between sessions.

---

## 📋 Changelog

### Version 4.0.11
- **AI-Powered Missing Description Autofill** – New **Describe missing** menu option fills only undocumented columns and tables, leaving existing descriptions intact.
- **View Description Intelligence** – For views with many missing column descriptions, the AI parses the view definition to identify source base tables and inherits their column descriptions before calling the LLM, significantly reducing token usage.
- **Batch Describe Improvements** – Sequential object processing with pending-edit saves between each object and live progress feedback.
- **Search/Replace Robustness** – Fixed wrap-around bug; Replace All is now a single atomic undo action; `Escape` no longer inserts a character into the editor.

### Version 4.0.9
- **HTML-to-Markdown Paste** – Pasting from a browser or rich-text source into a Markdown editor tab now automatically converts HTML to Markdown.

### Version 4.0.6 – 4.0.8
- **Describe with…** – Generate descriptions with optional additional context passed to the AI.
- **Save Object Descriptions** – Persist table and column descriptions to the database from the description editor.
- **Smart Change Tracking** – Avoids redundant database writes by comparing current and original description values before saving.
- **HTML-to-Markdown paste** via the [ReverseMarkdown](https://github.com/mysticmind/reversemarkdown-net) library.

---

## 🛠️ Extensibility

SQL Server Script and Document Builder is designed for growth. Adding support for new output formats or programming languages is as simple as creating and loading a new template.

## 📸 Screenshots

### 🖥️ Main Interface with CREATE TABLE Scripts
![Main Interface](SQL%20Document%20Builder/DDLGenerate.png)

### 📂 Markdown Output
![Markdown Document Output](SQL%20Document%20Builder/Markdown.png)

### 📝 Template Editor
![Template Editor](SQL%20Document%20Builder/TemplateEditor.png)

## 📦 Installation

**SQL Server Script and Document Builder** is distributed via a **ClickOnce installer**.  

👉 [Click here to install](https://www.sherlocksoftwareinc.com/SQLDocBuilder/Publish.html)

When you visit the above link, the ClickOnce installer will guide you through the setup process.  

📌 After installation, the application will be available in your **Start Menu** under  
`SQL Server Script and Document Builder`  

📌 The app is installed per-user (not in `C:\Program Files`) and automatically keeps itself up-to-date.



