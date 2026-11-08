using CampusNet.Models;

namespace CampusNet.Views;

// Solo presentación en consola: no debe contener lógica del grafo.
// Cada método tiene un comentario con lo que debe imprimir.
public class ConsolaView
{
    // Línea separadora con el título del caso de uso (como en el taller de árboles).
    public void MostrarEncabezado(string titulo) { }

    // Imprime un texto tal cual. Con "" imprime una línea en blanco.
    public void MostrarMensaje(string mensaje) { }

    // Lista de adyacencia: una línea por usuario, con su Id, nombre y rol,
    // y a quiénes sigue. Ej.: "u1 (Ana, Estudiante) -> u2, u5".
    // Debe incluir la cantidad de vértices y de aristas.
    // Datos: grafo.Vertices, grafo.Seguidos(id), grafo.CantidadVertices, grafo.CantidadAristas.
    public void MostrarGrafo(Graph grafo) { }

    // Resultado de una operación de CRUD. 'operacion' es una descripción ya armada,
    // ej.: "Agregar usuario u13". Mostrar un texto distinto para cada ResultadoOperacion:
    // Exito, Duplicado (rechazado), NoExiste e Invalido.
    public void MostrarResultado(string operacion, ResultadoOperacion resultado) { }

    // BFS: usuario de origen, orden de visita y cantidad de vértices alcanzados
    // (por ejemplo "5 de 12 vértices", contando el propio origen).
    public void MostrarBfs(Graph grafo, string origen, List<string> orden) { }

    // DFS completo: orden de descubrimiento y los ciclos encontrados,
    // cada uno con el formato "A -> B -> C -> A". Si no hay, un mensaje que lo indique.
    // Datos: resultado.OrdenDescubrimiento, resultado.Ciclos, resultado.TieneCiclos.
    public void MostrarDfs(ResultadoDfs resultado) { }

    // Lista de usuarios de una consulta (sin seguidores, influyentes, activos).
    // Si 'detalle' no es null, se llama con cada usuario y se imprime al lado
    // (por ejemplo "seguidores: 4"). Si la lista está vacía, indicarlo.
    public void MostrarUsuarios(string titulo, IEnumerable<Vertex> usuarios, Func<Vertex, string>? detalle = null) { }

    // Respuesta a "¿A puede llegar a B?": sí o no, con los dos Ids.
    public void MostrarAlcanzabilidad(string origen, string destino, bool alcanzable) { }

    // Tabla de métricas: por cada usuario, Id, nombre, rol, grado de entrada y grado de salida.
    // Datos: grafo.Vertices, grafo.GradoEntrada(id), grafo.GradoSalida(id).
    public void MostrarMetricas(Graph grafo) { }
}