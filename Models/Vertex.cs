namespace CampusNet.Models;

// Un usuario de la red. El Id es único dentro del grafo.
public class Vertex
{
    public string Id { get; }
    public string Nombre { get; set; }
    public Rol Rol { get; set; }

    public Vertex(string id, string nombre, Rol rol)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("El Id no puede estar vacío.", nameof(id));
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));

        Id = id.Trim();
        Nombre = nombre.Trim();
        Rol = rol;
    }
}