using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ThalysonProjetoWEB.Data;
using ThalysonProjetoWEB.Models;

namespace ThalysonProjetoWEB.Controllers
{
    [TypeFilter(typeof(SessionAuthFilter))]
    public class RequestController : Controller
    {
        private readonly IMongoCollection<Pessoa> _pessoa;

        public RequestController(MongoDbContext context)
        {
            _pessoa = context.Pessoa;
        }

        public IActionResult postRequest()
        {
            return View();
        }

        public IActionResult entrarConta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult postRequest(Pessoa pessoa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Console.WriteLine($"Pessoa Recebida: {pessoa.Nome}, {pessoa.Idade}");
                    _pessoa.InsertOne(pessoa);
                    //return RedirectToAction("getRequest"); // ir pra página de get pra ver as pessoas inseridas
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                ModelState.AddModelError("", "Erro ao inserir no banco de dados.");
            }
            return View(pessoa);

        }

        [HttpGet]
        public IActionResult getRequest()
        {
            var pessoas = _pessoa.Find(_ => true).ToList();
            return View(pessoas);
        }
    }
}
