using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoWeb.Models;

namespace ProyectoWeb.Controllers
{
    public class Inventory_movementsController : Controller
    {
        private nowhereluvBDEntities db = new nowhereluvBDEntities();

        // GET: Inventory_movements
        public ActionResult Index()
        {
            var inventory_movements = db.Inventory_movements.Include(i => i.Inventory);
            return View(inventory_movements.ToList());
        }

        // GET: Inventory_movements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory_movements inventory_movements = db.Inventory_movements.Find(id);
            if (inventory_movements == null)
            {
                return HttpNotFound();
            }
            return View(inventory_movements);
        }

        // GET: Inventory_movements/Create
        public ActionResult Create()
        {
            ViewBag.movement_id = new SelectList(db.Inventory, "product_id", "name");
            return View();
        }

        // POST: Inventory_movements/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "movement_id,type_movement,product_id,quantity_moved")] Inventory_movements inventory_movements)
        {
            if (ModelState.IsValid)
            {
                db.Inventory_movements.Add(inventory_movements);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.movement_id = new SelectList(db.Inventory, "product_id", "name", inventory_movements.movement_id);
            return View(inventory_movements);
        }

        // GET: Inventory_movements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory_movements inventory_movements = db.Inventory_movements.Find(id);
            if (inventory_movements == null)
            {
                return HttpNotFound();
            }
            ViewBag.movement_id = new SelectList(db.Inventory, "product_id", "name", inventory_movements.movement_id);
            return View(inventory_movements);
        }

        // POST: Inventory_movements/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "movement_id,type_movement,product_id,quantity_moved")] Inventory_movements inventory_movements)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory_movements).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.movement_id = new SelectList(db.Inventory, "product_id", "name", inventory_movements.movement_id);
            return View(inventory_movements);
        }

        // GET: Inventory_movements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory_movements inventory_movements = db.Inventory_movements.Find(id);
            if (inventory_movements == null)
            {
                return HttpNotFound();
            }
            return View(inventory_movements);
        }

        // POST: Inventory_movements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventory_movements inventory_movements = db.Inventory_movements.Find(id);
            db.Inventory_movements.Remove(inventory_movements);
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
