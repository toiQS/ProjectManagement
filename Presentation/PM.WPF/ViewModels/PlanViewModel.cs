using PM.WPF.Models.plan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.ViewModels
{
    class PlanViewModel
    {
        public ObservableCollection<PlanItem> PlanItemsList { get; set; }

        public PlanViewModel()
        {
            // Initializing with sample data or fetching data from a data source
            PlanItemsList = new ObservableCollection<PlanItem>
            {
                new PlanItem { PlanName = "Sample Plan 1", PlanDescription = "Description 1" },
                new PlanItem { PlanName = "Sample Plan 2", PlanDescription = "Description 2" },
                new PlanItem { PlanName = "Sample Plan 2", PlanDescription = "Description 2" },
                new PlanItem { PlanName = "Sample Plan 2", PlanDescription = "Description 2" },
                new PlanItem { PlanName = "Sample Plan 2", PlanDescription = "Description 2" },
                new PlanItem { PlanName = "Sample Plan 2", PlanDescription = "Description 2" },
                new PlanItem { PlanName = "Sample Plan 2", PlanDescription = "Description 2" }
            };
        }
    }
}
