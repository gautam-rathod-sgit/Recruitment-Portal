using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobPostViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "JobTitle")]
        [Required(ErrorMessageResourceName = "JobTitleRequired", ErrorMessageResourceType = typeof(Messages))]
        public string job_title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Location")]
        [Required(ErrorMessageResourceName = "LocationRequired", ErrorMessageResourceType = typeof(Messages))]
        public string location { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "JobRole")]
        [Required(ErrorMessageResourceName = "JobRoleRequired", ErrorMessageResourceType = typeof(Messages))]
        public string job_role { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Vacancy")]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "ValidVacancyRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "VacancyRequired", ErrorMessageResourceType = typeof(Messages))]
        public int vacancy { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "JobType")]
        [Required(ErrorMessageResourceName = "JobTypeRequired", ErrorMessageResourceType = typeof(Messages))]
        public string job_type { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "EligibilityCriteria")]
        [Required(ErrorMessageResourceName = "EligibilityCriteriaRequired", ErrorMessageResourceType = typeof(Messages))]
        public string eligibility_criteria { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Experience")]
        [Required(ErrorMessageResourceName = "ExperienceRequired", ErrorMessageResourceType = typeof(Messages))]
        public string experience { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Skills")]
        [Required(ErrorMessageResourceName = "SkillsRequired", ErrorMessageResourceType = typeof(Messages))]
        public string skills { get; set; }

        public bool isActive { get; set; }
        //for counter part of available jobs on Home page
        public int AvailableJobsCount { get; set; }
        //relationship with JobCategoryViewModel
        public int JobCategoryId { get; set; }
        //vacancy_fulfilled counter
        public bool vacancy_overflow { get; set; }
        public string categoryId { get; set; }
    }
}
