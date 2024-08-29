using Kalendarzyk.Models.EventModels;
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
                await _database.CreateTableAsync<IconModel>();
                await _database.CreateTableAsync<EventGroupModel>();
                await _database.CreateTableAsync<EventTypeModel>();
                await _database.CreateTableAsync<EventModel>();
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
            try
            {
                var eventGroups = await _database.Table<EventGroupModel>().ToListAsync();
                foreach (var eventGroup in eventGroups)
                {
                    eventGroup.SelectedVisualElement = await _database.Table<IconModel>()
                        .FirstOrDefaultAsync(icon => icon.Id == eventGroup.SelectedVisualElementId);
                }
                return eventGroups;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve event groups: {ex.Message}");
                return Enumerable.Empty<EventGroupModel>(); // Return an empty list in case of failure
            }
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
            try
            {
                return await _database.Table<EventTypeModel>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve event types: {ex.Message}");
                return Enumerable.Empty<EventTypeModel>(); // Return an empty list in case of failure
            }
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
