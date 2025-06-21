using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs;

[ApiController]
[Route("api/creditcards")]
public class CreditCardInfoController : ControllerBase
{
    private readonly ICreditCardInfoService _creditCardInfoService;

    public CreditCardInfoController(ICreditCardInfoService service)
    {
        _creditCardInfoService = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddCreditCard([FromBody] CreditCardInfoDto dto)
    {
        await _creditCardInfoService.AddCreditCardAsync(dto);
        return Ok("Credit Card Added Successfully!");
    }

    [HttpGet("{cardId}")]
    public async Task<IActionResult> GetCreditCard(int cardId)
    {
        var creditCard = await _creditCardInfoService.GetCreditCardAsync(cardId);
        return creditCard == null ? NotFound("Credit Card not found.") : Ok(creditCard);
    }

    [HttpPut("{cardId}/expiration")]
    public async Task<IActionResult> UpdateExpirationDate(int cardId, [FromBody] string newExpirationDate)
    {
        await _creditCardInfoService.UpdateExpirationDateAsync(cardId, newExpirationDate);
        return Ok("Expiration date updated successfully!");
    }
}