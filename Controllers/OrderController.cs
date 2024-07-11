using DB;
using infomatica.dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace infomatica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private InfomaticaContext _context;

        public OrderController(InfomaticaContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.detailsOrder).ThenInclude(dv => dv.Product).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.detailsOrder).ThenInclude(dv => dv.Product).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(CreateOrderDto createOrderDto)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Crear la orden sin los detalles
                var order = new Order
                {
                    fecha = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();


                //Crea los detalles de la orden
                foreach (var detailDto in createOrderDto.DetailsOrder)
                {

                    var detailOrder = new DetailOrder
                    {
                        OrderId = order.Id,
                        ProductId = detailDto.ProductId,
                        quantity = detailDto.Quantity
                    };

                    _context.DetailsOrder.Add(detailOrder);

                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

    }
}
