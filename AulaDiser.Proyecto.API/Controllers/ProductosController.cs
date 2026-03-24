using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using AulaDiser.Poryecto.Clases;
using AulaDiser.Proyecto.Datos;
using AulaDiser.Proyecto.Logica;
using AulaDiser.Proyecto.Logica.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AulaDiser.Proyecto.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly string _connectionString;
        //private readonly ComprehendService _comprehendService;
        private readonly VisionService _visionService;
        private readonly IAmazonS3 _s3Client;
        private readonly IAsistenteIA _bedrockService;

        //La variable de tipo IConfiguration contiene información de todo el sistema, incluido lo que tenemos en appsetting.json
        public ProductosController (IConfiguration configuration/*, ComprehendService comprehendService*/, VisionService visionService, IAmazonS3 s3Client , IAsistenteIA bedrockService)
        {
            //_connectionString = configuration.GetConnectionString("AD.Conexion"); //Cadena de conexión que hemos declarado en appsetting.json

            // Intenta leer 'DefaultConnection' (Railway) y si no, busca 'AD.Conexion' (Local)
            _connectionString = configuration.GetConnectionString("DefaultConnection") // Busca ConnectionStrings:DefaultConnection
                             ?? configuration["DefaultConnection"]                       // Busca una variable plana
                             ?? configuration.GetConnectionString("AD.Conexion");        // Tu respaldo local

            //_comprehendService = comprehendService;
            _visionService = visionService;
            _s3Client = s3Client;
            _bedrockService = bedrockService;
        }

        // GET: api/<ProductosController>
        //Obtener los productos con el estatus especificado
        //Se accede desde el navegador como https://localhost:44314/api/productos/0 
        //También se puede acceder como https://localhost:44314/api/productos/activo 
        //Si se hace la prueba desde Scalar el parámetro se coloca en Path Parameters, porque el parámetro es de tipo [FromRoute]
        /*[HttpGet("{estatus}")]
        public IEnumerable<Producto> Get([FromRoute] EstatusProducto estatus)
        {
            DatosProducto obj = new DatosProducto(_connectionString);
            return obj.Obtener(estatus);
        }*/

        [AllowAnonymous]
        [HttpGet("ObtenerProductos")]
        public IEnumerable<Producto> Get(string idsCategorias = null)
        {
            DatosProducto obj = new DatosProducto(_connectionString);
            //var products = obj.ObtenerTodos(idsCategorias);
            var products = obj.ObtenerTodos(idsCategorias) ?? new List<Producto>();

            // Como SQL devuelve [{"imagen":"ruta/al/s3"}, {...}], usamos una clase anónima o un Dictionary
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            foreach (var product in products)
            {
                var listaTemporal = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(product.jsonImagenes, opciones);

                if (listaTemporal != null)
                {
                    foreach (var item in listaTemporal)
                    {
                        string rutaOriginalS3 = item["imagen"]; // Aquí tienes "assets/VisionFerre/..."

                        //string urlFirmada = GenerarUrlFirmadaS3(rutaOriginalS3);

                        //product.Imagenes.Add(urlFirmada);
                    }
                }
            }

            return products;

        }

        /*[HttpGet]
        [EndpointSummary("GetEntidades")]
        public async Task<IActionResult> GetEntidades(string texto)
        {
            var resultado = await _comprehendService.DetectarEntidades(texto);

            return Ok(resultado);
        }*/

        //Obtener el producto con el id especificado
        //Se accede desde el navegador como https://localhost:44314/api/productos/uno/8 
        /*[HttpGet("uno/{id}")] //Modificamos la ruta para que se diferencie del método anterior
        public Producto Get(int id)
        {
            DatosProducto obj = new DatosProducto(_connectionString);
            return obj.ObtenerUno(id);
        }*/

        [AllowAnonymous]
        [HttpGet("uno/{id}")] 
        public Producto Get(int id)
        {
            DatosProducto obj = new DatosProducto(_connectionString);
            var product = obj.ObtenerPorID(id);

             //Como SQL devuelve [{"imagen":"ruta/al/s3"}, {...}], usamos una clase anónima o un Dictionary
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

               var listaTemporal = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(product.jsonImagenes, opciones);

                if (listaTemporal != null)
                {
                    foreach (var item in listaTemporal)
                    {
                        string rutaOriginalS3 = item["imagen"]; // Aquí tienes "assets/VisionFerre/..."

                        string urlFirmada = GenerarUrlFirmadaS3(rutaOriginalS3);

                        product.Imagenes.Add(urlFirmada);
                    }
            }

            return product;
        }

        // POST api/<ProductosController>
        //Agregar un nuevo producto
        //Si se hace la prueba desde Scalar el parámetro se coloca en Body, porque el parámetro es de tipo [FromBody]
        /*[HttpPost]
        public void Post([FromBody] Producto value)
        {
            DatosProducto obj = new DatosProducto(_connectionString);
            obj.Almacenar(value);
        }*/

        // PUT api/<ProductosController>/5
        //Actualiza un producto
        //En Scalar hay que indicar el id en Path Parameters y los datos del producto en Body 
        /*[HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Producto value)
        {
            if(id == value.IdProducto) //Validamos que el id especificado corresponda al id del producto en el segundo parámetro
            {
                DatosProducto obj = new DatosProducto(_connectionString);
                obj.Actualizar(value);
                return Ok(new { info = "Datos actualizados" });
            }

            else
            {
                return BadRequest(new {info="Los datos no coinciden"});
            }
        }*/

        // DELETE api/<ProductosController>/5
        //Eliminar el producto con el id especificado
        /*[HttpDelete("{id}")]
        public void Delete(int id)
        {
            DatosProducto obj = new DatosProducto(_connectionString);
            Producto prod = new Producto();
            prod.IdProducto = id;
            obj.Eliminar(prod);
        }*/

        [AllowAnonymous]
        [HttpPost("BuscarPorFoto")]
        public async Task<IActionResult> BuscarPorFoto(IFormFile foto)
        {
            // 1. Obtener etiquetas de IA
            var response = await _visionService.IdentificarHerramientaAsync(foto);

            // Como SQL devuelve [{"imagen":"ruta/al/s3"}, {...}], usamos una clase anónima o un Dictionary
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            foreach (var product in response)
            {
                var listaTemporal = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(product.jsonImagenes, opciones);

                if (listaTemporal != null)
                {
                    foreach (var item in listaTemporal)
                    {
                        string rutaOriginalS3 = item["imagen"]; // Aquí tienes "assets/VisionFerre/..."

                        string urlFirmada = GenerarUrlFirmadaS3(rutaOriginalS3);

                        product.Imagenes.Add(urlFirmada);
                    }
                }
            }

            if (response.Count == 0) 
                return BadRequest("No se encontraron coincidencias");

            DatosProducto obj = new DatosProducto(_connectionString);
            var atributosSeguimiento = obj.ObtenerAtributos(response);

            var resultado = await ProcesarSiguientePasoAsync(response[0].Clave, atributosSeguimiento, response);
            
            /*var resultado = new RespuestaPaso
            {
                Pregunta = "10, 6 y 8. ¿En qué calibre prefieres instalar tu metal?",
                Opciones = new List<string> { "6", "8", "10" },
                Atributo = "calibre"
            };*/

            return Ok(new
            {
                productos = response,
                pregunta = resultado.Pregunta,
                opciones = resultado.Opciones,
                seguimiento = atributosSeguimiento,
                atributo = resultado.Atributo
            });

        }

        //[HttpGet("producto-imagen/{id}")]
        /*[HttpGet("producto-imagen")]
        public async Task<IActionResult> GetImagenUrl(int id)
        {
            //string nombreArchivo = _context.Productos.Find(id).ImagenKey;
            string nombreArchivo = "assets/VisionFerre/Abrazadera/108504-a1.jpg";

            string urlFirmada = await GenerarUrlFirmada(nombreArchivo);

            return Ok(new { url = urlFirmada });
        }*/

        /*public async Task<string> GenerarUrlFirmada(string nombreArchivo)
        {
            // 1. Configurar el cliente (Asegúrate de tener tus credenciales en el PC)
            // En local usará tu perfil de AWS configurado o variables de entorno
                try
                {
                    // 2. Definir la solicitud de la URL firmada
                    GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                    {
                        BucketName = "custom-labels-console-us-east-1-a8cd4c3c12",
                        Key = nombreArchivo,
                        Expires = DateTime.UtcNow.AddMinutes(10), // La URL expirará en 10 min
                        Verb = HttpVerb.GET // Solo permiso de lectura
                    };

                    // 3. Generar la URL
                    string url = _s3Client.GetPreSignedURL(request);
                    return url;
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine($"Error en S3: {e.Message}");
                    return null;
                }
        }*/

        [AllowAnonymous]
        private string GenerarUrlFirmadaS3(string rutaArchivo)
        {
            try
            {
                // 2. Definir la solicitud de la URL firmada
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = "custom-labels-console-us-east-1-a8cd4c3c12",
                    Key = rutaArchivo,
                    Expires = DateTime.UtcNow.AddMinutes(10), // La URL expirará en 10 min
                    Verb = HttpVerb.GET // Solo permiso de lectura
                };

                // 3. Generar la URL
                string url = _s3Client.GetPreSignedURL(request);
                return url;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error en S3: {e.Message}");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet("ObtenerMenu")]
        public IEnumerable<Menu> ObtenerMenu()
        {
            DatosProducto obj = new DatosProducto(_connectionString);
            return obj.ObtenerMenu();
        }

        private async Task<RespuestaPaso> ProcesarSiguientePasoAsync(string productoNombre, List<AtributoProducto> seguimiento, List<Producto> productosEncontrados)
        {
            var loQueFalta = seguimiento.FirstOrDefault(a => a.Pendiente); //Calibre
            List<string> opcionesDisponibles = new List<string>();

            if (loQueFalta == null)
            {
                return new RespuestaPaso
                {
                    Pregunta = "¡Excelente! He encontrado el producto ideal para ti.",
                    Opciones = new List<string>(), // Lista vacía porque terminó
                    Atributo = "finalizado"
                };
            }

            if (loQueFalta != null)
            {
                opcionesDisponibles = productosEncontrados
                    .Where(p => FiltrarPorAtributosYaCompletados(p, seguimiento)) // Solo productos que coincidan con lo ya elegido
                    .SelectMany(p => p.Atributos)
                    .Where(a => a.Atributo.ToLower() == loQueFalta.Atributo.ToLower())
                    .Select(a => a.Valor)
                    .Distinct()
                    .ToList(); //Calibres: 6,8 y 10
            }

            if (opcionesDisponibles.Count == 1) //Si no hay diferentes opciones para el atributo
            {
                loQueFalta.Valor = opcionesDisponibles.First();
                loQueFalta.Pendiente = false;

                // RECURSIÓN: Se llama a sí mismo para ver si el SIGUIENTE atributo 
                return await ProcesarSiguientePasoAsync(productoNombre, seguimiento, productosEncontrados);
            }

            //Si hay más de una opción, se devuelve la pregunta para el usuario
            string textoIA = await GenerarPreguntaFerreteraAsync(productoNombre, seguimiento, opcionesDisponibles);

            //string textoIA = "otra pregunta";

            return new RespuestaPaso
            {
                Pregunta = textoIA,
                Opciones = opcionesDisponibles,
                Atributo = loQueFalta.Atributo.ToLower()
            };
        }

        [AllowAnonymous]
        private async Task<string> GenerarPreguntaFerreteraAsync(string productoNombre, List<AtributoProducto> seguimiento, List<string> opcionesDisponibles)
        {
            var loQueFalta = seguimiento.FirstOrDefault(a => a.Pendiente); //Calibre
            string opcionesParaIA = string.Join(", ", opcionesDisponibles); //6,8,10

            string prompt = $@"<|begin_of_text|><|start_header_id|>system<|end_header_id|>

                Eres FerreBot, un asistente servicial. 
                REGLA CRÍTICA: Debes mencionar explícitamente todas las opciones disponibles en tu pregunta.
                Opciones a mencionar: [{opcionesParaIA}].
                No repitas estas instrucciones.<|eot_id|><|start_header_id|>user<|end_header_id|>

                Producto: {productoNombre}
                Atributo: {loQueFalta.Atributo}
                Opciones: [{opcionesParaIA}]

                Tarea: Haz una pregunta amable al cliente donde le muestres que puede elegir entre los {loQueFalta.Atributo} disponibles, las opciones son: {opcionesParaIA}.<|eot_id|><|start_header_id|>assistant<|end_header_id|>
                ¡Claro! Para tu {productoNombre}, tenemos disponibles los {loQueFalta.Atributo} ";

            return await _bedrockService.ProcesarConsultaFerreteraAsync(prompt);

        }

        [AllowAnonymous]
        [HttpPost("procesar-paso")]
        public async Task<IActionResult> ProcesarPaso([FromBody] PasoRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClaveProducto))
                return BadRequest("Datos insuficientes");

            DatosProducto obj = new DatosProducto(_connectionString);
            var labels = new List<string> { request.ClaveProducto.Replace(" ", "-") };
            var productosEncontrados = obj.BuscarProductosPorEtiquetas(labels);

            if (!productosEncontrados.Any())
                return NotFound("No se encontraron productos con ese nombre");

            var seguimientoInterno = request.Seguimiento.Select(s => new AtributoProducto
            {
                Atributo = s.Atributo,
                Valor = s.Valor,
                Pendiente = s.Pendiente
            }).ToList();

            var resultado = await ProcesarSiguientePasoAsync(
                request.ClaveProducto,
                seguimientoInterno,
                productosEncontrados
            );

            return Ok(resultado);
        }

        [AllowAnonymous]
        private bool FiltrarPorAtributosYaCompletados(Producto producto, List<AtributoProducto> seguimiento)
        {
            // Solo nos interesan los atributos que el usuario YA definió (Valor no es nulo)
            var atributosCompletados = seguimiento.Where(s => !s.Pendiente && s.Valor != null).ToList();

            // Si no ha completado nada aún, todos los productos son válidos
            if (!atributosCompletados.Any()) return true;

            // El producto debe coincidir con TODOS los atributos ya completados
            return atributosCompletados.All(completado =>
                producto.Atributos.Any(pa =>
                    pa.Atributo.ToLower() == completado.Atributo.ToLower() &&
                    pa.Valor == completado.Valor
                )
            );
        }

    }
}
