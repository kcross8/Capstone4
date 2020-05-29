using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Capstone4TaskList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone4TaskList.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TaskListDbContext _context;
        public TasksController(TaskListDbContext context)
        {
            _context = context;
        }
        public IActionResult TasksIndex()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thisUsersTasks = _context.Tasks.Where(x => x.UserId == id).ToList();
            return View(thisUsersTasks);
        }

        [HttpGet]
        public IActionResult AddTask()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddTask(Tasks newTask)
        {
            newTask.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        
            if(ModelState.IsValid)
            {
                _context.Tasks.Add(newTask);
                _context.SaveChanges();
                return RedirectToAction("TasksIndex");
            }
            else
            {
                return View();
            }
        }
        public IActionResult CompleteTask(int id)
        {
            Tasks found = _context.Tasks.Find(id);

            if (found != null)
            {
                found.Complete = !found.Complete;

                _context.Entry(found).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Update(found);
                _context.SaveChanges();
            }
            return RedirectToAction("TasksIndex");
        }
        public IActionResult DeleteTask(int id)
        {
            Tasks found = _context.Tasks.Find(id);
            if(found !=null)
            {
                _context.Tasks.Remove(found);
                _context.SaveChanges();
            }
            return RedirectToAction("TasksIndex");
        }
    }
}