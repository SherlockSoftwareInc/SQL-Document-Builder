
# SQL Server Script and Document Builder

**SQL Server Script and Document Builder** is a comprehensive SQL document and script builder featuring advanced database object browsing, script generation, documentation export (Markdown, WIKI, SharePoint), and multi-tab SQL editing — all integrated with robust connection and template management.  

Because it is a **template-based document builder**, it can easily support any output format or language by simply adding a new template or language definition.

---

## 🚀 Key Features

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

---

### 📄 Template-Based Documentation
- **Flexible Output** – Supports Markdown, HTML, Wiki, SharePoint, and more through customizable templates.
- **Template Management** – Create, edit, and manage templates to define output formats as needed.

---

### 📋 Clipboard and File Integration
- **Data Import** – Import table data or descriptions from Excel or the clipboard.
- **Data Export** – Export object descriptions as formatted Excel files.
- **File Operations** – Open/save SQL, Markdown, HTML, and plain text files.

---

### ⚡ SQL Execution and Validation
- **Precise Execution** – Run entire scripts or selected statements against active connections.
- **Syntax Checking** – Validate scripts before running to catch errors early.
- **Safety First** – Confirmation dialogs prevent accidental execution of critical operations (e.g. `DROP`).

---

### 💻 UI and Usability Enhancements
- **Dark Mode** – Comfortable on the eyes during extended use or in low-light environments.
- **User Feedback** – Progress bar and status updates during long operations.
- **Hotkeys** – Efficient workflows with shortcuts for search, replace, editor functions, etc.
- **Advanced Editor Tools** – Find/replace, quick search panels, text indent/outdent, zoom, case conversion.
- **Customization** – Adjust editor font and appearance to your liking.

---

### ⚙️ Miscellaneous
- **Session Management** – Recent files are remembered for easy reopening.
- **Settings Persistence** – Save preferences for batch sizes, row limits, UI layout, and more between sessions.

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


