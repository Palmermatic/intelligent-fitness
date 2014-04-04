using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IntelligentFitness.Models.ViewModels
{
    public class GroupViewModel
    {
        public IEnumerable<ExerciseGroup> ExerciseGroups { get; set; }
        public IEnumerable<Exercise> Exercises { get; set; }
        public string SelectedGroupID { get; set; }
    }
}