using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagerApp
{

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








