using COSUMIR_API_TEST.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;


namespace COSUMIR_API_TEST.Servicios
{
    public class Servicio_API : IServicio_API
    {
        private static string _usuario;
        private static string _clave;
        private static string _baseUrl;
        private static string _token;

        public Servicio_API() {

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            _usuario = builder.GetSection("ApiSetting:usuario").Value;
            _clave = builder.GetSection("ApiSetting:clave").Value;
            _baseUrl = builder.GetSection("ApiSetting:baseUrl").Value;
        }

        //USAR REFERENCIAS 
        public async Task Autenticar() {

            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseUrl);

            var credenciales = new Credencial() { correo = _usuario, clave = _clave };

            var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync("api/Autenticacion/Validar", content);
            var json_respuesta = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<ResultadoCredencial>(json_respuesta);
            _token = resultado.token;
        }

        public async Task<List<Producto>> Lista() { 
           

            await Autenticar();
            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseUrl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await cliente.GetAsync("api/Producto/Lista");

            Producto producto = new Producto();
            List<Producto> listaProducto = new List<Producto>();

            if (response.IsSuccessStatusCode) {

                var json_respuesta = await response.Content.ReadAsStringAsync();

                //var resultado = JsonConvert.DeserializeObject<ResultadoApi>(json_respuesta);

                JObject resultado = JObject.Parse(json_respuesta);
              

                if (resultado == null)
                {
                    throw new Exception("Fallo al deserializar!!");
                }

                foreach (var result in resultado["response"])
                {                    
                    listaProducto.Add(new Producto() { 
                        IdProducto = (int)result["idProducto"],
                        CodigoBarra = (string)result["codigoBarra"],
                        Nombre = (string)result["nombre"],
                        Marca = (string)result["marca"],
                        Categoria = (string)result["categoria"],
                        Precio = (decimal)result["precio"]
                });
                }
               
            }
            return listaProducto;
        }

        public async Task<Producto> Obtener(int idProducto)
        {
            Producto objeto = new Producto();

            await Autenticar();


            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseUrl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await cliente.GetAsync($"api/Producto/Obtener/{idProducto}");

            if (response.IsSuccessStatusCode)
            {

                var json_respuesta = await response.Content.ReadAsStringAsync();
               
                var resultado = JObject.Parse(json_respuesta);

                if (resultado == null)
                {
                    throw new Exception("Fallo al deserializar!!");
                }

                objeto.IdProducto = (int)resultado["response"]["idProducto"];
                objeto.CodigoBarra = (string)resultado["response"]["codigoBarra"];
                objeto.Nombre = (string)resultado["response"]["nombre"];
                objeto.Marca = (string)resultado["response"]["marca"];
                objeto.Categoria = (string)resultado["response"]["categoria"];
                objeto.Precio = (decimal)resultado["response"]["precio"];

            }

            return objeto;
        }

        public async Task<bool> Guardar(Producto objeto)
        {
            bool respuesta = false;

            await Autenticar();


            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseUrl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var content = new StringContent(JsonConvert.SerializeObject(objeto), Encoding.UTF8, "application/json");

            var response = await cliente.PostAsync("api/Producto/Guardar/", content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }

            return respuesta;
        }

        public async Task<bool> Editar(Producto objeto)
        {
            bool respuesta = false;

            await Autenticar();


            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseUrl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var content = new StringContent(JsonConvert.SerializeObject(objeto), Encoding.UTF8, "application/json");

            var response = await cliente.PutAsync("api/Producto/Editar/", content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }

            return respuesta;
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            bool respuesta = false;

            await Autenticar();


            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseUrl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);


            var response = await cliente.DeleteAsync($"api/Producto/Eliminar/{idProducto}");

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }

            return respuesta;
        }

    }
}
