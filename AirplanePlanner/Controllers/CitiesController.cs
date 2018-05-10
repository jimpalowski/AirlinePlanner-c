using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using AirplanePlanner.Models;
using System;

namespace AirplanePlanner.Controllers
{
    public class CitiesController : Controller
    {
       [HttpGet("/cities")]
       public ActionResult Index()
       {
           List<City> allCities = City.GetAll();
           return View(allCities);
       }

       [HttpGet("/cities/new")]
        public ActionResult CreateForm()
        {
            return View();
        }
        [HttpPost("/cities")]
        public ActionResult Create()
        {
            City newCity = new City(Request.Form["city-description"]);
            newCity.Save();
            return RedirectToAction("Success", "Home");
        }

       [HttpGet("/flights/{flightId}/cities/new")]
       public ActionResult CreateForm(int flightId)
       {
          Dictionary<string, object> model = new Dictionary<string, object>();
          Flight flight = Flight.Find(flightId);
          return View(flight);
       }

       [HttpGet("/cities/{id}")]
        public ActionResult Details(int id)
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            City selectedCity = City.Find(id);
            List<Flight> cityFlights = selectedCity.GetFlights();
            List<Flight> allFlights = Flight.GetAll();
            model.Add("selectedCity", selectedCity);
            model.Add("cityFlights", cityFlights);
            model.Add("allFlights", allFlights);
            return View(model);

        }

       [HttpGet("/flights/{flightId}/cities/{cityId}")]
       public ActionResult Details(int flightId, int cityId)
       {
          City city = City.Find(cityId);
          Dictionary<string, object> model = new Dictionary<string, object>();
          Flight flight = Flight.Find(flightId);
          model.Add("city", city);
          model.Add("flight", flight);
          return View(city);
       }

      [HttpPost("/cities/{cityId}/flights/new")]
        public ActionResult AddFlight(int cityId)
        {
            City city = City.Find(cityId);
            Flight flight = Flight.Find(Int32.Parse(Request.Form["flight-id"]));
            city.AddFlight(flight);
            return RedirectToAction("Details",  new { id = cityId });
        }
       [HttpGet("/cities/{id}/update")]
        public ActionResult UpdateForm(int id)
        {
            City thisCity = City.Find(id);
            return View(thisCity);
        }
        [HttpPost("/cities/{id}/update")]
        public ActionResult Update(int id)
        {
            City thisCity = City.Find(id);
            thisCity.Edit(Request.Form["newname"]);
            return RedirectToAction("Details");
        }
        // [HttpGet("/cities/{id}/delete")]
        // public ActionResult Delete(int id)
        // {
        //   City thisCity = City.Find(id);
        //   thisCity.Delete();
        //   return RedirectToAction("Details");
        // }
        [HttpPost("/cities/{id}/delete")]
         public ActionResult DeleteCity(int id)
         {
             City.Delete(id);
             return RedirectToAction("Details", "Cities", new { id = id });
         }
    }
}
