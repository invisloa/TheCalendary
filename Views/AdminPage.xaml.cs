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
    }
}
