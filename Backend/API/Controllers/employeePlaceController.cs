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

namespace API.Controllers
{
    [RoutePrefix("api/employee/place")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [employeeAuth]
    public class employeePlaceController : ApiController
    {
        private int getID(HttpRequestMessage request)
        {
            string tokenString = request.Headers.Authorization.ToString();
            return authService.authorizeUser(tokenString).userid;
        }
        [HttpGet]
        [Route("all")]
        public HttpResponseMessage allPlace()
        {
            var data = employeePlaceService.allPlace();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        [HttpPost]
        [Route("add")]
        public HttpResponseMessage addPlace(placeDTO obj)
        {
            try
            {
                obj.emp_id = getID(Request);
                var data = employeePlaceService.addPlace(obj);
                string message = data ? "New place is created" : "New place is not created";
                return Request.CreateResponse(HttpStatusCode.OK, new { message = message });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpPut]
        [Route("update")]
        public HttpResponseMessage updatePlace(placeDTO obj)
        {
            try
            {
                if (employeePlaceService.GetPlace(obj.id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The place doesn't exits" });
                }
                obj.emp_id = getID(Request);
                var data = employeePlaceService.updatePlace(obj);
                string message = data ? "place is updated" : "place is not updated";
                return Request.CreateResponse(HttpStatusCode.OK, new { message = message });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpDelete]
        [Route("delete/{id}")]
        public HttpResponseMessage deletePlace(int id)
        {
            try
            {
                if (employeePlaceService.GetPlace(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The place doesn't exits" });
                }
                var data = employeePlaceService.deletePlace(id);
                string message = data ? "place is deleted" : "place is not deleted";
                return Request.CreateResponse(HttpStatusCode.OK, new { message = message });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpGet]
        [Route("get/{id}")]
        public HttpResponseMessage findPlace(int id)
        {
            try
            {
                if (employeePlaceService.GetPlace(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { message = "The place doesn't exits" });
                }
                var data = employeePlaceService.GetPlace(id);
                //string message = data ? "place is deleted" : "place is not deleted";
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
