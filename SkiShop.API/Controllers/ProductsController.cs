using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SkiShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IGenericRepository<Product> repo) : ControllerBase
    {
       
        [HttpGet]
        public async Task<ActionResult<IReadOnlySet<Product>>> GetProducts(string? brand,
            string? type,string? sort)
        {
           var spec = new ProductSpecification(brand,type,sort);
            var products = await repo.ListAsync(spec);
            return Ok(products);
        }
        [HttpGet ("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.Add(product);
            if (await repo.SaveAllAsync())
            {
                return CreatedAtAction("GetProduct",new {id=product.Id}, product);
            }
            return BadRequest("Problem creating product");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult>UpdateProduct(int id, Product product)
        {
            if (product.Id !=id || !ProductExists(id))
            
                return BadRequest("Cannot Update this Product");
            
            repo.Update(product);

           if(await repo.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem updateing the product");
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product =await repo.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            repo.Remove(product);
            if (await repo.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the product");

            
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlySet<string>>> GetBrands()
        {
           var spec = new BrandListSpecification();
            var brands = await repo.ListAsync(spec);
            return Ok(brands);
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlySet<string>>> GetTypes()
        {
           var spec = new TypeListSpecification();
            var types = await repo.ListAsync(spec);
            return Ok(types);
        }
        private bool ProductExists(int id)
        {
            return repo.Exists(id);
        }
    }
}
