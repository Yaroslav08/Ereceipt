﻿using Echack.Application.Interfaces;
using Echack.Application.ViewModels.Chack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Echack.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChacksController : BaseController
    {
        private readonly IChackService _chackService;
        public ChacksController(IChackService chackService)
        {
            _chackService = chackService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateChack([FromBody] ChackCreateViewModel model)
        {
            var chack = await _chackService.CreateCheck(model);
            return Ok(chack);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChackById(Guid id)
        {
            var chack = await _chackService.GetChack(id);
            return Ok(chack);
        }

    }
}