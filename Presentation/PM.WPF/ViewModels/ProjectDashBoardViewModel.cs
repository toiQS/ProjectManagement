using PM.WPF.Models.projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.WPF.ViewModels
{
    public class ProjectDashBoardViewModel
    {
        public ProjectDashBoardViewModel()
        {
            new ProjectDashBoard
            {
                ProjectName = "demo project",
                PercentProgress = 47.5,
                ProjectMember = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        {"Id","demo-user-1" },
                        {"Name","User 1" }
                    },
                    new Dictionary<string, object>()
                    {
                        {"Id","demo-user-1" },
                        {"Name","User 1" }
                    },
                    new Dictionary<string, object>()
                    {
                        {"Id","demo-user-1" },
                        {"Name","User 1" }
                    },
                    new Dictionary<string, object>()
                    {
                        {"Id","demo-user-1" },
                        {"Name","User 1" }
                    }
                },
                ProjectOwner = "Person Owner",
                ProjectStatus = "Active",
                ProjectDescription = "Demo"

            };
        }
        
    }
}
