using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Hotel
{
    class Program
    {
        // Nombre del archivo donde se guarda el reporte
        private const string ArchivoReporte = "reporte_hotel.txt";

        static void Main(string[] args)
        {
            // Para que las tildes y la ñ se vean bien en la consola
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("===== SISTEMA DE RESERVAS DEL HOTEL =====\n");

            // ---------- 1. Datos de prueba ----------
            // 2 recepcionistas (nombres de personajes de películas)
            Recepcionista han = new Recepcionista("Han Solo", "1001", "Mañana", "REC-01");
            Recepcionista leia = new Recepcionista("Leia Organa", "1002", "Tarde", "REC-02");

            // 4 huéspedes (3 mínimos + 1 extra para probar un rechazo)
            Huesped jack = new Huesped("Jack Sparrow", "2001", 1, 3);       // 2 personas, 3 noches
            Huesped marty = new Huesped("Marty McFly", "2002", 0, 2);       // 1 persona, 2 noches
            Huesped forrest = new Huesped("Forrest Gump", "2003", 2, 4);    // 3 personas, 4 noches
            Huesped hermione = new Huesped("Hermione Granger", "2004", 1, 2); // 2 personas, 2 noches

            // 5 habitaciones
            Habitacion h101 = new Habitacion(101, TipoHabitacion.Sencilla, 1, 80000m);
            Habitacion h102 = new Habitacion(102, TipoHabitacion.Doble, 2, 120000m);
            Habitacion h201 = new Habitacion(201, TipoHabitacion.Doble, 2, 130000m);
            Habitacion h202 = new Habitacion(202, TipoHabitacion.Suite, 4, 250000m);
            Habitacion h301 = new Habitacion(301, TipoHabitacion.Suite, 5, 300000m);

            // ---------- 2. Registro de huéspedes (y rechazos) ----------
            Console.WriteLine("--- Registro de huéspedes ---");
            han.RegistrarHuesped(jack);
            han.RegistrarHuesped(marty);
            han.RegistrarHuesped(forrest);
            leia.RegistrarHuesped(hermione);

            Console.WriteLine("\nPrueba de rechazo: huésped null");
            han.RegistrarHuesped(null);

            Console.WriteLine("\nPrueba de rechazo: huésped duplicado");
            han.RegistrarHuesped(jack);

            // ---------- 3. Asignación de habitaciones (y rechazos) ----------
            Console.WriteLine("\n--- Asignación de habitaciones ---");
            han.AsignarHabitacion(jack, h102);    // OK: 2 personas en capacidad 2
            han.AsignarHabitacion(jack, h201);    // OK: segunda habitación para Jack
            han.AsignarHabitacion(marty, h101);   // OK: 1 persona en capacidad 1
            han.AsignarHabitacion(forrest, h202); // OK: 3 personas en capacidad 4
            leia.AsignarHabitacion(hermione, h301); // OK

            Console.WriteLine("\nPrueba de rechazo: huésped que registró otro recepcionista");
            han.AsignarHabitacion(hermione, h102);

            Console.WriteLine("\nPrueba de rechazo: habitación ocupada");
            han.AsignarHabitacion(forrest, h101);

            Console.WriteLine("\nPrueba de rechazo: capacidad insuficiente");
            // Se crea una habitación libre pero pequeña para que el rechazo sea por capacidad
            Habitacion h302 = new Habitacion(302, TipoHabitacion.Doble, 2, 125000m);
            han.AsignarHabitacion(forrest, h302); // Forrest viaja con 3 personas

            // ---------- 4. Validación de Noches (ArgumentException) ----------
            Console.WriteLine("\n--- Prueba de validación de Noches ---");
            try
            {
                marty.Noches = 0; // esto debe lanzar una excepción
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  [EXCEPCIÓN] {ex.Message}");
            }

            // ---------- 5. Polimorfismo con List<Persona> ----------
            Console.WriteLine("\n--- Lista de personas (polimorfismo) ---");
            List<Persona> personas = new List<Persona> { han, leia, jack, marty, forrest, hermione };
            foreach (Persona p in personas)
            {
                // Cada objeto responde según su propia clase
                Console.WriteLine("  " + p.IdentificacionIndividuo());
            }

            // ---------- 6. Polimorfismo con List<IFacturable> ----------
            Console.WriteLine("\n--- Lista de facturables (interfaz) ---");
            List<IFacturable> facturables = new List<IFacturable> { jack, marty, forrest, hermione, h101, h102, h202 };
            foreach (IFacturable f in facturables)
            {
                Console.WriteLine($"  Total: ${f.TotalAPagar().ToString("N0", CultureInfo.InvariantCulture)}");
            }

            Console.WriteLine("\n--- Facturas de los huéspedes ---");
            Huesped[] huespedes = { jack, marty, forrest, hermione };
            foreach (Huesped h in huespedes)
            {
                Console.WriteLine(h.Factura());
                Console.WriteLine();
            }

            // ---------- 7. Manejo de archivos ----------
            Habitacion[] habitaciones = { h101, h102, h201, h202, h301, h302 };
            GuardarReporte(personas, habitaciones);
            LeerReporte();

            Console.WriteLine("\n===== FIN DEL PROGRAMA =====");
        }

        // Guarda la información de cada objeto en reporte_hotel.txt
        static void GuardarReporte(List<Persona> personas, Habitacion[] habitaciones)
        {
            try
            {
                // El bloque using cierra el archivo automáticamente al terminar
                using (StreamWriter escritor = new StreamWriter(ArchivoReporte))
                {
                    escritor.WriteLine("=== PERSONAS ===");
                    foreach (Persona p in personas)
                    {
                        escritor.WriteLine(p.ToString() + " | " + p.IdentificacionIndividuo());
                    }

                    escritor.WriteLine("=== HABITACIONES ===");
                    foreach (Habitacion h in habitaciones)
                    {
                        escritor.WriteLine(h.ToString());
                    }

                    escritor.WriteLine("=== TOTALES A PAGAR ===");
                    foreach (Persona p in personas)
                    {
                        // Solo los huéspedes tienen total a pagar
                        if (p is Huesped huesped)
                        {
                            escritor.WriteLine($"{huesped.Nombre}: ${huesped.TotalAPagar().ToString("N0", CultureInfo.InvariantCulture)}");
                        }
                    }
                }
                Console.WriteLine($"\nReporte guardado en {ArchivoReporte}.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"  [ERROR DE ARCHIVO] No se pudo guardar el reporte: {ex.Message}");
            }
        }

        // Lee el archivo con File.ReadAllLines y muestra cuántas líneas tiene y su contenido
        static void LeerReporte()
        {
            try
            {
                string[] lineas = File.ReadAllLines(ArchivoReporte);
                Console.WriteLine($"\n--- Contenido de {ArchivoReporte} ({lineas.Length} líneas) ---");
                foreach (string linea in lineas)
                {
                    Console.WriteLine(linea);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"  [ERROR DE ARCHIVO] No se pudo leer el reporte: {ex.Message}");
            }
        }
    }
}
