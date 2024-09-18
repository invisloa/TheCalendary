using Kalendarzyk.Models.EventModels;
using Microsoft.Data.Sqlite;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kalendarzyk.Services.Data
{
    public class SQLiteRepository : IEventRepository
    {
        private readonly SQLiteAsyncConnection _database;

        // Private constructor to force use of the factory method
        private SQLiteRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        // Static factory method
        public static async Task<SQLiteRepository> CreateAsync()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "KalendarzykEvents.db");
            var database = new SQLiteAsyncConnection(databasePath);
            var repository = new SQLiteRepository(database);
            await repository.InitializeDatabaseAsync();
            return repository;
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
            //  await ResetDatabaseAsync();
                await _database.CreateTableAsync<IconModel>();
                await _database.CreateTableAsync<EventGroupModel>();
                await _database.CreateTableAsync<EventTypeModel>();
                await _database.CreateTableAsync<EventModel>();
                await _database.CreateTableAsync<MicroTaskModel>();
                await _database.CreateTableAsync<QuantityModel>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
                throw;
            }
        }

        // Event Methods
        public async Task<OperationResult> AddEventAsync(EventModel eventToAdd)
        {
            try
            {
                await _database.InsertAsync(eventToAdd);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to add event: {ex.Message}");
            }
        }

        public async Task<IEnumerable<EventModel>> GetEventsListAsync()
        {
            try
            {
                return await _database.Table<EventModel>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve events: {ex.Message}");
                return Enumerable.Empty<EventModel>(); // Return an empty list in case of failure
            }
        }

        public async Task<EventModel> GetEventByIdAsync(int eventId)
        {
            try
            {
                return await _database.Table<EventModel>().FirstOrDefaultAsync(e => e.Id == eventId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve event by ID: {ex.Message}");
                return null; // Return null in case of failure
            }
        }

        public async Task<OperationResult> UpdateEventAsync(EventModel eventToUpdate)
        {
            try
            {
                await _database.UpdateAsync(eventToUpdate);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to update event: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteEventAsync(EventModel eventToDelete)
        {
            try
            {
                await _database.DeleteAsync(eventToDelete);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to delete event: {ex.Message}");
            }
        }

        public async Task<OperationResult> ClearAllEventsAsync()
        {
            try
            {
                await _database.DeleteAllAsync<EventModel>();
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to clear all events: {ex.Message}");
            }
        }

        // Event Group Methods
        public async Task<OperationResult> AddEventGroupAsync(EventGroupModel eventGroupToAdd)
        {
            try
            {
                // Insert the IconModel to the database
                await _database.InsertAsync(eventGroupToAdd.SelectedVisualElement);

                // Retrieve the generated ID of the inserted IconModel
                eventGroupToAdd.SelectedVisualElementId = eventGroupToAdd.SelectedVisualElement.Id; // The Id is set automatically after insertion
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to add SelectedVisualElement: {ex.Message}");
            }

            try
            {
                // Insert the EventGroupModel to the database
                await _database.InsertAsync(eventGroupToAdd);
                return OperationResult.Success();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed"))
            {
                // Handle UNIQUE constraint failure by deleting the previously added visual element
                await SafeDeleteVisualElementAsync(eventGroupToAdd.SelectedVisualElement);
                return OperationResult.Failure($"An event group with the name '{eventGroupToAdd.GroupName}' already exists.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions by deleting the previously added visual element
                await SafeDeleteVisualElementAsync(eventGroupToAdd.SelectedVisualElement);
                return OperationResult.Failure($"Failed to add event group: {ex.Message}");
            }
        }
        private async Task SafeDeleteVisualElementAsync(IconModel visualElement)
        {
            try
            {
                await _database.DeleteAsync(visualElement);
            }
            catch
            {
                // Swallow the exception silently since there's nothing more we can do
            }
        }




        public async Task<IEnumerable<EventGroupModel>> GetEventGroupsListAsync()
        {
            var eventGroups = new List<EventGroupModel>();

            try
            {
                var connectionString = $"Data Source={_database.DatabasePath};"; // Use "Data Source=" format

                // Open a connection to the database
                using (var connection = new SqliteConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // SQL query to perform a LEFT JOIN to fetch EventGroupModels with their related IconModels
                    var query = @"SELECT 
                                eg.Id AS EventGroupId, 
                                eg.GroupName, 
                                eg.SelectedVisualElementId, 
                                im.Id AS IconId, 
                                im.ElementName, 
                                im.BackgroundColorString, 
                                im.TextColorString 
                                FROM 
                                EventGroupModel eg 
                                LEFT JOIN 
                                IconModel im 
                                ON 
                                eg.SelectedVisualElementId = im.Id";

                    // Create a command with the SQL query
                    using (var command = new SqliteCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Read each row in the result set
                        while (await reader.ReadAsync())
                        {
                            var eventGroupId = reader.GetInt32(reader.GetOrdinal("EventGroupId"));
                            var groupName = reader.GetString(reader.GetOrdinal("GroupName"));
                            var selectedVisualElementId = reader.GetInt32(reader.GetOrdinal("SelectedVisualElementId"));

                            // Initialize EventGroupModel
                            var eventGroup = new EventGroupModel
                            {
                                Id = eventGroupId,
                                GroupName = groupName,
                                SelectedVisualElementId = selectedVisualElementId
                            };

                            // Initialize IconModel if present
                            if (!reader.IsDBNull(reader.GetOrdinal("IconId")))
                            {
                                eventGroup.SelectedVisualElement = new IconModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("IconId")),
                                    ElementName = reader.GetString(reader.GetOrdinal("ElementName")),
                                    BackgroundColorString = reader.GetString(reader.GetOrdinal("BackgroundColorString")),
                                    TextColorString = reader.GetString(reader.GetOrdinal("TextColorString"))
                                };
                            }

                            // Add the initialized EventGroupModel to the list
                            eventGroups.Add(eventGroup);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve event groups with join: {ex.Message}");
            }

            return eventGroups;
        }


        public async Task<OperationResult> UpdateEventGroupAsync(EventGroupModel eventGroupToUpdate)
        {
            try
            {
                await _database.UpdateAsync(eventGroupToUpdate);
                return OperationResult.Success();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed"))
            {
                return OperationResult.Failure($"An event group with the name '{eventGroupToUpdate.GroupName}' already exists.");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to update event group: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteEventGroupAsync(EventGroupModel eventGroupToDelete)
        {
            try
            {
                await _database.DeleteAsync(eventGroupToDelete);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to delete event group: {ex.Message}");
            }
        }

        public async Task<OperationResult> ClearAllEventGroupsAsync()
        {
            try
            {
                await _database.DeleteAllAsync<EventGroupModel>();
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to clear all event groups: {ex.Message}");
            }
        }

        // Event Type Methods
        public async Task<OperationResult> AddEventTypeAsync(EventTypeModel eventTypeToAdd)
        {
            try
            {
                await _database.InsertAsync(eventTypeToAdd);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to add event type: {ex.Message}");
            }
        }





public async Task<IEnumerable<EventTypeModel>> GetEventTypesListAsync()
    {
        var eventTypes = new List<EventTypeModel>();

        try
        {
            // Correct the connection string format
            var connectionString = $"Data Source={_database.DatabasePath};"; // Use "Data Source=" format

            // Open a connection to the database
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                // SQL query to perform a LEFT JOIN to fetch EventTypeModels with their related EventGroupModels and IconModels
                var query = @"SELECT 
                            et.Id AS EventTypeId, 
                            et.EventGroupId,
                            et.EventTypeName, 
                            et.EventTypeColorString, 
                            et.DefaultEventTimeSpanString,
                            et.IsValueType,
                            et.MeasurementUnit,
                            et.DefaultValue,    
                            et.IsMicroTaskType,
                            eg.Id AS EventGroupId, 
                            eg.GroupName,
                            eg.SelectedVisualElementId,
                            im.Id AS IconId, 
                            im.ElementName, 
                            im.BackgroundColorString, 
                            im.TextColorString 
                            
                          FROM 
                            EventTypeModel et 
                          LEFT JOIN 
                            EventGroupModel eg 
                          ON 
                            et.EventGroupId = eg.Id
                          LEFT JOIN 
                            IconModel im 
                          ON 
                            eg.SelectedVisualElementId = im.Id

";

                // Create a command with the SQL query
                using (var command = new SqliteCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Read each row in the result set
                    while (await reader.ReadAsync())
                    {
                        // Read EventTypeModel data
                        var eventTypeId = reader.GetInt32(reader.GetOrdinal("EventTypeId"));
                        var eventGroupId = reader.GetInt32(reader.GetOrdinal("EventGroupId"));
                        var eventTypeName = reader.GetString(reader.GetOrdinal("EventTypeName"));
                        var eventTypeColorString = reader.GetString(reader.GetOrdinal("EventTypeColorString"));

                        // Safely handle TimeSpan conversion
                        string defaultEventTimeSpan = reader.GetString(reader.GetOrdinal("DefaultEventTimeSpanString"));
                        var isValueType = reader.GetBoolean(reader.GetOrdinal("IsValueType"));
                        var isMicroTaskType = reader.GetBoolean(reader.GetOrdinal("IsMicroTaskType"));


                            // Initialize EventTypeModel
                            var eventType = new EventTypeModel
                            {
                                Id = eventTypeId,
                                EventGroupId = eventGroupId,
                                EventTypeName = eventTypeName,
                                EventTypeColorString = eventTypeColorString,
                                DefaultEventTimeSpanString = defaultEventTimeSpan,
                                DefaultEventTimeSpan = TimeSpan.Parse(defaultEventTimeSpan),
                                IsValueType = isValueType,
                                IsMicroTaskType = isMicroTaskType,
                                DefaultMicroTasks = new List<MicroTaskModel>() // Initialize the list for MicroTasks
                            };
                            var defaultValue = reader.IsDBNull(reader.GetOrdinal("DefaultValue"))
                            ? (decimal?)null
                            : reader.GetDecimal(reader.GetOrdinal("DefaultValue"));
                                                    var measurementUnitString = reader.IsDBNull(reader.GetOrdinal("MeasurementUnit"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("MeasurementUnit"));

                            MeasurementUnit? measurementUnit = null;

                            if (measurementUnitString != null && Enum.TryParse<MeasurementUnit>(measurementUnitString, out var parsedUnit))
                            {
                                measurementUnit = parsedUnit;
                                eventType.DefaultQuantity = new QuantityModel(measurementUnit.Value, defaultValue.Value);
                            }









                            // Initialize EventGroupModel if present
                            if (!reader.IsDBNull(reader.GetOrdinal("EventGroupId")))
                        {
                            var eventGroup = new EventGroupModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EventGroupId")),
                                GroupName = reader.GetString(reader.GetOrdinal("GroupName")),
                                SelectedVisualElementId = reader.GetInt32(reader.GetOrdinal("SelectedVisualElementId")),
                            };

                            // Initialize IconModel if present
                            if (!reader.IsDBNull(reader.GetOrdinal("IconId")))
                            {
                                eventGroup.SelectedVisualElement = new IconModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("IconId")),
                                    ElementName = reader.GetString(reader.GetOrdinal("ElementName")),
                                    BackgroundColorString = reader.GetString(reader.GetOrdinal("BackgroundColorString")),
                                    TextColorString = reader.GetString(reader.GetOrdinal("TextColorString"))
                                };
                            }

                            // Assign the initialized EventGroup to EventType
                            eventType.EventGroup = eventGroup;
                        }

                        // Fetch MicroTasks associated with the current EventType
                        var microTaskQuery = @"SELECT 
                                            mt.Id, 
                                            mt.AssociatedId, 
                                            mt.Type, 
                                            mt.Name, 
                                            mt.IsCompleted 
                                           FROM 
                                            MicroTaskModel mt 
                                           WHERE 
                                            mt.AssociatedId = @EventTypeId AND mt.Type = @TaskType";

                        using (var microTaskCommand = new SqliteCommand(microTaskQuery, connection))
                        {
                            microTaskCommand.Parameters.AddWithValue("@EventTypeId", eventTypeId);
                            microTaskCommand.Parameters.AddWithValue("@TaskType", (int)TaskType.EventType); // Filter by EventType

                            using (var microTaskReader = await microTaskCommand.ExecuteReaderAsync())
                            {
                                while (await microTaskReader.ReadAsync())
                                {
                                    var microTask = new MicroTaskModel
                                    {
                                        Id = microTaskReader.GetInt32(microTaskReader.GetOrdinal("Id")),
                                        AssociatedId = microTaskReader.GetInt32(microTaskReader.GetOrdinal("AssociatedId")),
                                        Type = (TaskType)microTaskReader.GetInt32(microTaskReader.GetOrdinal("Type")),
                                        Name = microTaskReader.GetString(microTaskReader.GetOrdinal("Name")),
                                        IsCompleted = microTaskReader.GetBoolean(microTaskReader.GetOrdinal("IsCompleted"))
                                    };

                                    // Add MicroTask to the EventType's list
                                    eventType.DefaultMicroTasks.Add(microTask);
                                }
                            }
                        }

                        // Add the initialized EventTypeModel to the list
                        eventTypes.Add(eventType);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to retrieve event types: {ex.Message}");
        }

        return eventTypes;
    }




    public async Task<OperationResult> UpdateEventTypeAsync(EventTypeModel eventTypeToUpdate)
        {
            try
            {
                await _database.UpdateAsync(eventTypeToUpdate);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to update event type: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteEventTypeAsync(EventTypeModel eventTypeToDelete)
        {
            try
            {
                await _database.DeleteAsync(eventTypeToDelete);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to delete event type: {ex.Message}");
            }
        }

        public async Task<OperationResult> ClearAllEventTypesAsync()
        {
            try
            {
                await _database.DeleteAllAsync<EventTypeModel>();
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to clear all event types: {ex.Message}");
            }
        }

        // File Operations
        public Task<OperationResult> SaveEventsAndTypesToFile(IEnumerable<EventModel> eventsToSave = null)
        {
            // Implement saving to file logic
            // xxx
            throw new NotImplementedException();
        }

        public Task<OperationResult> LoadEventsAndTypesFromFile()
        {
            // Implement loading from file logic
            // xxx
            throw new NotImplementedException();
        }
        public async Task<OperationResult> ResetDatabaseAsync()
        {
            try
            {
                // Get the path to the database file
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "KalendarzykEvents.db");

                // Close the connection if it's open
                await _database.CloseAsync();

                // Delete the existing database file
                if (File.Exists(databasePath))
                {
                    File.Delete(databasePath);
                }

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to reset the database: {ex.Message}");
            }
        }
        public async Task<OperationResult> ClearAll()
        {
            await ClearAllEventsAsync();
            await ClearAllEventGroupsAsync();
            await ClearAllEventTypesAsync();
            return OperationResult.Success();
        }

    }

    // OperationResult class to encapsulate success or failure of operations
    public class OperationResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        private OperationResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static OperationResult Success() => new OperationResult(true);
        public static OperationResult Failure(string errorMessage) => new OperationResult(false, errorMessage);
    }
}
