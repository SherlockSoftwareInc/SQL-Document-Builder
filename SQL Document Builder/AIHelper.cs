using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Provides helper methods for generating SQL table and column documentation
    /// using a Large Language Model (LLM) API. Includes context and result models,
    /// prompt construction, and LLM API interaction.
    /// </summary>
    public class AIHelper
    {
        /// <summary>
        /// Generates professional and concise descriptions for a SQL table and its columns using a Large Language Model (LLM).
        /// The method sends the provided table context, reference context, and optional database description to the LLM,
        /// receives the completed descriptions, and updates the original context with the results.
        /// </summary>
        /// <param name="context">The <see cref="TableContext"/> containing schema, table, and column information to be documented.</param>
        /// <param name="referenceContext">A JSON string providing additional reference information for related tables or columns.</param>
        /// <param name="databaseDescription">An optional description of the database to provide context for the LLM.</param>
        /// <returns>
        /// <c>true</c> if the descriptions were successfully generated and applied to the context; <c>false</c> if deserialization fails or the LLM response is invalid.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the LLM response does not contain a valid JSON object or required settings are missing.
        /// </exception>
        public async Task<bool> GenerateTableAndColumnDescriptionsAsync(TableContext context, string referenceContext, string? databaseDescription, string additionalInfo)
        {
            const int batchSize = 10;
            int totalColumns = context.Columns.Count;
            bool isFirstColumnSet = true;
            bool anySuccess = false;

            // Store original table description
            string originalTableDescription = context.TableDescription;

            for (int batchStart = 0; batchStart < totalColumns; batchStart += batchSize)
            {
                var batchColumns = context.Columns.GetRange(batchStart, Math.Min(batchSize, totalColumns - batchStart));
                var batchContext = new TableContext
                {
                    TableSchema = context.TableSchema,
                    TableName = context.TableName,
                    TableDescription = context.TableDescription,
                    Columns = batchColumns
                };

                // Compose the prompt for the LLM
                string tableDescriptionPrompt = BuildPrompt(batchContext, referenceContext, databaseDescription, additionalInfo, isFirstColumnSet);

                // Call the LLM
                string llmResponse = await CallLLMAsync(tableDescriptionPrompt);

                // Extract JSON part (find first '{' and last '}')
                int start = llmResponse.IndexOf('{');
                int end = llmResponse.LastIndexOf('}');
                if (start == -1 || end == -1 || end < start)
                {
                    if (isFirstColumnSet) throw new InvalidOperationException("No valid JSON object found in LLM response.");
                    continue;
                }

                string json = llmResponse.Substring(start, end - start + 1);
                System.Diagnostics.Debug.Print(json);

                // Parse JSON into TableContext with error handling
                TableContext? updatedContext = null;
                try
                {
                    updatedContext = JsonSerializer.Deserialize<TableContext>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException)
                {
                    if (isFirstColumnSet) return false;
                    continue;
                }
                catch (Exception)
                {
                    if (isFirstColumnSet) return false;
                    continue;
                }

                if (updatedContext == null)
                {
                    if (isFirstColumnSet) return false;
                    continue;
                }

                // Only update table description from first batch
                if (isFirstColumnSet && !string.IsNullOrWhiteSpace(updatedContext.TableDescription))
                {
                    context.TableDescription = updatedContext.TableDescription;
                }

                // Update column descriptions for this batch
                foreach (var col in batchColumns)
                {
                    var updatedColumn = updatedContext.Columns.Find(c => c.ColumnName == col.ColumnName);
                    if (updatedColumn != null)
                    {
                        var origCol = context.Columns.Find(c => c.ColumnName == col.ColumnName);
                        if (origCol != null)
                        {
                            origCol.Description = updatedColumn.Description;
                        }
                    }
                }

                anySuccess = true;
                isFirstColumnSet = false;
            }

            // Restore original table description if none was updated
            if (string.IsNullOrWhiteSpace(context.TableDescription))
                context.TableDescription = originalTableDescription;

            return anySuccess;
        }

        /// <summary>
        /// Modifies the provided SQL code according to the user's request using a Large Language Model (LLM).
        /// The method composes a prompt including the SQL code and user request, sends it to the LLM,
        /// and returns the modified SQL code.
        /// </summary>
        /// <param name="currentConnection">The current connection.</param>
        /// <param name="selectedObject">The selected object.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="tableBuilderForm">The table builder form.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string?> ModifyCodeAsync(object definition, string userRequest)
        {
            // Extract SQL code from the definition (string or object)
            string sqlCode = definition switch
            {
                string s => s,
                _ => definition?.ToString() ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(sqlCode))
                return string.Empty;

            // Compose the optimized prompt for code modification
            string prompt = $@"Role: Senior Database Engineer and SQL Expert.
Task: Modify the provided SQL code based on the User Request while adhering to strict structural and formatting constraints.

User Request:
{userRequest}

Output Constraints:
1. Pure Plain Text: Use only standard plain text for all prose. Absolutely no markdown (bolding, italics, or backticks) in your conversational explanations.
2. The Comment Rule: All modification notes, logic changes, and explanations must be placed inside a single SQL multi-line comment block /* ... */ at the very beginning of the response.
3. Line Wrap Rule: Within that comment block, manually wrap text so no line exceeds 100 characters.
4. Markdown for Code: Unlike the explanation prose, the SQL script itself MUST be wrapped in a markdown code block (```sql ... ```).

Script Sequencing (Inside the SQL block):
1. IF OBJECT_ID Statement: Drop the object if it exists.
2. GO
3. Documentation Header: {DocumentHeaderInstructions()}
4. CREATE Statement: The modified SQL logic.
5. GO
6. Extended Properties: EXEC usp_addupdateextendedproperty scripts at the end.

Language: {LanguageInstruction()}

Response Structure:
[/* Comment Block with Notes */]
[```sql 
Modified SQL Script
```]

Input Code to Modify:
" + sqlCode;

            var aiHelper = new AIHelper();
            string result = await aiHelper.CallLLMAsync(prompt);
            return result;
        }

        /// <summary>
        /// Optimizes the provided SQL code using a Large Language Model (LLM) by analyzing,
        /// </summary>
        /// <param name="currentConnection">The current connection.</param>
        /// <param name="selectedObject">The selected object.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="tableBuilderForm">The table builder form.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> OptimizeCodeAsync(object definition)
        {
            // Extract SQL code from the definition (string or object)
            string sqlCode = definition switch
            {
                string s => s,
                _ => definition?.ToString() ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(sqlCode))
                return string.Empty;

            // Compose the prompt
            string prompt = $@"Your Role: Act as a Senior Database Engineer and SQL Server Performance Tuning Expert. Your goal is to analyze, debug, and optimize T-SQL queries for maximum efficiency.
Analysis Workflow:
Bug Audit: Identify logical errors, such as Cartesian products (missing join conditions), incorrect NULL handling, or data type mismatches.
Performance Bottlenecks: Identify non-SARGable predicates (e.g., functions on columns in WHERE clauses), implicit conversions, unnecessary subqueries, or redundant joins that trigger full table scans.
Refactoring: Rewrite the query to minimize I/O and CPU usage while strictly preserving the original result set.

Strict Output Constraints:
Plain Text Only: Use standard plain text for all conversational explanations. Do not use backticks or markdown bolding/italics in your prose.
The Comment Rule: All explanations, bug reports, and optimization notes must be contained within a single SQL multi-line comment block /* ... */.
Line Wrap Rule: Within that comment block, manually wrap text so no line exceeds 100 characters.
Script Sequencing: 1. IF OBJECT_ID... DROP statement. 2. GO 3. Documentation Header (after the comment block). 4. CREATE statement. 5. GO 6. EXEC usp_addupdateextendedproperty scripts at the very end.

{DocumentHeaderInstructions()}

{LanguageInstruction()}

SQL Formatting: No markdown code blocks (```sql) for all SQL statements.

Input Code for Optimization::
" + sqlCode;

            // Use the same LLM call as other AI features
            var aiHelper = new AIHelper();
            string result = await aiHelper.CallLLMAsync(prompt);
            return result;
        }

        /// <summary>
        /// Returns the documentation header instructions for SQL code modification and optimization prompts.
        /// </summary>
        /// <returns>A string containing the documentation header instructions.</returns>
        private static string DocumentHeaderInstructions()
        {
            return @"- Documentation Header: Add a header before the CREATE statement using the following format:
-- =============================================
-- Author:      [author name]
-- Create date: [original date]
-- Description: [Briefly summarize what the view/query does here]
-- Modify by:   AI advisor
-- Modify date: [Current Date]
-- =============================================";
        }

        /// <summary>
        /// Languages the instruction.
        /// </summary>
        /// <returns>A string.</returns>
        private static string LanguageInstruction()
        {
            // Get language from settings
            string aiLanguage = AISettingsManager.Current.AILanguage ?? "English";
            string languageInstruction = string.Empty;
            if (!aiLanguage.Equals("English", StringComparison.OrdinalIgnoreCase))
            {
                languageInstruction = $"\nWrite all descriptions in {aiLanguage}.";
            }

            return languageInstruction;
        }

        /// <summary>
        /// Builds the prompt for the LLM using the provided table context.
        /// </summary>
        /// <param name="context">The table context to serialize and include in the prompt.</param>
        /// <returns>A string containing the prompt for the LLM.</returns>
        private static string BuildPrompt(TableContext context, string referenceContext, string? databaseDescription, string additionalInfo, bool isFirstColumnSet = true)
        {
            // Convert context to JSON
            string contextJson = JsonSerializer.Serialize(context, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            string dbDescription = !string.IsNullOrWhiteSpace(databaseDescription)
                ? $"Database context: {databaseDescription}\n\n"
                : string.Empty;

            string referenceInfo = !string.IsNullOrWhiteSpace(referenceContext)
                ? $"Reference data:\n```json\n{referenceContext}\n```\n"
                : string.Empty;

            string additionalInfoBlock = !string.IsNullOrWhiteSpace(additionalInfo)
                ? $"Extra context: {additionalInfo}\n"
                : string.Empty;

            string tableInstruction = isFirstColumnSet
                ? "Table Description: Provide a 1-sentence summary of stored data."
                : "Table Description: DO NOT MODIFY.";

            string prompt = $@"You are a technical database documentation assistant.
{dbDescription}
### Task
Complete the 'Description' fields in the JSON provided below.

### Style Guidelines
* **Ultra-Concise:** Use short, functional phrases (e.g., 'Primary key' or 'Event timestamp'). Avoid 'This column represents...' or 'A unique identifier for...'.
* **Format:** If a column references another table, append 'Reference: [schema].[table].[column]'. Omit if no reference exists or if the object is a View.
* **Integrity:** Do not change table names, column names, or data types. Return only valid JSON.

### Input JSON
```json
{contextJson}
```
{referenceInfo}{additionalInfoBlock}Additional guidelines:

{tableInstruction}
Each column description should explain what the field contains and its purpose.
Do not change schema names, object names, or property names.
If additional information is provided above, use it as extra context for your descriptions.
Output only valid JSON.{LanguageInstruction()}";

            return prompt;
        }

        /// <summary>
        /// Calls the LLM API with the provided prompt and returns the response content.
        /// </summary>
        /// <param name="prompt">The prompt to send to the LLM.</param>
        /// <returns>The response content from the LLM.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if required settings are not set or if no choices are returned from the LLM.
        /// </exception>
        private async Task<string> CallLLMAsync(string prompt)
        {
            // Retrieve settings from AISettingsManager
            var aiSettings = AISettingsManager.Current;
            string endpoint = aiSettings.AIEndpoint ?? throw new InvalidOperationException("OpenAI__Endpoint not set.");
            string model = aiSettings.AIModel ?? throw new InvalidOperationException("OpenAI__Model not set.");
            string apiKey = aiSettings.AIApiKey ?? throw new InvalidOperationException("OpenAI__ApiKey not set.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant for SQL documentation." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.2
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(responseStream);

            // Extract the assistant's reply (OpenAI chat format)
            var root = doc.RootElement;
            var choices = root.GetProperty("choices");
            if (choices.GetArrayLength() == 0)
                throw new InvalidOperationException("No choices returned from OpenAI.");

            var message = choices[0].GetProperty("message");
            var result = message.GetProperty("content").GetString();

            return result ?? string.Empty;
        }

        /// <summary>
        /// Fixes the SQL code based on the verify result using a Large Language Model (LLM).
        /// </summary>
        /// <param name="sqlCode">The sql code.</param>
        /// <param name="verifyResult">The verify result.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string?> FixSQLCodeAsync(string originalCode, string? generatedCode, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(originalCode) || string.IsNullOrWhiteSpace(generatedCode) || string.IsNullOrWhiteSpace(errorMessage))
                return string.Empty;

            // If errorMessage is empty or null, nothing to fix
            if (string.IsNullOrWhiteSpace(errorMessage))
                return originalCode;

            string prompt = $@"Act as a Senior Database Engineer and SQL Server Troubleshooting Expert. You previously generated a SQL script that returned an execution error. Your goal is to perform a root cause analysis and provide the corrected script.

### Input Context
* **Original Intent:** {originalCode}

* **Faulty Generation:** {generatedCode}

* **SQL Server Error Message:** {errorMessage}

### Your Debugging Task
1. **Root Cause Analysis:** Compare the generated code against the error message. Identify if the issue is a syntax error, a binding error (invalid object names), or a logic violation.
2. **Resolution:** Fix the error while ensuring the logic strictly fulfills the original intent.
3. **Refactor & Validate:** Ensure the fix maintains performance best practices (SARGability) and preserves the original result set.

### Constraints & Formatting
* **Plain Text Rule:** Use standard plain text for all explanations—do not use backticks or markdown formatting for conversational text.
* **SQL Formatting:** Use markdown code blocks (```sql) to wrap all SQL statements.
* **The Comment Rule:** All explanations, bug reports, and the Documentation Header must be contained within a single SQL multi-line comment block /* ... */.
* **Line Wrap Rule:** Within the comment block, you must manually wrap the text so that no single line exceeds 100 characters.
* **Script Sequencing:**
    1. Start with the DROP statement: IF OBJECT_ID(N'[schema].[name]', 'V') IS NOT NULL DROP VIEW [schema].[name];
    2. Follow with the GO command.
    3. Insert the multi-line comment block (containing the analysis and the Documentation Header).
    4. Follow with the CREATE statement.
    5. Follow with the GO command.
    6. End with the EXEC usp_addupdateextendedproperty scripts.

Please provide the analysis first, followed by the corrected SQL script.
";

            var aiHelper = new AIHelper();
            string result = await aiHelper.CallLLMAsync(prompt);
            return result;
        }
    }
}