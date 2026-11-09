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

`````text
============================================================
CASO 1: CONSTRUCCIÓN DEL GRAFO
Grafo dirigido vacío creado.

Insertando usuarios:
Agregar usuario U01 (Ana Torres, Estudiante): OK
Agregar usuario U02 (Bruno Ríos, Estudiante): OK
Agregar usuario U03 (Carla Mejía, Profesor): OK
Agregar usuario U04 (Diego Salazar, Egresado): OK
Agregar usuario U05 (Elena Vargas, Estudiante): OK
Agregar usuario U06 (Felipe Cano, Profesor): OK
Agregar usuario U07 (Gloria Pineda, Egresado): OK
Agregar usuario U08 (Hugo Marín, Estudiante): OK
Agregar usuario U09 (Irene Duque, Estudiante): OK
Agregar usuario U10 (Javier Osorio, Profesor): OK
Agregar usuario U11 (Karen Londoño, Egresado): OK
Agregar usuario U12 (Luis Arango, Estudiante): OK

Insertando relaciones de seguimiento:
Agregar relación U01 -> U02: OK
Agregar relación U01 -> U03: OK
Agregar relación U01 -> U07: OK
Agregar relación U01 -> U08: OK
Agregar relación U02 -> U03: OK
Agregar relación U02 -> U05: OK
Agregar relación U03 -> U01: OK
Agregar relación U03 -> U04: OK
Agregar relación U03 -> U05: OK
Agregar relación U03 -> U06: OK
Agregar relación U04 -> U06: OK
Agregar relación U04 -> U09: OK
Agregar relación U05 -> U06: OK
Agregar relación U05 -> U10: OK
Agregar relación U06 -> U07: OK
Agregar relación U07 -> U10: OK
Agregar relación U08 -> U03: OK
Agregar relación U08 -> U09: OK
Agregar relación U09 -> U10: OK
Agregar relación U10 -> U04: OK
Agregar relación U11 -> U03: OK
Agregar relación U11 -> U06: OK
Agregar relación U12 -> U01: OK
Agregar relación U12 -> U05: OK

Prueba de control de duplicados y datos inválidos:
Agregar usuario u01 (Otra Persona, Profesor): RECHAZADO: ya existe (duplicado)
Agregar relación U01 -> U02: RECHAZADO: ya existe (duplicado)
Agregar relación U01 -> U01: RECHAZADO: datos inválidos
Agregar relación U01 -> U99: RECHAZADO: no existe (usuario o relación)

Estructura del grafo (lista de adyacencia):
Grafo dirigido: 12 usuarios, 24 relaciones
  U01 (Ana Torres, Estudiante)          -> U02, U03, U07, U08
  U02 (Bruno Ríos, Estudiante)          -> U03, U05
  U03 (Carla Mejía, Profesor)           -> U01, U04, U05, U06
  U04 (Diego Salazar, Egresado)         -> U06, U09
  U05 (Elena Vargas, Estudiante)        -> U06, U10
  U06 (Felipe Cano, Profesor)           -> U07
  U07 (Gloria Pineda, Egresado)         -> U10
  U08 (Hugo Marín, Estudiante)          -> U03, U09
  U09 (Irene Duque, Estudiante)         -> U10
  U10 (Javier Osorio, Profesor)         -> U04
  U11 (Karen Londoño, Egresado)         -> U03, U06
  U12 (Luis Arango, Estudiante)         -> U01, U05

============================================================
CASO 2: RECORRIDOS
============================================================
BFS desde tres usuarios distintos:
BFS desde U01:
  Orden de visita: U01 -> U02 -> U03 -> U07 -> U08 -> U05 -> U04 -> U06 -> U10 -> U09
  Vértices alcanzados: 10 de 12
BFS desde U10:
  Orden de visita: U10 -> U04 -> U06 -> U09 -> U07
  Vértices alcanzados: 5 de 12
BFS desde U12:
  Orden de visita: U12 -> U01 -> U05 -> U02 -> U03 -> U07 -> U08 -> U06 -> U10 -> U04 -> U09
  Vértices alcanzados: 11 de 12

DFS completo:
  Orden de descubrimiento: U01 -> U02 -> U03 -> U04 -> U06 -> U07 -> U10 -> U09 -> U05 -> U08 -> U11 -> U12
  Vértices descubiertos: 12
  Ciclos dirigidos encontrados: 2
    U01 -> U02 -> U03 -> U01
    U04 -> U06 -> U07 -> U10 -> U04

============================================================
CASO 3: CONSULTAS SOCIALES
============================================================

Usuarios sin seguidores (grado de entrada 0):
  U11 Karen Londoño (Egresado)
  U12 Luis Arango (Estudiante)

Usuarios influyentes (mayor grado de entrada):
  U03 Carla Mejía (Profesor) - seguidores: 4
  U06 Felipe Cano (Profesor) - seguidores: 4

Usuarios más activos (mayor grado de salida):
  U01 Ana Torres (Estudiante) - sigue a: 4
  U03 Carla Mejía (Profesor) - sigue a: 4

Alcanzabilidad (BFS):
¿U12 puede llegar a U10? SÍ
¿U11 puede llegar a U09? SÍ
¿U10 puede llegar a U01? NO
¿U01 puede llegar a U12? NO

============================================================
CASO 4: OPERACIONES CRUD
============================================================
1) Agregar un usuario:
Agregar usuario U13 (Marta Soto, Egresado): OK
Grafo dirigido: 13 usuarios, 24 relaciones
  U01 (Ana Torres, Estudiante)          -> U02, U03, U07, U08
  U02 (Bruno Ríos, Estudiante)          -> U03, U05
  U03 (Carla Mejía, Profesor)           -> U01, U04, U05, U06
  U04 (Diego Salazar, Egresado)         -> U06, U09
  U05 (Elena Vargas, Estudiante)        -> U06, U10
  U06 (Felipe Cano, Profesor)           -> U07
  U07 (Gloria Pineda, Egresado)         -> U10
  U08 (Hugo Marín, Estudiante)          -> U03, U09
  U09 (Irene Duque, Estudiante)         -> U10
  U10 (Javier Osorio, Profesor)         -> U04
  U11 (Karen Londoño, Egresado)         -> U03, U06
  U12 (Luis Arango, Estudiante)         -> U01, U05
  U13 (Marta Soto, Egresado)            -> (no sigue a nadie)

2) Agregar relaciones:
Agregar relación U13 -> U01: OK
Agregar relación U13 -> U04: OK
Grafo dirigido: 13 usuarios, 26 relaciones
  U01 (Ana Torres, Estudiante)          -> U02, U03, U07, U08
  U02 (Bruno Ríos, Estudiante)          -> U03, U05
  U03 (Carla Mejía, Profesor)           -> U01, U04, U05, U06
  U04 (Diego Salazar, Egresado)         -> U06, U09
  U05 (Elena Vargas, Estudiante)        -> U06, U10
  U06 (Felipe Cano, Profesor)           -> U07
  U07 (Gloria Pineda, Egresado)         -> U10
  U08 (Hugo Marín, Estudiante)          -> U03, U09
  U09 (Irene Duque, Estudiante)         -> U10
  U10 (Javier Osorio, Profesor)         -> U04
  U11 (Karen Londoño, Egresado)         -> U03, U06
  U12 (Luis Arango, Estudiante)         -> U01, U05
  U13 (Marta Soto, Egresado)            -> U01, U04

3) Eliminar una relación (la segunda vez ya no existe):
Eliminar relación U13 -> U04: OK
Eliminar relación U13 -> U04: RECHAZADO: no existe (usuario o relación)
Grafo dirigido: 13 usuarios, 25 relaciones
  U01 (Ana Torres, Estudiante)          -> U02, U03, U07, U08
  U02 (Bruno Ríos, Estudiante)          -> U03, U05
  U03 (Carla Mejía, Profesor)           -> U01, U04, U05, U06
  U04 (Diego Salazar, Egresado)         -> U06, U09
  U05 (Elena Vargas, Estudiante)        -> U06, U10
  U06 (Felipe Cano, Profesor)           -> U07
  U07 (Gloria Pineda, Egresado)         -> U10
  U08 (Hugo Marín, Estudiante)          -> U03, U09
  U09 (Irene Duque, Estudiante)         -> U10
  U10 (Javier Osorio, Profesor)         -> U04
  U11 (Karen Londoño, Egresado)         -> U03, U06
  U12 (Luis Arango, Estudiante)         -> U01, U05
  U13 (Marta Soto, Egresado)            -> U01

4) Actualizar nombre y rol (el último intento es inválido):
Actualizar usuario U02: nombre = 'Bruno Ríos Mejía': OK
Actualizar usuario U08: rol = Egresado: OK
Actualizar usuario U02: nombre = '': RECHAZADO: datos inválidos
Grafo dirigido: 13 usuarios, 25 relaciones
  U01 (Ana Torres, Estudiante)          -> U02, U03, U07, U08
  U02 (Bruno Ríos Mejía, Estudiante)    -> U03, U05
  U03 (Carla Mejía, Profesor)           -> U01, U04, U05, U06
  U04 (Diego Salazar, Egresado)         -> U06, U09
  U05 (Elena Vargas, Estudiante)        -> U06, U10
  U06 (Felipe Cano, Profesor)           -> U07
  U07 (Gloria Pineda, Egresado)         -> U10
  U08 (Hugo Marín, Egresado)            -> U03, U09
  U09 (Irene Duque, Estudiante)         -> U10
  U10 (Javier Osorio, Profesor)         -> U04
  U11 (Karen Londoño, Egresado)         -> U03, U06
  U12 (Luis Arango, Estudiante)         -> U01, U05
  U13 (Marta Soto, Egresado)            -> U01

5) Eliminar un usuario junto con sus relaciones:
Eliminar usuario U09: OK
  Relaciones eliminadas junto con el usuario: 3
Eliminar usuario U99: RECHAZADO: no existe (usuario o relación)
Grafo dirigido: 12 usuarios, 22 relaciones
  U01 (Ana Torres, Estudiante)          -> U02, U03, U07, U08
  U02 (Bruno Ríos Mejía, Estudiante)    -> U03, U05
  U03 (Carla Mejía, Profesor)           -> U01, U04, U05, U06
  U04 (Diego Salazar, Egresado)         -> U06
  U05 (Elena Vargas, Estudiante)        -> U06, U10
  U06 (Felipe Cano, Profesor)           -> U07
  U07 (Gloria Pineda, Egresado)         -> U10
  U08 (Hugo Marín, Egresado)            -> U03
  U10 (Javier Osorio, Profesor)         -> U04
  U11 (Karen Londoño, Egresado)         -> U03, U06
  U12 (Luis Arango, Estudiante)         -> U01, U05
  U13 (Marta Soto, Egresado)            -> U01

============================================================
CASO 5: MÉTRICAS Y ANÁLISIS FINAL
============================================================
Métricas por usuario (entrada = seguidores, salida = a cuántos sigue):
Id    Nombre              Rol          Entrada  Salida
------------------------------------------------------
U01   Ana Torres          Estudiante         3       4
U02   Bruno Ríos Mejía    Estudiante         1       2
U03   Carla Mejía         Profesor           4       4
U04   Diego Salazar       Egresado           2       1
U05   Elena Vargas        Estudiante         3       2
U06   Felipe Cano         Profesor           4       1
U07   Gloria Pineda       Egresado           2       1
U08   Hugo Marín          Egresado           1       1
U10   Javier Osorio       Profesor           2       1
U11   Karen Londoño       Egresado           0       2
U12   Luis Arango         Estudiante         0       2
U13   Marta Soto          Egresado           0       1
------------------------------------------------------
Total: 12 usuarios y 22 relaciones


Usuarios sin seguidores en la estructura final:
  U11 Karen Londoño (Egresado)
  U12 Luis Arango (Estudiante)
  U13 Marta Soto (Egresado)

Usuarios influyentes en la estructura final:
  U03 Carla Mejía (Profesor) - seguidores: 4
  U06 Felipe Cano (Profesor) - seguidores: 4

Usuarios más activos en la estructura final:
  U01 Ana Torres (Estudiante) - sigue a: 4
  U03 Carla Mejía (Profesor) - sigue a: 4

DFS completo sobre la estructura final:
  Orden de descubrimiento: U01 -> U02 -> U03 -> U04 -> U06 -> U07 -> U10 -> U05 -> U08 -> U11 -> U12 -> U13
  Vértices descubiertos: 12
  Ciclos dirigidos encontrados: 2
    U01 -> U02 -> U03 -> U01
    U04 -> U06 -> U07 -> U10 -> U04
```
