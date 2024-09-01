using Kalendarzyk.ViewModels.ModelsViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalendarzyk.ViewModels.CCViewModels
{
    internal class EventGroupsSelectorCCViewModel
    {
        public ObservableCollection<EventGroupViewModel> EventGroups { get; set; }
        public EventGroupsSelectorCCViewModel(ObservableCollection<EventGroupViewModel> eventGroupViewModels)
        {
            EventGroups = eventGroupViewModels;
        }
    }
}
