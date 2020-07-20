using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;
using TechJobsPersistent.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechJobsPersistent.Controllers
{
    public class HomeController : Controller
    {
        private JobDbContext context;

        public HomeController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Job> jobs = context.Jobs.Include(j => j.Employer).ToList();
            if(jobs.Count > 0)
            {
                return View(jobs);
            }
            return View();
        }

        [HttpGet]
        public IActionResult AddJob()
        {
            List<Skill> skills = context.Skills.ToList();
            List<Employer> employers = context.Employers.ToList();
            AddJobViewModel viewModel = new AddJobViewModel(employers, skills);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddJob(AddJobViewModel viewModel, string[] selectedSkills)
        {
            if (ModelState.IsValid)
            {

                Job job = new Job
                {
                    Name = viewModel.Name,
                    EmployerId = viewModel.EmployerId
                };

                List<Job> jobs = context.Jobs.ToList();

                List<int> jobIds = new List<int>();

                int jobId;
                if (jobs.Count == 0)
                {
                    jobId = 1;
                }

                else
                {
                    foreach (Job j in jobs)
                    {
                        jobIds.Add(j.Id);
                    }

                    int maxId = jobIds.Max();
                    jobId = maxId + 1;
                }

                foreach (string skill in selectedSkills)
                {
                    int skillId = int.Parse(skill);

                    JobSkill jobSkill = new JobSkill
                    {
                        JobId = jobId,
                        SkillId = skillId
                    };
                    
                    context.JobSkills.Add(jobSkill);
                }

                context.Jobs.Add(job);               
                context.SaveChanges();
                return Redirect("/");

            }
            return View();
        }

        public IActionResult Detail(int id)
        {
            Job theJob = context.Jobs
                .Include(j => j.Employer)
                .Single(j => j.Id == id);

            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.JobId == id)
                .Include(js => js.Skill)
                .ToList();

            JobDetailViewModel viewModel = new JobDetailViewModel(theJob, jobSkills);
            return View(viewModel);
        }
    }
}
