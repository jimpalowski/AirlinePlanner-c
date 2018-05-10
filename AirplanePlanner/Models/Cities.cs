using System.Collections.Generic;
using MySql.Data.MySqlClient;
using AirplanePlanner.Models;
using System;

namespace AirplanePlanner.Models
{
  public class City
    {
        private string _description;
        //private string _dueDate;
        private bool _isDone;
        private int _id;


        // We no longer declare _categoryId here

        public City(string description, int id = 0, bool done = false)
        {
            _description = description;
            _id = id;
           // categoryId is removed from the constructor
        }

        public override bool Equals(System.Object otherCity)
        {
          if (!(otherCity is City))
          {
            return false;
          }
          else
          {
             City newCity = (City) otherCity;
             bool idEquality = this.GetId() == newCity.GetId();
             bool descriptionEquality = this.GetDescription() == newCity.GetDescription();
            //  bool dueDateEquality = this.GetDate() == newItem.GetDate();
             // We no longer compare Items' categoryIds in a categoryEquality bool here.
             return (idEquality && descriptionEquality);
           }
        }
        public override int GetHashCode()
        {
             return this.GetDescription().GetHashCode();
        }

        public string GetDescription()
        {
            return _description;
        }

        public int GetId()
        {
            return _id;
        }

        // public string GetDate()
        // {
        //     return _dueDate;
        // }

        public bool GetDone()
        {
            return _isDone;
        }

        public void SetDone(bool maybeDone)
        {
            _isDone = maybeDone;
        }

        // public void SetDate(string newDate)
        // {
        //     _dueDate = newDate;
        // }

        public void AddFlight(Flight newFlight)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO cities_flights (flight_id, city_id) VALUES (@FlightId, @CityId);";

            MySqlParameter flight_id = new MySqlParameter();
            flight_id.ParameterName = "@FlightId";
            flight_id.Value = newFlight.GetId();
            cmd.Parameters.Add(flight_id);

            MySqlParameter city_id = new MySqlParameter();
            city_id.ParameterName = "@CityId";
            city_id.Value = _id;
            cmd.Parameters.Add(city_id);

            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public List<Flight> GetFlights()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT flight_id FROM cities_flights WHERE city_id = @cityId;";

            MySqlParameter cityIdParameter = new MySqlParameter();
            cityIdParameter.ParameterName = "@cityId";
            cityIdParameter.Value = _id;
            cmd.Parameters.Add(cityIdParameter);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;

            List<int> flightIds = new List<int> {};
            while(rdr.Read())
            {
                int flightId = rdr.GetInt32(0);
                flightIds.Add(flightId);
            }
            rdr.Dispose();

            List<Flight> flights = new List<Flight> {};
            foreach (int flightId in flightIds)
            {
                var flightQuery = conn.CreateCommand() as MySqlCommand;
                flightQuery.CommandText = @"SELECT * FROM flights WHERE id = @FlightId;";

                MySqlParameter flightIdParameter = new MySqlParameter();
                flightIdParameter.ParameterName = "@FlightId";
                flightIdParameter.Value = flightId;
                flightQuery.Parameters.Add(flightIdParameter);

                var flightQueryRdr = flightQuery.ExecuteReader() as MySqlDataReader;
                while(flightQueryRdr.Read())
                {
                    int thisFlightId = flightQueryRdr.GetInt32(0);
                    string flightName = flightQueryRdr.GetString(1);
                    string flightStatus = flightQueryRdr.GetString(5);
                    Flight foundFlight = new Flight(flightName, flightStatus, thisFlightId);
                    flights.Add(foundFlight);
                }
                flightQueryRdr.Dispose();
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return flights;
        }

        // We've removed the GetCategoryId() method entirely.

        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO cities (description) VALUES (@description);";

            MySqlParameter description = new MySqlParameter();
            description.ParameterName = "@description";
            description.Value = this._description;
            cmd.Parameters.Add(description);

            // MySqlParameter dueDate = new MySqlParameter();
            // dueDate.ParameterName = "@dueDate";
            // dueDate.Value = this._dueDate;
            // cmd.Parameters.Add(dueDate);

            // Code to declare, set, and add values to a categoryId SQL parameters has also been removed.

            cmd.ExecuteNonQuery();
            _id = (int) cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public static List<City> GetAll()
        {
            List<City> allCities = new List<City> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM cities;";
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
              int cityId = rdr.GetInt32(0);
            //   string itemDueDate = rdr.GetString(2);
              string cityDescription = rdr.GetString(1);
              // We no longer need to read categoryIds from our items table here.
              // Constructor below no longer includes a itemCategoryId parameter:
              City newCity = new City(cityDescription, cityId);
              //newItem.SetDate(itemDueDate);
              allCities.Add(newCity);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allCities;
        }

        public static City Find(int id)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM cities WHERE id = (@searchId);";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = id;
            cmd.Parameters.Add(searchId);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            int cityId = 0;
            string cityName = "";
            //string itemDueDate = "";
            // We remove the line setting a itemCategoryId value here.

            while(rdr.Read())
            {
              cityId = rdr.GetInt32(0);
              cityName = rdr.GetString(1);
          //    itemDueDate = rdr.GetString(2);
              // We no longer read the itemCategoryId here, either.
            }

            // Constructor below no longer includes a itemCategoryId parameter:
            City newCity = new City(cityName, cityId);
          //  newItem.SetDate(ItemDueDate);
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

            return newCity;
        }

        public void UpdateDescription(string newDescription)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"UPDATE cities SET description = @newDescription WHERE id = @searchId;";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = _id;
            cmd.Parameters.Add(searchId);

            MySqlParameter description = new MySqlParameter();
            description.ParameterName = "@newDescription";
            description.Value = newDescription;
            cmd.Parameters.Add(description);

            cmd.ExecuteNonQuery();
            _description = newDescription;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

        }

        public static void Delete(int id)
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"DELETE FROM cities_flights WHERE flight_id = @CityId;";

        MySqlParameter cityIdParameter = new MySqlParameter();
        cityIdParameter.ParameterName = "@CityId";
        cityIdParameter.Value = id;
        cmd.Parameters.Add(cityIdParameter);

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
            cmd.CommandText = @"DELETE FROM cities;";
            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
    }
}
