using PM.WPF.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PM.WPF.ViewModels
{
    public class ViewModel
    {
        public ObservableCollection<Person> BasicListViewItems { get; set; }
        public ObservableCollection<Header> BasicListViewHeaders { get; set; }

        public ViewModel()
        {
            BasicListViewItems = new ObservableCollection<Person>
            {
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },
                new Person { Name = "John Doe", Company = "ABC Corp", ImagePath = "\\Assets\\background.jpg" },

                new Person { Name = "Jane Smith", Company = "XYZ Inc", ImagePath = "\\Assets\\background.jpg" }
            };
            BasicListViewHeaders = new ObservableCollection<Header>()
            {
                new Header
                {
                    ImageLogoPath = "\\Assets\\logo.png",
                    ImageUserPath= "\\Assets\\background.jpg"
                }

            };
        }

    }

}
