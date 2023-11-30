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
    public class PurchasesController : Controller
    {
        private nowhereluvBDEntities db = new nowhereluvBDEntities();

        // GET: Purchases
        public ActionResult Index()
        {
            var purchases = db.Purchases.Include(p => p.Suppliers);
            return View(purchases.ToList());
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchases purchases = db.Purchases.Find(id);
            if (purchases == null)
            {
                return HttpNotFound();
            }
            return View(purchases);
        }

        // GET: Purchases/Create
        public ActionResult Create()
        {
            ViewBag.supplier_id = new SelectList(db.Suppliers, "nit", "name");
            return View();
        }

        // POST: Purchases/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "purchase_id,supplier_id,date,product,quantity,unit_cost,total_cost")] Purchases purchases)
        {
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Interconectar con inventario y compras
                        int lastMovement = db.Inventory_movements.Max(x => (int?) x.movement_id) ?? 0;
                        db.Purchases.Add(purchases);
                        db.SaveChanges();

                        // Verificar si el producto existe
                        Inventory existing = db.Inventory.SingleOrDefault(i => i.name == purchases.product);

                        if (existing != null)
                        {
                            existing.stock += purchases.quantity;
                            //Enviando a la tabla inventory_movements
                            Inventory_movements inventory_movements = new Inventory_movements
                            {
                                movement_id = lastMovement + 1,
                                type_movement = "Entrada",
                                product_id = existing.product_id,
                                quantity_moved = purchases.quantity,
                            };
                            db.Inventory_movements.Add(inventory_movements);
                        }
                        else
                        {
                            //Agregando a inventario
                            Inventory inventory = new Inventory
                            {
                                product_id = (db.Inventory.Max(x => (int?)x.product_id) ?? 0) + 1,
                                name = purchases.product,
                                cost = purchases.total_cost,
                                stock = purchases.quantity,
                                purchase_id = purchases.purchase_id,
                            };
                            db.Inventory.Add(inventory);

                            //Enviando a la tabla inventory_movements
                            Inventory_movements inventory_movements = new Inventory_movements
                            {
                                movement_id = lastMovement + 1,
                                type_movement = "Entrada",
                                product_id = inventory.product_id,
                                quantity_moved = purchases.quantity,
                            };
                        }
                        db.SaveChanges();

                        dbContextTransaction.Commit();
                        
                        return RedirectToAction("Index");
                    }
                    catch(Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }

            ViewBag.supplier_id = new SelectList(db.Suppliers, "nit", "name", purchases.supplier_id);
            return View(purchases);
        }

        // GET: Purchases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchases purchases = db.Purchases.Find(id);
            if (purchases == null)
            {
                return HttpNotFound();
            }
            ViewBag.supplier_id = new SelectList(db.Suppliers, "nit", "name", purchases.supplier_id);
            return View(purchases);
        }

        // POST: Purchases/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "purchase_id,supplier_id,date,product,quantity,unit_cost,total_cost")] Purchases purchases)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchases).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.supplier_id = new SelectList(db.Suppliers, "nit", "name", purchases.supplier_id);
            return View(purchases);
        }

        // GET: Purchases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchases purchases = db.Purchases.Find(id);
            if (purchases == null)
            {
                return HttpNotFound();
            }
            return View(purchases);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchases purchases = db.Purchases.Find(id);
            db.Purchases.Remove(purchases);
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
