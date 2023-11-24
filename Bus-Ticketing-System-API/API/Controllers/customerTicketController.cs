﻿using API.Auth;
using BLL.DTOs;
using BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Services.Description;

namespace API.Controllers
{
    [RoutePrefix("api/customer/ticket")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [customerAuth]
    public class customerTicketController : ApiController
    {
        private int getID(HttpRequestMessage request)
        {
            string tokenString = request.Headers.Authorization.ToString();
            return authService.authorizeUser(tokenString).userid;
        }
        [HttpPost]
        [Route("parchase")]
        public HttpResponseMessage purchase(ticketDTO ticket)
        {
            try
            {
                if(customerTicketService.isSeatAvailable(ticket.trip_id, ticket.seat_no) == false) 
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new {message = "Requested seat is not available"});
                }
                var tripObj = customerTicketService.getTripInDetails(ticket.trip_id);
                if(tripObj == null || tripObj.status != "added")
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Trip is not available" });
                }
                if(tripObj.startTime.CompareTo(DateTime.Now) < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The bus has been gone" });
                }
                if(tripObj.startTime.AddHours(-1).CompareTo(DateTime.Now) < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The ticket can not be purchased now (at least 1 hours before)" });
                }
                if(ticket.cupon != null && ticket.cupon != "")
                {
                    ticket.dc_id = customerTicketService.getCuponIDbyString(ticket.cupon);
                    if(ticket.dc_id == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Applied cupon is not valid" });
                    }
                }
                else
                {
                    ticket.dc_id = null;
                }
                ticket.cust_id = getID(Request);
                ticket.seat_no = ticket.seat_no.Distinct().ToList();
                var data = customerTicketService.purchaseTicket(ticket);
                var message = data ? "The ticket is booked" : "The ticket is not booked";
                return Request.CreateResponse(HttpStatusCode.OK, new { message = message});
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            
        }
        [HttpPost]
        [Route("cancel/{ticketID}")]
        public HttpResponseMessage cancel(int ticketID)
        {
            try
            {
                int cust_id = getID(Request);
                if(customerTicketService.isOwnerOfTicket(cust_id, ticketID) == false)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The user is not the owner of this ticket" });
                var obj = customerTicketService.GetTicket(ticketID);
                if (obj.status.Equals("cancelled"))
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The ticket is already cancelled" });
                if (obj.status.Equals("done"))
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The ticket is completed" });
                if (obj.status.Equals("booked") == false)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The ticket can not be cancelled" });
                var data = customerTicketService.cancelTicket(ticketID);
                var message = data ? "The ticket is cancelled" : "The ticket is not cancelled";
                return Request.CreateResponse(HttpStatusCode.OK, new { message = message});
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            
        }
        [HttpGet]
        [Route("get/{ticketID}")]
        public HttpResponseMessage getTicket(int ticketID)
        {
            //try
            {
                int cust_id = getID(Request);
                if(customerTicketService.isOwnerOfTicket(cust_id, ticketID) == false)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The user is not the owner of this ticket" });
                var data = customerTicketService.GetTicket(ticketID);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            //catch (Exception ex)
            {
                //return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpGet]
        [Route("all")]
        public HttpResponseMessage getTicket()
        {
            try
            {
                int cust_id = getID(Request);
                var data = customerTicketService.GetTicketList(cust_id);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}
