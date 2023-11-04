﻿using AutoMapper;
using BLL.DTOs;
using DAL.EF.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class employeeTripService
    {
        public static List<tripDTO> allTrip()
        {
            var data = DataAccessFactory.getTrip().All();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<trip, tripDTO>());
            var mapper = config.CreateMapper();
            return mapper.Map<List<tripDTO>>(data);
        }
        private static List<int> convertSeat(string seats)
        {
            return seats.Split(',').Select(s => Convert.ToInt32(s)).ToList();
        }
        private static string convertSeat(List<int> seats)
        {
            return string.Join(",", seats.Select(s => s.ToString()));
        }
        public static List<tripInDetailsDTO> allTripDetails()
        {
            var tripData = DataAccessFactory.getTrip().All();
            var config = new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<trip, tripInDetailsDTO>()
                        .ForMember
                        (
                            dst => dst.bookedSeat,
                            opt => opt.MapFrom
                            (
                                src => src.tickets
                                .Where(t => t.status.Equals("booked") || t.status.Equals("done"))
                                .SelectMany(t => convertSeat(t.seat_no)).ToList()
                            )
                        )
                        ;
                        cfg.CreateMap<place, placeDTO>();
                    }
                );
            var mapper = config.CreateMapper();
            return mapper.Map<List<tripInDetailsDTO>>(tripData.OrderByDescending(t => t.id));
        }
        public static List<tripInDetailsDTO> searchTrip(string search)
        {
            var convertedData = allTripDetails();
            search = search.ToLower();
            var searchedData = convertedData.Where(
                t =>
                t.id.ToString().ToLower().Contains(search)
                || t.id.ToString().ToLower().Contains(search)
                || t.ticketPrice.ToString().ToLower().Contains(search)
                || t.status.ToString().ToLower().Contains(search)
                || t.startTime.ToString().ToLower().Contains(search)
                || t.endTime.ToString().ToLower().Contains(search)
                || t.depot.name.ToString().ToLower().Contains(search)
                || t.destination.name.ToString().ToLower().Contains(search)
                );
            return searchedData.OrderByDescending(t => t.id).ToList();
        }
        public static tripDTO GetTrip(int id)
        {
            var data = DataAccessFactory.getTrip().get(id);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<trip, tripDTO>());
            var mapper = config.CreateMapper();
            return mapper.Map<tripDTO>(data);
        }

        // add amount to account
        private static bool addAccount(int bp_id, int ammount, string details)
        {
            var obj = new transaction()
            {
                details = "Added: " + details,
                amount = ammount,
                time = DateTime.Now,
                userID = bp_id
            };
            return DataAccessFactory.getTransaction().create(obj);
        }
        public static bool acceptCancelTrip(int tripID)
        {
            var tripData = DataAccessFactory.getTrip().get(tripID);
            foreach(var tk in tripData.tickets)
            {
                tk.status = "cancelled";
                DataAccessFactory.getTicket().update(tk);
                addAccount(tk.cust_id, tk.ammount, "Refunded");
            }
            foreach (var tk in tripData.tickets)
            {
                tk.status = "cancelled";
                DataAccessFactory.getTicket().update(tk);
            }
            tripData.status = "cancelled";
            return DataAccessFactory.getTrip().update(tripData);
        }
        public static bool acceptAddTrip(int tripID)
        {
            var tripData = DataAccessFactory.getTrip().get(tripID);
            tripData.status = "added";
            return DataAccessFactory.getTrip().update(tripData);
        }
        public static bool doneTrip(int tripID)
        {
            var tripData = DataAccessFactory.getTrip().get(tripID);
            tripData.status = "done";
            int ammount = tripData.tickets.Select(t => t.ammount).Sum();
            foreach(var tk in tripData.tickets)
            {
                tk.status = "done";
                DataAccessFactory.getTicket().update(tk);
            }
            addAccount(tripData.bus.bp_id, ammount, "Done trip");
            return DataAccessFactory.getTrip().update(tripData);
        }
        /*public static bool updateTrip(tripDTO obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<tripDTO, trip>());
            var mapper = config.CreateMapper();
            var newObj = mapper.Map<trip>(obj);
            return DataAccessFactory.getTrip().update(newObj);
        }*/
    }
}
