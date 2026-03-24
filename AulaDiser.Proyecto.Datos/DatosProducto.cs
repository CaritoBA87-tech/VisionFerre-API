using AulaDiser.Poryecto.Clases;
using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;

namespace AulaDiser.Proyecto.Datos
{
    public class DatosProducto
    {
        public string MensajeError { get; set; }
        public bool HayError { get; set; }
        private string cadenaConexion;

        //Constructor
        public DatosProducto(string conexion)
        {
            cadenaConexion = conexion;
            MensajeError = string.Empty;
            HayError = false;
        }

        //Agrega un nuevo producto
        public void Almacenar(Producto infoProducto)
        {
            //Declara la conexión
            SqlConnection connection = new SqlConnection(cadenaConexion);
            //Indicar el procedimiento almacenado
            SqlCommand command = new SqlCommand("pv.sp_Producto_Alta", connection);
            //Especificar los parámetros
            command.Parameters.AddWithValue("@descripcion", infoProducto.Descripcion);
            //Especificar el tipo de comando
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                //Abrir la conexión
                connection.Open();
                //Ejecutamos el comando
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                MensajeError = ex.Message;
                HayError = true;
            }
            finally
            {
                //El código que se ejecuta independientemente del resultado del bloque try
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }                   

                connection.Dispose();
            }

        }

        //Obtiene la lista de productos de acuerdo al estatus
        public List<Producto> Obtener(EstatusProducto estatus)
        {
            List<Producto> lst = new List<Producto>();
            SqlConnection connection = new SqlConnection(cadenaConexion);
            SqlCommand command = new SqlCommand("pv.sp_Producto_ObtenerPorEstatus", connection);
            command.Parameters.AddWithValue("@estatus", (int)estatus);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Producto producto = new Producto();
                    producto.IdProducto = Convert.ToInt32(dr["idProducto"]);
                    producto.Descripcion = dr["descripcion"].ToString();
                    producto.Estatus = Convert.ToInt32(dr["estatus"]);
                    lst.Add(producto);
                }

                HayError = false;
                connection.Close();

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                HayError = true;

                lst = null;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();

            }
            return lst;
        }


        public List<Producto> ObtenerTodos(string idsCategorias = null)
        {
            List<Producto> lst = new List<Producto>();
            SqlConnection connection = new SqlConnection(cadenaConexion);
            SqlCommand command = new SqlCommand("pv.sp_Productos_Obtener", connection);
            command.Parameters.AddWithValue("@idsCategorias", (object)idsCategorias ?? DBNull.Value);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Producto producto = new Producto();
                    producto.IdProducto = Convert.ToInt32(dr["idProducto"]);
                    producto.SKU = dr["sku"].ToString();
                    producto.Nombre = dr["nombre"].ToString();
                    producto.Descripcion = dr["descripcion"].ToString();
                    producto.Precio = Convert.ToDecimal(dr["precio"]);
                    producto.Categoria = dr["categoria"].ToString();
                    producto.Marca = dr["marca"].ToString();
                    producto.jsonImagenes = dr["imagenesJSON"] != DBNull.Value ? dr["imagenesJSON"].ToString() : "[]";

                    lst.Add(producto);
                }

                HayError = false;
                connection.Close();

            }
            catch (Exception ex)
            {
                // Esto imprimirá el error REAL en los logs de Railway (ej. "Login failed" o "Network error")
                Console.WriteLine($"DEBUG ERROR SQL: {ex.Message}");

                MensajeError = ex.Message;
                HayError = true;

                // CAMBIO VITAL: Devuelve una lista vacía en lugar de null
                lst = new List<Producto>();
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();

            }
            return lst;
        }

        /*public List<Producto> ObtenerTodos(string idsCategorias = null)
        {
            List<Producto> lst = new List<Producto>();

            using (NpgsqlConnection connection = new NpgsqlConnection(cadenaConexion))
            {
                using (NpgsqlCommand command = new NpgsqlCommand("pv.sp_productos_obtener", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("ids_categorias", (object)idsCategorias ?? DBNull.Value);

                    try
                    {
                        connection.Open();

                        using (NpgsqlDataReader dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Producto producto = new Producto();
                                producto.IdProducto = Convert.ToInt32(dr["id_producto"]);
                                producto.SKU = dr["sku"].ToString();
                                producto.Nombre = dr["nombre"].ToString();
                                producto.Descripcion = dr["descripcion"].ToString();
                                producto.Precio = Convert.ToDecimal(dr["precio"]);
                                producto.Categoria = dr["categoria"].ToString();
                                producto.Marca = dr["marca"].ToString();

                                producto.jsonImagenes = dr["imagenes_json"] != DBNull.Value
                                                        ? dr["imagenes_json"].ToString()
                                                        : "[]";

                                lst.Add(producto);
                            }
                        }
                        HayError = false;
                    }
                    catch (Exception ex)
                    {
                        MensajeError = ex.Message;
                        HayError = true;
                        lst = null;
                    }
                    // Ya no necesitas el bloque finally porque el 'using' cierra la conexión por ti
                }
            }
            return lst;
        }*/

        public List<AtributoProducto> ObtenerAtributos(List<Producto> productos)
        {
            var nombresAtributos = productos
            .SelectMany(p => p.Atributos) 
            .Select(a => a.Atributo)  // Selecciona el nombre (ej. "Calibre")
            .Distinct()                   
            .ToList();

            var seguimiento = nombresAtributos.Select(nombre => new AtributoProducto
            {
                Atributo = nombre.ToLower(), 
                Valor = (string)null,
                Pendiente = true
            }).ToList();

            return seguimiento;
        }

        public List<Menu> ObtenerMenu()
        {
            List<Menu> lst = new List<Menu>();
            SqlConnection connection = new SqlConnection(cadenaConexion);
            SqlCommand command = new SqlCommand("pv.sp_Obtener_Menu", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Menu item = new Menu();
                    item.ID = Convert.ToInt32(dr["ID"]);
                    item.Categoria = dr["categoria1"].ToString();
                    item.Subcategoria = dr["categoria2"].ToString();
                    lst.Add(item);
                }

                HayError = false;
                connection.Close();

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                HayError = true;

                lst = null;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();

            }
            return lst;
        }

        /*public List<Menu> ObtenerMenu()
        {
            List<Menu> lst = new List<Menu>();

            using (NpgsqlConnection connection = new NpgsqlConnection(cadenaConexion))
            {
                using (NpgsqlCommand command = new NpgsqlCommand("pv.sp_obtener_menu", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        connection.Open();

                        using (NpgsqlDataReader dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Menu item = new Menu();

                                item.ID = Convert.ToInt32(dr["id"]);

                                item.Categoria = dr["categoria1"].ToString();
                                item.Subcategoria = dr["categoria2"].ToString();

                                lst.Add(item);
                            }
                        }

                        HayError = false;
                    }
                    catch (Exception ex)
                    {
                        MensajeError = ex.Message;
                        HayError = true;
                        lst = null;
                    }
                }
            }
            return lst;
        }*/

        public Producto ObtenerPorID(int productoId)
        {
            Producto producto = new Producto();
            SqlConnection connection = new SqlConnection(cadenaConexion);
            SqlCommand command = new SqlCommand("pv.sp_Producto_ObtenerPorID", connection);
            command.Parameters.AddWithValue("@id", (int)productoId);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    producto.IdProducto = Convert.ToInt32(dr["idProducto"]);
                    producto.SKU = dr["sku"].ToString();
                    producto.Nombre = dr["nombre"].ToString();
                    producto.Descripcion = dr["descripcion"].ToString();
                    producto.Precio = Convert.ToDecimal(dr["precio"]);
                    producto.Categoria = dr["categoria"].ToString();
                    producto.Marca = dr["marca"].ToString();
                    producto.jsonImagenes = dr["imagenesJSON"] != DBNull.Value ? dr["imagenesJSON"].ToString() : "[]";
                }

                HayError = false;
                connection.Close();

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                HayError = true;

                producto = null;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();

            }
            return producto;
        }

        /*public Producto ObtenerPorID(int productoId)
        {
            Producto producto = new Producto();

            using (NpgsqlConnection connection = new NpgsqlConnection(cadenaConexion))
            {
                using (NpgsqlCommand command = new NpgsqlCommand("pv.sp_producto_obtenerporid", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("id", productoId);

                    try
                    {
                        connection.Open();
                        using (NpgsqlDataReader dr = command.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                producto.IdProducto = Convert.ToInt32(dr["id_producto"]);
                                producto.SKU = dr["sku"].ToString();
                                producto.Nombre = dr["nombre"].ToString();
                                producto.Descripcion = dr["descripcion"].ToString();
                                producto.Precio = Convert.ToDecimal(dr["precio"]);
                                producto.Categoria = dr["categoria"].ToString();
                                producto.Marca = dr["marca"].ToString();

                                // Manejo de nulos para el JSON
                                producto.jsonImagenes = dr["imagenes_json"] != DBNull.Value
                                                        ? dr["imagenes_json"].ToString()
                                                        : "[]";
                            }
                            else
                            {
                                producto = null;
                            }
                        }
                        HayError = false;
                    }
                    catch (Exception ex)
                    {
                        MensajeError = ex.Message;
                        HayError = true;
                        producto = null;
                    }
                }
            }
            return producto;
        }*/

        //Obtener el producto con el id especificado
        public Producto ObtenerUno(int idProducto)
        {
            List<Producto> lst = new List<Producto>();
            lst = Obtener(EstatusProducto.Todos);
            Producto encontrado = lst.Find(x => x.IdProducto == idProducto);
            return encontrado;
        }

        //Modifica un producto
        public void Actualizar(Producto infoProducto)
        {
            //Declara la conexión
            SqlConnection connection = new SqlConnection(cadenaConexion);
            //Indicar el procedimiento almacenado
            SqlCommand command = new SqlCommand("pv.sp_Producto_Actualiza", connection);
            //Especificar los parámetros
            command.Parameters.AddWithValue("@id", infoProducto.IdProducto);
            command.Parameters.AddWithValue("@descripcion", infoProducto.Descripcion);
            //Especificar el tipo de comando
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                //Abrir la conexión
                connection.Open();
                //Ejecutamos el comando
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                MensajeError = ex.Message;
                HayError = true;
            }
            finally
            {
                //El código que se ejecuta independientemente del resultado del bloque try
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();
            }

        }

        //Elimina un producto
        public void Eliminar(Producto infoProducto)
        {
            //Declara la conexión
            SqlConnection connection = new SqlConnection(cadenaConexion);
            //Indicar el procedimiento almacenado
            SqlCommand command = new SqlCommand("pv.sp_Producto_Elimina", connection);
            //Especificar los parámetros
            command.Parameters.AddWithValue("@id", infoProducto.IdProducto);
            //Especificar el tipo de comando
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                //Abrir la conexión
                connection.Open();
                //Ejecutamos el comando
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                MensajeError = ex.Message;
                HayError = true;
            }
            finally
            {
                //El código que se ejecuta independientemente del resultado del bloque try
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();
            }

        }

        /*public List<Producto> BuscarProductosPorEtiquetas(List<string> etiquetas)
        {
            List<Producto> productosEncontrados = new List<Producto>();

            using (SqlConnection connection = new SqlConnection(cadenaConexion))
            {
                // 1. Construimos el WHERE dinámicamente: Tags LIKE '%Tag1%' OR Tags LIKE '%Tag2%'...
                List<string> condiciones = new List<string>();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                for (int i = 0; i < etiquetas.Count; i++)
                {
                    string paramName = "@tag" + i;
                    condiciones.Add($"Tags LIKE {paramName}");
                    command.Parameters.AddWithValue(paramName, "%" + etiquetas[i] + "%");
                }

                string sql = "SELECT * FROM pv.producto";
                if (condiciones.Count > 0)
                {
                    sql += " WHERE " + string.Join(" OR ", condiciones);
                }

                command.CommandText = sql;

                // 2. Ejecutamos y mapeamos manualmente
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productosEncontrados.Add(new Producto
                        {
                            IdProducto = reader.GetInt32(0),
                            Descripcion = reader.GetString(1),
                            Estatus = reader.GetInt32(2)
                        });
                    }
                }
            }
            return productosEncontrados;
        }*/

        public List<Producto> BuscarProductosPorEtiquetas(List<string> etiquetas)
        {
            List<Producto> productosEncontrados = new List<Producto>();

            using (SqlConnection connection = new SqlConnection(cadenaConexion))
            {
                List<string> condiciones = new List<string>();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                string sql = @"
                    SELECT A.IdProducto, A.SKU, A.nombre, A.descripcion, A.precio, CONCAT(B.categoria1, ' / ', B.categoria2, ' / ', B.categoria3) AS categoria, C.marca, B.Clave, 		(
                       SELECT TOP 1 I.imagen 
                       FROM dbo.Producto_Imagen I 
                       WHERE I.productoID = A.idProducto 
                       FOR JSON PATH
                   ) AS imagenesJSON,
                   (
                    SELECT ATR.Nombre AS Atributo, PA.Valor
                    FROM dbo.Producto_Atributo PA
                    INNER JOIN dbo.Atributos ATR ON PA.AtributoID = ATR.ID
                    WHERE PA.ProductoID = A.IdProducto
                    FOR JSON PATH
                ) AS atributosJSON
                        FROM [VisionFerreDB].[pv].[producto] A
                    INNER JOIN [VisionFerreDB].[dbo].[Categorias_CAT] B ON B.ID = A.categoriaID
                    INNER JOIN[dbo].[Marca_CAT] C
                    ON A.marcaID = C.ID";

                for (int i = 0; i < etiquetas.Count; i++)
                {
                    string paramName = "@tag" + i;
                    condiciones.Add($"B.Clave LIKE {paramName}");
                    command.Parameters.AddWithValue(paramName, "%" + etiquetas[i] + "%");
                }

                if (condiciones.Count > 0)
                {
                    sql += " WHERE " + string.Join(" OR ", condiciones);
                }

                command.CommandText = sql;

                connection.Open();
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        productosEncontrados.Add(new Producto
                        {
                            IdProducto = Convert.ToInt32(dr["idProducto"]),
                            SKU = dr["sku"].ToString(),
                            Nombre = dr["nombre"].ToString(),
                            Descripcion = dr["descripcion"].ToString(),
                            Precio = Convert.ToDecimal(dr["precio"]),
                            Categoria = dr["categoria"].ToString(),
                            Marca = dr["marca"].ToString(),
                            Clave = dr["clave"].ToString(),
                            jsonImagenes = dr["imagenesJSON"] != DBNull.Value ? dr["imagenesJSON"].ToString() : "[]",
                            AtributosJSON = dr["atributosJSON"] != DBNull.Value ? dr["atributosJSON"].ToString() : "[]"
                        });
                    }
                }
            }

            return productosEncontrados;
        }

        /*public List<Producto> BuscarProductosPorEtiquetas(List<string> etiquetas)
        {
            List<Producto> productosEncontrados = new List<Producto>();

            using (NpgsqlConnection connection = new NpgsqlConnection(cadenaConexion))
            {
                List<string> condiciones = new List<string>();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                // Adaptamos el SQL a la sintaxis de PostgreSQL
                // 1. Usamos COALESCE y json_build_object en lugar de FOR JSON PATH
                // 2. Usamos LIMIT 1 en lugar de TOP 1
                // 3. Cambiamos nombres a snake_case (id_producto, imagenes_json, etc)
                string sql = @"
            SELECT 
                A.id_producto, 
                A.sku, 
                A.nombre, 
                A.descripcion, 
                A.precio, 
                CONCAT(B.categoria1, ' / ', B.categoria2, ' / ', B.categoria3) AS categoria, 
                C.marca, 
                B.clave,
                (
                    SELECT json_build_object('imagen', I.imagen)::text
                    FROM pv.producto_imagen I 
                    WHERE I.producto_id = A.id_producto 
                    LIMIT 1
                ) AS imagenes_json,
                (
                    SELECT json_agg(json_build_object('Atributo', ATR.nombre, 'Valor', PA.valor))::text
                    FROM pv.producto_atributo PA
                    INNER JOIN pv.atributos ATR ON PA.atributo_id = ATR.id
                    WHERE PA.producto_id = A.id_producto
                ) AS atributos_json
            FROM pv.producto A
            INNER JOIN pv.categorias_cat B ON B.id = A.categoria_id
            INNER JOIN pv.marca_cat C ON A.marca_id = C.id";

                // Construcción de parámetros dinámicos
                for (int i = 0; i < etiquetas.Count; i++)
                {
                    // En Postgres usamos : o @, Npgsql soporta ambos, pero el nombre del parámetro va sin prefijo en AddWithValue
                    string paramName = "tag" + i;
                    condiciones.Add($"B.clave LIKE @{paramName}");
                    command.Parameters.AddWithValue(paramName, "%" + etiquetas[i] + "%");
                }

                if (condiciones.Count > 0)
                {
                    sql += " WHERE " + string.Join(" OR ", condiciones);
                }

                command.CommandText = sql;

                try
                {
                    connection.Open();
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            productosEncontrados.Add(new Producto
                            {
                                IdProducto = Convert.ToInt32(dr["id_producto"]),
                                SKU = dr["sku"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                                Descripcion = dr["descripcion"].ToString(),
                                Precio = Convert.ToDecimal(dr["precio"]),
                                Categoria = dr["categoria"].ToString(),
                                Marca = dr["marca"].ToString(),
                                Clave = dr["clave"].ToString(),
                                jsonImagenes = dr["imagenes_json"] != DBNull.Value ? dr["imagenes_json"].ToString() : "[]",
                                AtributosJSON = dr["atributos_json"] != DBNull.Value ? dr["atributos_json"].ToString() : "[]"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Aquí puedes manejar el error según tu lógica (MensajeError = ex.Message;)
                    throw;
                }
            }

            return productosEncontrados;
        }*/
    }
}
