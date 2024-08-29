using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kalendarzyk.Services.Data;
using Kalendarzyk.ViewModels.CustomControls;
using Kalendarzyk.Services;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
using Kalendarzyk.Helpers;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels.ModelsViewModels;

namespace Kalendarzyk.ViewModels.Event
{
    internal class AddNewEventGroupPageViewModel : BaseViewModel
    {

        private readonly IEventRepository _eventRepository;
        private Dictionary<string, ObservableCollection<string>> _stringToOCMapper;
        private EventGroupModel _currentGroup;
        private bool _isEdit;
        private string lastSelectedIconType = "Top";
        private bool _isIconsTabSelected = true;
        private bool _isBgColorsTabSelected = false;
        private bool _isTextColorsTabSelected = false;
        private IEventsService _eventsService;

        private IColorButtonsSelectorHelperClass _backGroundColorsHelper = Factory.CreateNewIColorButtonsHelperClass(startingColor: Colors.Red);
        public IColorButtonsSelectorHelperClass BackGroundColorsHelper
        {
            get { return _backGroundColorsHelper; }
        }
        public bool IsIconsTabSelected
        {
            get => _isIconsTabSelected;
            set
            {
                _isIconsTabSelected = value;
                OnPropertyChanged();
            }
        }
        public bool IsBgColorsTabSelected
        {
            get => _isBgColorsTabSelected;
            set
            {
                _isBgColorsTabSelected = value;
                OnPropertyChanged();
            }
        }
        public bool IsTextColorsTabSelected
        {
            get => _isTextColorsTabSelected;
            set
            {
                _isTextColorsTabSelected = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SelectableButtonViewModel> MainButtonVisualsSelectors { get; set; }
        public ObservableCollection<SelectableButtonViewModel> IconsTabsOC { get; set; }

        public string SubmitGroupButtonText => _isEdit ? "SUBMIT CHANGES" : "ADD NEW GROUP";
        public string MainTypePlaceholderText => _isEdit ? $"TYPE NEW NAME FOR: {EventGroupName}" : "...NEW GROUP NAME...";


        #region Properties
        public string EventGroupName
        {
            get => _currentGroup.GroupName;
            set
            {
                _currentGroup.GroupName = value;
                OnPropertyChanged();
                AsyncSubmitGroupCommand.RaiseCanExecuteChanged();
            }
        }
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;
                OnPropertyChanged();
            }
        }
        public string SelectedVisualElementString
        {
            get => _currentGroup.SelectedVisualElement.ElementName;
            set
            {
                _currentGroup.SelectedVisualElement.ElementName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> IconsToShowStringsOC { get; set; }
        //public RelayCommand GoToAllMainTypesPageCommand { get; set; }
        public RelayCommand<string> ExactIconSelectedCommand { get; set; }
        public AsyncRelayCommand AsyncSubmitGroupCommand { get; set; }
        public AsyncRelayCommand DeleteAsyncSelectedEventGroupCommand { get; set; }
        #endregion
        private EventGroupViewModel _eventGroup;

        #region Constructors
        //Constructor for create mode
        public AddNewEventGroupPageViewModel()
        {
            _currentGroup = new EventGroupModel();
            IsEdit = false;
            _eventRepository = Factory.GetEventRepository;
            InitializeCommon();
        }
        //Constructor for edit mode
        public AddNewEventGroupPageViewModel(EventGroupModel currentGroupModel)
        {
            IsEdit = true;
            _eventRepository = Factory.GetEventRepository;
            _eventGroup = new EventGroupViewModel(currentGroupModel);
            InitializeCommon();
            EventGroupName = currentGroupModel.GroupName;
            SelectedVisualElementString = currentGroupModel.SelectedVisualElement.ElementName;
            BackGroundColorsHelper.SelectedColor = Color.FromArgb(currentGroupModel.SelectedVisualElement.BackgroundColorString);
            DeleteAsyncSelectedEventGroupCommand = new AsyncRelayCommand(OnDeleteMainTypeCommand);
        }
        #endregion

        #region private methods
        private void InitializeCommon()
        {
            _eventsService = Factory.GetEventService;
            RefreshIconsToShowOC();
            InitializeCommands();
            InitializeSelectors();
        }
        private void InitializeIconsTabs()
        {
            IconsTabsOC = new ObservableCollection<SelectableButtonViewModel>
            {
                new SelectableButtonViewModel("Top", true, new RelayCommand(() => OnExactIconsTabCommand("Top"))),
                new SelectableButtonViewModel("Activities", false, new RelayCommand(() => OnExactIconsTabCommand("Activities"))),
                new SelectableButtonViewModel("IT", false, new RelayCommand(() => OnExactIconsTabCommand("IT"))),
                new SelectableButtonViewModel("Travel", false, new RelayCommand(() => OnExactIconsTabCommand("Travel"))),
                new SelectableButtonViewModel("Others", false, new RelayCommand(() => OnExactIconsTabCommand("Others"))),
            };
            RefreshIconsToShowOC();
            OnPropertyChanged(nameof(IconsTabsOC));
        }
        private void RefreshIconsToShowOC()
        {
            _stringToOCMapper = new Dictionary<string, ObservableCollection<string>>
            {
                { "Top", IconFontsInitialiserHelperClass.GetTopIcons() },
                { "Activities", IconFontsInitialiserHelperClass.GetActivitiesIcons() },
                { "IT", IconFontsInitialiserHelperClass.GetItIcons() },
                { "Travel", IconFontsInitialiserHelperClass.GetTravelIcons()},
                { "Others", IconFontsInitialiserHelperClass.GetOthersIcons() }
            };
        }
        private void InitializeCommands()
        {
            AsyncSubmitGroupCommand = new AsyncRelayCommand(OnSubmitGroupCommand, CanExecuteSubmitGroupCommand);
            ExactIconSelectedCommand = new RelayCommand<string>(OnExactIconSelectedCommand);
        }


        // TODO extract to separate class and make ctor in so that takes 2 params : 1. string name, 2. method for relaycommand
        private void InitializeSelectors()      // TODO CHANGE THIS TO DYNAMIC LIST !!!!!
        {
            SelectedVisualElementString = IconFont.Accessibility;
            MainButtonVisualsSelectors = new ObservableCollection<SelectableButtonViewModel>
            {
                new SelectableButtonViewModel("Icons", false, new RelayCommand<SelectableButtonViewModel>(OnShowIconsTabCommand)),
                new SelectableButtonViewModel("Background Colors", false, new RelayCommand<SelectableButtonViewModel>(OnShowBgColorsCommand)),
            };
        }

        private async Task OnSubmitGroupCommand()
        {
            var iconForEventGroup = Factory.CreateGroupVisualElement(SelectedVisualElementString, BackGroundColorsHelper.SelectedColor, Colors.Black);

            if (_isEdit)
            {
                _currentGroup.SelectedVisualElement = iconForEventGroup;
                var result = await _eventsService.UpdateEventGroupAsync(_currentGroup);

                if (result.IsSuccess)
                {
                    await Shell.Current.GoToAsync(".."); // Navigate back
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", result.ErrorMessage, "OK");
                }
            }
            else
            {
                var newEventGroup = Factory.CreateNewEventGroup(EventGroupName, iconForEventGroup);
                var result = await _eventsService.AddEventGroupAsync(newEventGroup);

                if (result.IsSuccess)
                {
                    await Shell.Current.GoToAsync(".."); // Navigate back
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", result.ErrorMessage, "OK");
                }
            }
        }




        private async Task OnDeleteMainTypeCommand()
        {
            var eventTypesInDb = _eventsService.AllEventTypesOC.Where(x => x.EventGroupId == _currentGroup.Id);
            if (eventTypesInDb.Any())
            {
                var action = await Application.Current.MainPage.DisplayActionSheet("This main type is used...", "Cancel", null, "Delete all associated data", "\n", "Go to All EventTypes Page");
                switch (action)
                {
                    case "Delete all associated data":
                        // Perform the operation to delete all events of the event type.
                        await DeleteEventGroup();
                        await Shell.Current.GoToAsync("..");

                        // TODO make a confirmation message
                        break;
                    case "Go to All EventTypes Page":
                        // Redirect to the All Events Page.
                        await Shell.Current.GoToAsync("AllEventTypesPage");   // TODO SELECT PROPPER MAINEVENTTYPE FOR THE PAGE
                        break;
                    default:
                        // Cancel was selected or back button was pressed.
                        break;
                }
                return;
            }
            else
            {
                await DeleteEventGroup();
                await Shell.Current.GoToAsync("..");
            }
        }
        private bool CanExecuteSubmitGroupCommand()
        {
            return !string.IsNullOrEmpty(EventGroupName);
        }

        private void OnExactIconsTabCommand(string iconType)
        {
            var lastSelectedButton = IconsTabsOC.Single(x => x.ButtonText == iconType);
            OnExactIconsTabClick(lastSelectedButton, _stringToOCMapper[iconType]);
        }
        private void OnExactIconsTabClick(SelectableButtonViewModel clickedButton, ObservableCollection<string> iconsToShowOC)
        {
            SelectableButtonViewModel.SingleButtonSelection(clickedButton, IconsTabsOC);
            lastSelectedIconType = clickedButton.ButtonText;
            IconsToShowStringsOC = iconsToShowOC;
            OnPropertyChanged(nameof(IconsToShowStringsOC));
        }
        private async Task DeleteEventGroup()
        {
            // Perform the operation to delete all events of the event type.
            await _eventsService.DeleteEventGroupAsync(_currentGroup);

        }

        private void OnExactIconSelectedCommand(string visualStringSource)
        {
            SelectedVisualElementString = visualStringSource;
        }
        #endregion
        private void OnShowIconsTabCommand(SelectableButtonViewModel clickedButton)
        {
            SetAllSubTabsVisibilityOff();
            SelectableButtonViewModel.SingleButtonSelection(clickedButton, MainButtonVisualsSelectors);
            IsIconsTabSelected = true;
            InitializeIconsTabs();
            var buttonToSelect = IconsTabsOC.Single(x => x.ButtonText == lastSelectedIconType);
            OnExactIconsTabClick(buttonToSelect, _stringToOCMapper[lastSelectedIconType]);
        }
        private void ClearIconsTabs()
        {
            if (IconsTabsOC != null && IconsTabsOC.Any())
            {
                IconsTabsOC.Clear();
            }
            if (IconsToShowStringsOC != null && IconsToShowStringsOC.Any())
            {
                IconsToShowStringsOC.Clear();
            }
        }
        #region COLOR BUTTONS
        private void OnShowBgColorsCommand(SelectableButtonViewModel clickedButton)
        {
            SetAllSubTabsVisibilityOff();
            IsBgColorsTabSelected = true;
            ClearIconsTabs();
            SelectableButtonViewModel.SingleButtonSelection(clickedButton, MainButtonVisualsSelectors);
        }
        private void OnShowTextColorsCommand(SelectableButtonViewModel clickedButton)
        {
            SetAllSubTabsVisibilityOff();
            IsTextColorsTabSelected = true;
            ClearIconsTabs();
            SelectableButtonViewModel.SingleButtonSelection(clickedButton, MainButtonVisualsSelectors);
        }
        private void SetAllSubTabsVisibilityOff()
        {
            IsTextColorsTabSelected = false;
            IsBgColorsTabSelected = false;
            IsIconsTabSelected = false;
        }
        #endregion



    }
}
