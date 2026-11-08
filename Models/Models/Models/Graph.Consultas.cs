namespace CampusNet.Models;

public partial class Graph
{
    // Cantidad de usuarios que siguen a este (aristas de entrada). 0 si no existe.
    public int GradoEntrada(string id)
    {
        Vertex? vertice = ObtenerVertice(id);
        if (vertice == null)
            return 0;

        return _adyacencia.Values.Count(
            seguidos => seguidos.Contains(vertice.Id, StringComparer.OrdinalIgnoreCase));
    }

    // Cantidad de usuarios que este sigue (aristas de salida). 0 si no existe.
    public int GradoSalida(string id)
    {
        Vertex? vertice = ObtenerVertice(id);
        if (vertice == null)
            return 0;

        return _adyacencia[vertice.Id].Count;
    }

    // Usuarios con grado de entrada 0.
    public List<Vertex> UsuariosSinSeguidores()
    {
        return _vertices.Values.Where(vertice => GradoEntrada(vertice.Id) == 0).ToList();
    }

    // Usuarios con el grado de entrada más alto (puede haber empates).
    public List<Vertex> UsuariosInfluyentes()
    {
        return ConMayorGrado(GradoEntrada);
    }

    // Usuarios con el grado de salida más alto (puede haber empates).
    public List<Vertex> UsuariosMasActivos()
    {
        return ConMayorGrado(GradoSalida);
    }

    // ¿Puede el origen llegar al destino siguiendo relaciones dirigidas? Usa BFS.
    public bool Alcanzable(string origen, string destino)
    {
        Vertex? vDestino = ObtenerVertice(destino);
        if (vDestino == null)
            return false;

        return Bfs(origen).Contains(vDestino.Id, StringComparer.OrdinalIgnoreCase);
    }

    private List<Vertex> ConMayorGrado(Func<string, int> grado)
    {
        if (_vertices.Count == 0)
            return new List<Vertex>();

        int maximo = _vertices.Values.Max(vertice => grado(vertice.Id));
        return _vertices.Values.Where(vertice => grado(vertice.Id) == maximo).ToList();
    }
}