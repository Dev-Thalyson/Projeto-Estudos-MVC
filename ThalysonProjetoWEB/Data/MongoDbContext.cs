using MongoDB.Driver;
using ThalysonProjetoWEB.Models;
using MongoDB.Bson;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext()
    {
        string nome = "nome_do_usuario";
        string senha = "senha_do_usuario";

        string connectionUri = $"mongodb+srv://{nome}:{senha}@cluster0.wesgg.mongodb.net/?retryWrites=true&w=majority";
        // o nome e a senha são referentes ao seu tipo de usuário dentro do banco mongo, não da sua conta real.

        var settings = MongoClientSettings.FromConnectionString(connectionUri);

        settings.ServerApi = new ServerApi(ServerApiVersion.V1); //versionamento pelo kestrel

        var client = new MongoClient(settings);

        // precisa colocar isso aqui VVV pra conectar no DB do mongo
        _database = client.GetDatabase("ProjetoWEB"); // Database chamado ProjetoWEB
    }

    public IMongoDatabase Database => _database;

    // p/ acessar a collection do mongo, oq vale é oq tá em parênteses, o resto é apenas indicações/nome da entity
    public IMongoCollection<Pessoa> Pessoa => _database.GetCollection<Pessoa>("pessoas_teste"); // collection pessoas_teste que está dentro do database
    public IMongoCollection<Cadastro> Cadastro => _database.GetCollection<Cadastro>("cadastro");
    public IMongoCollection<Produto> Produto => _database.GetCollection<Produto>("produto");
}

