using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace TestForJPsProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MyObject _selectedListItem;

        private ObservableCollection<MyObject> _checkedListItems =
            new ObservableCollection<MyObject>();
        
        //the property containing the list box item selected by clicking on it
        public MyObject SelectedListItem 
        {
            get
            {
                return _selectedListItem;
            }
            set
            {
                _selectedListItem = value;
                
                RaisePropertyChangedEvent(nameof(SelectedListItem));
                
                //since our evaluation of CanExecute is bound to this property in both commands,
                //we call their RaiseCanExecuteChanged methods to make them & the GUI aware of
                //the changes
                ClickOnListItemCommand.RaiseCanExecuteChanged();
                ConfirmSelectionCommand.RaiseCanExecuteChanged();
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _labelText;
        public string LabelText 
        {
            get
            {
                return _labelText;
            }
            set
            {
                _labelText = value;
                
                RaisePropertyChangedEvent(nameof(LabelText));
            }
        }
        public ObservableCollection<MyObject> MyList { get; set; }

        public ObservableCollection<MyObject> CheckedListItems 
        { 
            get => _checkedListItems;
            set
            {
                _checkedListItems = value;

                RaisePropertyChangedEvent(nameof(CheckedListItems));
            }
        }

        public DelegateCommand ClickOnListItemCommand { get; set; }

        public DelegateCommand ConfirmSelectionCommand { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            LabelText = "There's nothing selected yet!";
            
            MyList = new ObservableCollection<MyObject>() 
            {
                new MyObject() { MyName = "String 1" },
                new MyObject() { MyName = "String 2", IsSelected = true },
                new MyObject() { MyName = "String 3" }
            };

            //at start, we filter MyList for any items with property IsSelected == true
            CheckedListItems = new ObservableCollection<MyObject>(
                MyList.Where(item => item.IsSelected));

            //setting the bound commands with the command action and the canExecute predicate
            ClickOnListItemCommand = new DelegateCommand(
                ClickOnListItem,
                (x) => SelectedListItem != null);

            ConfirmSelectionCommand = new DelegateCommand(
                PrintSelectedItem,
                (x) => SelectedListItem != null);
        }

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(
                this, 
                new PropertyChangedEventArgs(propertyName));
        }
        
        private void PrintSelectedItem()
        {
            if(SelectedListItem == null)
            {
                MessageBox.Show("Nothing selected, so nothing to show here!");

                return;
            }
            
            LabelText = 
                $"The currently selected list item is {SelectedListItem.MyName}!";
        }

        private void ClickOnListItem()
        {
            //when clicking on list box item, its IsSelected property is toggled
            //and it is either added or removed from the list of checked list items
            //note: we could also get same result using the property getter and 
            //have the setter of SelectedListItem notify ClickOnListItem as well
            SelectedListItem.IsSelected = !SelectedListItem.IsSelected;

            if (SelectedListItem.IsSelected)
            {
                _checkedListItems.Add(SelectedListItem);
            }
            else
            {
                _checkedListItems.Remove(SelectedListItem);
            }

            CheckedListItems = 
                new ObservableCollection<MyObject>(_checkedListItems);

            RaisePropertyChangedEvent(nameof(CheckedListItems));
        }
    }

    public class MyObject : INotifyPropertyChanged
    {
        private bool _isSelected = false;
        
        public string MyName { get; set; }

        public bool IsSelected 
            { 
                get => _isSelected; 
                set 
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }

        public int MyOtherProperty { get; set; } 

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this, 
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
