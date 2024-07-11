using DB;
using infomatica.dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace infomatica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {

        private InfomaticaContext _context;

        public SubCategoryController(InfomaticaContext context)
        {
            _context = context;
        }


        // GET: api/SubCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategories()
        {
            return await _context.SubCategories.Include( sc => sc.Category ).ToListAsync();
        }

        // GET: api/SubCategory/2
        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategory>> GetSubCategory(int id)
        {
            var Subcategory = await _context.SubCategories.FindAsync(id);

            if (Subcategory == null)
            {
                return NotFound();
            }

            return Subcategory;
        }

        // POST: api/SubCategory
        [HttpPost]
        public async Task<ActionResult<SubCategory>> PostSubCategory(CreateSubCategoryDto subcategoryDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var cat = await _context.Categories.FindAsync(subcategoryDto.CategoryId);

            var subcat = new SubCategory
            {
                Name = subcategoryDto.Name,
                CategoryId = subcategoryDto.CategoryId,
            };

            _context.SubCategories.Add(subcat);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetSubCategory), new { id = subcat.Id }, subcat);
        }

        // PUT: api/SubCategory/2
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubCategory(int id, SubCategory subcategory)
        {
            if (id != subcategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(subcategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubCategoryExists(id))
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

        // DELETE: api/SubCategory/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var subcategory = await _context.SubCategories.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }

            _context.SubCategories.Remove(subcategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubCategoryExists(int id)
        {
            return _context.SubCategories.Any(e => e.Id == id);
        }

    }
}
