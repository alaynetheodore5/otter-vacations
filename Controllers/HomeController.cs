using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OtterVacations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace OtterVacations.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context { get; set; }

        public HomeController(MyContext context)
        {
            _context = context;
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.Otters = _context.Otters.ToList();
            // this allows us to push viewbag to the cshtml page
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(Otter newbie)
        {
            if(ModelState.IsValid)
            {
                // this will return a boolean
                if(_context.Otters.Any( o => o.Name == newbie.Name))
                {
                    ModelState.AddModelError("Name", "Choose anotter name!!!");
                    ViewBag.Otters = _context.Otters.ToList();
                    //important to have
                    return View("Index");
                }
                else
                {
                    _context.Otters.Add(newbie);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("OtterId", newbie.OtterId);
                    return Redirect($"/dashboard/{newbie.OtterId}");
                }
            }
            else
            {
                ViewBag.Otters = _context.Otters.ToList();
                //very important to have in all where it is index
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(int OtterId)
        {
            HttpContext.Session.SetInt32("OtterId", OtterId);
            //set user in session
            return Redirect($"/dashboard/{OtterId}");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("dashboard/{otterId}")]
        public IActionResult Dashboard( int otterId)
        {
            if(HttpContext.Session.GetInt32("OtterId") == null)
            {
                return RedirectToAction("Logout");
                //this checks if there is a user in session and if not, redirects to logging out method.
            }
            Otter otterInDb = _context.Otters
                            .Include( o => o.PlannedVacations)
                            .FirstOrDefault( o => o.OtterId == otterId);
            ViewBag.myVacays = otterInDb.PlannedVacations
                            .OrderBy( v => v.StartDate).ToList();
            ViewBag.Otter = otterInDb;
            List<Vacation> AllTrips = _context.Vacations
                            .Include(v => v.GroupMembers)
                            .ThenInclude(a => a.Traveller)
                            //you need to switch from v to a because you are now switching to a different model
                            .Include(v => v.Planner)
                            .ToList();
            return View(AllTrips);
        }

        [HttpGet("vacation/new")]
        public IActionResult AddVacation()
        {
            return View();
        }

        [HttpPost("create/vacation")]
        public IActionResult CreateVacation(Vacation newVac)
        {
            if(ModelState.IsValid)
            {
                int? otterId = HttpContext.Session.GetInt32("OtterId");
                //the question mark allows it to be null (if there is no user logged in)
                List<Vacation> myVacays = _context.Otters
                    .Include(o => o.PlannedVacations)
                    .FirstOrDefault(o => o.OtterId == (int) otterId)
                    .PlannedVacations.ToList();
                if(IsAlreadyBooked(newVac, myVacays))
                {
                    ModelState.AddModelError("StartDate", "You are already on a vacation at this time");
                    return View("AddVacation");
                }
                newVac.OtterId = (int)otterId; // foreign key AND need to cast back into an int
                _context.Vacations.Add(newVac);
                _context.SaveChanges();
                return Redirect($"/dashboard/{otterId}");
            }
            else
            {
                return View("AddVacation");
            }
        }

        public static bool IsAlreadyBooked(Vacation newVac, List<Vacation> vacations)
        {
            DateTime testStart = newVac.StartDate;
            DateTime testEnd = newVac.StartDate.AddDays(newVac.NumberDays);
            foreach(var v in vacations)
            {
                DateTime start = v.StartDate;
                DateTime end = v.StartDate.AddDays(v.NumberDays);
                // no part of the test start or test end can overlap this block of time
                if(start <= testEnd && testStart <= end)
                {
                    return true;
                }
            }
            return false;
        }

        [HttpGet("join/{vacationId}/{otterId}")]
        public IActionResult JoinVacation( int vacationId, int otterId)
        {
            Association join = new Association();
            join.VacationId = vacationId;
            join.OtterId = otterId;
            _context.Associations.Add(join);
            _context.SaveChanges();
            return Redirect($"/dashboard/{otterId}");
        }

        [HttpGet("leave/{vacationId}/{otterId}")]
        public IActionResult LeaveVacation( int vacationId, int otterId)
        {
            Association leave = _context.Associations.FirstOrDefault(a => a.VacationId == vacationId && a.OtterId == otterId);
            _context.Associations.Remove(leave);
            _context.SaveChanges();
            return Redirect($"/dashboard/{otterId}");
        }

        [HttpGet ("delete/{vacationId}/vacation")]
        public IActionResult CancelVacation (int vacationId)
        {
            Vacation toDelete = _context.Vacations
                .FirstOrDefault (v => v.VacationId == vacationId);
            _context.Vacations.Remove (toDelete);
            _context.SaveChanges ();
            int? otterId = HttpContext.Session.GetInt32 ("OtterId");
            return Redirect ($"/dashboard/{otterId}");
        }

        [HttpGet ("vacation/{vacationId}")]
        public IActionResult ShowVacation (int vacationId)
        {
            Vacation toShow = _context.Vacations
                .Include( v => v.Planner)
                .Include(v => v.GroupMembers)
                .ThenInclude(a => a.Traveller)
                .FirstOrDefault (v => v.VacationId == vacationId);
            ViewBag.toShow = toShow;
            return View("ShowVacation", toShow);
        }

    }
}
