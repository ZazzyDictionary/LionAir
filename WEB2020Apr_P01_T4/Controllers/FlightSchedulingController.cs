﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WEB2020Apr_P01_T4.DAL;
using WEB2020Apr_P01_T4.ViewModel;
using WEB2020Apr_P01_T4.Models;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WEB2020Apr_P01_T4.Controllers
{
    public class FlightSchedulingController : Controller
    {


        RouteDAL routeDAL = new RouteDAL();
        FlightScheduleDAL flightScheduleDAL = new FlightScheduleDAL();

      

        // GET: /<controller>/
        public IActionResult Index()
        {
            ScheduleRouteViewModel scheduleRouteViewModel = new ScheduleRouteViewModel
            {
                FlightScheduleList = flightScheduleDAL.GetAllFlightSchedule(),
                RouteList = routeDAL.getAllRoutes(),
                CreateRoute = new Route(),
                CreateSchedule = new FlightSchedule()
                
            };
            return View(scheduleRouteViewModel);
        }

        

        [HttpPost]
        public IActionResult SaveRoute(Route route)
        {
         
            //Insert the data
            routeDAL.insertData(route);
         
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult SaveSchedule(FlightSchedule flightSchedule)
        {

            //Insert the data
            flightScheduleDAL.InsertData(flightSchedule);

            return RedirectToAction("Index");
        }

    }
}
