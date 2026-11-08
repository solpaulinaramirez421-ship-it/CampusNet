# CampusNet

Sistema de gestión de red social universitaria desarrollado en **C#** utilizando el patrón de arquitectura **MVC (Modelo-Vista-Controlador)** y una estructura de datos de **Grafo Dirigido** mediante listas de adyacencia.

---

##  Autores

* **Sol Paulina Ramirez** — *Model y controller* 
* **Mariana Jimenez** — *View y README*

---

##  Tecnologías y Arquitectura

El proyecto está diseñado siguiendo una separación clara de responsabilidades:
* **Lenguaje:** C# (.NET)
* **Patrón de Arquitectura:** MVC (`CampusNet.Models`, `CampusNet.Views`, `CampusNet.Controllers`)
* **Estructura de Datos Principal:** Grafo Dirigido (Vértices como Usuarios y Aristas como Relaciones de Seguimiento).
* **Algoritmos e Implementaciones:**
  * Recorridos en anchura (**BFS**) y profundidad (**DFS completo**).
  * Pruebas de alcanzabilidad entre nodos.
  * Análisis de métricas: Usuarios influyentes (grado de entrada), usuarios activos (grado de salida) y usuarios sin seguidores.
  * Operaciones CRUD completas sobre vértices y aristas con control de duplicados y eliminación en cascada.

---

##  Estructura del Proyecto

```text
CampusNet/
│
├── Controllers/
│   └── CampusNetController.cs   # Controlador principal con la lógica de ejecución
├── Modelos/
│   ├── Edge.cs                  # Representación de las aristas del grafo
│   ├── Graph.Consultas.cs       # Métodos de consulta social y métricas
│   ├── Grafo.Recorridos.cs      # Algoritmos de recorrido BFS y DFS
│   ├── Gráfico.cs               # Clase principal del Grafo
│   ├── Rol.cs                   # Enumeración de roles (Estudiante, Profesor, Egresado)
│   └── Vértice.cs               # Representación de los nodos (usuarios)
├── Views/
│   └── ConsolaView.cs           # Salida de datos y formato en consola
└── README.md                    # Documentación del proyecto
