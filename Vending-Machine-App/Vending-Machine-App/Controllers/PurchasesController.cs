using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using iTextSharp.text.pdf.parser.clipper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vending_Machine_App.Models;

namespace Vending_Machine_App.Controllers
{

    [Route("api/purchases")]
    [ApiController]
    
    public class PurchasesController : ControllerBase
    {
        private readonly VendingMachineDbContext _dbContext;

        public PurchasesController(VendingMachineDbContext context)
        {
            _dbContext = context;
         
        }

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
