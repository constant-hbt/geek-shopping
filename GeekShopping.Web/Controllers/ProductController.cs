using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public async Task<ActionResult<IEnumerable<ProductModel>>> ProductIndex()
        {
            var products = await _productService.FindAllProducts();
            return View(products);
        }

        public ActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<ProductModel>> ProductCreate(ProductModel model)
        {
            if(ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(model);
                if(response != null)
                    return RedirectToAction(nameof(ProductIndex));
            }
            return View(model);
        }

        public async Task<IActionResult> ProductUpdate(int id)
        {
            var model = await _productService.FindProductById(id);
            if (model != null) return View(model);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(model);
                if (response != null) return RedirectToAction(nameof(ProductIndex));
            }
            return View(model);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ProductDelete(int id)
        {
            var product = await _productService.FindProductById(id);
            if (product != null) return View(product);
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ProductDelete(ProductModel model)
        {
            var response = await _productService.DeleteProduct(model.Id);
            if (response) return RedirectToAction(nameof(ProductIndex));
            return View(model);
        }
    }
}
