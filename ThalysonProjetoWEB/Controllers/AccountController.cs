using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ThalysonProjetoWEB.Data;
using ThalysonProjetoWEB.Models;

namespace ThalysonProjetoWEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMongoCollection<Cadastro> _cadastro;

        public AccountController(MongoDbContext context)
        {
            _cadastro = context.Cadastro;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult criarConta()
        {
            return View();
        }

        public IActionResult entrarConta()
        {
            return View();
        }

        public IActionResult Password()
        {
            ViewData["nomeUsuario"] = HttpContext.Session.GetString("nomeUsuario");
            ViewData["senhaUsuario"] = HttpContext.Session.GetString("senhaUsuario");

            return View();
        }

        [TypeFilter(typeof(SessionAuthFilter))]
        public IActionResult Profile()
        {
            ViewData["nomeUsuario"] = HttpContext.Session.GetString("nomeUsuario");
            ViewData["senhaUsuario"] = HttpContext.Session.GetString("senhaFake");

            return View();
        }

        [HttpPost]
        public IActionResult Desconectar()
        {
            HttpContext.Session.SetString("LoggedIn", "");
            return RedirectToAction("entrarConta", "Account");
        }

        public IActionResult trocarSenha()
        {
            return RedirectToAction("Password", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> passwordChange(Cadastro cadastro)
        {
            try
            {
                var nome_usuario = HttpContext.Session.GetString("nomeUsuario");
                var senha_antiga = HttpContext.Session.GetString("senhaUsuario");

                var user = await _cadastro.Find(x => x.Username == nome_usuario).FirstOrDefaultAsync();
                if (user == null)
                {
                    ViewData["MensagemErro"] = "Usuário não encontrado.";
                    return View("Password");
                }

                if (string.IsNullOrEmpty(cadastro.Password) || cadastro.Password.Length < 5)
                {
                    ViewData["MensagemErro"] = "A senha deve ter pelo menos 5 caracteres.";
                    return View("Password");
                }

                if (cadastro.Password != senha_antiga)
                {
                    ViewData["MensagemErro"] = "Senha atual incorreta.";
                    return View("Password");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(cadastro.Password);

                var filtro = Builders<Cadastro>.Filter.Eq(x => x.Username, nome_usuario);
                var atualizar = Builders<Cadastro>.Update.Set(x => x.Password, hashedPassword);

                var result = await _cadastro.UpdateOneAsync(filtro, atualizar);

                if (result.ModifiedCount > 0)
                {
                    ViewData["MensagemSucesso"] = "Senha alterada com sucesso!";
                }
                else
                {
                    ViewData["MensagemErro"] = "Não foi possível alterar a senha.";
                    return View("Password");
                }

                return View("Password");
            }
            catch (Exception ex)
            {
                ViewData["MensagemErro"] = "Ocorreu um erro ao alterar a senha. Tente novamente mais tarde.";
                return View("Password");
            }
        }

        [HttpPost]
        public async Task<IActionResult> entrarConta(Cadastro cadastro)
        {
            try
            {
                var usuario = await _cadastro.Find(x => x.Username == cadastro.Username).FirstOrDefaultAsync();

                if (usuario != null)
                {
                    bool senhaValida = BCrypt.Net.BCrypt.Verify(cadastro.Password, usuario.Password);

                    if (senhaValida)
                    {
                        HttpContext.Session.SetString("nomeUsuario", usuario.Username);
                        HttpContext.Session.SetString("senhaUsuario", cadastro.Password);
                        HttpContext.Session.SetString("senhaFake", "*******");
                        HttpContext.Session.SetString("LoggedIn", "true");

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewData["MensagemErro"] = "Usuário ou senha inválidos. Tente novamente.";
                        return View();
                    }
                }
                else
                {
                    ViewData["MensagemErro"] = "Usuário ou senha inválidos. Tente novamente.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante o login: {ex.Message}");
                ViewData["MensagemErro"] = "Ocorreu um erro ao tentar fazer login. Tente novamente mais tarde.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> criarConta(Cadastro cadastro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuarioExistente = await _cadastro.Find(x => x.Username == cadastro.Username).FirstOrDefaultAsync();
                    if (usuarioExistente != null)
                    {
                        ViewData["MensagemErro"] = "O nome de usuário '" + cadastro.Username + "' já existe.";
                        return View();
                    }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(cadastro.Password);
                    cadastro.Password = hashedPassword;

                    await _cadastro.InsertOneAsync(cadastro);

                    Console.WriteLine($"Cadastro realizado: {cadastro.Username}, {hashedPassword}");
                    return RedirectToAction("entrarConta");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                ModelState.AddModelError("", "Erro ao inserir no banco de dados.");
            }
            return View(cadastro);
        }
    }
}