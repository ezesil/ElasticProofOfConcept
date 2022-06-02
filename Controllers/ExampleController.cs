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
        public async Task<IReadOnlyCollection<Documento>> GetById([FromQuery] string id, string index = "documentos")
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
        public async Task<IReadOnlyCollection<Documento>> GetByName([FromQuery] string name, string index = "documentos")
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(index);
            var client = new ElasticClient(connparams);

            var response = await client.SearchAsync<Documento>(s => s
            .Query(q => q
                 .Match(m => m
                    .Field(f => f.Name)
                    .Query(name))));

            return response.Documents;
        }

        [HttpPost]
        [Route("InsertDocument")]
        public async Task<object> InsertDocument([FromQuery] Documento document, string index = "documentos")
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(index);
            var client = new ElasticClient(connparams);

            var response = await client.CreateDocumentAsync(document);
            return new { response.Result, response.IsValid, response.Type, response.Version, response.Id, response.Index };
        }

        [HttpPost]
        [Route("CreateIndex")]
        public async Task<object> CreateIndex([FromQuery] string indexname)
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200"));
            var client = new ElasticClient(connparams);

            var response = await client.Indices.CreateAsync(indexname,
                    index => index.Map(x => x.AutoMap())
                    );

            return new { response.Acknowledged, response.IsValid, response.Index };
        }

        [HttpDelete]
        [Route("DeleteIndex")]
        public async Task<object> DeleteIndex([FromQuery] string indexname)
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200"));
            var client = new ElasticClient(connparams);

            var response = await client.Indices.DeleteAsync(indexname);

            return new { response.Acknowledged, response.IsValid };
        }

        [HttpPost]
        [Route("CheckIfIndexExists")]
        public async Task<object> GetIndex([FromQuery] string indexname)
        {
            var connparams = new ConnectionSettings(new Uri("http://localhost:9200"));
            var client = new ElasticClient(connparams);

            var response = await client.Indices.GetAsync(indexname);

            return new { response.IsValid };
        }

        [HttpPut]
        [Route("UpdateDocumentById")]
        public async Task<object> UpdateDocumentById([FromQuery] Documento document, string index = "documentos")
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