using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vending_Machine_App.Models;

namespace Vending_Machine_App.Controllers
{
    /// <summary>
    /// Controller class for managing purchases in the vending machine app.
    /// </summary>
    [Route("api/purchases")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly VendingMachineDbContext _dbContext;

        public PurchasesController(VendingMachineDbContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Retrieves the purchase history.
        /// </summary>
        /// <returns>A list of purchase records.</returns>
        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchaseHistory()
        {
            if (_dbContext.Purchases == null)
            {
                return NotFound();
            }
            return await _dbContext.Purchases.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific purchase record by its ID.
        /// </summary>
        /// <param name="id">The ID of the purchase record.</param>
        /// <returns>The purchase record with the specified ID.</returns>
        // GET: api/PurchaseItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchaseHistoryById(int id)
        {
            if (_dbContext.Purchases == null)
            {
                return NotFound("Item not found");
            }
            var purchaseItem = await _dbContext.Purchases.FindAsync(id);

            if (purchaseItem == null)
            {
                return NotFound();
            }

            return purchaseItem;
        }

        /// <summary>
        /// Makes a purchase of an item in the vending machine.
        /// </summary>
        /// <param name="itemId">The ID of the item to purchase.</param>
        /// <param name="amountPaid">The amount paid for the item.</param>
        /// <returns>The result of the purchase operation.</returns>
        [HttpPost()]
        public async Task<IActionResult> MakePurchase(int itemId, decimal amountPaid)
        {
            // Get the item from the database.
            var item = await _dbContext.Items.FindAsync(itemId);
            if (item == null)
                return NotFound("Item not found");

            if (amountPaid < item.ItemPrice)
                return BadRequest("Insufficient payment");

            var change = amountPaid - item.ItemPrice;

            var purchase = new Purchase
            {
                ItemId = itemId,
                ItemName = item.ItemName!,
                PurchaseDate = DateTime.Now,
                AmountPaid = amountPaid,
                Change = change
            };

            // Add the purchase record to the database.
            _dbContext.Purchases.Add(purchase);
            // Save the changes to the database.
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Item purchased successfully.",
                Change = change
            });
        }

        /// <summary>
        /// Deletes a purchase record by its ID.
        /// </summary>
        /// <param name="id">The ID of the purchase record to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        // DELETE: api/PurchaseItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchases(int id)
        {
            if (_dbContext.Purchases == null)
            {
                return NotFound();
            }
            var purchase = await _dbContext.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            _dbContext.Purchases.Remove(purchase);
            await _dbContext.SaveChangesAsync();

            return Ok(await _dbContext.Purchases.ToListAsync());
        }
    }
}
