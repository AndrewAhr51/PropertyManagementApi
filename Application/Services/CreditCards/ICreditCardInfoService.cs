﻿using PropertyManagementAPI.Domain.DTOs.Payments;

namespace PropertyManagementAPI.Application.Services.CreditCards
{
    public interface ICreditCardInfoService
    {
        Task AddCreditCardAsync(CreditCardInfoDto dto);
        Task<CreditCardInfoDto?> GetCreditCardAsync(int cardId);
        Task UpdateExpirationDateAsync(int cardId, string newExpirationDate);  // New method
    }
}
