using ProyectoWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWeb.Controllers
{
    public class ReporteVentasController : Controller
    {
        nowhereluvBDEntities bddatos = new nowhereluvBDEntities();
        // GET: ReporteVentas
        public ActionResult Index()
        {
            return View(bddatos.Sales.ToList());
        }
    }
}