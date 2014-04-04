using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntelligentFitness.Models;
using IntelligentFitness.DAL;
using System.Net;
using System.Data.Entity.Infrastructure;
using IntelligentFitness.Models.ViewModels;

namespace IntelligentFitness.Controllers
{
    public class GroupController : Controller
    {
        private WorkoutContext db = new WorkoutContext();

        //
        // GET: /Group/
        public ActionResult Index(int? groupID)
        {
            var model = new IntelligentFitness.Models.ViewModels.GroupViewModel();

            model.ExerciseGroups = db.ExerciseGroups
                .Include(g => g.Exercises).ToList()
                .OrderBy(g => g.Created);

            //model.Exercises = db.Exercises.ToList().OrderBy(e => e.ExerciseGroup.Name).OrderBy(p => p.Name);

            if (groupID != null)
            {
                ViewBag.GroupID = groupID;
                //model.Exercises = SelectedGroup.Exercises.Where(
                //    e => e.ExerciseGroup.ID == groupID).ToList().OrderBy(p => p.Created);
            }

            return View(model);
        }

        // POST: /Group/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                // add records
                model.ExerciseGroups.Where(g => (!g.Delete && g.Insert)).ToList().ForEach(g =>
                {
                    ExerciseGroup group;
                    group = new ExerciseGroup();
                    group.Exercises = new List<Exercise>();
                    // MAP NEW PROPERTIES
                    group.Name = g.Name;
                    if (g.Exercises != null)
                    {
                        g.Exercises.ToList().ForEach(e => group.Exercises.Add(new Exercise() { Name = e.Name }));
                    }
                    db.ExerciseGroups.Add(group);
                });

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: /Group/Edit/3
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExerciseGroup group = db.ExerciseGroups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            var model = new GroupViewModel();
            db.Entry(group).Collection(g => g.Exercises).Load();
            model.Exercises = group.Exercises;
            model.SelectedGroupID = id.ToString();
            return View(model);
        }


        // POST: /Group/Edit/3
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(GroupViewModel model, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                var groupToUpdate = db.ExerciseGroups.Include(g => g.Exercises).Single(g => g.ID == id);

                // remove records which came from db and were deleted from the model
                model.Exercises.Where(e => (e.Delete && !e.Insert))
                    .ToList().ForEach(e => db.Exercises.Remove(db.Exercises.Single(ex => ex.ID == e.ID)));

                // add or update records
                model.Exercises.Where(e => !e.Delete).ToList().ForEach(e => {
                    Exercise exer;
                    if (e.Insert)
                    {
                        exer = new Exercise();
                        // MAP NEW PROPERTIES
                        exer.Name = e.Name;
                        exer.ExerciseGroup = groupToUpdate;
                        exer.ExerciseGroupID = groupToUpdate.ID;
                        groupToUpdate.Exercises.Add(exer);
                    }
                    else
                    {
                        exer = groupToUpdate.Exercises.Single(ex => ex.ID == e.ID);
                        // MAP UPDATE PROPERTIES
                        exer.Name = e.Name;
                    }
                });

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: /Group/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExerciseGroup group = db.ExerciseGroups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: /Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExerciseGroup group = db.ExerciseGroups.Find(id);
            db.ExerciseGroups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}