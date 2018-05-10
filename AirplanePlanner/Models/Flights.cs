using System.Collections.Generic;
using MySql.Data.MySqlClient;
using AirplanePlanner.Models;
using System;

namespace AirplanePlanner.Models
{
  public class Flight
  {
    private string _name;
    private string _status;
    private int _id;
    private List<City> _cities;

    public Flight(string flightName, string flightStatus, int flight_id = 0)
    {
      _name = flightName;
      _status = flightStatus;
      _id = flight_id;

    }

    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        bool idEquality = this.GetId() == newFlight.GetId();
        bool nameEquality = this.GetName() == newFlight.GetName();

        return (idEquality && nameEquality);
      }
    }

    public override int GetHashCode()
    {
         return this.GetName().GetHashCode();
    }

    public List<City> GetCities()
          {
              MySqlConnection conn = DB.Connection();
              conn.Open();
              MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
              cmd.CommandText = @"SELECT cities.* FROM flights
                  JOIN cities_flights ON (flights.id = cities_flights.flight_id)
                  JOIN cities ON (cities_flights.city_id = cities.id)
                  WHERE flights.id = @FlightId;";

              MySqlParameter flightIdParameter = new MySqlParameter();
              flightIdParameter.ParameterName = "@FlightId";
              flightIdParameter.Value = _id;
              cmd.Parameters.Add(flightIdParameter);

              MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
              List<City> cities = new List<City>{};

              while(rdr.Read())
              {
                int cityId = rdr.GetInt32(0);
                string cityDescription = rdr.GetString(1);
                City newCity = new City(cityDescription, cityId);
                cities.Add(newCity);
              }
              conn.Close();
              if (conn != null)
              {
                  conn.Dispose();
              }
              return cities;
          }

    public void AddCity(City newCity)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO cities_flights (flight_id, city_id) VALUES (@FlightId, @CityId);";

            MySqlParameter flight_id = new MySqlParameter();
            flight_id.ParameterName = "@FlightId";
            flight_id.Value = _id;
            cmd.Parameters.Add(flight_id);

            MySqlParameter city_id = new MySqlParameter();
            city_id.ParameterName = "@CityId";
            city_id.Value = newCity.GetId();
            cmd.Parameters.Add(city_id);

            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

    public string GetName()
    {
      return _name;
    }
    public int GetId()
    {
      return _id;
    }
    public string GetStatus()
    {
      return _status;
    }
    public void SetId(int newId)
    {
      _id = newId;
    }

    public void SetName(string newName)
    {
      _name = newName;
    }

    public void SetList(List<City> cities)
    {
      _cities = cities;
    }



    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO flights (name) VALUES (@name);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      // Code to declare, set, and add values to a categoryId SQL parameters has also been removed.

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Flight> GetAll()
    {
      List<Flight> allFlights = new List<Flight> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        string flightName = rdr.GetString(1);
        string flightStatus = rdr.GetString(5);
        Flight newFlight = new Flight(flightName, flightStatus);
        newFlight.SetId(flightId);
        allFlights.Add(newFlight);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allFlights;
    }

    public static Flight Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int flightId = 0;
      string flightName = "";
      string flightStatus = "";
      //string itemDueDate = "";
      // We remove the line setting a itemCategoryId value here.

      while(rdr.Read())
      {
        flightId = rdr.GetInt32(0);
        flightName = rdr.GetString(1);
        flightStatus = rdr.GetString(5);
    //    itemDueDate = rdr.GetString(2);
        // We no longer read the itemCategoryId here, either.
      }

      // Constructor below no longer includes a itemCategoryId parameter:
      Flight newFlight = new Flight(flightName, flightStatus, flightId);
    //  newCategory.SetDate(ItemDueDate);
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }

      return newFlight;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM flights WHERE id = @FlightId; DELETE FROM cities_flights WHERE flight_id = @FlightId;", conn);
      MySqlParameter flightIdParameter = new MySqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();

      cmd.Parameters.Add(flightIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM flights;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
