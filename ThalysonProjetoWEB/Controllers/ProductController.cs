using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ThalysonProjetoWEB.Data;
using ThalysonProjetoWEB.Models;

namespace ThalysonProjetoWEB.Controllers
{
    [TypeFilter(typeof(SessionAuthFilter))]
    public class ProductController : Controller
    {
        private readonly IMongoCollection<Produto> _produto;
        private readonly IMongoDatabase _database;

        public ProductController(MongoDbContext context)
        {
            _produto = context.Produto;
            _database = context.Database;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult criarProduto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> criarProduto(Produto produto, IFormFile imagem)
        {
            if (imagem != null && imagem.Length > 0)
            {
                var bucket = new GridFSBucket(_database);
                var imagemId = await bucket.UploadFromStreamAsync(imagem.FileName, imagem.OpenReadStream());

                produto.ImageId = imagemId;
            }

            await _produto.InsertOneAsync(produto);
            ViewData["ProdutoCriar"] = "Produto '" + produto.Name + "' criado com sucesso!";

            return View();
            //return RedirectToAction("Index");
        }

        public async Task<IActionResult> Imagem(string id)
        {
            var produto = await _produto.Find(p => p.Id == id).FirstOrDefaultAsync();

            if (produto == null || produto.ImageId == ObjectId.Empty)
            {
                return NotFound();
            }

            // Recupera a imagem do GridFS
            var bucket = new GridFSBucket(_database);
            var imagemStream = await bucket.OpenDownloadStreamAsync(produto.ImageId);

            return File(imagemStream, "image/jpeg");
        }

        public async Task<IActionResult> verProduto()
        {
            var produtos = await _produto.Find(_ => true).ToListAsync();
            return View(produtos);
        }
    }
}
