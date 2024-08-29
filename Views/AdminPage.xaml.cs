using Kalendarzyk.Services;
using Kalendarzyk.Models.EventModels;
using System;
using System.Linq;
using Kalendarzyk.Services.Data;
using Kalendarzyk.Models;

namespace Kalendarzyk.Views
{
    public partial class AdminPage : ContentPage
    {
        private readonly IEventsService _eventsService;
        private readonly IEventRepository _eventRepository;

        public AdminPage()
        {
            InitializeComponent();
            _eventsService = Factory.GetEventService;
            _eventRepository = Factory.GetEventRepository;
        }

        private async void DeleteAllEventGroups_Clicked(object sender, EventArgs e)
        {
            try
            {
                foreach (var group in _eventsService.AllEventGroupsOC.ToList())
                {
                    await _eventsService.DeleteEventGroupAsync(group);
                }
                await DisplayAlert("Success", "All event groups have been deleted.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete event groups: {ex.Message}", "OK");
            }
        }

        private async void DeleteAllEvents_Clicked(object sender, EventArgs e)
        {
            try
            {
                foreach (var eventModel in _eventsService.AllEventsOC.ToList())
                {
                    await _eventsService.DeleteEventAsync(eventModel);
                }
                await DisplayAlert("Success", "All events have been deleted.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete events: {ex.Message}", "OK");
            }
        }

        private async void DeleteAllEventTypes_Clicked(object sender, EventArgs e)
        {
            try
            {
                foreach (var eventType in _eventsService.AllEventTypesOC.ToList())
                {
                    await _eventsService.DeleteEventTypeAsync(eventType);
                }
                await DisplayAlert("Success", "All event types have been deleted.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete event types: {ex.Message}", "OK");
            }
        }

        private async void DeleteAllEventGroups_Clicked_2(object sender, EventArgs e)
        {
            try
            {
                foreach (var group in _eventsService.AllEventGroupsOC.ToList())
                {
                    await _eventsService.DeleteEventGroupAsync(group);
                }
                await DisplayAlert("Success", "All event groups have been deleted.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete event groups: {ex.Message}", "OK");
            }
        }

        private async void AddDummyEvent_Clicked(object sender, EventArgs e)
        {
            try
            {
                var dummyEvent = new EventModel
                {
                    Name = "Dummy Event",
                    EventType = _eventsService.AllEventTypesOC.FirstOrDefault()
                };

                _eventsService.AllEventsOC.Add(dummyEvent);
                await _eventsService.AddEventAsync(dummyEvent);
                await DisplayAlert("Success", "Dummy event added successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add dummy event: {ex.Message}", "OK");
            }
        }

        private async void AddDummyEventType_Clicked(object sender, EventArgs e)
        {
            try
            {
                var dummyEventType = new EventTypeModel
                {
                    EventTypeName = "Dummy EventType",
                    EventGroup = _eventsService.AllEventGroupsOC.FirstOrDefault()
                };

                _eventsService.AllEventTypesOC.Add(dummyEventType);
                await _eventsService.AddEventTypeAsync(dummyEventType);
                await DisplayAlert("Success", "Dummy event type added successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add dummy event type: {ex.Message}", "OK");
            }
        }

        private async void AddDummyEventGroup_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Create a dummy IconModel
                var dummyIcon = new IconModel
                {
                    ElementName = "Car",
                    BackgroundColorString = "#FF5733", // Example color code
                    TextColorString = "#FFFFFF"        // Example color code
                };

                // Optionally add the dummyIcon to the service/repository if needed

                // Create a dummy EventGroupModel
                var dummyEventGroup = new EventGroupModel
                {
                    GroupName = "Dummy EventGroup",
                    SelectedVisualElement = dummyIcon
                };

                var resoult = await _eventsService.AddEventGroupAsync(dummyEventGroup);
                if(resoult.IsSuccess)
                {
                    await DisplayAlert("Success", "Dummy event group added successfully.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", $"Failed to add dummy event group: {resoult.ErrorMessage}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add dummy event group: {ex.Message}", "OK");
            }
        }

    }
}
