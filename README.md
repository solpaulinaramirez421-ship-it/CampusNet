# CampusNet

Módulo interno que simula una red social dirigida de usuarios académicos (estudiantes, profesores y egresados), desarrollado en **C# (.NET Console App)** con arquitectura **MVC estricta** y un **grafo dirigido** implementado con lista de adyacencia.

Actividad Colaborativa II: Taller Integrador – Grafos Dirigidos. Programación III, Universidad de Manizales.

## Integrantes y responsabilidades

| Integrante | Responsabilidad |
|---|---|
| **Sol Paulina Ramirez** | Model (`Vertex`, `Edge`, `Graph`, BFS, DFS, consultas y CRUD) y Controller (casos de uso) |
| **Mariana Jimenez** | View (impresión en consola) y README |

## Cómo ejecutar

Requisitos: SDK de .NET instalado (el proyecto apunta a `net10.0`, ver `CampusNet.csproj`).

```bash
git clone https://github.com/solpaulinaramirez421-ship-it/CampusNet.git
cd CampusNet
dotnet run
```

No requiere configuración adicional. El programa ejecuta los 5 casos en orden y muestra todo por consola.

## Arquitectura

- **Model** (`Models/`): `Vertex` (id, nombre, rol), `Edge` (origen → destino), `Rol` y `Graph`, con la lista de adyacencia `Dictionary<string, List<string>>`. El Model no usa consola: devuelve resultados (`Exito`, `Duplicado`, `NoExiste`, `Invalido`) y la View los traduce a mensajes.
- **View** (`Views/ConsolaView.cs`): solo imprime (lista de adyacencia, recorridos, consultas, métricas).
- **Controller** (`Controllers/CampusNetController.cs`): ejecuta la secuencia de casos de uso y conecta Model y View.
- `Program.cs` solo invoca al Controller.

```text
CampusNet/
├── Controllers/
│   └── CampusNetController.cs   # Casos de uso y comunicación entre capas
├── Models/
│   ├── Edge.cs                  # Arista dirigida (Origen -> Destino)
│   ├── Graph.cs                 # Grafo, lista de adyacencia y CRUD
│   ├── Graph.Recorridos.cs      # BFS y DFS completo (con detección de ciclos)
│   ├── Graph.Consultas.cs       # Grados, influyentes, activos, sin seguidores y alcanzabilidad
│   ├── Rol.cs                   # Estudiante, Profesor, Egresado
│   └── Vertex.cs                # Usuario (vértice)
├── Views/
│   └── ConsolaView.cs           # Salida por consola
├── CampusNet.csproj
├── Program.cs                   # Solo invoca al Controller
└── README.md
```

## Control de duplicados

- No se aceptan dos usuarios con el mismo Id (se compara sin distinguir mayúsculas: `u01` es igual a `U01`).
- No se aceptan dos relaciones iguales (mismo origen y destino).
- Tampoco se aceptan relaciones con usuarios inexistentes ni un usuario que se siga a sí mismo.
- El Controller prueba estos casos al construir el grafo y la consola muestra cada rechazo.
- Al eliminar un usuario se eliminan también todas las relaciones que entran o salen de él.

## Casos de uso implementados

1. **Construcción:** 12 usuarios (6 estudiantes, 3 profesores y 3 egresados) y 24 relaciones.
2. **Recorridos:** BFS desde U01, U10 y U12 (orden de visita y cantidad alcanzada) y DFS completo (orden de descubrimiento y ciclos).
3. **Consultas:** usuarios sin seguidores, influyentes, más activos y alcanzabilidad con BFS.
4. **CRUD:** agregar y eliminar usuarios, agregar y eliminar relaciones, actualizar nombre y rol. Cada operación imprime su resultado y el grafo.
5. **Métricas y análisis final** sobre la estructura resultante.

Cumplimiento de los requisitos del grafo inicial:

- Grado de salida ≥ 4: **U01** (4) y **U03** (4).
- Ciclos dirigidos: U01 → U02 → U03 → U01 y U04 → U06 → U07 → U10 → U04.
- Grado de entrada 0: **U11** y **U12**.

## Resultados y análisis

### Recorridos sobre el grafo inicial

| Recorrido | Orden | Alcanzados |
|---|---|---|
| BFS desde U01 | U01, U02, U03, U07, U08, U05, U04, U06, U10, U09 | 10 de 12 |
| BFS desde U10 | U10, U04, U06, U09, U07 | 5 de 12 |
| BFS desde U12 | U12, U01, U05, U02, U03, U07, U08, U06, U10, U04, U09 | 11 de 12 |
| DFS completo | U01, U02, U03, U04, U06, U07, U10, U09, U05, U08, U11, U12 | 12 de 12 |

**Justificación:**

- Desde U01 no se alcanza a U11 ni a U12 porque nadie los sigue: no tienen aristas de entrada, así que ningún recorrido que empiece en otro usuario puede llegar a ellos. Por eso U01 alcanza 10 de 12 y U12 alcanza 11 (todos menos U11).
- U10 alcanza solo 5 usuarios porque solo sigue a U04, y desde ahí el recorrido queda dentro del ciclo U04 → U06 → U07 → U10 más U09, que se alcanza desde U04.
- El DFS completo reinicia desde los usuarios no descubiertos (U11 y U12), por eso descubre los 12.
- El DFS detecta dos ciclos dirigidos mediante aristas de retroceso (un seguido que todavía está en el camino actual): U01 → U02 → U03 → U01 y U04 → U06 → U07 → U10 → U04.

### Consultas sociales (grafo inicial)

- **Sin seguidores:** U11 y U12 (grado de entrada 0).
- **Más influyentes:** U03 y U06, empatados con 4 seguidores cada uno. U03 es seguido por U01, U02, U08 y U11. U06 es seguido por U03, U04, U05 y U11.
- **Más activos:** U01 y U03, empatados siguiendo a 4 usuarios cada uno.
- **Alcanzabilidad (BFS):** U12 → U10 es SÍ; U11 → U09 es SÍ; U10 → U01 es NO; U01 → U12 es NO.
  - U10 → U01 es NO porque desde U10 solo se llega a U04, U06, U07 y U09, y ninguno de ellos conduce a U01.
  - U01 → U12 es NO porque U12 no tiene seguidores.

### Estructura final tras el CRUD

Operaciones aplicadas: agregar U13, agregar U13 → U01 y U13 → U04, eliminar U13 → U04, actualizar el nombre de U02 y el rol de U08, y eliminar U09 con sus relaciones.

- Al eliminar U09 se eliminan 3 relaciones: U04 → U09, U08 → U09 y U09 → U10.
- Estructura final: **12 usuarios y 22 relaciones**. Cuentas: 24 relaciones iniciales + 2 agregadas − 1 eliminada − 3 de U09 = 22.
- **Sin seguidores:** U11, U12 y U13.
- **Más influyentes:** U03 y U06 siguen empatados con 4 seguidores (la eliminación de U09 no les afecta).
- **Más activos:** U01 y U03 siguen con 4. U04 baja a 1 porque perdió su relación con U09.
- Los dos ciclos se mantienen: ninguna de las relaciones eliminadas formaba parte de ellos. El DFS final los detecta de nuevo.

## Evidencia de ejecución

> Pega aquí las capturas de pantalla de la consola (grafo inicial, BFS/DFS, consultas, CRUD y métricas finales), por ejemplo:
>
> `![Caso 1](capturas/caso1.png)`

