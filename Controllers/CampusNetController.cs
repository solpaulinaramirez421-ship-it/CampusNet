using CampusNet.Models;
using CampusNet.Views;

namespace CampusNet.Controllers;

public class CampusNetController
{
    private readonly Graph _grafo = new();
    private readonly ConsolaView _vista = new();

    private static readonly (string Id, string Nombre, Rol RolUsuario)[] UsuariosIniciales =
    {
        ("U01", "Ana Torres", Rol.Estudiante),
        ("U02", "Bruno Ríos", Rol.Estudiante),
        ("U03", "Carla Mejía", Rol.Profesor),
        ("U04", "Diego Salazar", Rol.Egresado),
        ("U05", "Elena Vargas", Rol.Estudiante),
        ("U06", "Felipe Cano", Rol.Profesor),
        ("U07", "Gloria Pineda", Rol.Egresado),
        ("U08", "Hugo Marín", Rol.Estudiante),
        ("U09", "Irene Duque", Rol.Estudiante),
        ("U10", "Javier Osorio", Rol.Profesor),
        ("U11", "Karen Londoño", Rol.Egresado),
        ("U12", "Luis Arango", Rol.Estudiante)
    };

    // Cada par significa: Origen sigue a Destino.
    private static readonly (string Origen, string Destino)[] RelacionesIniciales =
    {
        ("U01", "U02"), ("U01", "U03"), ("U01", "U07"), ("U01", "U08"),
        ("U02", "U03"), ("U02", "U05"),
        ("U03", "U01"), ("U03", "U04"), ("U03", "U05"), ("U03", "U06"),
        ("U04", "U06"), ("U04", "U09"),
        ("U05", "U06"), ("U05", "U10"),
        ("U06", "U07"),
        ("U07", "U10"),
        ("U08", "U03"), ("U08", "U09"),
        ("U09", "U10"),
        ("U10", "U04"),
        ("U11", "U03"), ("U11", "U06"),
        ("U12", "U01"), ("U12", "U05")
    };

    private static readonly (string Origen, string Destino)[] PruebasAlcanzabilidad =
    {
        ("U12", "U10"),
        ("U11", "U09"),
        ("U10", "U01"),
        ("U01", "U12")
    };

    public void Ejecutar()
    {
        ConstruirGrafo();
        RealizarRecorridos();
        RealizarConsultas();
        RealizarOperacionesCrud();
        MostrarMetricasFinales();
    }

    private void ConstruirGrafo()
    {
        _vista.MostrarEncabezado("CASO 1: CONSTRUCCIÓN DEL GRAFO");
        _vista.MostrarMensaje("Grafo dirigido vacío creado.");

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("Insertando usuarios:");
        foreach (var (id, nombre, rol) in UsuariosIniciales)
            AgregarUsuario(id, nombre, rol);

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("Insertando relaciones de seguimiento:");
        foreach (var (origen, destino) in RelacionesIniciales)
            AgregarRelacion(origen, destino);

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("Prueba de control de duplicados y datos inválidos:");
        AgregarUsuario("u01", "Otra Persona", Rol.Profesor);
        AgregarRelacion("U01", "U02");
        AgregarRelacion("U01", "U01");
        AgregarRelacion("U01", "U99");

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("Estructura del grafo (lista de adyacencia):");
        _vista.MostrarGrafo(_grafo);
    }

    private void RealizarRecorridos()
    {
        _vista.MostrarEncabezado("CASO 2: RECORRIDOS");

        _vista.MostrarMensaje("BFS desde tres usuarios distintos:");
        foreach (string origen in new[] { "U01", "U10", "U12" })
            _vista.MostrarBfs(_grafo, origen, _grafo.Bfs(origen));

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("DFS completo:");
        _vista.MostrarDfs(_grafo.DfsCompleto());
    }

    private void RealizarConsultas()
    {
        _vista.MostrarEncabezado("CASO 3: CONSULTAS SOCIALES");

        _vista.MostrarUsuarios(
            "Usuarios sin seguidores (grado de entrada 0)",
            _grafo.UsuariosSinSeguidores());

        _vista.MostrarUsuarios(
            "Usuarios influyentes (mayor grado de entrada)",
            _grafo.UsuariosInfluyentes(),
            usuario => $"seguidores: {_grafo.GradoEntrada(usuario.Id)}");

        _vista.MostrarUsuarios(
            "Usuarios más activos (mayor grado de salida)",
            _grafo.UsuariosMasActivos(),
            usuario => $"sigue a: {_grafo.GradoSalida(usuario.Id)}");

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("Alcanzabilidad (BFS):");
        foreach (var (origen, destino) in PruebasAlcanzabilidad)
            _vista.MostrarAlcanzabilidad(origen, destino, _grafo.Alcanzable(origen, destino));
    }

