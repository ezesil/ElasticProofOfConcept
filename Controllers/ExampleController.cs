using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ProofOfConceptAnses.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private readonly ILogger<ExampleController> _logger;

        public ExampleController(ILogger<ExampleController> logger)
        {
            _logger = logger;
        }

        [Route("GetById")]
        [HttpGet]
        public async Task<IReadOnlyCollection<Documento>> GetById(string id, string index = "documentos")
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(index);
            var client = new ElasticClient(connparams);

            var response = await client.SearchAsync<Documento>(s => s
            .Query(q => q
                 .Match(m => m
                    .Field(f => f.Id)
                    .Query(id))));

            return response.Documents;
        }

        [Route("GetByName")]
        [HttpGet]
        public async Task<IReadOnlyCollection<Documento>> GetByName(string id, string index = "documentos")
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(index);
            var client = new ElasticClient(connparams);

            var response = await client.SearchAsync<Documento>(s => s
            .Query(q => q
                 .Match(m => m
                    .Field(f => f.Name)
                    .Query(id))));

            return response.Documents;
        }

        [HttpPost]
        [Route("InsertDocument")]
        public async Task<object> InsertDocument([FromBody] Documento document, string index = "documentos")
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(index);
            var client = new ElasticClient(connparams);

            var response = await client.CreateDocumentAsync(document);
            return new { response.Result, response.IsValid, response.Type, response.Version, response.Id, response.Index };
        }

        [HttpPut]
        [Route("UpdateDocumentById")]
        public async Task<object> UpdateDocumentById([FromBody] Documento document, string index = "documentos")
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(index);
            var client = new ElasticClient(connparams);

            var response = await client.UpdateAsync<Documento>(document.Id, u => u.Doc(new Documento { Name = document.Name }));

            return new { response.Result, response.IsValid, response.Type, response.Version };
        }

        [HttpDelete]
        [Route("DeleteById")]
        public async Task<object> DeleteById ([FromQuery] string Id, string index = "documentos")
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(index);
            var client = new ElasticClient(connparams);

            var response = await client.DeleteAsync<Documento>(Id);

            return new { response.Result, response.IsValid, response.Type, response.Version };
        }
    }
}