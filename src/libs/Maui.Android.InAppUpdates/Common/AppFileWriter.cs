using Newtonsoft.Json;
using System.Text;

namespace InAppUpdates.iOSAndroid.Maui.Common
{
    /// <summary>
    /// Utility class for reading and writing objects to JSON files
    /// </summary>
    /// <typeparam name="T">The type of object to read/write</typeparam>
    internal static class AppFileWriter<T> where T : class
    {
        /// <summary>
        /// Saves an object to a JSON file in the app's data directory
        /// </summary>
        /// <param name="data">The object to save</param>
        /// <param name="options">Options for in-app updates</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task SaveAsync(T data, InAppUpdatesOptions options)
        {
            try
            {
                // Get the app data directory
                string appDataDir = FileSystem.AppDataDirectory;
                
                // Create the full file path
                string filePath = Path.Combine(appDataDir, 
                                $"{options.AppUpdatePreferenceFileName}.json");
                
                // Serialize the object to JSON
                string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
                
                // Write the JSON to the file
                await File.WriteAllTextAsync(filePath, jsonData, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                options.DebugAction($"Error saving data to file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves an object from a JSON file in the app's data directory
        /// </summary>
        /// <param name="options">Options for in-app updates</param>
        /// <returns>The deserialized object, or null if the file doesn't exist</returns>
        public static async Task<T?> GetAsync(InAppUpdatesOptions options)
        {
            try
            {
                // Get the app data directory
                string appDataDir = FileSystem.AppDataDirectory;
                string filePath = Path.Combine(appDataDir, 
                                $"{options.AppUpdatePreferenceFileName}.json");

                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    options.DebugAction($"File does not exist: {filePath}");

                    return null;
                }

                // Read the JSON from the file
                string jsonData = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

                // De-serialize the JSON to an object
                var result = JsonConvert.DeserializeObject<T>(jsonData);
                options.DebugAction($"Successfully loaded data from {filePath}");
                return result;
            }
            catch (Exception ex)
            {
                options.DebugAction($"Error reading data from file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a JSON file from the app's data directory
        /// </summary>
        /// <param name="options">Options for in-app updates</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static Task DeleteAsync(InAppUpdatesOptions options)
        {
            try
            {
                // Get the app data directory
                string appDataDir = FileSystem.AppDataDirectory;
                string filePath = Path.Combine(appDataDir, 
                            $"{options.AppUpdatePreferenceFileName}.json");
                
                // Delete the file if it exists
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                options.DebugAction($"Error deleting file: {ex.Message}");
                throw;
            }
        }
    }
}