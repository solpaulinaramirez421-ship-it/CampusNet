namespace CampusNet.Models;

// Resultado del DFS completo: orden de descubrimiento y ciclos dirigidos encontrados.
public record ResultadoDfs(List<string> OrdenDescubrimiento, List<List<string>> Ciclos)
{
    public bool TieneCiclos => Ciclos.Count > 0;
}

public partial class Graph
{
    private enum EstadoDfs
    {
        EnProceso,
        Terminado
    }

    // Orden de visita desde el usuario de origen. Incluye al propio origen; vacío si no existe.
    public List<string> Bfs(string origen)
    {
        var orden = new List<string>();

        Vertex? inicio = ObtenerVertice(origen);
        if (inicio == null)
            return orden;

        var visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { inicio.Id };
        var cola = new Queue<string>();
        cola.Enqueue(inicio.Id);

        while (cola.Count > 0)
        {
            string actual = cola.Dequeue();
            orden.Add(actual);

            foreach (string seguido in _adyacencia[actual])
            {
                if (visitados.Add(seguido))
                    cola.Enqueue(seguido);
            }
        }

        return orden;
    }

    // Recorre todo el grafo (reinicia desde cada usuario aún no descubierto).
    public ResultadoDfs DfsCompleto()
    {
        var descubrimiento = new List<string>();
        var ciclos = new List<List<string>>();
        var estado = new Dictionary<string, EstadoDfs>(StringComparer.OrdinalIgnoreCase);
        var camino = new List<string>();

        foreach (Vertex vertice in _vertices.Values)
        {
            if (!estado.ContainsKey(vertice.Id))
                VisitarDfs(vertice.Id, estado, camino, descubrimiento, ciclos);
        }

        return new ResultadoDfs(descubrimiento, ciclos);
    }

    private void VisitarDfs(
        string id,
        Dictionary<string, EstadoDfs> estado,
        List<string> camino,
        List<string> descubrimiento,
        List<List<string>> ciclos)
    {
        estado[id] = EstadoDfs.EnProceso;
        descubrimiento.Add(id);
        camino.Add(id);

        foreach (string seguido in _adyacencia[id])
        {
            if (!estado.TryGetValue(seguido, out EstadoDfs estadoSeguido))
            {
                VisitarDfs(seguido, estado, camino, descubrimiento, ciclos);
            }
            else if (estadoSeguido == EstadoDfs.EnProceso)
            {
                // Arista de retroceso: apunta a un usuario del camino actual y cierra un ciclo dirigido.
                int inicioCiclo = camino.IndexOf(seguido);
                List<string> ciclo = camino.GetRange(inicioCiclo, camino.Count - inicioCiclo);
                ciclo.Add(seguido);
                ciclos.Add(ciclo);
            }
        }

        camino.RemoveAt(camino.Count - 1);
        estado[id] = EstadoDfs.Terminado;
    }
}