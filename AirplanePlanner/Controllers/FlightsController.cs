using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using AirplanePlanner.Models;

namespace AirplanePlanner.Controllers
{
    public class FlightsController : Controller
    {

      [HttpGet("/flights")]
       public ActionResult Index()
       {
            List<Flight> allFlights = Flight.GetAll();
           return View(allFlights);
       }
       [HttpGet("/flights/new")]
        public ActionResult CreateForm()
        {
            return View();
        }
        [HttpPost("/flights")]
        public ActionResult Create()
        {
            Flight newFlight = new Flight(Request.Form["flight-name"],45 , "DepartureCity", "ArrivalCity", "OnTime");
            newFlight.Save();
            return RedirectToAction("Success", "Home");
        }


      [HttpGet("/flights/{id}")]
      public ActionResult Details(int id)
      {
         Dictionary<string, object> model = new Dictionary<string, object>();
         Flight selectedFlight = Flight.Find(id);
         List<City> flightCities = selectedFlight.GetCities();
         List<City> allCities = City.GetAll();
         model.Add("selectedFlight", selectedFlight);
         model.Add("flightCities", flightCities);
         model.Add("allCities", allCities);
         return View(model);
      }

      //  [HttpPost("/items")]
      //  public ActionResult CreateItem()
      //  {
      //    Dictionary<string, object> model = new Dictionary<string, object>();
      //    Category foundCategory = Category.Find(Int32.Parse(Request.Form["category-id"]));
      //    Item newItem = new Item(Request.Form["item-description"]);
      //   //  newItem.SetDate(Request.Form["item-due"]);
      //    newItem.Save();
      //    foundCategory.AddItem(newItem);
      //    List<Item> categoryItems = foundCategory.GetItems();
      //    model.Add("items", categoryItems);
      //    model.Add("category", foundCategory);
      //    return View("Details", model);
      //  }
      //

       [HttpPost("/flights/{flightId}/cities/new")]
        public ActionResult AddCity(int flightId)
        {
            Flight flight = Flight.Find(flightId);
            City city = City.Find(Int32.Parse(Request.Form["city-id"]));
            flight.AddCity(city);
            return RedirectToAction("Details",  new { id = flightId });
        }
      //  [HttpPost("/items/{id}/update")]
      //  public ActionResult Update(int id)
      //  {
      //      Item thisItem = Item.Find(id);
      //      thisItem.Edit(Request.Form["newname"]);
      //      return RedirectToAction("Details");
      //  }
    }
}
