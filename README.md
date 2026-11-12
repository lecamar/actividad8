# Sistema de Reservas de Hotel (C#)

Programa de consola hecho en **C#** para controlar las reservas de un hotel pequeño. Evita los errores típicos del registro a mano: que una misma habitación se asigne a dos huéspedes, que se meta más gente de la que cabe, o que el cobro final no coincida con las noches reales.

---

## Archivos del proyecto

| Archivo | ¿Qué contiene? |
|---|---|
| `Persona.cs` | Clase abstracta que sirve de base para las personas del hotel. |
| `Recepcionista.cs` | Hereda de `Persona`. Registra huéspedes y asigna habitaciones. |
| `Huesped.cs` | Hereda de `Persona` e implementa `IFacturable`. Guarda noches, acompañantes y habitaciones. |
| `Habitacion.cs` | Representa una habitación (no hereda de `Persona`). También implementa `IFacturable`. Incluye el `enum TipoHabitacion`. |
| `IFacturable.cs` | Interfaz con los métodos `TotalAPagar()` y `Factura()`. |
| `Program.cs` | Programa principal: crea los datos de prueba, muestra los rechazos y guarda/lee el reporte. |
| `Hotel.csproj` | Archivo de configuración del proyecto (.NET 11). |

Todo el código está dentro del espacio de nombres `Hotel`.

---

## Cómo está organizado el código

```
            Persona (abstracta)                 IFacturable (interfaz)
             /              \                    /                \
    Recepcionista          Huesped ────────────┘                  │
                              │                                    │
                              │ tiene una lista de                 │
                              ▼                                    │
                          Habitacion ──────────────────────────────┘
```

- `Recepcionista` y `Huesped` **heredan** de `Persona`.
- `Huesped` y `Habitacion` **implementan** `IFacturable`, aunque no tienen nada que ver entre sí en herencia.
- Un `Huesped` **tiene** habitaciones (asociación) y un `Recepcionista` **tiene** huéspedes que registró (asociación).

---

## Explicación de cada clase

### `Persona` (clase abstracta)
Es la plantilla común de las personas. Al ser **abstracta**, no se pueden crear objetos `Persona` directamente.

- Campos `protected`: `nombre` y `documento` (los hijos pueden usarlos, el resto del programa no).
- Propiedades de solo lectura: `Nombre` y `Documento` (se pueden consultar pero no cambiar).
- `ToString()` sobrescrito: muestra la persona como `Nombre (Documento: 123)`.
- `IdentificacionIndividuo()` es **abstracto**: no tiene código aquí, cada clase hija lo escribe a su manera.
- El constructor lanza `ArgumentException` si el nombre o el documento vienen vacíos.

### `Recepcionista`
Hereda de `Persona` y llama al constructor del padre con `: base(nombre, documento)`.

- Agrega `Turno`, `CodigoEmpleado` y una lista **privada** de los huéspedes que ha registrado (se consulta con `HuespedesRegistrados`, que devuelve una `IReadOnlyList`, o sea, una lista de solo lectura).
- `RegistrarHuesped(huesped)` rechaza:
  - un huésped `null`,
  - un huésped repetido (mismo documento).
- `AsignarHabitacion(huesped, habitacion)` rechaza si:
  1. el huésped **no lo registró este mismo recepcionista**,
  2. la habitación **ya está ocupada**,
  3. la **capacidad** de la habitación es menor que las personas que viajan (el huésped + sus acompañantes).
- Cada rechazo imprime un mensaje claro con la etiqueta `[RECHAZADO]`, y cada acción correcta con `[OK]`. Los métodos devuelven `true` si todo salió bien y `false` si hubo rechazo.

### `Huesped`
Hereda de `Persona` (con `: base(...)`) e implementa `IFacturable`.

- Guarda `NumeroAcompanantes`, `Noches` y una lista **privada** de habitaciones (se consulta con `Habitaciones`, también de solo lectura).
- `TotalPersonas` = el huésped + sus acompañantes.
- La propiedad `Noches` **valida en el `set`**: si el valor es menor que 1 lanza una `ArgumentException`.
- `AgregarHabitacion(...)` es `internal`: solo el código del proyecto (el `Recepcionista`) puede usarlo, así nadie le agrega habitaciones "a escondidas".
- `TotalAPagar()` calcula:

  ```
  (suma de precios por noche de sus habitaciones) × noches × 1.19
  ```
  El `1.19` incluye el **19 % de IVA**.
- `Factura()` devuelve un texto con cada habitación, las noches, el subtotal, el IVA y el total.

### `Habitacion`
No hereda de `Persona`, pero implementa `IFacturable`.

- Datos: `Numero`, `Tipo` (`Sencilla`, `Doble` o `Suite`), `Capacidad`, `PrecioPorNoche` (tipo `decimal`) y `Ocupada`.
- Todas las propiedades son de solo lectura. Para marcarla como ocupada se usa `Ocupar()`, que es `internal` (solo lo llama el recepcionista).
- `TotalAPagar()` devuelve el valor de **una noche**.

### `IFacturable` (interfaz)
Es un "contrato": cualquier clase que la implemente **debe** tener estos dos métodos.

```csharp
decimal TotalAPagar();
string Factura();
```

Gracias a esto, un `Huesped` y una `Habitacion` se pueden tratar igual aunque sean clases distintas.

---

## Polimorfismo en acción

En `Program.cs` hay dos listas que demuestran el polimorfismo:

