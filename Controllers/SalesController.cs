using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ProyectoWeb.Models;

namespace ProyectoWeb.Controllers
{
    public class SalesController : Controller
    {
        private nowhereluvBDEntities db = new nowhereluvBDEntities();

        // GET: Sales
        public ActionResult Index()
        {
            var sales = db.Sales.Include(s => s.Inventory).Include(s => s.User);
            return View(sales.ToList());
        }

        // GET: Sales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sales sales = db.Sales.Find(id);
            if (sales == null)
            {
                return HttpNotFound();
            }
            return View(sales);
        }

        // GET: Sales/Create
        public ActionResult Create()
        {
            ViewBag.product_id = new SelectList(db.Inventory, "product_id", "name");
            ViewBag.user_id = new SelectList(db.User, "user_id", "firstname");
            return View();
        }

        // POST: Sales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sales_id,user_id,date,product_id,quantity,unit_cost,total_cost")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                using (var dbContextTrasaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Interconectar con inventario y ventas
                        Inventory inventory = db.Inventory.Find(sales.product_id);
                        int lastIncventoryMovement = db.Inventory_movements.Max(x => (int?)x.movement_id) ?? 0;
                        db.Sales.Add(sales);
                        db.SaveChanges();


                        //Enviando a la tabla inventory_movements
                        Inventory_movements inventory_movements = new Inventory_movements
                        {
                            movement_id = lastIncventoryMovement + 1,
                            type_movement = "Out",
                            product_id = sales.product_id,
                            quantity_moved = sales.quantity
                        };
                        db.Inventory_movements.Add(new Inventory_movements
                        {
                            movement_id = lastIncventoryMovement + 1,
                            type_movement = "Out",
                            product_id = sales.product_id,
                            quantity_moved = sales.quantity
                        });

                        //Actualizando el inventario
                        if (inventory != null)
                        {
                            //Actualizar el stock
                            inventory.stock = inventory.stock - sales.quantity;
                            //Guardar cambios
                            db.SaveChanges();
                        }
                        dbContextTrasaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch(Exception)
                    {
                        dbContextTrasaction.Rollback();
                        throw;
                    }
                }
            }

            ViewBag.product_id = new SelectList(db.Inventory, "product_id", "name", sales.product_id);
            ViewBag.user_id = new SelectList(db.User, "user_id", "firstname", sales.user_id);
            return View(sales);
        }

        // GET: Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sales sales = db.Sales.Find(id);
            if (sales == null)
            {
                return HttpNotFound();
            }
            ViewBag.product_id = new SelectList(db.Inventory, "product_id", "name", sales.product_id);
            ViewBag.user_id = new SelectList(db.User, "user_id", "firstname", sales.user_id);
            return View(sales);
        }

        // POST: Sales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sales_id,user_id,date,product_id,quantity,unit_cost,total_cost")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sales).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.product_id = new SelectList(db.Inventory, "product_id", "name", sales.product_id);
            ViewBag.user_id = new SelectList(db.User, "user_id", "firstname", sales.user_id);
            return View(sales);
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sales sales = db.Sales.Find(id);
            if (sales == null)
            {
                return HttpNotFound();
            }
            return View(sales);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sales sales = db.Sales.Find(id);
            db.Sales.Remove(sales);
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
