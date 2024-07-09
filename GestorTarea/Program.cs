using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagerApp
{
    public class Tarea
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Prioridad { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Categoria { get; set; }

        public Tarea(string titulo, string descripcion, int prioridad, DateTime fechaVencimiento, string categoria)
        {
            Titulo = titulo;
            Descripcion = descripcion;
            Prioridad = prioridad;
            FechaVencimiento = fechaVencimiento;
            Categoria = categoria;
        }

        public override string ToString()
        {
            return $"{Titulo}: {Descripcion} (Prioridad: {Prioridad}, Vence: {FechaVencimiento.ToShortDateString()}, Categoría: {Categoria})";
        }
    }

    public class GestorTareas
    {
        private List<Tarea> tareas = new List<Tarea>();
        private Stack<string> historialAcciones = new Stack<string>();
        private Stack<string> pilaRehacer = new Stack<string>();
        private Queue<Tarea> tareasUrgentes = new Queue<Tarea>();
        private Dictionary<string, List<Tarea>> arbolTareas = new Dictionary<string, List<Tarea>>();
        private Dictionary<string, Tarea> respaldoTareas = new Dictionary<string, Tarea>();

        public void AgregarTarea(Tarea tarea)
        {
            tareas.Add(tarea);
            tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
            historialAcciones.Push($"Agregar:{tarea.Titulo}");
            AgregarTareaAlArbol(tarea);
            respaldoTareas[tarea.Titulo] = tarea;
        }

        public void EliminarTarea(string titulo)
        {
            Tarea tarea = tareas.Find(t => t.Titulo == titulo);
            if (tarea != null)
            {
                tareas.Remove(tarea);
                historialAcciones.Push($"Eliminar:{tarea.Titulo}");
                EliminarTareaDelArbol(tarea);
                respaldoTareas[tarea.Titulo] = tarea;
            }
        }

        public void ModificarTarea(string titulo, Tarea nuevaTarea)
        {
            Tarea tareaOriginal = tareas.Find(t => t.Titulo == titulo);
            if (tareaOriginal != null)
            {
                respaldoTareas[titulo] = tareaOriginal;
                EliminarTarea(titulo);
                AgregarTarea(nuevaTarea);
                historialAcciones.Push($"Modificar:{titulo}");
            }
        }

        public void MostrarTareas()
        {
            foreach (var tarea in tareas)
            {
                Console.WriteLine(tarea);
            }
        }

        public void AgregarTareaUrgente(Tarea tarea)
        {
            tareasUrgentes.Enqueue(tarea);
            historialAcciones.Push($"AgregarUrgente:{tarea.Titulo}");
            respaldoTareas[tarea.Titulo] = tarea;
        }

        public void ProcesarTareasUrgentes()
        {
            while (tareasUrgentes.Count > 0)
            {
                Tarea tarea = tareasUrgentes.Dequeue();
                Console.WriteLine($"Procesando tarea urgente: {tarea}");
            }
        }

        private void AgregarTareaAlArbol(Tarea tarea)
        {
            if (!arbolTareas.ContainsKey(tarea.Categoria))
            {
                arbolTareas[tarea.Categoria] = new List<Tarea>();
            }
            arbolTareas[tarea.Categoria].Add(tarea);
        }

        private void EliminarTareaDelArbol(Tarea tarea)
        {
            if (arbolTareas.ContainsKey(tarea.Categoria))
            {
                arbolTareas[tarea.Categoria].Remove(tarea);
            }
        }

        public void MostrarArbolTareas()
        {
            foreach (var categoria in arbolTareas.Keys)
            {
                Console.WriteLine($"{categoria}:");
                foreach (var tarea in arbolTareas[categoria])
                {
                    Console.WriteLine($"\t{tarea}");
                }
            }
        }

        public void Deshacer()
        {
            if (historialAcciones.Count > 0)
            {
                string ultimaAccion = historialAcciones.Pop();
                string[] partesAccion = ultimaAccion.Split(':');
                string tipoAccion = partesAccion[0];
                string tituloTarea = partesAccion[1];

                switch (tipoAccion)
                {
                    case "Agregar":
                        Tarea tareaAgregada = respaldoTareas[tituloTarea];
                        tareas.Remove(tareaAgregada);
                        EliminarTareaDelArbol(tareaAgregada);
                        pilaRehacer.Push(ultimaAccion);
                        break;
                    case "Eliminar":
                        Tarea tareaEliminada = respaldoTareas[tituloTarea];
                        tareas.Add(tareaEliminada);
                        AgregarTareaAlArbol(tareaEliminada);
                        pilaRehacer.Push(ultimaAccion);
                        break;
                    case "Modificar":
                        Tarea tareaOriginal = respaldoTareas[tituloTarea];
                        Tarea tareaModificada = tareas.Find(t => t.Titulo == tituloTarea);
                        if (tareaModificada != null)
                        {
                            tareas.Remove(tareaModificada);
                            tareas.Add(tareaOriginal);
                            tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
                            EliminarTareaDelArbol(tareaModificada);
                            AgregarTareaAlArbol(tareaOriginal);
                            pilaRehacer.Push(ultimaAccion);
                        }
                        break;
                    case "AgregarUrgente":
                        Tarea tareaUrgente = respaldoTareas[tituloTarea];
                        tareasUrgentes = new Queue<Tarea>(tareasUrgentes.Where(t => t.Titulo != tituloTarea));
                        pilaRehacer.Push(ultimaAccion);
                        break;
                }
            }
        }

        public void Rehacer()
        {
            if (pilaRehacer.Count > 0)
            {
                string ultimaAccionDeshecha = pilaRehacer.Pop();
                string[] partesAccion = ultimaAccionDeshecha.Split(':');
                string tipoAccion = partesAccion[0];
                string tituloTarea = partesAccion[1];

                Tarea tarea = respaldoTareas[tituloTarea];

                switch (tipoAccion)
                {
                    case "Agregar":
                        tareas.Add(tarea);
                        tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
                        AgregarTareaAlArbol(tarea);
                        break;
                    case "Eliminar":
                        tareas.Remove(tarea);
                        EliminarTareaDelArbol(tarea);
                        break;
                    case "Modificar":
                        Tarea tareaModificada = tareas.Find(t => t.Titulo == tarea.Titulo);
                        if (tareaModificada != null)
                        {
                            tareas.Remove(tareaModificada);
                            tareas.Add(tarea);
                            tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
                            EliminarTareaDelArbol(tareaModificada);
                            AgregarTareaAlArbol(tarea);
                        }
                        break;
                    case "AgregarUrgente":
                        tareasUrgentes.Enqueue(tarea);
                        break;
                }
                historialAcciones.Push(ultimaAccionDeshecha);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GestorTareas gestorTareas = new GestorTareas();
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("Seleccione la opcion deseada:");
                Console.WriteLine("1. Agregar tarea");
                Console.WriteLine("2. Eliminar tarea");
                Console.WriteLine("3. Modificar tarea");
                Console.WriteLine("4. Mostrar tareas");
                Console.WriteLine("5. Agregar tarea urgente");
                Console.WriteLine("6. Procesar tareas urgentes");
                Console.WriteLine("7. Mostrar árbol de tareas");
                Console.WriteLine("8. Deshacer última acción");
                Console.WriteLine("9. Rehacer última acción");
                Console.WriteLine("10. Salir");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Título: ");
                        string titulo = Console.ReadLine();
                        Console.Write("Descripción: ");
                        string descripcion = Console.ReadLine();

                        int prioridad;
                        while (true)
                        {
                            Console.Write("Prioridad (bajo, medio, alto): ");
                            string entrada = Console.ReadLine().ToLower();

                            if (entrada == "bajo")
                            {
                                prioridad = 1;
                                break;
                            }
                            else if (entrada == "medio")
                            {
                                prioridad = 2;
                                break;
                            }
                            else if (entrada == "alto")
                            {
                                prioridad = 3;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("la Prioridad no es válida. Inténtelo de nuevo.");
                            }
                        }

                        DateTime fechaVencimiento;
                        while (true)
                        {
                            Console.Write("Fecha de vencimiento (yyyy-MM-dd): ");
                            if (DateTime.TryParse(Console.ReadLine(), out fechaVencimiento))
                                break;
                            Console.WriteLine("la Fecha no es válida. Inténtelo de nuevo.");
                        }

                        Console.Write("Categoría: ");
                        string categoria = Console.ReadLine();

                        Tarea nuevaTarea = new Tarea(titulo, descripcion, prioridad, fechaVencimiento, categoria);
                        gestorTareas.AgregarTarea(nuevaTarea);
                        break;

                    case "2":
                        Console.Write("Título de la tarea a eliminar: ");
                        string tituloEliminar = Console.ReadLine();
                        gestorTareas.EliminarTarea(tituloEliminar);
                        break;

                    case "3":
                        Console.Write("Título de la tarea que desea modificar: ");
                        string tituloModificar = Console.ReadLine();
                        Console.Write("Nuevo título: ");
                        string nuevoTitulo = Console.ReadLine();
                        Console.Write("Nueva descripción: ");
                        string nuevaDescripcion = Console.ReadLine();

                        int nuevaPrioridad;
                        while (true)
                        {
                            Console.Write("Nueva prioridad (número): ");
                            if (int.TryParse(Console.ReadLine(), out nuevaPrioridad))
                                break;
                            Console.WriteLine("la Prioridad no es válida. Inténtelo de nuevo.");
                        }

                        DateTime nuevaFechaVencimiento;
                        while (true)
                        {
                            Console.Write("Nueva fecha de vencimiento (yyyy-MM-dd): ");
                            if (DateTime.TryParse(Console.ReadLine(), out nuevaFechaVencimiento))
                                break;
                            Console.WriteLine("la Fecha no es válida. Inténtelo de nuevo.");
                        }

                        Console.Write("Nueva categoría: ");
                        string nuevaCategoria = Console.ReadLine();

                        Tarea tareaModificada = new Tarea(nuevoTitulo, nuevaDescripcion, nuevaPrioridad, nuevaFechaVencimiento, nuevaCategoria);
                        gestorTareas.ModificarTarea(tituloModificar, tareaModificada);
                        break;

                    case "4":
                        gestorTareas.MostrarTareas();
                        break;

                    case "5":
                        Console.Write("Título: ");
                        string tituloUrgente = Console.ReadLine();
                        Console.Write("Descripción: ");
                        string descripcionUrgente = Console.ReadLine();

                        int prioridadUrgente;
                        while (true)
                        {
                            Console.Write("Prioridad (número): ");
                            if (int.TryParse(Console.ReadLine(), out prioridadUrgente))
                                break;
                            Console.WriteLine("la Prioridad no es válida. Inténtelo de nuevo.");
                        }

                        DateTime fechaVencimientoUrgente;
                        while (true)
                        {
                            Console.Write("Fecha de vencimiento (yyyy-MM-dd): ");
                            if (DateTime.TryParse(Console.ReadLine(), out fechaVencimientoUrgente))
                                break;
                            Console.WriteLine("la Fecha no es válida. Inténtelo de nuevo.");
                        }

                        Console.Write("Categoría: ");
                        string categoriaUrgente = Console.ReadLine();

                        Tarea tareaUrgente = new Tarea(tituloUrgente, descripcionUrgente, prioridadUrgente, fechaVencimientoUrgente, categoriaUrgente);
                        gestorTareas.AgregarTareaUrgente(tareaUrgente);
                        break;

                    case "6":
                        gestorTareas.ProcesarTareasUrgentes();
                        break;

                    case "7":
                        gestorTareas.MostrarArbolTareas();
                        break;

                    case "8":
                        gestorTareas.Deshacer();
                        break;

                    case "9":
                        gestorTareas.Rehacer();
                        break;

                    case "10":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("esta Opción es no válida porfavor intenta con otra opcion.");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}







