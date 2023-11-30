using Microsoft.Reporting.WebForms;
using ProyectoWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ProyectoWeb.Controllers
{
    public class ReporteVentasController : Controller
    {
        nowhereluvBDEntities bddatos = new nowhereluvBDEntities();
        // GET: ReporteVentas
        public ActionResult Index()
        {
            var sales = bddatos.Sales.Include(s => s.Inventory).Include(s => s.User);
            return View(sales.ToList());
        }
        public ActionResult VistaReporteVentas(string id ) { 
        
            LocalReport reporte = new LocalReport();
            string ruta = Path.Combine(Server.MapPath("~/Reportes"), "ReportVentas.rdlc");
            reporte.ReportPath = ruta;
            List<Sales> listado = new List<Sales>();
            listado= bddatos.Sales.ToList();
            ReportDataSource verreporte = new ReportDataSource("Datos_Venta", listado);
            reporte.DataSources.Add(verreporte);
            String tiporeporte = id;
            string mime, codificacion, archivo;
            string[] flujo;
            Warning[] aviso;
            string dispositivo = @"<DeviceInfo>" +
             " <OutputFormat>" + id + "</OutputFormat>" +
             " <PageWidth>8.5in</PageWidth>" +
             " <PageHeight>11in</PageHeight>" +
             " <MarginTop>0.5in</MarginTop>" +
             " <MarginLeft>1in</MarginLeft>" +
             " <MarginRight>1in</MarginRight>" +
             " <MarginBottom>0.5in</MarginBottom>" +
             " <EmbedFonts>None</EmbedFonts>" +
             "</DeviceInfo>";
            byte[] enviar = reporte.Render(id, dispositivo, out mime, out codificacion, out archivo, out flujo, out aviso);                   
            return File(enviar, mime);
        }
    }
}