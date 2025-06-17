using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;
using System.Text;
using System.Threading.Tasks;

public class CreditCardInfoRepository : ICreditCardInfoRepository
{
    private readonly SQLServerDbContext _context;
    private readonly EncryptionHelper _encryptionHelper;

    public CreditCardInfoRepository(SQLServerDbContext context, EncryptionHelper encryptionHelper)
    {
        _context = context;
        _encryptionHelper = encryptionHelper;
    }

    public async Task AddCreditCardAsync(CreditCardInfo creditCard)
    {
        // Trim whitespace and extract last four digits before encryption
        creditCard.CardHolderName = creditCard.CardHolderName.Trim();
        

        // Encrypt sensitive data
        creditCard.CardNumber = Convert.FromBase64String(_encryptionHelper.Encrypt(creditCard.CardNumber.ToString()));
        creditCard.CVV = Convert.FromBase64String(_encryptionHelper.Encrypt(creditCard.CVV.ToString()));


        if (creditCard.CreatedAt == default)
            creditCard.CreatedAt = DateTime.UtcNow;

        _context.Set<CreditCardInfo>().Add(creditCard);
        await _context.SaveChangesAsync();
    }

    public async Task<CreditCardInfo?> GetCreditCardAsync(int cardId)
    {
        var card = await _context.Set<CreditCardInfo>().FindAsync(cardId);
        if (card != null)
        {
            // Convert byte[] to Base64 before decrypting
            var decryptedCardNumber = _encryptionHelper.Decrypt(Convert.ToBase64String(card.CardNumber));
            card.CardNumber = Encoding.UTF8.GetBytes(decryptedCardNumber);

            var decryptedCVV = _encryptionHelper.Decrypt(Convert.ToBase64String(card.CVV));
            card.CVV = Encoding.UTF8.GetBytes(decryptedCVV);
        }
        return card;
    }

    public async Task UpdateExpirationDateAsync(int cardId, string newExpirationDate)
    {
        var card = await _context.Set<CreditCardInfo>().FindAsync(cardId);
        if (card == null)
            throw new KeyNotFoundException("Credit card not found.");

        card.ExpirationDate = newExpirationDate;
        await _context.SaveChangesAsync();
    }
}