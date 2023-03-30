using System.Security.Authentication;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace P1AppAndroid
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        ImageView Imagen;
        EditText? txtNombre, txtDomicilio, txtCorreo, txtEdad, txtSaldo, txtID, txtURL;
        Button? btnGuardar, btnConsultar;
        string? Nombre, Domicilio, Correo, URL;
        int Edad, ID;
        double Saldo;
        HttpClient cliente = new HttpClient();

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Imagen = FindViewById<ImageView>(Resource.Id.imagen);
            txtNombre = FindViewById<EditText>(Resource.Id.txtnombre);
            txtDomicilio = FindViewById<EditText>(Resource.Id.txtdomicilio);
            txtCorreo = FindViewById<EditText>(Resource.Id.txtcorreo);
            txtEdad = FindViewById<EditText>(Resource.Id.txtedad);
            txtSaldo = FindViewById<EditText>(Resource.Id.txtsaldo);
            txtID = FindViewById<EditText>(Resource.Id.txtid);
            txtURL = FindViewById<EditText>(Resource.Id.txturl);
            btnGuardar = FindViewById<Button>(Resource.Id.btnguardar);
            btnConsultar = FindViewById<Button>(Resource.Id.btnconsultar);

            btnGuardar.Click += delegate
            {
                #region GUARDAR EN XML
                try
                {
                    var DC = new Datos();

                    DC.Id = int.Parse(txtID.Text);
                    DC.Nombre = txtNombre.Text;
                    DC.Domicilio = txtDomicilio.Text;
                    DC.Correo = txtCorreo.Text;
                    DC.Edad = int.Parse(txtEdad.Text);
                    DC.Saldo = double.Parse(txtSaldo.Text);
                    DC.URL = txtURL.Text;
                    var serializador = new XmlSerializer(typeof(Datos));
                    var Escritura = new StreamWriter
                        (Path.Combine(System.Environment.GetFolderPath
                            (System.Environment.SpecialFolder.Personal),
                                txtID.Text + ".xml"));
                    serializador.Serialize(Escritura, DC);
                    Escritura.Close();
                    Toast.MakeText(this, "Archivo XML Guardado Correctamente",
                        ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "XML " + ex.Message,
                        ToastLength.Long).Show();
                }
                #endregion

                #region GUARDAR EN SQL SERVER
                try
                {
                    Nombre = txtNombre.Text;
                    Domicilio = txtDomicilio.Text;
                    Correo = txtCorreo.Text;
                    Edad = int.Parse(txtEdad.Text);
                    Saldo = double.Parse(txtSaldo.Text);
                    URL = txtURL.Text;
                    var API = String.Format("http://192.168.100.16:81/Principal/AlmacenarSQLServer?Nombre={0}" +
                        "&Domicilio={1}&Correo={2}&Edad={3}&Saldo={4}&URL={5}",
                        Nombre, Domicilio, Correo, Edad, Saldo, URL);
                    HttpResponseMessage response = cliente.GetAsync(API).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = response.Content.ReadAsStringAsync().Result;
                        Toast.MakeText(this, "SQL Server " + resultado.ToString(), ToastLength.Long).Show();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "SQL Server " + ex.Message,
                        ToastLength.Long).Show();
                }
                #endregion

                #region GUARDAR EN SQLITE
                try
                {
                    var csql = new ClaseSQLite();
                    csql.nombre = txtNombre.Text;
                    csql.domicilio = txtDomicilio.Text;
                    csql.correo = txtCorreo.Text;
                    csql.edad = int.Parse(txtEdad.Text);
                    csql.saldo = double.Parse(txtSaldo.Text);
                    csql.url = txtURL.Text;
                    csql.ConexionBase();
                    if ((csql.IngresarDatos(csql.nombre, csql.domicilio, csql.correo, csql.edad, csql.saldo, csql.url)) == true)
                    {
                        Toast.MakeText(this, "Guardado Correctamente en SQLite", ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "NO guardado en SQLite", ToastLength.Long).Show();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "SQLite " + ex.Message,
                        ToastLength.Long).Show();
                }
                #endregion
            };

            btnConsultar.Click += BtnConsultar_Click;
        }

        private async void BtnConsultar_Click(object sender, EventArgs e)
        {
            Imagen.SetImageURI(null);

            #region CONSULTAR EN XML
            try
            {
                var DC = new Datos();

                DC.Id = int.Parse(txtID.Text);
                var serializador = new XmlSerializer(typeof(Datos));
                var Lectura = new StreamReader
                    (Path.Combine(System.Environment.GetFolderPath
                        (System.Environment.SpecialFolder.Personal),
                            txtID.Text + ".xml"));
                var datos = (Datos)serializador.Deserialize(Lectura);
                Lectura.Close();
                txtNombre.Text = datos.Nombre;
                txtDomicilio.Text = datos.Domicilio;
                txtCorreo.Text = datos.Correo;
                txtEdad.Text = datos.Edad.ToString();
                txtSaldo.Text = datos.Saldo.ToString();
                txtURL.Text = datos.URL;

                var ruta = await DescargadeImagen(datos.URL);
                Android.Net.Uri RutaImagen = Android.Net.Uri.Parse(ruta);
                Imagen.SetImageURI(RutaImagen);

                Toast.MakeText(this, "Archivo XML Consultado",
                        ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "XML " + ex.Message,
                    ToastLength.Long).Show();
            }
            #endregion

            //LimpiarCampos();
            //Toast.MakeText(this, "Limpiando Campos",
            //            ToastLength.Long).Show();

            #region CONSULTAR EN SQL SERVER
            try
            {
                ID = int.Parse(txtID.Text);
                var API = String.Format("http://192.168.100.16:81/Principal/ConsultarSQLServer?ID={0}", ID);
                var json = await TraerDatos(API);
                foreach (var repo in json)
                {
                    txtNombre.Text = repo.Nombre;
                    txtDomicilio.Text = repo.Domicilio;
                    txtCorreo.Text = repo.Correo;
                    txtEdad.Text = repo.Edad.ToString();
                    txtSaldo.Text = repo.Saldo.ToString();
                    txtURL.Text = repo.URL;

                    var ruta = await DescargadeImagen(repo.URL);
                    Android.Net.Uri RutaImagen = Android.Net.Uri.Parse(ruta);
                    Imagen.SetImageURI(RutaImagen);

                    Toast.MakeText(this, "SQL Server Consultado",
                        ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "SQL Server " + ex.Message,
                    ToastLength.Long).Show();
            }
            #endregion

            //LimpiarCampos();
            //Toast.MakeText(this, "Limpiando Campos",
            //            ToastLength.Long).Show();

            #region CONSULTAR EN SQLITE
            try
            {
                var csql = new ClaseSQLite();
                csql.id = int.Parse(txtID.Text);
                csql.Buscar(csql.id);
                txtNombre.Text = csql.nombre;
                txtDomicilio.Text = csql.domicilio;
                txtCorreo.Text = csql.correo;
                txtEdad.Text = csql.edad.ToString();
                txtSaldo.Text = csql.saldo.ToString();
                txtURL.Text = csql.url.ToString();

                var ruta = await DescargadeImagen(csql.url);
                Android.Net.Uri RutaImagen = Android.Net.Uri.Parse(ruta);
                Imagen.SetImageURI(RutaImagen);

                Toast.MakeText(this, "SQLite Consultado",
                    ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "SQLite " + ex.Message,
                    ToastLength.Long).Show();
            }
            #endregion
        }

        public async Task<string> DescargadeImagen(string URL)
        {
            HttpClientHandler CabeceraCliente =
                new HttpClientHandler();
            CabeceraCliente.
                ServerCertificateCustomValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) =>
                { return true; };
            CabeceraCliente.SslProtocols = SslProtocols.Tls;
            HttpClient ClienteHTTP = new HttpClient
                (CabeceraCliente);
            var respuesta = ClienteHTTP.GetAsync
                (URL).Result;

            Byte[] datosImagen = respuesta.
                Content.ReadAsByteArrayAsync().Result;
            var carpeta = System.Environment.GetFolderPath
                (System.Environment.SpecialFolder.Personal);
            var archivo = "Imagen.jpg";
            var rutalocal = Path.Combine(carpeta, archivo);
            File.WriteAllBytes(rutalocal, datosImagen);
            return rutalocal;
        }

        public async Task<List<Datos>> TraerDatos(string API)
        {
            cliente.DefaultRequestHeaders.Accept.Clear();
            var streamTask = cliente.GetStreamAsync(API);
            var repositorio = await
                System.Text.Json.JsonSerializer.
                DeserializeAsync<List<Datos>>(await streamTask);
            return repositorio;
        }

        //public void LimpiarCampos()
        //{
        //    txtNombre.Text = String.Empty;
        //    txtDomicilio.Text = String.Empty;
        //    txtCorreo.Text = String.Empty;
        //    txtEdad.Text = String.Empty;
        //    txtSaldo.Text = String.Empty;
        //    txtURL.Text = String.Empty;
        //}
    }

    public class Datos
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("domicilio")]
        public string Domicilio { get; set; }
        [JsonPropertyName("correo")]
        public string Correo { get; set; }
        [JsonPropertyName("edad")]
        public int Edad { get; set; }
        [JsonPropertyName("saldo")]
        public double Saldo { get; set; }
        [JsonPropertyName("url")]
        public string URL { get; set; }
    }
}