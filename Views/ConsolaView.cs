
using System.Text;
using CampusNet.Models;

namespace CampusNet.Views;

// Solo presentación en consola: no contiene lógica del grafo.
public class ConsolaView
{
    public ConsolaView()
    {
        // Para que la consola muestre bien las tildes y la ñ.
        Console.OutputEncoding = Encoding.UTF8;
    }

    public void MostrarEncabezado(string titulo)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine(titulo);
        Console.WriteLine(new string('=', 60));
    }

    public void MostrarMensaje(string mensaje)
    {
        Console.WriteLine(mensaje);
    }

    // Lista de adyacencia: una línea por usuario con a quiénes sigue.
    public void MostrarGrafo(Graph grafo)
    {
        Console.WriteLine($"Grafo dirigido: {grafo.CantidadVertices} usuarios, {grafo.CantidadAristas} relaciones");

        foreach (Vertex usuario in grafo.Vertices)
        {
            IReadOnlyList<string> seguidos = grafo.Seguidos(usuario.Id);
            string destino = seguidos.Count == 0 ? "(no sigue a nadie)" : string.Join(", ", seguidos);
            string izquierda = $"{usuario.Id} ({usuario.Nombre}, {usuario.Rol})";

            Console.WriteLine($"  {izquierda.PadRight(38)}-> {destino}");
        }
    }

    public void MostrarResultado(string operacion, ResultadoOperacion resultado)
    {
        string mensaje = resultado switch
        {
            ResultadoOperacion.Exito => "OK",
            ResultadoOperacion.Duplicado => "RECHAZADO: ya existe (duplicado)",
            ResultadoOperacion.NoExiste => "RECHAZADO: no existe (usuario o relación)",
            ResultadoOperacion.Invalido => "RECHAZADO: datos inválidos",
            _ => "Resultado desconocido"
        };

        Console.WriteLine($"{operacion}: {mensaje}");
    }

    public void MostrarBfs(Graph grafo, string origen, List<string> orden)
    {
        Console.WriteLine($"BFS desde {origen}:");

        if (orden.Count == 0)
        {
            Console.WriteLine("  El usuario no existe.");
            return;
        }

        Console.WriteLine($"  Orden de visita: {string.Join(" -> ", orden)}");
        Console.WriteLine($"  Vértices alcanzados: {orden.Count} de {grafo.CantidadVertices}");
    }

    public void MostrarDfs(ResultadoDfs resultado)
    {
        Console.WriteLine($"  Orden de descubrimiento: {string.Join(" -> ", resultado.OrdenDescubrimiento)}");
        Console.WriteLine($"  Vértices descubiertos: {resultado.OrdenDescubrimiento.Count}");

        if (!resultado.TieneCiclos)
        {
            Console.WriteLine("  Ciclos: no se encontraron ciclos dirigidos.");
            return;
        }

        Console.WriteLine($"  Ciclos dirigidos encontrados: {resultado.Ciclos.Count}");
        foreach (List<string> ciclo in resultado.Ciclos)
            Console.WriteLine($"    {string.Join(" -> ", ciclo)}");
    }

    public void MostrarUsuarios(string titulo, IEnumerable<Vertex> usuarios, Func<Vertex, string>? detalle = null)
    {
        Console.WriteLine();
        Console.WriteLine($"{titulo}:");

        List<Vertex> lista = usuarios.ToList();
        if (lista.Count == 0)
        {
            Console.WriteLine("  (ninguno)");
            return;
        }

        foreach (Vertex usuario in lista)
        {
            string extra = detalle != null ? $" - {detalle(usuario)}" : "";
            Console.WriteLine($"  {usuario.Id} {usuario.Nombre} ({usuario.Rol}){extra}");
        }
    }

    public void MostrarAlcanzabilidad(string origen, string destino, bool alcanzable)
    {
        string respuesta = alcanzable ? "SÍ" : "NO";
        Console.WriteLine($"¿{origen} puede llegar a {destino}? {respuesta}");
    }

    // Tabla de grados por usuario.
    public void MostrarMetricas(Graph grafo)
    {
        Console.WriteLine("Métricas por usuario (entrada = seguidores, salida = a cuántos sigue):");
        Console.WriteLine(string.Format("{0,-6}{1,-20}{2,-12}{3,8}{4,8}", "Id", "Nombre", "Rol", "Entrada", "Salida"));
        Console.WriteLine(new string('-', 54));

        foreach (Vertex usuario in grafo.Vertices)
        {
            Console.WriteLine(string.Format(
                "{0,-6}{1,-20}{2,-12}{3,8}{4,8}",
                usuario.Id,
                usuario.Nombre,
                usuario.Rol,
                grafo.GradoEntrada(usuario.Id),
                grafo.GradoSalida(usuario.Id)));
        }

        Console.WriteLine(new string('-', 54));
        Console.WriteLine($"Total: {grafo.CantidadVertices} usuarios y {grafo.CantidadAristas} relaciones");
    }
}
