using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SkiShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repo) : ControllerBase
    {
        //private readonly StoreContext context;
        //public ProductsController(StoreContext context)
        //{
        //    this.context = context;
        //}
        [HttpGet]
        public async Task<ActionResult<IReadOnlySet<Product>>> GetProducts(string? brand,
            string? type,string? sort)
        {
           
            return Ok(await repo.GetProductsAsync(brand ,type,sort));
        }
        [HttpGet ("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.AddProduct(product);
            if (await repo.SaveChangesAsync())
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
            
            repo.UpdateProduct(product);

           if(await repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem updateing the product");
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product =await repo.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            repo.DeleteProduct(product);
            if (await repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the product");

            
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlySet<string>>> GetBrands()
        {
            return Ok( await repo.GetBrandsAsync());
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlySet<string>>> GetTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }
        private bool ProductExists(int id)
        {
            return repo.ProductExists(id);
        }
    }
}