1. **`List<Persona>`** con recepcionistas y huéspedes. Al recorrerla y llamar a `IdentificacionIndividuo()`, cada objeto responde con **su propio texto**, aunque la lista sea de tipo `Persona`.
2. **`List<IFacturable>`** con huéspedes y habitaciones. Al recorrerla y llamar a `TotalAPagar()`, cada objeto calcula el total a su manera.

---

## Manejo de archivos

- **Guardar:** el método `GuardarReporte` usa `StreamWriter` dentro de un bloque `using` (así el archivo se cierra solo) y escribe en `reporte_hotel.txt` las personas, las habitaciones y los totales a pagar.
- **Leer:** el método `LeerReporte` usa `File.ReadAllLines`, muestra **cuántas líneas** tiene el archivo y luego imprime su contenido.
- Ambos métodos tienen un `try/catch` para controlar la `IOException` (por ejemplo, si el archivo está bloqueado o no se puede escribir).

---

## Pruebas incluidas (datos de prueba)

**Recepcionistas:** Han Solo (turno de mañana) y Leia Organa (turno de tarde).

**Huéspedes:**

| Huésped | Acompañantes | Personas | Noches | Registrado por |
|---|---|---|---|---|
| Jack Sparrow | 1 | 2 | 3 | Han Solo |
| Marty McFly | 0 | 1 | 2 | Han Solo |
| Forrest Gump | 2 | 3 | 4 | Han Solo |
| Hermione Granger | 1 | 2 | 2 | Leia Organa |

**Habitaciones:**

| Número | Tipo | Capacidad | Precio por noche |
|---|---|---|---|
| 101 | Sencilla | 1 | $80,000 |
| 102 | Doble | 2 | $120,000 |
| 201 | Doble | 2 | $130,000 |
| 202 | Suite | 4 | $250,000 |
| 301 | Suite | 5 | $300,000 |
| 302 | Doble | 2 | $125,000 |

> La habitación 302 solo se usa para probar el rechazo por capacidad: se crea libre, pero es muy pequeña para el grupo de Forrest.

### Casos que el programa demuestra

| Prueba | Qué pasa | Rechazo esperado |
|---|---|---|
| Registrar `null` | Han intenta registrar un huésped nulo | `[RECHAZADO]` huésped nulo |
| Registrar duplicado | Han registra otra vez a Jack | `[RECHAZADO]` ya estaba registrado |
| Huésped de otro recepcionista | Han intenta asignar la 102 a Hermione (la registró Leia) | `[RECHAZADO]` no fue registrado por este recepcionista |
| Habitación ocupada | Han intenta asignar la 101 a Forrest (ya la tiene Marty) | `[RECHAZADO]` habitación ocupada |
| Capacidad insuficiente | Han intenta asignar la 302 (capacidad 2) a Forrest (3 personas) | `[RECHAZADO]` capacidad insuficiente |
| Noches inválidas | Se intenta poner `marty.Noches = 0` | `ArgumentException` capturada y mostrada |

### Totales esperados (con IVA del 19 %)

| Huésped | Cálculo | Total a pagar |
|---|---|---|
| Jack Sparrow | (120,000 + 130,000) × 3 × 1.19 | $892,500 |
| Marty McFly | 80,000 × 2 × 1.19 | $190,400 |
| Forrest Gump | 250,000 × 4 × 1.19 | $1,190,000 |
| Hermione Granger | 300,000 × 2 × 1.19 | $714,000 |

---

## Conceptos aplicados

| Concepto | Dónde se ve |
|---|---|
| **Herencia** | `Recepcionista` y `Huesped` heredan de `Persona` |
| **Clase abstracta** | `Persona` y su método `IdentificacionIndividuo()` |
| **Interfaz** | `IFacturable`, implementada por `Huesped` y `Habitacion` |
| **Encapsulamiento** | Listas privadas, propiedades de solo lectura, `internal`, validación en `Noches` |
| **Polimorfismo** | `List<Persona>` y `List<IFacturable>` en `Program.cs` |
| **Asociación entre clases** | Un huésped tiene habitaciones; un recepcionista tiene huéspedes |
| **Reutilización de código** | Los hijos reutilizan `nombre`, `documento`, `ToString()` y el constructor de `Persona` |
| **Manejo de excepciones** | `ArgumentException` en validaciones e `IOException` en archivos |
| **Manejo de archivos** | `StreamWriter` con `using` y `File.ReadAllLines` |
| **Modularidad** | Un archivo `.cs` por clase e interfaz, todo en el namespace `Hotel` |

##Evidencias

<img width="1920" height="1032" alt="Screenshot 2026-11-11 225118" src="https://github.com/user-attachments/assets/3a175e89-894d-4e50-aab9-75275e9349bc" />
<img width="1920" height="1032" alt="Screenshot 2026-11-11 225138" src="https://github.com/user-attachments/assets/3122da7f-af61-4f19-8be0-c11fef095870" />
<img width="1920" height="1032" alt="Screenshot 2026-11-11 225156" src="https://github.com/user-attachments/assets/f25d44ee-e3c5-4521-8fd9-c7881c183bfa" />
<img width="1920" height="1032" alt="Screenshot 2026-11-11 225203" src="https://github.com/user-attachments/assets/1b62f794-338a-4ecb-987e-41fdfa9e886f" />
<img width="1920" height="1032" alt="Screenshot 2026-11-11 225213" src="https://github.com/user-attachments/assets/486d59a7-98c5-4301-b77d-fe5285cc0821" />
