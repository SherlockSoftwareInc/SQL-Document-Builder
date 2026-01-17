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
        public async Task<bool> GenerateTableAndColumnDescriptionsAsync(TableContext context, string referenceContext, string? databaseDescription)
        {
            // Compose the prompt for the LLM
            string tableDescriptionPrompt = BuildPrompt(context, referenceContext, databaseDescription);

            // Call the LLM
            string llmResponse = await CallLLMAsync(tableDescriptionPrompt);

            // Extract JSON part (find first '{' and last '}')
            int start = llmResponse.IndexOf('{');
            int end = llmResponse.LastIndexOf('}');
            if (start == -1 || end == -1 || end < start)
                throw new InvalidOperationException("No valid JSON object found in LLM response.");

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
            catch (JsonException ex)
            {
                // Optionally log the error, e.g.:
                // Console.Error.WriteLine($"Deserialization failed: {ex.Message}");
                return false;
            }
            catch (Exception)
            {
                // Optionally handle other unexpected exceptions
                return false;
            }

            if (updatedContext == null)
                return false;

            // Update the original context with new descriptions
            context.TableDescription = updatedContext.TableDescription;
            for (int i = 0; i < context.Columns.Count; i++)
            {
                var updatedColumn = updatedContext.Columns.Find(c => c.ColumnName == context.Columns[i].ColumnName);
                if (updatedColumn != null)
                {
                    context.Columns[i].Description = updatedColumn.Description;
                }
            }

            return true;
        }

        /// <summary>
        /// Builds the prompt for the LLM using the provided table context.
        /// </summary>
        /// <param name="context">The table context to serialize and include in the prompt.</param>
        /// <returns>A string containing the prompt for the LLM.</returns>
        private string BuildPrompt(TableContext context, string referenceContext, string? databaseDescription)
        {
            // Convert context to JSON
            string contextJson = JsonSerializer.Serialize(context, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            string dbDescription = databaseDescription ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(dbDescription))
            {
                dbDescription = $"Database description: {dbDescription}\n\n";
            }

            string referenceInfo = string.Empty;
            if (!string.IsNullOrWhiteSpace(referenceContext))
            {
                referenceInfo = $@"Reference context:
~~~json
{referenceContext}
~~~
";
            }

            // Get language from settings
            string aiLanguage = AISettingsManager.Current.AILanguage ?? "English";
            string languageInstruction = string.Empty;
            if (!aiLanguage.Equals("English", StringComparison.OrdinalIgnoreCase))
            {
                languageInstruction = $"\nWrite all descriptions in {aiLanguage}.";
            }

            string prompt = $@"You are a technical database documentation assistant.
{dbDescription}
I’ll provide you with a JSON object describing a table and its columns.
Your task is to complete the missing or empty Description fields for both the table and each column, using professional and concise technical language.
You should also review and improve any existing descriptions to ensure clarity and accuracy.
When writing descriptions, you should infer meaning from the table name, column names, and data types.
When a column appears to reference another table or entity, include that in the description as Reference:#reference table and column#; otherwise, do not add Reference. Do not add Reference to the columns in the view. Do not reference the table or view itself.
Keep descriptions clear, factual, and concise; avoid any fluff.
Return the completed JSON in the same structure I provide, without altering existing text or field names.

Here is the input JSON:
~~~json
{contextJson}
~~~
{referenceInfo}
Additional guidelines:

The table description should summarize what the table stores or represents.
Each column description should explain what the field contains and its purpose.
Do not change schema names, object names, or property names.
Output only valid JSON.{languageInstruction}";

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
    }
}