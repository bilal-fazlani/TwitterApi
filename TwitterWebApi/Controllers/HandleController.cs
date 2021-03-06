﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitterWebApi.ExternalServices.Handle;
using TwitterWebApi.Models;

namespace TwitterWebApi.Controllers
{
    public class HandleController : Controller
    {
        private readonly IHandleService _handleService;

        public HandleController(IHandleService handleService)
        {
            _handleService = handleService;
        }

        [HttpGet]
        [Route("/api/handle/list")]
        public async Task<IActionResult> List()
        {
            var handles =
                (await _handleService.Gethandles())
                    .Select(x => new
                {
                    x.name,
                    id = x._id.ToString()
                });

            return Json(handles);
        }

        [HttpPost]
        [Route("/api/handle")]
        public async Task<IActionResult> Add([FromBody]Handle handle)
        {
            await _handleService.AddHandle(handle);
            return StatusCode(((int) HttpStatusCode.Created), new
            {
                id = handle._id.ToString(),
                handle.localId
            });
        }

        [HttpDelete]
        [Route("/api/handle/{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            await _handleService.RemoveHandle(id);
            return StatusCode((int) HttpStatusCode.OK);
        }
    }
}