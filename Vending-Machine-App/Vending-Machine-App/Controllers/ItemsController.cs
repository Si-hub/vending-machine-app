using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vending_Machine_App.Models;

namespace Vending_Machine_App.Controllers
{
    
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly VendingMachineDbContext _context;

        public ItemsController(VendingMachineDbContext context)
        {
            _context = context;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
          if (_context.Items == null)
          {
              return NotFound();
          }
            return await _context.Items.ToListAsync();
        }


    }
}
