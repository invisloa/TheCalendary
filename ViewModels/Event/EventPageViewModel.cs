using Kalendarzyk.Models.EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalendarzyk.ViewModels.Event
{
    public class EventPageViewModel : BaseViewModel
    {

        private bool _isEditMode;
        private DateTime _selectedDate;

        private EventModel _eventToEdit;

        //ctor for adding events
        public EventPageViewModel(DateTime selectedDate)
        {
            _selectedDate = selectedDate;
            _isEditMode = false;
        }

        //ctor for editing events
        public EventPageViewModel(EventModel eventToEdit)
        {
            _eventToEdit = eventToEdit;
            _selectedDate = eventToEdit.StartDateTime;
            _isEditMode = true;
        }











    }
}
