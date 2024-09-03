using Kalendarzyk.Models.EventModels;
using System.Collections.ObjectModel;

namespace Kalendarzyk.Views.CustomControls.CCInterfaces.EventTypeExtraOptions
{
	public interface IMicroTasksCC
	{
		string MicroTaskToAddName { get; set; }
		RelayCommand AddMicroTaskEventCommand { get; set; }
		List<MicroTaskModel> MicroTasksOC { get; set; }
		RelayCommand<MicroTaskModel> SelectMicroTaskCommand { get; set; }
	}
}
