namespace CampusNet.Models;

// Resultado de una operación del grafo; la View lo traduce a un mensaje.
public enum ResultadoOperacion
{
    Exito,
    Duplicado,
    NoExiste,
    Invalido
}

public partial class Graph
{
    // Lista de adyacencia: Id del usuario -> Ids de los usuarios que sigue (aristas de salida).
    private readonly Dictionary<string, List<string>> _adyacencia = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Vertex> _vertices = new(StringComparer.OrdinalIgnoreCase);

    // Aristas que se eliminaron junto con el último vértice borrado (para mostrarlo en la View).
    public int UltimasAristasEliminadas { get; private set; }

    public int CantidadVertices => _vertices.Count;

    public int CantidadAristas => _adyacencia.Values.Sum(seguidos => seguidos.Count);

    public IEnumerable<Vertex> Vertices => _vertices.Values;

    public IEnumerable<Edge> Aristas =>
        _adyacencia.SelectMany(par => par.Value.Select(destino => new Edge(par.Key, destino)));

    public bool ExisteVertice(string id) => ObtenerVertice(id) != null;

    public Vertex? ObtenerVertice(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return _vertices.TryGetValue(id.Trim(), out Vertex? vertice) ? vertice : null;
    }

    // Ids de los usuarios que sigue el usuario indicado. Lista vacía si no existe.
    public IReadOnlyList<string> Seguidos(string id)
    {
        Vertex? vertice = ObtenerVertice(id);
        if (vertice == null)
            return Array.Empty<string>();

        return _adyacencia[vertice.Id];
    }

    // Duplicado: el Id ya existe. Inválido: Id o nombre vacíos.
    public ResultadoOperacion AgregarVertice(string id, string nombre, Rol rol)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre))
            return ResultadoOperacion.Invalido;

        var nuevo = new Vertex(id, nombre, rol);
        if (_vertices.ContainsKey(nuevo.Id))
            return ResultadoOperacion.Duplicado;

        _vertices[nuevo.Id] = nuevo;
        _adyacencia[nuevo.Id] = new List<string>();
        return ResultadoOperacion.Exito;
    }

    // También elimina todas las aristas que entran o salen del usuario.
    public ResultadoOperacion EliminarVertice(string id)
    {
        UltimasAristasEliminadas = 0;

        Vertex? vertice = ObtenerVertice(id);
        if (vertice == null)
            return ResultadoOperacion.NoExiste;

        UltimasAristasEliminadas = _adyacencia[vertice.Id].Count;

        _vertices.Remove(vertice.Id);
        _adyacencia.Remove(vertice.Id);

        foreach (List<string> seguidos in _adyacencia.Values)
        {
            UltimasAristasEliminadas += seguidos.RemoveAll(
                otro => otro.Equals(vertice.Id, StringComparison.OrdinalIgnoreCase));
        }

        return ResultadoOperacion.Exito;
    }

    // Un valor null significa "no cambiar ese dato".
    public ResultadoOperacion ActualizarVertice(string id, string? nuevoNombre, Rol? nuevoRol)
    {
        Vertex? vertice = ObtenerVertice(id);
        if (vertice == null)
            return ResultadoOperacion.NoExiste;

        if (nuevoNombre != null && string.IsNullOrWhiteSpace(nuevoNombre))
            return ResultadoOperacion.Invalido;

        if (nuevoNombre != null)
            vertice.Nombre = nuevoNombre.Trim();

        if (nuevoRol.HasValue)
            vertice.Rol = nuevoRol.Value;

        return ResultadoOperacion.Exito;
    }

    // NoExiste: falta algún usuario. Inválido: nadie se sigue a sí mismo. Duplicado: la arista ya existe.
    public ResultadoOperacion AgregarArista(string origen, string destino)
    {
        Vertex? vOrigen = ObtenerVertice(origen);
        Vertex? vDestino = ObtenerVertice(destino);

        if (vOrigen == null || vDestino == null)
            return ResultadoOperacion.NoExiste;

        if (vOrigen.Id.Equals(vDestino.Id, StringComparison.OrdinalIgnoreCase))
            return ResultadoOperacion.Invalido;

        List<string> seguidos = _adyacencia[vOrigen.Id];
        if (seguidos.Contains(vDestino.Id, StringComparer.OrdinalIgnoreCase))
            return ResultadoOperacion.Duplicado;

        seguidos.Add(vDestino.Id);
        return ResultadoOperacion.Exito;
    }

    public ResultadoOperacion EliminarArista(string origen, string destino)
    {
        Vertex? vOrigen = ObtenerVertice(origen);
        Vertex? vDestino = ObtenerVertice(destino);

        if (vOrigen == null || vDestino == null)
            return ResultadoOperacion.NoExiste;

        int eliminadas = _adyacencia[vOrigen.Id].RemoveAll(
            id => id.Equals(vDestino.Id, StringComparison.OrdinalIgnoreCase));

        return eliminadas > 0 ? ResultadoOperacion.Exito : ResultadoOperacion.NoExiste;
    }
}