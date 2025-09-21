// -----------------------------------------------------------------------
// <copyright file="TestController.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace HttpFilesMW.Core.IntegrationTests.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Hello World" });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id, Name = $"Item {id}" });
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateItemRequest request)
    {
        return Ok(new { Id = 1, request.Name });
    }
}

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
}