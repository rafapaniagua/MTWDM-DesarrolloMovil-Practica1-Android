using Microsoft.Data.Sqlite;

namespace P1AppAndroid
{
    public class ClaseSQLite
    {
        string basededatos = "InformacionP1.db3";
        public int id, edad;
        public string nombre, domicilio, correo, url;
        public double saldo;
        public void ConexionBase()
        {
            var RutaBase = Path.Combine(System.Environment.GetFolderPath
                        (System.Environment.SpecialFolder.Personal), basededatos);
            bool existencia = File.Exists(RutaBase);
            if (!existencia)
            {
                //SqliteConnection.CreateFile(RutaBase);
                var conexion = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=" + RutaBase);
                var sql = "CREATE TABLE Datos (ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                          "Nombre VARCHAR(50), Domicilio VARCHAR(50), Correo VARCHAR(50), " +
                          "Edad INTEGER, Saldo NUMERIC(18,2), URL VARCHAR(500));";
                conexion.Open();
                using (var query = conexion.CreateCommand())
                {
                    query.CommandText = sql;
                    query.ExecuteNonQuery();
                }
                conexion.Close();
            }
        }
        public bool IngresarDatos(string Nombre, string Domicilio, string Correo, int Edad, double Saldo, string URL)
        {
            var RutaBase = Path.Combine(System.Environment.GetFolderPath
                (System.Environment.SpecialFolder.Personal), basededatos);
            var conexion = new SqliteConnection("Data Source=" + RutaBase);
            try
            {
                conexion.Open();
                var sql = "INSERT INTO Datos (Nombre, Domicilio, Correo, Edad, Saldo, URL)" +
                    "VALUES (@Nombre, @Domicilio, @Correo, @Edad, @Saldo, @URL);";
                using (var query = conexion.CreateCommand())
                {
                    query.CommandText = sql;
                    query.Parameters.AddWithValue("@Nombre", Nombre);
                    query.Parameters.AddWithValue("@Domicilio", Domicilio);
                    query.Parameters.AddWithValue("@Correo", Correo);
                    query.Parameters.AddWithValue("@Edad", Edad);
                    query.Parameters.AddWithValue("@Saldo", Saldo);
                    query.Parameters.AddWithValue("@URL", URL);
                    query.ExecuteNonQuery();
                }
                conexion.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                conexion.Close();
                return false;
            }
        }
        public void Buscar(int id)
        {
            var RutaBase = Path.Combine(System.Environment.GetFolderPath
                (System.Environment.SpecialFolder.Personal), basededatos);
            var conexion = new SqliteConnection("Data Source=" + RutaBase);
            try
            {
                conexion.Open();
                using (var contenido = conexion.CreateCommand())
                {
                    contenido.CommandText = "SELECT * FROM Datos WHERE ID='" + id + "';";
                    var lectura = contenido.ExecuteReader();
                    while (lectura.Read())
                    {
                        id = int.Parse(lectura[0].ToString());
                        nombre = lectura[1].ToString();
                        domicilio = lectura[2].ToString();
                        correo = lectura[3].ToString();
                        edad = int.Parse(lectura[4].ToString());
                        saldo = double.Parse(lectura[5].ToString());
                        url = lectura[6].ToString();
                    }
                }
                conexion.Close();
            }
            catch (System.Exception ex)
            {
                conexion.Close();
            }
        }
    }
}