    private void RealizarOperacionesCrud()
    {
        _vista.MostrarEncabezado("CASO 4: OPERACIONES CRUD");

        _vista.MostrarMensaje("1) Agregar un usuario:");
        AgregarUsuario("U13", "Marta Soto", Rol.Egresado);
        _vista.MostrarGrafo(_grafo);

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("2) Agregar relaciones:");
        AgregarRelacion("U13", "U01");
        AgregarRelacion("U13", "U04");
        _vista.MostrarGrafo(_grafo);

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("3) Eliminar una relación (la segunda vez ya no existe):");
        EliminarRelacion("U13", "U04");
        EliminarRelacion("U13", "U04");
        _vista.MostrarGrafo(_grafo);

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("4) Actualizar nombre y rol (el último intento es inválido):");
        ActualizarUsuario("U02", "Bruno Ríos Mejía", null);
        ActualizarUsuario("U08", null, Rol.Egresado);
        ActualizarUsuario("U02", "", null);
        _vista.MostrarGrafo(_grafo);

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("5) Eliminar un usuario junto con sus relaciones:");
        EliminarUsuario("U09");
        EliminarUsuario("U99");
        _vista.MostrarGrafo(_grafo);
    }

    private void MostrarMetricasFinales()
    {
        _vista.MostrarEncabezado("CASO 5: MÉTRICAS Y ANÁLISIS FINAL");
        _vista.MostrarMetricas(_grafo);

        _vista.MostrarMensaje("");
        _vista.MostrarUsuarios(
            "Usuarios sin seguidores en la estructura final",
            _grafo.UsuariosSinSeguidores());

        _vista.MostrarUsuarios(
            "Usuarios influyentes en la estructura final",
            _grafo.UsuariosInfluyentes(),
            usuario => $"seguidores: {_grafo.GradoEntrada(usuario.Id)}");

        _vista.MostrarUsuarios(
            "Usuarios más activos en la estructura final",
            _grafo.UsuariosMasActivos(),
            usuario => $"sigue a: {_grafo.GradoSalida(usuario.Id)}");

        _vista.MostrarMensaje("");
        _vista.MostrarMensaje("DFS completo sobre la estructura final:");
        _vista.MostrarDfs(_grafo.DfsCompleto());
    }

    private void AgregarUsuario(string id, string nombre, Rol rol)
    {
        ResultadoOperacion resultado = _grafo.AgregarVertice(id, nombre, rol);
        _vista.MostrarResultado($"Agregar usuario {id} ({nombre}, {rol})", resultado);
    }

    private void EliminarUsuario(string id)
    {
        ResultadoOperacion resultado = _grafo.EliminarVertice(id);
        _vista.MostrarResultado($"Eliminar usuario {id}", resultado);

        if (resultado == ResultadoOperacion.Exito)
            _vista.MostrarMensaje($"  Relaciones eliminadas junto con el usuario: {_grafo.UltimasAristasEliminadas}");
    }

    private void ActualizarUsuario(string id, string? nombre, Rol? rol)
    {
        ResultadoOperacion resultado = _grafo.ActualizarVertice(id, nombre, rol);

        var cambios = new List<string>();
        if (nombre != null) cambios.Add($"nombre = '{nombre}'");
        if (rol.HasValue) cambios.Add($"rol = {rol.Value}");

        _vista.MostrarResultado($"Actualizar usuario {id}: {string.Join(", ", cambios)}", resultado);
    }

    private void AgregarRelacion(string origen, string destino)
    {
        ResultadoOperacion resultado = _grafo.AgregarArista(origen, destino);
        _vista.MostrarResultado($"Agregar relación {origen} -> {destino}", resultado);
    }

    private void EliminarRelacion(string origen, string destino)
    {
        ResultadoOperacion resultado = _grafo.EliminarArista(origen, destino);
        _vista.MostrarResultado($"Eliminar relación {origen} -> {destino}", resultado);
    }
}